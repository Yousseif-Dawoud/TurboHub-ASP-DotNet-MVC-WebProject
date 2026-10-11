namespace TurboHub.Models;

public class ProductCompatibility
{
    public int Id { get; set; }

    // FK - Product
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // FK - CarBrand
    public int CarBrandId { get; set; }
    public CarBrand CarBrand { get; set; } = null!;

    // FK - CarModel
    public int CarModelId { get; set; }
    public CarModel CarModel { get; set; } = null!;

    // FK - CarEngine (optional, some parts don't care about engine type)
    public int? CarEngineId { get; set; }
    public CarEngine? CarEngine { get; set; }
}