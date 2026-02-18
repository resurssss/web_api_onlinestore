using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        string connectionString = "Data Source=OnlineStore.Core/OnlineStore.db";
        
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        using var command = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='FileMetadata'", connection);
        var result = command.ExecuteScalar();
        
        if (result != null)
        {
            Console.WriteLine("Таблица FileMetadata существует в базе данных.");
            
            // Проверим структуру таблицы
            using var cmd = new SqliteCommand("PRAGMA table_info(FileMetadata)", connection);
            using var reader = cmd.ExecuteReader();
            
            Console.WriteLine("Структура таблицы FileMetadata:");
            while (reader.Read())
            {
                Console.WriteLine($"  {reader["name"]} ({reader["type"]}) {(Convert.ToBoolean(reader["notnull"]) ? "NOT NULL" : "")} {(Convert.ToBoolean(reader["pk"]) ? "PRIMARY KEY" : "")}");
            }
        }
        else
        {
            Console.WriteLine("Таблица FileMetadata НЕ существует в базе данных.");
        }
    }
}