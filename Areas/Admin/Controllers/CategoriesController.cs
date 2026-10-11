using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurboHub.Data;
using TurboHub.Models;
using TurboHub.ViewModels.Admin;

namespace TurboHub.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .IgnoreQueryFilters()
            .Include(c => c.Parent)
            .Include(c => c.Products)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Icon = c.Icon,
                ParentId = c.ParentId,
                ProductCount = c.Products.Count(p => !p.IsDeleted)
            })
            .ToListAsync();

        return View(categories);
    }

    // GET: /Admin/Categories/Create
    public async Task<IActionResult> Create()
    {
        await LoadParentCategoriesAsync();
        LoadIconList();
        return View(new CategoryViewModel { Name = string.Empty });
    }

    // POST: /Admin/Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadParentCategoriesAsync();
            LoadIconList();
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name,
            Icon = model.Icon,
            ParentId = model.ParentId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم إضافة القسم بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Categories/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Icon = category.Icon,
            ParentId = category.ParentId
        };

        await LoadParentCategoriesAsync(id);
        LoadIconList();
        return View(model);
    }

    // POST: /Admin/Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadParentCategoriesAsync(id);
            LoadIconList();
            return View(model);
        }

        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        category.Name = model.Name;
        category.Icon = model.Icon;
        category.ParentId = model.ParentId;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تعديل القسم بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/Categories/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        var hasProducts = await _context.Products
            .IgnoreQueryFilters()
            .AnyAsync(p => p.CategoryId == id && !p.IsDeleted);

        if (hasProducts)
        {
            TempData["Error"] = "لا يمكن حذف القسم لأنه يحتوي على منتجات";
            return RedirectToAction(nameof(Index));
        }

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حذف القسم بنجاح";
        return RedirectToAction(nameof(Index));
    }

    // ===== Helpers =====

    private async Task LoadParentCategoriesAsync(int? excludeId = null)
    {
        var categories = await _context.Categories
            .IgnoreQueryFilters()
            .Where(c => !c.IsDeleted && c.ParentId == null)
            .Where(c => excludeId == null || c.Id != excludeId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.ParentCategories = new SelectList(categories, "Id", "Name");
    }

    private void LoadIconList()
    {
        ViewBag.IconList = new SelectList(new List<object>
        {
            new { Value = "⚙️",  Text = "⚙️  المحرك وملحقاته" },
            new { Value = "🔩",  Text = "🔩  ناقل الحركة والدفع" },
            new { Value = "🛞",  Text = "🛞  العفشة والفرامل" },
            new { Value = "❄️",  Text = "❄️  التبريد والتكييف" },
            new { Value = "💨",  Text = "💨  نظام العادم" },
            new { Value = "⚡",  Text = "⚡  كهرباء وإضاءة" },
            new { Value = "🚗",  Text = "🚗  الهيكل والزجاج" },
            new { Value = "🔄",  Text = "🔄  إطارات وجنوط" },
            new { Value = "🧰",  Text = "🧰  كماليات وأدوات" },
            new { Value = "🛢️",  Text = "🛢️  الزيوت والفلاتر" },
            new { Value = "💺",  Text = "💺  الصالون والأمان" },
            new { Value = "⛽",  Text = "⛽  نظام الوقود" },
            new { Value = "🔦",  Text = "🔦  الإضاءة الخارجية" },
            new { Value = "🪟",  Text = "🪟  الزجاج والمساحات" },
            new { Value = "🔋",  Text = "🔋  البطاريات" },
        }, "Value", "Text");
    }
}