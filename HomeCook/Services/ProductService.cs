using AutoMapper;
using HomeCook.Data;
using HomeCook.Data.CustomException;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.DTO.Product;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeCook.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(DefaultDbContext context,
        IMapper mapper) : base(context, mapper)
        {
        }
        #region ProductCategory
        public async Task<ProductCategory> AddProductCategory(string categoryName)
        {
            var productCategory = Context.ProductCategories.FirstOrDefault(x => x.Name == categoryName);
            if (productCategory is not null)
            {
                throw new ProductException(ProductException.ProductCategoryAlreadyExist);
            }

            var newCategory = new ProductCategory() { Name = categoryName };
            Context.ProductCategories.Add(newCategory);
            Context.SaveChanges();
            return newCategory;
        }

        public void DeleteProductCategory(string Id)
        {
            var productCategory = Context.ProductCategories.FirstOrDefault(x => x.PublicId == Id);
            if (productCategory is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }
            Context.Remove(productCategory);
            Context.SaveChanges();
        }
        public async Task<ProductCategoryDto> GetProductCategoryDto(string Id)
        {
            var productCategory = Context.ProductCategories.FirstOrDefault(x => x.PublicId == Id);
            if (productCategory is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }

            var productCategoryDto = Mapper.Map<ProductCategoryDto>(productCategory);
            return productCategoryDto;
        }

        public async Task<List<ProductCategoryDto>> GetAllProductCategory()
        {
            var productCategory = Context.ProductCategories.ToList();
            if (productCategory is null)
            {
                throw new ProductException(ProductException.SomethingWentWrong);
            }

            var productCategoryDto = Mapper.Map<List<ProductCategoryDto>>(productCategory);
            return productCategoryDto;
        }
        public async Task<ProductCategoryDto> UpdateProductCategory(ProductCategoryDto newProductCategory)
        {
            var productCategory = Context.ProductCategories.FirstOrDefault(x => x.PublicId == newProductCategory.Id);
            if (productCategory is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }
            productCategory.Name = newProductCategory.Name;
            Context.ProductCategories.Update(productCategory);
            Context.SaveChanges();

            var productCategoryDto = Mapper.Map<ProductCategoryDto>(productCategory);
            return productCategoryDto;
        }

        public ProductCategory FindProductCategory(string publicId)
        {
            var result = Context.ProductCategories.FirstOrDefault(x => x.PublicId == publicId);
            if (result is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }
            return result;
        }
        #endregion

        public async Task<Product> AddProduct(ProductDto newProduct)
        {
            var product = Context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
            if (product is not null)
            {
                throw new ProductException(ProductException.ProductAlreadyExist);
            }

            product = Mapper.Map<Product>(newProduct);

            var productCategory = FindProductCategory(newProduct.CategoryId);
            product.CategoryId = productCategory.Id;

            Context.Products.Add(product);
            Context.SaveChanges();

            return product;
        }

        public void DeleteProduct(string Id)
        {
            var product = Context.Products.FirstOrDefault(x => x.PublicId == Id);
            if (product is null)
            {
                throw new ProductException(ProductException.ProductDoesntExist);
            }
            Context.Remove(product);
            Context.SaveChanges();
        }

        public async Task<ProductDto> GetProduct(string Id)
        {
            var product = Context.Products.Include(c => c.Category).FirstOrDefault(x => x.PublicId == Id);
            if (product is null)
            {
                throw new ProductException(ProductException.ProductDoesntExist);
            }

            var productDto = Mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<List<ProductDto>> GetProductList(string category)
        {
            var products = new List<Product>();
            if (!String.IsNullOrEmpty(category))
            {
                products = Context.Products
                    .Include(c => c.Category)
                    .Where(x => x.Category.Name.ToUpper() == category.ToUpper())
                    .ToList();
            }
            else
            {
                products = Context.Products
                    .Include(c => c.Category)
                    .ToList();
            }

            if (products is null)
            {
                throw new ProductException(ProductException.SomethingWentWrong);
            }

            var productListDto = Mapper.Map<List<ProductDto>>(products);
            return productListDto;
        }
    }
}
