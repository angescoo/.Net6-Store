namespace API.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public DateTime CreationDate { get; set; }
    public BrandDto Brand { get; set; }
    public CategoryDto Category { get; set; }
}
