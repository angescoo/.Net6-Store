using API.Dtos;

namespace API.Services;
public interface IUserService
{
    Task<string> RegisterAsync(RegisterDto model);

    Task<UsersDataDto> GetTokenAsync(LoginDto login);

    Task<string> AddRoleAsync(AddRoleDto addRoleDto);

    Task<UsersDataDto> RefreshTokenAsync(string refreshToken);
}