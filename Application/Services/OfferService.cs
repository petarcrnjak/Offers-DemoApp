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
    private readonly IValidator<OfferDto> _validator;

    public OfferService(IOfferRepository offerRepository, IValidator<OfferDto> validator)
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

        var updatedItems = updatedOffer.OfferItems ?? [];

        var validationResult = await _validator.ValidateAsync(updatedOffer);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var domainOffer = new Offer
        {
            Id = id,
            Date = updatedOffer.Date,
            OfferDetails = updatedItems.Select(i => new OfferDetails
            {
                OfferId = id,
                OfferItemId = i.Id,
                Quantity = i.Quantity
            }).ToList()
        };

        var updated = await _offerRepository.UpdateOffer(domainOffer);
        if (updated == null)
            return null;

        return OfferMapper.OfferToDto(updated);
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