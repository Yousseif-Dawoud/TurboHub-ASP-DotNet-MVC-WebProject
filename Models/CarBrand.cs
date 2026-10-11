namespace TurboHub.Models;

public class CarBrand
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    public string? LogoPath { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
}