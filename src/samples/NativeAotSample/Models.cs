using ElBruno.AotMapper;

namespace NativeAotSample;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public List<string> Tags { get; set; } = new();
}

public enum ProductCategory { Library, Tool, Framework }

[MapFrom(typeof(Product))]
public partial record ProductDto(int Id, string Name, decimal Price, string Category, List<string> Tags);
