using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly StoreContext _context;
    public ProductRepository(StoreContext context) : base(context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Product>> GetExpensiverProducts(int amount)
    {
        return await _context.Products.OrderByDescending(p => p.Price)
            .Take(amount)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
                            .Include(u => u.Brand)
                            .Include(u => u.Category)
                            .ToListAsync();
    }

    public override async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products
                            .Include(p => p.Brand)
                            .Include(p => p.Category)
                            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<(int totalLogs, IEnumerable<Product> logs)> GetAllAsync(int pageIndex, int pageSize, string search)
    {
        var request = _context.Products as IQueryable<Product>;
        if (!string.IsNullOrEmpty(search))
        {
            request = request.Where(p => p.Name.ToLower().Contains(search));
        }

        var totalLogs = await request.CountAsync();

        var logs = await request
                            .Include(u => u.Brand)
                            .Include(u => u.Category)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        return (totalLogs, logs);
    }
}
