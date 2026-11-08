using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers;

[Authorize(Roles = "PropertyOwner")]
public class OwnerBuildingsController : Controller
{
    private readonly ApplicationDbContext _db;

    public OwnerBuildingsController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private IQueryable<Building> MyBuildings => _db.Buildings.Where(b => b.PropertyOwnerId == CurrentUserId);

    public async Task<IActionResult> Index(string? search)
    {
        var q = MyBuildings;
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

    public async Task<IActionResult> Details(int id)
    {
        var b = await _db.Buildings
            .Include(x => x.Apartments)
            .Include(x => x.PropertyManager)
            .FirstOrDefaultAsync(x => x.Id == id && x.PropertyOwnerId == CurrentUserId);
        if (b == null)
            return NotFound();
        return View(b);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var b = await MyBuildings.FirstOrDefaultAsync(x => x.Id == id);
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

        var existing = await MyBuildings.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null)
            return NotFound();

        // Form only posts address fields; FKs are not posted but stay [Required] on the model.
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
}
