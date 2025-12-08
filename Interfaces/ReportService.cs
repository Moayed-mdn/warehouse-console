using System.Text;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Utilities;

namespace WarehouseManagementSystem.Services;

public sealed class ReportService
{
    private readonly OrderService _orderService;

    public ReportService(OrderService orderService)
    {
        _orderService = orderService;
    }

    public string GenerateMonthlySalesReport(int month, int year)
    {
        var orders = _orderService.GetAll();
        var monthlyOrders = orders.Where(o => 
            o.OrderDate.Month == month && 
            o.OrderDate.Year == year).ToList();
        
        var reportBuilder = new ReportBuilder()
            .AddTitle($"Monthly Sales Report - {month}/{year}")
            .AddMonthlySalesReport(monthlyOrders, month, year);
        
        return reportBuilder.Build();
    }

    public string GenerateCustomerInvoicesReport(string customerName)
    {
        var orders = _orderService.GetAll();
        var customerOrders = orders.Where(o => 
            o.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
        
        var reportBuilder = new ReportBuilder()
            .AddTitle($"Customer Invoices Report - {customerName}")
            .AddCustomerInvoices(customerName, customerOrders);
        
        return reportBuilder.Build();
    }

    public string GenerateInventoryReport()
    {
        var orders = _orderService.GetAll();
        var recentOrders = orders.Where(o => 
            o.OrderDate >= DateTime.Now.AddMonths(-1)).ToList();
        
        var totalRevenue = recentOrders.Sum(o => o.TotalAmount);
        var totalOrders = recentOrders.Count;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        
        var report = new StringBuilder();
        report.AppendLine("=".PadRight(60, '='));
        report.AppendLine("INVENTORY REPORT".PadLeft(40));
        report.AppendLine("=".PadRight(60, '='));
        report.AppendLine();
        report.AppendLine($"Report Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Total Orders (Last 30 Days): {totalOrders}");
        report.AppendLine($"Total Revenue: {totalRevenue:C2}");
        report.AppendLine($"Average Order Value: {averageOrderValue:C2}");
        report.AppendLine("=".PadRight(60, '='));
        
        return report.ToString();
    }
}