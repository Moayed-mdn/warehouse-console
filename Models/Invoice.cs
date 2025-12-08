namespace WarehouseManagementSystem.Models;

public sealed class Invoice
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public DateTime InvoiceDate { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
}
