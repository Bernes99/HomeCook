using HomeCook.Data.Models;
using HomeCook.DTO.Product;

namespace HomeCook.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProduct(ProductDto newProduct);
        Task<ProductCategory> AddProductCategory(string categoryName);
        void DeleteProduct(string Id);
        void DeleteProductCategory(string Id);
        Task<List<ProductCategoryDto>> GetAllProductCategory();
        Task<ProductDto> GetProduct(string Id);
        Task<ProductCategoryDto> GetProductCategoryDto(string Id);
        Task<List<ProductDto>> GetProductList(string category);
        Task<Product> UpdateProduct(ProductDto newProduct);
        Task<ProductCategoryDto> UpdateProductCategory(ProductCategoryDto newProductCategory);
    }
}