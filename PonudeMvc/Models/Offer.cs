namespace Ponude.Models;

public class Offer
{
    public int Id { get; set; }
    public string OfferNumber { get; set; }
    public DateTime OfferDate { get; set; }
    public List<OfferItem> OfferItems { get; set; }

    public decimal TotalAmount => OfferItems.Sum(item => item.TotalPrice);  // Calculate total amount for the offer
}