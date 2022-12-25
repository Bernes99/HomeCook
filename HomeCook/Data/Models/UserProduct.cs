using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class UserProduct : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public long ProductId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public float? Amount { get; set; }
        public bool IsOnShoppingList { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
