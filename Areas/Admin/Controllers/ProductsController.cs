namespace TurboHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductsController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: /Admin/Products
    public async Task<IActionResult> Index(string? search, int? categoryId,
        bool? lowStock, bool? inactive, int page = 1)
    {
        int pageSize = 10;

        var query = _context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Name.Contains(search) ||
                (p.SKU != null && p.SKU.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (lowStock == true)
            query = query.Where(p => p.StockQuantity <= p.LowStockThreshold);

        if (inactive == true)
            query = query.Where(p => !p.IsActive);

        var total = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Brand = p.Brand,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                MainImage = p.MainImagePath ?? p.MainImageUrl,
                CategoryName = p.Category.Name,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                Condition = p.Condition,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.LowStock = lowStock;
        ViewBag.Inactive = inactive;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Total = total;
        ViewBag.Categories = new SelectList(
            await _context.Categories
                .IgnoreQueryFilters()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "Id", "Name");

        return View(products);
    }

    // GET: /Admin/Products/Create
    public async Task<IActionResult> Create()
    {
        await LoadFormDataAsync();

        // Auto generate SKU
        var lastProduct = await _context.Products
            .IgnoreQueryFilters()
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        int nextId = (lastProduct?.Id ?? 0) + 1;
        string autoSku = $"TRB-{nextId:D4}";

        return View(new ProductViewModel
        {
            Name = string.Empty,
            SKU = autoSku
        });
    }

    // POST: /Admin/Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model,
        List<string>? compatBrandIds,
        List<string>? compatModelIds,
        List<string>? compatEngineIds)
    {
        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync();
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name,
            SKU = model.SKU,
            Brand = model.Brand,
            Price = model.Price,
            DiscountPrice = model.DiscountPrice,
            StockQuantity = model.StockQuantity,
            LowStockThreshold = model.LowStockThreshold,
            Description = model.Description,
            WarrantyPeriod = model.WarrantyPeriod,
            Condition = model.Condition,
            IsActive = model.IsActive,
            IsFeatured = model.IsFeatured,
            CategoryId = model.CategoryId
        };

        // Handle main image
        if (model.MainImageFile != null && model.MainImageFile.Length > 0)
        {
            product.MainImagePath = await SaveImageAsync(model.MainImageFile);
        }
        else if (!string.IsNullOrWhiteSpace(model.MainImageUrl))
        {
            product.MainImageUrl = model.MainImageUrl;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Handle extra images
        if (model.ExtraImageFiles != null)
        {
            foreach (var file in model.ExtraImageFiles
                .Where(f => f.Length > 0))
            {
                var path = await SaveImageAsync(file);
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImagePath = path,
                    IsMain = false
                });
            }
        }

        if (model.ExtraImageUrls != null)
        {
            foreach (var url in model.ExtraImageUrls
                .Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = url,
                    IsMain = false
                });
            }
        }

        // Handle compatibility
        if (compatBrandIds != null)
        {
            for (int i = 0; i < compatBrandIds.Count; i++)
            {
                if (!int.TryParse(compatBrandIds[i], out int brandId)) continue;
                if (!int.TryParse(compatModelIds?[i], out int modelId)) continue;

                int? engineId = null;
                if (compatEngineIds != null &&
                    i < compatEngineIds.Count &&
                    int.TryParse(compatEngineIds[i], out int eid))
                    engineId = eid;

                _context.ProductCompatibilities.Add(new ProductCompatibility
                {
                    ProductId = product.Id,
                    CarBrandId = brandId,
                    CarModelId = modelId,
                    CarEngineId = engineId
                });
            }
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم إضافة المنتج بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Products/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images)
            .Include(p => p.Compatibilities)
                .ThenInclude(c => c.CarBrand)
            .Include(p => p.Compatibilities)
                .ThenInclude(c => c.CarModel)
            .Include(p => p.Compatibilities)
                .ThenInclude(c => c.CarEngine)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Brand = product.Brand,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            Description = product.Description,
            WarrantyPeriod = product.WarrantyPeriod,
            Condition = product.Condition,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId,
            MainImagePath = product.MainImagePath,
            MainImageUrl = product.MainImageUrl,
            Compatibilities = product.Compatibilities.Select(c =>
                new CompatibilityItemViewModel
                {
                    CarBrandId = c.CarBrandId,
                    CarBrandName = c.CarBrand.Name,
                    CarModelId = c.CarModelId,
                    CarModelName = c.CarModel.Name,
                    CarEngineId = c.CarEngineId,
                    CarEngineName = c.CarEngine?.Name
                }).ToList()
        };

        ViewBag.ExtraImages = product.Images.ToList();
        await LoadFormDataAsync();
        return View(model);
    }

    // POST: /Admin/Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model,
        List<string>? compatBrandIds,
        List<string>? compatModelIds,
        List<string>? compatEngineIds,
        List<int>? deleteImageIds)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images)
            .Include(p => p.Compatibilities)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Id = id;
            model.MainImagePath = product.MainImagePath;
            model.Compatibilities = ParseCompatibilities(
                    id, compatBrandIds, compatModelIds, compatEngineIds)
                .Select(c => new CompatibilityItemViewModel
                {
                    CarBrandId = c.CarBrandId,
                    CarModelId = c.CarModelId,
                    CarEngineId = c.CarEngineId
                }).ToList();

            ViewBag.ExtraImages = product.Images.ToList();
            await LoadFormDataAsync();
            return View(model);
        }

        product.Name = model.Name;
        product.SKU = model.SKU;
        product.Brand = model.Brand;
        product.Price = model.Price;
        product.DiscountPrice = model.DiscountPrice;
        product.StockQuantity = model.StockQuantity;
        product.LowStockThreshold = model.LowStockThreshold;
        product.Description = model.Description;
        product.WarrantyPeriod = model.WarrantyPeriod;
        product.Condition = model.Condition;
        product.IsActive = model.IsActive;
        product.IsFeatured = model.IsFeatured;
        product.CategoryId = model.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        // Main image: a new upload wins, then a new URL
        if (model.MainImageFile is { Length: > 0 })
        {
            DeleteImageFile(product.MainImagePath);
            product.MainImagePath = await SaveImageAsync(model.MainImageFile);
            product.MainImageUrl = null;
        }
        else if (!string.IsNullOrWhiteSpace(model.MainImageUrl)
                 && model.MainImageUrl != product.MainImageUrl)
        {
            DeleteImageFile(product.MainImagePath);
            product.MainImagePath = null;
            product.MainImageUrl = model.MainImageUrl;
        }

        // Delete the extra images the admin ticked
        if (deleteImageIds != null && deleteImageIds.Any())
        {
            var toRemove = product.Images
                .Where(i => deleteImageIds.Contains(i.Id))
                .ToList();

            foreach (var img in toRemove)
                DeleteImageFile(img.ImagePath);

            _context.ProductImages.RemoveRange(toRemove);
        }

        // Add new extra images (files)
        if (model.ExtraImageFiles != null)
        {
            foreach (var file in model.ExtraImageFiles.Where(f => f.Length > 0))
            {
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImagePath = await SaveImageAsync(file),
                    IsMain = false
                });
            }
        }

        // Add new extra images (URLs)
        if (model.ExtraImageUrls != null)
        {
            foreach (var url in model.ExtraImageUrls
                .Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = url,
                    IsMain = false
                });
            }
        }

        // Compatibility: remove the old rows, add the new ones
        _context.ProductCompatibilities.RemoveRange(product.Compatibilities);
        _context.ProductCompatibilities.AddRange(
            ParseCompatibilities(product.Id,
                compatBrandIds, compatModelIds, compatEngineIds));

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تعديل المنتج بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/Products/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حذف المنتج بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/Products/ToggleActive
    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return Json(new { success = false });

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isActive = product.IsActive });
    }

    // GET: /Admin/Products/GetModelsByBrand
    [HttpGet]
    public async Task<IActionResult> GetModelsByBrand(int brandId)
    {
        var models = await _context.CarModels
            .Where(m => m.CarBrandId == brandId && m.IsActive)
            .OrderBy(m => m.Name)
            .Select(m => new { m.Id, m.Name })
            .ToListAsync();

        return Json(models);
    }

    // GET: /Admin/Products/GetEnginesByModel
    [HttpGet]
    public async Task<IActionResult> GetEnginesByModel(int modelId)
    {
        var engines = await _context.CarEngines
            .Where(e => e.CarModelId == modelId && e.IsActive)
            .OrderBy(e => e.Name)
            .Select(e => new { e.Id, e.Name })
            .ToListAsync();

        return Json(engines);
    }

    // ========================
    // HELPERS
    // ========================

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/products/{uniqueName}";
    }

    private async Task LoadFormDataAsync()
    {
        ViewBag.Categories = new SelectList(
            await _context.Categories
                .IgnoreQueryFilters()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "Id", "Name");

        // Fix — use named class instead of anonymous type
        var brands = await _context.CarBrands
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new CarBrandJsonItem
            {
                Id = b.Id,
                Name = b.Name
            })
            .ToListAsync();

        ViewBag.CarBrands = brands;

        ViewBag.Conditions = new SelectList(
            Enum.GetValues<ProductCondition>()
                .Select(c => new
                {
                    Value = (int)c,
                    Text = c == ProductCondition.New ? "جديد" : "مجدد"
                }),
            "Value", "Text");
    }

    private static List<ProductCompatibility> ParseCompatibilities(
    int productId,
    List<string>? brandIds,
    List<string>? modelIds,
    List<string>? engineIds)
    {
        var result = new List<ProductCompatibility>();
        if (brandIds == null || modelIds == null) return result;

        for (int i = 0; i < brandIds.Count && i < modelIds.Count; i++)
        {
            if (!int.TryParse(brandIds[i], out int brandId)) continue;
            if (!int.TryParse(modelIds[i], out int modelId)) continue;

            int? engineId = null;
            if (engineIds != null && i < engineIds.Count
                && int.TryParse(engineIds[i], out int eid))
                engineId = eid;

            result.Add(new ProductCompatibility
            {
                ProductId = productId,
                CarBrandId = brandId,
                CarModelId = modelId,
                CarEngineId = engineId
            });
        }

        // Remove duplicates if the admin added the same car twice
        return result
            .DistinctBy(c => (c.CarBrandId, c.CarModelId, c.CarEngineId))
            .ToList();
    }

    private void DeleteImageFile(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return;
        if (!imagePath.StartsWith("/images/products/")) return;

        var fullPath = Path.Combine(
            _env.WebRootPath, "images", "products",
            Path.GetFileName(imagePath));

        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }

}
public class CarBrandJsonItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}