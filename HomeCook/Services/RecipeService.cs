﻿using AutoMapper;
using HomeCook.Data.Extensions;
using HomeCook.Data;
using HomeCook.Services.Interfaces;
using HomeCook.Data.Models;
using HomeCook.DTO.Recipe;
using Microsoft.AspNetCore.Mvc;
using HomeCook.Data.CustomException;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using HomeCook.DTO.Product;

namespace HomeCook.Services
{
    public class RecipeService : BaseService, IRecipeService
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        public RecipeService(DefaultDbContext context,
        IMapper mapper,
        IUserService userService,
        IImageService imageService,
        ICategoryService categoryService,
        IProductService productService) : base(context, mapper)
        {
            _userService = userService;
            _imageService = imageService;
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<Recipe> AddRecipe(IFormFile? mainPicture, IFormFile?[] pictures, [FromBody] AddRecipeDto model)
        {
            var newRecipe = Mapper.Map<Recipe>(model);
            newRecipe.DateCreatedUtc = DateTime.UtcNow;

            var productsIds = _productService.FindAllPorductIds();
            foreach (var product in model.Products)
            {
                var productId = productsIds.FirstOrDefault(x => product.ProductId == x.Value);
                if (product is null)
                {
                    throw new ProductException(ProductException.ProductDoesntExist);//TODO exeption
                }

                newRecipe.RecipeProducts.Add(new RecipeProduct { RecipeId = newRecipe.Id, ProductId = productId.Key, Amount = product.Amount });
            }
            var categoriesIds = _categoryService.FindAllCategoriesIds();
            
            if (model.CategoriesIds.Count != model.CategoriesIds.Distinct().Count())
            {
                throw new CategoryException(ProductException.CantAddManyOfTheSameProduscts);//TODO exeption
            }

            foreach (var category in model.CategoriesIds)
            {
                var categoryId = categoriesIds.FirstOrDefault(x => x.Value == category);
                if (category is null)
                {
                    throw new CategoryException(CategoryException.SomethingWentWrong); //TODO exeption
                }
                newRecipe.RecipesCategories.Add(new RecipesCategory { RecipeId = newRecipe.Id, CategoryId = categoryId.Key });
            }

            var tagsInternalIds = AddTags(model.Tags);
            foreach (var id in tagsInternalIds)
            {
                newRecipe.RecipesTags.Add(new RecipesTag { TagId = id, RecipeId = newRecipe.Id });
            }

            Create(newRecipe);

            _imageService.UpdateRecipeImage(mainPicture, newRecipe.Id, true);
            foreach (var picture in pictures)
            {
                _imageService.UpdateRecipeImage(picture, newRecipe.Id, false);
            }
            return newRecipe;
        }


        public async Task<RecipeDetailsDto> GetRecipeDetails(string recipePublicId)
        {
            var recipe = Context.Recipes.Include(x => x.RecipeProducts).ThenInclude(x => x.Product)
                .Include(x => x.RecipesTags).ThenInclude(x => x.Tag)
                .Include(x => x.RecipesCategories).ThenInclude(x => x.Category)
                .Include(x => x.Comments) // TODO to mozna pominac
                .Include(x => x.RecipesImages).FirstOrDefault(x => x.PublicId == recipePublicId);
            if (recipe == null)
            {
                throw new Exception("fail"); //TODO change Exeption
            }

            var user = Context.Users.FirstOrDefault(x => x.Id == recipe.AuthorId);

            var recipeDto = Mapper.Map<RecipeDetailsDto>(recipe);
            recipeDto.Author = Mapper.Map<RecipeUserDto>(user);

            var products = new List<Product>();
            foreach (var item in recipe.RecipeProducts)
            {
                products.Add(item.Product);
            }
            recipeDto.Products = Mapper.Map<List<ProductResponseDto>>(products);

            var categories = new List<Category>();
            foreach (var item in recipe.RecipesCategories)
            {
                categories.Add(item.Category);
            }
            recipeDto.Categories = Mapper.Map<List<CategoryDto>>(categories);

            var images = new List<string>();
            foreach (var item in recipe.RecipesImages)
            {
                if (item.MainPicture)
                {
                    recipeDto.MainImage = string.Format("data:image/png;base64, {0}", Convert.ToBase64String(item.Value));
                }
                else
                {
                    images.Add(string.Format("data:image/png;base64, {0}", Convert.ToBase64String(item.Value)));
                } 
            }
            recipeDto.Images = images;

            var tags = new List<Tag>();
            foreach (var item in recipe.RecipesTags)
            {
                tags.Add(item.Tag);
            }
            recipeDto.Tags = Mapper.Map<List<TagDto>>(tags);


            return recipeDto;
        }


        public Dictionary<long, string> FindAllTagsIds()
        {
            return Context.Tags.Select(p => new KeyValuePair<long, string>(p.Id, p.PublicId)).ToDictionary(x => x.Key, x => x.Value);
        }


        /// <summary>
        /// Method update add tags if not exist
        /// </summary>
        /// <param name="tags">List of tag names</param>
        /// <returns>Returns List of tags ids</returns>
        private List<long> AddTags(List<string> tags)
        {
            var currentTagNames = Context.Tags.Select(x => x.Name).ToArray();
            var newTags = new List<Tag>();
            foreach (var tag in tags)
            {
                if (!currentTagNames.Contains(tag))
                {
                    newTags.Add(new Tag(){ Name = tag });
                }
            }
            if (newTags.Count > 0)
            {
                Context.AddRange(newTags);
                SaveChanges();
            }
           
            return newTags.Select(x => x.Id).ToList();
        }
    }
}
