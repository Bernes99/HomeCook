using HomeCook.Data.Models;
using HomeCook.DTO.Recipe;
using HomeCook.DTO.SearchEngine;
using Microsoft.AspNetCore.Mvc;

namespace HomeCook.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<CommentResponseDto> AddComment(string recipeId, string userId, string text);
        Task<Recipe> AddRecipe(IFormFile? mainPicture, IFormFile?[] pictures, [FromBody] AddRecipeDto model);
        void DeleteComment(string recipeId, string commentId, string userId);
        Task<bool> DeleteRecipe(string recipePublicId, string userId);
        Comment FindCommentByPublicId(string commentId);
        Recipe FindRecipeByPublicId(string recipePublicId);
        Task<List<CommentResponseDto>> GetComments(string recipeId);
        Task<RecipeDetailsDto> GetRecipeDetails(string recipePublicId);
        Task<RecipeDetailsDto> GetRecipeDetails(long recipeInternalId);
        Task<List<LuceneRecipeSearchResultItem>> GetRecipesList(string searchString, RecipeFilters filters);
        Task<List<RecipeDetailsDto>> GetUserRecipes(string userId);
        void InitialIndexes();
        Task<RecipeDetailsDto> UpdateRecipe(AddRecipeDto model, string recipePublicId);
    }
}