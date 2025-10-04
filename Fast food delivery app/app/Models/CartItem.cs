using SQLite;

public class CartItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string ImageUrl { get; set; }
    public int ItemId { get; set; }
    public int SpecialOfferId { get; set; }
}