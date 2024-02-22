namespace Domain.Subscriptions.Enums;

public enum SubscriptionStatus : ushort
{
    Active = 1,
    Renewed = 2,
    PendingApproval = 3,
    Inactive = 4,

}
