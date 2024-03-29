using System;
using System.Collections.Generic;

namespace HomeCook.Data.Models
{
    public partial class ProfileImage : IEntity
    {
        public long Id { get; set; }
        public string PublicId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Name { get; set; }
        public byte[]? Value { get; set; }
        public string? Path { get; set; }
    }
}
