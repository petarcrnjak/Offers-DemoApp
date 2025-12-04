using Application.Requests;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using Core.Processors;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Processing;

public class OfferImportProcessor : IOfferImportProcessor
{
    private readonly IBulkOfferRepository _bulkOfferRepository;
    private readonly IDbExceptionParser _dbExceptionParser;

    public OfferImportProcessor(IBulkOfferRepository bulkOfferRepository, IDbExceptionParser dbExceptionParser)
    {
        _bulkOfferRepository = bulkOfferRepository;
        _dbExceptionParser = dbExceptionParser;
    }

    public async Task<OperationResult> ProcessOfferAsync(OfferProcessingRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return OperationResult.Fail("Request was null");

        var article = await GetOrCreateArticleAsync(request.ArticleId, request.UnitPrice,
            request.ArticleName, cancellationToken);

        if (article == null)
            return OperationResult.Fail("Failed to process article");

        try
        {
            var newOffer = new Offer
            {
                Id = request.OfferId,
                Date = request.Date,
                OfferDetails =
                [
                    new OfferDetails
                    {
                        OfferItemId = article.Id,
                        Quantity = request.Quantity
                    }
                ]
            };

            await _bulkOfferRepository.CreateNewOfferImportAsync(newOffer, cancellationToken);
            return OperationResult.Ok();
        }
        catch (DbUpdateException dbEx) when (_dbExceptionParser.IsUniqueKeyViolation(dbEx))
        {
            // Offer likely exists already; try to add the detail once.
            var added = await _bulkOfferRepository.AddOfferDetailAsync(request.OfferId, article.Id, request.Quantity, cancellationToken);
            if (added)
                return OperationResult.Ok();

            // If not added, then the (OfferId, OfferItemId) pair already exists -> duplicate
            return OperationResult.Fail($"Artikl {article.Id} već postoji u narudžbi {request.OfferId}");
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

    private async Task<OfferItem?> GetOrCreateArticleAsync(int id, decimal unitPrice, string name, CancellationToken cancellationToken = default)
    {
        var article = await _bulkOfferRepository.GetOfferItemByIdAsync(id);
        if (article != null)
            return article;

        var newArticle = new OfferItem
        {
            Id = id,
            UnitPrice = unitPrice,
            Article = name
        };

        return await _bulkOfferRepository.CreateNewOfferItem(newArticle, cancellationToken);
    }
}