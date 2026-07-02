namespace EcoSphere.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Store as plain text or simple hash
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Admin" or "User"
    public string Department { get; set; } = string.Empty; // e.g. "IT", "HR", "Logistics", "Operations"
}
