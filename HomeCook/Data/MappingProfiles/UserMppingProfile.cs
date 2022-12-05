using AutoMapper;
using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Product;

namespace HomeCook.Data.MappingProfiles
{
    public class UserMppingProfile : Profile
    {
        public UserMppingProfile()
        {
            CreateMap<AppUser, UserDto>();


        }
    }
}
