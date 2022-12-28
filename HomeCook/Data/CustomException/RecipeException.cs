using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class RecipeException: Exception
    {
        public const string ReicpeDoesntExist = "Reicpe Doesn't Exist",
            CommentDoesntExist = "Comment Doesn't Exist";

        readonly Dictionary<string, string> errors = new Dictionary<string, string>();
        public RecipeException(string errorMessage) : base(String.Format(errorMessage))
        {

        }

        public RecipeException(IdentityError[] identityErrors)
        {
            foreach (var err in identityErrors)
            {
                errors.Add(err.Code, err.Description);
            }
        }
    }
}
