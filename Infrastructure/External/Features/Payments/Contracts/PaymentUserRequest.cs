using Domain.Features.Users.Entities;

namespace Infrastructure.External.Features.Payments.Contracts;

public record PaymentUserRequest(
    string Name,
    string Email,
    string CpfCnpj
)
{
    public static implicit operator PaymentUserRequest(User user)
        => new(user.Name.Value!, user.Email.Value!, user.Document.Value!);
}
