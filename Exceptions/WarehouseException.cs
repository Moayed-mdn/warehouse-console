namespace WarehouseManagementSystem.Exceptions;

public class WarehouseException : Exception
{
    public WarehouseException() { }
    public WarehouseException(string message) : base(message) { }
    public WarehouseException(string message, Exception inner) : base(message, inner) { }
}

public class ValidationException : WarehouseException
{
    public ValidationException(string message) : base(message) { }
}

public class InsufficientStockException : WarehouseException
{
    public InsufficientStockException(string productName, int requested, int available) 
        : base($"Insufficient stock for {productName}. Requested: {requested}, Available: {available}") { }
}
