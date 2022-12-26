using API.Dtos;
using API.Helpers;
using API.Services;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    public readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt, IPasswordHasher<User> passwordHasher)
    {

        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User
        {
            Names = registerDto.Names,
            LastName = registerDto.LastName,
            MotherLastName = registerDto.MotherLastName,
            Email = registerDto.Email,
            UserName = registerDto.UserName
        };

        user.Password = _passwordHasher.HashPassword(user, registerDto.Password);

        var userExist = _unitOfWork.Users
                            .Find(u => u.UserName.ToLower().Equals(registerDto.UserName))
                            .FirstOrDefault();

        if (userExist == null)
        {
            var defaultRole = _unitOfWork.Roles
                              .Find(u => u.Name.Equals(Auth.DEFAULT_ROLE.ToString()))
                              .First();

            try
            {
                user.Roles.Add(defaultRole);
                _unitOfWork.Users.Add(user);
                await _unitOfWork.SaveAsync();

                return $"The user {registerDto.UserName} has been successfully registered";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        else
        {
            return $"The user {registerDto.UserName} already exists";
        }
    }

    public async Task<UsersDataDto> GetTokenAsync(LoginDto login)
    {
        UsersDataDto userDataDto = new UsersDataDto();
        var user = await _unitOfWork.Users
                                    .GetByUsernameAsync(login.UserName);

        if(user == null)
        {
            userDataDto.IsAuthenticated = false;
            userDataDto.Message = $"There is no user with username {login.UserName}";
            return userDataDto;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

        if(result.Equals(PasswordVerificationResult.Success))
        {
            userDataDto.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(user);
            userDataDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            userDataDto.Email = user.Email;
            userDataDto.UserName = user.UserName;
            userDataDto.Roles = user.Roles
                                .Select(x => x.Name)
                                .ToList();

            if (user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();

                userDataDto.RefreshToken = activeRefreshToken.Token;
                userDataDto.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = CreateRefreshToken();
                userDataDto.RefreshToken = refreshToken.Token;
                userDataDto.RefreshTokenExpiration = refreshToken.Expires;
                user.RefreshTokens.Add(refreshToken);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }

            return userDataDto;
        }
        userDataDto.IsAuthenticated = false;
        userDataDto.Message = $"Incorrect credentials for the user {user.UserName}";
        return userDataDto;
    }

    private JwtSecurityToken CreateJwtToken(User user)
    {
        var roles = user.Roles;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Name));
        }

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString()),
        }
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }

    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new Byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }
    }

    public async Task<string> AddRoleAsync(AddRoleDto addRoleDto)
    {
        var user = await _unitOfWork.Users
                                    .GetByUsernameAsync(addRoleDto.Username);

        if(user == null)
        {
            return $"There is no users with username: '{addRoleDto.Username}'";
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, addRoleDto.Password);

        if(result.Equals(PasswordVerificationResult.Success))
        {
            var roleExist = _unitOfWork.Roles
                                    .Find(u => u.Name.ToLower().Equals(addRoleDto.Role.ToLower()))
                                    .FirstOrDefault();

            if (roleExist != null)
            {
                bool userHasRole = user.Roles.Any(u => u.Id.Equals(roleExist.Id));

                if(!userHasRole)
                {
                    user.Roles.Add(roleExist);
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveAsync();
                }
                return $"Role '{addRoleDto.Role}' has been added to account '{addRoleDto.Username}' successfuly";
            }

            return $"Role '{addRoleDto.Role} not found'";
        }
        return $"Incorrect credentials for user {addRoleDto.Username}";
    }

    public async Task<UsersDataDto> RefreshTokenAsync(string refreshToken)
    {
        var userDataDto = new UsersDataDto();
        var user = await _unitOfWork.Users
                                .GetByRefreshTokenAsync(refreshToken);

        if(user == null)
        {
            userDataDto.IsAuthenticated = false;
            userDataDto.Message = $"The token don't belong to any user";
            return userDataDto;
        }

        var refreshTokenBd = user.RefreshTokens.Single(x => x.Token.Equals(refreshToken));

        if (!refreshTokenBd.IsActive)
        {
            userDataDto.IsAuthenticated = false;
            userDataDto.Message = $"The token isn't active";
            return userDataDto;
        }
        // revocamos el refresh token actual
        refreshTokenBd.Revoked = DateTime.UtcNow;

        // generamos un nuevo refresh token y lo guardamos en la db
        var newRefreshToken = CreateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveAsync();

        // Generamos un nuevo Json web token
        userDataDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(user);
        userDataDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        userDataDto.Email = user.Email;
        userDataDto.UserName = user.UserName;
        userDataDto.Roles = user.Roles
                                    .Select(u => u.Name)
                                    .ToList();
        userDataDto.RefreshToken = newRefreshToken.Token;
        userDataDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return userDataDto;

    }
}
