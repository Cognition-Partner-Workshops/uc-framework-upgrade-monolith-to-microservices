# CQRS API - .NET Core Web API

A .NET 8 Web API implementing the **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR** and **Dapper** with SQLite.

## Tech Stack

- **.NET 8** - Web API framework
- **MediatR** - Mediator pattern implementation for CQRS
- **Dapper** - Lightweight ORM for data access
- **SQLite** - Embedded database (auto-created on startup)
- **Swagger/OpenAPI** - API documentation and testing UI

## Project Structure

```
CqrsApi/
├── Controllers/          # API Controllers (Products, Sales, Purchases, Users)
├── Data/
│   ├── DapperContext.cs          # Database connection factory
│   └── DatabaseInitializer.cs    # Auto-creates tables on startup
├── Features/
│   ├── Products/
│   │   ├── Commands/     # CreateProduct, UpdateProduct, DeleteProduct
│   │   └── Queries/      # GetAllProducts, GetProductById
│   ├── Sales/
│   │   ├── Commands/     # CreateSale, UpdateSale, DeleteSale
│   │   └── Queries/      # GetAllSales, GetSaleById
│   ├── Purchases/
│   │   ├── Commands/     # CreatePurchase, UpdatePurchase, DeletePurchase
│   │   └── Queries/      # GetAllPurchases, GetPurchaseById
│   └── Users/
│       ├── Commands/     # CreateUser, UpdateUser, DeleteUser
│       └── Queries/      # GetAllUsers, GetUserById
├── Models/               # Domain entities (Product, Sale, Purchase, User)
└── Program.cs            # Application entry point and DI configuration
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run the API

```bash
dotnet restore
dotnet run
```

The API will start and Swagger UI will be available at `http://localhost:5000` (root).

## API Endpoints

### Products
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Sales
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/sales` | Get all sales |
| GET | `/api/sales/{id}` | Get sale by ID |
| POST | `/api/sales` | Create a sale |
| PUT | `/api/sales/{id}` | Update a sale |
| DELETE | `/api/sales/{id}` | Delete a sale |

### Purchases
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/purchases` | Get all purchases |
| GET | `/api/purchases/{id}` | Get purchase by ID |
| POST | `/api/purchases` | Create a purchase |
| PUT | `/api/purchases/{id}` | Update a purchase |
| DELETE | `/api/purchases/{id}` | Delete a purchase |

### Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create a user |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Delete a user |

## CQRS Pattern

Each entity follows the CQRS pattern:
- **Commands** handle write operations (Create, Update, Delete)
- **Queries** handle read operations (GetAll, GetById)
- **MediatR** dispatches requests to the appropriate handler
- **Dapper** executes raw SQL queries for optimal performance
