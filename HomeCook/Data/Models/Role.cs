using HomeCook.Data.Extensions.Interfaces;
using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class Role : IEntity
    {
        public Role()
        {
            Users = new HashSet<AppUser>();
        }

        public long Id { get; set; }
        public string Value { get; set; } = null!;

        public virtual ICollection<AppUser> Users { get; set; }
    }
}
