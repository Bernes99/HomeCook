using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Pagination;
using HomeCook.DTO.Product;

namespace HomeCook.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProduct(ProductDto newProduct);
        Task<ProductCategory> AddProductCategory(string categoryName);
        Task<List<UserProduct>> AddOrUpdateUserProducts(List<AddUserProductDto> model, string userId);
        void DeleteProduct(string Id, string UserId);
        void DeleteProductCategory(string Id);
        Task<List<UserProduct>> DeleteUserProduct(IdsDto model, string userId);
        Task<List<CategoryDto>> GetAllProductCategories();
        Task<ProductResponseDto> GetProduct(string Id);
        Task<CategoryDto> GetProductCategoryDto(string Id);
        Task<PaginationResult<ProductResponseDto>> GetProductList(string category, ProductPaginationQuery query);
        Task<List<UserProductDto>> GetUserProductList(string userId);
        Task<Product> UpdateProduct(ProductDto newProduct);
        Task<CategoryDto> UpdateProductCategory(CategoryDto newProductCategory);
        Dictionary<long, string> FindAllPorductIds();
        Task<List<ProductResponseDto>> GetProductList(string category);
    }
}