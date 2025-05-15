using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.Success
                ? Ok(ApiResponse<string>.SuccessResponse(result.Token!, "Register successful"))
                : BadRequest(ApiResponse<string>.FailureResponse(string.Join("; ", result.Errors!)));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result.Success
                ? Ok(ApiResponse<string>.SuccessResponse(result.Token!, "Login successful"))
                : BadRequest(ApiResponse<string>.FailureResponse(string.Join("; ", result.Errors!)));
        }
    }
}
