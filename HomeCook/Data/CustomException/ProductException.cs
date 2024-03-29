﻿using HomeCook.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class ProductException : Exception
    {
        public const string ProductCategoryAlreadyExist = "Product Category Already Exist",
            ProductCategoryDoesntExist = "Product Category Doesn't Exist",
            SomethingWentWrong = "Something Went Wrong",
            ProductAlreadyExist = "Product Already Exist",
            ProductDoesntExist = "Product Doesn't Exist",
            UserAlreadyHaveThisProduct = "User Already Have This Product",
            CantAddManyOfTheSameProducts = "you can't add many of the same products";

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
