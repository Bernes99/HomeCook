namespace HomeCook.Services.Interfaces
{
    public interface IImageService
    {
        string GetProfileImage(string userId);
        void UpdateProfileImage(IFormFile file, string userId);
        void UpdateRecipeImage(IFormFile file, string recipeId, bool isMainImage = false);
        void UpdateRecipeImage(IFormFile file, long recipeId, bool isMainImage = false);
    }
}