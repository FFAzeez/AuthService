using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AuthService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDbContext"/> class.
        /// </summary>
        /// <param name="options">db options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;
                }
            }

            foreach (var entity in ChangeTracker.Entries<BaseModel>())
            {
                if (entity.State == EntityState.Modified)
                {
                    var audit = new AuditLog();
                    audit.TableName = entity.Entity.GetType().Name;
                    audit.InitiatedDate = DateTime.Now;
                    audit.Action = EntityState.Modified.ToString();
                    audit.RecordId = entity.Property("Id").CurrentValue?.ToString();

                    foreach (var property in entity.OriginalValues.Properties)
                    {
                        var original = entity.OriginalValues[property];
                        var current = entity.CurrentValues[property];

                        if (!Equals(original, current))
                        {
                            audit.AffectedColumns += $"{property.Name}, ";
                            audit.OldValue += $"{original}, ";
                            audit.NewValue += $"{current}, ";
                        }
                    }

                    // Remove trailing commas
                    audit.AffectedColumns = audit.AffectedColumns?.TrimEnd(',', ' ');
                    audit.OldValue = audit.OldValue?.TrimEnd(',', ' ');
                    audit.NewValue = audit.NewValue?.TrimEnd(',', ' ');
                    AuditLogs.Add(audit);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
