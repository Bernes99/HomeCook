using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class RecipeTag : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public long TagId { get; set; }
        public long RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
