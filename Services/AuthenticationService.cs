using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class AuthenticationService
{
    private readonly FileService<User> _fileService;
    private List<User> _users;
    private User? _currentUser;

    public AuthenticationService()
    {
        _fileService = new FileService<User>("Users.json");
        _users = _fileService.LoadData();
        
        // Add default admin if no users exist
        if (_users.Count == 0)
        {
            var admin = new User("admin", "Admin123", "System Administrator", Enums.UserRole.SystemAdmin);
            admin.Id = 1;
            _users.Add(admin);
            _fileService.SaveData(_users);
        }
    }

    public bool Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
            u.Password == password && 
            u.IsActive);

        if (user != null)
        {
            _currentUser = user;
            return true;
        }

        return false;
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public User? GetCurrentUser() => _currentUser;

    public bool HasPermission(Enums.UserRole requiredRole)
    {
        return _currentUser != null && _currentUser.Role <= requiredRole;
    }

    public void CreateUser(string username, string password, string fullName, Enums.UserRole role)
    {
        if (!HasPermission(Enums.UserRole.SystemAdmin))
            throw new UnauthorizedAccessException("Only system admin can create users");

        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Username already exists");

        var newUser = new User(username, password, fullName, role);
        newUser.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
        
        _users.Add(newUser);
        _fileService.SaveData(_users);
    }

    public List<User> GetAllUsers()
    {
        if (!HasPermission(Enums.UserRole.SystemAdmin))
            throw new UnauthorizedAccessException("Only system admin can view all users");

        return _users;
    }
}