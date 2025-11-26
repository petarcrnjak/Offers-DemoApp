using Application.Requests;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using Core.Processors;

namespace Infrastructure.Processing;

public class OfferImportProcessor : IOfferImportProcessor
{
    private readonly IOfferRepository _offerRepository;

    public OfferImportProcessor(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public async Task<OperationResult> ProcessOfferAsync(OfferProcessingRequest request)
    {
        //var errors = new List<string>();
        var article = await GetOrCreateArticleAsync(request.ArticleId, request.UnitPrice,
            request.ArticleName);

        if (article == null)
        {
            //errors.Add("Failed to process article");
            return OperationResult.Fail("Failed to process article");
        }

        var existingOffer = await _offerRepository.GetOfferByIdAsync(request.OfferId);

        if (existingOffer != null)
        {
            var isDuplicate = existingOffer.OfferDetails.Any(od => od.OfferItemId == request.ArticleId);
            if (isDuplicate)
            {
                //errors.Add($"Artikl {request.ArticleId} već postoji u narudžbi {request.OfferId}");
                return OperationResult.Fail($"Artikl {request.ArticleId} već postoji u narudžbi {request.OfferId}");
            }

            existingOffer.OfferDetails.Add(new OfferDetails
            {
                OfferItem = article,
                Quantity = request.Quantity
            });

            await _offerRepository.UpdateOffer(existingOffer);
            return OperationResult.Ok();
        }

        var newOffer = new Offer
        {
            Id = request.OfferId,
            Date = request.Date,
            OfferDetails = new List<OfferDetails>
            {
                new() { OfferItem = article, Quantity = request.Quantity }
            }
        };

        await _offerRepository.CreateNewOfferImportAsync(newOffer);

        return OperationResult.Ok();
    }

    private async Task<OfferItem?> GetOrCreateArticleAsync(int id, decimal unitPrice, string name)
    {
        var article = await _offerRepository.GetOfferItemByIdAsync(id);
        if (article != null) return article;

        var newArticle = new OfferItem { Id = id, UnitPrice = unitPrice, Article = name };
        return await _offerRepository.CreateNewOfferItem(newArticle);
    }
}