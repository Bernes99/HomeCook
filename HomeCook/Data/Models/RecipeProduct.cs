using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class RecipeProduct : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public long RecipeId { get; set; }
        public long ProductId { get; set; }
        public string? Amount { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
