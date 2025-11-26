using System.Text.Json.Serialization;
using Application.Mappers;
using Core.Entities;

namespace Application.DTOs;

public class OfferDto
{
    public OfferDto(Offer offer)
    {
        Id = offer.Id;
        Date = offer.Date;
        OfferItems = OfferMapper.OfferItemsToDto(offer.OfferDetails);
        TotalAmount = offer.CalculateTotalAmount();
    }

    public OfferDto()
    {
    }

    public int Id { get; set; }
    public DateTime Date { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<OfferItemDto>? OfferItems { get; set; }

    public decimal TotalAmount { get; set; }
}