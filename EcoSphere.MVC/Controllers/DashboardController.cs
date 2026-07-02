using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Repositories;
using EcoSphere.Core.Models;
using EcoSphere.Core.Session;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Serilog;

namespace EcoSphere.MVC.Controllers;

public class DashboardController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var energyLogs = await _unitOfWork.EnergyConsumptions.GetAllAsync();
        var wasteLogs = await _unitOfWork.WasteManagements.GetAllAsync();
        var goals = await _unitOfWork.SustainabilityGoals.GetAllAsync();
        var certificates = await _unitOfWork.EcoCertificates.GetAllAsync();
        var audits = await _unitOfWork.SustainabilityAudits.GetAllAsync();

        // Calculate statistics
        double totalCO2 = energyLogs.Sum(e => e.CO2Equivalent);
        double totalWaste = wasteLogs.Sum(w => w.WeightKg);
        double recycledWaste = wasteLogs.Where(w => w.IsRecycled).Sum(w => w.WeightKg);
        double recyclingRate = totalWaste > 0 ? (recycledWaste / totalWaste) * 100 : 0;

        int activeGoalsCount = goals.Count(g => g.Status == "Aktif");
        int achievedGoalsCount = goals.Count(g => g.Status == "Tamamlandı");
        int totalGoalsCount = goals.Count();

        // Pass to View via ViewBag
        ViewBag.TotalCO2 = Math.Round(totalCO2, 2);
        ViewBag.TotalWaste = Math.Round(totalWaste, 2);
        ViewBag.RecyclingRate = Math.Round(recyclingRate, 1);
        ViewBag.ActiveGoalsCount = activeGoalsCount;
        ViewBag.AchievedGoalsCount = achievedGoalsCount;
        ViewBag.TotalGoalsCount = totalGoalsCount;

        // Recent items
        ViewBag.RecentEnergy = energyLogs.OrderByDescending(e => e.DateLogged).Take(5).ToList();
        ViewBag.RecentWaste = wasteLogs.OrderByDescending(w => w.DateLogged).Take(5).ToList();
        ViewBag.RecentGoals = goals.OrderByDescending(g => g.TargetDate).Take(5).ToList();
        ViewBag.RecentCertificates = certificates.Take(5).ToList();
        ViewBag.RecentAudits = audits.OrderByDescending(a => a.AuditDate).Take(5).ToList();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DownloadExcel()
    {
        var energy = await _unitOfWork.EnergyConsumptions.GetAllAsync();
        var waste = await _unitOfWork.WasteManagements.GetAllAsync();

        using (var package = new ExcelPackage())
        {
            // Sheet 1: Energy Logs
            var wsEnergy = package.Workbook.Worksheets.Add("Enerji Tuketimi");
            wsEnergy.Cells[1, 1].Value = "ID";
            wsEnergy.Cells[1, 2].Value = "Tuketim Tipi";
            wsEnergy.Cells[1, 3].Value = "Miktar";
            wsEnergy.Cells[1, 4].Value = "Birim";
            wsEnergy.Cells[1, 5].Value = "Kayit Tarihi";
            wsEnergy.Cells[1, 6].Value = "CO2 Esdegeri (kg)";
            
            using (var range = wsEnergy.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MediumSeaGreen);
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            int row = 2;
            foreach (var item in energy)
            {
                wsEnergy.Cells[row, 1].Value = item.Id;
                wsEnergy.Cells[row, 2].Value = item.ConsumptionType;
                wsEnergy.Cells[row, 3].Value = item.Quantity;
                wsEnergy.Cells[row, 4].Value = item.Unit;
                wsEnergy.Cells[row, 5].Value = item.DateLogged.ToString("dd.MM.yyyy HH:mm");
                wsEnergy.Cells[row, 6].Value = item.CO2Equivalent;
                row++;
            }
            wsEnergy.Cells[wsEnergy.Dimension.Address].AutoFitColumns();

            // Sheet 2: Waste Logs
            var wsWaste = package.Workbook.Worksheets.Add("Atik Yonetimi");
            wsWaste.Cells[1, 1].Value = "ID";
            wsWaste.Cells[1, 2].Value = "Atik Tipi";
            wsWaste.Cells[1, 3].Value = "Agirlik (kg)";
            wsWaste.Cells[1, 4].Value = "Geri Donusturuldu mu?";
            wsWaste.Cells[1, 5].Value = "Kayit Tarihi";

            using (var range = wsWaste.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SlateGray);
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            row = 2;
            foreach (var item in waste)
            {
                wsWaste.Cells[row, 1].Value = item.Id;
                wsWaste.Cells[row, 2].Value = item.WasteType;
                wsWaste.Cells[row, 3].Value = item.WeightKg;
                wsWaste.Cells[row, 4].Value = item.IsRecycled ? "Evet" : "Hayir";
                wsWaste.Cells[row, 5].Value = item.DateLogged.ToString("dd.MM.yyyy HH:mm");
                row++;
            }
            wsWaste.Cells[wsWaste.Dimension.Address].AutoFitColumns();

            var fileBytes = package.GetAsByteArray();
            Log.Information("Excel raporu olusturuldu ve indirildi.");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EcoSphere_Surdurulebilirlik_Raporu.xlsx");
        }
    }

    [HttpGet]
    public async Task<IActionResult> DownloadPdf()
    {
        var energy = await _unitOfWork.EnergyConsumptions.GetAllAsync();
        var waste = await _unitOfWork.WasteManagements.GetAllAsync();
        
        double totalCO2 = energy.Sum(e => e.CO2Equivalent);
        double totalWaste = waste.Sum(w => w.WeightKg);
        double recycledWaste = waste.Where(w => w.IsRecycled).Sum(w => w.WeightKg);
        double recyclingRate = totalWaste > 0 ? (recycledWaste / totalWaste) * 100 : 0;

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("ECOSPHERE").FontSize(22).Bold().FontColor(Colors.Green.Medium);
                        col.Item().Text("Kurumsal Surdurulebilirlik Raporu").FontSize(14).SemiBold().FontColor(Colors.Grey.Darken2);
                    });
                    row.ConstantItem(100).AlignRight().AlignMiddle().Text(DateTime.Now.ToString("dd.MM.yyyy")).FontSize(10).FontColor(Colors.Grey.Medium);
                });

                // Content
                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(15);

                    // Stats Section
                    col.Item().Text("1. Genel Ozet Istatistikler").FontSize(13).Bold().FontColor(Colors.Green.Darken2);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Headers
                        table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Toplam CO2 Ayak Izi").Bold();
                        table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Toplam Atik Miktari").Bold();
                        table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Geri Donusum Orani").Bold();

                        // Values
                        table.Cell().Padding(5).Text($"{totalCO2:N2} kg CO2");
                        table.Cell().Padding(5).Text($"{totalWaste:N2} kg");
                        table.Cell().Padding(5).Text($"%{recyclingRate:N1}");
                    });

                    // Energy Section
                    col.Item().Text("2. Enerji Tuketim Detaylari").FontSize(13).Bold().FontColor(Colors.Green.Darken2);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("ID").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Tuketim Tipi").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Miktar").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Tarih").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("CO2 Esdegeri").Bold();

                        foreach (var item in energy.Take(10))
                        {
                            table.Cell().Padding(4).Text(item.Id.ToString());
                            table.Cell().Padding(4).Text(item.ConsumptionType);
                            table.Cell().Padding(4).Text($"{item.Quantity} {item.Unit}");
                            table.Cell().Padding(4).Text(item.DateLogged.ToString("dd.MM.yyyy"));
                            table.Cell().Padding(4).Text($"{item.CO2Equivalent:N2} kg");
                        }
                    });

                    // Waste Section
                    col.Item().Text("3. Atik Geri Kazanim Detaylari").FontSize(13).Bold().FontColor(Colors.Green.Darken2);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("ID").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Atik Tipi").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Agirlik (kg)").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Durum").Bold();
                        table.Cell().Background(Colors.Green.Lighten4).Padding(5).Text("Tarih").Bold();

                        foreach (var item in waste.Take(10))
                        {
                            table.Cell().Padding(4).Text(item.Id.ToString());
                            table.Cell().Padding(4).Text(item.WasteType);
                            table.Cell().Padding(4).Text(item.WeightKg.ToString("N1"));
                            table.Cell().Padding(4).Text(item.IsRecycled ? "Geri Donusturuldu" : "Geri Donusturulmedi");
                            table.Cell().Padding(4).Text(item.DateLogged.ToString("dd.MM.yyyy"));
                        }
                    });
                });

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Sayfa ").FontSize(9).FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();

        Log.Information("PDF raporu olusturuldu ve indirildi.");
        return File(pdfBytes, "application/pdf", "EcoSphere_Surdurulebilirlik_Raporu.pdf");
    }
}
