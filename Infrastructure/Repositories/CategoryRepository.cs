using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly StoreContext _context;
        public CategoryRepository(StoreContext context) : base(context)
        {
            _context = context;
        }
    }
}
