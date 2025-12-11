using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class ProductService : IRepository<Product>
{
    private readonly FileService<Product> _fileService;
    private readonly CategoryService _categoryService;
    private List<Product> _products;
    private int _nextId;

    public ProductService(CategoryService categoryService)
    {
        _fileService = new FileService<Product>("Products.json");
        _categoryService = categoryService;
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
        Validator.ValidateCategoryExists(product.CategoryId, _categoryService);
        Validator.ValidateSKU(product.SKU);
        product.Id = _nextId++;
        product.CreatedAt = DateTime.Now;
          //    SKU تحقق من عدم تكرار 
        if (_products.Any(p => p.SKU.Equals(product.SKU, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException($"Product with SKU '{product.SKU}' already exists");

        _products.Add(product);
        Save();
    }

    public void Update(Product updatedProduct)
    {
        var existingProduct = GetById(updatedProduct.Id);
    
        Validator.ValidateProductName(updatedProduct.Name);
        Validator.ValidatePositiveDecimal("Price", updatedProduct.Price);
        Validator.ValidatePositiveInteger("Quantity", updatedProduct.Quantity);
        Validator.ValidateCategoryExists(updatedProduct.CategoryId, _categoryService); 
        Validator.ValidateSKU(updatedProduct.SKU);

        if (_products.Any(p => p.Id != updatedProduct.Id && 
            p.SKU.Equals(updatedProduct.SKU, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException($"Product with SKU '{updatedProduct.SKU}' already exists");

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