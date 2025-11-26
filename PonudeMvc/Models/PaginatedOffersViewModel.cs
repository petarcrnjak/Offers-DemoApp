using Application.DTOs;

namespace Ponude.Models;

public class PaginatedOffersViewModel
{
    public List<OfferDto> Offers { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}