using ElBruno.AotMapper;

namespace MinimalApiSample.Models;

[MapFrom(typeof(Customer))]
[MapProperty(nameof(Customer.FirstName), nameof(Name))]
public partial record CustomerDto(int Id, string Name, string Email, string Tier);

[MapFrom(typeof(Address))]
public partial record AddressDto(string Street, string City, string Country);
