namespace Domain.Enums
{
    public enum OrderStatus : int
    {
        PendingPayment = 1,
        Accepted = 2,
        InTransit = 3,
        Finished = 4,
        Canceled = 5
    }
}