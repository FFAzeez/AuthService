using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories
{

    public class BaseRepository<TEntity>(AppDbContext dbContext) : IAsyncRepository<TEntity>
        where TEntity : class
    {
        protected readonly AppDbContext _dbContext = dbContext;
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

      
        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Log.Information($"getting of single data query initiated {predicate}");
            var query = _dbSet.AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            var response = await query.FirstOrDefaultAsync();
            Log.Information($"getting of single data query response {response}");
            return response;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
