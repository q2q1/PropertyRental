using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PropertyRental.Data;
using PropertyRental.Models;
using PropertyRental.ViewModels;

namespace PropertyRental.Controllers;

[Authorize(Roles = "PropertyManager,PropertyOwner")]
public class AdminTenantsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminTenantsController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(DbInitializer.RoleTenant);
        var query = usersInRole.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u =>
                u.Email!.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                u.FirstName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.Search = search;
        return View(query.OrderBy(u => u.LastName).ToList());
    }

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        if (!await _userManager.IsInRoleAsync(user, DbInitializer.RoleTenant))
            return Forbid();

        var vm = new TenantEditModel
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Edit(string id, TenantEditModel model)
    {
        if (id != model.Id)
            return BadRequest();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        if (!await _userManager.IsInRoleAsync(user, DbInitializer.RoleTenant))
            return Forbid();

        if (!ModelState.IsValid)
            return View(model);

        user.Email = model.Email;
        user.UserName = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        TempData["Status"] = "Tenant profile updated.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        if (!await _userManager.IsInRoleAsync(user, DbInitializer.RoleTenant))
            return Forbid();
        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
        if (!await _userManager.IsInRoleAsync(user, DbInitializer.RoleTenant))
            return Forbid();

        await _userManager.DeleteAsync(user);
        TempData["Status"] = "Tenant account removed.";
        return RedirectToAction(nameof(Index));
    }
}
