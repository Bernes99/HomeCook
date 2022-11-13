using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class RecipesImage : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public long RecipeId { get; set; }
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;

        public virtual Recipe Recipe { get; set; } = null!;
    }
}
