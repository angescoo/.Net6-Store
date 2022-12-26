using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetExpensiverProducts(int amount);
}
