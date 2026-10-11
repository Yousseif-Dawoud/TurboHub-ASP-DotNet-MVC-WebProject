
namespace TurboHub.Models;

public class CarEngine
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }  // e.g. "1.3", "1.6", "V6"

    public bool IsActive { get; set; } = true;

    // FK
    public int CarModelId { get; set; }
    public CarModel CarModel { get; set; } = null!;

    // Navigation
    public ICollection<ProductCompatibility> ProductCompatibilities { get; set; } = new List<ProductCompatibility>();
}