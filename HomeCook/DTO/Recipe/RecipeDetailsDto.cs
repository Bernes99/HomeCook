using HomeCook.DTO.Product;

namespace HomeCook.DTO.Recipe
{
    public class RecipeDetailsDto
    {
        public string Id { get; set; }
        public DateTime DateUtc { get; set; }
        public string Title { get; set; }
        public string? Introdution { get; set; }
        public string? Text { get; set; }
        public float? Rating { get; set; }
        public string Portion { get; set; }
        public string PreparingTime { get; set; }
        public float Difficulty { get; set; }
        public RecipeUserDto Author { get; set; }
        public List<RecipeProductResponseDto> Products { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public string MainImage { get; set; }
        public List<string> Images { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
