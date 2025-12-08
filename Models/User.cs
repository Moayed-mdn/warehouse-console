using System.Text.RegularExpressions;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Utilities; 
namespace WarehouseManagementSystem.Models;

public sealed class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public User(string username, string password, string fullName, UserRole role)
    {
        if (!Validator.IsValidUsername(username))
            throw new ArgumentException("Invalid username format");
        
        if (!Validator.IsValidPassword(password))
            throw new ArgumentException("Password must be at least 8 characters");
        
        Username = username;
        Password = password;
        FullName = fullName;
        Role = role;
        CreatedAt = DateTime.Now;
        IsActive = true;
    }
}
