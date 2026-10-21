using ConsoleApp8;
using System;
using System.Data.Entity;
using System.Linq;

namespace CarRentalDbFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА: АВТОПАРК\n");

            // Вызовы методов
            Task1_AddInitialData();
            Task2_GetAllCars();
            Task3_FindCarByModel("Camry");
            Task4_GetCarById(1);
            Task5_GetCarsWithBrandInfo();
            Task6_AddNewCar("X5", 2023, 150.00m, 1);
            Task7_UpdateCarPrice(1, 200.00m);

            Console.ReadLine();
        }

        // Задание 1
        static void Task1_AddInitialData()
        {
            Console.WriteLine("Добавление начальных данных...");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                if (!db.Brands.Any())
                {
                    var toyota = new Brand { Name = "Toyota", Country = "Япония" };
                    var bmw = new Brand { Name = "BMW", Country = "Германия" };
                    db.Brands.Add(toyota);
                    db.Brands.Add(bmw);
                    db.SaveChanges();

                    var car1 = new Car { Model = "Camry", ReleaseYear = 2020, PricePerDay = 50.00m, BrandId = 1 };
                    var car2 = new Car { Model = "Corolla", ReleaseYear = 2019, PricePerDay = 40.00m, BrandId = 1 };
                    var car3 = new Car { Model = "X5", ReleaseYear = 2022, PricePerDay = 100.00m, BrandId = 2 };

                    db.Cars.Add(car1);
                    db.Cars.Add(car2);
                    db.Cars.Add(car3);
                    db.SaveChanges();

                    Console.WriteLine("Данные успешно добавлены!");
                }
                else
                {
                    Console.WriteLine("База данных уже содержит записи.");
                }
            }
        }

        // Задание 2
        static void Task2_GetAllCars()
        {
            Console.WriteLine("\nЗадание 2: Все автомобили по алфавиту");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                var cars = db.Cars.OrderBy(c => c.Model).ToList();

                foreach (var car in cars)
                {
                    Console.WriteLine($"Модель: {car.Model}, Цена за день: {car.PricePerDay}");
                }
            }
        }

        // Задание 3
        static void Task3_FindCarByModel(string modelName)
        {
            Console.WriteLine($"\nЗадание 3: Поиск машины '{modelName}'");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                var car = db.Cars.FirstOrDefault(c => c.Model == modelName);

                if (car != null)
                {
                    Console.WriteLine($"Найдена модель: {car.Model}, Год выпуска: {car.ReleaseYear}");
                }
                else
                {
                    Console.WriteLine("Машина не найдена");
                }
            }
        }

        // Задание 4
        static void Task4_GetCarById(int id)
        {
            Console.WriteLine($"\nЗадание 4: Поиск по ID ({id})");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                var car = db.Cars.Find(id);

                if (car != null)
                {
                    Console.WriteLine($"Модель найденной машины: {car.Model}");
                }
                else
                {
                    Console.WriteLine("Машина с таким ID не найдена");
                }
            }
        }

        // Задание 5
        static void Task5_GetCarsWithBrandInfo()
        {
            Console.WriteLine("\nЗадание 5: Автомобили с названием марки");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                var cars = db.Cars.ToList();

                foreach (var car in cars)
                {
                    Console.WriteLine($"Модель: {car.Model} | Марка: {car.Brand.Name} | Страна: {car.Brand.Country}");
                }
            }
        }

        // Задание 6
        static void Task6_AddNewCar(string model, int year, decimal price, int brandId)
        {
            Console.WriteLine($"\nЗадание 6: Добавление машины '{model}'");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                var newCar = new Car
                {
                    Model = model,
                    ReleaseYear = year,
                    PricePerDay = price,
                    BrandId = brandId
                };

                db.Cars.Add(newCar);
                db.SaveChanges();

                Console.WriteLine("Машина успешно добавлена!");
            }
        }

        // Задание 7
        static void Task7_UpdateCarPrice(int carId, decimal newPrice)
        {
            Console.WriteLine($"\nЗадание 7: Обновление цены машины (ID: {carId})");
            using (CarRentalDbEntities db = new CarRentalDbEntities())
            {
                var car = db.Cars.Find(carId);

                if (car != null)
                {
                    car.PricePerDay = newPrice;
                    db.SaveChanges();
                    Console.WriteLine("Цена успешно обновлена!");
                }
                else
                {
                    Console.WriteLine("Машина не найдена.");
                }
            }
        }
    }
}