namespace HomeCook.Services.Interfaces
{
    public interface IImageService
    {
        string GetProfileImage(string userId);
        string GetrecipeMainImage(string recipePublicId);
        string GetrecipeMainImage(long recipeId);
        void UpdateProfileImage(IFormFile file, string userId);
        void UpdateRecipeImage(IFormFile file, string recipeId, bool isMainImage = false);
        void UpdateRecipeImage(IFormFile file, long recipeId, bool isMainImage = false);
    }
}