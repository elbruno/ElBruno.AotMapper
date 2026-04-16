using EfProjectionSample;
using Microsoft.EntityFrameworkCore;

// Setup database
using var db = new OrderDbContext();
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

// Seed data
db.Orders.AddRange(
    new Order { CustomerName = "Bruno Capuano", TotalAmount = 299.99m, Status = OrderStatus.Delivered, OrderDate = DateTime.Now.AddDays(-5) },
    new Order { CustomerName = "Jane Smith", TotalAmount = 149.50m, Status = OrderStatus.Shipped, OrderDate = DateTime.Now.AddDays(-2) },
    new Order { CustomerName = "John Doe", TotalAmount = 599.00m, Status = OrderStatus.Processing, OrderDate = DateTime.Now.AddDays(-1) }
);
await db.SaveChangesAsync();

Console.WriteLine("=== EF Core Projection Sample ===\n");

// Example 1: In-memory mapping (load then map)
Console.WriteLine("1. In-memory mapping:");
var orders = await db.Orders.ToListAsync();
var dtos = orders.Select(o => o.ToOrderDto());

foreach (var dto in dtos)
{
    Console.WriteLine($"  Order #{dto.Id}: {dto.CustomerName} - ${dto.TotalAmount} ({dto.Status})");
}

Console.WriteLine("\n✅ EF Core projection works!");

// Example 2: Queryable with manual projection (translates to SQL)
Console.WriteLine("\n2. Queryable projection (SQL):");
var projectedOrders = await db.Orders
    .Where(o => o.TotalAmount > 100)
    .Select(o => new OrderDto(o.Id, o.CustomerName, o.TotalAmount, o.Status.ToString()))
    .ToListAsync();

foreach (var order in projectedOrders)
{
    Console.WriteLine($"  Order #{order.Id}: {order.CustomerName} - ${order.TotalAmount} ({order.Status})");
}
