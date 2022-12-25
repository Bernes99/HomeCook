using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Recipe : IEntity
    {
        public Recipe()
        {
            Comments = new HashSet<Comment>();
            RecipeCategories = new HashSet<RecipeCategory>();
            RecipeImages = new HashSet<RecipeImage>();
            RecipeProducts = new HashSet<RecipeProduct>();
            RecipeTags = new HashSet<RecipeTag>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public DateTime DateCreatedUtc { get; set; }
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

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<RecipeCategory> RecipeCategories { get; set; }
        public virtual ICollection<RecipeImage> RecipeImages { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        public virtual ICollection<RecipeTag> RecipeTags { get; set; }
    }
}
