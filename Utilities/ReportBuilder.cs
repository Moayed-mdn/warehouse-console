using System.Text;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Utilities;

public sealed class ReportBuilder
{
    private readonly StringBuilder _builder = new StringBuilder();

    public ReportBuilder AddTitle(string title)
    {
        _builder.AppendLine();
        _builder.AppendLine("=".PadRight(60, '='));
        _builder.AppendLine(title.ToUpper().PadLeft(40));
        _builder.AppendLine("=".PadRight(60, '='));
        _builder.AppendLine();
        return this;
    }

    public ReportBuilder AddSection(string sectionName)
    {
        _builder.AppendLine();
        _builder.AppendLine(sectionName);
        _builder.AppendLine("-".PadRight(60, '-'));
        return this;
    }

    public ReportBuilder AddMonthlySalesReport(List<Order> orders, int month, int year)
    {
        var monthlyOrders = orders.Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year).ToList();
        decimal totalProfit = monthlyOrders.Sum(o => o.TotalAmount);

        _builder.AppendLine($"Month: {month}/{year}");
        _builder.AppendLine($"Total Orders: {monthlyOrders.Count}");
        _builder.AppendLine($"Total Revenue: {totalProfit:C2}");
        _builder.AppendLine();

        _builder.AppendLine("Order Details:");
        _builder.AppendLine("-".PadRight(60, '-'));
        _builder.AppendLine($"{"Order ID",-10} {"Customer",-20} {"Date",-20} {"Amount",-10}");
        _builder.AppendLine("-".PadRight(60, '-'));

        foreach (var order in monthlyOrders)
        {
            _builder.AppendLine($"{order.Id,-10} {order.CustomerName,-20} {order.OrderDate:yyyy-MM-dd HH:mm,-20} {order.TotalAmount,-10:C2}");
        }

        return this;
    }

    public ReportBuilder AddCustomerInvoices(string customerName, List<Order> orders)
    {
        var customerOrders = orders.Where(o => o.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase)).ToList();

        _builder.AppendLine($"Customer: {customerName}");
        _builder.AppendLine($"Total Invoices: {customerOrders.Count}");
        _builder.AppendLine($"Total Spent: {customerOrders.Sum(o => o.TotalAmount):C2}");
        _builder.AppendLine();

        _builder.AppendLine("Invoice Details:");
        _builder.AppendLine("-".PadRight(60, '-'));
        _builder.AppendLine($"{"Invoice #",-10} {"Date",-20} {"Items",-10} {"Amount",-10}");
        _builder.AppendLine("-".PadRight(60, '-'));

        foreach (var order in customerOrders)
        {
            _builder.AppendLine($"{order.Id,-10} {order.OrderDate:yyyy-MM-dd HH:mm,-20} {order.Items.Count,-10} {order.TotalAmount,-10:C2}");
        }

        return this;
    }

    public string Build()
    {
        return _builder.ToString();
    }
}
