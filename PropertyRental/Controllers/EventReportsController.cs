using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;
using PropertyRental.ViewModels;

namespace PropertyRental.Controllers;

[Authorize]
public class EventReportsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventReportsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> ManagerIndex()
    {
        var q = _db.OwnerEventReports
            .Include(r => r.PropertyOwner)
            .Where(r => r.PropertyManagerId == CurrentUserId)
            .OrderByDescending(r => r.CreatedUtc);
        return View("ManagerIndex", await q.ToListAsync());
    }

    [Authorize(Roles = "PropertyOwner")]
    public async Task<IActionResult> OwnerIndex()
    {
        var q = _db.OwnerEventReports
            .Include(r => r.PropertyManager)
            .Where(r => r.PropertyOwnerId == CurrentUserId)
            .OrderByDescending(r => r.CreatedUtc);
        return View("OwnerIndex", await q.ToListAsync());
    }

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Create()
    {
        await PopulateRecipientDropdown();
        return View(new EventReportCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Create(EventReportCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateRecipientDropdown(model.PropertyOwnerId);
            return View(model);
        }

        var report = new OwnerEventReport
        {
            PropertyManagerId = CurrentUserId,
            PropertyOwnerId = model.PropertyOwnerId.Trim(),
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            CreatedUtc = DateTime.UtcNow,
            IsAcknowledged = false
        };

        _db.OwnerEventReports.Add(report);
        await _db.SaveChangesAsync();
        TempData["Status"] = "Report sent to the property owner.";
        return RedirectToAction(nameof(ManagerIndex));
    }

    private async Task PopulateRecipientDropdown(string? selectedId = null)
    {
        var recipients = (await _userManager.GetUsersInRoleAsync(DbInitializer.RolePropertyOwner))
            .OrderBy(u => u.LastName)
            .ToList();
        var items = new List<SelectListItem> { new() { Value = "", Text = "— Select recipient —" } };
        items.AddRange(recipients.Select(u => new SelectListItem
        {
            Value = u.Id,
            Text = $"{u.FirstName} {u.LastName} ({u.Email})"
        }));
        ViewBag.OwnerOptions = new SelectList(items, "Value", "Text", selectedId);
    }

    [Authorize(Roles = "PropertyOwner")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Acknowledge(int id)
    {
        var r = await _db.OwnerEventReports.FirstOrDefaultAsync(x => x.Id == id && x.PropertyOwnerId == CurrentUserId);
        if (r == null)
            return NotFound();
        r.IsAcknowledged = true;
        await _db.SaveChangesAsync();
        TempData["Status"] = "Report marked as acknowledged.";
        return RedirectToAction(nameof(OwnerIndex));
    }
}
