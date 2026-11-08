using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PropertyRental.Data;
using PropertyRental.Models;
using PropertyRental.ViewModels;

namespace PropertyRental.Controllers;

public class TenantRegisterController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantRegisterController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [AllowAnonymous]
    public IActionResult Index() => View(new TenantRegisterModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(TenantRegisterModel model)
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

        await _userManager.AddToRoleAsync(user, DbInitializer.RoleTenant);
        TempData["Status"] = "Account created. You can sign in.";
        return Redirect("/Identity/Account/Login");
    }
}
