using HomeCook.Data.Models;
using System.Data;

namespace HomeCook.Data.Seeders
{
    public class ProductCategorySeeder
    {
        protected DefaultDbContext Context { get; }

        public ProductCategorySeeder(DefaultDbContext dbContext)
        {
            Context = dbContext;
        }

        public void Seed()
        {
            if (Context.Database.CanConnect())
            {
                if (!Context.Roles.Any())
                {
                    var productCategories = GetProductCategories();
                    Context.ProductCategories.AddRange(productCategories);
                    Context.SaveChanges();
                }
            }
        }

        private IEnumerable<ProductCategory> GetProductCategories()
        {
            var productCategories = new List<ProductCategory>()
            {
                new ProductCategory()
                {
                    Name = "Vegetables & Greens"
                },
                new ProductCategory()
                {
                    Name = "Mushrooms"
                },
                new ProductCategory()
                {
                    Name = "Fruits"
                },
                new ProductCategory()
                {
                    Name = "Berries"
                },
                new ProductCategory()
                {
                    Name = "Nuts & Seeds"
                },
                new ProductCategory()
                {
                    Name = "Cheeses"
                },
                new ProductCategory()
                {
                    Name = "Dairy & Eggs"
                },
                new ProductCategory()
                {
                    Name = "Meats"
                },
                new ProductCategory()
                {
                    Name = "Poultry"
                },
                new ProductCategory()
                {
                    Name = "Fish"
                },
                new ProductCategory()
                {
                    Name = "Seafood & Seaweed"
                },
                new ProductCategory()
                {
                    Name = "Herbs & Spices"
                },
                new ProductCategory()
                {
                    Name = "Sugar & Sweeteners"
                },
                new ProductCategory()
                {
                    Name = "Seasonings & Spice Blends"
                },
                new ProductCategory()
                {
                    Name = "Baking"
                },
                new ProductCategory()
                {
                    Name = "Pre-Made Doughs & Wrappers"
                },
                new ProductCategory()
                {
                    Name = "Grains & Cereals"
                },
                new ProductCategory()
                {
                    Name = "Legumes"
                },
                new ProductCategory()
                {
                    Name = "Pasta"
                },
                new ProductCategory()
                {
                    Name = "Bread & Salty Snacks"
                },
                new ProductCategory()
                {
                    Name = "Pasta"
                },
                new ProductCategory()
                {
                    Name = "Oils & Fats"
                },
                new ProductCategory()
                {
                    Name = "Dressings & Vinegars"
                },
                new ProductCategory()
                {
                    Name = "Condiments & Relishes"
                },
                new ProductCategory()
                {
                    Name = "Sauces, Spreads & Dips"
                },
                new ProductCategory()
                {
                    Name = "Soups, Stews & Stocks"
                },
                new ProductCategory()
                {
                    Name = "Desserts & Sweet Snacks"
                },
                new ProductCategory()
                {
                    Name = "Wine, Beer & Spirits"
                },
                new ProductCategory()
                {
                    Name = "Beverages"
                },
                new ProductCategory()
                {
                    Name = "Supplements & Powders"
                }

            };
            return productCategories;
        }
    }
}
