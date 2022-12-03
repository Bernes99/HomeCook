using AutoMapper;
using HomeCook.Data.Models;
using HomeCook.DTO;

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
