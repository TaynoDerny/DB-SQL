using System;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Configuration;
using System.Collections.Generic;

namespace CountriesReportApp
{
    // Классы таблиц БД
    [Table(Name = "Countries")]
    public class Country
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int CountryID { get; set; }
        [Column] public string Name { get; set; }
        [Column] public int Population { get; set; }
        [Column] public double Area { get; set; }
        [Column] public string PartOfWorld { get; set; }
    }

    [Table(Name = "Capitals")]
    public class Capital
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int CapitalID { get; set; }
        [Column] public string Name { get; set; }
        [Column] public int Population { get; set; }
        [Column] public int CountryID { get; set; }
    }

    [Table(Name = "Cities")]
    public class City
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int CityID { get; set; }
        [Column] public string Name { get; set; }
        [Column] public int Population { get; set; }
        [Column] public int CountryID { get; set; }
    }

    public class CountriesDataContext : DataContext
    {
        public CountriesDataContext() : base(ConfigurationManager.ConnectionStrings["CountriesDBConnectionString"].ConnectionString) { }
        public Table<Country> Countries { get { return GetTable<Country>(); } }
        public Table<Capital> Capitals { get { return GetTable<Capital>(); } }
        public Table<City> Cities { get { return GetTable<City>(); } }
    }

    class Program
    {
        static CountriesDataContext db;
        static bool isConnected = false;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Задача 1
            try
            {
                db = new CountriesDataContext();
                int count = db.Countries.Count();
                isConnected = true;
                Console.WriteLine("УСПЕШНОЕ ПОДКЛЮЧЕНИЕ К БАЗЕ ДАННЫХ 'CountriesDB'!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ОШИБКА ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ!");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                return;
            }

            while (isConnected)
            {
                Console.WriteLine("\n--- МЕНЮ ОТЧЕТОВ ---");
                Console.WriteLine("1. [Задача 2] Полная информация о странах");
                Console.WriteLine("2. [Задача 2] Частичная информация (выбор полей)");
                Console.WriteLine("3. [Задача 2] Информация о конкретной стране");
                Console.WriteLine("4. [Задача 2] Города конкретной страны");
                Console.WriteLine("5. [Задача 3] Страны на указанную букву");
                Console.WriteLine("6. [Задача 3] Столицы на указанную букву");
                Console.WriteLine("7. [Задача 3] Топ-3 столиц с наименьшим населением");
                Console.WriteLine("8. [Задача 3] Топ-3 стран с наименьшим населением");
                Console.WriteLine("9. [Задача 3] Среднее население столиц по частям света");
                Console.WriteLine("10. [Задача 4] Топ-3 стран по частям света (наименьшее население)");
                Console.WriteLine("11. [Задача 4] Топ-3 стран по частям света (наибольшее население)");
                Console.WriteLine("12. [Задача 4] Среднее население городов в стране");
                Console.WriteLine("13. [Задача 4] Город с наименьшим населением в стране");
                Console.WriteLine("0. Выйти");
                Console.Write("Выберите пункт: ");

                string choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1": FullInfo(); break;
                    case "2": PartialInfo(); break;
                    case "3": SpecificCountryInfo(); break;
                    case "4": CitiesOfCountry(); break;
                    case "5": CountriesByLetter(); break;
                    case "6": CapitalsByLetter(); break;
                    case "7": Top3CapitalsLeastPop(); break;
                    case "8": Top3CountriesLeastPop(); break;
                    case "9": AvgCapitalPopByPart(); break;
                    case "10": Top3CountriesLeastByPart(); break;
                    case "11": Top3CountriesMostByPart(); break;
                    case "12": AvgCityPopInCountry(); break;
                    case "13": CityLeastPopInCountry(); break;
                    case "0":
                        db.Dispose();
                        isConnected = false;
                        Console.WriteLine("Выход из программы.");
                        break;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
            }
        }

        static void AskSaveToFile(string content, string defaultFileName)
        {
            Console.Write("\nВывести в файл? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                File.WriteAllText(defaultFileName, content);
                Console.WriteLine($"Отчет сохранен в файл: {defaultFileName}");
            }
        }

        // Задача 2
        static void FullInfo()
        {
            var list = db.Countries.ToList();
            string res = string.Join("\n", list.Select(c => $"Страна: {c.Name}, Население: {c.Population}, Площадь: {c.Area}, Часть света: {c.PartOfWorld}"));
            Console.WriteLine(res);
            AskSaveToFile(res, "FullCountries.txt");
        }

        static void PartialInfo()
        {
            Console.Write("Введите сколько полей вывести (1-4): ");
            int fieldsCount = int.Parse(Console.ReadLine());
            var list = db.Countries.ToList();

            List<string> resList = new List<string>();
            foreach (var c in list)
            {
                string row = "";
                if (fieldsCount >= 1) row += $"Страна: {c.Name}; ";
                if (fieldsCount >= 2) row += $"Население: {c.Population}; ";
                if (fieldsCount >= 3) row += $"Площадь: {c.Area}; ";
                if (fieldsCount >= 4) row += $"Часть света: {c.PartOfWorld}; ";
                resList.Add(row);
            }
            string res = string.Join("\n", resList);
            Console.WriteLine(res);
            AskSaveToFile(res, "PartialCountries.txt");
        }

        static void SpecificCountryInfo()
        {
            Console.Write("Введите название страны: ");
            string name = Console.ReadLine();
            var c = db.Countries.FirstOrDefault(x => x.Name == name);
            if (c != null)
            {
                string res = $"Страна: {c.Name}, Население: {c.Population}, Площадь: {c.Area}, Часть света: {c.PartOfWorld}";
                Console.WriteLine(res);
                AskSaveToFile(res, $"Info_{c.Name}.txt");
            }
            else Console.WriteLine("Страна не найдена.");
        }

        static void CitiesOfCountry()
        {
            Console.Write("Введите название страны: ");
            string name = Console.ReadLine();
            var cities = db.Cities.Join(db.Countries, city => city.CountryID, country => country.CountryID, (city, country) => new { City = city, Country = country })
                .Where(x => x.Country.Name == name)
                .Select(x => $"{x.City.Name} (Население: {x.City.Population})")
                .ToList();

            if (cities.Any())
            {
                string res = string.Join("\n", cities);
                Console.WriteLine(res);
                AskSaveToFile(res, $"Cities_{name}.txt");
            }
            else Console.WriteLine("Города не найдены.");
        }

        // Задача 3
        static void CountriesByLetter()
        {
            Console.Write("Введите букву: ");
            string letter = Console.ReadLine().ToLower();
            var list = db.Countries.Where(c => c.Name.ToLower().StartsWith(letter)).Select(c => c.Name).ToList();
            Console.WriteLine(list.Any() ? string.Join(", ", list) : "Не найдено.");
        }

        static void CapitalsByLetter()
        {
            Console.Write("Введите букву: ");
            string letter = Console.ReadLine().ToLower();
            var list = db.Capitals.Where(c => c.Name.ToLower().StartsWith(letter)).Select(c => c.Name).ToList();
            Console.WriteLine(list.Any() ? string.Join(", ", list) : "Не найдено.");
        }

        static void Top3CapitalsLeastPop()
        {
            var list = db.Capitals.OrderBy(c => c.Population).Take(3).Select(c => $"{c.Name} - {c.Population}").ToList();
            Console.WriteLine(string.Join("\n", list));
        }

        static void Top3CountriesLeastPop()
        {
            var list = db.Countries.OrderBy(c => c.Population).Take(3).Select(c => $"{c.Name} - {c.Population}").ToList();
            Console.WriteLine(string.Join("\n", list));
        }

        static void AvgCapitalPopByPart()
        {
            var list = db.Countries.Join(db.Capitals, c => c.CountryID, cap => cap.CountryID, (c, cap) => new { Country = c, Capital = cap })
                .GroupBy(x => x.Country.PartOfWorld)
                .Select(g => new { Part = g.Key, AvgPop = g.Average(x => x.Capital.Population) })
                .ToList();

            foreach (var item in list)
                Console.WriteLine($"{item.Part}: {item.AvgPop:F0} чел.");
        }

        // Задача 4
        static void Top3CountriesLeastByPart()
        {
            var countries = db.Countries.ToList();
            var grouped = countries.GroupBy(c => c.PartOfWorld);

            foreach (var group in grouped)
            {
                Console.WriteLine($"\nЧасть света: {group.Key}");
                var top3 = group.OrderBy(c => c.Population).Take(3);
                foreach (var c in top3) Console.WriteLine($" - {c.Name} ({c.Population})");
            }
        }

        static void Top3CountriesMostByPart()
        {
            var countries = db.Countries.ToList();
            var grouped = countries.GroupBy(c => c.PartOfWorld);

            foreach (var group in grouped)
            {
                Console.WriteLine($"\nЧасть света: {group.Key}");
                var top3 = group.OrderByDescending(c => c.Population).Take(3);
                foreach (var c in top3) Console.WriteLine($" - {c.Name} ({c.Population})");
            }
        }

        static void AvgCityPopInCountry()
        {
            Console.Write("Введите страну: ");
            string name = Console.ReadLine();

            var cities = db.Cities.Join(db.Countries, city => city.CountryID, country => country.CountryID, (city, country) => new { City = city, Country = country })
                .Where(x => x.Country.Name == name)
                .Select(x => x.City.Population)
                .ToList();

            if (cities.Any())
                Console.WriteLine($"Среднее население городов в {name}: {cities.Average():F0} чел.");
            else
                Console.WriteLine("Нет данных по городам этой страны.");
        }

        static void CityLeastPopInCountry()
        {
            Console.Write("Введите страну: ");
            string name = Console.ReadLine();

            var city = db.Cities.Join(db.Countries, c => c.CountryID, co => co.CountryID, (c, co) => new { City = c, Country = co })
                .Where(x => x.Country.Name == name)
                .OrderBy(x => x.City.Population)
                .Select(x => x.City)
                .FirstOrDefault();

            if (city != null)
                Console.WriteLine($"Город с наименьшим населением в {name}: {city.Name} ({city.Population} чел.)");
            else
                Console.WriteLine("Города не найдены.");
        }
    }
}