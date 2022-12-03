using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class ProductCategory : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
