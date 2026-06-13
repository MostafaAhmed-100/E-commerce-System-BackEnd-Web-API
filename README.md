# 🛒 E-Commerce REST API V2

A full-featured E-Commerce backend built with **ASP.NET Core Web API**. The system supports three roles — **Admin**, **Seller**, and **Buyer** — and covers everything from product and variant management to order processing, coupon discounts, rate limiting, and automated background jobs.

> **For AI context:** This is an ASP.NET Core 8 Web API project using EF Core + SQL Server, JWT Bearer authentication, and a Repository + Service Layer architecture. It has no frontend — it's a pure REST API consumed via Swagger or any HTTP client. The latest additions include ASP.NET Core rate limiting middleware and Hangfire-based background job scheduling.

---

## 🚀 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Architecture | Repository Pattern + Service Layer |
| Rate Limiting | ASP.NET Core Built-in Rate Limiting |
| Background Jobs | Hangfire + Hangfire.SqlServer |

---

## 🏗️ Project Structure

```
├── Controllers/               # API endpoints (thin layer, delegates to services)
├── Services/                  # Business logic layer
├── Repository/
│   ├── GenericRepository/     # Base CRUD operations
│   └── SpecificRepository/    # Domain-specific queries
├── Entities/                  # EF Core models
├── DTOs/
│   ├── Request_DTOs/          # Input models (validated)
│   └── Response_DTOs/         # Output models (consistent wrapper)
├── Constants/                 # OrderStatus constants and other shared literals
├── BackgroundJobs/            # Hangfire job definitions
└── Data/                      # AppDbContext + migrations
```

---

## 👥 Roles & Permissions

| Role | Capabilities |
|---|---|
| `Admin` | Manages categories, updates any order status |
| `Seller` | Creates/manages their own products, variants, and coupons |
| `Buyer` | Browses products, manages cart, places and cancels orders |

---

## 📦 API Endpoints

### 🔐 Auth — `/api/Auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Login` | ❌ | Login and receive JWT token |
| POST | `/Register` | ❌ | Register as Buyer |
| POST | `/Register-Seller` | ❌ | Register as Seller |
| POST | `/Register-Admin` | ❌ | Register as Admin (requires secret key) |

---

### 👤 Account — `/api/Account`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| DELETE | `/Delete-Account` | ✅ Any | Soft-delete own account |

---

### 📍 Address — `/api/Address`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Address` | ✅ Any | Add a new address |
| PUT | `/Update-Address/{addressId}` | ✅ Any | Update an existing address |
| DELETE | `/Delete-Address/{addressId}` | ✅ Any | Delete an address |
| GET | `/Get-Address/{addressId}` | ❌ | Get address by ID |
| GET | `/My-Addresses` | ✅ Any | Get all addresses for the current user |

---

### 🛍️ Product — `/api/Product`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Product` | ✅ Seller | Create a product with one or more variants |
| PUT | `/Update-Product/{productId}` | ✅ Seller | Update product info and variants |
| DELETE | `/Delete-Product/{productId}` | ✅ Seller | Soft-delete a product |
| GET | `/Get-Product/{productId}` | ❌ | Get product details by ID |
| GET | `/Get-All` | ❌ | Get all products (paginated, filterable by category) |
| GET | `/Out-Of-Stock` | ✅ Seller | Get the seller's out-of-stock products |

---

### 🗂️ Category — `/api/Category`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Category` | ✅ Admin | Create a category (supports parent/subcategory) |
| PUT | `/Update-Category/{categoryId}` | ✅ Admin | Update a category |
| DELETE | `/Delete-Category/{categoryId}` | ✅ Admin | Delete a category |
| GET | `/Get-Category/{categoryId}` | ❌ | Get category with its subcategories |
| GET | `/Get-All` | ❌ | Get all categories (paginated) |

---

### 🛒 Cart — `/api/Cart`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/My-Cart` | ✅ Buyer | View current cart |
| POST | `/Add-Item` | ✅ Buyer | Add a product variant to cart |
| PUT | `/Update-Quantity/{variantId}` | ✅ Buyer | Update quantity of a cart item |
| DELETE | `/Remove-Item/{variantId}` | ✅ Buyer | Remove an item from cart |
| DELETE | `/Clear-Cart` | ✅ Buyer | Clear entire cart |

---

