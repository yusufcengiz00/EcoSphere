using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;
using Serilog;

namespace EcoSphere.MVC.Areas.Admin.Controllers;

[Area("Admin")]
public class SustainabilityGoalController : EcoSphere.MVC.Controllers.BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public SustainabilityGoalController(IUnitOfWork unitOfWork)
    {
        RequireAdmin = true;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var goals = await _unitOfWork.SustainabilityGoals.GetAllAsync();
        return View(goals);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SustainabilityGoal goal)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.SustainabilityGoals.AddAsync(goal);
            await _unitOfWork.CompleteAsync();
            Log.Information("Yeni sürdürülebilirlik hedefi oluşturuldu: {Title}", goal.Title);
            return RedirectToAction(nameof(Index));
        }
        return View(goal);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var goal = await _unitOfWork.SustainabilityGoals.GetByIdAsync(id);
        if (goal == null)
        {
            return NotFound();
        }
        return View(goal);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SustainabilityGoal goal)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.SustainabilityGoals.UpdateAsync(goal);
            await _unitOfWork.CompleteAsync();
            Log.Information("Sürdürülebilirlik hedefi güncellendi: ID={Id}, Title={Title}", goal.Id, goal.Title);
            return RedirectToAction(nameof(Index));
        }
        return View(goal);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _unitOfWork.SustainabilityGoals.GetByIdAsync(id);
        if (goal != null)
        {
            await _unitOfWork.SustainabilityGoals.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            Log.Information("Sürdürülebilirlik hedefi silindi: ID={Id}, Title={Title}", goal.Id, goal.Title);
        }
        return RedirectToAction(nameof(Index));
    }
}
