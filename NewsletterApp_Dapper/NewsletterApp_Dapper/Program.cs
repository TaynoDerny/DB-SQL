using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Dapper;

namespace NewsletterApp_Dapper
{
    // Модели данных (соответствуют таблицам)
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PromoItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    internal class Program
    {
        // Строка подключения 
        private static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=NewsletterDB;Trusted_Connection=True;";

        static void Main(string[] args)
        {
            Console.WriteLine("=== ЗАДАНИЕ 2: ПОДКЛЮЧЕНИЕ ===");
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("Успешное подключение к базе данных 'Список рассылки'!\n");

                // --- ЗАДАНИЕ 3 ---
                Console.WriteLine("=== ЗАДАНИЕ 3: ВЫВОД ВСЕХ ДАННЫХ ===");
                ShowAllCustomers(connection);
                ShowAllEmails(connection);
                ShowAllCategories(connection);
                ShowAllPromoItems(connection);
                ShowAllCities(connection);
                ShowAllCountries(connection);

                // --- ЗАДАНИЕ 4 ---
                Console.WriteLine("\n=== ЗАДАНИЕ 4: ФИЛЬТРАЦИЯ ===");
                ShowCustomersByCity(connection, "Москва");
                ShowCustomersByCountry(connection, "Россия");
                ShowPromoItemsByCountry(connection, "США");

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
            }
        }

        // ЗАДАНИЕ 3
        static void ShowAllCustomers(SqlConnection connection)
        {
            var customers = connection.Query<Customer>("SELECT * FROM Customers").ToList();
            Console.WriteLine("Покупатели:");
            foreach (var c in customers) Console.WriteLine($" - {c.FullName} ({c.Email})");
        }

        static void ShowAllEmails(SqlConnection connection)
        {
            // Выбираем только колонку Email
            var emails = connection.Query<string>("SELECT Email FROM Customers").ToList();
            Console.WriteLine("\nEmail всех покупателей:");
            foreach (var e in emails) Console.WriteLine($" - {e}");
        }

        static void ShowAllCategories(SqlConnection connection)
        {
            var categories = connection.Query<Category>("SELECT * FROM Categories").ToList();
            Console.WriteLine("\nРазделы:");
            foreach (var c in categories) Console.WriteLine($" - {c.Name}");
        }

        static void ShowAllPromoItems(SqlConnection connection)
        {
            var items = connection.Query<PromoItem>("SELECT * FROM PromoItems").ToList();
            Console.WriteLine("\nАкционные товары:");
            foreach (var i in items) Console.WriteLine($" - {i.Name} (Страна: {i.Country}, до {i.EndDate:dd.MM.yyyy})");
        }

        static void ShowAllCities(SqlConnection connection)
        {
            // DISTINCT убирает дубликаты городов
            var cities = connection.Query<string>("SELECT DISTINCT City FROM Customers").ToList();
            Console.WriteLine("\nГорода:");
            foreach (var c in cities) Console.WriteLine($" - {c}");
        }

        static void ShowAllCountries(SqlConnection connection)
        {
            var countries = connection.Query<string>("SELECT DISTINCT Country FROM Customers").ToList();
            Console.WriteLine("\nСтраны покупателей:");
            foreach (var c in countries) Console.WriteLine($" - {c}");
        }

        // ЗАДАНИЕ 4
        static void ShowCustomersByCity(SqlConnection connection, string city)
        {
            // Используем параметры чтобы избежать SQL-инъекций
            var customers = connection.Query<Customer>(
                "SELECT * FROM Customers WHERE City = @City",
                new { City = city }).ToList();

            Console.WriteLine($"\nПокупатели из города {city}:");
            foreach (var c in customers) Console.WriteLine($" - {c.FullName}");
        }

        static void ShowCustomersByCountry(SqlConnection connection, string country)
        {
            var customers = connection.Query<Customer>(
                "SELECT * FROM Customers WHERE Country = @Country",
                new { Country = country }).ToList();

            Console.WriteLine($"\nПокупатели из страны {country}:");
            foreach (var c in customers) Console.WriteLine($" - {c.FullName}");
        }

        static void ShowPromoItemsByCountry(SqlConnection connection, string country)
        {
            var items = connection.Query<PromoItem>(
                "SELECT * FROM PromoItems WHERE Country = @Country",
                new { Country = country }).ToList();

            Console.WriteLine($"\nАкции для страны {country}:");
            foreach (var i in items) Console.WriteLine($" - {i.Name}");
        }
    }
}