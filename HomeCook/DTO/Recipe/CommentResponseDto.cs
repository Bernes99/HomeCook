namespace HomeCook.DTO.Recipe
{
    public class CommentResponseDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public string Author { get; set; }
        public string AuthorProfile { get; set; }
    }
}
