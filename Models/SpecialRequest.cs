
namespace TurboHub.Models;

public class SpecialRequest : BaseEntity
{
    [MaxLength(20)]
    public string? RequestNumber { get; set; }

    // Car info
    [MaxLength(100)]
    public string? CarBrand { get; set; }

    [MaxLength(100)]
    public string? CarModel { get; set; }

    public string? CarEngine { get; set; }   // Free text "1.6" ← ADD THIS

    public int? CarYear { get; set; }

    // Part info
    [MaxLength(200)]
    public required string PartName { get; set; }

    [MaxLength(100)]
    public string? OEMNumber { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    // Contact info
    [MaxLength(150)]
    public required string ContactName { get; set; }

    // we need add adress

    [MaxLength(20)]
    public required string ContactPhone { get; set; }

    [MaxLength(150)]
    public string? ContactEmail { get; set; }

    public SpecialRequestStatus Status { get; set; } = SpecialRequestStatus.New;

    public string? AdminNotes { get; set; }

    // FK — nullable because guests can also submit requests
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}