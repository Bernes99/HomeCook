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
        [HttpPost("AddProductCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> AddProductCategory(string CategoryName)
        {
            await _productService.AddProductCategory(CategoryName);
            return Ok();
        }
        [HttpPost("UpdateProductCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> UpdateProductCategory([FromBody] ProductCategoryDto newProductCategory)
        {
            await _productService.UpdateProductCategory(newProductCategory);
            return Ok();
        }

        [HttpDelete("DeleteProductCategory/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> DeleteProductCategory(string Id)
        {
            _productService.DeleteProductCategory(Id);
            return Ok();
        }

        [HttpGet("GetProductCategory/{Id}")]
        public async Task<ActionResult> GetProductCategory(string Id)
        {
            var result = await _productService.GetProductCategory(Id);
            return Ok(result);
        }

        [HttpGet("GetProductCategoryList")]
        public async Task<ActionResult> GetProductCategory()
        {
            var result = await _productService.GetAllProductCategory();
            return Ok(result);
        }
        #endregion
    }
}
