using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Product : IEntity
    {
        public Product()
        {
            RecipeProducts = new HashSet<RecipeProduct>();
            UserProducts = new HashSet<UserProduct>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long? Calories { get; set; }
        public int? Category { get; set; }
        public int? UnitType { get; set; }

        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }
    }
}
