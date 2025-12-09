using System.Text.RegularExpressions;
using WarehouseManagementSystem.Exceptions;

namespace WarehouseManagementSystem.Utilities;

public static class Validator
{
    private static readonly Regex UsernameRegex = new Regex(@"^[a-zA-Z0-9_]{3,20}$");
    private static readonly Regex PasswordRegex = new Regex(@"^.{8,}$");
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex PhoneRegex = new Regex(@"^\+?[\d\s\-\(\)]{10,}$");
    private static readonly Regex FileNameRegex = new Regex(@"^[a-zA-Z0-9_\-]+\.(json|txt|csv)$");

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
}
