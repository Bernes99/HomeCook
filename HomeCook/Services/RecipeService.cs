using AutoMapper;
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
using HomeCook.Data.Extensions.SearchEngine;
using HomeCook.DTO.SearchEngine;

namespace HomeCook.Services
{
    public class RecipeService : BaseService, IRecipeService
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IRecipeSearchEngine _recipeSearchEngine;
        public RecipeService(DefaultDbContext context,
        IMapper mapper,
        IUserService userService,
        IImageService imageService,
        ICategoryService categoryService,
        IProductService productService,
        IRecipeSearchEngine recipeSearchEngine) : base(context, mapper)
        {
            _recipeSearchEngine = recipeSearchEngine;
            _userService = userService;
            _imageService = imageService;
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<Recipe> AddRecipe(IFormFile? mainPicture, IFormFile?[] pictures, AddRecipeDto model)
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

            Create(newRecipe);

            AddTags(model.Tags);
            var tagsInternalIds = GetTagsInternalIds(model.Tags);

            foreach (var id in tagsInternalIds)
            {
                newRecipe.RecipesTags.Add(new RecipesTag { TagId = id, RecipeId = newRecipe.Id });
            }

            _imageService.AddOrUpdateRecipeImage(mainPicture, newRecipe.Id, true);
            foreach (var picture in pictures)
            {
                _imageService.AddOrUpdateRecipeImage(picture, newRecipe.Id, false);
            }

            _recipeSearchEngine.AddOrUpdateRange(Context.Recipes.Include(x => x.RecipeProducts).ThenInclude(x => x.Product)
                .Include(x => x.RecipesTags).ThenInclude(x => x.Tag)
                .Include(x => x.RecipesCategories).ThenInclude(x => x.Category).Where(x => x.Id == newRecipe.Id).ToList());

            return newRecipe;
        }
        public async Task<RecipeDetailsDto> UpdateRecipe(AddRecipeDto model, string recipePublicId)
        {
            var recipe = Context.Recipes.Include(x => x.RecipeProducts).ThenInclude(x => x.Product)
                .Include(x => x.RecipesTags).ThenInclude(x => x.Tag)
                .Include(x => x.RecipesCategories).ThenInclude(x => x.Category).FirstOrDefault(x => x.PublicId == recipePublicId);
            if (recipe is null)
            {
                throw new NullReferenceException(); //TODO
            }
            Mapper.Map(model, recipe);
            recipe.DateModifiedUtc = DateTime.UtcNow;

            var previousRecipeProducts = recipe.RecipeProducts.ToList();
            Context.RemoveRange(previousRecipeProducts);
            SaveChanges();

            var productsIds = _productService.FindAllPorductIds();
            foreach (var product in model.Products)
            {
                var productId = productsIds.FirstOrDefault(x => product.ProductId == x.Value);
                if (product is null)
                {
                    throw new ProductException(ProductException.ProductDoesntExist);//TODO exeption
                }

                recipe.RecipeProducts.Add(new RecipeProduct { RecipeId = recipe.Id, ProductId = productId.Key, Amount = product.Amount });
            }
            var categoriesIds = _categoryService.FindAllCategoriesIds();

            if (model.CategoriesIds.Count != model.CategoriesIds.Distinct().Count())
            {
                throw new CategoryException(ProductException.CantAddManyOfTheSameProduscts);//TODO exeption
            }

            var previousRecipeCategories = recipe.RecipesCategories.ToList();
            Context.RemoveRange(previousRecipeCategories);
            SaveChanges();
            foreach (var category in model.CategoriesIds)
            {
                var categoryId = categoriesIds.FirstOrDefault(x => x.Value == category);
                if (category is null)
                {
                    throw new CategoryException(CategoryException.SomethingWentWrong); //TODO exeption
                }
                recipe.RecipesCategories.Add(new RecipesCategory { RecipeId = recipe.Id, CategoryId = categoryId.Key });
            }
            AddTags(model.Tags);
            var tagsInternalIds = GetTagsInternalIds(model.Tags);
                

            var previousRecipeTags = recipe.RecipesTags.ToList();
            Context.RemoveRange(previousRecipeTags);
            SaveChanges();
            foreach (var id in tagsInternalIds)
            {
                recipe.RecipesTags.Add(new RecipesTag { TagId = id, RecipeId = recipe.Id });
            }


            Update(recipe);

            _recipeSearchEngine.AddOrUpdateRange(new List<Recipe>(){ recipe });

            return await GetRecipeDetails(recipe.Id);


        }

        public async Task<bool> DeleteRecipe( string recipePublicId, string userId)
        {
            var recipe = FindRecipeByPublicId(recipePublicId);

            recipe.DeletedBy = userId;
            recipe.DateDeletedUtc = DateTime.Now;
            _recipeSearchEngine.Remove(recipe);

            Update(recipe);
            return true;
        }

        public async Task<List<RecipeDetailsDto>> GetUserRecipes(string userId)
        {
            var recipesId = Context.Recipes.Where(x => x.AuthorId == userId && x.DeletedBy == null && x.DateDeletedUtc == null).Select(x => x.Id).ToList();

            var recipes = new List<RecipeDetailsDto>();
            foreach (var id in recipesId)
            {
                var recipe = await GetRecipeDetails(id);
                if (recipe is null)
                {
                    throw new NullReferenceException();
                }
                recipes.Add(recipe);
            }
            return recipes;
        }

