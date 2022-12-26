namespace Core.Entities;

public class User : BaseEntity
{
    public string Names { get; set; }
    public string LastName { get; set; }
    public string MotherLastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    public ICollection<UsersRols> UsersRoles { get; set; }
}
