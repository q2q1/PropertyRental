using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers;

[Authorize(Roles = "PropertyManager")]
public class ApartmentsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ApartmentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private IQueryable<Building> MyBuildings =>
        _db.Buildings.Where(b => b.PropertyManagerId == CurrentUserId);

    public async Task<IActionResult> Index(int? buildingId)
    {
        var q = _db.Apartments
            .Include(a => a.Building)
            .Where(a => a.Building!.PropertyManagerId == CurrentUserId);

        if (buildingId.HasValue)
            q = q.Where(a => a.BuildingId == buildingId.Value);

        var filterItems = await MyBuildings
            .OrderBy(b => b.Name)
            .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
            .ToListAsync();
        filterItems.Insert(0, new SelectListItem { Value = "", Text = "All buildings" });
        ViewBag.BuildingFilter = new SelectList(filterItems, "Value", "Text", buildingId?.ToString());
        return View(await q.OrderBy(a => a.Building!.Name).ThenBy(a => a.UnitNumber).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateBuildingsDropDown();
        return View(new Apartment { Status = ApartmentStatus.Available, ImageUrl = "/images/apartment-sample.jpg" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Apartment model)
    {
        var buildingOk = await MyBuildings.AnyAsync(b => b.Id == model.BuildingId);
        if (!buildingOk)
            ModelState.AddModelError(nameof(model.BuildingId), "Invalid building.");

        if (!ModelState.IsValid)
        {
            await PopulateBuildingsDropDown(model.BuildingId);
            return View(model);
        }

        _db.Apartments.Add(model);
        await _db.SaveChangesAsync();
        TempData["Status"] = "Apartment created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.Building!.PropertyManagerId == CurrentUserId);
        if (a == null)
            return NotFound();
        await PopulateBuildingsDropDown(a.BuildingId);
        return View(a);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Apartment model)
    {
        if (id != model.Id)
            return BadRequest();

        var buildingOk = await MyBuildings.AnyAsync(b => b.Id == model.BuildingId);
        if (!buildingOk)
            ModelState.AddModelError(nameof(model.BuildingId), "Invalid building.");

        var existing = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.Building!.PropertyManagerId == CurrentUserId);
        if (existing == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateBuildingsDropDown(model.BuildingId);
            return View(model);
        }

        existing.BuildingId = model.BuildingId;
        existing.UnitNumber = model.UnitNumber;
        existing.Bedrooms = model.Bedrooms;
        existing.Bathrooms = model.Bathrooms;
        existing.MonthlyRent = model.MonthlyRent;
        existing.Status = model.Status;
        existing.Description = model.Description;
        existing.ImageUrl = model.ImageUrl;
        await _db.SaveChangesAsync();
        TempData["Status"] = "Apartment updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.Building!.PropertyManagerId == CurrentUserId);
        if (a == null)
            return NotFound();
        return View(a);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var a = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.Building!.PropertyManagerId == CurrentUserId);
        if (a == null)
            return NotFound();
        _db.Apartments.Remove(a);
        await _db.SaveChangesAsync();
        TempData["Status"] = "Apartment deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateBuildingsDropDown(int? selectedId = null)
    {
        var items = await MyBuildings.OrderBy(b => b.Name)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name,
                Selected = selectedId == b.Id
            }).ToListAsync();
        ViewBag.BuildingOptions = new SelectList(items, "Value", "Text", selectedId?.ToString());
    }
}
