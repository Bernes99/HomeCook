using HomeCook.Data.Extensions.Interfaces;
using HomeCook.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeCook.Data.Models
{
    public partial class AppUser : IdentityUser
    {
        public string? firstName { get; set; }
        public string? surname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsDeleted { get; set; }



    }
}
