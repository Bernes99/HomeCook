namespace HomeCook.Services.Interfaces
{
    public interface IImageService
    {
        string GetProfileImage(string userId);
        void UpdateProfileImage(IFormFile file, string userId);
    }
}