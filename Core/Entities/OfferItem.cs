namespace Core.Entities;

public class OfferItem
{
    public int Id { get; set; }
    public string Article { get; set; }
    public decimal UnitPrice { get; set; }

    public ICollection<OfferDetails> OfferDetails { get; set; } // Many-to-many relationship
}