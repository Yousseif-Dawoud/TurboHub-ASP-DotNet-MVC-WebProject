namespace TurboHub.ViewModels.Admin;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم القسم مطلوب")]
    [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
    public required string Name { get; set; }

    [MaxLength(50)]
    public string? Icon { get; set; }

    public int? ParentId { get; set; }

    public int ProductCount { get; set; }
    public bool IsDeleted { get; set; }
}