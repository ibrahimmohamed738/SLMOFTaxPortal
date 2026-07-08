using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SLMOFTaxPortal.Models;
using SLMOFTaxPortal.Services;
using System.Security.Claims;

namespace SLMOFTaxPortal.Controllers;

public class AccountController(IAuthService authService) : Controller
{
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(model.Username) ||
            string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError("", "Username and password are required.");
            return View(model);
        }

        var user = await authService.ValidateUser(model.Username, model.Password);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        var permissions = await authService.GetUserPermissions(user.Id);

        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim("FullName", user.FullName),
    new Claim(ClaimTypes.Role, user.RoleName)
};

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return Redirect(returnUrl ?? "/Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
