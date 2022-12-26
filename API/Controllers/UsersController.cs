using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult> SignUpAsync([FromBody] RegisterDto registerDto)
    {
        var result = await _userService.RegisterAsync(registerDto);
        return Ok(result);
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetTokenAsync(LoginDto login)
    {
        var result = await _userService.GetTokenAsync(login);
        SetRefreshTokenInCookie(result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("addrole")]
    public async Task<IActionResult> AddRoleAsync(AddRoleDto addRoleDto)
    {
        var result = await _userService.AddRoleAsync(addRoleDto);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _userService.RefreshTokenAsync(refreshToken);

        if (!string.IsNullOrEmpty(response.RefreshToken))
            SetRefreshTokenInCookie(response.RefreshToken);

        return Ok(response);
    }

    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
