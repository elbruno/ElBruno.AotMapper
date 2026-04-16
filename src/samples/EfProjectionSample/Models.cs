using ElBruno.AotMapper;
using Microsoft.EntityFrameworkCore;

namespace EfProjectionSample;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
}

public enum OrderStatus { Pending, Processing, Shipped, Delivered }

[MapFrom(typeof(Order))]
public partial record OrderDto(int Id, string CustomerName, decimal TotalAmount, string Status);

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=orders.db");
    }
}
