using HomeCook.Data.Enums;

namespace HomeCook.DTO.Product
{
    public class RecipeProductResponseDto
    {
        public ProductResponseDto Product { get; set; }
        public float? Amount { get; set; }

    }
}
