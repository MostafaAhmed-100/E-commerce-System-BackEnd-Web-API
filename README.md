# 🛒 E-Commerce REST API V3

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-388E3C?style=for-the-badge&logo=nuget&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)

A full-featured, production-ready E-Commerce backend built with **ASP.NET Core 8 Web API**. The system supports three distinct roles — **Admin**, **Seller**, and **Buyer** — with enterprise-grade architecture, comprehensive authentication, advanced business logic, structured logging, background jobs, and complete bilingual localization (Arabic + English).

> **🎉 Project Status: Complete (V3).** All features implemented end-to-end across data layer, repositories, services, DTOs, validation, controllers, and comprehensive unit testing.

## 📋 Table of Contents

- [Features & Modules](#-features--modules)
- [Architecture & Design Patterns](#️-architecture--design-patterns)
- [Technology Stack](#️-technology-stack)
- [Project Structure](#-project-structure)
- [Authentication & Authorization](#-authentication--authorization)
- [API Endpoints Overview](#-api-endpoints-overview)
- [Key Business Features](#-key-business-features)
- [Performance Optimization](#-performance-optimization)
- [Testing & Quality Assurance](#-testing--quality-assurance)
- [Configuration & Setup](#️-configuration--setup)
- [Getting Started](#️-getting-started)
- [Contributors](#-contributors)
- [Mentorship](#-mentorship)
- [License](#-license)

---

## 🚀 Features & Modules

### ✅ V1 — Core E-Commerce Foundation
- Product & Category management with hierarchy support
- Product variants (SKU, size, color, price, stock)
- Shopping cart with quantity management
- Order processing with coupon support
- Role-based access control (Admin/Seller/Buyer)

### ✅ V2 — Advanced Business Logic
- Sophisticated stock management with reserved quantities
- Auto-cancellation of unpaid orders via Hangfire
- Payment gateway integration with webhook support
- Saved payment cards (tokenized)
- Review & Rating system on product variants
- Wishlist management with smart toggling

### ✅ V3 — Enterprise Hardening & Advanced Features
- **Email Verification** — Required before login
- **Password Recovery** — Secure tokenized reset flow
- **Refresh Token Rotation** — 1-year access token lifecycle
- **Loyalty Points System** — Earn on purchase, spend at checkout, auto-refund on cancellation
- **Health Monitoring** — SQL Server connectivity + Live dashboard
- **Advanced Logging** — Serilog structured logging with request tracking
- **CI/CD Pipeline** — GitHub Actions (build + test automation)
- **Comprehensive Testing** — NUnit + Moq with high service-layer coverage
- **Bilingual Localization** — Full Arabic + English support (Accept-Language header)
- **Rate Limiting** — Three policies protecting Auth, Checkout, and Browsing endpoints

---

## 🏗️ Architecture & Design Patterns

The project embodies enterprise-level design principles applied consistently across every layer:

### Clean Architecture Separation
- **Controllers** — Thin request handlers that delegate to services
- **Services** — Pure business logic, completely HTTP-agnostic
- **Repositories** — Data access abstraction layer
- **DTOs** — Strict input/output contracts with role-based isolation
- **Entities** — Clean POCOs with no HTTP dependencies

### Repository & Unit of Work Pattern
- **GenericRepository** — Handles standard CRUD operations
- **SpecificRepositories** — Complex domain queries (Include, AsSplitQuery)
- **IUnitOfWork** — Centralized transaction management; every service operates through it for atomic commits/rollbacks
- **Transactional Integrity** — All multi-repository operations commit as a single atomic unit

### Dependency Injection (DI)
- Controllers and services depend on interfaces, not implementations
- Fully decoupled, independently testable architecture
- All services registered in `Program.cs` following the Dependency Injection container pattern

### Service Layer & DTO Mapping
- Business logic encapsulated in dedicated services
- AutoMapper profiles handle entity ↔ DTO transformations
- Paged results standardized through unified response wrapper
- Role-specific DTOs prevent data leakage between Admin/Seller/Buyer

### Validation Pipeline
- **FluentValidation** rules isolated from DTOs
- Global `ValidationFilter` intercepts requests before service execution
- Property-based grouped errors (e.g., `{ "Email": [...], "Password": [...] }`)
- Bilingual error messages (Arabic + English) based on `Accept-Language` header

### Soft Deletion Strategy
- Every entity carries an `IsDeleted` flag
- EF Core global query filters automatically exclude deleted records
- No manual `.Where(x => !x.IsDeleted)` needed throughout the codebase
- Zero data loss, GDPR-friendly record management

### Global Exception Handling
- Centralized `ExceptionMiddleware` catches all unhandled exceptions
- Exceptions converted to standardized HTTP responses
- Prevents stack trace leakage in production
- Full logging via Serilog with request path context

### Structured Logging
- **Serilog** integrated into service layer and middleware
- Structured log events (not plain text) for queryability
- Request tracking for debugging production issues
- Info/Warning/Error trails for business-critical operations

### Authentication & Authorization
- **JWT Bearer tokens** with configurable expiration
- **Refresh Token rotation** for long-lived sessions (1-year lifecycle)
- **Email verification** required before first login
- **Policy-based authorization** (can be extended: AdminOnly, SellerOnly, BuyerOnly)
- **Role claims** in token payload (NameIdentifier, Email, Role, ProfileId)

### Performance Optimization
- **AsNoTracking** on all read-only queries (browsing, filtering)
- **AsSplitQuery** to prevent Cartesian explosions on multi-collection includes
- **True DB Pagination** — Skip/Take executed at SQL Server level, not in-memory
- **Eager Loading with Include** — Eliminates N+1 query problems

### Rate Limiting
- **AuthPolicy** — Protects login/register/password-reset from brute-force
- **CheckoutPolicy** — Prevents order-spam from malicious checkout attempts
- **BrowsingPolicy** — Relaxed limits for product/category browsing
- Returns `429 Too Many Requests` when limits exceeded

---

## 🛠️ Technology Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 8 Web API |
| **Language** | C# 11 |
| **ORM** | Entity Framework Core |
| **Database** | Microsoft SQL Server |
| **Authentication** | ASP.NET Core Identity, JWT Bearer, Refresh Tokens |
| **Mapping & Validation** | AutoMapper, FluentValidation |
| **Logging** | Serilog (File & Console Sinks) |
| **Email Service** | MailKit & MimeKit (SMTP) |
| **Background Jobs** | Hangfire + SQL Server persistence |
| **Testing** | NUnit, Moq |
| **Health Monitoring** | ASP.NET Core Health Checks + SQL Server probe |
| **API Documentation** | Swagger / OpenAPI |
| **CI/CD** | GitHub Actions |
| **Localization** | ASP.NET Core Request Localization (.resx files) |

---

## 📁 Project Structure

```
├── Controllers/                      # API endpoints (thin layer)
│   ├── AuthController.cs
│   ├── AccountController.cs
│   ├── ProductController.cs
│   ├── OrderController.cs
│   ├── WishlistController.cs
│   ├── ReviewController.cs
│   ├── HealthController.cs
│   └── ...
│
├── Services/                         # Business logic (HTTP-agnostic)
│   ├── AuthService.cs
│   ├── ProductService.cs
│   ├── OrderService.cs
│   ├── CartService.cs
│   ├── ReviewService.cs
│   ├── WishlistService.cs
│   ├── LoyaltyPointsService.cs
│   └── ...
│
├── Repository/
│   ├── GenericRepository.cs          # Base CRUD
│   ├── SpecificRepository/           # Domain-specific queries
│   │   ├── ProductRepository.cs
│   │   ├── OrderRepository.cs
│   │   ├── ReviewRepository.cs
│   │   └── ...
│   ├── IUnitOfWork.cs
│   └── UnitOfWork.cs                 # Transaction management
│
├── Entities/                         # EF Core POCOs
│   ├── Product.cs
│   ├── ProductVariant.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Review.cs
│   ├── Wishlist.cs
│   ├── Cart.cs
│   ├── LoyaltyPoints.cs
│   ├── SavedCard.cs
│   ├── Category.cs
│   └── ...
│
├── DTOs/
│   ├── Request_DTOs/
│   │   ├── ProductRequestDTO.cs
│   │   ├── OrderCreateRequestDTO.cs
│   │   ├── ReviewRequestDTO.cs
│   │   └── ...
│   └── Response_DTOs/
│       ├── ProductResponseDTO.cs
│       ├── OrderResponseDTO.cs
│       ├── ReviewResponseDTO.cs
│       ├── ApiResponseDto.cs           # Unified wrapper
│       └── ...
│
├── Validators/                       # FluentValidation rules
│   ├── ProductValidator.cs
│   ├── OrderValidator.cs
│   ├── ReviewValidator.cs
│   └── ...
│
├── Mappings/                         # AutoMapper Profiles
│   ├── ProductMappingProfile.cs
│   ├── OrderMappingProfile.cs
│   ├── ReviewMappingProfile.cs
│   └── ...
│
├── Filters/                          # Action Filters
│   └── ValidationFilter.cs            # Global validation interceptor
│
├── Middlewares/                      # Custom pipeline
│   ├── ExceptionMiddleware.cs         # Global exception handler
│   └── LoggingMiddleware.cs           # Request/response logging
│
├── Exceptions/                       # Custom domain exceptions
│   ├── NotFoundException.cs
│   ├── BadRequestException.cs
│   ├── UnauthorizedException.cs
│   ├── ConflictException.cs
│   └── ValidationException.cs
│
├── BackgroundJobs/                   # Hangfire definitions
│   ├── OrderCancellationJob.cs        # Auto-cancel unpaid orders
│   └── ...
│
├── HealthChecks/                     # Custom health probes
│   └── SqlServerHealthCheck.cs
│
├── Resources/                        # Localization files
│   ├── ValidationMessages.en.resx
│   ├── ValidationMessages.ar.resx
│   └── ...
│
├── Tests/                            # NUnit test suite
│   ├── Services/
│   │   ├── OrderServiceTests.cs
│   │   ├── ProductServiceTests.cs
│   │   ├── ReviewServiceTests.cs
│   │   ├── WishlistServiceTests.cs
│   │   └── ...
│   └── Fixtures/                     # Test data builders
│
├── Data/                             # EF Core context
│   ├── AppDbContext.cs
│   ├── Configurations/
│   │   ├── ProductConfiguration.cs
│   │   ├── OrderConfiguration.cs
│   │   ├── ReviewConfiguration.cs
│   │   └── ...
│   └── Migrations/                   # Database migrations
│
├── Program.cs                        # DI Container + Pipeline setup
└── appsettings.json                  # Configuration
```

---

## 🔐 Authentication & Authorization

### JWT Bearer Token Flow

```
1. Register (Buyer/Seller/Admin) with email
   → Confirmation email dispatched
   
2. Click confirmation link in email
   → Email verified, account activated
   
3. Login with email + password
   → JWT + Refresh Token issued
   
4. Include JWT in subsequent requests
   → Authorization: Bearer <access_token>
   
5. When access token expires
   → Use Refresh Token to obtain new pair
   
6. Forgot Password
   → Reset link emailed with secure token
   → Set new password securely
```

### Token Structure

```json
{
  "NameIdentifier": "user-id-uuid",
  "Email": "buyer@example.com",
  "Role": "Buyer",
  "ProfileId": "buyer-profile-id",
  "exp": 1609459200,
  "iat": 1609459200
}
```

### Authorization Policies

- **AdminOnly** — Manage categories, update any order status
- **SellerOnly** — Create/manage own products, variants, coupons (requires National ID verification)
- **BuyerOnly** — Browse, cart, orders, wishlists, reviews

---

## 📦 API Endpoints Overview

### 🔐 Auth — `/api/Auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Login` | ❌ | Login (requires confirmed email) |
| POST | `/Register` | ❌ | Register as Buyer |
| POST | `/Register-Seller` | ❌ | Register as Seller |
| POST | `/Register-Admin` | ❌ | Register as Admin (secret key required) |
| GET | `/Confirm-Email` | ❌ | Verify email via token |
| POST | `/Forgot-Password` | ❌ | Send password reset link |
| POST | `/Reset-Password` | ❌ | Reset password using token |
| POST | `/Refresh-Token` | ❌ | Issue new token pair |

### 👤 Account — `/api/Account`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/Profile` | ✅ Any | Retrieve role-specific profile |
| PUT | `/Profile` | ✅ Any | Update profile with validation |
| POST | `/Change-Password` | ✅ Any | Securely change password |
| DELETE | `/Delete-Account` | ✅ Any | Soft-delete own account |

### ❤️ Wishlist — `/api/Wishlist`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/` | ✅ Buyer | Create wishlist |
| GET | `/` | ✅ Buyer | Get all wishlists |
| DELETE | `/{wishlistId}` | ✅ Buyer | Delete wishlist |
| POST | `/{wishlistId}/toggle-item` | ✅ Buyer | Add/remove product variant |
| GET | `/{wishlistId}/items` | ✅ Buyer | Paginated wishlist items |

### ⭐ Review — `/api/Review`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/` | ✅ Buyer | Submit review (purchase validated) |
| PUT | `/{reviewId}` | ✅ Buyer | Update own review |
| DELETE | `/{reviewId}` | ✅ Buyer | Delete own review |
| GET | `/variant/{variantId}` | ❌ | Get paginated reviews |
| GET | `/variant/{variantId}/summary` | ❌ | Get rating summary |

### 🏪 Product — `/api/Product`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Product` | ✅ Seller | Create product with variants |
| PUT | `/Update-Product/{productId}` | ✅ Seller | Update product |
| DELETE | `/Delete-Product/{productId}` | ✅ Seller | Soft-delete product |
| GET | `/Get-Product/{productId}` | ❌ | Get product by ID |
| GET | `/Get-All` | ❌ | Get all products (paginated, filterable) |
| GET | `/Out-Of-Stock` | ✅ Seller | Get seller's out-of-stock products |

### 🛒 Cart — `/api/Cart`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/My-Cart` | ✅ Buyer | View current cart |
| POST | `/Add-Item` | ✅ Buyer | Add item to cart |
| PUT | `/Update-Quantity/{variantId}` | ✅ Buyer | Update quantity |
| DELETE | `/Remove-Item/{variantId}` | ✅ Buyer | Remove item |
| DELETE | `/Clear-Cart` | ✅ Buyer | Clear entire cart |

### 📋 Order — `/api/Order`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Order` | ✅ Buyer | Create order from cart (coupon support) |
| GET | `/My-Orders` | ✅ Buyer | Get all buyer orders |
| GET | `/Get-Order/{orderId}` | ✅ Buyer | Get order details |
| DELETE | `/Cancel-Order/{orderId}` | ✅ Buyer | Cancel pending order |
| PUT | `/Update-Status/{orderId}` | ✅ Admin/Seller | Update order status |

### 💳 SavedCard — `/api/SavedCard`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/` | ✅ Buyer | Save tokenized card |
| GET | `/` | ✅ Buyer | Get all saved cards |
| DELETE | `/{cardId}` | ✅ Buyer | Delete saved card |

### 🎟️ Coupon — `/api/Coupon`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Coupon` | ✅ Seller | Create discount coupon |
| PUT | `/Update-Coupon/{couponId}` | ✅ Seller | Update coupon |
| DELETE | `/Delete-Coupon/{couponId}` | ✅ Seller | Delete coupon |
| GET | `/Validate-Coupon/{couponCode}` | ✅ Any | Validate coupon code |

### 🏆 LoyaltyPoints — `/api/LoyaltyPoints`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/Balance` | ✅ Buyer | Get current points balance |
| GET | `/History` | ✅ Buyer | Get transaction history |
| POST | `/Redeem` | ✅ Buyer | Redeem points as discount |

### 🩺 Health — `/api/health`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | ❌ | API health status (Healthy/Degraded/Unhealthy) |

---

## 🧠 Key Business Features

### Advanced Authentication
- **Email Verification** — Mandatory before login
- **Secure Password Recovery** — Tokenized reset links with expiration
- **Refresh Token Rotation** — Long-lived sessions without exposing long-term credentials
- **Custom Identity Customization** — ASP.NET Identity linked to custom role entities

### Role-Based Profiles
- **Buyer Profile** — Address management, payment cards, order history, loyalty points
- **Seller Profile** — Product catalog, coupons, order fulfillment, analytics
- **Admin Profile** — Category management, system-wide order updates, user oversight

### Product Variant System
- Each product supports multiple SKU variants (size, color, price)
- Independent stock tracking per variant
- Live aggregate rating/review counts per variant
- Variant-level discount/coupon application

### Sophisticated Stock Management
- **Reserved Quantity** — Tracked separately from available stock
- **Auto-Restoration** — Stock restored on order cancellation or payment failure
- **Out-of-Stock Visibility** — Sellers see which variants need replenishment
- **Conflict Prevention** — Cart prevents overselling via real-time availability checks

### Review & Rating System
- **Purchase Validation** — Only buyers who purchased the variant can review
- **Aggregated Statistics** — AverageRating, TotalReviews updated automatically
- **Buyer Verification** — Reviews marked with verified-purchase badge
- **One Review Per Buyer** — Prevents review spam

### Wishlist Management
- **Smart Toggling** — Add/remove with single endpoint call
- **Multi-Wishlist Support** — Organize items across named lists
- **True Pagination** — Database-level pagination for large wishlists
- **Price Tracking** — Optional email notifications on price drops

### Loyalty Points System
- **Earn on Purchase** — Points calculated as percentage of order subtotal
- **Spend at Checkout** — Redeem points as discount (1 point = $0.01)
- **Auto-Refund on Cancellation** — Points restored if order cancelled
- **Lifetime History** — Complete transaction log accessible to buyer

### Payment Gateway Integration
- **Tokenized Cards** — Store cards securely for repeat purchases
- **Webhook Support** — Async payment verification from payment provider
- **Payment Failure Handling** — Auto-cancel order + restore stock + refund points
- **Transaction Audit Trail** — Complete payment history logged

### Hangfire Background Jobs
- **Auto-Cancel Unpaid Orders** — Delayed job cancels pending orders after timeout (default: 30 min)
- **Stock Restoration** — Restores reserved quantities when order expires
- **Async Email Dispatch** — Confirmation, password reset, verification emails sent asynchronously
- **Reliable Retry Logic** — Failed jobs retry with exponential backoff

---

## ⚡ Performance Optimization

### Database Query Optimization
- **AsNoTracking** — Applied globally to all read-only queries (browsing, filtering)
- **AsSplitQuery** — Multi-collection includes split into separate queries to prevent Cartesian explosions
- **Eager Loading** — Explicit `.Include()` eliminates N+1 query problems
- **True DB Pagination** — Skip/Take executed at SQL Server level, never in-memory

### Efficient Data Loading
- **Separated Concerns** — Large structural joins eliminated; resources retrieved as standalone entries
- **Lazy Loading Disabled** — Prevents accidental round-trips; only eager loading used
- **Index Strategy** — Database indexes defined via Fluent API on frequently queried fields

### Memory Management
- **Streamlined Repositories** — Removed redundant/duplicate methods in favor of shared generic operations
- **Lean DTOs** — Only include fields necessary for client consumption
- **No Change Tracking on Reads** — Massive memory relief on high-volume browsing

### API Response Efficiency
- **Consistent Response Wrapper** — Single `ApiResponseDto` shape reduces parsing logic
- **Calculated Fields in Mapping** — Offload computations to AutoMapper profiles, not controllers
- **Selective Field Inclusion** — Response DTOs include only relevant data per role

---

## 🧪 Testing & Quality Assurance

### NUnit + Moq Test Suite

Comprehensive unit testing of service layer logic with high coverage:

```bash
dotnet test
```

**Test Coverage:**

| Component | Coverage | Details |
|---|---|---|
| **AuthService** | ✅ High | Login, register, email verification, token refresh, password reset |
| **ProductService** | ✅ High | CRUD, variant management, stock validation |
| **OrderService** | ✅ High | Checkout, coupon validation, stock reservation, cancellation |
| **ReviewService** | ✅ High | Purchase validation, duplicate prevention, rating aggregation |
| **WishlistService** | ✅ High | CRUD, smart toggling, pagination |
| **LoyaltyPointsService** | ✅ High | Earning, spending, refund logic |
| **CartService** | ✅ High | Item management, quantity updates, cart clearing |

### Test Isolation
- Repositories fully mocked (Moq)
- Unit of Work mocked for transactional tests
- UserManager/RoleManager mocked for identity tests
- No database required; tests run deterministically in milliseconds

### Boundary & Edge Case Testing
- Invalid/expired tokens
- Out-of-stock products
- Duplicate/unauthorized reviews
- Expired coupons
- Concurrent order attempts
- Stock restoration on failed payments

### CI/CD Integration
- GitHub Actions workflow runs on every push/PR to `main`/`master`
- Build + test suite automated; merge blocked if tests fail
- Fast feedback loop (tests complete in < 30 seconds)

---

## ⚙️ Configuration & Setup

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ECommerceDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "AdminSecretKey": "your-super-secret-admin-key-change-in-production",
  "Jwt": {
    "Key": "your-jwt-secret-key-min-32-chars-long-change-in-production",
    "Issuer": "https://localhost:7132/",
    "Audience": "APISecureUser",
    "ExpirationInMinutes": 525600
  },
  "EmailSettings": {
    "EmailHost": "smtp.gmail.com",
    "EmailPort": 587,
    "SenderName": "E-Commerce API",
    "SenderEmail": "your-email@gmail.com",
    "Password": "your-app-specific-password"
  },
  "OrderSettings": {
    "UnpaidCancellationMinutes": 30
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Environment Variables (Production)

```bash
export ConnectionStrings__DefaultConnection="your-production-db-connection"
export Jwt__Key="your-production-jwt-key"
export EmailSettings__SenderEmail="noreply@yourdomain.com"
export EmailSettings__Password="your-smtp-password"
export AdminSecretKey="your-production-admin-secret"
```

---

## ️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or remote instance)
- Visual Studio 2022 or VS Code

### Installation

```bash
# Clone repository
git clone https://github.com/MostafaAhmed-100/E-Commerce-API-V2.git
cd E-Commerce-API-V2

# Restore dependencies
dotnet restore

# Apply migrations (creates database + schema)
dotnet ef database update

# Run application
dotnet run
```

### Verify Setup

```bash
# Swagger UI
https://localhost:7132/swagger

# Hangfire Dashboard
https://localhost:7132/hangfire

# Health Check
https://localhost:7132/api/health

# Health Check UI
https://localhost:7132/api/health/ui
```

---

## 📁 Response Format

### Success Response

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "errorCode": "",
  "message": "Product retrieved successfully.",
  "data": {
    "id": "product-id",
    "name": "Laptop",
    "description": "High-performance laptop",
    "variants": [
      {
        "id": "variant-id",
        "sku": "LAPTOP-001",
        "size": "15.6",
        "color": "Silver",
        "price": 1299.99,
        "stock": 50,
        "averageRating": 4.5,
        "totalReviews": 120
      }
    ]
  }
}
```

### Validation Error Response (Property-Based)

```json
{
  "isSuccess": false,
  "statusCode": 400,
  "errorCode": "ValidationError",
  "message": "One or more validation errors occurred.",
  "data": {
    "Email": [
      "'Email' is not a valid email address."
    ],
    "Password": [
      "Password must be at least 8 characters long.",
      "Password must contain at least one uppercase letter."
    ]
  }
}
```

### Not Found Error

```json
{
  "isSuccess": false,
  "statusCode": 404,
  "errorCode": "",
  "message": "Product not found.",
  "data": null
}
```

### Unauthorized Error

```json
{
  "isSuccess": false,
  "statusCode": 401,
  "errorCode": "",
  "message": "Unauthorized. Please log in.",
  "data": null
}
```

---

## 🌍 Bilingual Localization

The API supports bilingual responses (Arabic + English) for all validation errors and system messages.

### Trigger Localization

Pass the `Accept-Language` header in your HTTP requests:

```http
Accept-Language: ar    # Arabic
Accept-Language: en    # English (default)
```

### Example (Arabic Validation Errors)

```json
{
  "isSuccess": false,
  "statusCode": 400,
  "errorCode": "ValidationError",
  "message": "حدثت أخطاء تحقق واحدة أو أكثر.",
  "data": {
    "Email": [
      "البريد الإلكتروني ليس بصيغة صحيحة."
    ]
  }
}
```

---

## 🚦 Rate Limiting

Three rate limiting policies protect the API:

| Policy | Endpoints | Limit | Purpose |
|---|---|---|---|
| **AuthPolicy** | `/api/Auth/*` | 5 requests/min | Prevent brute-force attacks |
| **CheckoutPolicy** | `/api/Order/Create-Order` | 10 requests/min | Prevent order spam |
| **BrowsingPolicy** | GET `/api/Product/*`, `/api/Category/*` | 100 requests/min | Normal browsing |

### Rate Limit Headers

```http
RateLimit-Limit: 100
RateLimit-Remaining: 95
RateLimit-Reset: 1609459200
```

### 429 Response

```json
{
  "isSuccess": false,
  "statusCode": 429,
  "message": "Too many requests. Please try again later.",
  "data": null
}
```

---

## 🩺 Health Checks

The API exposes a dedicated health monitoring endpoint:

```http
GET /api/health
```

### Healthy Response

```json
{
  "status": "Healthy",
  "checks": {
    "sql_server": {
      "status": "Healthy",
      "description": "Database is accessible"
    }
  }
}
```

### Degraded Response

```json
{
  "status": "Degraded",
  "checks": {
    "sql_server": {
      "status": "Unhealthy",
      "description": "Cannot connect to database"
    }
  }
}
```

---

## 📊 Hangfire Background Jobs

Hangfire dashboard available at `https://localhost:7132/hangfire` (Admin-only in production):

### Auto-Cancel Unpaid Orders Job

- **Trigger:** When order is created in `Pending` status
- **Delay:** Configured in `OrderSettings.UnpaidCancellationMinutes` (default: 30 min)
- **Action:** Cancels order + restores reserved stock + refunds loyalty points
- **Retry Logic:** 3 automatic retries with exponential backoff

### Job Monitoring

```csharp
// Enqueue job
BackgroundJob.Schedule(
    () => _orderService.CancelUnpaidOrderAsync(orderId),
    TimeSpan.FromMinutes(30)
);
```

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

Located in `.github/workflows/dotnet-ci.yml`:

```yaml
name: .NET CI

on: [push, pull_request]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

### Workflow Behavior

- Runs on every push and pull request to `main`/`master`
- Installs .NET 8 SDK
- Restores NuGet packages
- Builds the solution
- Runs NUnit test suite
- Blocks merge if any step fails

---

## 👥 Contributors

| Name | Role | LinkedIn |
|---|---|---|
| **Mostafa Ahmed Soudi** | Backend Developer | [linkedin.com/in/mostafa-ahmed-745497326](https://www.linkedin.com/in/mostafa-ahmed-745497326/) |

*Computer Software Engineering, Egyptian Chinese University (ECU)*

---

## 🙏 Mentorship

Special thanks to the following mentors for their guidance throughout this project:

| Name | LinkedIn |
|---|---|
| **AbdALlatif Hossni** | [linkedin.com/in/abdallatif-hossni](https://www.linkedin.com/in/abdallatif-hossni/) |
| **Omar Ahmed** | [linkedin.com/in/omar-ahmed](https://www.linkedin.com/in/omar-ahmed-33a467298/) |

---

## 📜 License

This project is licensed under the MIT License.

---

**Developed with ❤️ by Mostafa Ahmed Soudi © 2026**