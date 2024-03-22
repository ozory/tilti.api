namespace Application.Features.Security.Contracts
{
    public class RefreshTokens
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime LastLogin { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}