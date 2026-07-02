using System;
using Microsoft.EntityFrameworkCore;
using EcoSphere.Core.Models;

namespace EcoSphere.Data;

public class EcoSphereDbContext : DbContext
{
    public EcoSphereDbContext()
    {
    }

    public EcoSphereDbContext(DbContextOptions<EcoSphereDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<EnergyConsumption> EnergyConsumptions { get; set; } = null!;
    public DbSet<WasteManagement> WasteManagements { get; set; } = null!;
    public DbSet<SustainabilityGoal> SustainabilityGoals { get; set; } = null!;
    public DbSet<EcoCertificate> EcoCertificates { get; set; } = null!;
    public DbSet<SustainabilityAudit> SustainabilityAudits { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EcoSphereDb_v2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships if needed
        modelBuilder.Entity<EnergyConsumption>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WasteManagement>()
            .HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed Users
        var admin = new User
        {
            Id = 1,
            Username = "admin",
            Password = "admin123", // Plain text as requested for basic coding
            FullName = "Ahmet Yılmaz (Sürdürülebilirlik Yöneticisi)",
            Role = "Admin",
            Department = "Yönetim"
        };

        var user = new User
        {
            Id = 2,
            Username = "user",
            Password = "user123",
            FullName = "Mehmet Demir (Tesis Temsilcisi)",
            Role = "User",
            Department = "Üretim"
        };

        modelBuilder.Entity<User>().HasData(admin, user);

        // Seed Sustainability Goals (Admin Side)
        modelBuilder.Entity<SustainabilityGoal>().HasData(
            new SustainabilityGoal
            {
                Id = 1,
                Title = "Yıllık Karbon Ayak İzi Azaltımı",
                TargetMetric = "CO2 Azaltımı",
                TargetValue = 10000.0,
                CurrentValue = 4250.0,
                Unit = "kg CO2",
                TargetDate = new DateTime(2026, 12, 31),
                Status = "Aktif"
            },
            new SustainabilityGoal
            {
                Id = 2,
                Title = "Sıfır Atık Geri Dönüşüm Oranı",
                TargetMetric = "Geri Dönüşüm Oranı",
                TargetValue = 90.0,
                CurrentValue = 72.5,
                Unit = "%",
                TargetDate = new DateTime(2026, 9, 30),
                Status = "Aktif"
            },
            new SustainabilityGoal
            {
                Id = 3,
                Title = "Su Tasarrufu Kampanyası",
                TargetMetric = "Su Tasarrufu",
                TargetValue = 500.0,
                CurrentValue = 500.0,
                Unit = "m3",
                TargetDate = new DateTime(2026, 5, 30),
                Status = "Tamamlandı"
            }
        );

        // Seed Eco Certificates (Admin Side)
        modelBuilder.Entity<EcoCertificate>().HasData(
            new EcoCertificate
            {
                Id = 1,
                CertificateName = "ISO 14001 Çevre Yönetim Sistemi",
                IssuingOrganization = "TSE",
                IssueDate = new DateTime(2025, 6, 1),
                ExpiryDate = new DateTime(2027, 6, 1),
                Status = "Geçerli",
                CertificateImagePath = null
            },
            new EcoCertificate
            {
                Id = 2,
                CertificateName = "Yeşil Ofis Diploması",
                IssuingOrganization = "WWF Türkiye",
                IssueDate = new DateTime(2026, 1, 1),
                ExpiryDate = new DateTime(2026, 12, 31),
                Status = "Yenileme Bekliyor",
                CertificateImagePath = null
            }
        );

        // Seed Sustainability Audits (Admin Side)
        modelBuilder.Entity<SustainabilityAudit>().HasData(
            new SustainabilityAudit
            {
                Id = 1,
                DepartmentName = "Üretim Departmanı",
                AuditorName = "Zeynep Kaya",
                AuditDate = new DateTime(2026, 4, 30),
                Score = 85,
                Findings = "Atık ayrıştırma kutuları düzgün kullanılıyor fakat enerji tasarruf bilinci artırılmalı.",
                Recommendations = "Makine bekleme modları optimize edilmeli."
            },
            new SustainabilityAudit
            {
                Id = 2,
                DepartmentName = "Lojistik Departmanı",
                AuditorName = "Zeynep Kaya",
                AuditDate = new DateTime(2026, 5, 31),
                Score = 68,
                Findings = "Araç rotaları optimize edilmemiş, emisyon yüksek.",
                Recommendations = "Yazılım tabanlı rota optimizasyonuna geçilmeli."
            }
        );

        // Seed Energy Consumptions (User Side)
        modelBuilder.Entity<EnergyConsumption>().HasData(
            new EnergyConsumption
            {
                Id = 1,
                ConsumptionType = "Elektrik",
                Quantity = 1200.0,
                Unit = "kWh",
                DateLogged = new DateTime(2026, 6, 20),
                CO2Equivalent = 480.0, // 1200 * 0.4
                UserId = 2
            },
            new EnergyConsumption
            {
                Id = 2,
                ConsumptionType = "Doğalgaz",
                Quantity = 350.0,
                Unit = "m3",
                DateLogged = new DateTime(2026, 6, 25),
                CO2Equivalent = 700.0, // 350 * 2.0
                UserId = 2
            }
        );

        // Seed Waste Managements (User Side)
        modelBuilder.Entity<WasteManagement>().HasData(
            new WasteManagement
            {
                Id = 1,
                WasteType = "Plastik",
                WeightKg = 45.0,
                IsRecycled = true,
                DateLogged = new DateTime(2026, 6, 22),
                ReceiptImagePath = null,
                UserId = 2
            },
            new WasteManagement
            {
                Id = 2,
                WasteType = "Kağıt",
                WeightKg = 120.0,
                IsRecycled = true,
                DateLogged = new DateTime(2026, 6, 27),
                ReceiptImagePath = null,
                UserId = 2
            }
        );
    }
}
