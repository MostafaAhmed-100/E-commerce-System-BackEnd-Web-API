🛒 E-Commerce REST API V2

A full-featured, highly optimized E-Commerce backend built with **ASP.NET Core 8 Web API**. The system supports three distinct roles — **Admin**, **Seller**, and **Buyer** — and covers everything from product and variant management to order processing, payment gateway integration, coupon discounts, product reviews & ratings, rate limiting, background jobs, structured logging, clean data validation, centralized exception handling, secure account recovery, and full bilingual localization.

## 🚀 Tech Stack

| Layer | Technology |
| --- | --- |
| **Framework** | ASP.NET Core 8 Web API (using C# 11 features) |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Auth** | ASP.NET Core Identity + JWT Bearer + Refresh Tokens |
| **Architecture** | Clean Architecture principles, Repository Pattern + Unit of Work & Transactions + Service Layer |
| **Object Mapping** | AutoMapper |
| **Input Validation** | Fluent Validation + C# 11 `required` modifiers |
| **Logging** | Serilog (File & Console Sinks, Request Tracking) |
| **Email Service** | MailKit & MimeKit (SMTP Integration) |
| **Localization** | ASP.NET Core Request Localization (`.resx`) |
| **Rate Limiting** | ASP.NET Core Built-in Rate Limiting |
| **Background Jobs** | Hangfire + Hangfire.SqlServer |
| **External Integration** | HttpClientFactory (Payment Gateway Webhooks) |
| **Testing** | NUnit & Moq (Comprehensive Unit Testing for Business Logic) |

## 🏗️ Project Structure

```text
├── Controllers/                 # API endpoints (thin layer, delegates to services)
├── Services/                    # Business logic layer (Clean & free of HTTP concerns)
├── Repository/
│   ├── GenericRepository/       # Base CRUD operations
│   ├── SpecificRepository/      # Domain-specific queries
│   └── UnitOfWork/              # Unit of Work & Transaction management (used across all services)
├── Entities/                    # EF Core models (Clean POCOs)
├── DTOs/
│   ├── Request_DTOs/            # Input models (Strictly enforced with `required`)
│   └── Response_DTOs/           # Output models (consistent wrapper)
├── Validators/                  # Fluent Validation rules isolated from DTOs
├── Mappings/                    # AutoMapper Profiles (Clean DTO transformation)
├── Filters/                     # Action Filters (e.g., Validation interceptors)
├── Middlewares/                 # Custom request pipeline (Exception handling & Log enrichment)
├── Exceptions/                  # Domain-specific custom exceptions
├── Resources/                   # Localization files (English & Arabic)
├── Constants/                   # OrderStatus constants and other shared literals
├── BackgroundJobs/              # Hangfire job definitions
├── Tests/                       # Unit Tests using NUnit and Moq
└── Data/
    ├── Configurations/          # EF Core Fluent API entity configurations
    └── AppDbContext             # Context and migrations

```

---

## 👥 Roles & Permissions

| Role | Capabilities |
| --- | --- |
| `Admin` | Manages categories, updates any order status. |
| `Seller` | Creates/manages their own products, variants, and coupons. *(Requires National ID verification).* |
| `Buyer` | Browses products, manages cart, manages wishlists, places and cancels orders, manages saved payment cards, earns/redeems loyalty points, and leaves verified reviews & ratings on purchased products. |

---

## 🌍 Localization (Multi-Language Support)

The API fully supports bilingual responses (English & Arabic) for all validation errors and system messages.
To change the response language, simply pass the `Accept-Language` header in your HTTP requests:

* **English (Default):** `Accept-Language: en`
* **Arabic:** `Accept-Language: ar`

*(This feature is seamlessly integrated into the Swagger UI via a custom Operation Filter).*

---

## 📦 API Endpoints

### 🔐 Auth — `/api/Auth`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/Login` | ❌ | Login and receive JWT & Refresh Token (Requires Confirmed Email) |
| POST | `/Register` | ❌ | Register as Buyer and dispatch confirmation email |
| POST | `/Register-Seller` | ❌ | Register as Seller and dispatch confirmation email |
| POST | `/Register-Admin` | ❌ | Register as Admin (requires secret key) |
| GET | `/Confirm-Email` | ❌ | Verify email address via secure token |
| POST | `/Forgot-Password` | ❌ | Send password reset link to user's email |
| POST | `/Reset-Password` | ❌ | Reset password using the emailed token |
| POST | `/Refresh-Token` | ❌ | Generate new Access & Refresh tokens using a valid Refresh Token |

### 👤 Account — `/api/Account`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| GET | `/Profile` | ✅ Any | Retrieve tailored role-specific profile data (Buyer/Seller DTOs) |
| PUT | `/Profile` | ✅ Any | Update role-specific profile data with strict FluentValidation |
| POST | `/Change-Password` | ✅ Any | Securely change the current user's password |
| DELETE | `/Delete-Account` | ✅ Any | Soft-delete own account |

### ❤️ Wishlist — `/api/Wishlist`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/` | ✅ Buyer | Create a new customized wishlist |
| GET | `/` | ✅ Buyer | Retrieve all wishlists for the authenticated buyer |
| DELETE | `/{wishlistId}` | ✅ Buyer | Delete a specific wishlist and its items |
| POST | `/{wishlistId}/toggle-item` | ✅ Buyer | Smartly toggle (add/remove) a product variant in the wishlist |
| GET | `/{wishlistId}/items` | ✅ Buyer | Get paginated items (variants) inside a specific wishlist |

### ⭐ Review — `/api/Review`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/` | ✅ Buyer | Submit a review & rating for a product variant (validated against actual purchase history) |
| PUT | `/{reviewId}` | ✅ Buyer | Update the authenticated buyer's own review |
| DELETE | `/{reviewId}` | ✅ Buyer | Delete the authenticated buyer's own review |
| GET | `/variant/{variantId}` | ❌ | Get paginated reviews for a specific product variant |
| GET | `/variant/{variantId}/summary` | ❌ | Get aggregated rating summary (`AverageRating`, `TotalReviews`) for a variant |

*(Note: Other modules including Address, Product, Category, Cart, SavedCard, Order, Payment, and Coupon follow a similarly structured RESTful design).*

---

## 🔑 Authentication & Security

JWT Bearer tokens are used across all protected endpoints. After a successful login (which requires a **confirmed email address**), pass the token in every request:

```http
Authorization: Bearer <your_token>

```

Each JWT payload contains:

| Claim | Value |
| --- | --- |
| `NameIdentifier` | User ID |
| `Email` | User email |
| `Role` | Admin / Seller / Buyer |
| `ProfileId` | Role-specific profile ID (BuyerId or SellerId) |

---

## 🚦 Rate Limiting

Three policies are applied globally via ASP.NET Core's built-in rate limiting middleware:

| Policy | Applied To | Purpose |
| --- | --- | --- |
| `AuthPolicy` | `/api/Auth/*` | Prevent brute-force on login/register & password resets |
| `CheckoutPolicy` | `/api/Order/Create-Order` | Limit order spam |
| `BrowsingPolicy` | Public GET endpoints | Relaxed limits for product/category browsing |

Clients that exceed limits receive `429 Too Many Requests`.

---

## ⚡ Performance Optimization & Database Architecture

The project architecture has undergone significant data-layer refactoring to secure high throughput and minimize memory footprints under heavy production loads:

* **Unit of Work Everywhere:** Every service now operates exclusively through `IUnitOfWork`, giving atomic commits/rollbacks and consistent transactional integrity across the entire data-modifying surface (not just Order/Payment flows).
* **Clean POCOs & Fluent API:** Completely stripped `Data Annotations` from domain entities. Relies exclusively on EF Core's `IEntityTypeConfiguration` (Fluent API) for schema generation, business logic constraints, default values, and index definitions, ensuring pure domain models.
* **True Database-Level Pagination:** Completely decoupled list endpoints from in-memory processing. All `GET` list requests stream `PageNumber` and `PageSize` parameters straight to SQL Server using optimized `.Skip()` and `.Take()` operations.
* **Separation of Concerns for Heavy Relationships:** To avoid massive memory allocations, large structural joins have been eliminated. Resources are retrieved efficiently as light, standalone entries.
* **Aggressive No-Tracking Strategy:** Applied `.AsNoTracking()` globally across all read-only repository queries, bypassing EF Core's change tracker and providing immediate CPU and memory relief.
* **Split Queries for Collections:** Adopted `.AsSplitQuery()` on multi-collection includes (e.g., product variants with reviews) to avoid cartesian-explosion row duplication and reduce data transferred over the wire.
* **Elimination of N+1 Query Problems:** Refactored relational retrieval loops to utilize explicit eager loading (`.Include()`), condensing nested calls down to a single, high-performance `JOIN`.
* **Streamlined Repository Interfaces:** Removed redundant/duplicate repository methods in favor of shared generic operations, shrinking the surface area and easing maintenance.

---

## 🧪 Unit Testing & Quality Assurance

To ensure the highest level of reliability and prevent regressions, the core business logic has been rigorously tested:

* **Frameworks Used:** `NUnit` as the testing framework and `Moq` for isolating dependencies.
* **Test Coverage:** Comprehensive testing of the Service Layer (e.g., `OrderService`, `AuthService`, `ProductService`, `CartService`, `WishlistService`, `CategoryService`, `CouponService`, `AddressService`, `SavedCardService`, `ReviewService`).
* **Boundary & Exception Testing:** Every critical path is tested, including complex business logic (e.g., Stock reservation, Loyalty Points calculations, Invalid/Expired Coupons, duplicate/unauthorized reviews, and Unauthorized modifications).
* **Mocked Integrations:** External services, Repositories, Unit of Work, and Identity Managers (`UserManager`/`RoleManager`) are fully mocked to ensure tests run fast and deterministically without needing a live database or SMTP server.

---

## 🛡️ Clean Validation, Logging & Global Error Handling

The project implements a robust, centralized error-handling and observability architecture:

* **Structured Logging (Serilog):** Integrated globally with File and Console sinks. The Service layer proactively logs business-critical events (warnings on security breaches, information on successful checkouts) creating a deep diagnostic audit trail.
* **Strict Input Validation:** DTOs are protected using C# 11 `required` properties alongside `FluentValidation`. A global `ValidationFilter` intercepts incoming requests, preventing bad data from ever reaching the Controller.
* **Centralized Exception Middleware:** A custom `ExceptionMiddleware` sits at the top of the request pipeline. It captures unhandled exceptions, logs them with full request paths via Serilog, prevents app crashes, and maps them to standardized HTTP responses.
* **Domain-Specific Exceptions:** Repetitive error returns in the Service Layer have been replaced with clean, domain-specific exceptions (`NotFoundException`, `BadRequestException`, `UnauthorizedException`, `ConflictException`).
* **Transactional Integrity:** With every service now routed through `IUnitOfWork`, failures mid-operation trigger a full rollback, with the originating error logged alongside the affected request path.

---

## 🔄 Object Mapping Layer (AutoMapper)

The project leverages **AutoMapper** profiles across the Service Layer to maintain a separation of concerns, eliminating messy manual mapping code:

* **Recursive Mapping:** Configured to automatically resolve infinite self-referencing hierarchy loops.
* **Calculated Fields Mapping:** Offloads computation logic (such as subtotals, price reductions, and variant `AverageRating`) directly into the mapping profiles.
* **State Updates Tracking:** Utilizes existing instance updating semantics to maintain EF Core's change tracking state.
* **Strict Collection Typing:** Synchronized mapped types directly with response wrapper expectations.

---

## ⏱️ Background Jobs (Hangfire)

Hangfire is integrated with SQL Server persistence and a dashboard for job monitoring.

**Automated Unpaid Order Cancellation:** When a Buyer places an order, a Hangfire **Delayed Job** is scheduled. If the order is still in `Pending` / unpaid status after the configured timeout, the job automatically cancels it and restores reserved stock — no manual intervention needed.

Hangfire Dashboard is available at: `/hangfire` *(Admin only in production)*

---

## ⚙️ Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_sql_server_connection_string"
  },
  "AdminSecretKey": "your_admin_secret",
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "https://localhost:7132/",
    "Audience": "APISecureUser"
  },
  "EmailSettings": {
    "EmailHost": "smtp.gmail.com",
    "EmailPort": 587,
    "SenderName": "E-Commerce API",
    "SenderEmail": "your_email@gmail.com",
    "Password": "your_app_password"
  },
  "OrderSettings": {
    "UnpaidCancellationMinutes": 30
  }
}

