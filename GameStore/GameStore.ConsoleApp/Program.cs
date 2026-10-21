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
            using (AppDbContext db = new AppDbContext())
            {
                // Создаем базу данных
                db.Database.EnsureCreated();

                // Добавляем данные, если база пустая
                if (!db.Games.Any())
                {
                    db.Games.Add(new Game { Name = "The Witcher 3", Studio = "CD Projekt", Genre = "RPG", ReleaseDate = new DateTime(2015, 5, 19) });
                    db.Games.Add(new Game { Name = "GTA V", Studio = "Rockstar", Genre = "Action", ReleaseDate = new DateTime(2013, 9, 17) });
                    db.SaveChanges();
                }

                Console.WriteLine("Список игр в базе:");
                foreach (var game in db.Games.ToList())
                {
                    Console.WriteLine($"- {game.Name} | Студия: {game.Studio} | Жанр: {game.Genre} | Дата: {game.ReleaseDate.ToShortDateString()}");
                }
            }

            Console.ReadLine();
        }
    }
}