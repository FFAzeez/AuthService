using AuthService.API.Filters;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthServices authServices) : ControllerBase
    {
        /// <summary>
        /// Register a new User.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register")]
        [ServiceFilter(typeof(RegisterUserValidationFilter))]
        public async Task<IActionResult>  Registration(RegisterDto request)
        {
            var result = await authServices.RegisterUser(request);
            if (result.IsSuccessful)
                return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// Log in Existing User.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        [ServiceFilter(typeof(LoginValidationFilter))]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            Log.Information($"login request initiated");
            var result = await authServices.Login(request);
            if(result.IsSuccessful)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
