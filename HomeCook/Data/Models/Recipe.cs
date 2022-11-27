using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Recipe : IEntity
    {
        public Recipe()
        {
            CommentsRecipes = new HashSet<CommentsRecipe>();
            RecipeProducts = new HashSet<RecipeProduct>();
            RecipesCategories = new HashSet<RecipesCategory>();
            RecipesImages = new HashSet<RecipesImage>();
            RecipesTags = new HashSet<RecipesTag>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public DateTime DateCreatedUtc { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? DateModifiedUtc { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DateDeletedUtc { get; set; }
        public string? DeletedBy { get; set; }
        public string Title { get; set; } = null!;
        public string? Introdution { get; set; }
        public string? Text { get; set; }
        public float? Rating { get; set; }
        public string Portion { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public string PreparingTime { get; set; } = null!;
        public float Difficulty { get; set; }

        public virtual ICollection<CommentsRecipe> CommentsRecipes { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        public virtual ICollection<RecipesCategory> RecipesCategories { get; set; }
        public virtual ICollection<RecipesImage> RecipesImages { get; set; }
        public virtual ICollection<RecipesTag> RecipesTags { get; set; }
    }
}
