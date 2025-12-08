namespace WarehouseManagementSystem.Models;

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public string SKU { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Product(string name, string description, decimal price, int quantity, int categoryId, string sku)
    {
        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
        CategoryId = categoryId;
        SKU = sku;
        CreatedAt = DateTime.Now;
    }
}
