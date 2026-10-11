using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurboHub.Data;
using TurboHub.Models;
using TurboHub.ViewModels.Admin;

namespace TurboHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CarsController : Controller
{
    private readonly AppDbContext _context;

    public CarsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Cars
    public async Task<IActionResult> Index()
    {
        var brands = await _context.CarBrands
            .Include(b => b.Models)
            .OrderBy(b => b.Name)
            .Select(b => new CarBrandViewModel
            {
                Id = b.Id,
                Name = b.Name,
                IsActive = b.IsActive,
                ModelsCount = b.Models.Count
            })
            .ToListAsync();

        return View(brands);
    }

    // =====================
    // BRAND ACTIONS
    // =====================

    [HttpPost]
    public async Task<IActionResult> CreateBrand(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "اسم الماركة مطلوب" });

        var exists = await _context.CarBrands.AnyAsync(b => b.Name == name);
        if (exists)
            return Json(new { success = false, message = "هذه الماركة موجودة بالفعل" });

        var brand = new CarBrand { Name = name, IsActive = true };
        _context.CarBrands.Add(brand);
        await _context.SaveChangesAsync();

        return Json(new { success = true, id = brand.Id, name = brand.Name });
    }

    [HttpPost]
    public async Task<IActionResult> EditBrand(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "اسم الماركة مطلوب" });

        var brand = await _context.CarBrands.FindAsync(id);
        if (brand == null)
            return Json(new { success = false, message = "الماركة غير موجودة" });

        brand.Name = name;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var brand = await _context.CarBrands
            .Include(b => b.Models)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand == null)
            return Json(new { success = false, message = "الماركة غير موجودة" });

        if (brand.Models.Any())
            return Json(new { success = false, message = "لا يمكن حذف الماركة لأنها تحتوي على موديلات" });

        _context.CarBrands.Remove(brand);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // =====================
    // MODEL ACTIONS
    // =====================

    [HttpGet]
    public async Task<IActionResult> GetModels(int brandId)
    {
        var models = await _context.CarModels
            .Where(m => m.CarBrandId == brandId)
            .OrderBy(m => m.Name)
            .Select(m => new CarModelViewModel
            {
                Id = m.Id,
                Name = m.Name,
                IsActive = m.IsActive,
                CarBrandId = m.CarBrandId,
                EnginesCount = m.Engines.Count
            })
            .ToListAsync();

        return Json(models);
    }

    [HttpPost]
    public async Task<IActionResult> CreateModel(string name, int carBrandId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "اسم الموديل مطلوب" });

        var exists = await _context.CarModels
            .AnyAsync(m => m.Name == name && m.CarBrandId == carBrandId);

        if (exists)
            return Json(new { success = false, message = "هذا الموديل موجود بالفعل" });

        var model = new CarModel
        {
            Name = name,
            CarBrandId = carBrandId,
            IsActive = true
        };

        _context.CarModels.Add(model);
        await _context.SaveChangesAsync();

        return Json(new { success = true, id = model.Id, name = model.Name });
    }

    [HttpPost]
    public async Task<IActionResult> EditModel(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "اسم الموديل مطلوب" });

        var model = await _context.CarModels.FindAsync(id);
        if (model == null)
            return Json(new { success = false, message = "الموديل غير موجود" });

        model.Name = name;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteModel(int id)
    {
        var model = await _context.CarModels
            .Include(m => m.Engines)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (model == null)
            return Json(new { success = false, message = "الموديل غير موجود" });

        if (model.Engines.Any())
            return Json(new { success = false, message = "لا يمكن حذف الموديل لأنه يحتوي على محركات" });

        _context.CarModels.Remove(model);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // =====================
    // ENGINE ACTIONS
    // =====================

    [HttpGet]
    public async Task<IActionResult> GetEngines(int modelId)
    {
        var engines = await _context.CarEngines
            .Where(e => e.CarModelId == modelId)
            .OrderBy(e => e.Name)
            .Select(e => new CarEngineViewModel
            {
                Id = e.Id,
                Name = e.Name,
                IsActive = e.IsActive,
                CarModelId = e.CarModelId
            })
            .ToListAsync();

        return Json(engines);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEngine(string name, int carModelId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "نوع المحرك مطلوب" });

        var exists = await _context.CarEngines
            .AnyAsync(e => e.Name == name && e.CarModelId == carModelId);

        if (exists)
            return Json(new { success = false, message = "هذا المحرك موجود بالفعل" });

        var engine = new CarEngine
        {
            Name = name,
            CarModelId = carModelId,
            IsActive = true
        };

        _context.CarEngines.Add(engine);
        await _context.SaveChangesAsync();

        return Json(new { success = true, id = engine.Id, name = engine.Name });
    }

    [HttpPost]
    public async Task<IActionResult> EditEngine(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Json(new { success = false, message = "نوع المحرك مطلوب" });

        var engine = await _context.CarEngines.FindAsync(id);
        if (engine == null)
            return Json(new { success = false, message = "المحرك غير موجود" });

        engine.Name = name;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEngine(int id)
    {
        var engine = await _context.CarEngines.FindAsync(id);
        if (engine == null)
            return Json(new { success = false, message = "المحرك غير موجود" });

        _context.CarEngines.Remove(engine);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}