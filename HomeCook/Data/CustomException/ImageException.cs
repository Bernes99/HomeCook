using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class ImageException : Exception
    {
        public const string ProfileImageError = "Profile Image Error",
            UserhasNoProfileImage = "User has no profile picture",
            RecipeImageError = "Recipe Image Error",
            RecipeHasNoMainImage = "Recipe has no main Image ";

        readonly Dictionary<string, string> errors = new Dictionary<string, string>();
        public ImageException(string errorMessage) : base(String.Format(errorMessage))
        {

        }

        public ImageException(IdentityError[] identityErrors)
        {
            foreach (var err in identityErrors)
            {
                errors.Add(err.Code, err.Description);
            }
        }
    }
}
