using Application.Requests;
using Core.Common;

namespace Core.Processors;

public interface IOfferImportProcessor
{
    Task<OperationResult> ProcessOfferAsync(OfferProcessingRequest request);
}