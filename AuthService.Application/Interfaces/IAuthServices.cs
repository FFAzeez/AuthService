

using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;

namespace AuthService.Application.Interfaces
{
    public interface IAuthServices
    {
        Task<Response> RegisterUser(RegisterDto register);
        Task<Response<UserResponseDto>> Login(LoginRequestDto userRequest);

    }
}