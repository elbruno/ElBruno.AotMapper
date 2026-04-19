using ElBruno.AotMapper;
using ElBruno.AotMapper.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ElBruno.AotMapper.EntityFramework.Tests;

// Test entities
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
}

public enum ProductStatus { Active, Discontinued }

[MapFrom(typeof(Product))]
public partial record ProductDto(int Id, string Name, decimal Price, string Status);

// DbContext for tests
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    
    public DbSet<Product> Products => Set<Product>();
}

public class ApplyProjectionTests
{
    [Fact]
    public void ApplyProjection_Helper_Works()
    {
        // Arrange
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();
        
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new TestDbContext(options);
        context.Database.EnsureCreated();
        
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product A", Price = 10.99m, Status = ProductStatus.Active },
            new Product { Id = 2, Name = "Product B", Price = 20.50m, Status = ProductStatus.Discontinued }
        );
        context.SaveChanges();

        // Act - Using the ApplyProjection helper
        var projection = context.Products.AsQueryable().ApplyProjection(
            p => new ProductDto(p.Id, p.Name, p.Price, p.Status.ToString())
        );
        var results = projection.ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("Product A", results[0].Name);
        Assert.Equal("Active", results[0].Status);
        
        connection.Close();
    }
}

// Note: ProjectTo tests will be added once the generator emits ProjectTo methods
public class ProjectToTests
{
    [Fact(Skip = "ProjectTo generation not yet implemented - waiting for generator build fix")]
    public void ProjectTo_Translates_To_SQL()
    {
        // This test will verify that ProjectToProductDto() method is generated
        // and works with EF Core SQL translation
    }

    [Fact(Skip = "ProjectTo generation not yet implemented - waiting for generator build fix")]
    public void ProjectTo_With_Nested_Mapping()
    {
        // This test will verify nested DTO projection works
    }
}
