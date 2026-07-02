using System;

namespace EcoSphere.Core.Models;

public class EnergyConsumption
{
    public int Id { get; set; }
    public string ConsumptionType { get; set; } = string.Empty; // "Elektrik", "Doğalgaz", "Yakıt"
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty; // "kWh", "m3", "Litre"
    public DateTime DateLogged { get; set; }
    public double CO2Equivalent { get; set; } // Calculated: e.g. electricity * 0.4, natural gas * 2.0, fuel * 2.7
    public int UserId { get; set; }
    
    // Navigation property
    public User? User { get; set; }
}
