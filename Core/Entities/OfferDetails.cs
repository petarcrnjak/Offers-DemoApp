namespace Core.Entities;

public class OfferDetails
{
    public int OfferId { get; set; }
    public Offer Offer { get; set; }

    public int OfferItemId { get; set; }
    public OfferItem OfferItem { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice => Quantity * OfferItem.UnitPrice;
}