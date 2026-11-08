using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers;

[Authorize(Roles = "PropertyOwner")]
public class OwnerApartmentsController : Controller
{
    private readonly ApplicationDbContext _db;

    public OwnerApartmentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private IQueryable<Building> MyBuildings =>
        _db.Buildings.Where(b => b.PropertyOwnerId == CurrentUserId);

    public async Task<IActionResult> Index(int? buildingId)
    {
        var q = _db.Apartments
            .Include(a => a.Building)
            .Where(a => a.Building!.PropertyOwnerId == CurrentUserId);

        if (buildingId.HasValue)
            q = q.Where(a => a.BuildingId == buildingId.Value);

        var filterItems = await MyBuildings
            .OrderBy(b => b.Name)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = string.IsNullOrEmpty(b.City)
                    ? $"{b.Name} — {b.StreetAddress}"
                    : $"{b.Name} — {b.StreetAddress}, {b.City}"
            })
            .ToListAsync();
        filterItems.Insert(0, new SelectListItem { Value = "", Text = "All buildings" });
        ViewBag.BuildingFilter = new SelectList(filterItems, "Value", "Text", buildingId?.ToString());
        return View(await q.OrderBy(a => a.Building!.Name).ThenBy(a => a.UnitNumber).ToListAsync());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.Building!.PropertyOwnerId == CurrentUserId);
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
            ModelState.AddModelError(nameof(model.BuildingId), "You can only assign this unit to one of your buildings.");

        var existing = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.Building!.PropertyOwnerId == CurrentUserId);
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
