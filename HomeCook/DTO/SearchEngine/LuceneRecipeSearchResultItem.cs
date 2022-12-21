namespace HomeCook.DTO.SearchEngine
{
    public class LuceneRecipeSearchResultItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string DateCreatedUtc { get; set; }
        public string Author { get; set; }
        public float Rating { get; set; }
        public float Difficulty { get; set; }
        public string MainImage { get; set; }
        public List<string> Products { get; set; }
    }
}
