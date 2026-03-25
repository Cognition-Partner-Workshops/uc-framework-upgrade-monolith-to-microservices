using Dapper;

namespace CqrsApi.Data;

public class DatabaseInitializer
{
    private readonly DapperContext _context;

    public DatabaseInitializer(DapperContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL DEFAULT '',
                Price REAL NOT NULL DEFAULT 0,
                StockQuantity INTEGER NOT NULL DEFAULT 0,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            );
        ");

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL UNIQUE,
                FullName TEXT NOT NULL DEFAULT '',
                Role TEXT NOT NULL DEFAULT 'User',
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            );
        ");

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Sales (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                TotalAmount REAL NOT NULL,
                SaleDate TEXT NOT NULL DEFAULT (datetime('now')),
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (ProductId) REFERENCES Products(Id),
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );
        ");

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Purchases (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitCost REAL NOT NULL,
                TotalCost REAL NOT NULL,
                Supplier TEXT NOT NULL DEFAULT '',
                PurchaseDate TEXT NOT NULL DEFAULT (datetime('now')),
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (ProductId) REFERENCES Products(Id),
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );
        ");
    }
}
