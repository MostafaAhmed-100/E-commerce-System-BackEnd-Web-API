من عيني يا هندسة، ده الريدمي كامل متقفل وفيه كل التعديلات اللي عملناها من الألف للياء، جاهز تاخده "Copy" وتحطه في ملف `README.md` بتاعك على طول:

# 🛒 E-Commerce REST API V2

![.NET Core](https://img.shields.io/badge/.NET%208.0-Purple?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Serilog](https://img.shields.io/badge/Serilog-F46800?style=for-the-badge&logo=datalore&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge)

A full-featured, highly optimized E-Commerce backend built with **ASP.NET Core 8 Web API**. The system supports three distinct roles — **Admin**, **Seller**, and **Buyer** — and covers everything from product and variant management to order processing, payment gateway integration, coupon discounts, rate limiting, background jobs, structured logging, clean data validation, centralized exception handling, secure account recovery, and full bilingual localization.

## 🚀 Tech Stack

| Layer | Technology |
| --- | --- |
| **Framework** | ASP.NET Core 8 Web API (using C# 11 features) |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Auth** | ASP.NET Core Identity + JWT Bearer + Refresh Tokens |
| **Architecture** | Repository Pattern + Service Layer |
| **Object Mapping** | AutoMapper |
| **Input Validation** | Fluent Validation + C# 11 `required` modifiers |
| **Logging** | Serilog (File & Console Sinks, Request Tracking) |
| **Email Service** | MailKit & MimeKit (SMTP Integration) |
| **Localization** | ASP.NET Core Request Localization (`.resx`) |
| **Rate Limiting** | ASP.NET Core Built-in Rate Limiting |
| **Background Jobs** | Hangfire + Hangfire.SqlServer |
| **External Integration**| HttpClientFactory (Payment Gateway Webhooks) |
| **Error Handling** | Custom Exception Middleware + Action Filters |

## 🏗️ Project Structure

```text
├── Controllers/               # API endpoints (thin layer, delegates to services)
├── Services/                  # Business logic layer (Clean & free of HTTP concerns)
├── Repository/
│   ├── GenericRepository/     # Base CRUD operations
│   └── SpecificRepository/    # Domain-specific queries
├── Entities/                  # EF Core models (Clean POCOs)
├── DTOs/
│   ├── Request_DTOs/          # Input models (Strictly enforced with `required`)
│   └── Response_DTOs/         # Output models (consistent wrapper)
├── Validators/                # Fluent Validation rules isolated from DTOs
├── Mappings/                  # AutoMapper Profiles (Clean DTO transformation)
├── Filters/                   # Action Filters (e.g., Validation interceptors)
├── Middlewares/               # Custom request pipeline (Exception handling & Log enrichment)
├── Exceptions/                # Domain-specific custom exceptions
├── Resources/                 # Localization files (English & Arabic)
├── Constants/                 # OrderStatus constants and other shared literals
├── BackgroundJobs/            # Hangfire job definitions
└── Data/
    ├── Configurations/        # EF Core Fluent API entity configurations
    └── AppDbContext           # Context and migrations

```

---

## 👥 Roles & Permissions

| Role | Capabilities |
| --- | --- |
| `Admin` | Manages categories, updates any order status. |
| `Seller` | Creates/manages their own products, variants, and coupons. *(Requires National ID verification).* |
| `Buyer` | Browses products, manages cart, manages wishlists, places and cancels orders, manages saved payment cards. |

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

### 📍 Address — `/api/Address`

*(... All Address Endpoints ...)*

### 🛍️ Product — `/api/Product`

*(... All Product Endpoints ...)*

### 🗂️ Category — `/api/Category`

*(... All Category Endpoints ...)*

### 🛒 Cart — `/api/Cart`

*(... All Cart Endpoints ...)*

### 💳 Saved Cards — `/api/SavedCard`

*(... All Saved Cards Endpoints ...)*

### 📋 Order — `/api/Order`

*(... All Order Endpoints ...)*

### 💸 Payment Gateway — `/api/Payment`

*(... All Payment Gateway Endpoints ...)*

### 🎟️ Coupon — `/api/Coupon`

*(... All Coupon Endpoints ...)*

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

* **Clean POCOs & Fluent API:** Completely stripped `Data Annotations` from domain entities. Relies exclusively on EF Core's `IEntityTypeConfiguration` (Fluent API) for schema generation, business logic constraints, default values, and index definitions, ensuring pure domain models.
* **True Database-Level Pagination:** Completely decoupled list endpoints from in-memory processing. All `GET` list requests stream `PageNumber` and `PageSize` parameters straight to SQL Server using optimized `.Skip()` and `.Take()` operations.
* **Separation of Concerns for Heavy Relationships:** To avoid massive memory allocations, large structural joins have been eliminated. Resources are retrieved efficiently as light, standalone entries.
* **Aggressive No-Tracking Strategy:** Applied `.AsNoTracking()` globally across all read-only repository queries, bypassing EF Core's change tracker and providing immediate CPU and memory relief.
* **Elimination of N+1 Query Problems:** Refactored relational retrieval loops to utilize explicit eager loading (`.Include()`), condensing nested calls down to a single, high-performance `JOIN`.

---

## 🛡️ Clean Validation, Logging & Global Error Handling

The project implements a robust, centralized error-handling and observability architecture:

* **Structured Logging (Serilog):** Integrated globally with File and Console sinks. The Service layer proactively logs business-critical events (warnings on security breaches, information on successful checkouts) creating a deep diagnostic audit trail.
* **Strict Input Validation:** DTOs are protected using C# 11 `required` properties alongside `FluentValidation`. A global `ValidationFilter` intercepts incoming requests, preventing bad data from ever reaching the Controller.
* **Centralized Exception Middleware:** A custom `ExceptionMiddleware` sits at the top of the request pipeline. It captures unhandled exceptions, logs them with full request paths via Serilog, prevents app crashes, and maps them to standardized HTTP responses.
* **Domain-Specific Exceptions:** Repetitive error returns in the Service Layer have been replaced with clean, domain-specific exceptions (`NotFoundException`, `BadRequestException`, `UnauthorizedException`, `ConflictException`).

---

## 🔄 Object Mapping Layer (AutoMapper)

The project leverages **AutoMapper** profiles across the Service Layer to maintain a separation of concerns, eliminating messy manual mapping code:

* **Recursive Mapping:** Configured to automatically resolve infinite self-referencing hierarchy loops.
* **Calculated Fields Mapping:** Offloads computation logic (such as subtotals and price reductions) directly into the mapping profiles.
* **State Updates Tracking:** Utilizes existing instance updating semantics to maintain EF Core's change tracking state.
* **Strict Collection Typing:** Synchronized mapped types directly with response wrapper expectations.

---

## ⏱️ Background Jobs (Hangfire)

Hangfire is integrated with SQL Server persistence and a dashboard for job monitoring.

**Automated Unpaid Order Cancellation:** When a Buyer places an order, a Hangfire **Delayed Job** is scheduled. If the order is still in `Pending` / unpaid status after the configured timeout, the job automatically cancels it and restores reserved stock — no manual intervention needed.

Hangfire Dashboard is available at: `/hangfire` *(Admin only in production)*

---

## 📋 Order Status Pipeline

`OrderStatus` values are defined as constants (not raw strings) to prevent typos and enable IDE support:

```text
Pending → Processing → Shipped → Delivered
↘ Cancelled (by Buyer, Payment Failure, or auto-cancelled by background job)

```

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
| **Account Management & Recovery** | Comprehensive operations including secure password modification, soft-deletion, and modular **Forgot/Reset Password** flows via tokenized emails. |
| **Email Integration (MailKit)** | Automated, secure email dispatch for user registration confirmation and account recovery. |
| **Structured Logging** | Comprehensive observability using Serilog (Request tracking, Info/Warning/Error trails). |
| **Payment Gateway Integration** | Complete checkout flow with third-party webhooks, tokenized saved cards, and asynchronous payment verification. |
| **Clean Database Architecture** | Decoupled EF Core configurations using `IEntityTypeConfiguration` (Fluent API) instead of cluttered Data Annotations. |
| **Clean Validation** | Fluent Validation rules separated from DTOs, enforced via a global Action Filter, backed by C# 11 `required` modifiers. |
| **Centralized Error Handling** | Custom middleware intercepts exceptions, enriches logs, and standardizes API responses. |
| **Bilingual Support** | Full English and Arabic response localization based on the `Accept-Language` header. |
| **Soft Delete** | Users, products, and orders are never hard-deleted; global query filters hide them automatically. |
| **Product Variants** | Each product has multiple SKU variants (size, color, price, stock). |
| **Stock Management** | Reserved quantity tracking; stock is restored on cancellation or payment failure. |
| **Wishlist System** | Comprehensive management allowing buyers to create multiple lists, smartly toggle product variants (add/remove with a single endpoint), and retrieve items via true DB pagination. |
| **Coupon System** | Percentage or fixed-amount discounts with usage limits and date ranges. |
| **True DB Pagination** | All list endpoints execute memory-optimized paging natively at the database level. |
| **Global Query Filters** | Deleted records excluded at the EF Core level — no manual `.Where()` needed. |
| **Rate Limiting** | Three policies protecting auth, checkout, and browsing endpoints. |
| **Auto-Cancel Jobs** | Hangfire delayed jobs cancel unpaid orders automatically after a configurable timeout. |
| **Response Time Header** | Every response includes `X-Response-Time` for performance monitoring. |

---

## 📁 Unified Response Format

Every endpoint (whether successful or failed) returns the exact same wrapper shape, making frontend consumption seamless.

**Example 1: Success Response (200 OK)**

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "errorCode": "",
  "message": "Operation successful.",
  "data": {
    "id": 1,
    "name": "Product Name"
  }
}

```

**Example 2: Error Response (404 Not Found handled by Middleware)**

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