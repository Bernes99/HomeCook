using HomeCook.Data.Models;
using HomeCook.DTO.Product;

namespace HomeCook.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> AddCategory(string categoryName);
        void DeleteCategory(string publicId);
        Dictionary<long, string> FindAllCategoriesIds();
        Category FindCategory(string publicId);
        Task<List<CategoryDto>> GetAllCategories();
        Task<CategoryDto> GetCategoryDto(string Id);
        Task<CategoryDto> UpdateCategory(CategoryDto newProductCategory);
    }
}