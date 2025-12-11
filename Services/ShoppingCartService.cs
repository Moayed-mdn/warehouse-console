
using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public class ShoppingCartService : IShoppingCart
{
    private readonly List<OrderItem> _items = new();
    private readonly ProductService _productService;

    public ShoppingCartService(ProductService productService)
    {
        _productService = productService;
    }

    public void AddItem(Product product, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
       
        if (existingItem.ProductId != 0)
        {
            _items.Remove(existingItem);
            existingItem.Quantity += quantity;
            _items.Add(existingItem);
        }
        else
        {
            _items.Add(new OrderItem(product.Id, product.Name, product.Price, quantity));
        }
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item.ProductId != 0)
        {
            _items.Remove(item);
        }
    }

    public void ClearCart() => _items.Clear();

    public decimal CalculateTotal() => _items.Sum(i => i.TotalPrice);

    public List<OrderItem> GetItems() => _items.ToList();

    public int ItemCount => _items.Sum(i => i.Quantity);
   
    public bool IsEmpty => !_items.Any();
   
    public event EventHandler CartChanged;
}