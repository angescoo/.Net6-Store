using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterDto
{
    [Required]
    public string Names { get; set; }
    [Required]
    public string LastName { get; set; }
    public string MotherLastName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
