using System;

namespace EcoSphere.Core.Models;

public class EcoCertificate
{
    public int Id { get; set; }
    public string CertificateName { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? CertificateImagePath { get; set; } // Uploaded certificate image path
    public string Status { get; set; } = "Geçerli"; // "Geçerli", "Yenileme Bekliyor", "Süresi Doldu"
}
