using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoSphere.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EcoCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuingOrganization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificateImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcoCertificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SustainabilityAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SustainabilityAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SustainabilityGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetMetric = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetValue = table.Column<double>(type: "float", nullable: false),
                    CurrentValue = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SustainabilityGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnergyConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumptionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateLogged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CO2Equivalent = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyConsumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnergyConsumptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WasteManagements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WasteType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: false),
                    IsRecycled = table.Column<bool>(type: "bit", nullable: false),
                    DateLogged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiptImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WasteManagements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EcoCertificates",
                columns: new[] { "Id", "CertificateImagePath", "CertificateName", "ExpiryDate", "IssueDate", "IssuingOrganization", "Status" },
                values: new object[,]
                {
                    { 1, null, "ISO 14001 Çevre Yönetim Sistemi", new DateTime(2027, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "TSE", "Geçerli" },
                    { 2, null, "Yeşil Ofis Diploması", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "WWF Türkiye", "Yenileme Bekliyor" }
                });

            migrationBuilder.InsertData(
                table: "SustainabilityAudits",
                columns: new[] { "Id", "AuditDate", "AuditorName", "DepartmentName", "Findings", "Recommendations", "Score" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Zeynep Kaya", "Üretim Departmanı", "Atık ayrıştırma kutuları düzgün kullanılıyor fakat enerji tasarruf bilinci artırılmalı.", "Makine bekleme modları optimize edilmeli.", 85 },
                    { 2, new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Zeynep Kaya", "Lojistik Departmanı", "Araç rotaları optimize edilmemiş, emisyon yüksek.", "Yazılım tabanlı rota optimizasyonuna geçilmeli.", 68 }
                });

            migrationBuilder.InsertData(
                table: "SustainabilityGoals",
                columns: new[] { "Id", "CurrentValue", "Status", "TargetDate", "TargetMetric", "TargetValue", "Title", "Unit" },
                values: new object[,]
                {
                    { 1, 4250.0, "Aktif", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "CO2 Azaltımı", 10000.0, "Yıllık Karbon Ayak İzi Azaltımı", "kg CO2" },
                    { 2, 72.5, "Aktif", new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Geri Dönüşüm Oranı", 90.0, "Sıfır Atık Geri Dönüşüm Oranı", "%" },
                    { 3, 500.0, "Tamamlandı", new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Su Tasarrufu", 500.0, "Su Tasarrufu Kampanyası", "m3" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Department", "FullName", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Yönetim", "Ahmet Yılmaz (Sürdürülebilirlik Yöneticisi)", "admin123", "Admin", "admin" },
                    { 2, "Üretim", "Mehmet Demir (Tesis Temsilcisi)", "user123", "User", "user" }
                });

            migrationBuilder.InsertData(
                table: "EnergyConsumptions",
                columns: new[] { "Id", "CO2Equivalent", "ConsumptionType", "DateLogged", "Quantity", "Unit", "UserId" },
                values: new object[,]
                {
                    { 1, 480.0, "Elektrik", new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1200.0, "kWh", 2 },
                    { 2, 700.0, "Doğalgaz", new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 350.0, "m3", 2 }
                });

            migrationBuilder.InsertData(
                table: "WasteManagements",
                columns: new[] { "Id", "DateLogged", "IsRecycled", "ReceiptImagePath", "UserId", "WasteType", "WeightKg" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 2, "Plastik", 45.0 },
                    { 2, new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, 2, "Kağıt", 120.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnergyConsumptions_UserId",
                table: "EnergyConsumptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WasteManagements_UserId",
                table: "WasteManagements",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcoCertificates");

            migrationBuilder.DropTable(
                name: "EnergyConsumptions");

            migrationBuilder.DropTable(
                name: "SustainabilityAudits");

            migrationBuilder.DropTable(
                name: "SustainabilityGoals");

            migrationBuilder.DropTable(
                name: "WasteManagements");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
