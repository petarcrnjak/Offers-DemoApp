namespace Core.Entities;

public class Offer
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public ICollection<OfferDetails> OfferDetails { get; set; } = new List<OfferDetails>();

    public decimal CalculateTotalAmount()
    {
        return OfferDetails?.Sum(od => od.Quantity * od.OfferItem.UnitPrice) ?? 0;
    }
}