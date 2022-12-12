namespace HomeCook.Services.Interfaces
{
    public interface IImageService
    {
        string GetProfileImage(string userId);
        string GetrecipeMainImage(string recipePublicId);
        string GetRecipeMainImage(long recipeId);
        void UpdateProfileImage(IFormFile file, string userId);
        void AddOrUpdateRecipeImage(IFormFile file, string recipeId, bool isMainImage = false);
        void AddOrUpdateRecipeImage(IFormFile file, long recipeId, bool isMainImage = false);
        bool DeleteRecipeImages(string[] imagesId);
        void AddRecipeImageRange(IFormFile[] files, string recipeId);
    }
}