namespace HomeCook.DTO
{
    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

        public IFormFile? File { get; set; }
    }
}
