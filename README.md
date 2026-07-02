# EcoSphere: Kurumsal Sürdürülebilirlik Portalı

EcoSphere, bir şirketin günlük operasyonlarından kaynaklanan çevresel etkileri (karbon ayak izi, kaynak tüketimleri ve atıklar) izlemek, sürdürülebilirlik hedefleri koymak ve resmi sertifikaları/denetimleri yönetmek için geliştirilmiş **ASP.NET Core 10.0 MVC** tabanlı kurumsal bir web portalıdır.

---

## 🛠️ Kullanılan Teknolojiler ve Mimari Yapı

Proje, kurumsal yazılım standartlarına uygun olarak katmanlı mimari (N-Tier Architecture), tasarım desenleri (Design Patterns) ve modern web teknolojileri kullanılarak geliştirilmiştir.

### 1. Backend & Framework Teknolojileri
*   **Çalışma Zamanı (Runtime):** `.NET 10.0 SDK`
*   **Web Framework:** `ASP.NET Core MVC (Model-View-Controller)`
*   **Modüler Alanlar (Areas):** İdari yönetim ve çalışan işlemlerini fiziksel olarak ayırmak için **Areas** yapısı kullanılmıştır:
    *   `Areas/Admin`: Hedeflerin, denetimlerin ve sertifikaların yönetildiği idari kısım.
    *   `Areas/User`: Çalışanların enerji ve atık kaydı girdiği portal arayüzü.
    *   `Root (Ana Dizin)`: Giriş, çıkış ve ortak Dashboard istatistikleri.

### 2. Veri Erişim Katmanı (Data Access Layer)
*   **ORM (Object-Relational Mapping):** `Entity Framework Core 10.0`
*   **Veritabanı:** `Microsoft SQL Server LocalDB ((localdb)\MSSQLLocalDB)`
*   **Veritabanı Yönetimi:** `EF Core Migrations` (Kod öncelikli - Code First yaklaşımıyla veritabanı şeması ve başlangıç tohum verileri [Seed Data] otomatik olarak yönetilir).
*   **Repository Tasarım Deseni:** Generic repository (`IRepository<T>`) taban alınarak her tabloya özel veri erişim arayüzleri (`IUserRepository`, `IEnergyConsumptionRepository`, `IWasteManagementRepository`, vb.) tanımlanmış ve DI konteynerine kaydedilmiştir.
*   **Unit of Work Tasarım Deseni:** Bütün yazma işlemlerini tek bir veritabanı işlemi (transaction) üzerinden atomik olarak yürütmek için `IUnitOfWork` ve `UnitOfWork` sınıfları entegre edilmiştir. Generic repository içindeki otomatik `SaveChangesAsync` çağrıları kaldırılarak kontrol tamamen Unit of Work üzerindeki `CompleteAsync()` metoduna devredilmiştir.

### 3. Kullanılan Kütüphaneler (NuGet Packages)
*   **EPPlus (v7+):** Çalışan ve yöneticilerin tüketim verilerini `.xlsx` biçiminde Excel raporu olarak indirebilmesi için kullanılmıştır.
*   **QuestPDF:** Kurumun karbon ayak izini ve atık geri dönüşüm istatistiklerini içeren modern, logolu ve tablolu PDF Sürdürülebilirlik Karnesi raporunun oluşturulması için kullanılmıştır (Community lisansı aktif edilmiştir).
*   **Serilog & Serilog.AspNetCore:** Uygulama içerisindeki tüm kritik olayları (giriş denemeleri, veri ekleme, şifre sıfırlama, rapor indirmeleri) konsola ve günlük olarak oluşturulan `logs/ecosphere_log.txt` dosyasına yapılandırılmış (structured) biçimde kaydeder.
*   **Microsoft.EntityFrameworkCore.SqlServer:** MS SQL Server/LocalDB bağlantısı için EF Core sağlayıcısı.

