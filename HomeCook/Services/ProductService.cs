using AutoMapper;
using HomeCook.Data;
using HomeCook.Data.CustomException;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.DTO.Product;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HomeCook.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(DefaultDbContext context,
        IMapper mapper) : base(context, mapper)
        {
        }
        #region ProductCategory CRUD
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
        public async Task<ProductCategoryDto> GetProductCategory(string Id)
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
        #endregion
    }
}
