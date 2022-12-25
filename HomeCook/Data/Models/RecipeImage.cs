using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class RecipeImage : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public long RecipeId { get; set; }
        public string? Name { get; set; }
        public byte[]? Value { get; set; }
        public bool MainPicture { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;
    }
}
