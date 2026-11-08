using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;

namespace PropertyRental.Controllers;

[Authorize(Roles = "PropertyOwner")]
public class OwnerDashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public OwnerDashboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var uid = CurrentUserId;
        var buildings = await _db.Buildings
            .AsNoTracking()
            .Include(b => b.Apartments)
            .Include(b => b.PropertyManager)
            .Where(b => b.PropertyOwnerId == uid)
            .OrderBy(b => b.Name)
            .ToListAsync();

        var appointments = await _db.Appointments
            .AsNoTracking()
            .Include(a => a.Tenant)
            .Include(a => a.Apartment)!.ThenInclude(ap => ap!.Building)
            .Where(a => a.Apartment!.Building!.PropertyOwnerId == uid)
            .OrderByDescending(a => a.StartUtc)
            .Take(50)
            .ToListAsync();

        ViewBag.Buildings = buildings;
        return View(appointments);
    }
}

