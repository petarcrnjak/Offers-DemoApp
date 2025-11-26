using Application.DTOs;

namespace Application.Mappers;

public class PaginatedOffersResponse
{
    public List<OfferDto> Offers { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}