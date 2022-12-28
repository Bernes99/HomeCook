using AutoMapper;
using HomeCook.Data;
using HomeCook.Data.CustomException;
using HomeCook.Data.Enums;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Pagination;
using HomeCook.DTO.Product;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeCook.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IUserService _userService;
        public ProductService(DefaultDbContext context,
        IMapper mapper,
        IUserService userService) : base(context, mapper)
        {
            _userService = userService;
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
            Create(newCategory);
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
        public async Task<CategoryDto> GetProductCategoryDto(string Id)
        {
            var productCategory = Context.ProductCategories.FirstOrDefault(x => x.PublicId == Id);
            if (productCategory is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }

            var productCategoryDto = Mapper.Map<CategoryDto>(productCategory);
            return productCategoryDto;
        }

        public async Task<List<CategoryDto>> GetAllProductCategories()
        {
            var productCategory = Context.ProductCategories.ToList();
            if (productCategory is null)
            {
                throw new ProductException(ProductException.SomethingWentWrong);
            }

            var productCategoryDto = Mapper.Map<List<CategoryDto>>(productCategory);
            return productCategoryDto;
        }
        public async Task<CategoryDto> UpdateProductCategory(CategoryDto newProductCategory)
        {
            var productCategory = Context.ProductCategories.FirstOrDefault(x => x.PublicId == newProductCategory.Id);
            if (productCategory is null)
            {
                throw new ProductException(ProductException.ProductCategoryDoesntExist);
            }
            productCategory.Name = newProductCategory.Name;
            Context.ProductCategories.Update(productCategory);
            Context.SaveChanges();

            var productCategoryDto = Mapper.Map<CategoryDto>(productCategory);
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

        public async Task<ProductResponseDto> GetProduct(string Id)
        {
            var product = Context.Products.Include(c => c.Category).FirstOrDefault(x => x.PublicId == Id);
            if (product is null)
            {
                throw new ProductException(ProductException.ProductDoesntExist);
            }

            var productDto = Mapper.Map<ProductResponseDto>(product);
            return productDto;
        }

        public async Task<List<ProductResponseDto>> GetProductList(string category)
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

            var productListDto = Mapper.Map<List<ProductResponseDto>>(products);
            return productListDto;
        }
        public async Task<PaginationResult<ProductResponseDto>> GetProductList(string category, ProductPaginationQuery query)
        {

            if (query is not null && query.SortBy is not null)
            {
                query.SortBy = query.SortBy.ToUpper();
            }

            var products = !String.IsNullOrEmpty(category) ? 
                Context.Products
                    .Include(c => c.Category)
                    .Where(x => x.Category.Name.ToUpper() == category.ToUpper()) :  
                Context.Products
                    .Include(c => c.Category);

            if (products is null)
            {
                throw new ProductException(ProductException.SomethingWentWrong);
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Product, object>>>
                {
                    { nameof(Product.Name).ToUpper(), u => u.Name },
                    { nameof(Product.Calories).ToUpper(), u => u.Calories },
                    { nameof(Product.UnitType).ToUpper(), u => u.UnitType },
                };
                var selectedColumn = columnsSelectors[query.SortBy];

                products = query.SortDirection == SortDirection.ASC ? products.OrderBy(selectedColumn) : products.OrderByDescending(selectedColumn);
            }

            var pagedProducts = products.Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize);

            var productListDto = Mapper.Map<List<ProductResponseDto>>(pagedProducts);

            var productsCount = products.Count();
            var result = new PaginationResult<ProductResponseDto>(productListDto, productsCount, query.PageSize, query.PageNumber);
            return result;
        }

        public async Task<Product> UpdateProduct(ProductDto newProduct)
        {
            var product = Context.Products.FirstOrDefault(x => x.PublicId == newProduct.Id);
            if (product is null)
            {
                throw new ProductException(ProductException.ProductDoesntExist);
            }
            var productCategory = FindProductCategory(newProduct.CategoryId);

            product.CategoryId = productCategory?.Id ?? product.CategoryId;
            product.Name = newProduct.Name;
            product.Calories = newProduct.Calories ?? product.Calories;
            product.UnitType = (int)newProduct.UnitType;

            Update(product);

            return product;
        }
        #region UserProduct
        public async Task<List<UserProduct>> AddOrUpdateUserProducts(List<AddUserProductDto> model, string userId)
        {
            //checking user
            var user = await _userService.FindUserAsyncbyId(userId);
            if (user is null)
            {
                throw new AuthException(AuthException.UserDoesNotExist);
            }
            //checking if product exist
            var productIDs = FindAllPorductIds();
            var productsExist = model.Select(y => y.ProductId).All(productId => productIDs.Values.Contains(productId));
            if (!productsExist)
            {
                throw new ProductException(ProductException.ProductDoesntExist);
            }
            //converting dto to model
            var modelProducts = model.Select(x => x.ProductId).ToArray();
            if (modelProducts.Length != modelProducts.Distinct().Count())
            {
                throw new ProductException(ProductException.CantAddManyOfTheSameProducts);
            }
            foreach (var uProduct in model)
            {
                uProduct.ProductInternalId = productIDs.First(x => x.Value.Equals(uProduct.ProductId)).Key;
            }
            var userProducts = Mapper.Map<List<UserProduct>>(model);
            //checking if one user didnt have product duplicates
            var userProductsBeforeUpdate = Context.UserProducts.Where(x => x.UserId == userId).Select(x => x.Product.Id).ToArray();

            List<UserProduct> addUserProduct = new List<UserProduct>();
            List<UserProduct> updateUserProduct = new List<UserProduct>();
            foreach (var userProduct in userProducts)
            {
                userProduct.UserId = userId;

                if (userProductsBeforeUpdate.Contains(userProduct.ProductId))
                {
                    updateUserProduct.Add(userProduct);
                }
                else
                {
                    addUserProduct.Add(userProduct);
                }
            }
            if (addUserProduct.Count >0)
            {
                Context.UserProducts.AddRange(addUserProduct);
            }
            if (updateUserProduct.Count > 0)
            {
                Context.UserProducts.UpdateRange(updateUserProduct);
            }
            SaveChanges();
            return userProducts;
        }
        public async Task<List<UserProductDto>> GetUserProductList(string userId)
        {
            var user = await _userService.FindUserAsyncbyId(userId);
            if (user is null)
            {
                throw new AuthException(AuthException.UserDoesNotExist);
            }

            var userProducts = Context.UserProducts.Include(x => x.Product).Where(x => x.UserId == userId).ToList();

            var userProductsDto = Mapper.Map<List<UserProductDto>>(userProducts);
            return userProductsDto;

        }
        public async Task<List<UserProduct>> DeleteUserProduct(IdsDto model, string userId)
        {
            //checking user
            var user = await _userService.FindUserAsyncbyId(userId);
            if (user is null)
            {
                throw new AuthException(AuthException.UserDoesNotExist);
            }
           
            var userProducts = Context.UserProducts.Where(x => model.Id.Contains(x.PublicId)).ToList();

            Context.UserProducts.RemoveRange(userProducts);
            SaveChanges();
            return userProducts;
        }
        #endregion

        public Dictionary<long, string> FindAllPorductIds()
        {
            return Context.Products.Select(p => new KeyValuePair<long, string>(p.Id, p.PublicId)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
