using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;
using Serilog;

namespace EcoSphere.MVC.Areas.Admin.Controllers;

[Area("Admin")]
public class EcoCertificateController : EcoSphere.MVC.Controllers.BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public EcoCertificateController(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment hostEnvironment)
    {
        RequireAdmin = true;
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var certificates = await _unitOfWork.EcoCertificates.GetAllAsync();
        return View(certificates);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(EcoCertificate certificate, IFormFile? certificateImage)
    {
        if (ModelState.IsValid)
        {
            if (certificateImage != null && certificateImage.Length > 0)
            {
                var uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(certificateImage.FileName);
                var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, uniqueName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await certificateImage.CopyToAsync(fileStream);
                }

                certificate.CertificateImagePath = "/uploads/" + uniqueName;
                Log.Information("Sertifika için dosya yüklendi: {Path}", certificate.CertificateImagePath);
            }

            await _unitOfWork.EcoCertificates.AddAsync(certificate);
            await _unitOfWork.CompleteAsync();
            Log.Information("Yeni çevre sertifikası oluşturuldu: {Name}", certificate.CertificateName);
            return RedirectToAction(nameof(Index));
        }
        return View(certificate);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var certificate = await _unitOfWork.EcoCertificates.GetByIdAsync(id);
        if (certificate == null)
        {
            return NotFound();
        }
        return View(certificate);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EcoCertificate certificate, IFormFile? certificateImage)
    {
        if (ModelState.IsValid)
        {
            var existing = await _unitOfWork.EcoCertificates.GetByIdAsync(certificate.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.CertificateName = certificate.CertificateName;
            existing.IssuingOrganization = certificate.IssuingOrganization;
            existing.IssueDate = certificate.IssueDate;
            existing.ExpiryDate = certificate.ExpiryDate;
            existing.Status = certificate.Status;

            if (certificateImage != null && certificateImage.Length > 0)
            {
                var uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(certificateImage.FileName);
                var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsPath, uniqueName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await certificateImage.CopyToAsync(fileStream);
                }

                existing.CertificateImagePath = "/uploads/" + uniqueName;
                Log.Information("Sertifika dosyası güncellendi: {Path}", existing.CertificateImagePath);
            }

            await _unitOfWork.EcoCertificates.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();
            Log.Information("Sertifika güncellendi: ID={Id}, Name={Name}", existing.Id, existing.CertificateName);
            return RedirectToAction(nameof(Index));
        }
        return View(certificate);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var certificate = await _unitOfWork.EcoCertificates.GetByIdAsync(id);
        if (certificate != null)
        {
            if (!string.IsNullOrEmpty(certificate.CertificateImagePath))
            {
                var fullPath = Path.Combine(_hostEnvironment.WebRootPath, certificate.CertificateImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            await _unitOfWork.EcoCertificates.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            Log.Information("Sertifika silindi: ID={Id}, Name={Name}", certificate.Id, certificate.CertificateName);
        }
        return RedirectToAction(nameof(Index));
    }
}
