
namespace TurboHub.Models;

public class Category : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(50)]
    public string? Icon { get; set; }

    // Self-referencing for subcategories (optional)
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}