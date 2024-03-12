using Domain.Features.Users.Entities;

namespace Infrastructure.Data.Postgreesql.Features.Security.Entities;

public class RefreshTokens
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime LastLogin { get; set; }
    public string RefreshToken { get; set; } = null!;

    public User User { get; set; } = null!;
}
