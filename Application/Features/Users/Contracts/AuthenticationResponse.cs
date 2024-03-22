namespace Application.Features.Users.Contracts;

public sealed record AuthenticationResponse(
    long? Id,
    string? Name,
    string? Email,
    string? Token,
    string? RefreshToken)
{ }
