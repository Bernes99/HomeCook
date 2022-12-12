using HomeCook.Data.Models;
using HomeCook.DTO.Recipe;
using HomeCook.DTO.SearchEngine;
using Microsoft.AspNetCore.Mvc;

namespace HomeCook.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<Recipe> AddRecipe(IFormFile? mainPicture, IFormFile?[] pictures, [FromBody] AddRecipeDto model);
        Task<RecipeDetailsDto> GetRecipeDetails(string recipePublicId);
        Task<RecipeDetailsDto> GetRecipeDetails(long recipeInternalId);
        Task<List<LuceneRecipeSearchResultItem>> GetRecipesList(string searchString, RecipeFilters filters);
        void InitialIndexes();
        Task<RecipeDetailsDto> UpdateRecipe(AddRecipeDto model, string recipePublicId);
    }
}