        public async Task<RecipeDetailsDto> GetRecipeDetails(string recipePublicId)
        {
            var recipe = Context.Recipes.FirstOrDefault(x => x.PublicId == recipePublicId);
            if (recipe == null)
            {
                throw new Exception("fail"); //TODO change Exeption
            }
            return await GetRecipeDetails(recipe.Id);
        }

        public async Task<RecipeDetailsDto> GetRecipeDetails(long recipeInternalId)
        {
            var recipe = Context.Recipes.Include(x => x.RecipeProducts).ThenInclude(x => x.Product)
                .Include(x => x.RecipesTags).ThenInclude(x => x.Tag)
                .Include(x => x.RecipesCategories).ThenInclude(x => x.Category)
                .Include(x => x.Comments) // TODO to mozna pominac
                .Include(x => x.RecipesImages).FirstOrDefault(x => x.Id == recipeInternalId);
            if (recipe == null)
            {
                throw new Exception("fail"); //TODO change Exeption
            }

            var user = Context.Users.FirstOrDefault(x => x.Id == recipe.AuthorId);

            var recipeDto = Mapper.Map<RecipeDetailsDto>(recipe);
            recipeDto.Author = Mapper.Map<RecipeUserDto>(user);

            var products = new List<RecipeProduct>();
            foreach (var item in recipe.RecipeProducts)
            {
                products.Add(item);
            }
            recipeDto.Products = Mapper.Map<List<RecipeProductResponseDto>>(products);

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

        public async Task<List<LuceneRecipeSearchResultItem>> GetRecipesList(string searchString, RecipeFilters filters)
        {
            if (filters.CategoryNames is null)
            {
                filters.CategoryNames = new string[] { };
            }
            if (filters.Products is null)
            {
                filters.Products = new string[] { };
            }
            var searchResults = _recipeSearchEngine.Search(searchString, filters);
            foreach (var item in searchResults)
            {
                var user = Context.Users.FirstOrDefault(x => x.Id == item.Author);
                if (user is null)
                {
                    item.Author = "";
                    continue;
                }
                    
                var displayName = user.firstName +" "+ user.surname;
                item.Author = String.IsNullOrWhiteSpace(displayName) ? user.UserName : displayName;

                item.MainImage = _imageService.GetrecipeMainImage(item.Id);
            }
            return searchResults;
        }

        public async Task<CommentResponseDto> AddComment(string recipeId, string userId, string text)
        {
            var recipe = FindRecipeByPublicId(recipeId);
            
            var comment = new Comment() { RecipeId = recipe.Id, AuthorId = userId, DateCreatedUtc = DateTime.UtcNow, Text = text};

            Create(comment);

            var commentDto = Mapper.Map<CommentResponseDto>(comment);
            return commentDto;
        }

        public async Task<List<CommentResponseDto>> GetComments(string recipeId)
        {
            var recipe = FindRecipeByPublicId(recipeId);
            var comments = Context.Comments.Where(x => x.RecipeId == recipe.Id && x.DeletedBy == null && x.DateDeletedUtc == null).ToList();

            if (recipe is null)
            {
                throw new ArgumentNullException();//TODO
            }

            var commentsDto = Mapper.Map<List<CommentResponseDto>>(recipe.Comments);
            foreach (var comment in commentsDto)
            {
                var user = await _userService.FindUserAsyncbyId(comment.Author);
                var displayName = user.firstName + " " + user.surname;
                comment.Author = String.IsNullOrWhiteSpace(displayName) ? user.UserName : displayName;
                comment.AuthorProfile = _imageService.GetProfileImage(user.Id);
            }

            return commentsDto;
        }

        public void DeleteComment(string recipeId, string commentId, string userId)
        {
            var recipe = FindRecipeByPublicId(recipeId);
            var comment = Context.Comments.FirstOrDefault(x => x.PublicId == commentId && x.RecipeId == recipe.Id);

            if (comment is null)
            {
                throw new ArgumentNullException(); //TODO
            }

            comment.DeletedBy = userId;
            comment.DateDeletedUtc = DateTime.UtcNow;
            Update(comment);
        }

        public Comment FindCommentByPublicId(string commentId)
        {
            var comment = Context.Comments.FirstOrDefault(x => x.PublicId == commentId);
            if (comment is null)
            {
                throw new ArgumentNullException(); //TODO
            }
            return comment;
        }
        public Recipe FindRecipeByPublicId(string recipePublicId)
        {
            var recipe = Context.Recipes.FirstOrDefault(x => x.PublicId == recipePublicId);
            if (recipe is null)
            {
                throw new NullReferenceException(); //TODO
            }
            return recipe;
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
                return newTags.Select(x => x.Id).ToList();
            }
            return Context.Tags.Where(x=> tags.Contains(x.Name)).Select(x => x.Id).ToList();

        }
        private List<long> GetTagsInternalIds(List<string> tags)
        {
            return Context.Tags.Where(x => tags.Contains(x.Name)).Select(x => x.Id).ToList();
        }


        public void InitialIndexes()
        {
            var recipies = Context.Recipes.Include(x => x.RecipeProducts).ThenInclude(x => x.Product)
                .Include(x => x.RecipesTags).ThenInclude(x => x.Tag)
                .Include(x => x.RecipesCategories).ThenInclude(x => x.Category).ToList();
            _recipeSearchEngine.AddOrUpdateRange(recipies);
        }
    }
}
