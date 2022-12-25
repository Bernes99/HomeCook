using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class RecipeCategory : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public long CategoryId { get; set; }
        public long RecipeId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
