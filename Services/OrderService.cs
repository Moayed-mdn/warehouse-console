using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class OrderService : IRepository<Order>, IPaymentProcessor
{
    private readonly FileService<Order> _fileService;
    private readonly ProductService _productService;
    private List<Order> _orders;
    private int _nextId;

    public OrderService(ProductService productService)
    {
        _fileService = new FileService<Order>("Orders.json");
        _productService = productService;
        _orders = _fileService.LoadData();
        _nextId = _orders.Count > 0 ? _orders.Max(o => o.Id) + 1 : 1;
    }

    public IEnumerable<Order> GetAll() => _orders;

    public Order GetById(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");
        return order;
    }

    public void Add(Order order)
    {
        // Update stock for each item
        foreach (var item in order.Items)
        {
            _productService.UpdateStock(item.ProductId, -item.Quantity);
        }

        order.Id = _nextId++;
        order.OrderDate = DateTime.Now;
        _orders.Add(order);
        Save();
    }

    public void Update(Order order)
    {
        var existingOrder = GetById(order.Id);
        
        // Return old stock and deduct new stock
        foreach (var oldItem in existingOrder.Items)
        {
            _productService.UpdateStock(oldItem.ProductId, oldItem.Quantity);
        }
        
        foreach (var newItem in order.Items)
        {
            _productService.UpdateStock(newItem.ProductId, -newItem.Quantity);
        }

        existingOrder.CustomerName = order.CustomerName;
        existingOrder.Items = order.Items;
        existingOrder.TotalAmount = order.TotalAmount;
        existingOrder.PaymentMethod = order.PaymentMethod;

        Save();
    }

    public void Delete(int id)
    {
        var order = GetById(id);
        
        // Return stock
        foreach (var item in order.Items)
        {
            _productService.UpdateStock(item.ProductId, item.Quantity);
        }

        _orders.Remove(order);
        Save();
    }

    public bool Exists(int id) => _orders.Any(o => o.Id == id);

    public bool ProcessPayment(decimal amount, string paymentMethod)
    {
        // Simulate payment processing
        return !string.IsNullOrEmpty(paymentMethod) && amount > 0;
    }

    public string GenerateReceipt(decimal amount, string paymentMethod)
    {
        return $"Payment of {amount:C2} processed via {paymentMethod} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    }

    public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
    {
        return _orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).ToList();
    }

    public List<Order> GetOrdersByCustomer(string customerName)
    {
        return _orders.Where(o => o.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private void Save() => _fileService.SaveData(_orders);
}