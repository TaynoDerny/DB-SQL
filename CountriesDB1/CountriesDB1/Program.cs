using System;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Configuration;

namespace CountryApp
{

    [Table(Name = "Countries")]
    public class Country
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int CountryID { get; set; }
        [Column] public string Name { get; set; }
        [Column] public int Population { get; set; }
        [Column] public double Area { get; set; }
        [Column] public string PartOfWorld { get; set; }
    }

    [Table(Name = "Capitals")]
    public class Capital
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int CapitalID { get; set; }
        [Column] public string Name { get; set; }
        [Column] public int Population { get; set; }
        [Column] public int CountryID { get; set; }
    }

    [Table(Name = "Cities")]
    public class City
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int CityID { get; set; }
        [Column] public string Name { get; set; }
        [Column] public int Population { get; set; }
        [Column] public int CountryID { get; set; }
    }

    public class CountriesDataContext : DataContext
    {
        public CountriesDataContext()
            : base(ConfigurationManager.ConnectionStrings["CountriesDBConnectionString"].ConnectionString)
        {
        }

        public Table<Country> Countries { get { return GetTable<Country>(); } }
        public Table<Capital> Capitals { get { return GetTable<Capital>(); } }
        public Table<City> Cities { get { return GetTable<City>(); } }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (CountriesDataContext db = new CountriesDataContext())
            {
                Console.WriteLine(" ЗАДАНИЕ 2 ");

                Console.WriteLine("\n--- 1. Вся информация о странах ---");
                var allInfo = db.Countries.ToList();
                foreach (var c in allInfo) Console.WriteLine($"{c.Name}, Население: {c.Population}, Площадь: {c.Area}, Часть света: {c.PartOfWorld}");

                Console.WriteLine("\n--- 2. Названия стран ---");
                db.Countries.Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 3. Названия столиц ---");
                db.Capitals.Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 4. Крупные города России ---");
                var citiesInRussia = db.Cities
                    .Join(db.Countries, city => city.CountryID, country => country.CountryID, (city, country) => new { City = city, Country = country })
                    .Where(x => x.Country.Name == "Россия")
                    .Select(x => x.City.Name)
                    .ToList();
                citiesInRussia.ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 5. Столицы с населением > 5 млн ---");
                db.Capitals.Where(c => c.Population > 5000000).Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 6. Европейские страны ---");
                db.Countries.Where(c => c.PartOfWorld.Contains("Европа")).Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 7. Страны с площадью > 5 млн ---");
                db.Countries.Where(c => c.Area > 5000000).Select(c => c.Name).ToList().ForEach(Console.WriteLine);


                Console.WriteLine("\n ЗАДАНИЕ 3 ");

                Console.WriteLine("\n--- 1. Столицы с буквами 'а' и 'р' ---");
                db.Capitals.Where(c => c.Name.ToLower().Contains("а") && c.Name.ToLower().Contains("р")).Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 2. Столицы на букву 'К' ---");
                db.Capitals.Where(c => c.Name.ToLower().StartsWith("к")).Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 3. Страны с площадью от 500к до 10млн ---");
                db.Countries.Where(c => c.Area >= 500000 && c.Area <= 10000000).Select(c => c.Name).ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 4. Страны с населением > 150 млн ---");
                db.Countries.Where(c => c.Population > 150000000).Select(c => c.Name).ToList().ForEach(Console.WriteLine);


                Console.WriteLine("\n ЗАДАНИЕ 4 ");

                Console.WriteLine("\n--- 1. Топ-5 стран по площади ---");
                db.Countries.OrderByDescending(c => c.Area).Take(5).Select(c => $"{c.Name} - {c.Area}").ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 2. Топ-5 столиц по населению ---");
                db.Capitals.OrderByDescending(c => c.Population).Take(5).Select(c => $"{c.Name} - {c.Population}").ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 3. Самая большая страна ---");
                var largest = db.Countries.OrderByDescending(c => c.Area).FirstOrDefault();
                if (largest != null) Console.WriteLine($"{largest.Name} - {largest.Area}");

                Console.WriteLine("\n--- 4. Самая населенная столица ---");
                var mostPop = db.Capitals.OrderByDescending(c => c.Population).FirstOrDefault();
                if (mostPop != null) Console.WriteLine($"{mostPop.Name} - {mostPop.Population}");

                Console.WriteLine("\n--- 5. Самая маленькая страна в Европе ---");
                var smallest = db.Countries.Where(c => c.PartOfWorld.Contains("Европа")).OrderBy(c => c.Area).FirstOrDefault();
                if (smallest != null) Console.WriteLine($"{smallest.Name} - {smallest.Area}");

                Console.WriteLine("\n--- 6. Средняя площадь в Европе ---");
                Console.WriteLine($"Средняя площадь: {db.Countries.Where(c => c.PartOfWorld.Contains("Европа")).Average(c => c.Area):F2}");

                Console.WriteLine("\n--- 7. Топ-3 города Китая ---");
                var topCities = db.Cities
                    .Join(db.Countries, c => c.CountryID, co => co.CountryID, (c, co) => new { City = c, Country = co })
                    .Where(x => x.Country.Name == "Китай")
                    .OrderByDescending(x => x.City.Population)
                    .Take(3)
                    .Select(x => $"{x.City.Name} - {x.City.Population}")
                    .ToList();
                topCities.ForEach(Console.WriteLine);

                Console.WriteLine("\n--- 8. Общее количество стран ---");
                Console.WriteLine($"Всего: {db.Countries.Count()}");

                Console.WriteLine("\n--- 9. Часть света с макс. количеством стран ---");
                var maxPart = db.Countries.GroupBy(c => c.PartOfWorld).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
                Console.WriteLine(maxPart);

                Console.WriteLine("\n--- 10. Стран в каждой части света ---");
                db.Countries.GroupBy(c => c.PartOfWorld).Select(g => new { Part = g.Key, Count = g.Count() }).ToList().ForEach(x => Console.WriteLine($"{x.Part}: {x.Count}"));
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadLine();
        }
    }
}