using System;
using System.Linq;
using GameStore.Data;
using GameStore.Models;

namespace GameStore.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создаем базу при первом запуске
            using (AppDbContext db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                // Если база пустая, добавим стартовые данные
                if (!db.Games.Any())
                {
                    db.Games.Add(new Game { Name = "The Witcher 3", Studio = "CD Projekt", Genre = "RPG", ReleaseDate = new DateTime(2015, 5, 19), GameMode = "Однопользовательский", CopiesSold = 50000000 });
                    db.Games.Add(new Game { Name = "GTA V", Studio = "Rockstar", Genre = "Action", ReleaseDate = new DateTime(2013, 9, 17), GameMode = "Многопользовательский", CopiesSold = 185000000 });
                    db.Games.Add(new Game { Name = "Cyberpunk 2077", Studio = "CD Projekt", Genre = "RPG", ReleaseDate = new DateTime(2020, 12, 10), GameMode = "Однопользовательский", CopiesSold = 25000000 });
                    db.Games.Add(new Game { Name = "Minecraft", Studio = "Mojang", Genre = "Sandbox", ReleaseDate = new DateTime(2011, 11, 18), GameMode = "Многопользовательский", CopiesSold = 300000000 });
                    db.SaveChanges();
                }
            }

            // Главное меню
            while (true)
            {
                Console.Clear();
                Console.WriteLine("ГЛАВНОЕ МЕНЮ:");
                Console.WriteLine("1. Поиск игр");
                Console.WriteLine("2. Аналитика и топы игр");
                Console.WriteLine("3. Добавить новую игру");
                Console.WriteLine("4. Изменить данные игры");
                Console.WriteLine("5. Удалить игру");
                Console.WriteLine("6. Показать все игры");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": SearchMenu(); break;
                    case "2": AnalyticsMenu(); break;
                    case "3": AddGame(); break;
                    case "4": UpdateGame(); break;
                    case "5": DeleteGame(); break;
                    case "6": ShowAllGames(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        static void PrintGame(Game game)
        {
            Console.WriteLine($"[ID: {game.Id}] {game.Name} | Студия: {game.Studio} | Жанр: {game.Genre} | Дата: {game.ReleaseDate.ToShortDateString()} | Режим: {game.GameMode} | Продано: {game.CopiesSold} копий");
        }

        static void ShowAllGames()
        {
            using (AppDbContext db = new AppDbContext())
            {
                var games = db.Games.ToList();
                if (!games.Any()) { Console.WriteLine("База данных пуста."); return; }

                foreach (var game in games) PrintGame(game);
            }
        }

        // Задание 1. Поиск
        static void SearchMenu()
        {
            Console.Clear();
            Console.WriteLine("ПОИСК ИГР:");
            Console.WriteLine("1. По названию игры");
            Console.WriteLine("2. По названию студии");
            Console.WriteLine("3. По студии и названию игры");
            Console.WriteLine("4. По стилю (жанру)");
            Console.WriteLine("5. По году релиза");
            Console.Write("Выберите тип поиска: ");
            string choice = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                var games = db.Games.AsQueryable();

                if (choice == "1")
                {
                    Console.Write("Введите название игры: ");
                    string name = Console.ReadLine().ToLower();
                    games = games.Where(g => g.Name.ToLower().Contains(name));
                }
                else if (choice == "2")
                {
                    Console.Write("Введите название студии: ");
                    string studio = Console.ReadLine().ToLower();
                    games = games.Where(g => g.Studio.ToLower().Contains(studio));
                }
                else if (choice == "3")
                {
                    Console.Write("Введите название студии: ");
                    string studio = Console.ReadLine().ToLower();
                    Console.Write("Введите название игры: ");
                    string name = Console.ReadLine().ToLower();
                    games = games.Where(g => g.Studio.ToLower().Contains(studio) && g.Name.ToLower().Contains(name));
                }
                else if (choice == "4")
                {
                    Console.Write("Введите жанр: ");
                    string genre = Console.ReadLine().ToLower();
                    games = games.Where(g => g.Genre.ToLower().Contains(genre));
                }
                else if (choice == "5")
                {
                    Console.Write("Введите год: ");
                    if (int.TryParse(Console.ReadLine(), out int year))
                        games = games.Where(g => g.ReleaseDate.Year == year);
                    else Console.WriteLine("Ошибка: введите число.");
                }

                var result = games.ToList();
                if (result.Any()) result.ForEach(PrintGame);
                else Console.WriteLine("Игры не найдены.");
            }
        }

        // Задание 2. Аналитика
        static void AnalyticsMenu()
        {
            Console.Clear();
            Console.WriteLine("АНАЛИТИКА:");
            Console.WriteLine("1. Все однопользовательские игры");
            Console.WriteLine("2. Все многопользовательские игры");
            Console.WriteLine("3. Игра с максимальными продажами");
            Console.WriteLine("4. Игра с минимальными продажами");
            Console.WriteLine("5. Топ-3 самых продаваемых игр");
            Console.WriteLine("6. Топ-3 самых непродаваемых игр");
            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                if (choice == "1")
                {
                    var games = db.Games.Where(g => g.GameMode == "Однопользовательский").ToList();
                    games.ForEach(PrintGame);
                }
                else if (choice == "2")
                {
                    var games = db.Games.Where(g => g.GameMode == "Многопользовательский").ToList();
                    games.ForEach(PrintGame);
                }
                else if (choice == "3")
                {
                    var game = db.Games.OrderByDescending(g => g.CopiesSold).FirstOrDefault();
                    if (game != null) PrintGame(game);
                }
                else if (choice == "4")
                {
                    var game = db.Games.OrderBy(g => g.CopiesSold).FirstOrDefault();
                    if (game != null) PrintGame(game);
                }
                else if (choice == "5")
                {
                    var games = db.Games.OrderByDescending(g => g.CopiesSold).Take(3).ToList();
                    games.ForEach(PrintGame);
                }
                else if (choice == "6")
                {
                    var games = db.Games.OrderBy(g => g.CopiesSold).Take(3).ToList();
                    games.ForEach(PrintGame);
                }
            }
        }

        // Задание 3. Добавление
        static void AddGame()
        {
            Console.Clear();
            Console.WriteLine("ДОБАВЛЕНИЕ ИГРЫ");

            Console.Write("Название игры: "); string name = Console.ReadLine();
            Console.Write("Студия: "); string studio = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                // Проверка на дубликат
                bool exists = db.Games.Any(g => g.Name.ToLower() == name.ToLower() && g.Studio.ToLower() == studio.ToLower());
                if (exists)
                {
                    Console.WriteLine("Ошибка: Игра с таким названием от этой студии уже существует!");
                    return;
                }

                Console.Write("Жанр: "); string genre = Console.ReadLine();
                Console.Write("Дата релиза (гггг-мм-дд): "); DateTime date = DateTime.Parse(Console.ReadLine());
                Console.Write("Режим игры (Однопользовательский/Многопользовательский): "); string mode = Console.ReadLine();
                Console.Write("Продано копий: "); int copies = int.Parse(Console.ReadLine());

                Game newGame = new Game { Name = name, Studio = studio, Genre = genre, ReleaseDate = date, GameMode = mode, CopiesSold = copies };
                db.Games.Add(newGame);
                db.SaveChanges();
                Console.WriteLine("Игра успешно добавлена!");
            }
        }

        // Задание 3. Обновление
        static void UpdateGame()
        {
            Console.Clear();
            Console.WriteLine("ИЗМЕНЕНИЕ ИГРЫ");
            Console.Write("Введите название игры для поиска: ");
            string searchName = Console.ReadLine().ToLower();

            using (AppDbContext db = new AppDbContext())
            {
                var game = db.Games.FirstOrDefault(g => g.Name.ToLower().Contains(searchName));
                if (game == null) { Console.WriteLine("Игра не найдена."); return; }

                Console.WriteLine("\nНайдена игра:");
                PrintGame(game);

                Console.WriteLine("\nЧто вы хотите изменить?");
                Console.WriteLine("1. Название\n2. Студию\n3. Жанр\n4. Дату релиза\n5. Режим\n6. Количество копий");
                Console.Write("Выбор: ");
                string field = Console.ReadLine();

                switch (field)
                {
                    case "1": Console.Write("Новое название: "); game.Name = Console.ReadLine(); break;
                    case "2": Console.Write("Новая студия: "); game.Studio = Console.ReadLine(); break;
                    case "3": Console.Write("Новый жанр: "); game.Genre = Console.ReadLine(); break;
                    case "4": Console.Write("Новая дата (гггг-мм-дд): "); game.ReleaseDate = DateTime.Parse(Console.ReadLine()); break;
                    case "5": Console.Write("Новый режим: "); game.GameMode = Console.ReadLine(); break;
                    case "6": Console.Write("Новое кол-во копий: "); game.CopiesSold = int.Parse(Console.ReadLine()); break;
                }

                db.SaveChanges();
                Console.WriteLine("Данные обновлены!");
            }
        }

        // Задание 3. Удаление
        static void DeleteGame()
        {
            Console.Clear();
            Console.WriteLine("УДАЛЕНИЕ ИГРЫ");
            Console.Write("Введите название игры: "); string name = Console.ReadLine().ToLower();
            Console.Write("Введите студию: "); string studio = Console.ReadLine().ToLower();

            using (AppDbContext db = new AppDbContext())
            {
                var game = db.Games.FirstOrDefault(g => g.Name.ToLower().Contains(name) && g.Studio.ToLower().Contains(studio));

                if (game == null)
                {
                    Console.WriteLine("Игра с такими данными не найдена.");
                    return;
                }

                Console.WriteLine("\nНайдена игра для удаления:");
                PrintGame(game);

                Console.Write("\nВы уверены, что хотите её удалить? (y/n): ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    db.Games.Remove(game);
                    db.SaveChanges();
                    Console.WriteLine("Игра удалена.");
                }
                else
                {
                    Console.WriteLine("Удаление отменено.");
                }
            }
        }
    }
}