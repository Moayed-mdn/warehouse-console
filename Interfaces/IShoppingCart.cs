using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Interfaces;

public interface IShoppingCart
{
    void AddItem(Product product, int quantity);
    void RemoveItem(int productId);
    void ClearCart();
    decimal CalculateTotal();
    List<OrderItem> GetItems();
    int ItemCount { get; }
    bool IsEmpty { get; }
    
    event EventHandler CartChanged;
}