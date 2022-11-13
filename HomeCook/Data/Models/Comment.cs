using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Comment : IEntity
    {
        public Comment()
        {
            CommentsRecipes = new HashSet<CommentsRecipe>();
        }

        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public DateTime? DateDeletedUtc { get; set; }
        public string? DeletedBy { get; set; }
        public string Text { get; set; } = null!;

        public virtual ICollection<CommentsRecipe> CommentsRecipes { get; set; }
    }
}
