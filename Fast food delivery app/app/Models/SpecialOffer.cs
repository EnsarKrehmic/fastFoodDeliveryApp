using SQLite;

public class SpecialOffer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string ImageUrl { get; set; }
    public int ItemId { get; set; }
}
