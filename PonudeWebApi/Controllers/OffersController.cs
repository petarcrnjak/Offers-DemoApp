using Application.DTOs;
using Application.Enums;
using Application.Interfaces;
using Application.Mappers;
using Microsoft.AspNetCore.Mvc;
using static Core.Helpers.PaginationHelper;
using static Application.Helpers.ApiResponseHelper;

namespace PonudeWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OffersController : ControllerBase
{
    private const int DefaultPageSize = 3;
    private readonly IOfferImportService _importService;
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService, IOfferImportService importService)
    {
        _offerService = offerService;
        _importService = importService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OfferDto>>> GetOffersAsync()
    {
        var offers = await _offerService.GetOffersAsync();
        return Ok(offers);
    }

    [HttpGet("paginated")]
    public async Task<ActionResult<List<OfferDto>>> GetOffersAsync([FromQuery] int pageIndex = 1)
    {
        // Get the total number of offers
        var totalOffersCount = await _offerService.GetTotalOffersCountAsync();
        var totalPages = CalculateTotalPages(totalOffersCount, DefaultPageSize);
        var offers = await _offerService.GetOffersAsync(pageIndex, DefaultPageSize);

        var response = new PaginatedOffersResponse
        {
            Offers = offers,
            TotalPages = totalPages,
            CurrentPage = pageIndex,
            PageSize = DefaultPageSize
        };
        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetOfferById")]
    public async Task<ActionResult<OfferDto>> GetOfferByIdAsync(int id)
    {
        var offer = await _offerService.GetOfferByIdAsync(id);
        if (offer == null)
            return NotFoundResponse<OfferDto>("Offer not found");
        return Ok(offer);
    }

    [HttpPost]
    public async Task<ActionResult<OfferDto>> CreateNewOffer()
    {
        var offer = await _offerService.CreateNewOfferAsync();
        if (offer == null)
            return BadRequestResponse("Failed to create a new Offer");

        return CreatedAtRoute("GetOfferById", new { id = offer.Id }, offer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOffer(int id, [FromBody] OfferDto offerDto)
    {
        var updatedOffer = await _offerService.UpdateOffer(id, offerDto);

        if (updatedOffer == null)
            return BadRequestResponse("Error updating offer");

        return Ok(updatedOffer);
    }

    [HttpDelete("{offerId}/items/{itemId}")]
    public async Task<IActionResult> DeleteOfferItem(int offerId, int itemId)
    {
        var result = await _offerService.DeleteOfferItem(offerId, itemId);
        if (result)
            return Ok(new { success = true });

        return BadRequest($"Failed to delete offer item: {itemId}");
    }

    [HttpGet("offerItems")]
    public async Task<ActionResult> GetArticlesForSelect()
    {
        var offerItemNames = await _offerService.GetOfferItemsNames();
        return Ok(offerItemNames.ToList());
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportResultDto>> ImportOffers([FromBody] List<OfferImportDto> offers,
        CancellationToken cancellationToken = default)
    {
        if (offers.Count == 0)
            return BadRequestResponse("No offers provided for import");

        var result = await _importService.ImportOffersAsync(offers, (int)BatchSettings.DefualtBatchSize, cancellationToken);
        return Ok(result);
    }
}