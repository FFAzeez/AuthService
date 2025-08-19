using AuthService.Application.Common;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using Serilog;

namespace AuthService.Application.Services
{
    public class AuthServices(IAsyncRepository<User> userRepository, IJwtTokenGenerator jwt) : IAuthServices
    {
        /// <summary>
        ///  Register a new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public async Task<Response> RegisterUser(RegisterDto register)
        {
            var response = new Response();
            try
            {
                Log.Information($"Registration request {register}");
                var user = new User()
                {
                    UserName = register.Email,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                     Email = register.Email,
                     Address = register.Address,
                      PhoneNumber = register.PhoneNumber,
                };
                user.UserName = register.Email;
                user.Password = register.Password.HashPassword();

                var existing = await userRepository.GetSingleAsync(_=>_.Email.Equals(register.Email));
                if (existing != null) throw new CustomException(Constants.alreadyExist);

                await userRepository.AddAsync(user);
                response.IsSuccessful = true;
                response.Message = Constants.success;
                Log.Information($"registration response {response}");
            }
            catch (CustomException ex)
            {
                response.IsSuccessful = false;
                response.Message = ex.Message;
                Log.Information($"Registration error {ex.Message}");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Message = Constants.notCompleted;
                Log.Information($"Registration error {ex.Message}");
            }

            return response;
        }
        
        /// <summary>
        /// Login method and also generate jwt token
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public async Task<Response<UserResponseDto>> Login(LoginRequestDto userRequest)
        {
            var response = new Response<UserResponseDto>();
            try
            {
                Log.Information($"login request {userRequest}");
                var user = await userRepository.GetSingleAsync(_ => _.Email == userRequest.Email);
                if (user == null)
                    throw new CustomException(Constants.inValid);
                if(!userRequest.Password.VerifyPassword(user.Password))
                    throw new CustomException(Constants.inValid);

                var token = jwt.GenerateToken(user);
                response.IsSuccessful = true;
                response.Message = Constants.login;
                response.Data = new UserResponseDto()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FirstName +" "+user.LastName,
                    UserToken = token,
                };
                Log.Information($"login response {response}");
            }
            catch (CustomException ex)
            {
                response.IsSuccessful = false;
                response.Message = ex.Message;
                Log.Information($"Login error {ex.Message}");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Message = Constants.notCompleted;
                Log.Information($"login error {ex.Message}");
            }
            return response;
        }
    }
}
