using System;

namespace EcoSphere.Core.Models;

public class SustainabilityAudit
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string AuditorName { get; set; } = string.Empty;
    public DateTime AuditDate { get; set; }
    public int Score { get; set; } // 1-100
    public string Findings { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
}
