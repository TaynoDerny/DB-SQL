using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace NewsletterApp_Dapper_Part2
{
    public class CityCount
    {
        public string City { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
    }

    public class CountryCount
    {
        public string Country { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public int CityCount { get; set; }
    }

    public class PromoItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    internal class Program
    {
        private static string connectionString = @"Server=(localdb)\mssqllocaldb;Database=NewsletterDB;Trusted_Connection=True;";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== ЗАДАНИЕ 1: СТАТИСТИКА ГОРОДОВ И СТРАН ===");
            await ShowCustomerCountByCity();
            await ShowCustomerCountByCountry();
            await ShowCityCountByCountry();
            await ShowAverageCitiesPerCountry();

            Console.WriteLine("\n=== ЗАДАНИЕ 2: СВЯЗИ И АКЦИИ ===");
            await ShowCategoriesForCountry("Россия");
            await ShowPromoItemsByCategoryAndDate(1, new DateTime(2026, 10, 1), new DateTime(2026, 12, 31));
            await ShowPromoItemsForCustomer("ivan@mail.ru");

            Console.WriteLine("\n=== ЗАДАНИЕ 3: ТОП-3 И ЛИДЕРЫ ===");
            await ShowTop3Countries();
            await ShowBestCountry();
            await ShowTop3Cities();
            await ShowBestCity();
        }

        static async Task ShowCustomerCountByCity()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT City, COUNT(*) as CustomerCount FROM Customers GROUP BY City";
            var result = await db.QueryAsync<CityCount>(sql);

            Console.WriteLine("Количество покупателей в каждом городе:");
            foreach (var item in result) Console.WriteLine($" - {item.City}: {item.CustomerCount}");
        }

        static async Task ShowCustomerCountByCountry()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT Country, COUNT(*) as CustomerCount FROM Customers GROUP BY Country";
            var result = await db.QueryAsync<CountryCount>(sql);

            Console.WriteLine("\nКоличество покупателей в каждой стране:");
            foreach (var item in result) Console.WriteLine($" - {item.Country}: {item.CustomerCount}");
        }

        static async Task ShowCityCountByCountry()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT Country, COUNT(DISTINCT City) as CityCount FROM Customers GROUP BY Country";
            var result = await db.QueryAsync<CountryCount>(sql);

            Console.WriteLine("\nКоличество городов в каждой стране:");
            foreach (var item in result) Console.WriteLine($" - {item.Country}: {item.CityCount}");
        }

        static async Task ShowAverageCitiesPerCountry()
        {
            using var db = new SqlConnection(connectionString);
            var sql = @"
                SELECT AVG(CAST(CityCount AS FLOAT)) 
                FROM (SELECT COUNT(DISTINCT City) as CityCount FROM Customers GROUP BY Country) AS SubQuery";

            var average = await db.ExecuteScalarAsync<double>(sql);
            Console.WriteLine($"\nСреднее количество городов по всем странам: {average:F2}");
        }

        static async Task ShowCategoriesForCountry(string country)
        {
            using var db = new SqlConnection(connectionString);
            var sql = @"
                SELECT DISTINCT c.Name 
                FROM Categories c
                JOIN CustomerCategories cc ON c.Id = cc.CategoryId
                JOIN Customers cust ON cc.CustomerId = cust.Id
                WHERE cust.Country = @Country";

            var categories = await db.QueryAsync<string>(sql, new { Country = country });

            Console.WriteLine($"Разделы, интересующие покупателей из страны {country}:");
            foreach (var cat in categories) Console.WriteLine($" - {cat}");
        }

        static async Task ShowPromoItemsByCategoryAndDate(int categoryId, DateTime start, DateTime end)
        {
            using var db = new SqlConnection(connectionString);
            var sql = @"
                SELECT * FROM PromoItems 
                WHERE CategoryId = @CategoryId 
                AND StartDate >= @Start AND EndDate <= @End";

            var items = await db.QueryAsync<PromoItem>(sql, new { CategoryId = categoryId, Start = start, End = end });

            Console.WriteLine($"\nАкции в категории ID={categoryId} с {start:dd.MM.yyyy} по {end:dd.MM.yyyy}:");
            foreach (var item in items) Console.WriteLine($" - {item.Name} ({item.StartDate:dd.MM} - {item.EndDate:dd.MM})");
        }

        static async Task ShowPromoItemsForCustomer(string email)
        {
            using var db = new SqlConnection(connectionString);
            var sql = @"
                SELECT DISTINCT p.* 
                FROM PromoItems p
                JOIN CustomerCategories cc ON p.CategoryId = cc.CategoryId
                JOIN Customers c ON cc.CustomerId = c.Id
                WHERE c.Email = @Email";

            var items = await db.QueryAsync<PromoItem>(sql, new { Email = email });

            Console.WriteLine($"\nАкционные товары для покупателя {email}:");
            foreach (var item in items) Console.WriteLine($" - {item.Name}");
        }

        static async Task ShowTop3Countries()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT TOP 3 Country, COUNT(*) as CustomerCount FROM Customers GROUP BY Country ORDER BY CustomerCount DESC";
            var result = await db.QueryAsync<CountryCount>(sql);

            Console.WriteLine("Топ-3 стран по количеству покупателей:");
            foreach (var item in result) Console.WriteLine($" - {item.Country}: {item.CustomerCount}");
        }

        static async Task ShowBestCountry()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT TOP 1 Country, COUNT(*) as CustomerCount FROM Customers GROUP BY Country ORDER BY CustomerCount DESC";
            var result = await db.QueryFirstOrDefaultAsync<CountryCount>(sql);

            Console.WriteLine($"Лучшая страна по покупателям: {result?.Country} ({result?.CustomerCount} чел.)");
        }

        static async Task ShowTop3Cities()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT TOP 3 City, COUNT(*) as CustomerCount FROM Customers GROUP BY City ORDER BY CustomerCount DESC";
            var result = await db.QueryAsync<CityCount>(sql);

            Console.WriteLine("\nТоп-3 городов по количеству покупателей:");
            foreach (var item in result) Console.WriteLine($" - {item.City}: {item.CustomerCount}");
        }

        static async Task ShowBestCity()
        {
            using var db = new SqlConnection(connectionString);
            var sql = "SELECT TOP 1 City, COUNT(*) as CustomerCount FROM Customers GROUP BY City ORDER BY CustomerCount DESC";
            var result = await db.QueryFirstOrDefaultAsync<CityCount>(sql);

            Console.WriteLine($"Лучший город по покупателям: {result?.City} ({result?.CustomerCount} чел.)");
        }
    }
}