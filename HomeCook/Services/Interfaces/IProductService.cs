using HomeCook.Data.Models;
using HomeCook.DTO.Product;

namespace HomeCook.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductCategory> AddProductCategory(string categoryName);
        void DeleteProductCategory(string Id);
        Task<List<ProductCategoryDto>> GetAllProductCategory();
        Task<ProductCategoryDto> GetProductCategory(string Id);
        Task<ProductCategoryDto> UpdateProductCategory(ProductCategoryDto newProductCategory);
    }
}