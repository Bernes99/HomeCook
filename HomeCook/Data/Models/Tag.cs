using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Tag : IEntity
    {
        public Tag()
        {
            RecipeTags = new HashSet<RecipeTag>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<RecipeTag> RecipeTags { get; set; }
    }
}
