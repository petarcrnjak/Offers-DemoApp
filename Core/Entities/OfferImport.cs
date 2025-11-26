namespace Core.Entities;

public class OfferImport
{
    public int OfferId { get; set; }
    public DateTime Date { get; set; }
    public int ArticleId { get; set; }
    public string Article { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int CellNumber { get; set; }
}