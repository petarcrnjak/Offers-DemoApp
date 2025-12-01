namespace Application.DTOs;

public class PaginatedOffersDto
{
    public List<OfferDto> Offers { get; set; } = new List<OfferDto>();
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}