using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;
using EcoSphere.Core.Session;
using Serilog;

namespace EcoSphere.MVC.Areas.User.Controllers;

[Area("User")]
public class WasteManagementController : EcoSphere.MVC.Controllers.BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public WasteManagementController(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _unitOfWork.WasteManagements.GetAllAsync();
        return View(logs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(WasteManagement log, IFormFile? receiptImage)
    {
        if (ModelState.IsValid)
        {
            log.UserId = CurrentSession.CurrentUser?.Id ?? 2;
            log.DateLogged = DateTime.Now;

            if (receiptImage != null && receiptImage.Length > 0)
            {
                var uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(receiptImage.FileName);
                var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, uniqueName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await receiptImage.CopyToAsync(fileStream);
                }

                log.ReceiptImagePath = "/uploads/" + uniqueName;
                Log.Information("Atık kaydı için fiş görseli yüklendi: {Path}", log.ReceiptImagePath);
            }

            await _unitOfWork.WasteManagements.AddAsync(log);
            await _unitOfWork.CompleteAsync();
            Log.Information("Yeni atık kaydı eklendi: Type={Type}, Weight={Weight}kg, Recycled={Recycled}", 
                log.WasteType, log.WeightKg, log.IsRecycled);
                
            return RedirectToAction(nameof(Index));
        }
        return View(log);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var log = await _unitOfWork.WasteManagements.GetByIdAsync(id);
        if (log == null)
        {
            return NotFound();
        }
        return View(log);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(WasteManagement log, IFormFile? receiptImage)
    {
        if (ModelState.IsValid)
        {
            var existing = await _unitOfWork.WasteManagements.GetByIdAsync(log.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.WasteType = log.WasteType;
            existing.WeightKg = log.WeightKg;
            existing.IsRecycled = log.IsRecycled;

            if (receiptImage != null && receiptImage.Length > 0)
            {
                var uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(receiptImage.FileName);
                var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsPath, uniqueName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await receiptImage.CopyToAsync(fileStream);
                }

                existing.ReceiptImagePath = "/uploads/" + uniqueName;
                Log.Information("Atık fiş görseli güncellendi: {Path}", existing.ReceiptImagePath);
            }

            await _unitOfWork.WasteManagements.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();
            Log.Information("Atık kaydı güncellendi: ID={Id}, Type={Type}", existing.Id, existing.WasteType);
            
            return RedirectToAction(nameof(Index));
        }
        return View(log);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await _unitOfWork.WasteManagements.GetByIdAsync(id);
        if (log != null)
        {
            if (!string.IsNullOrEmpty(log.ReceiptImagePath))
            {
                var fullPath = Path.Combine(_hostEnvironment.WebRootPath, log.ReceiptImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            await _unitOfWork.WasteManagements.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            Log.Information("Atık kaydı silindi: ID={Id}", id);
        }
        return RedirectToAction(nameof(Index));
    }
}
