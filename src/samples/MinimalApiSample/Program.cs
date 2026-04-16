using MinimalApiSample.Models;
using ElBruno.AotMapper.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAotMapper();

var app = builder.Build();

// Sample data
var customers = new List<Customer>
{
    new() { Id = 1, FirstName = "Bruno", LastName = "Capuano", Email = "bruno@example.com", Tier = CustomerTier.Enterprise,
            Address = new() { Street = "123 Dev St", City = "Seattle", Country = "US" } },
    new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Tier = CustomerTier.Premium }
};

app.MapGet("/customers", () => customers.Select(c => c.ToCustomerDto()));
app.MapGet("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    return customer is null ? Results.NotFound() : Results.Ok(customer.ToCustomerDto());
});

app.Run();
