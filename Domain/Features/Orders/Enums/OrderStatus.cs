namespace Domain.Enums
{
    public enum OrderStatus : int
    {
        PendingPayment = 1,
        ReadyToAccept = 2,
        Accepted = 3,
        InTransit = 4,
        Finished = 5,
        Canceled = 6
    }
}