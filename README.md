# 🛒 E-Commerce REST API V2

![.NET Core](https://img.shields.io/badge/.NET%208.0-Purple?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge)

A full-featured, highly optimized E-Commerce backend built with **ASP.NET Core 8 Web API**. The system supports three distinct roles — **Admin**, **Seller**, and **Buyer** — and covers everything from product and variant management to order processing, coupon discounts, rate limiting, background jobs, clean data validation, centralized exception handling, and full bilingual localization.

---

## 🚀 Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 8 Web API |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Auth** | ASP.NET Core Identity + JWT Bearer |
| **Architecture** | Repository Pattern + Service Layer |
| **Object Mapping** | AutoMapper |
| **Input Validation** | Fluent Validation |
| **Localization** | ASP.NET Core Request Localization (`.resx`) |
| **Rate Limiting** | ASP.NET Core Built-in Rate Limiting |
| **Background Jobs** | Hangfire + Hangfire.SqlServer |
| **Error Handling** | Custom Exception Middleware + Action Filters |

---

## 🏗️ Project Structure

```text
├── Controllers/               # API endpoints (thin layer, delegates to services)
├── Services/                  # Business logic layer (Clean & free of HTTP concerns)
├── Repository/
│   ├── GenericRepository/     # Base CRUD operations
│   └── SpecificRepository/    # Domain-specific queries
├── Entities/                  # EF Core models
├── DTOs/
│   ├── Request_DTOs/          # Input models (pure data carriers)
│   └── Response_DTOs/         # Output models (consistent wrapper)
├── Validators/                # Fluent Validation rules isolated from DTOs
├── Mappings/                  # AutoMapper Profiles (Clean DTO transformation)
├── Filters/                   # Action Filters (e.g., Validation interceptors)
├── Middlewares/               # Custom request pipeline (e.g., ExceptionMiddleware)
├── Exceptions/                # Domain-specific custom exceptions
├── Resources/                 # Localization files (English & Arabic)
├── Constants/                 # OrderStatus constants and other shared literals
├── BackgroundJobs/            # Hangfire job definitions
└── Data/                      # AppDbContext + migrations

```

---

## 👥 Roles & Permissions

| Role | Capabilities |
| --- | --- |
| `Admin` | Manages categories, updates any order status. |
| `Seller` | Creates/manages their own products, variants, and coupons. |
| `Buyer` | Browses products, manages cart, places and cancels orders. |

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
| POST | `/Login` | ❌ | Login and receive JWT token |
| POST | `/Register` | ❌ | Register as Buyer |
| POST | `/Register-Seller` | ❌ | Register as Seller |
| POST | `/Register-Admin` | ❌ | Register as Admin (requires secret key) |

### 👤 Account — `/api/Account`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| DELETE | `/Delete-Account` | ✅ Any | Soft-delete own account |

### 📍 Address — `/api/Address`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/Create-Address` | ✅ Any | Add a new address |
| PUT | `/Update-Address/{addressId}` | ✅ Any | Update an existing address |
| DELETE | `/Delete-Address/{addressId}` | ✅ Any | Delete an address |
| GET | `/Get-Address/{addressId}` | ❌ | Get address by ID |
| GET | `/My-Addresses` | ✅ Any | Get all addresses for the current user |

### 🛍️ Product — `/api/Product`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/Create-Product` | ✅ Seller | Create a product with one or more variants |
| PUT | `/Update-Product/{productId}` | ✅ Seller | Update product info and variants |
| DELETE | `/Delete-Product/{productId}` | ✅ Seller | Soft-delete a product |
| GET | `/Get-Product/{productId}` | ❌ | Get product details by ID |
| GET | `/Get-All` | ❌ | Get all products (paginated, filterable by category) |
| GET | `/Out-Of-Stock` | ✅ Seller | Get the seller's out-of-stock products |

### 🗂️ Category — `/api/Category`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/Create-Category` | ✅ Admin | Create a category (supports parent/subcategory) |
| PUT | `/Update-Category/{categoryId}` | ✅ Admin | Update a category |
| DELETE | `/Delete-Category/{categoryId}` | ✅ Admin | Delete a category |
| GET | `/Get-Category/{categoryId}` | ❌ | Get category with its subcategories |
| GET | `/Get-All` | ❌ | Get all categories (paginated) |

### 🛒 Cart — `/api/Cart`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| GET | `/My-Cart` | ✅ Buyer | View current cart |
| POST | `/Add-Item` | ✅ Buyer | Add a product variant to cart |
| PUT | `/Update-Quantity/{variantId}` | ✅ Buyer | Update quantity of a cart item |
| DELETE | `/Remove-Item/{variantId}` | ✅ Buyer | Remove an item from cart |
| DELETE | `/Clear-Cart` | ✅ Buyer | Clear entire cart |

### 📋 Order — `/api/Order`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/Create-Order` | ✅ Buyer | Place order from cart (optional coupon code) |
| GET | `/My-Orders` | ✅ Buyer | List all orders for the current buyer (paginated) |
| GET | `/Get-Order/{orderId}` | ✅ Buyer | Get full order details |
| DELETE | `/Cancel-Order/{orderId}` | ✅ Buyer | Cancel a pending order (restores stock) |
| PUT | `/Update-Status/{orderId}` | ✅ Admin/Seller | Move order through status pipeline |

### 🎟️ Coupon — `/api/Coupon`

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| POST | `/Create-Coupon` | ✅ Seller | Create a discount coupon |
| PUT | `/Update-Coupon/{couponId}` | ✅ Seller | Update coupon details |
| DELETE | `/Delete-Coupon/{couponId}` | ✅ Seller | Delete a coupon |
| GET | `/Validate-Coupon/{couponCode}` | ✅ Any | Check if a coupon is valid and get discount value |

---

## 🔑 Authentication

JWT Bearer tokens are used across all protected endpoints. After login or registration, pass the token in every request:

```text
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
| `AuthPolicy` | `/api/Auth/*` | Prevent brute-force on login/register |
| `CheckoutPolicy` | `/api/Order/Create-Order` | Limit order spam |
| `BrowsingPolicy` | Public GET endpoints | Relaxed limits for product/category browsing |

Clients that exceed limits receive `429 Too Many Requests`.

---

## ⚡ Performance Optimization & Pagination

The project architecture has undergone significant data-layer refactoring to secure high throughput and minimize memory footprints under heavy production loads:

* **True Database-Level Pagination:** Completely decoupled list endpoints from in-memory processing. All `GET` list requests stream `PageNumber` and `PageSize` parameters straight to SQL Server using optimized `.Skip()` and `.Take()` operations.
* **Separation of Concerns for Heavy Relationships:** To avoid massive memory allocations, large structural joins have been eliminated. Resources are retrieved efficiently as light, standalone entries.
* **Aggressive No-Tracking Strategy:** Applied `.AsNoTracking()` globally across all read-only repository queries, bypassing EF Core's change tracker and providing immediate CPU and memory relief.
* **Elimination of N+1 Query Problems:** Refactored relational retrieval loops to utilize explicit eager loading (`.Include()`), condensing nested calls down to a single, high-performance `JOIN`.

---

## 🛡️ Clean Validation & Global Error Handling

The project implements a robust, centralized error-handling and validation architecture to keep controllers and services completely clean of boilerplate code:

* **Fluent Validation & Action Filters:** DTOs are kept pure. Validation logic is isolated in dedicated `FluentValidation` classes. A global `ValidationFilter` intercepts incoming requests, validates the DTOs automatically, and throws domain exceptions if validation fails, never allowing bad data to reach the Controller.
* **Centralized Exception Middleware:** A custom `ExceptionMiddleware` sits at the top of the request pipeline, catching unhandled exceptions (including Validation errors), preventing app crashes, logging the errors, and mapping them to standardized HTTP responses.
* **Domain-Specific Exceptions:** Repetitive error returns in the Service Layer have been replaced with clean, domain-specific exceptions (`NotFoundException`, `BadRequestException`, `UnauthorizedException`, `ConflictException`, `ValidationException`).

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
                ↘ Cancelled (by Buyer or auto-cancelled by background job)

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
| **Clean Validation** | Fluent Validation rules separated from DTOs, enforced via a global Action Filter. |
| **Centralized Error Handling** | Custom middleware intercepts exceptions and standardizes API responses. |
| **Bilingual Support** | Full English and Arabic response localization based on the `Accept-Language` header. |
| **Soft Delete** | Users, products, and orders are never hard-deleted; global query filters hide them automatically. |
| **Product Variants** | Each product has multiple SKU variants (size, color, price, stock). |
| **Stock Management** | Reserved quantity tracking; stock is restored on cancellation. |
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

```

```