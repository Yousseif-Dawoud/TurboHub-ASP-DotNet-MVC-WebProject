
namespace TurboHub.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(150)]
    public string? FullName { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<SpecialRequest> SpecialRequests { get; set; } = new List<SpecialRequest>();
}