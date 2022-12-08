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

            CreateMap<Tag, TagDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId));

            CreateMap<AddRecipeDto, Recipe>();

            CreateMap<AppUser, RecipeUserDto>();


            CreateMap<Recipe, RecipeDetailsDto>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.PublicId))
            .ForMember(d => d.DateUtc, m => m.MapFrom((src, dest) =>
            {
                if (src.DateModifiedUtc is not null && src.DateModifiedUtc > src.DateCreatedUtc)
                {
                    return src.DateModifiedUtc;
                }
                return src.DateCreatedUtc;
            }));


        }
    }
}
