using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient; // Для SQL Server

namespace DapperExample
{
    class Program
    {
        // Строка подключения. В реальном проекте храним ее в конфигурации (appsettings.json)
        static string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=TestDB;Integrated Security=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            // 1. ВСТАВКА (INSERT)
            var newUser = new User { Name = "Иван Петров", Email = "ivan@example.com", Age = 30 };
            string insertSql = "INSERT INTO Users (Name, Email, Age) VALUES (@Name, @Email, @Age)";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rowsAffected = db.Execute(insertSql, newUser);
                Console.WriteLine($"Вставлено строк: {rowsAffected}");
            }

            // 2. ЧТЕНИЕ (SELECT)
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Получаем всех пользователей старше 25 лет
                var users = db.Query<User>("SELECT * FROM Users WHERE Age > @Age", new { Age = 25 }).ToList();

                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Имя: {user.Name}, Email: {user.Email}, Возраст: {user.Age}");
                }
            }

            // 3. ОБНОВЛЕНИЕ (UPDATE)
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var updateSql = "UPDATE Users SET Age = @Age WHERE Name = @Name";
                db.Execute(updateSql, new { Age = 31, Name = "Иван Петров" });
            }

            // 4. УДАЛЕНИЕ (DELETE)
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var deleteSql = "DELETE FROM Users WHERE Name = @Name";
                db.Execute(deleteSql, new { Name = "Иван Петров" });
            }

            Console.ReadLine(); // Оставил, чтобы консоль не закрывалась сразу после выполнения
        }
    }
}