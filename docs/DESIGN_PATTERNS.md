# Design Patterns – Tasarım Kararları ve Gerekçeleri

Bu dokümanda projede kullanılan tasarım desenleri ve neden tercih edildikleri açıklanmaktadır.

---

## 1. Onion Architecture (Soğan Mimarisi)

**Nerede:** Tüm mikroservisler (Auth, Product, Log)

**Yapı:** Domain → Application → Infrastructure → API (dıştan içe bağımlılık)

| Katman | Açıklama |
|--------|----------|
| **Domain** | Entity’ler, repository interface’leri. Hiçbir dış katmana bağımlı değildir. |
| **Application** | İş kuralları, CQRS handler’lar, event’ler. Sadece Domain’e bağlıdır. |
| **Infrastructure** | Veritabanı, harici servisler, repository implementasyonları. Application’a bağlıdır. |
| **API** | Controller’lar, DI kayıtları. Sadece giriş noktasıdır. |

**Neden:** Bağımlılıklar tek yönlü, test kolay, veritabanı veya framework değiştiğinde sadece Infrastructure etkilenir.

---

## 2. CQRS (Command Query Responsibility Segregation)

**Nerede:** MediatR ile tüm servislerde

**Yapı:**
- **Command:** `CreateProductCommand`, `RegisterCommand` – durum değiştiren işlemler
- **Query:** `GetAllProductsQuery`, `GetLogsByLevelQuery` – okuma işlemleri

```
Controller → IMediator.Send(command/query) → Handler → Repository
```

**Neden:** Okuma ve yazma sorumlulukları ayrılır; her handler tek bir iş yapar. Genişletme ve test kolaylaşır.

---

## 3. Repository Pattern

**Nerede:** `IProductRepository`, `IUserRepository`, `ILogRepository`

**Yapı:** Domain’de interface, Infrastructure’da implementation (EF Core kullanımı)

**Neden:** Veri erişimi soyutlanır; uygulama katmanı veritabanı detayından bağımsız kalır. Mock’lama ve test kolaylaşır.

---

## 4. Domain-Driven Design (DDD)

**Nerede:** `Product`, `ApplicationUser`, `LogEntry`, `RefreshToken` entity’leri

**Uygulananlar:**
- **Rich Domain Model:** `private set`, domain metotları (`Update`, `UpdateStock`, `Activate`)
- **Entity içinde validasyon:** Constructor ve metotlarda null/negatif kontrolü
- **Encapsulation:** State doğrudan set edilmez, domain metotları ile yönetilir

**Örnek (Product):**
```csharp
public void UpdateStock(int quantity)  // İş kuralı entity içinde
{
    if (Stock + quantity < 0) throw new InvalidOperationException("Insufficient stock");
    Stock += quantity;
}
```

**Neden:** İş kuralları entity içinde toplanır; tutarsız state oluşumu engellenir.

---

## 5. Event-Driven Architecture (Pub/Sub)

**Nerede:** RabbitMQ EventBus, `ProductCreatedEvent`, `ProductUpdatedEvent`

**Yapı:**
- Product API ürün eklendiğinde `ProductCreatedEvent` yayınlar
- Log API bu event’e abone olur, log kaydı oluşturur

**Neden:** Servisler gevşek bağlı kalır; Log API doğrudan Product’ı çağırmaz. Yeni event tüketicileri kolay eklenir.

---

## 6. API Gateway Pattern

**Nerede:** Yarp Reverse Proxy (5000 portu)

**Yapı:** Tek giriş noktası → Auth / Product / Log servislerine yönlendirme

**Neden:** Rate limiting, JWT doğrulama, routing tek noktada toplanır; backend servisler sadece iş mantığına odaklanır.

---

## 7. Cache-Aside Pattern

**Nerede:** Product API – Redis ile `GetAllProductsQuery`

**Yapı:**
- Önce cache’e bak
- Yoksa veritabanından oku, cache’e yaz
- Update/Delete sonrası `CacheInvalidationService` ile ilgili anahtarlar temizlenir

**Neden:** Okuma yükü azalır, cache ile veritabanı tutarlılığı invalidation ile korunur.

---

## 8. Dependency Injection (Constructor Injection)

**Nerede:** Tüm handler’lar, controller’lar, repository’ler

**Yapı:** Constructor üzerinden interface enjeksiyonu; `AddScoped`, `AddSingleton` ile DI container’da kayıt

**Neden:** Bağımlılıklar açık; testte mock interface kullanımı kolay; SOLID prensipleriyle uyumlu.

---

## 9. FluentValidation (Validation Pipeline)

**Nerede:** `RegisterCommandValidator`, `CreateProductCommandValidator`

**Yapı:** Her command için ayrı validator; MediatR pipeline ile otomatik çalıştırılır

**Neden:** Validasyon kuralları tek yerde toplanır; Controller sadeleşir; hata mesajları tutarlı hale gelir.

---

## 10. Strategy Pattern (implicit)

**Nerede:** Event Handler’lar (`ProductCreatedEventHandler`, `ProductUpdatedEventHandler`)

**Yapı:** Her event tipi için farklı handler; EventBus runtime’da doğru handler’ı seçer

**Neden:** Yeni event tipleri eklenirken mevcut kod değişmez; Open/Closed prensibine uyumlu.
