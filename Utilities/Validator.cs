using System.Text.RegularExpressions;
using WarehouseManagementSystem.Exceptions;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Utilities;

public static class Validator
{
    private static readonly Regex UsernameRegex = new Regex(@"^[a-zA-Z0-9_]{3,20}$");
<<<<<<< HEAD
    private static readonly Regex PasswordRegex = new Regex(@"^.{8,20}$");
=======
    private static readonly Regex PasswordRegex = new Regex(@"^.{8,}$");
>>>>>>> 5687eafa6d0521c37ad506dfc58a65f886cf82ee
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex PhoneRegex = new Regex(@"^\+?[\d\s\-\(\)]{10,}$");
    private static readonly Regex FileNameRegex = new Regex(@"^[a-zA-Z0-9_\-]+\.(json|txt|csv)$"); 
    private static readonly Regex SKURegex = new Regex(@"^[A-Z0-9\-]+$");

public static bool IsValidUsername(string username) 
    {
        return !string.IsNullOrWhiteSpace(username) && UsernameRegex.IsMatch(username);
    }

    public static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && PasswordRegex.IsMatch(password);
    }

    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    public static bool IsValidPhone(string phone)
    {
        return !string.IsNullOrWhiteSpace(phone) && PhoneRegex.IsMatch(phone);
    }

    public static bool IsValidFileName(string fileName)
    {
        return !string.IsNullOrWhiteSpace(fileName) && FileNameRegex.IsMatch(fileName);
    }

    public static void ValidatePositiveDecimal(string fieldName, decimal value)
    {
        if (value <= 0)
            throw new ValidationException($"{fieldName} must be greater than 0");
    }

    public static void ValidatePositiveInteger(string fieldName, int value)
    {
        if (value <= 0)
            throw new ValidationException($"{fieldName} must be greater than 0");
    }

    public static void ValidateCategoryExists(int categoryId, CategoryService categoryService)
    {
        if (categoryService == null)
            throw new ArgumentNullException(nameof(categoryService));
        
        if (!categoryService.Exists(categoryId))
            throw new ValidationException($"Category with ID {categoryId} does not exist");
    }

        

    public static void ValidateSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ValidationException("SKU cannot be empty");
        
        if (sku.Length < 3 || sku.Length > 20)
            throw new ValidationException("SKU must be between 3 and 20 characters");
        
        if (!SKURegex.IsMatch(sku))
            throw new ValidationException("SKU can only contain uppercase letters, numbers, and hyphens");
    }

    public static void ValidateProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ValidationException("Product name cannot be empty");
        
        if (productName.Length < 2 || productName.Length > 100)
            throw new ValidationException("Product name must be between 2 and 100 characters");
    }

}
