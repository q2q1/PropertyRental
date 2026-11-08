using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PropertyRental.Data;
using PropertyRental.Models;
using PropertyRental.ViewModels;

namespace PropertyRental.Controllers;

/// <summary>Property owners may create manager accounts and view the list; they cannot edit or delete managers.</summary>
[Authorize(Roles = "PropertyOwner")]
public class PropertyManagersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PropertyManagersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var role = await _roleManager.FindByNameAsync(DbInitializer.RolePropertyManager);
        if (role == null)
            return View(Array.Empty<ApplicationUser>());

        var usersInRole = await _userManager.GetUsersInRoleAsync(DbInitializer.RolePropertyManager);
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

    public IActionResult Create() => View(new PropertyManagerInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PropertyManagerInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, DbInitializer.RolePropertyManager);
        TempData["Status"] = "Property manager account created.";
        return RedirectToAction(nameof(Index));
    }
}
