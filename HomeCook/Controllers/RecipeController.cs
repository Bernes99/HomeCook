using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Product;
using HomeCook.DTO.Recipe;
using HomeCook.DTO.SearchEngine;
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
        private IImageService _imageService;

        public RecipeController(ICategoryService categoryService, IRecipeService recipeService, IImageService imageService)
        {
            _categoryService = categoryService;
            _recipeService = recipeService;
            _imageService = imageService;
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
        [HttpPut("Category/UpdateCategory")]
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

        [HttpGet("Category/GetRecipeCategory/{Id}")]
        public async Task<ActionResult> GetCategory([FromRoute] string Id)
        {
            var result = await _categoryService.GetCategoryDto(Id);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("Category/GetCategoryList")]
        public async Task<ActionResult> GetProductCategory()
        {
            var result = await _categoryService.GetAllCategories();
            return Ok(result);
        }
        #endregion
        [HttpPost("AddRecipe")]
        public async Task<ActionResult> AddRecipe( IFormFile? mainPicture,  IFormFile?[] pictures, [FromForm][ModelBinder(BinderType = typeof(FormDataJsonModelBinder))] AddRecipeDto model)
        {
            if (!IsSelfOrAdmin(model.AuthorId))
            {
                return Unauthorized();
            }
            var test = await _recipeService.AddRecipe(mainPicture, pictures, model);

            return Ok();
        }

        [AllowAnonymous] 
        [HttpGet("{Id}")]
        public async Task<ActionResult> GetRecipe([FromRoute] string Id)
        {

            var result = await _recipeService.GetRecipeDetails(Id);

            return Ok(result);
        }
        [AllowAnonymous]//TODO tylko do testow
        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateRecipe([FromRoute]string Id,[FromBody] AddRecipeDto model)
        {

            var result = await _recipeService.UpdateRecipe(model, Id);

            return Ok(result);
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteRecipe([FromRoute] string Id)
        {
            var recipe = _recipeService.FindRecipeByPublicId(Id);

            if (!IsSelfOrAdmin(recipe.AuthorId))
            {
                return Unauthorized();
            }

            var result = await _recipeService.DeleteRecipe(Id, User.Claims.FirstOrDefault(x => x.Type == "PublicId")?.Value);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("GetList")]
        public async Task<ActionResult> GetRecipesList([FromQuery] string? searchPhrase, [FromBody] RecipeFilters filters)
        {

            var result = await _recipeService.GetRecipesList(searchPhrase, filters);

            return Ok(result);
        }

        [HttpPut("{Id}/UpdateMainImage")]
        public async Task<ActionResult> UpdateMainImage([FromRoute] string Id, [FromForm] IFormFile file)
        {
            var recipe = _recipeService.FindRecipeByPublicId(Id);

            if (!IsSelfOrAdmin(recipe.AuthorId))
            {
                return Unauthorized();
            }
            _imageService.AddOrUpdateRecipeImage(file, Id, true);

            return Ok();
        }

        [HttpPost("{Id}/AddImages")]
        public async Task<ActionResult> AddRecipeImages([FromRoute] string Id, [FromForm] IFormFile[] files)
        {
            var recipe = _recipeService.FindRecipeByPublicId(Id);

            if (!IsSelfOrAdmin(recipe.AuthorId))
            {
                return Unauthorized();
            }
            _imageService.AddRecipeImageRange(files, Id);

            return Ok();
        }

        [HttpDelete("{Id}/DeleteImages")]
        public async Task<ActionResult> DeleteRecipeImages([FromRoute] string Id, [FromBody] string[] imageIds)
        {
            var recipe = _recipeService.FindRecipeByPublicId(Id);

            if (!IsSelfOrAdmin(recipe.AuthorId))
            {
                return Unauthorized();
            }
            _imageService.DeleteRecipeImages(imageIds);

            return Ok();
        }

        [HttpPost("{Id}/AddComment")]
        public async Task<ActionResult> AddComment([FromRoute] string Id, [FromQuery]string text)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "PublicId")?.Value;
            if (!IsSelfOrAdmin(userId))
            {
                return Unauthorized();
            }
            await _recipeService.AddComment(Id, userId, text);

            return Ok();
        }
        [AllowAnonymous]
        [HttpGet("{Id}/GetComments")]
        public async Task<ActionResult> GetComments([FromRoute] string Id)
        {
            var comments = await _recipeService.GetComments(Id);

            return Ok(comments);
        }

        [HttpDelete("{Id}/DeleteComment/{commentId}")]
        public async Task<ActionResult> GetComments([FromRoute] string Id, [FromRoute]string commentId)
        {
            var comment = _recipeService.FindCommentByPublicId(commentId);
            var userId = User.Claims.FirstOrDefault(x => x.Type == "PublicId")?.Value;
            if (!IsSelfOrAdmin(comment.AuthorId))
            {
                return Unauthorized();
            }
            _recipeService.DeleteComment(Id,commentId,userId);

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
