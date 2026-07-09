using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLMOFTaxPortal.Models.ViewModels;
using SLMOFTaxPortal.Services;

namespace SLMOFTaxPortal.Controllers;

[Authorize(Policy = "ManageUsers")]
public class RolesController(IRolePermissionService roleService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var roles = await roleService.GetRoles();
        return View(roles);
    }

    public IActionResult Create()
    {
        return View(new RoleViewModel { Active = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.RoleName))
            ModelState.AddModelError(nameof(model.RoleName), "Role name is required.");

        if (!ModelState.IsValid)
            return View(model);

        await roleService.CreateRole(model);

        TempData["Success"] = "Role created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var role = await roleService.GetRoleById(id);

        if (role == null)
            return NotFound();

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoleViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.RoleName))
            ModelState.AddModelError(nameof(model.RoleName), "Role name is required.");

        if (!ModelState.IsValid)
            return View(model);

        await roleService.UpdateRole(model);

        TempData["Success"] = "Role updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Permissions(int id)
    {
        var model = await roleService.GetRolePermissions(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissions(int roleId, List<int> permissionIds)
    {
        await roleService.UpdateRolePermissions(roleId, permissionIds ?? new List<int>());

        TempData["Success"] = "Role permissions updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Disable(int id)
    {
        await roleService.DisableRole(id);

        TempData["Success"] = "Role disabled successfully.";
        return RedirectToAction(nameof(Index));
    }
}
