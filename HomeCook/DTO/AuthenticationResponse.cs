namespace HomeCook.DTO
{
    public class AuthenticationResponse
    {
        public string? JwtToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? JwtTokenExpiryTime { get; set; }
    }
}
