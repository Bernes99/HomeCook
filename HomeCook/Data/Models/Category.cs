using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Category : IEntity
    {
        public Category()
        {
            RecipesCategories = new HashSet<RecipesCategory>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<RecipesCategory> RecipesCategories { get; set; }
    }
}
