using NativeAotSample;

var product = new Product
{
    Id = 42,
    Name = "ElBruno.AotMapper",
    Price = 0.0m,
    Category = ProductCategory.Library,
    Tags = new List<string> { "aot", "mapper", "dotnet" }
};

var dto = product.ToProductDto();
Console.WriteLine($"Product: {dto.Name} (#{dto.Id})");
Console.WriteLine($"  Price: {dto.Price:C}");
Console.WriteLine($"  Category: {dto.Category}");
Console.WriteLine($"  Tags: {string.Join(", ", dto.Tags)}");
Console.WriteLine("\n✅ AOT mapping works!");
