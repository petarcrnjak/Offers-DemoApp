namespace Application.Requests;

public class OfferProcessingRequest
{
    public OfferProcessingRequest(int offerId, int articleId, decimal unitPrice, string articleName, int quantity, DateTime date)
    {
        OfferId = offerId;
        ArticleId = articleId;
        ArticleName = articleName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Date = date;
    }

    public int OfferId { get; set; }
    public int ArticleId { get; set; }
    public decimal UnitPrice { get; set; }
    public string ArticleName { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
}