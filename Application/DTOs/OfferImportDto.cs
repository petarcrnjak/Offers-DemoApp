namespace Application.DTOs;

public class OfferImportDto
{
    public int RowNumber { get; set; }
    public int OfferId { get; set; }
    public DateTime OfferDate { get; set; }
    public int? ArticleId { get; set; }
    public string? Article { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? Quantity { get; set; }
}