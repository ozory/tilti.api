namespace Domain.Features.Payments.Enums;

public enum PaymentType : int
{
    CreditCard = 1,
    DebitCard = 2,
    Pix = 3,
    PassengerRide = 4,  // NEW - For passenger ride payments
}