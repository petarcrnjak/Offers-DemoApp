using Core.Entities;

namespace Application.DTOs;

public class OfferItemDto
{
    public OfferItemDto(OfferItem offerItem)
    {
        Id = offerItem.Id;
        Article = offerItem.Article;
        UnitPrice = offerItem.UnitPrice;
    }

    public OfferItemDto()
    {
    }

    public int Id { get; set; }
    public string Article { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal? TotalPrice => Quantity * UnitPrice;
}