using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class ProductService : IRepository<Product>
{
    private readonly FileService<Product> _fileService;
    private List<Product> _products;
    private int _nextId;

    public ProductService()
    {
        _fileService = new FileService<Product>("Products.json");
        _products = _fileService.LoadData();
        _nextId = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
    }

    public IEnumerable<Product> GetAll() => _products;

    public Product GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");
        return product;
    }

    public void Add(Product product)
    {
        Validator.ValidatePositiveDecimal("Price", product.Price);
        Validator.ValidatePositiveInteger("Quantity", product.Quantity);

        product.Id = _nextId++;
        product.CreatedAt = DateTime.Now;
        _products.Add(product);
        Save();
    }

    public void Update(Product updatedProduct)
    {
        var existingProduct = GetById(updatedProduct.Id);
        
        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Quantity = updatedProduct.Quantity;
        existingProduct.CategoryId = updatedProduct.CategoryId;
        existingProduct.SKU = updatedProduct.SKU;
        existingProduct.UpdatedAt = DateTime.Now;

        Save();
    }

    public void Delete(int id)
    {
        var product = GetById(id);
        _products.Remove(product);
        Save();
    }

    public bool Exists(int id) => _products.Any(p => p.Id == id);

    public void UpdateStock(int productId, int quantityChange)
    {
        var product = GetById(productId);
        
        if (product.Quantity + quantityChange < 0)
            throw new InsufficientStockException(product.Name, Math.Abs(quantityChange), product.Quantity);

        product.Quantity += quantityChange;
        product.UpdatedAt = DateTime.Now;
        Save();
    }

    private void Save() => _fileService.SaveData(_products);
}