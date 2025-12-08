namespace WarehouseManagementSystem.Interfaces;

public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount, string paymentMethod);
    string GenerateReceipt(decimal amount, string paymentMethod);
}
