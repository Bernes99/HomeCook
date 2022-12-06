using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Comment : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public DateTime? DateDeletedUtc { get; set; }
        public string? DeletedBy { get; set; }
        public string? Text { get; set; }
        public long RecipeId { get; set; }
        public DateTime DateCreatedUtc { get; set; }

        public virtual Recipe Recipe { get; set; } = null!;
    }
}
