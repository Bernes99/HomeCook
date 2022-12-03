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
            CreateMap<ProductCategory, ProductCategoryDto>()
                .ForMember( d => d.Id , m => m.MapFrom( s => s.PublicId));


        }
    }
}
