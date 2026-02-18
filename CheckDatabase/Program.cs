using Microsoft.Data.Sqlite;
using System;

class Program
{
    static void Main()
    {
        var connectionString = "Data Source=../OnlineStore.API/OnlineStore.db";
        
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // Добавим тестовую категорию
        Console.WriteLine("Добавляем тестовую категорию...");
        using var insertCategoryCommand = new SqliteCommand(
            "INSERT OR IGNORE INTO Categories (Id, Name, Description, CreatedAt, UpdatedAt) VALUES (1, 'Электроника', 'Электронные устройства', datetime('now'), datetime('now'))", 
            connection);
        insertCategoryCommand.ExecuteNonQuery();
        
        // Добавим тестовый продукт
        Console.WriteLine("Добавляем тестовый продукт...");
        using var insertProductCommand = new SqliteCommand(
            "INSERT OR IGNORE INTO Products (Name, Description, Price, Stock, CategoryId, CreatedAt, UpdatedAt) VALUES ('Компьютер', 'Мощный компьютер', 50000.0, 10, 1, datetime('now'), datetime('now'))", 
            connection);
        insertProductCommand.ExecuteNonQuery();
        
        // Проверим содержимое таблицы Categories
        Console.WriteLine("Категории:");
        using var categoriesCommand = new SqliteCommand("SELECT Id, Name, Description FROM Categories", connection);
        using var categoriesReader = categoriesCommand.ExecuteReader();
        
        while (categoriesReader.Read())
        {
            Console.WriteLine($"ID: {categoriesReader["Id"]}, Name: {categoriesReader["Name"]}, Description: {categoriesReader["Description"]}");
        }
        
        categoriesReader.Close();
        
        // Проверим содержимое таблицы Products
        Console.WriteLine("\nПродукты:");
        using var productsCommand = new SqliteCommand("SELECT Id, Name, CategoryId FROM Products", connection);
        using var productsReader = productsCommand.ExecuteReader();
        
        while (productsReader.Read())
        {
            Console.WriteLine($"ID: {productsReader["Id"]}, Name: {productsReader["Name"]}, CategoryId: {productsReader["CategoryId"]}");
        }
    }
}
