﻿using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Product;

namespace HomeCook.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProduct(ProductDto newProduct);
        Task<ProductCategory> AddProductCategory(string categoryName);
        Task<List<UserProduct>> UpdateUserProducts(List<AddUserProductDto> model, string userId);
        void DeleteProduct(string Id);
        void DeleteProductCategory(string Id);
        Task<List<UserProduct>> DeleteUserProduct(IdsDto model, string userId);
        Task<List<CategoryDto>> GetAllProductCategories();
        Task<ProductDto> GetProduct(string Id);
        Task<CategoryDto> GetProductCategoryDto(string Id);
        Task<List<ProductDto>> GetProductList(string category);
        Task<List<UserProductDto>> GetUserProductList(string userId);
        Task<Product> UpdateProduct(ProductDto newProduct);
        Task<CategoryDto> UpdateProductCategory(CategoryDto newProductCategory);
        Dictionary<long, string> FindAllPorductIds();
    }
}