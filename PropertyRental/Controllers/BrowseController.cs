using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;
using PropertyRental.ViewModels;

namespace PropertyRental.Controllers;

public class BrowseController : Controller
{
    private readonly ApplicationDbContext _db;

    public BrowseController(ApplicationDbContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(ApartmentSearchViewModel? filter)
    {
        filter ??= new ApartmentSearchViewModel();
        var q = _db.Apartments
            .Include(a => a.Building)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            var c = filter.City.Trim();
            q = q.Where(a => a.Building!.City != null && a.Building.City.Contains(c));
        }

        if (filter.MinBedrooms.HasValue)
            q = q.Where(a => a.Bedrooms >= filter.MinBedrooms.Value);

        if (filter.MaxRent.HasValue)
            q = q.Where(a => a.MonthlyRent <= filter.MaxRent.Value);

        if (filter.Status.HasValue)
            q = q.Where(a => a.Status == filter.Status.Value);
        else
            q = q.Where(a => a.Status == ApartmentStatus.Available || a.Status == ApartmentStatus.Reserved);

        filter.Results = await q
            .OrderBy(a => a.Building!.City)
            .ThenBy(a => a.MonthlyRent)
            .ToListAsync();
        return View(filter);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var a = await _db.Apartments
            .Include(x => x.Building)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (a == null)
            return NotFound();
        return View(a);
    }
}
