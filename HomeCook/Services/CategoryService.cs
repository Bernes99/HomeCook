using AutoMapper;
using HomeCook.Data;
using HomeCook.Data.CustomException;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Product;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HomeCook.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(DefaultDbContext context,
        IMapper mapper,
        IUserService userService) : base(context, mapper)
        {
        }
        #region Category
        public async Task<Category> AddCategory(string categoryName)
        {
            var productCategory = Context.Categories.FirstOrDefault(x => x.Name == categoryName);
            if (productCategory is not null)
            {
                throw new CategoryException(CategoryException.CategoryAlreadyExist);
            }

            var newCategory = new Category() { Name = categoryName };
            Create(newCategory);
            return newCategory;
        }

        public void DeleteCategory(string publicId)
        {
            var productCategory = FindCategory(publicId);
            Remove(productCategory);
        }
        public async Task<CategoryDto> GetCategoryDto(string Id)
        {
            var recipeCategory = Context.RecipeCategories.FirstOrDefault(x => x.PublicId == Id);
            if (recipeCategory is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }

            var productCategoryDto = Mapper.Map<CategoryDto>(recipeCategory);
            return productCategoryDto;
        }

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var productCategory = Context.Categories.ToList();
            if (productCategory is null)
            {
                throw new ProductException(ProductException.SomethingWentWrong);
            }

            var productCategoryDto = Mapper.Map<List<CategoryDto>>(productCategory);
            return productCategoryDto;
        }
        public async Task<CategoryDto> UpdateCategory(CategoryDto newProductCategory)
        {
            var productCategory = FindCategory(newProductCategory.Id);

            productCategory.Name = newProductCategory.Name;
            Update(productCategory);

            var productCategoryDto = Mapper.Map<CategoryDto>(productCategory);
            return productCategoryDto;
        }

        public Category FindCategory(string publicId)
        {
            var result = Context.Categories.FirstOrDefault(x => x.PublicId == publicId);
            if (result is null)
            {
                throw new CategoryException(CategoryException.CategoryDoesntExist);
            }
            return result;
        }
        #endregion

        public Dictionary<long, string> FindAllCategoriesIds()
        {
            return Context.Categories.Select(p => new KeyValuePair<long, string>(p.Id, p.PublicId)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
