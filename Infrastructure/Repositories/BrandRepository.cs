using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
	private readonly StoreContext _context;
	public BrandRepository(StoreContext context) : base(context)
	{
		_context = context;
	}
}
