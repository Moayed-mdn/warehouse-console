using System.Text.Json;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class FileService<T> where T : class
{
    private readonly string _filePath;

    public FileService(string fileName)
    {
        if (!Validator.IsValidFileName(fileName))
            throw new ArgumentException("Invalid file name");

        _filePath = Path.Combine("Data", fileName);
        
        // Create directory if it doesn't exist
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create file if it doesn't exist
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public List<T> LoadData()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading data from {_filePath}: {ex.Message}");
        }
    }

    public void SaveData(List<T> data)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error saving data to {_filePath}: {ex.Message}");
        }
    }
}