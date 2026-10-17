using System;
using System.Collections.Generic;
using System.Linq; 

namespace CountryApp
{
    public class Country
    {
        public string Name { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public int Population { get; set; }
        public double Area { get; set; }
        public string PartOfWorld { get; set; } = string.Empty;
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Country> countries = new List<Country>
            {
                new Country { Name = "Россия", Capital = "Москва", Population = 146000000, Area = 17098246, PartOfWorld = "Европа/Азия" },
                new Country { Name = "Франция", Capital = "Париж", Population = 67390000, Area = 551695, PartOfWorld = "Европа" },
                new Country { Name = "Германия", Capital = "Берлин", Population = 83240000, Area = 357022, PartOfWorld = "Европа" },
                new Country { Name = "Китай", Capital = "Пекин", Population = 1402112000, Area = 9596961, PartOfWorld = "Азия" },
                new Country { Name = "Египет", Capital = "Каир", Population = 100000000, Area = 1002450, PartOfWorld = "Африка" },
                new Country { Name = "США", Capital = "Вашингтон", Population = 331000000, Area = 9833517, PartOfWorld = "Северная Америка" },
                new Country { Name = "Бразилия", Capital = "Бразилиа", Population = 211000000, Area = 8515767, PartOfWorld = "Южная Америка" }
            };

            Console.WriteLine("--- Таблица: Страны ---");
            foreach (var country in countries)
            {
                Console.WriteLine($"Страна: {country.Name}, Столица: {country.Capital}, Население: {country.Population}, Площадь: {country.Area}, Часть света: {country.PartOfWorld}");
            }

            //     задача 2
            // 1. Отобразить всю информацию о странах;
            Console.WriteLine("\n Задача 2");
            Console.WriteLine("\n--- 1. Вся информация о странах ---");
            var allInfo = from c in countries
                          select c;
            foreach (var c in allInfo)
            {
                Console.WriteLine($"{c.Name}, {c.Capital}, {c.Population}, {c.Area}, {c.PartOfWorld}");
            }

            // 2. Отобразить название стран;
            Console.WriteLine("\n--- 2. Названия стран ---");
            var countryNames = from c in countries
                               select c.Name;
            foreach (var name in countryNames)
            {
                Console.WriteLine(name);
            }

            // 3. Отобразить название столиц;
            Console.WriteLine("\n--- 3. Названия столиц ---");
            var capitalNames = from c in countries
                               select c.Capital;
            foreach (var capital in capitalNames)
            {
                Console.WriteLine(capital);
            }

            // 4. Отобразить название всех европейских стран;
            Console.WriteLine("\n--- 4. Европейские страны ---");
            var europeanCountries = from c in countries
                                    where c.PartOfWorld.Contains("Европа")
                                    select c.Name;
            foreach (var name in europeanCountries)
            {
                Console.WriteLine(name);
            }

            // 5. Отобразить название стран с площадью большей конкретного числа.
            double targetArea = 5000000;
            Console.WriteLine($"\n--- 5. Страны с площадью больше {targetArea} ---");
            var largeCountries = from c in countries
                                 where c.Area > targetArea
                                 select c.Name;
            foreach (var name in largeCountries)
            {
                Console.WriteLine(name);
            }

            Console.WriteLine("\nЗадача 3");
            Console.WriteLine("\n--- 1. Страны, в названии которых есть буквы 'а' и 'и' ---");
            var countriesWithAandI = from c in countries
                                     where c.Name.ToLower().Contains("а") && c.Name.ToLower().Contains("и")
                                     select c.Name;
            foreach (var name in countriesWithAandI)
            {
                Console.WriteLine(name);
            }

            // 2. Отобразить все страны, у которых название начинается с буквы а;
            Console.WriteLine("\n--- 2. Страны, начинающиеся на букву 'А' ---");
            var countriesStartingWithA = from c in countries
                                         where c.Name.ToLower().StartsWith("а")
                                         select c.Name;
            foreach (var name in countriesStartingWithA)
            {
                Console.WriteLine(name);
            }

            // 3. Отобразить название стран, у которых площадь находится в указанном диапазоне;
            double minArea = 500000;
            double maxArea = 10000000;
            Console.WriteLine($"\n--- 3. Страны с площадью от {minArea} до {maxArea} ---");
            var countriesInAreaRange = from c in countries
                                       where c.Area >= minArea && c.Area <= maxArea
                                       select c.Name;
            foreach (var name in countriesInAreaRange)
            {
                Console.WriteLine(name);
            }

            // 4. Отобразить название стран, у которых количество жителей больше указанного числа.
            int targetPopulation = 150000000;
            Console.WriteLine($"\n--- 4. Страны с населением больше {targetPopulation} ---");
            var highPopulationCountries = from c in countries
                                          where c.Population > targetPopulation
                                          select c.Name;
            foreach (var name in highPopulationCountries)
            {
                Console.WriteLine(name);
            }





            Console.WriteLine("\nЗадача 4");
            Console.WriteLine("\n--- 1. Топ-5 стран по площади ---");
            var top5ByArea = (from c in countries
                              orderby c.Area descending
                              select c).Take(5);
            foreach (var c in top5ByArea)
            {
                Console.WriteLine($"{c.Name} - Площадь: {c.Area}");
            }

            // 2. Показать топ-5 стран по количеству жителей;
            Console.WriteLine("\n--- 2. Топ-5 стран по количеству жителей ---");
            var top5ByPopulation = (from c in countries
                                    orderby c.Population descending
                                    select c).Take(5);
            foreach (var c in top5ByPopulation)
            {
                Console.WriteLine($"{c.Name} - Население: {c.Population}");
            }

            // 3. Показать страну с самой большой площадью;
            Console.WriteLine("\n--- 3. Страна с самой большой площадью ---");
            double absoluteMaxArea = countries.Max(c => c.Area);
            var largestCountry = (from c in countries
                                  where c.Area == absoluteMaxArea
                                  select c).FirstOrDefault();
            if (largestCountry != null)
                Console.WriteLine($"{largestCountry.Name} - Площадь: {largestCountry.Area}");

            // 4. Показать страну с самым большим количеством жителей;
            Console.WriteLine("\n--- 4. Страна с самым большим количеством жителей ---");
            int maxPopulation = countries.Max(c => c.Population);
            var mostPopulousCountry = (from c in countries
                                       where c.Population == maxPopulation
                                       select c).FirstOrDefault();
            if (mostPopulousCountry != null)
                Console.WriteLine($"{mostPopulousCountry.Name} - Население: {mostPopulousCountry.Population}");

            // 5. Показать страну с самой маленькой площадью в Африке;
            Console.WriteLine("\n--- 5. Страна с самой маленькой площадью в Африке ---");
            var smallestInAfrica = (from c in countries
                                    where c.PartOfWorld == "Африка"
                                    orderby c.Area ascending
                                    select c).FirstOrDefault();
            if (smallestInAfrica != null)
                Console.WriteLine($"{smallestInAfrica.Name} - Площадь: {smallestInAfrica.Area}");
            else
                Console.WriteLine("Стран в Африке не найдено.");

            // 6. Показать среднюю площадь стран в Азии.
            Console.WriteLine("\n--- 6. Средняя площадь стран в Азии ---");
            if (countries.Any(c => c.PartOfWorld == "Азия"))
            {
                double avgAreaAsia = (from c in countries
                                      where c.PartOfWorld == "Азия"
                                      select c.Area).Average();
                Console.WriteLine($"Средняя площадь: {avgAreaAsia:F2}"); // F2 округляет до 2 знаков
            }
            else
            {
                Console.WriteLine("Стран в Азии не найдено.");
            }

        }
    }
}