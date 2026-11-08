using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers;

[Authorize(Roles = "PropertyManager")]
public class BuildingsController : Controller
{
    private readonly ApplicationDbContext _db;

    public BuildingsController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Buildings.Where(b => b.PropertyManagerId == CurrentUserId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(b =>
                b.Name.Contains(s) || b.StreetAddress.Contains(s) ||
                (b.City != null && b.City.Contains(s)));
        }

        ViewBag.Search = search;
        return View(await q.OrderBy(b => b.Name).ToListAsync());
    }

    public IActionResult Create() => View(new Building());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Building model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.PropertyManagerId = CurrentUserId;
        _db.Buildings.Add(model);
        await _db.SaveChangesAsync();
        TempData["Status"] = "Building created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var b = await _db.Buildings.FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (b == null)
            return NotFound();
        return View(b);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Building model)
    {
        if (id != model.Id)
            return BadRequest();
        var existing = await _db.Buildings.FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (existing == null)
            return NotFound();

        ModelState.Remove(nameof(Building.PropertyOwnerId));
        ModelState.Remove(nameof(Building.PropertyManagerId));
        ModelState.Remove(nameof(Building.PropertyOwner));
        ModelState.Remove(nameof(Building.PropertyManager));
        ModelState.Remove(nameof(Building.Apartments));

        if (!ModelState.IsValid)
            return View(model);

        existing.Name = model.Name;
        existing.StreetAddress = model.StreetAddress;
        existing.City = model.City;
        existing.PostalCode = model.PostalCode;
        await _db.SaveChangesAsync();
        TempData["Status"] = "Building updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var b = await _db.Buildings.FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (b == null)
            return NotFound();
        return View(b);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var b = await _db.Buildings.FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (b == null)
            return NotFound();
        _db.Buildings.Remove(b);
        await _db.SaveChangesAsync();
        TempData["Status"] = "Building deleted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var b = await _db.Buildings
            .Include(x => x.Apartments)
            .FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (b == null)
            return NotFound();
        return View(b);
    }
}
