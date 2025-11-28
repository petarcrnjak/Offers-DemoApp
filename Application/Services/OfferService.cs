using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;

namespace Application.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IValidator<OfferItemDto> _validator;

    public OfferService(IOfferRepository offerRepository, IValidator<OfferItemDto> validator)
    {
        _offerRepository = offerRepository;
        _validator = validator;
    }

    public async Task<List<OfferDto>> GetOffersAsync()
    {
        var offers = await _offerRepository.GetAllOffersAsync();

        return offers.Select(offer => new OfferDto(offer)).ToList();
    }

    public async Task<List<OfferDto>> GetOffersAsync(int pageIndex, int pageSize)
    {
        var paginatedOffers = await _offerRepository.GetAllOffersAsync(pageIndex, pageSize);

        return paginatedOffers.Select(offer => new OfferDto(offer)).ToList();
    }

    public async Task<OfferDto?> GetOfferByIdAsync(int id)
    {
        var offer = await _offerRepository.GetOfferByIdAsync(id);

        return offer == null ? null : new OfferDto(offer); // Mapping to DTO
    }

    public async Task<OfferDto?> UpdateOffer(int id, OfferDto updatedOffer)
    {
        var offer = await _offerRepository.GetOfferByIdAsync(id);
        if (offer == null)
            return null;

        // Remove OfferDetails that are not in the updatedOffer (to handle removals)
        var itemsToRemove = offer.OfferDetails
            .Where(od => updatedOffer.OfferItems != null && updatedOffer.OfferItems.All(dto => dto.Id != od.OfferItemId))
            .ToList();

        foreach (var item in itemsToRemove)
            offer.OfferDetails.Remove(item);

        if (updatedOffer.OfferItems != null)
            foreach (var item in updatedOffer.OfferItems)
            {
                var validationResult = await _validator.ValidateAsync(item);
                if (!validationResult.IsValid)
                {
                    validationResult.Errors.ForEach(error => error.ErrorMessage = $"{item.Article}: {error.ErrorMessage}");
                    throw new ValidationException(validationResult.Errors);
                }

                var existingOfferItem = await _offerRepository.GetOfferItemByIdAsync(item.Id);
                if (existingOfferItem == null)
                    continue;

                // Add or update OfferDetails
                var offerDetails = offer.OfferDetails.FirstOrDefault(od => od.OfferItemId == item.Id);
                if (offerDetails == null)
                {
                    offerDetails = new OfferDetails
                    {
                        OfferId = id,
                        OfferItemId = item.Id,
                        OfferItem = existingOfferItem,
                        Offer = offer,
                        Quantity = item.Quantity
                    };
                    offer.OfferDetails.Add(offerDetails);
                }
                else
                {
                    // Update existing OfferDetails if needed
                    offerDetails.Quantity = item.Quantity;
                }
            }

        var updatedOfferResult = await _offerRepository.UpdateOffer(offer);
        if (updatedOfferResult == null)
            return null;

        return OfferMapper.OfferToDto(offer);
    }

    public async Task<bool> DeleteOfferItem(int offerId, int itemId)
    {
        return await _offerRepository.DeleteOfferItemAsync(offerId, itemId);
    }

    public async Task<int> GetTotalOffersCountAsync()
    {
        return await _offerRepository.GetTotalOffersCount();
    }

    public async Task<IEnumerable<OfferItem>> GetOfferItemsNames()
    {
        return await _offerRepository.GetOfferItemsNames();
    }

    public async Task<OfferDto?> CreateNewOfferAsync()
    {
        var newOffer = new Offer
        {
            Date = DateTime.Now
        };

        var createdOffer = await _offerRepository.CreateNewOfferAsync(newOffer);
        return OfferMapper.OfferToDto(createdOffer);
    }
}