```

---

## 🗄️ Database Setup

```bash
dotnet ef database update

```

*Hangfire creates its own schema tables automatically on the first run.*

---

## 🏃 Running the Project

```bash
git clone [https://github.com/MostafaAhmed-100/E-Commerce-API-V2.git](https://github.com/MostafaAhmed-100/E-Commerce-API-V2.git)
cd E-Commerce-API-V2
dotnet restore
dotnet ef database update
dotnet run

```

* **Swagger UI** → `https://localhost:7132/swagger`
* **Hangfire Dashboard** → `https://localhost:7132/hangfire`

---

## 🧠 Key Features

| Feature | Details |
| --- | --- |
| **Advanced Authentication** | Secure JWT issuance with long-lived Refresh Tokens, coupled with **strict email verification** required prior to login access. |
| **Role-Based Profiles** | Dynamic profile retrieval and updating tailored to user roles (Buyer/Seller) utilizing AutoMapper, strict DTO isolation, and KYC/National ID enforcement for sellers. |
| **Account Management** | Comprehensive operations including secure password modification, soft-deletion, and modular **Forgot/Reset Password** flows via tokenized emails. |
| **Unit Testing** | High test coverage using **NUnit & Moq** to validate business rules, transactions, and edge cases safely. |
| **Email Integration** | Automated, secure email dispatch for user registration confirmation and account recovery. |
| **Structured Logging** | Comprehensive observability using Serilog (Request tracking, Info/Warning/Error trails). |
| **Payment Gateway** | Complete checkout flow with third-party webhooks, tokenized saved cards, and asynchronous payment verification. |
| **Clean DB Architecture** | Unit of Work pattern applied across every service for atomic, transactional data operations, on top of decoupled EF Core configurations (`IEntityTypeConfiguration`). |
| **Clean Validation** | Fluent Validation rules separated from DTOs, enforced via a global Action Filter. |
| **Centralized Error Handling** | Custom middleware intercepts exceptions, enriches logs, and standardizes API responses. |
| **Bilingual Support** | Full English and Arabic response localization based on the `Accept-Language` header. |
| **Soft Delete** | Users, products, and orders are never hard-deleted; global query filters hide them automatically. |
| **Product Variants** | Each product has multiple SKU variants (size, color, price, stock), each carrying live `AverageRating` and `TotalReviews` aggregates. |
| **Reviews & Ratings** | Buyers submit ratings/comments on purchased variants; purchase is validated before submission, and aggregate rating stats update automatically. |
| **Stock Management** | Reserved quantity tracking; stock is restored on cancellation or payment failure. |
| **Wishlist System** | Management allowing buyers to smartly toggle product variants and retrieve items via true DB pagination. |
| **Coupon System** | Percentage or fixed-amount discounts with usage limits and date ranges. |
| **True DB Pagination** | All list endpoints execute memory-optimized paging natively at the database level. |
| **Global Query Filters** | Deleted records excluded at the EF Core level — no manual `.Where()` needed. |
| **Rate Limiting** | Three policies protecting auth, checkout, and browsing endpoints. |
| **Auto-Cancel Jobs** | Hangfire delayed jobs cancel unpaid orders automatically after a configurable timeout. |
| **Loyalty Points System** | Buyers earn points upon successful payment completion, and spend points as a discount at checkout (with auto-refunds on cancellation). |

---

## 📁 Unified Response Format

Every endpoint (whether successful or failed) returns the exact same wrapper shape, making frontend consumption seamless.

**Example: Error Response (404 Not Found handled by Middleware)**

```json
{
  "isSuccess": false,
  "statusCode": 404,
  "errorCode": "",
  "message": "The product does not exist.",
  "data": null
}

```

---

## 🙏 Mentorship

Special thanks to the following mentors for their guidance throughout this project:

| Name | LinkedIn |
| --- | --- |
| AbdALlatif Hossni | [linkedin.com/in/abdallatif-hossni](https://www.google.com/search?q=https://www.linkedin.com/in/abdallatif-hossni-9217091b9/) |
| Omar Ahmed | [linkedin.com/in/omar-ahmed](https://www.google.com/search?q=https://www.linkedin.com/in/omar-ahmed-33a467298/) |

---

## 📜 License

This project is open-source and available under the MIT License.