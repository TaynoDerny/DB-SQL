using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SpainFootball_Part4
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Player
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TeamId { get; set; }
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

    public class TeamStats
    {
        public string TeamName { get; set; } = string.Empty;
        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }
        public int Points { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }

        public ApplicationContext() { }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SpainFootballDB;Trusted_Connection=True;");
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            using var context = new ApplicationContext();

            // ЗАДАНИЕ 1: ХРАНИМАЯ ПРОЦЕДУРА
            GenerateRandomMatches(context);

            // ЗАДАНИЕ 2: БОМБАРДИРЫ
            ShowScorers(context);

            // ЗАДАНИЕ 3: ГОЛЫ КОМАНД
            ShowGoalsStats(context);

            // ЗАДАНИЕ 4: ОЧКИ КОМАНД
            ShowPointsStats(context);
        }

        static void GenerateRandomMatches(ApplicationContext context)
        {
            Console.WriteLine("=== ЗАДАНИЕ 1: ХРАНИМАЯ ПРОЦЕДУРА ===");

            Console.WriteLine("Вызываем процедуру sp_GenerateRandomMatch 5 раз...");

            for (int i = 0; i < 5; i++)
            {
                context.Database.ExecuteSqlRaw("EXEC sp_GenerateRandomMatch");
            }

            Console.WriteLine("Готово! Матчи добавлены в базу.\n");
        }

        static void ShowScorers(ApplicationContext context)
        {
            Console.WriteLine("=== ЗАДАНИЕ 2: БОМБАРДИРЫ ===");

            var matches = context.Matches.Where(m => m.ScorersInfo != "").ToList();
            var scorers = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                var parts = match.ScorersInfo.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    string name = part.Trim();
                    int goals = 1;

                    if (name.Contains("("))
                    {
                        int start = name.IndexOf('(') + 1;
                        int end = name.IndexOf(')');
                        if (int.TryParse(name.Substring(start, end - start), out int g)) goals = g;
                        name = name.Substring(0, name.IndexOf('(')).Trim();
                    }

                    if (scorers.ContainsKey(name)) scorers[name] += goals;
                    else scorers[name] = goals;
                }
            }

            var top3 = scorers.OrderByDescending(x => x.Value).Take(3).ToList();
            if (top3.Any())
            {
                Console.WriteLine("Топ-3 бомбардира чемпионата:");
                foreach (var s in top3) Console.WriteLine($" - {s.Key}: {s.Value}");
                Console.WriteLine($"Лучший бомбардир: {top3.First().Key}\n");
            }
        }

        static void ShowGoalsStats(ApplicationContext context)
        {
            Console.WriteLine("=== ЗАДАНИЕ 3: ГОЛЫ КОМАНД ===");

            var stats = GetTeamStats(context);

            var topScored = stats.OrderByDescending(t => t.GoalsScored).Take(3).ToList();
            var topConceded = stats.OrderBy(t => t.GoalsConceded).Take(3).ToList();

            Console.WriteLine("Топ-3 по забитым голам:");
            foreach (var t in topScored) Console.WriteLine($" - {t.TeamName}: {t.GoalsScored}");
            Console.WriteLine($"Лучшая атака: {topScored.First().TeamName}");

            Console.WriteLine("Топ-3 по пропущенным (меньше - лучше):");
            foreach (var t in topConceded) Console.WriteLine($" - {t.TeamName}: {t.GoalsConceded}");
            Console.WriteLine($"Лучшая оборона: {topConceded.First().TeamName}\n");
        }

        static void ShowPointsStats(ApplicationContext context)
        {
            Console.WriteLine("=== ЗАДАНИЕ 4: ОЧКИ КОМАНД ===");

            var stats = GetTeamStats(context);

            var topPoints = stats.OrderByDescending(t => t.Points).Take(3).ToList();
            var lowPoints = stats.OrderBy(t => t.Points).Take(3).ToList();

            Console.WriteLine("Топ-3 по очкам:");
            foreach (var t in topPoints) Console.WriteLine($" - {t.TeamName}: {t.Points}");
            Console.WriteLine($"Чемпион: {topPoints.First().TeamName}");

            Console.WriteLine("Худшие 3 команды по очкам:");
            foreach (var t in lowPoints) Console.WriteLine($" - {t.TeamName}: {t.Points}");
        }

        public static List<TeamStats> GetTeamStats(ApplicationContext context)
        {
            var teams = context.Teams.ToList();
            var matches = context.Matches.ToList();
            var result = new List<TeamStats>();

            foreach (var team in teams)
            {
                var stat = new TeamStats { TeamName = team.Name };

                foreach (var m in matches)
                {
                    if (m.Team1Id == team.Id)
                    {
                        stat.GoalsScored += m.Team1Goals;
                        stat.GoalsConceded += m.Team2Goals;
                        if (m.Team1Goals > m.Team2Goals) stat.Points += 3;
                        else if (m.Team1Goals == m.Team2Goals) stat.Points += 1;
                    }
                    else if (m.Team2Id == team.Id)
                    {
                        stat.GoalsScored += m.Team2Goals;
                        stat.GoalsConceded += m.Team1Goals;
                        if (m.Team2Goals > m.Team1Goals) stat.Points += 3;
                        else if (m.Team2Goals == m.Team1Goals) stat.Points += 1;
                    }
                }
                result.Add(stat);
            }
            return result;
        }
    }
}