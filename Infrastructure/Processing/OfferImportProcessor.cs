using Application.Requests;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using Core.Processors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Processing;

public class OfferImportProcessor : IOfferImportProcessor
{
    private readonly IOfferRepository _offerRepository;

    public OfferImportProcessor(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public async Task<OperationResult> ProcessOfferAsync(OfferProcessingRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return OperationResult.Fail("Request was null");

        try
        {
            var article = await GetOrCreateArticleAsync(request.ArticleId, request.UnitPrice,
                request.ArticleName);

            if (article == null)
            {
                return OperationResult.Fail("Failed to process article");
            }

            var existingOffer = await _offerRepository.GetOfferByIdAsync(request.OfferId);

            if (existingOffer != null)
            {
                var isDuplicate = existingOffer.OfferDetails.Any(od => od.OfferItemId == request.ArticleId);
                if (isDuplicate)
                {
                    return OperationResult.Fail($"Artikl {request.ArticleId} već postoji u narudžbi {request.OfferId}");
                }

                existingOffer.OfferDetails.Add(new OfferDetails
                {
                    OfferItemId = article.Id,
                    Quantity = request.Quantity
                });

                await _offerRepository.UpdateOffer(existingOffer, cancellationToken);
                return OperationResult.Ok();
            }

            var newOffer = new Offer
            {
                Id = request.OfferId,
                Date = request.Date,
                OfferDetails = new List<OfferDetails>
            {
                new() {
                    OfferItemId = article.Id,
                    Quantity = request.Quantity
                }
            }
            };

            await _offerRepository.CreateNewOfferImportAsync(newOffer, cancellationToken);

            return OperationResult.Ok();
        }
        catch (DbUpdateException)
        {
            return OperationResult.Fail("Database error occurred while processing the offer.");
        }
        catch (Exception)
        {
            return OperationResult.Fail("Unexpected error occurred while processing the offer.");
        }
    }

    private async Task<OfferItem?> GetOrCreateArticleAsync(int id, decimal unitPrice, string name)
    {
        var article = await _offerRepository.GetOfferItemByIdAsync(id);
        if (article != null)
            return article;

        var newArticle = new OfferItem
        {
            Id = id,
            UnitPrice = unitPrice,
            Article = name
        };

        return await _offerRepository.CreateNewOfferItem(newArticle);
    }
}