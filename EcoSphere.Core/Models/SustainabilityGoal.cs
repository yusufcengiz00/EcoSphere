using System;

namespace EcoSphere.Core.Models;

public class SustainabilityGoal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TargetMetric { get; set; } = string.Empty; // e.g. "CO2 Azaltımı", "Geri Dönüşüm Oranı"
    public double TargetValue { get; set; }
    public double CurrentValue { get; set; }
    public string Unit { get; set; } = string.Empty; // e.g. "kg CO2", "%", "kWh"
    public DateTime TargetDate { get; set; }
    public string Status { get; set; } = "Aktif"; // "Aktif", "Tamamlandı", "Başarısız"
}
