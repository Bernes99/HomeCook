using HomeCook.Data.Models;
using HomeCook.DTO.Product;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCook.Controllers
{
    [Route("api/Product")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : Controller
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        #region ProductCategory CRUD
        [HttpPost("Category/AddProductCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> AddProductCategory(string CategoryName)
        {
            await _productService.AddProductCategory(CategoryName);
            return Ok();
        }
        [HttpPost("Category/UpdateProductCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> UpdateProductCategory([FromBody] CategoryDto newProductCategory)
        {
            await _productService.UpdateProductCategory(newProductCategory);
            return Ok();
        }

        [HttpDelete("Category/DeleteProductCategory/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> DeleteProductCategory([FromRoute] string Id)
        {
            _productService.DeleteProductCategory(Id);
            return Ok();
        }

        [HttpGet("Category/GetProductCategory/{Id}")]
        public async Task<ActionResult> GetProductCategory([FromRoute] string Id)
        {
            var result = await _productService.GetProductCategoryDto(Id);
            return Ok(result);
        }

        [HttpGet("Category/GetProductCategoryList")]
        public async Task<ActionResult> GetProductCategory()
        {
            var result = await _productService.GetAllProductCategories();
            return Ok(result);
        }
        #endregion

        [HttpPost("AddProduct")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> AddProduct([FromBody] ProductDto newProduct)
        {
            await _productService.AddProduct(newProduct);
            return Ok();
        }

        [HttpDelete("DeleteProduct/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct([FromRoute] string Id)
        {
            if (String.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            _productService.DeleteProduct(Id);
            return Ok();
        }

        [HttpPost("UpdateProduct")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> UpdateProduct([FromBody] ProductDto newProduct)
        {
            await _productService.UpdateProduct(newProduct);
            return Ok();
        }

        [HttpGet("GetProduct/{Id}")]
        public async Task<ActionResult> GetProduct([FromRoute] string Id)
        {
            if (String.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            var result = await _productService.GetProduct(Id);
            return Ok(result);
        }

        [HttpGet("GetProductList")]
        public async Task<ActionResult> GetProductList([FromQuery] string? category)
        {
            var result = await _productService.GetProductList(category);
            return Ok(result);
        }
    }
}
