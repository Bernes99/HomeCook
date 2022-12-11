namespace HomeCook.DTO.SearchEngine
{
    public class RecipeFilters
    {
        public string[] CategoryNames { get; set; }
        public string[] Products { get; set; }
        public string DateCreatedUtc { get; set; }
        public float Difficulty { get; set; }
        public float Rating { get; set; }
    }
}
