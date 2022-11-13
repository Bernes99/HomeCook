using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class CommentsRecipe : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public long RecipeId { get; set; }
        public long CommentId { get; set; }

        public virtual Comment Comment { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
