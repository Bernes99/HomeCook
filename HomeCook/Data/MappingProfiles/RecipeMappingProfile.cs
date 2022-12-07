using AutoMapper;
using HomeCook.Data.Models;
using HomeCook.DTO.Product;
using HomeCook.DTO.Recipe;

namespace HomeCook.Data.MappingProfiles
{
    public class RecipeMappingProfile : Profile
    {
        public RecipeMappingProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId));

            CreateMap<AddRecipeDto, Recipe>();


        }
    }
}
