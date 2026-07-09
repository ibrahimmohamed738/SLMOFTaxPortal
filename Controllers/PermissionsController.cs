using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLMOFTaxPortal.Models.ViewModels;
using SLMOFTaxPortal.Security;
using SLMOFTaxPortal.Services;

namespace SLMOFTaxPortal.Controllers;

[Authorize(Policy = AppPermissions.ManagePermissions)]
public class PermissionsController(IPermissionService service) : Controller
{
    public async Task<IActionResult> Index()
        => View(await service.GetAll());

    public IActionResult Create()
        => View(new PermissionViewModel
        {
            Active = true
        });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PermissionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await service.Exists(model.PermissionCode))
        {
            ModelState.AddModelError(
                nameof(model.PermissionCode),
                "Permission code already exists.");
        }

        await service.Create(model);

        TempData["Success"] = "Permission created successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var permission = await service.GetById(id);

        if (permission == null)
            return NotFound();

        return View(permission);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PermissionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await service.Exists(model.PermissionCode, model.Id))
        {
            ModelState.AddModelError(
                nameof(model.PermissionCode),
                "Permission code already exists.");
        }

        await service.Update(model);

        TempData["Success"] = "Permission updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Disable(int id)
    {
        await service.Disable(id);

        TempData["Success"] = "Permission disabled successfully.";

        return RedirectToAction(nameof(Index));
    }
}
