using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class ProductCategory : IEntity
    {
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
