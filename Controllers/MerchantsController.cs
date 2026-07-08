using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLMOFTaxPortal.Models;
using SLMOFTaxPortal.Services;

namespace SLMOFTaxPortal.Controllers;

[Authorize(Policy = "ViewMerchants")]
public class MerchantsController(IMerchantManagementService merchantService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var merchants = await merchantService.GetAll();
        return View(merchants);
    }

    [Authorize(Policy = "ManageMerchants")]
    public IActionResult Create()
    {
        return View(new Merchant { Active = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Merchant merchant)
    {
        if (!ModelState.IsValid)
            return View(merchant);

        var existing = await merchantService.GetByUserId(merchant.UserID);

        if (existing != null)
        {
            ModelState.AddModelError(nameof(merchant.UserID), "User ID already exists.");
            return View(merchant);
        }

        await merchantService.Create(merchant);

        TempData["Success"] = "Merchant created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var merchant = await merchantService.GetByUserId(id);

        if (merchant == null)
            return NotFound();

        return View(merchant);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Merchant merchant)
    {
        if (!ModelState.IsValid)
            return View(merchant);

        await merchantService.Update(merchant);

        TempData["Success"] = "Merchant updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var merchant = await merchantService.GetByUserId(id);

        if (merchant == null)
            return NotFound();

        return View(merchant);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var merchant = await merchantService.GetByUserId(id);

        if (merchant == null)
            return NotFound();

        return View(merchant);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string userId)
    {
        await merchantService.Delete(userId);

        TempData["Success"] = "Merchant deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
