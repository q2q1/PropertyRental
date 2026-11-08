using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Data;
using PropertyRental.Models;

namespace PropertyRental.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AppointmentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> ManagerIndex()
    {
        var q = _db.Appointments
            .Include(a => a.Tenant)
            .Include(a => a.Apartment)!.ThenInclude(ap => ap!.Building)
            .Where(a => a.PropertyManagerId == CurrentUserId)
            .OrderByDescending(a => a.StartUtc);
        return View("ManagerIndex", await q.ToListAsync());
    }

    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> TenantIndex()
    {
        var q = _db.Appointments
            .Include(a => a.PropertyManager)
            .Include(a => a.Apartment)!.ThenInclude(ap => ap!.Building)
            .Where(a => a.TenantId == CurrentUserId)
            .OrderByDescending(a => a.StartUtc);
        return View("TenantIndex", await q.ToListAsync());
    }

    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Book(int apartmentId)
    {
        var apt = await _db.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == apartmentId);
        if (apt == null)
            return NotFound();
        if (apt.Status != ApartmentStatus.Available && apt.Status != ApartmentStatus.Reserved)
        {
            TempData["Error"] = "This apartment is not available for booking.";
            return RedirectToAction("Index", "Browse");
        }

        ViewBag.Apartment = apt;
        return View(new Appointment
        {
            ApartmentId = apartmentId,
            PropertyManagerId = apt.Building!.PropertyManagerId,
            TenantId = CurrentUserId,
            StartUtc = DateTime.UtcNow.AddDays(1).Date.AddHours(14)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Book(Appointment model)
    {
        var apt = await _db.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == model.ApartmentId);
        if (apt == null)
            return NotFound();

        model.TenantId = CurrentUserId;
        model.PropertyManagerId = apt.Building!.PropertyManagerId;
        model.Status = AppointmentStatus.Pending;

        if (model.StartUtc < DateTime.UtcNow)
            ModelState.AddModelError(nameof(model.StartUtc), "Choose a future date and time.");

        if (!ModelState.IsValid)
        {
            ViewBag.Apartment = apt;
            return View(model);
        }

        _db.Appointments.Add(model);

        // Notify owner (landlord) that an appointment was requested.
        if (apt.Building != null && !string.IsNullOrWhiteSpace(apt.Building.PropertyOwnerId))
        {
            _db.Messages.Add(new Message
            {
                SenderId = model.TenantId,
                ReceiverId = apt.Building.PropertyOwnerId,
                Subject = $"New appointment request: {apt.Building.Name} #{apt.UnitNumber}",
                Body =
                    $"A tenant requested an appointment.\n\n" +
                    $"Building: {apt.Building.Name}\n" +
                    $"Unit: {apt.UnitNumber}\n" +
                    $"Requested time (UTC): {model.StartUtc:g}\n" +
                    (!string.IsNullOrWhiteSpace(model.Notes) ? $"\nNotes:\n{model.Notes}\n" : ""),
                SentUtc = DateTime.UtcNow,
                IsRead = false
            });
        }

        await _db.SaveChangesAsync();
        TempData["Status"] = "Appointment request sent.";
        return RedirectToAction(nameof(TenantIndex));
    }

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> ManagerEdit(int id)
    {
        var a = await _db.Appointments
            .Include(x => x.Tenant)
            .Include(x => x.Apartment)!.ThenInclude(ap => ap!.Building)
            .FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (a == null)
            return NotFound();
        return View(a);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> ManagerEdit(int id, AppointmentStatus status, string? notes)
    {
        var a = await _db.Appointments
            .FirstOrDefaultAsync(x => x.Id == id && x.PropertyManagerId == CurrentUserId);
        if (a == null)
            return NotFound();
        a.Status = status;
        if (notes != null)
            a.Notes = notes;
        await _db.SaveChangesAsync();
        TempData["Status"] = "Appointment updated.";
        return RedirectToAction(nameof(ManagerIndex));
    }
}
