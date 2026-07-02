using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;
using EcoSphere.Core.Session;
using Serilog;

namespace EcoSphere.MVC.Areas.User.Controllers;

[Area("User")]
public class EnergyConsumptionController : EcoSphere.MVC.Controllers.BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public EnergyConsumptionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _unitOfWork.EnergyConsumptions.GetAllAsync();
        return View(logs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(EnergyConsumption log)
    {
        if (ModelState.IsValid)
        {
            log.UserId = CurrentSession.CurrentUser?.Id ?? 2;
            log.DateLogged = DateTime.Now;
            log.CO2Equivalent = CalculateCO2(log.ConsumptionType, log.Quantity);

            await _unitOfWork.EnergyConsumptions.AddAsync(log);
            await _unitOfWork.CompleteAsync();
            Log.Information("Yeni enerji tüketim kaydı eklendi: Type={Type}, Qty={Qty}, CO2={CO2}", 
                log.ConsumptionType, log.Quantity, log.CO2Equivalent);
                
            return RedirectToAction(nameof(Index));
        }
        return View(log);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var log = await _unitOfWork.EnergyConsumptions.GetByIdAsync(id);
        if (log == null)
        {
            return NotFound();
        }
        return View(log);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EnergyConsumption log)
    {
        if (ModelState.IsValid)
        {
            var existing = await _unitOfWork.EnergyConsumptions.GetByIdAsync(log.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.ConsumptionType = log.ConsumptionType;
            existing.Quantity = log.Quantity;
            existing.Unit = log.Unit;
            existing.CO2Equivalent = CalculateCO2(log.ConsumptionType, log.Quantity);

            await _unitOfWork.EnergyConsumptions.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();
            Log.Information("Enerji tüketim kaydı güncellendi: ID={Id}, Type={Type}, CO2={CO2}", 
                existing.Id, existing.ConsumptionType, existing.CO2Equivalent);
                
            return RedirectToAction(nameof(Index));
        }
        return View(log);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await _unitOfWork.EnergyConsumptions.GetByIdAsync(id);
        if (log != null)
        {
            await _unitOfWork.EnergyConsumptions.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            Log.Information("Enerji tüketim kaydı silindi: ID={Id}", id);
        }
        return RedirectToAction(nameof(Index));
    }

    private double CalculateCO2(string type, double quantity)
    {
        return type switch
        {
            "Elektrik" => quantity * 0.4,
            "Doğalgaz" => quantity * 2.0,
            "Yakıt" => quantity * 2.7,
            _ => quantity * 1.0
        };
    }
}
