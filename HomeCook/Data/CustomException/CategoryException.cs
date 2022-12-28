using HomeCook.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class CategoryException : Exception
    {
        public const string CategoryAlreadyExist = "Category Already Exist",
            CategoryDoesntExist = "Category Doesn't Exist",
            SomethingWentWrong = "Something Went Wrong",
            CantAddManyOfTheSameCategories = "you can't add many of the same Categories";

        readonly Dictionary<string, string> errors = new Dictionary<string, string>();
        public CategoryException(string errorMessage) : base(String.Format(errorMessage))
        {

        }

        public CategoryException(IdentityError[] identityErrors)
        {
            foreach (var err in identityErrors)
            {
                errors.Add(err.Code, err.Description);
            }
        }
    }
}
