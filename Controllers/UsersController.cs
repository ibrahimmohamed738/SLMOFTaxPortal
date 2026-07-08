using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SLMOFTaxPortal.Models;
using SLMOFTaxPortal.Services;

namespace SLMOFTaxPortal.Controllers;

[Authorize(Policy = "ViewUsers")]
public class UsersController(IUserManagementService userService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userService.GetUsers();
        return View(users);
    }

    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> Create()
    {
        await LoadRoles();
        return View(new UserFormViewModel { Active = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Password))
            ModelState.AddModelError(nameof(model.Password), "Password is required.");

        if (await userService.UsernameExists(model.Username))
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");

        if (!ModelState.IsValid)
        {
            await LoadRoles();
            return View(model);
        }

        await userService.CreateUser(model);

        TempData["Success"] = "User created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await userService.GetUserById(id);

        if (user == null)
            return NotFound();

        await LoadRoles();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> Edit(UserFormViewModel model)
    {
        if (await userService.UsernameExists(model.Username, model.Id))
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");

        if (!ModelState.IsValid)
        {
            await LoadRoles();
            return View(model);
        }

        await userService.UpdateUser(model);

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var user = await userService.GetUserById(id);

        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> ResetPassword(int id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            TempData["Error"] = "Password is required.";
            return RedirectToAction(nameof(ResetPassword), new { id });
        }

        await userService.ResetPassword(id, newPassword);

        TempData["Success"] = "Password reset successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ManageUsers")]
    public async Task<IActionResult> Delete(int id)
    {
        await userService.DeleteUser(id);

        TempData["Success"] = "User disabled successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadRoles()
    {
        var roles = await userService.GetRoles();

        ViewBag.Roles = roles
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.RoleName
            })
            .ToList();
    }
}
