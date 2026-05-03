namespace Application.Features.Payments.Contracts;

/// <summary>
/// Response contract for passenger payment operations
/// </summary>
public class PaymentResponse
{
    public string PaymentId { get; set; } = null!;      // Asaas payment ID
    public string? PixLink { get; set; }             // PIX link for frontend
    public string? QrCode { get; set; }              // PIX QR code (base64 or SVG)
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;        // "PENDING", "CONFIRMED", etc.
    public DateTime DueDate { get; set; }
}
