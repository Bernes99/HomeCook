﻿using AutoMapper;
using HomeCook.Data;
using HomeCook.Data.CustomException;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeCook.Services
{
    public class ImageService : BaseService, IImageService
    {
        private UserManager<AppUser> _userManager;
        public ImageService(DefaultDbContext context, IMapper mapper, UserManager<AppUser> userManager) : base(context, mapper)
        {
            _userManager = userManager;

        }


        public void UpdateProfileImage(IFormFile file, string userId)
        {
            var profileImage = Context.ProfileImages.FirstOrDefault(x => x.UserId == userId);

            if (file is not null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    // Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        try
                        {
                            var newPhoto = new ProfileImage()
                            {
                                Value = memoryStream.ToArray(),
                                Name = file.FileName,
                                UserId = userId,
                            };
                            if (profileImage is not null)
                            {
                                Context.ProfileImages.Remove(profileImage);
                            }
                            Context.ProfileImages.Add(newPhoto);
                            Context.SaveChanges();
                        }
                        catch (Exception)
                        {
                            throw new ImageException(ImageException.ProfileImageError);
                        }
                    }
                }
            }
        }

        public string GetProfileImage(string userId)
        {
            var profileImage = Context.ProfileImages.FirstOrDefault(x => x.UserId == userId);

            if (profileImage is null)
            {
                throw new ImageException(ImageException.UserhasNoProfileImage);
            }
            if (profileImage.Value is null)
            {
                if (profileImage.Path is null)
                {
                    throw new ImageException(ImageException.UserhasNoProfileImage);
                }
                return profileImage.Path;
            }
            return string.Format("data:image/png;base64, {0}", Convert.ToBase64String(profileImage.Value));
        }

        public void AddOrUpdateRecipeImage(IFormFile file, string recipeId, bool isMainImage = false)
        {
            var recipe = Context.Recipes.FirstOrDefault(x => x.PublicId == recipeId);
            if (recipe is null)
            {
                throw new RecipeException(RecipeException.RecipeDoesntExist);
            }
            AddOrUpdateRecipeImage(file, recipe.Id, isMainImage);
        }
        public void AddRecipeImageRange(IFormFile[] files, string recipeId)
        {
            foreach (var file in files)
            {
                AddOrUpdateRecipeImage(file, recipeId);
            }
        }
        public bool DeleteRecipeImages(string[] imageIds)
        {
            var images = Context.RecipeImages.Where(x => imageIds.Contains(x.PublicId));
            Context.RemoveRange(images);
            SaveChanges();
            return true;
        }
        public void AddOrUpdateRecipeImage(IFormFile file, long recipeId, bool isMainImage = false)
        {
            
            var recipeImage = Context.RecipeImages.FirstOrDefault(x => x.RecipeId == recipeId);

            if (file is not null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    // Upload the file if less than 10 MB
                    if (memoryStream.Length < 10485760)
                    {
                        try
                        {
                            var newPhoto = new RecipeImage()
                            {
                                Value = memoryStream.ToArray(),
                                Name = file.FileName,
                                RecipeId = recipeId,
                                MainPicture = isMainImage
                            };
                            if (recipeImage is not null && isMainImage)
                            {
                                Context.RecipeImages.Remove(recipeImage);
                            }
                            Context.RecipeImages.Add(newPhoto);
                            SaveChanges();
                        }
                        catch (Exception)
                        {
                            throw new ImageException(ImageException.RecipeImageError);
                        }
                    }
                }
            }
        }
        public string GetrecipeMainImage(string recipePublicId)
        {
            var recipe = Context.Recipes.FirstOrDefault(x => x.PublicId == recipePublicId);
            if (recipe == null)
            {
                throw new RecipeException(RecipeException.RecipeDoesntExist); 
            }
            return GetRecipeMainImage(recipe.Id);
        }
        public string GetRecipeMainImage(long recipeId)
        {
            var recipeMainImage = Context.RecipeImages.FirstOrDefault(x => x.RecipeId == recipeId && x.MainPicture == true);

            if (recipeMainImage is null || recipeMainImage.Value is null)
            {
                throw new ImageException(ImageException.RecipeHasNoMainImage);
            }
            return string.Format("data:image/png;base64, {0}", Convert.ToBase64String(recipeMainImage.Value));
        }
    }
}
