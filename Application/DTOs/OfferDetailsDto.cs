using Core.Entities;

namespace Application.DTOs;

public class OfferDetailsDto
{
    public int OfferId { get; set; }
    public Offer Offer { get; set; }

    public int OfferItemId { get; set; }
    public OfferItem OfferItem { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice => Quantity * OfferItem.UnitPrice;
}