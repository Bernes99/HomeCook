using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Category : IEntity
    {
        public Category()
        {
            RecipeCategories = new HashSet<RecipeCategory>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<RecipeCategory> RecipeCategories { get; set; }
    }
}
