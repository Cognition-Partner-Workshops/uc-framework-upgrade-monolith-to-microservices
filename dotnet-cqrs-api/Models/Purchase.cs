namespace CqrsApi.Models;

public class Purchase
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
