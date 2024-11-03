namespace Domain.Enums
{
    public enum UserStatus : ushort
    {
        Active = 1,
        PendingRegisterConfirmation = 2,
        PendingPaymentInformation = 3,
        Inactive = 4,
    }
}