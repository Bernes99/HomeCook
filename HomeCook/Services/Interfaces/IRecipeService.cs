using HomeCook.Data.Models;
using HomeCook.DTO.Recipe;
using Microsoft.AspNetCore.Mvc;

namespace HomeCook.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> AddRecipe(IFormFile? mainPicture, IFormFile?[] pictures, [FromBody] AddRecipeDto model);
        Task<RecipeDetailsDto> GetRecipeDetails(string recipePublicId);
    }
}