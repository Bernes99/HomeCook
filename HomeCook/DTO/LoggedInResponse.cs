namespace HomeCook.DTO
{
    public class LoggedInResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Id { get; set; }

    }
}
