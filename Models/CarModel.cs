
namespace TurboHub.Models;

public class CarModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;

    // FK
    public int CarBrandId { get; set; }
    public CarBrand CarBrand { get; set; } = null!;

    // Navigation
    public ICollection<CarEngine> Engines { get; set; } = new List<CarEngine>();
}