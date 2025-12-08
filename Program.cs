using Spectre.Console;
using WarehouseManagementSystem.Enums;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Services;
using WarehouseManagementSystem.Utilities;
using WarehouseManagementSystem.Interfaces;

namespace WarehouseManagementSystem;

class Program
{
    private static AuthenticationService _authService;
    private static CategoryService _categoryService;
    private static ProductService _productService;
    private static OrderService _orderService;
    private static ReportService _reportService;

    static void Main(string[] args)
    {
        InitializeServices();
        ShowMainMenu();
    }

    private static void InitializeServices()
    {
        _authService = new AuthenticationService();
        _categoryService = new CategoryService();
        _productService = new ProductService();
        _orderService = new OrderService(_productService);
        _reportService = new ReportService(_orderService);
    }

    private static void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]Warehouse Management System[/]").LeftJustified());
            
            if (_authService.GetCurrentUser() == null)
            {
                ShowLoginMenu();
            }
            else
            {
                ShowDashboard();
            }
        }
    }

    private static void ShowLoginMenu()
    {
        AnsiConsole.Write(new Panel("[bold blue]Login Required[/]")
            .BorderColor(Color.Blue));

        var username = AnsiConsole.Ask<string>("[green]Username:[/]");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Password:[/]")
                .Secret());

        if (_authService.Login(username, password))
        {
            AnsiConsole.MarkupLine("[green]Login successful![/]");
            Thread.Sleep(1000);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Invalid credentials![/]");
            Thread.Sleep(1500);
        }
    }

    private static void ShowDashboard()
    {
        var currentUser = _authService.GetCurrentUser();
        var roleName = currentUser.Role.ToString();

        var menu = new SelectionPrompt<string>()
            .Title($"[bold yellow]Welcome, {currentUser.FullName} ({roleName})[/]")
            .PageSize(10)
            .AddChoices(new[]
            {
                "📦 Manage Products",
                "📁 Manage Categories",
                "🛒 Point of Sale (POS)",
                "📊 View Orders",
                "📈 Generate Reports",
                "👥 Manage Users",
                "🔓 Logout",
                "❌ Exit"
            });

        var choice = AnsiConsole.Prompt(menu);

        switch (choice)
        {
            case "📦 Manage Products":
                ManageProducts();
                break;
            case "📁 Manage Categories":
                ManageCategories();
                break;
            case "🛒 Point of Sale (POS)":
                PointOfSale();
                break;
            case "📊 View Orders":
                ViewOrders();
                break;
            case "📈 Generate Reports":
                GenerateReports();
                break;
            case "👥 Manage Users":
                ManageUsers();
                break;
            case "🔓 Logout":
                _authService.Logout();
                break;
            case "❌ Exit":
                Environment.Exit(0);
                break;
        }
    }

    private static void ManageProducts()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]Product Management[/]").LeftJustified());

            var menu = new SelectionPrompt<string>()
                .Title("[bold blue]Select Operation[/]")
                .AddChoices(new[]
                {
                    "View All Products",
                    "Add New Product",
                    "Update Product",
                    "Delete Product",
                    "Back to Main Menu"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "View All Products":
                    ViewAllProducts();
                    break;
                case "Add New Product":
                    AddNewProduct();
                    break;
                case "Update Product":
                    UpdateProduct();
                    break;
                case "Delete Product":
                    DeleteProduct();
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private static void ViewAllProducts()
    {
        var products = _productService.GetAll();
        
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Price");
        table.AddColumn("Quantity");
        table.AddColumn("Category");
        table.AddColumn("SKU");

        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Price.ToString("C2"),
                product.Quantity.ToString(),
                product.CategoryId.ToString(),
                product.SKU
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void AddNewProduct()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Add New Product[/]").LeftJustified());

        try
        {
            var name = AnsiConsole.Ask<string>("Product Name:");
            var description = AnsiConsole.Ask<string>("Description:");
            var price = AnsiConsole.Ask<decimal>("Price:");
            var quantity = AnsiConsole.Ask<int>("Quantity:");
            var categoryId = AnsiConsole.Ask<int>("Category ID:");
            var sku = AnsiConsole.Ask<string>("SKU:");

            var product = new Product(name, description, price, quantity, categoryId, sku);
            _productService.Add(product);

            AnsiConsole.MarkupLine("[green]Product added successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void UpdateProduct()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Update Product[/]").LeftJustified());

        try
        {
            var productId = AnsiConsole.Ask<int>("Enter Product ID to update:");
            var existingProduct = _productService.GetById(productId);

            AnsiConsole.MarkupLine($"Current Name: [blue]{existingProduct.Name}[/]");
            var newName = AnsiConsole.Prompt(
                new TextPrompt<string>("New Name (press Enter to keep current):")
                    .DefaultValue(existingProduct.Name)
                    .AllowEmpty());

            AnsiConsole.MarkupLine($"Current Price: [blue]{existingProduct.Price:C2}[/]");
            var newPrice = AnsiConsole.Prompt(
                new TextPrompt<decimal>("New Price (press Enter to keep current):")
                    .DefaultValue(existingProduct.Price)
                    .AllowEmpty());

            AnsiConsole.MarkupLine($"Current Quantity: [blue]{existingProduct.Quantity}[/]");
            var newQuantity = AnsiConsole.Prompt(
                new TextPrompt<int>("New Quantity (press Enter to keep current):")
                    .DefaultValue(existingProduct.Quantity)
                    .AllowEmpty());

            var updatedProduct = new Product(
                string.IsNullOrEmpty(newName) ? existingProduct.Name : newName,
                existingProduct.Description,
                newPrice == 0 ? existingProduct.Price : newPrice,
                newQuantity == 0 ? existingProduct.Quantity : newQuantity,
                existingProduct.CategoryId,
                existingProduct.SKU
            )
            {
                Id = existingProduct.Id
            };

            _productService.Update(updatedProduct);
            AnsiConsole.MarkupLine("[green]Product updated successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void DeleteProduct()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Delete Product[/]").LeftJustified());

        try
        {
            var productId = AnsiConsole.Ask<int>("Enter Product ID to delete:");
            
            // Confirm deletion
            if (!AnsiConsole.Confirm($"Are you sure you want to delete product #{productId}?"))
                return;

            _productService.Delete(productId);
            AnsiConsole.MarkupLine("[green]Product deleted successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void PointOfSale()
    {
        var cart = new ShoppingCart(_productService);
        
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]Point of Sale[/]").LeftJustified());

            var menu = new SelectionPrompt<string>()
                .Title("[bold blue]POS Menu[/]")
                .AddChoices(new[]
                {
                    "Scan Product",
                    "View Cart",
                    "Remove Item",
                    "Checkout",
                    "Back to Main Menu"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "Scan Product":
                    ScanProduct(cart);
                    break;
                case "View Cart":
                    ViewCart(cart);
                    break;
                case "Remove Item":
                    RemoveItemFromCart(cart);
                    break;
                case "Checkout":
                    Checkout(cart);
                    return;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private static void ScanProduct(ShoppingCart cart)
    {
        var productId = AnsiConsole.Ask<int>("Enter Product ID:");
        
        try
        {
            var product = _productService.GetById(productId);
            
            if (product.Quantity <= 0)
            {
                AnsiConsole.MarkupLine("[red]Product out of stock![/]");
                Thread.Sleep(1500);
                return;
            }

            var maxQuantity = Math.Min(product.Quantity, 100);
            var quantity = AnsiConsole.Prompt(
                new TextPrompt<int>($"Quantity (1-{maxQuantity}):")
                    .Validate(q => q <= maxQuantity ? ValidationResult.Success() 
                        : ValidationResult.Error($"[red]Maximum quantity is {maxQuantity}[/]")));

            cart.AddItem(product, quantity);
            AnsiConsole.MarkupLine($"[green]Added {quantity} x {product.Name} to cart[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void ViewCart(ShoppingCart cart)
    {
        var items = cart.GetItems();
        
        if (!items.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Cart is empty[/]");
        }
        else
        {
            var table = new Table();
            table.AddColumn("Product");
            table.AddColumn("Qty");
            table.AddColumn("Price");
            table.AddColumn("Total");

            foreach (var item in items)
            {
                table.AddRow(
                    item.ProductName,
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("C2"),
                    item.TotalPrice.ToString("C2")
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"[bold]Total: {cart.CalculateTotal():C2}[/]");
        }

        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void Checkout(ShoppingCart cart)
    {
        var items = cart.GetItems();
        
        if (!items.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Cart is empty[/]");
            Thread.Sleep(1500);
            return;
        }

        var customerName = AnsiConsole.Ask<string>("Customer Name:");
        
        var paymentMethods = new[] { "Cash", "Credit Card", "Debit Card", "Mobile Payment" };
        var paymentMethod = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Payment Method:")
                .AddChoices(paymentMethods));

        var currentUser = _authService.GetCurrentUser();
        var order = new Order(customerName, currentUser.Id, paymentMethod);

        foreach (var item in items)
        {
            order.AddItem(item);
        }

        try
        {
            _orderService.Add(order);
            
            // Generate invoice
            var invoiceBuilder = new InvoiceBuilder()
                .AddHeader("Warehouse Store", "123 Main St, City", "555-1234")
                .AddCustomerInfo(customerName, DateTime.Now, order.Id)
                .AddItems(items)
                .AddFooter(order.TotalAmount, paymentMethod);

            Console.Clear();
            AnsiConsole.Write(new Rule("[green]INVOICE[/]").LeftJustified());
            Console.WriteLine(invoiceBuilder.Build());
            
            cart.ClearCart();
            AnsiConsole.MarkupLine("[green]Order completed successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error during checkout: {ex.Message}[/]");
        }

        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void GenerateReports()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]Reports[/]").LeftJustified());

            var menu = new SelectionPrompt<string>()
                .Title("[bold blue]Select Report Type[/]")
                .AddChoices(new[]
                {
                    "Monthly Sales Report",
                    "Customer Invoices Report",
                    "Back to Main Menu"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "Monthly Sales Report":
                    ShowMonthlySalesReport();
                    break;
                case "Customer Invoices Report":
                    ShowCustomerInvoices();
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private static void ShowMonthlySalesReport()
    {
        var month = AnsiConsole.Ask<int>("Enter month (1-12):");
        var year = AnsiConsole.Ask<int>("Enter year:");

        var report = _reportService.GenerateMonthlySalesReport(month, year);
        
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]Monthly Sales Report[/]").LeftJustified());
        Console.WriteLine(report);
        
        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void ManageUsers()
    {
        if (!_authService.HasPermission(UserRole.SystemAdmin))
        {
            AnsiConsole.MarkupLine("[red]Access denied! System Admin only.[/]");
            Thread.Sleep(2000);
            return;
        }

        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]User Management[/]").LeftJustified());

            var menu = new SelectionPrompt<string>()
                .Title("[bold blue]Select Operation[/]")
                .AddChoices(new[]
                {
                    "View All Users",
                    "Create New User",
                    "Back to Main Menu"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "View All Users":
                    ViewAllUsers();
                    break;
                case "Create New User":
                    CreateNewUser();
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private static void ViewAllUsers()
    {
        try
        {
            var users = _authService.GetAllUsers();
            
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Username");
            table.AddColumn("Full Name");
            table.AddColumn("Role");
            table.AddColumn("Created");

            foreach (var user in users)
            {
                table.AddRow(
                    user.Id.ToString(),
                    user.Username,
                    user.FullName,
                    user.Role.ToString(),
                    user.CreatedAt.ToString("yyyy-MM-dd")
                );
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void CreateNewUser()
    {
        try
        {
            var username = AnsiConsole.Ask<string>("Username:");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Password:")
                    .Secret());
            var fullName = AnsiConsole.Ask<string>("Full Name:");
            
            var roles = Enum.GetValues<UserRole>();
            var role = AnsiConsole.Prompt(
                new SelectionPrompt<UserRole>()
                    .Title("Select Role:")
                    .AddChoices(roles));

            _authService.CreateUser(username, password, fullName, role);
            AnsiConsole.MarkupLine("[green]User created successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void ManageCategories()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]Category Management[/]").LeftJustified());

            var menu = new SelectionPrompt<string>()
                .Title("[bold blue]Select Operation[/]")
                .AddChoices(new[]
                {
                    "View All Categories",
                    "Add New Category",
                    "Update Category",
                    "Delete Category",
                    "Search Categories",
                    "Back to Main Menu"
                });

            var choice = AnsiConsole.Prompt(menu);

            switch (choice)
            {
                case "View All Categories":
                    ViewAllCategories();
                    break;
                case "Add New Category":
                    AddNewCategory();
                    break;
                case "Update Category":
                    UpdateCategory();
                    break;
                case "Delete Category":
                    DeleteCategory();
                    break;
                case "Search Categories":
                    SearchCategories();
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private static void ViewAllCategories()
    {
        var categories = _categoryService.GetAll();
        
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");
        table.AddColumn("Created");
        table.AddColumn("Updated");

        foreach (var category in categories)
        {
            table.AddRow(
                category.Id.ToString(),
                category.Name,
                category.Description,
                category.CreatedAt.ToString("yyyy-MM-dd"),
                category.UpdatedAt?.ToString("yyyy-MM-dd") ?? "Never"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[yellow]Total Categories: {categories.Count()}[/]");
        
        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void AddNewCategory()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Add New Category[/]").LeftJustified());

        try
        {
            var name = AnsiConsole.Ask<string>("Category Name:");
            var description = AnsiConsole.Ask<string>("Description:");

            var category = new Category(name, description);
            _categoryService.Add(category);

            AnsiConsole.MarkupLine("[green]Category added successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void UpdateCategory()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Update Category[/]").LeftJustified());

        try
        {
            var categoryId = AnsiConsole.Ask<int>("Enter Category ID to update:");
            var existingCategory = _categoryService.GetById(categoryId);

            AnsiConsole.MarkupLine($"Current Name: [blue]{existingCategory.Name}[/]");
            var newName = AnsiConsole.Prompt(
                new TextPrompt<string>("New Name (press Enter to keep current):")
                    .DefaultValue(existingCategory.Name)
                    .AllowEmpty());

            AnsiConsole.MarkupLine($"Current Description: [blue]{existingCategory.Description}[/]");
            var newDescription = AnsiConsole.Prompt(
                new TextPrompt<string>("New Description (press Enter to keep current):")
                    .DefaultValue(existingCategory.Description)
                    .AllowEmpty());

            var updatedCategory = new Category(
                string.IsNullOrEmpty(newName) ? existingCategory.Name : newName,
                string.IsNullOrEmpty(newDescription) ? existingCategory.Description : newDescription
            )
            {
                Id = existingCategory.Id
            };

            _categoryService.Update(updatedCategory);
            AnsiConsole.MarkupLine("[green]Category updated successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void DeleteCategory()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Delete Category[/]").LeftJustified());

        try
        {
            var categoryId = AnsiConsole.Ask<int>("Enter Category ID to delete:");
            
            if (!AnsiConsole.Confirm($"Are you sure you want to delete category #{categoryId}?"))
                return;

            _categoryService.Delete(categoryId);
            AnsiConsole.MarkupLine("[green]Category deleted successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        Thread.Sleep(1500);
    }

    private static void SearchCategories()
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]Search Categories[/]").LeftJustified());

        var searchTerm = AnsiConsole.Ask<string>("Enter search term:");
        var results = _categoryService.SearchCategories(searchTerm);

        if (!results.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No categories found[/]");
        }
        else
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Description");

            foreach (var category in results)
            {
                table.AddRow(
                    category.Id.ToString(),
                    category.Name,
                    category.Description
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"[yellow]Found {results.Count} categories[/]");
        }

        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void ViewOrders()
    {
        var orders = _orderService.GetAll();
        
        if (!orders.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No orders found[/]");
        }
        else
        {
            var table = new Table();
            table.AddColumn("Order ID");
            table.AddColumn("Customer");
            table.AddColumn("Date");
            table.AddColumn("Items");
            table.AddColumn("Total");
            table.AddColumn("Payment");

            foreach (var order in orders)
            {
                table.AddRow(
                    order.Id.ToString(),
                    order.CustomerName,
                    order.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    order.Items.Count.ToString(),
                    order.TotalAmount.ToString("C2"),
                    order.PaymentMethod
                );
            }

            AnsiConsole.Write(table);
        }

        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void ShowCustomerInvoices()
    {
        var customerName = AnsiConsole.Ask<string>("Enter customer name to search:");
        var report = _reportService.GenerateCustomerInvoicesReport(customerName);
        
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]Customer Invoices Report[/]").LeftJustified());
        Console.WriteLine(report);
        
        AnsiConsole.Prompt(
            new TextPrompt<string>("Press Enter to continue...")
                .AllowEmpty());
    }

    private static void RemoveItemFromCart(ShoppingCart cart)
    {
        var items = cart.GetItems();
        
        if (!items.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Cart is empty[/]");
            Thread.Sleep(1500);
            return;
        }

        var productIds = items.Select(i => i.ProductId).ToList();
        var productId = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Select product to remove:")
                .AddChoices(productIds));

        cart.RemoveItem(productId);
        AnsiConsole.MarkupLine("[green]Item removed from cart[/]");
        Thread.Sleep(1500);
    }
}

// Shopping Cart Implementation
public class ShoppingCart : IShoppingCart
{
    private readonly List<OrderItem> _items = new();
    private readonly ProductService _productService;

    public ShoppingCart(ProductService productService)
    {
        _productService = productService;
    }

    public void AddItem(Product product, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        
        if (existingItem.ProductId != 0)
        {
            _items.Remove(existingItem);
            existingItem.Quantity += quantity;
            _items.Add(existingItem);
        }
        else
        {
            _items.Add(new OrderItem(product.Id, product.Name, product.Price, quantity));
        }
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item.ProductId != 0)
        {
            _items.Remove(item);
        }
    }

    public void ClearCart() => _items.Clear();

    public decimal CalculateTotal() => _items.Sum(i => i.TotalPrice);

    public List<OrderItem> GetItems() => _items.ToList();

    public int ItemCount => _items.Sum(i => i.Quantity);
    
    public bool IsEmpty => !_items.Any();
    
    public event EventHandler CartChanged;
}