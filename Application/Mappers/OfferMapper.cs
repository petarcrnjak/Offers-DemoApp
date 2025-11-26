using Application.DTOs;
using Application.Requests;
using Core.Entities;

namespace Application.Mappers;

public static class OfferMapper
{
    public static OfferDto OfferToDto(Offer offer)
    {
        return new OfferDto
        {
            Id = offer.Id,
            Date = offer.Date,
            OfferItems = offer.OfferDetails?.Select(o => new OfferItemDto
            {
                Id = o.OfferItem.Id,
                Article = o.OfferItem.Article,
                UnitPrice = o.OfferItem.UnitPrice,
                Quantity = o.Quantity
            }).ToList()
        };
    }

    public static List<OfferItemDto> OfferItemsToDto(ICollection<OfferDetails> offerDetails)
    {
        return offerDetails
            .Select(od => new OfferItemDto
            {
                Id = od.OfferItem.Id,
                Article = od.OfferItem.Article,
                UnitPrice = od.OfferItem.UnitPrice,
                Quantity = od.Quantity
            }).ToList();
    }

    public static OfferProcessingRequest ToProcessingRequest(OfferImportDto dto)
    {
        return new OfferProcessingRequest(
            dto.OfferId,
            dto.ArticleId!.Value,
            dto.UnitPrice!.Value,
            dto.Article!,
            dto.Quantity!.Value,
            dto.OfferDate
        );
    }
}