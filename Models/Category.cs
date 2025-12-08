namespace WarehouseManagementSystem.Models;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Category(string name, string description)
    {
        Name = name;
        Description = description;
        CreatedAt = DateTime.Now;
    }
}
