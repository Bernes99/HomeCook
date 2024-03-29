﻿using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class RecipeException: Exception
    {
        public const string RecipeDoesntExist = "Recipe Doesn't Exist",
            CommentDoesntExist = "Comment Doesn't Exist";
        public RecipeException(string errorMessage) : base(String.Format(errorMessage))
        {

        }
    }
}
