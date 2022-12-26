using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly StoreContext _context;
    private IProductRepository _productRepository;
    private IBrandRepository _brandRepository;
    private ICategoryRepository _categoryRepository;
    private IRoleRepository _roleRepository;
    private IUserRepository _userRepository;

    public UnitOfWork(StoreContext context)
    {
        _context = context;
    }
    public IProductRepository Products
    {
        get
        {
            if (_productRepository == null)
            {
                _productRepository = new ProductRepository(_context);
            }
            return _productRepository;
        }
    }

    public IBrandRepository Brands
    {
        get
        {
            if (_brandRepository == null)
            {
                _brandRepository = new BrandRepository(_context);
            }
            return _brandRepository;
        }
    }

    public ICategoryRepository Categories
    {
        get
        {
            if (_categoryRepository == null)
            {
                _categoryRepository = new CategoryRepository(_context);
            }
            return _categoryRepository;
        }
    }

    public IRoleRepository Roles
    {
        get
        {
            if (_roleRepository == null)
            {
                _roleRepository = new RoleRepository(_context);
            }
            return _roleRepository;
        }
    }

    public IUserRepository Users
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository = new UserRepository(_context);
            }
            return _userRepository;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
