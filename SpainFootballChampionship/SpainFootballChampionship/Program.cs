using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SpainFootballChampionship
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new();
        public List<Match> HomeMatches { get; set; } = new();
        public List<Match> AwayMatches { get; set; } = new();
    }

    public class Player
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Position { get; set; } = string.Empty;

        public int TeamId { get; set; }
        public Team? Team { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }
        public int Team1Id { get; set; }
        public Team? Team1 { get; set; }
        public int Team2Id { get; set; }
        public Team? Team2 { get; set; }
        public int Team1Goals { get; set; }
        public int Team2Goals { get; set; }
        public string ScorersInfo { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SpainFootballDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team1).WithMany(t => t.HomeMatches).HasForeignKey(m => m.Team1Id).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team2).WithMany(t => t.AwayMatches).HasForeignKey(m => m.Team2Id).OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new ApplicationContext();

            context.Database.EnsureCreated();

            if (!context.Teams.Any())
            {
                SeedData(context);
            }

            Console.WriteLine("--- ЗАДАНИЕ 2 ---");
            ShowGoalDifference(context, "Реал Мадрид");
            ShowGoalDifference(context, "Барселона");

            Console.WriteLine("\nПолная информация о матчах:");
            ShowAllMatches(context);

            DateTime testDate = DateTime.Today.AddDays(-2);
            Console.WriteLine($"\nМатчи на конкретную дату ({testDate:dd.MM.yyyy}):");
            ShowMatchesByDate(context, testDate);

            Console.WriteLine("\nМатчи конкретной команды (Барселона):");
            ShowMatchesByTeam(context, "Барселона");

            Console.WriteLine($"\nИгроки, забившие голы {testDate:dd.MM.yyyy}:");
            ShowScorersByDate(context, testDate);

            Console.WriteLine("\n--- ЗАДАНИЕ 3 ---");

            AddMatchWithCheck(context, "Реал Мадрид", "Барселона", DateTime.Today.AddDays(-2));
            AddMatchWithCheck(context, "Реал Мадрид", "Атлетико", DateTime.Today);

            UpdateMatch(context, matchId: 1, newTeam1Goals: 5, newScorers: "Винисиус (3), Беллингем (2); Левандовски");

            DeleteMatchByTeamsAndDate(context, "Реал Мадрид", "Атлетико", DateTime.Today);
        }

        static void SeedData(ApplicationContext context)
        {
            var realMadrid = new Team { Name = "Реал Мадрид" };
            var barcelona = new Team { Name = "Барселона" };
            var atletico = new Team { Name = "Атлетико" };
            context.Teams.AddRange(realMadrid, barcelona, atletico);
            context.SaveChanges();

            context.Players.AddRange(
                new Player { FullName = "Винисиус Жуниор", Country = "Бразилия", Number = 7, Position = "Нападающий", TeamId = realMadrid.Id },
                new Player { FullName = "Джуд Беллингем", Country = "Англия", Number = 5, Position = "Полузащитник", TeamId = realMadrid.Id },
                new Player { FullName = "Роберт Левандовски", Country = "Польша", Number = 9, Position = "Нападающий", TeamId = barcelona.Id }
            );
            context.SaveChanges();

            context.Matches.AddRange(
                new Match { Team1Id = realMadrid.Id, Team2Id = barcelona.Id, Team1Goals = 3, Team2Goals = 1, ScorersInfo = "Винисиус (2), Беллингем; Левандовски", MatchDate = DateTime.Today.AddDays(-2) },
                new Match { Team1Id = atletico.Id, Team2Id = barcelona.Id, Team1Goals = 0, Team2Goals = 2, ScorersInfo = "Левандовски (2)", MatchDate = DateTime.Today.AddDays(-5) }
            );
            context.SaveChanges();
        }

        static void ShowGoalDifference(ApplicationContext context, string teamName)
        {
            var team = context.Teams
                .Include(t => t.HomeMatches)
                .Include(t => t.AwayMatches)
                .FirstOrDefault(t => t.Name == teamName);

            if (team == null) return;

            int scored = team.HomeMatches.Sum(m => m.Team1Goals) + team.AwayMatches.Sum(m => m.Team2Goals);
            int conceded = team.HomeMatches.Sum(m => m.Team2Goals) + team.AwayMatches.Sum(m => m.Team1Goals);

            Console.WriteLine($"Команда {team.Name}: Забито {scored}, Пропущено {conceded}. Разница: {scored - conceded}");
        }

        static void ShowAllMatches(ApplicationContext context)
        {
            var matches = context.Matches.Include(m => m.Team1).Include(m => m.Team2).ToList();
            foreach (var m in matches)
            {
                Console.WriteLine($"[{m.Id}] {m.Team1.Name} {m.Team1Goals} : {m.Team2Goals} {m.Team2.Name} | Дата: {m.MatchDate:dd.MM.yyyy} | Голы: {m.ScorersInfo}");
            }
        }

        static void ShowMatchesByDate(ApplicationContext context, DateTime date)
        {
            var matches = context.Matches
                .Include(m => m.Team1).Include(m => m.Team2)
                .Where(m => m.MatchDate.Date == date.Date)
                .ToList();

            if (!matches.Any()) Console.WriteLine("Матчей на эту дату нет.");
            foreach (var m in matches)
                Console.WriteLine($"{m.Team1.Name} {m.Team1Goals} : {m.Team2Goals} {m.Team2.Name}");
        }

        static void ShowMatchesByTeam(ApplicationContext context, string teamName)
        {
            var matches = context.Matches
                .Include(m => m.Team1).Include(m => m.Team2)
                .Where(m => m.Team1.Name == teamName || m.Team2.Name == teamName)
                .ToList();

            foreach (var m in matches)
                Console.WriteLine($"{m.Team1.Name} {m.Team1Goals} : {m.Team2Goals} {m.Team2.Name}");
        }

        static void ShowScorersByDate(ApplicationContext context, DateTime date)
        {
            var scorers = context.Matches
                .Where(m => m.MatchDate.Date == date.Date)
                .Select(m => m.ScorersInfo)
                .ToList();

            if (!scorers.Any()) Console.WriteLine("Голов в этот день не было.");
            foreach (var s in scorers) Console.WriteLine($"- {s}");
        }

        static void AddMatchWithCheck(ApplicationContext context, string team1Name, string team2Name, DateTime date)
        {
            bool exists = context.Matches
                .Include(m => m.Team1).Include(m => m.Team2)
                .Any(m => m.Team1.Name == team1Name && m.Team2.Name == team2Name && m.MatchDate.Date == date.Date);

            if (exists)
            {
                Console.WriteLine($"Матч {team1Name} - {team2Name} на {date:dd.MM.yyyy} уже существует в базе!");
                return;
            }

            var t1 = context.Teams.First(t => t.Name == team1Name);
            var t2 = context.Teams.First(t => t.Name == team2Name);

            var newMatch = new Match
            {
                Team1Id = t1.Id,
                Team2Id = t2.Id,
                Team1Goals = 0,
                Team2Goals = 0,
                ScorersInfo = "Пока нет",
                MatchDate = date
            };

            context.Matches.Add(newMatch);
            context.SaveChanges();
            Console.WriteLine($"Матч {team1Name} - {team2Name} на {date:dd.MM.yyyy} успешно добавлен.");
        }

        static void UpdateMatch(ApplicationContext context, int matchId, int newTeam1Goals, string newScorers)
        {
            var match = context.Matches.Find(matchId);
            if (match == null)
            {
                Console.WriteLine($"Матч с ID {matchId} не найден.");
                return;
            }

            match.Team1Goals = newTeam1Goals;
            match.ScorersInfo = newScorers;
            context.SaveChanges();
            Console.WriteLine($"Данные матча с ID {matchId} успешно изменены.");
        }

        static void DeleteMatchByTeamsAndDate(ApplicationContext context, string team1Name, string team2Name, DateTime date)
        {
            var match = context.Matches
                .Include(m => m.Team1).Include(m => m.Team2)
                .FirstOrDefault(m => m.Team1.Name == team1Name && m.Team2.Name == team2Name && m.MatchDate.Date == date.Date);

            if (match == null)
            {
                Console.WriteLine($"Матч {team1Name} - {team2Name} на {date:dd.MM.yyyy} не найден для удаления.");
                return;
            }

            Console.Write($"Вы уверены, что хотите удалить матч {team1Name} - {team2Name} ({date:dd.MM.yyyy})? (y/n): ");
            string? answer = Console.ReadLine();

            if (answer?.ToLower() == "y")
            {
                context.Matches.Remove(match);
                context.SaveChanges();
                Console.WriteLine("Матч удален.");
            }
            else
            {
                Console.WriteLine("Удаление отменено.");
            }
        }
    }
}