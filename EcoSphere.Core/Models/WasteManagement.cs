using System;

namespace EcoSphere.Core.Models;

public class WasteManagement
{
    public int Id { get; set; }
    public string WasteType { get; set; } = string.Empty; // "Plastik", "Kağıt", "Cam", "Organik"
    public double WeightKg { get; set; }
    public bool IsRecycled { get; set; }
    public DateTime DateLogged { get; set; }
    public string? ReceiptImagePath { get; set; } // Uploaded image path
    public int UserId { get; set; }
    
    // Navigation property
    public User? User { get; set; }
}
