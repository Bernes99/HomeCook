using HomeCook.Data.Models;
using HomeCook.DTO.Product;

namespace HomeCook.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProduct(ProductDto newProduct);
        Task<ProductCategory> AddProductCategory(string categoryName);
        Task<List<UserProduct>> AddUserProducts(List<AddUserProductDto> model, string userId);
        void DeleteProduct(string Id);
        void DeleteProductCategory(string Id);
        Task<List<ProductCategoryDto>> GetAllProductCategory();
        Task<ProductDto> GetProduct(string Id);
        Task<ProductCategoryDto> GetProductCategoryDto(string Id);
        Task<List<ProductDto>> GetProductList(string category);
        Task<List<UserProductDto>> GetUserProductList(string userId);
        Task<Product> UpdateProduct(ProductDto newProduct);
        Task<ProductCategoryDto> UpdateProductCategory(ProductCategoryDto newProductCategory);
    }
}