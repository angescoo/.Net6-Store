using Core.Entities;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!context.Brands.Any())
            {
                using (var readerBrands = new StreamReader(path + @"/Data/Csvs/brands.csv"))
                {
                    using (var brandsCsv = new CsvReader(readerBrands, CultureInfo.InvariantCulture))
                    {
                        var brands = brandsCsv.GetRecords<Brand>();

                        foreach (var item in brands)
                        {
                            context.Brands.Add(new Brand
                            {
                                Name = item.Name
                            });
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }

            if (!context.Categories.Any())
            {
                using (var readerCategories = new StreamReader(path + @"/Data/Csvs/categories.csv"))
                {
                    using (var categoriesCsv = new CsvReader(readerCategories, CultureInfo.InvariantCulture))
                    {
                        var categories = categoriesCsv.GetRecords<Category>();

                        foreach (var item in categories)
                        {
                            context.Categories.Add(new Category
                            {
                                Name = item.Name
                            });
                        }
                        await context.SaveChangesAsync();
                    }
                }
            }

            if (!context.Products.Any())
            {
                using (var readerProducts = new StreamReader(path + @"/Data/Csvs/products.csv"))
                {
                    using (var productsCsv = new CsvReader(readerProducts, CultureInfo.InvariantCulture))
                    {
                        var productsList = productsCsv.GetRecords<Product>();

                        foreach (var item in productsList)
                        {
                            context.Products.Add(new Product
                            {
                                Name = item.Name,
                                Price = item.Price,
                                CreationDate = item.CreationDate,
                                BrandId = item.BrandId,
                                CategoryId = item.CategoryId
                            });
                        }
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<StoreContextSeed>();
            logger.LogError(ex.Message);
        }
    }

    public static async Task SeedRolesAsync(StoreContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>()
                {
                    new Role { Id = 1, Name = "Administrator" },
                    new Role { Id = 1, Name = "Manager" },
                    new Role { Id = 1, Name = "Employee" },
                };

                foreach (var item in roles)
                {
                    context.Roles.Add(new Role { Name = item.Name });
                }

                //context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<StoreContextSeed>();
            logger.LogError(ex.Message);
        }
    }
}
