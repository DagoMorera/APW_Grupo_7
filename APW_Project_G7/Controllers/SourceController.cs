using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Solo el rol Admin puede administrar las fuentes
[Authorize(Roles = "Admin")]
public class SourceController : Controller
{
    private readonly ISourceService _sourceService;

    public SourceController(ISourceService sourceService)
    {
        _sourceService = sourceService;
    }

    // GET /Source
    public async Task<IActionResult> Index()
    {
        var sources = await _sourceService.GetSourcesAsync();
        return View(sources);
    }

    // GET /Source/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST /Source/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SourceViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _sourceService.CreateSourceAsync(model);
        return RedirectToAction(nameof(Index));
    }

    // GET /Source/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var source = await _sourceService.GetSourceByIdAsync(id);
        if (source is null) return NotFound();
        return View(source);
    }

    // POST /Source/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SourceViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _sourceService.UpdateSourceAsync(id, model);
        return RedirectToAction(nameof(Index));
    }

    // GET /Source/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var source = await _sourceService.GetSourceByIdAsync(id);
        if (source is null) return NotFound();
        return View(source);
    }

    // POST /Source/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _sourceService.DeleteSourceAsync(id);
        return RedirectToAction(nameof(Index));
    }
}