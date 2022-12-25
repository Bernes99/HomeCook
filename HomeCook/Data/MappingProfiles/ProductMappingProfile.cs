using AutoMapper;
using HomeCook.Data.Models;
using HomeCook.DTO.Product;

namespace HomeCook.Data.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductCategory, CategoryDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId));

            CreateMap<Product, ProductDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId))
                .ForMember(d => d.CategoryId, m => m.MapFrom(s => s.Category.PublicId));

            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId));

            CreateMap<RecipeProduct, RecipeProductResponseDto>();

            CreateMap<ProductDto, Product>()
                .ForMember(d => d.Id, m => m.Ignore())
                .ForMember(d => d.CategoryId, m => m.Ignore());

            CreateMap<AddUserProductDto, UserProduct>()
                .ForMember(d => d.ProductId, m => m.MapFrom(s => s.ProductInternalId));

            CreateMap<UserProduct, UserProductDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId));

        }
    }
}
