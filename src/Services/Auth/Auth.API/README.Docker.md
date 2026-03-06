# Auth.API - Docker Kurulumu

## Seçenek 1: Tam Docker (SQL + API birlikte)

```bash
cd Auth.API
docker-compose up
```

SQL Server ve Auth.API container'da çalışır. API: http://localhost:5002

## Seçenek 2: Sadece SQL Server (API yerelde)

**Adım 1:** SQL Server'ı başlat
```bash
cd Auth.API
docker-compose -f docker-compose.dev.yml up -d
```

**Adım 2:** Projeyi Visual Studio/Rider ile F5 ile veya:
```bash
dotnet run --launch-profile Docker
```

## Seçenek 3: Otomatik başlatma (PowerShell)

Projeyi başlatırken SQL'in de otomatik ayağa kalkması için:
```powershell
.\run-dev.ps1
```

## Bağlantı Bilgileri

- **Server:** localhost,1433 (yerel) / sqlserver (container içi)
- **Database:** AuthDb
- **User:** sa
- **Password:** w0Lkan@Passw0rd

> Not: İlk çalıştırmada `dotnet ef database update` ile veritabanını oluşturmanız gerekebilir.