### 4. Frontend & Tasarım (User Interface)
*   **Şablon Motoru:** `Razor Pages (.cshtml)`
*   **Tasarım Dili (CSS):** Kurumsal yeşil doğa tonları (#10b981) ve asil koyu gri tonlarla tasarlanmış tamamen özel **Vanilla CSS** (`wwwroot/css/site.css`).
*   **Cam Efekti (Glassmorphism):** CSS `backdrop-filter: blur(12px)` ve yarı saydam koyu renk zeminler kullanılarak modern ve lüks bir web portalı hissiyatı oluşturulmuştur.
*   **Responsive Tasarım (Mobil Uyum):** Ekran genişliğine göre dikey hizalanan menüler, tek sütuna düşen grid kartları ve taşmayı önleyen yatay kaydırılabilir tablo kapsayıcıları (`.table-wrapper`) kullanılmıştır.
*   **İnteraktif Görsel Önizleme:** Yüklenen atık fişleri ve sertifika görsellerinin üzerine tıklandığında ekranı kaplayan özel Javascript modal penceresi (`globalImageModal`) entegre edilmiştir.

### 5. Oturum Yönetimi & Güvenlik
*   **Çerezsiz Oturum (Cookie-less Session):** Tarayıcı çerezleri veya oturum tokenleri kullanılmadan, sunucu tarafında oluşturulan statik `CurrentSession` sınıfı ile kimlik doğrulama ve rol yönetimi (Admin/User yetki denetimi) sağlanır.
*   **Beni Hatırla (Remember Me):** Tarayıcı yerel depolama alanı (`localStorage`) kullanılarak çerezsiz kullanıcı adı hatırlama desteği sunulmuştur.
*   **Şifre Sıfırlama (Forgot Password):** Giriş yapamayan kullanıcılar için kullanıcı adı ve ad-soyad doğrulamasıyla çalışan, veritabanı entegreli ve şifre güncelleyen güvenli modal ekranı.
*   **Benzersiz Dosya Yükleme:** Yüklenen görseller (sertifikalar, atık fişleri) çakışmaları önlemek için `Guid.NewGuid().ToString("N")` algoritmasıyla isimlendirilerek `wwwroot/uploads` altına kaydedilir.

---

## 🚀 Başlangıç ve Çalıştırma

1. **Gereksinimler:**
   - .NET 10.0 SDK
   - LocalDB (MS SQL Server Express LocalDB)

2. **Veritabanının Otomatik Oluşturulması:**
   Proje ilk çalıştığında `Program.cs` içerisindeki relational database creator ve migrator sayesinde `EcoSphereDb_v2` veritabanını LocalDB üzerinde tüm göçler ve tohum verilerle otomatik olarak oluşturur. Ekstra bir SQL komutu çalıştırmanıza gerek yoktur.

3. **Uygulamayı Çalıştırma:**
   Terminal üzerinden projenin ana dizininde şu komutu çalıştırın:
   ```bash
   dotnet run --project EcoSphere.MVC
   ```
   Uygulama ayağa kalktığında tarayıcınızdan `http://localhost:7018` adresine giderek portala erişebilirsiniz.

---

## 👤 Test Hesapları ve Giriş Bilgileri

*   **Yönetici (Admin):**
    *   Kullanıcı Adı: `admin`
    *   Şifre: `admin123`
*   **Çalışan (User):**
    *   Kullanıcı Adı: `user`
    *   Şifre: `user123`

---

## 📸 Uygulama Görselleri

### 👤 Kullanıcı Portalı

<p align="center">
<img width="1873" height="1007" alt="image" src="https://github.com/user-attachments/assets/bd1f5664-add6-4ec2-a126-b2249a8c6559" />
</p>

---

### 👥 Kullanıcı Portalı Enerji Kayıtları Yönetim Ekranı

<p align="center">
<img width="1157" height="688" alt="image" src="https://github.com/user-attachments/assets/720eebd6-56bb-422a-922c-220774ab485e" />
</p>

---

### 👥 Kullanıcı Portalı Atık Yönetim Ekranı

<p align="center">
<img width="1152" height="495" alt="image" src="https://github.com/user-attachments/assets/7ce9c129-0e3c-4fe7-a272-23f8c13847f5" />
</p>

---

### 🔐 Admin Panel

<p align="center">
<img width="1905" height="1010" alt="image" src="https://github.com/user-attachments/assets/87996561-756e-41e7-bb2c-d3e1162d9724" />
</p>

---

### 🧾 Hedef Yönetimi Ekranı

<p align="center">
<img width="1631" height="743" alt="image" src="https://github.com/user-attachments/assets/2bd5b2ce-1809-4bfd-a00e-40a1ececed67" />
</p>

---


### 🛒 Sertifika Yönetim Ekranı

<p align="center">
<img width="1637" height="752" alt="image" src="https://github.com/user-attachments/assets/8531cbd0-c8f0-4205-90c3-5410cadb6c6e" />
</p>

---

### 📦 Denetim Yönetim Ekranı

<p align="center">
<img width="1637" height="747" alt="image" src="https://github.com/user-attachments/assets/ae5c431f-cb95-4e74-a90a-d6e9f939ec93" />
</p>

