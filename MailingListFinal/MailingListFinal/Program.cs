using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MailingListFinal
{
    public class Section { public int Id { get; set; } public string Name { get; set; } = ""; }
    public class Buyer { public int Id { get; set; } public string Name { get; set; } = ""; public int Age { get; set; } public string Gender { get; set; } = ""; public string City { get; set; } = ""; public string Country { get; set; } = ""; }
    public class BuyerSection { public int Id { get; set; } public int BuyerId { get; set; } public int SectionId { get; set; } }
    public class PromoItem { public int Id { get; set; } public string Name { get; set; } = ""; public int SectionId { get; set; } public DateTime EndDate { get; set; } }
    public class ArchivePromoItem { public int Id { get; set; } public string Name { get; set; } = ""; public int SectionId { get; set; } public DateTime EndDate { get; set; } }

    public class AppDbContext : DbContext
    {
        public DbSet<Section> Sections { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<BuyerSection> BuyerSections { get; set; }
        public DbSet<PromoItem> PromoItems { get; set; }
        public DbSet<ArchivePromoItem> ArchivePromoItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MailingListDb");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== СПИСОК РАССЫЛКИ ===");
                Console.WriteLine("1. Задание 1: Тесты функциональности (Заполнить БД)");
                Console.WriteLine("2. Задание 2: Популярность разделов");
                Console.WriteLine("3. Задание 3: Акционные товары по разделам");
                Console.WriteLine("4. Задание 4: Анализ акций и средний возраст");
                Console.WriteLine("5. Задание 5: Анализ по полу покупателей");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите задание: ");

                string? choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1": Task1_SeedData(); break;
                    case "2": Task2_Popularity(); break;
                    case "3": Task3_PromoCounts(); break;
                    case "4": Task4_DatesAndAverages(); break;
                    case "5": Task5_GenderAnalytics(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
                Console.WriteLine("\nНажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        // Задание 1
        static void Task1_SeedData()
        {
            using var db = new AppDbContext();
            db.Database.EnsureDeleted();

            var s1 = new Section { Name = "Спорт" };
            var s2 = new Section { Name = "Технологии" };
            var s3 = new Section { Name = "Мода" };
            var s4 = new Section { Name = "Кулинария" };
            db.Sections.AddRange(s1, s2, s3, s4);
            db.SaveChanges();

            var b1 = new Buyer { Name = "Иван", Age = 25, Gender = "М", City = "Москва", Country = "Россия" };
            var b2 = new Buyer { Name = "Анна", Age = 30, Gender = "Ж", City = "СПб", Country = "Россия" };
            var b3 = new Buyer { Name = "Джон", Age = 40, Gender = "М", City = "Лондон", Country = "Англия" };
            var b4 = new Buyer { Name = "Мария", Age = 22, Gender = "Ж", City = "Москва", Country = "Россия" };
            db.Buyers.AddRange(b1, b2, b3, b4);
            db.SaveChanges();

            db.BuyerSections.AddRange(
                new BuyerSection { BuyerId = b1.Id, SectionId = s2.Id },
                new BuyerSection { BuyerId = b2.Id, SectionId = s2.Id },
                new BuyerSection { BuyerId = b2.Id, SectionId = s3.Id },
                new BuyerSection { BuyerId = b3.Id, SectionId = s1.Id },
                new BuyerSection { BuyerId = b4.Id, SectionId = s2.Id },
                new BuyerSection { BuyerId = b4.Id, SectionId = s3.Id }
            );
            db.SaveChanges();

            db.PromoItems.AddRange(
                new PromoItem { Name = "Смартфон X", SectionId = s2.Id, EndDate = DateTime.Now.AddDays(2) },
                new PromoItem { Name = "Кроссовки", SectionId = s1.Id, EndDate = DateTime.Now.AddDays(-1) },
                new PromoItem { Name = "Ноутбук Y", SectionId = s2.Id, EndDate = DateTime.Now.AddDays(10) },
                new PromoItem { Name = "Платье", SectionId = s3.Id, EndDate = DateTime.Now.AddDays(-5) },
                new PromoItem { Name = "Сковородка", SectionId = s4.Id, EndDate = DateTime.Now.AddDays(5) }
            );
            db.SaveChanges();

            Console.WriteLine("БД успешно заполнена тестовыми данными!");
        }

        // Задание 2
        static void Task2_Popularity()
        {
            using var db = new AppDbContext();

            var popularity = db.Sections
                .Select(s => new {
                    SectionName = s.Name,
                    SubCount = db.BuyerSections.Count(bs => bs.SectionId == s.Id)
                })
                .OrderByDescending(x => x.SubCount)
                .ToList();

            if (!popularity.Any()) { Console.WriteLine("Нет данных. Сначала выполните Задание 1."); return; }

            Console.WriteLine("--- Топ-3 популярных разделов ---");
            foreach (var item in popularity.Take(3)) Console.WriteLine($"{item.SectionName} - {item.SubCount} подписок");
            Console.WriteLine($"\nСамый популярный: {popularity.First().SectionName}");

            Console.WriteLine("\n--- Топ-3 непопулярных разделов ---");
            foreach (var item in popularity.OrderBy(x => x.SubCount).Take(3)) Console.WriteLine($"{item.SectionName} - {item.SubCount} подписок");
            Console.WriteLine($"\nСамый непопулярный: {popularity.OrderBy(x => x.SubCount).First().SectionName}");
        }

        // Задание 3
        static void Task3_PromoCounts()
        {
            using var db = new AppDbContext();

            var promoStats = db.Sections
                .Select(s => new {
                    SectionName = s.Name,
                    PromoCount = db.PromoItems.Count(p => p.SectionId == s.Id)
                })
                .OrderByDescending(x => x.PromoCount)
                .ToList();

            if (!promoStats.Any()) { Console.WriteLine("Нет данных. Сначала выполните Задание 1."); return; }

            Console.WriteLine("--- Топ-3 по кол-ву акционных товаров ---");
            foreach (var item in promoStats.Take(3)) Console.WriteLine($"{item.SectionName} - {item.PromoCount} акций");
            Console.WriteLine($"Наибольшее количество акций: {promoStats.First().SectionName}");

            Console.WriteLine("\n--- Топ-3 с наименьшим кол-вом акций ---");
            foreach (var item in promoStats.OrderBy(x => x.PromoCount).Take(3)) Console.WriteLine($"{item.SectionName} - {item.PromoCount} акций");
            Console.WriteLine($"Наименьшее количество акций: {promoStats.OrderBy(x => x.PromoCount).First().SectionName}");
        }

        // Задание 4
        static void Task4_DatesAndAverages()
        {
            using var db = new AppDbContext();
            DateTime today = DateTime.Now;

            var endingSoon = db.PromoItems.Where(p => p.EndDate >= today && p.EndDate <= today.AddDays(3)).ToList();
            Console.WriteLine("--- Акции, заканчивающиеся через 3 дня ---");
            endingSoon.ForEach(p => Console.WriteLine($"- {p.Name} (до {p.EndDate:dd.MM.yyyy})"));

            var expired = db.PromoItems.Where(p => p.EndDate < today).ToList();
            Console.WriteLine($"\n--- Просроченные акции ({expired.Count} шт.) ---");
            expired.ForEach(p => Console.WriteLine($"- {p.Name} (истекла {p.EndDate:dd.MM.yyyy})"));

            if (expired.Any())
            {
                foreach (var item in expired)
                {
                    db.ArchivePromoItems.Add(new ArchivePromoItem { Name = item.Name, SectionId = item.SectionId, EndDate = item.EndDate });
                    db.PromoItems.Remove(item);
                }
                db.SaveChanges();
                Console.WriteLine("\n[!] Просроченные акции перенесены в таблицу 'Архив акций'.");
            }

            Console.WriteLine("\n--- Средний возраст по разделам ---");
            var avgBySection = db.Sections.Select(s => new {
                Section = s.Name,
                AvgAge = db.BuyerSections.Where(bs => bs.SectionId == s.Id)
                                         .Join(db.Buyers, bs => bs.BuyerId, b => b.Id, (bs, b) => b.Age)
                                         .DefaultIfEmpty(0).Average()
            }).ToList();
            avgBySection.ForEach(x => Console.WriteLine($"- {x.Section}: {x.AvgAge:F1} лет"));

            Console.WriteLine("\n--- Средний возраст по городам ---");
            db.Buyers.GroupBy(b => b.City).ToList().ForEach(g => Console.WriteLine($"- {g.Key}: {g.Average(b => b.Age):F1} лет"));

            Console.WriteLine("\n--- Средний возраст по странам ---");
            db.Buyers.GroupBy(b => b.Country).ToList().ForEach(g => Console.WriteLine($"- {g.Key}: {g.Average(b => b.Age):F1} лет"));
        }

        // Задание 5
        static void Task5_GenderAnalytics()
        {
            using var db = new AppDbContext();

            var genderSections = db.BuyerSections
                .Join(db.Buyers, bs => bs.BuyerId, b => b.Id, (bs, b) => new { bs.SectionId, b.Gender })
                .Join(db.Sections, x => x.SectionId, s => s.Id, (x, s) => new { x.Gender, SectionName = s.Name })
                .GroupBy(x => new { x.Gender, x.SectionName })
                .Select(g => new { g.Key.Gender, g.Key.SectionName, Count = g.Count() })
                .ToList();

            if (!genderSections.Any()) { Console.WriteLine("Нет данных. Сначала выполните Задание 1."); return; }

            Console.WriteLine("--- Самый популярный раздел для каждого пола ---");
            var mostPop = genderSections.GroupBy(x => x.Gender)
                .Select(g => g.OrderByDescending(x => x.Count).FirstOrDefault())
                .ToList();
            mostPop.ForEach(x => Console.WriteLine($"- {x!.Gender}: {x.SectionName}"));

            Console.WriteLine("\n--- Топ-3 разделов для каждого пола ---");
            var top3 = genderSections.GroupBy(x => x.Gender).ToList();
            foreach (var group in top3)
            {
                Console.WriteLine($"Пол: {group.Key}");
                foreach (var item in group.OrderByDescending(x => x.Count).Take(3))
                    Console.WriteLine($"  * {item.SectionName} ({item.Count} чел.)");
            }

            Console.WriteLine("\n--- Количество покупателей каждого пола ---");
            db.Buyers.GroupBy(b => b.Gender).ToList()
                .ForEach(g => Console.WriteLine($"- {g.Key}: {g.Count()} чел."));

            Console.WriteLine("\n--- Количество покупателей по полу для каждой страны ---");
            var byCountry = db.Buyers.GroupBy(b => new { b.Country, b.Gender })
                .Select(g => new { g.Key.Country, g.Key.Gender, Count = g.Count() }).ToList();
            byCountry.ForEach(x => Console.WriteLine($"- {x.Country} | {x.Gender}: {x.Count} чел."));
        }
    }
}