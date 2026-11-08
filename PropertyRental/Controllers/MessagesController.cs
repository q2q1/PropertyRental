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

[Authorize(Roles = "PropertyManager,Tenant,PropertyOwner")]
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessagesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var uid = CurrentUserId;
        var all = await _db.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == uid || m.ReceiverId == uid)
            .OrderByDescending(m => m.SentUtc)
            .ToListAsync();

        ViewBag.Inbox = all.Where(m => m.ReceiverId == uid).ToList();
        ViewBag.Sent = all.Where(m => m.SenderId == uid).ToList();
        return View(all);
    }

    public async Task<IActionResult> Create(string? receiverId)
    {
        receiverId = string.IsNullOrWhiteSpace(receiverId) ? null : receiverId.Trim();
        await PopulatePartnerDropdown(receiverId);
        return View(new MessageComposeModel { ReceiverId = receiverId ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MessageComposeModel model)
    {
        var receiverIdInput = model.ReceiverId?.Trim() ?? string.Empty;
        if (receiverIdInput == CurrentUserId)
            ModelState.AddModelError(nameof(model.ReceiverId), "You cannot message yourself.");

        if (!ModelState.IsValid)
        {
            await PopulatePartnerDropdown(model.ReceiverId);
            return View(model);
        }

        var receiverId = receiverIdInput;
        var receiverExists = await _userManager.FindByIdAsync(receiverId);
        if (receiverExists == null)
        {
            ModelState.AddModelError(nameof(model.ReceiverId), "Recipient account was not found. Choose someone from the list.");
            await PopulatePartnerDropdown(model.ReceiverId);
            return View(model);
        }

        var msg = new Message
        {
            SenderId = CurrentUserId,
            ReceiverId = receiverId,
            Subject = model.Subject.Trim(),
            Body = model.Body.Trim(),
            SentUtc = DateTime.UtcNow,
            IsRead = false
        };

        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();
        TempData["Status"] = "Message sent.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var m = await _db.Messages.FirstOrDefaultAsync(x => x.Id == id);
        if (m == null)
            return NotFound();
        if (m.ReceiverId != CurrentUserId)
            return Forbid();
        m.IsRead = true;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulatePartnerDropdown(string? selectedReceiverId = null)
    {
        List<ApplicationUser> list;
        if (User.IsInRole(DbInitializer.RoleTenant))
        {
            // Tenants: message property managers or owners.
            var managers = await _userManager.GetUsersInRoleAsync(DbInitializer.RolePropertyManager);
            var owners = await _userManager.GetUsersInRoleAsync(DbInitializer.RolePropertyOwner);
            list = managers.Concat(owners).DistinctBy(u => u.Id).OrderBy(u => u.LastName).ToList();
        }
        else if (User.IsInRole(DbInitializer.RolePropertyManager))
        {
            list = (await _userManager.GetUsersInRoleAsync(DbInitializer.RoleTenant)).OrderBy(u => u.LastName).ToList();
        }
        else
        {
            var tenants = await _userManager.GetUsersInRoleAsync(DbInitializer.RoleTenant);
            var managers = await _userManager.GetUsersInRoleAsync(DbInitializer.RolePropertyManager);
            list = tenants.Concat(managers).DistinctBy(u => u.Id).OrderBy(u => u.LastName).ToList();
        }
        var items = new List<SelectListItem>
        {
            new() { Value = "", Text = "— Select recipient —" }
        };
        items.AddRange(list.Select(u => new SelectListItem
        {
            Value = u.Id,
            Text = $"{u.FirstName} {u.LastName} ({u.Email})"
        }));
        ViewBag.ReceiverOptions = new SelectList(items, "Value", "Text", selectedReceiverId);
    }
}
