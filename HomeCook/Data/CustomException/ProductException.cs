using HomeCook.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class ProductException : Exception
    {
        public const string ProductCategoryAlreadyExist = "Product Category Already Exist",
            ProductCategoryDoesntExist = "Product Category Doesn't Exist",
            SomethingWentWrong = "Something Went Wrong";

        readonly Dictionary<string, string> errors = new Dictionary<string, string>();
        public ProductException(string errorMessage) : base(String.Format(errorMessage))
        {

        }

        public ProductException(IdentityError[] identityErrors)
        {
            foreach (var err in identityErrors)
            {
                errors.Add(err.Code, err.Description);
            }
        }
    }
}
