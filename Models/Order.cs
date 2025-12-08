namespace WarehouseManagementSystem.Models;

public sealed class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public int CreatedByUserId { get; set; }
    public string PaymentMethod { get; set; }

    public Order(string customerName, int createdByUserId, string paymentMethod)
    {
        CustomerName = customerName;
        Items = new List<OrderItem>();
        CreatedByUserId = createdByUserId;
        PaymentMethod = paymentMethod;
        OrderDate = DateTime.Now;
        TotalAmount = 0;
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
        TotalAmount += item.TotalPrice;
    }
}
