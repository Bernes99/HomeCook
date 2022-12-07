﻿using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Product;
using HomeCook.DTO.Recipe;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeCook.Controllers
{
    [Route("api/Recipe")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RecipeController : Controller
    {
        private ICategoryService _categoryService;
        private IRecipeService _recipeService;

        public RecipeController(ICategoryService categoryService, IRecipeService recipeService)
        {
            _categoryService = categoryService;
            _recipeService = recipeService;
        }
        #region Category CRUD
        [HttpPost("Category/AddCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> AddProductCategory(string CategoryName)
        {
            await _categoryService.AddCategory(CategoryName);
            return Ok();
        }
        [HttpPost("Category/UpdateCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> UpdateProductCategory([FromBody] CategoryDto newProductCategory)
        {
            await _categoryService.UpdateCategory(newProductCategory);
            return Ok();
        }

        [HttpDelete("Category/DeleteCategory/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory([FromRoute] string Id)
        {
            _categoryService.DeleteCategory(Id);
            return Ok();
        }

        [HttpGet("Category/GetProductCategory/{Id}")]
        public async Task<ActionResult> GetCategory([FromRoute] string Id)
        {
            var result = await _categoryService.GetCategoryDto(Id);
            return Ok(result);
        }

        [HttpGet("Category/GetCategoryList")]
        public async Task<ActionResult> GetProductCategory()
        {
            var result = await _categoryService.GetAllCategories();
            return Ok(result);
        }
        #endregion
        [AllowAnonymous]
        [HttpPost("AddRecipe")]
        public async Task<ActionResult> AddRecipe( IFormFile? mainPicture,  IFormFile?[] pictures, [FromForm][ModelBinder(BinderType = typeof(FormDataJsonModelBinder))] AddRecipeDto model)
        {
            var form = Request.Form;
            //if (!IsSelfOrAdmin(model.AuthorId))
            //{
            //    return Unauthorized();
            //}
            var test = await _recipeService.AddRecipe(mainPicture, pictures, model);

            return Ok();
        }

        private bool IsSelfOrAdmin(string uesrId)
        {
            if (!User.IsInRole("Admin"))
            {
                var requestUserId = User.Claims.FirstOrDefault(x => x.Type == "PublicId")?.Value;
                if (requestUserId != uesrId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}