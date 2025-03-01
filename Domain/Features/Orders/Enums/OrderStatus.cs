namespace Domain.Orders.Enums
{
    public enum OrderStatus : ushort
    {
        PendingPayment = 1,
        ReadyToAccept = 2,
        Accepted = 3,
        InTransit = 4,
        Finished = 5,
        Canceled = 6,
        Expired = 7,
    }
}