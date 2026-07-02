using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;
using Serilog;

namespace EcoSphere.MVC.Areas.Admin.Controllers;

[Area("Admin")]
public class SustainabilityAuditController : EcoSphere.MVC.Controllers.BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public SustainabilityAuditController(IUnitOfWork unitOfWork)
    {
        RequireAdmin = true;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var audits = await _unitOfWork.SustainabilityAudits.GetAllAsync();
        return View(audits);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SustainabilityAudit audit)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.SustainabilityAudits.AddAsync(audit);
            await _unitOfWork.CompleteAsync();
            Log.Information("Yeni denetim raporu eklendi: Dep={Dep}, Auditor={Auditor}", audit.DepartmentName, audit.AuditorName);
            return RedirectToAction(nameof(Index));
        }
        return View(audit);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var audit = await _unitOfWork.SustainabilityAudits.GetByIdAsync(id);
        if (audit == null)
        {
            return NotFound();
        }
        return View(audit);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SustainabilityAudit audit)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.SustainabilityAudits.UpdateAsync(audit);
            await _unitOfWork.CompleteAsync();
            Log.Information("Denetim raporu güncellendi: ID={Id}, Dep={Dep}", audit.Id, audit.DepartmentName);
            return RedirectToAction(nameof(Index));
        }
        return View(audit);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var audit = await _unitOfWork.SustainabilityAudits.GetByIdAsync(id);
        if (audit != null)
        {
            await _unitOfWork.SustainabilityAudits.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            Log.Information("Denetim raporu silindi: ID={Id}, Dep={Dep}", audit.Id, audit.DepartmentName);
        }
        return RedirectToAction(nameof(Index));
    }
}