### 📋 Order — `/api/Order`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Order` | ✅ Buyer | Place order from cart (optional coupon code) |
| GET | `/My-Orders` | ✅ Buyer | List all orders for the current buyer |
| GET | `/Get-Order/{orderId}` | ✅ Buyer | Get full order details |
| DELETE | `/Cancel-Order/{orderId}` | ✅ Buyer | Cancel a pending order (restores stock) |
| PUT | `/Update-Status/{orderId}` | ✅ Admin/Seller | Move order through status pipeline |

---

### 🎟️ Coupon — `/api/Coupon`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Coupon` | ✅ Seller | Create a discount coupon |
| PUT | `/Update-Coupon/{couponId}` | ✅ Seller | Update coupon details |
| DELETE | `/Delete-Coupon/{couponId}` | ✅ Seller | Delete a coupon |
| GET | `/Validate-Coupon/{couponCode}` | ✅ Any | Check if a coupon is valid and get discount value |

---

## 🔑 Authentication

JWT Bearer tokens are used across all protected endpoints. After login or registration, pass the token in every request:

```
Authorization: Bearer <your_token>
```

Each JWT payload contains:

| Claim | Value |
|---|---|
| `NameIdentifier` | User ID |
| `Email` | User email |
| `Role` | Admin / Seller / Buyer |
| `ProfileId` | Role-specific profile ID (BuyerId or SellerId) |

---

## 🚦 Rate Limiting

Three policies are applied globally via ASP.NET Core's built-in rate limiting middleware:

| Policy | Applied To | Purpose |
|---|---|---|
| `AuthPolicy` | `/api/Auth/*` | Prevent brute-force on login/register |
| `CheckoutPolicy` | `/api/Order/Create-Order` | Limit order spam |
| `BrowsingPolicy` | Public GET endpoints | Relaxed limits for product/category browsing |

Clients that exceed limits receive `429 Too Many Requests`.

---

## ⏱️ Background Jobs (Hangfire)

Hangfire is integrated with SQL Server persistence and a dashboard for job monitoring.

**Automated Unpaid Order Cancellation:**  
When a Buyer places an order, a Hangfire **Delayed Job** is scheduled. If the order is still in `Pending` / unpaid status after the configured timeout, the job automatically cancels it and restores reserved stock — no manual intervention needed.

Hangfire Dashboard is available at: `/hangfire` (Admin only in production)

---

## 📋 Order Status Pipeline

`OrderStatus` values are defined as constants (not raw strings) to prevent typos and enable IDE support:

```
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

Hangfire creates its own schema tables automatically on first run.

---

## 🏃 Running the Project

```bash
git clone https://github.com/MostafaAhmed-100/E-Commerce-API-V2.git
cd E-Commerce-API-V2

dotnet restore
dotnet ef database update
dotnet run
```

- Swagger UI → `https://localhost:7132/swagger`  
- Hangfire Dashboard → `https://localhost:7132/hangfire`

---

## 🧠 Key Features

| Feature | Details |
|---|---|
| **Soft Delete** | Users, products, and orders are never hard-deleted; global query filters hide them automatically |
| **Product Variants** | Each product has multiple SKU variants (size, color, price, stock) |
| **Stock Management** | Reserved quantity tracking; stock is restored on cancellation |
| **Coupon System** | Percentage or fixed-amount discounts with usage limits and date ranges |
| **Pagination** | All list endpoints support `pageNumber` and `pageSize` |
| **Global Query Filters** | Deleted records excluded at the EF Core level — no manual `.Where()` needed |
| **Rate Limiting** | Three policies protecting auth, checkout, and browsing endpoints |
| **Auto-Cancel Jobs** | Hangfire delayed jobs cancel unpaid orders automatically after a configurable timeout |
| **Response Time Header** | Every response includes `X-Response-Time` for performance monitoring |

---

## 📁 Unified Response Format

Every endpoint returns the same wrapper shape:

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "errorCode": "",
  "message": "Operation successful.",
  "data": {}
}
```

---

## 🙏 Mentorship

Special thanks to the following mentors for their guidance throughout this project:

| Name | LinkedIn |
|---|---|
| AbdALlatif Hossni | [linkedin.com/in/abdallatif-hossni](https://www.linkedin.com/in/abdallatif-hossni-9217091b9/) |
| Omar Ahmed | [linkedin.com/in/omar-ahmed](https://www.linkedin.com/in/omar-ahmed-33a467298/) |

---

## 📜 License

This project is open-source and available under the [MIT License](LICENSE).