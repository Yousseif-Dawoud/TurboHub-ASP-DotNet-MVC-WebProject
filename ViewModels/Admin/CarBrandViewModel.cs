namespace TurboHub.ViewModels.Admin;

public class CarBrandViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الماركة مطلوب")]
    [MaxLength(100)]
    public required string Name { get; set; }

    public string? LogoPath { get; set; }
    public bool IsActive { get; set; } = true;
    public int ModelsCount { get; set; }
}

public class CarModelViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الموديل مطلوب")]
    [MaxLength(100)]
    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;
    public int CarBrandId { get; set; }
    public string? CarBrandName { get; set; }
    public int EnginesCount { get; set; }
}

public class CarEngineViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "نوع المحرك مطلوب")]
    [MaxLength(50)]
    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;
    public int CarModelId { get; set; }
    public string? CarModelName { get; set; }
}