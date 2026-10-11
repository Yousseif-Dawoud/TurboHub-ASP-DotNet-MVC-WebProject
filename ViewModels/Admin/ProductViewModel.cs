using System.ComponentModel.DataAnnotations;
using TurboHub.Models;

namespace TurboHub.ViewModels.Admin;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public string? SKU { get; set; }

    [MaxLength(100)]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(0, 9999999, ErrorMessage = "السعر غير صحيح")]
    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    [Required(ErrorMessage = "الكمية مطلوبة")]
    [Range(0, 99999)]
    public int StockQuantity { get; set; }

    public int LowStockThreshold { get; set; } = 5;

    public string? Description { get; set; }

    [MaxLength(50)]
    public string? WarrantyPeriod { get; set; }

    public ProductCondition Condition { get; set; } = ProductCondition.New;

    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;

    [Required(ErrorMessage = "القسم مطلوب")]
    public int CategoryId { get; set; }

    // Main image
    public string? MainImagePath { get; set; }
    public string? MainImageUrl { get; set; }
    public IFormFile? MainImageFile { get; set; }

    // Extra images
    public List<IFormFile>? ExtraImageFiles { get; set; }
    public List<string>? ExtraImageUrls { get; set; }

    // Compatible cars
    public List<CompatibilityItemViewModel> Compatibilities { get; set; } = new();
}

public class CompatibilityItemViewModel
{
    public int CarBrandId { get; set; }
    public string? CarBrandName { get; set; }
    public int CarModelId { get; set; }
    public string? CarModelName { get; set; }
    public int? CarEngineId { get; set; }
    public string? CarEngineName { get; set; }
}

public class ProductListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public string? MainImage { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public ProductCondition Condition { get; set; }
    public DateTime CreatedAt { get; set; }
}