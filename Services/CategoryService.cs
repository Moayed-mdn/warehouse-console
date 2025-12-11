using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class CategoryService : IRepository<Category>
{
    private readonly FileService<Category> _fileService;
    private List<Category> _categories;
    private int _nextId;

    public CategoryService()
    {
        _fileService = new FileService<Category>("Categories.json");
        _categories = _fileService.LoadData();
        _nextId = _categories.Count > 0 ? _categories.Max(c => c.Id) + 1 : 1;
    }

    public IEnumerable<Category> GetAll() => _categories;

    public Category GetById(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");
        return category;
    }

    public Category GetByName(string name)
    {
        var category = _categories.FirstOrDefault(c => 
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (category == null)
            throw new KeyNotFoundException($"Category with name '{name}' not found");
        return category;
    }

    public void Add(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new ValidationException("Category name cannot be empty");
        
        if (category.Name.Length < 2 || category.Name.Length > 100)
            throw new ValidationException("Category name must be between 2 and 100 characters");
        
        if (_categories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException($"Category '{category.Name}' already exists");

        category.Id = _nextId++;
        category.CreatedAt = DateTime.Now;
        _categories.Add(category);
        Save();
    }

    public void Update(Category updatedCategory)
    {
        var existingCategory = GetById(updatedCategory.Id);
        
        if (!existingCategory.Name.Equals(updatedCategory.Name, StringComparison.OrdinalIgnoreCase) &&
            _categories.Any(c => c.Id != updatedCategory.Id && 
                c.Name.Equals(updatedCategory.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException($"Category '{updatedCategory.Name}' already exists");
        }

        existingCategory.Name = updatedCategory.Name;
        existingCategory.Description = updatedCategory.Description;
        existingCategory.UpdatedAt = DateTime.Now;
        
        Save();
    }

    public void Delete(int id)
    {
        var category = GetById(id);
        
        
        _categories.Remove(category);
        Save();
    }

    public bool Exists(int id) => _categories.Any(c => c.Id == id);

    public bool ExistsByName(string name) => 
        _categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public int GetTotalCategories() => _categories.Count;

    public List<Category> SearchCategories(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return _categories;

        return _categories
            .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void ValidateCategoryId(int categoryId)
    {
        if (!Exists(categoryId))
            throw new ValidationException($"Category with ID {categoryId} does not exist");
    }

    private void Save()
    {
        try
        {
            _fileService.SaveData(_categories);
        }
        catch (Exception ex)
        {
            throw new WarehouseException($"Failed to save categories: {ex.Message}", ex);
        }
    }

    public List<Category> GetCategoriesCreatedBetween(DateTime startDate, DateTime endDate)
    {
        return _categories
            .Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate)
            .ToList();
    }

    public List<Category> GetRecentlyUpdatedCategories(int days)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        return _categories
            .Where(c => c.UpdatedAt.HasValue && c.UpdatedAt >= cutoffDate)
            .ToList();
    }
}