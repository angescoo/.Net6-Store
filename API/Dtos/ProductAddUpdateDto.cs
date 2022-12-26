using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class ProductAddUpdateDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreationDate { get; set; }
    public int BrandId { get; set; }
    public int CategoryId { get; set; }

}
