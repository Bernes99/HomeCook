using AutoMapper;
using HomeCook.Data.Models;
using HomeCook.DTO;
using HomeCook.DTO.Product;

namespace HomeCook.Data.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<AppUser, UserDto>();


        }
    }
}
