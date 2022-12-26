namespace Core.Entities;

public class UsersRols : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public User User { get; set; }
    public Role Role { get; set; }
}
