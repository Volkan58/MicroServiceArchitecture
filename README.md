# MicroService Architecture

.NET 10 tabanlı mikroservis mimarisi örneği. API Gateway, Auth, Product ve Log servislerini içerir.

## Mimari

- **API Gateway** (5000) – Tüm isteklerin giriş noktası
- **Auth API** – Kimlik doğrulama (JWT, Microsoft Identity)
- **Product API** – Ürün yönetimi
- **Log API** – Merkezi loglama
- **SQL Server** – Auth (1433), Product (1434), Log (1435) veritabanları
- **Redis** (6379) – Cache
- **RabbitMQ** (5672, 15672) – Mesajlaşma

## Gereksinimler

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

## Docker ile Çalıştırma

Projeyi klonladıktan sonra tek komutla tüm servisleri ayağa kaldırabilirsiniz:

```bash
# Proje dizinine geç
cd MicroServiceArchitecture

# Tüm servisleri build edip başlat
docker-compose up -d --build
```

İlk build 5–10 dakika sürebilir. Servisler hazır olduğunda:

| Servis        | URL                        |
|---------------|----------------------------|
| API Gateway   | http://localhost:5000      |
| RabbitMQ UI   | http://localhost:15672 (guest/guest) |

## API Gateway Routing

API versiyonlama (v1) kullanılmaktadır. Gateway üzerinden erişim örnekleri:

- Auth: `http://localhost:5000/api/v1/auth/*`
- Product: `http://localhost:5000/api/v1/products/*`
- Log: `http://localhost:5000/api/v1/logs/*`

## Durdurma ve Temizlik

```bash
# Servisleri durdur
docker-compose down

# Servisleri + volume'ları sil
docker-compose down -v

# Tam temizlik (tüm container, image, volume)
docker system prune -a -f --volumes
```

## Yerel Geliştirme (Docker olmadan)

1. SQL Server, Redis, RabbitMQ için Docker’da sadece altyapı servislerini çalıştırın:
   ```bash
   docker-compose up -d sqlserver-auth sqlserver-product sqlserver-log redis rabbitmq
   ```
2. Her API’yi Visual Studio veya `dotnet run` ile ayrı ayrı başlatın.
3. `appsettings.json` içinde connection string’leri localhost’a göre ayarlayın.

## API Versiyonlama

Tüm API'ler **v1** ile versiyonlanmıştır. İleride breaking change yapıldığında v2 eklenebilir, eski client'lar v1 ile çalışmaya devam eder.

| Versiyon | Base Path      | Durum   |
|----------|----------------|---------|
| v1       | /api/v1/*      | Aktif   |

## Kod Açıklamaları ve Design Patterns

Projede kullanılan tasarım desenleri ve karar gerekçeleri için: [Design Patterns Dokümantasyonu](docs/DESIGN_PATTERNS.md)

## Proje Yapısı

```
src/
├── Gateway/
│   └── ApiGateway/
├── Services/
│   ├── Auth/
│   │   ├── Auth.API
│   │   ├── Auth.Application
│   │   ├── Auth.Domain
│   │   └── Auth.Infrastructure
│   ├── Log/
│   │   ├── Log.API
│   │   ├── Log.Application
│   │   ├── Log.Domain
│   │   └── Log.Infrastructure
│   └── Product/
│       ├── Product.API
│       ├── Product.Application
│       ├── Product.Domain
│       └── Product.Infrastructure
└── BuildingBlocks/
    ├── EventBus
    └── EventBus.RabbitMQ
```

## Lisans

MIT
