using Application.DTOs;
using Core.Entities;

namespace Application.Mappers;

public static class OfferMapperToEntities
{
    public static Offer MappOfferItems(OfferDto offerDto)
    {
        var offer = new Offer
        {
            Id = offerDto.Id,
            Date = offerDto.Date,
            OfferDetails = offerDto.OfferItems?.Select(oi => new OfferDetails
            {
                OfferItemId = oi.Id,
                Quantity = oi.Quantity,
                OfferItem = new OfferItem
                {
                    Id = oi.Id,
                    Article = oi.Article,
                    UnitPrice = oi.UnitPrice
                }
            }).ToList() ?? new List<OfferDetails>()
        };

        return offer;
    }

    public static OfferImport MapOfferImportItems(OfferImportDto offerImportDto)
    {
        return new OfferImport
        {
            UnitPrice = (decimal)offerImportDto.UnitPrice,
            Quantity = (int)offerImportDto.Quantity,
            Article = offerImportDto.Article,
            Date = offerImportDto.OfferDate,
            ArticleId = (int)offerImportDto.ArticleId,
            OfferId = offerImportDto.OfferId,
            CellNumber = offerImportDto.RowNumber
        };
    }
}