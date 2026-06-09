# 🛒 E-Commerce REST API V2

A full-featured E-Commerce backend built with **ASP.NET Core**, **Entity Framework Core**, and **JWT Authentication**. The API supports three roles — Admin, Seller, and Buyer — and covers everything from product management to order processing with coupon support.

---

## 🚀 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Architecture | Repository Pattern + Service Layer |

---

## 🏗️ Architecture

```
├── Controllers/          # API endpoints
├── Services/             # Business logic layer
├── Repository/
│   ├── GenericRepository/    # Base CRUD
│   └── SpecificRepository/   # Domain-specific queries
├── Entities/             # EF Core models
├── DTOs/
│   ├── Request_DTOs/     # Input models
│   └── Response_DTOs/    # Output models
└── Data/                 # AppDbContext
```

---

## 👥 Roles

| Role | Description |
|---|---|
| `Admin` | Manages categories, updates order statuses |
| `Seller` | Creates/manages products and coupons |
| `Buyer` | Browses products, manages cart and orders |

---

## 📦 API Endpoints

### 🔐 Auth — `/api/Auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Login` | ❌ | Login and get JWT token |
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
| PUT | `/Update-Address/{addressId}` | ✅ Any | Update an address |
| DELETE | `/Delete-Address/{addressId}` | ✅ Any | Delete an address |
| GET | `/Get-Address/{addressId}` | ✅ Any | Get address by ID |
| GET | `/My-Addresses` | ✅ Any | Get all user addresses |

---

### 🛍️ Product — `/api/Product`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Product` | ✅ Seller | Create a product with variants |
| PUT | `/Update-Product/{productId}` | ✅ Seller | Update product and variants |
| DELETE | `/Delete-Product/{productId}` | ✅ Seller | Soft-delete a product |
| GET | `/Get-Product/{productId}` | ❌ | Get product by ID |
| GET | `/Get-All` | ❌ | Get all products (paginated, filterable by category) |
| GET | `/Out-Of-Stock` | ✅ Seller | Get seller's out-of-stock products |

---

### 🗂️ Category — `/api/Category`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Category` | ✅ Admin | Create a category (supports parent/sub) |
| PUT | `/Update-Category/{categoryId}` | ✅ Admin | Update a category |
| DELETE | `/Delete-Category/{categoryId}` | ✅ Admin | Delete a category |
| GET | `/Get-Category/{categoryId}` | ❌ | Get category with subcategories |
| GET | `/Get-All` | ❌ | Get all categories (paginated) |

---

### 🛒 Cart — `/api/Cart`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/My-Cart` | ✅ Buyer | View current cart |
| POST | `/Add-Item` | ✅ Buyer | Add item to cart |
| PUT | `/Update-Quantity/{variantId}` | ✅ Buyer | Update item quantity |
| DELETE | `/Remove-Item/{variantId}` | ✅ Buyer | Remove item from cart |
| DELETE | `/Clear-Cart` | ✅ Buyer | Clear entire cart |

---

### 📋 Order — `/api/Order`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Order` | ✅ Buyer | Create order from cart (supports coupon) |
| GET | `/My-Orders` | ✅ Buyer | Get all buyer orders |
| GET | `/Get-Order/{orderId}` | ✅ Buyer | Get order details |
| DELETE | `/Cancel-Order/{orderId}` | ✅ Buyer | Cancel a pending order |
| PUT | `/Update-Status/{orderId}` | ✅ Admin/Seller | Update order status |

---

### 🎟️ Coupon — `/api/Coupon`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/Create-Coupon` | ✅ Seller | Create a discount coupon |
| PUT | `/Update-Coupon/{couponId}` | ✅ Seller | Update coupon details |
| DELETE | `/Delete-Coupon/{couponId}` | ✅ Seller | Delete a coupon |
| GET | `/Validate-Coupon/{couponCode}` | ✅ Any | Validate a coupon code |

---

## 🔑 Authentication

The API uses **JWT Bearer tokens**. After login/register, include the token in every protected request:

```
Authorization: Bearer <your_token>
```

Each token contains:
- `NameIdentifier` — User ID
- `Email` — User email
- `Role` — Admin / Seller / Buyer
- `ProfileId` — Role-specific profile ID (BuyerId or SellerId)

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
  }
}
```

---

## 🗄️ Database Setup

```bash
# Apply migrations
dotnet ef database update
```

Make sure your connection string in `appsettings.json` points to a running SQL Server instance.

---

## 🏃 Running the Project

```bash
git clone https://github.com/your-username/your-repo.git
cd your-repo

# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run
dotnet run
```

Swagger UI will be available at: `https://localhost:7132/swagger`

---

## 🧠 Key Features

- **Soft Delete** — Users, Products, Orders are never hard-deleted
- **Product Variants** — Each product supports multiple SKU variants (size, color, price)
- **Stock Management** — Reserved quantity tracking; stock restored on order cancellation
- **Coupon System** — Percentage or fixed-amount discounts with usage limits and date ranges
- **Pagination** — All list endpoints support page number and page size
- **Global Query Filters** — Deleted records are automatically excluded from queries
- **Response Time Header** — Every response includes `X-Response-Time` header

---

## 📁 Response Format

All endpoints return a consistent wrapper:

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "errorCode": "",
  "message": "Operation successful.",
  "data": { }
}
```
Mentorship:
I would like to express my sincere gratitude to
| Name | linkidin |
|---|---|
| AbdALlatif Hossni | https://www.linkedin.com/in/abdallatif-hossni-9217091b9/|
| Omar Ahmed | https://www.linkedin.com/in/omar-ahmed-33a467298/|
 for their valuable guidance and mentorship throughout the development of this project. Their insights and technical advice were instrumental in helping me overcome challenges and refine the project architecture.
---

## 📜 License

This project is open-source and available under the [MIT License](LICENSE).
