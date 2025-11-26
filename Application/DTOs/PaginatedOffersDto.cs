namespace Application.DTOs;

public class PaginatedOffersDto
{
    public List<OfferDto>? Offers { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}