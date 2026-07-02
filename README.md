# EcoSphere: Kurumsal Sürdürülebilirlik Portalı

EcoSphere, bir şirketin günlük operasyonlarından kaynaklanan çevresel etkileri (karbon ayak izi, kaynak tüketimleri ve atıklar) izlemek, sürdürülebilirlik hedefleri koymak ve resmi sertifikaları/denetimleri yönetmek için geliştirilmiş **ASP.NET Core 10.0 MVC** tabanlı kurumsal bir web portalıdır.

## 🌟 Önemli Özellikler

- **Çift Arayüz Tasarımı (Layouts)**:
  - **Yönetici Paneli (Admin)**: Sol tarafta sabit dikey menü (sidebar) ile sürdürülebilirlik hedefleri, denetimler ve sertifikaların yönetildiği idari panel.
  - **Çalışan Portalı (User)**: Üstten navigasyonlu (Top Navbar), tamamen responsive, karanlık glassmorphism temalı, hızlı tüketim ve atık girişi yapılabilen son kullanıcı web arayüzü.
- **Repository Tasarım Deseni**: Tüm tablolar için kendine özel interface (`IEcoCertificateRepository`, `ISustainabilityGoalRepository` vb.) ve somut Repository sınıfları tanımlanmıştır.
- **EF Core Migrations**: Veritabanı şeması ve tohum verileri (Seed Data) EF Core göçleri ile yönetilmektedir.
- **Çift Tasarımlı Kayıt Ekranı**: `/Account/Register` sayfası Çalışan ve Yönetici seçimine göre doğa yeşili veya indigo moru temalarına bürünür. Yönetici kaydı için güvenlik kodu doğrulaması (`ECO-2026`) gerekir.
- **Çerezsiz Oturum Yönetimi**: Oturumlar ve kullanıcı rolleri cookie veya token kullanılmadan statik `CurrentSession` yapısı ile yönetilmektedir.
- **Beni Hatırla**: `localStorage` tabanlı cookie-less kullanıcı hatırlama desteği.
- **Şifremi Unuttum**: Kullanıcı adı ve Ad Soyad doğrulamasıyla veritabanında şifre sıfırlama modalı.

## 🚀 Başlangıç ve Çalıştırma

1. **Gereksinimler**:
   - .NET 10.0 SDK
   - LocalDB (MS SQL Server Express)

2. **Veritabanını Oluşturma & Migrations**:
   Proje ilk çalıştığında `Program.cs` içerisindeki relational database creator ve migrator sayesinde `EcoSphereDb_v2` veritabanını LocalDB üzerinde tüm göçler ve tohum verilerle otomatik olarak oluşturur.

3. **Uygulamayı Çalıştırma**:
   Terminale gelin ve aşağıdaki komutu girin:
   ```bash
   dotnet run --project EcoSphere.MVC
   ```
   Uygulama ayağa kalktığında tarayıcınızda `http://localhost:7018` (veya belirtilen portta) açılacaktır.

## 👤 Test Hesapları

- **Yönetici (Admin)**:
  - Kullanıcı Adı: `admin`
  - Şifre: `admin123`
- **Çalışan (User)**:
  - Kullanıcı Adı: `user`
  - Şifre: `user123`
