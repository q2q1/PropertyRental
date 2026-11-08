using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;
using PropertyRental.ViewModels;

namespace PropertyRental.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var featured = await _db.Apartments
            .Include(a => a.Building)
            .Where(a => a.Status == ApartmentStatus.Available || a.Status == ApartmentStatus.Reserved)
            .OrderBy(a => a.Id)
            .Take(6)
            .ToListAsync();
        return View(new HomeIndexViewModel { FeaturedApartments = featured });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
