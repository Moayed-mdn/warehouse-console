using System.Text;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Utilities;

public sealed class InvoiceBuilder
{
    private readonly StringBuilder _builder = new StringBuilder();

    public InvoiceBuilder AddHeader(string companyName, string address, string phone)
    {
        _builder.AppendLine("=".PadRight(50, '='));
        _builder.AppendLine(companyName.PadLeft(30));
        _builder.AppendLine(address.PadLeft(30));
        _builder.AppendLine(phone.PadLeft(30));
        _builder.AppendLine("=".PadRight(50, '='));
        _builder.AppendLine();
        return this;
    }

    public InvoiceBuilder AddCustomerInfo(string customerName, DateTime invoiceDate, int invoiceNumber)
    {
        _builder.AppendLine($"Customer: {customerName}");
        _builder.AppendLine($"Invoice #: {invoiceNumber}");
        _builder.AppendLine($"Date: {invoiceDate:yyyy-MM-dd HH:mm:ss}");
        _builder.AppendLine();
        return this;
    }

    public InvoiceBuilder AddItems(List<OrderItem> items)
    {
        _builder.AppendLine("Items:");
        _builder.AppendLine("-".PadRight(50, '-'));
        _builder.AppendLine($"{"Product",-30} {"Qty",5} {"Price",10} {"Total",12}");
        _builder.AppendLine("-".PadRight(50, '-'));

        foreach (var item in items)
        {
            _builder.AppendLine($"{item.ProductName,-30} {item.Quantity,5} {item.UnitPrice,10:C2} {item.TotalPrice,12:C2}");
        }

        _builder.AppendLine("-".PadRight(50, '-'));
        return this;
    }

    public InvoiceBuilder AddFooter(decimal totalAmount, string paymentMethod)
    {
        _builder.AppendLine();
        _builder.AppendLine($"Total Amount: {totalAmount:C2}");
        _builder.AppendLine($"Payment Method: {paymentMethod}");
        _builder.AppendLine("=".PadRight(50, '='));
        _builder.AppendLine("Thank you for your business!");
        _builder.AppendLine("=".PadRight(50, '='));
        return this;
    }

    public string Build()
    {
        return _builder.ToString();
    }
}
