using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SpainFootball_Part4;
 // ВАЖНО: Проверь, чтобы это совпадало с названием твоего основного проекта!
using System.Linq;
using Xunit;

namespace SpainFootball_Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_PointsCalculation_Win()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Win")
                .Options;

            using var context = new ApplicationContext(options);

            var teamA = new Team { Name = "Команда А" };
            var teamB = new Team { Name = "Команда Б" };
            context.Teams.AddRange(teamA, teamB);
            context.SaveChanges();

            context.Matches.Add(new Match
            {
                Team1Id = teamA.Id,
                Team2Id = teamB.Id,
                Team1Goals = 2,
                Team2Goals = 1,
                MatchDate = System.DateTime.Now,
                ScorersInfo = "Тест"
            });
            context.SaveChanges();

            var stats = SpainFootball_Part4.Program.GetTeamStats(context);

            var statA = stats.First(s => s.TeamName == "Команда А");
            var statB = stats.First(s => s.TeamName == "Команда Б");

            Assert.Equal(3, statA.Points);
            Assert.Equal(0, statB.Points);
        }

        [Fact]
        public void Test_PointsCalculation_Draw()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Draw")
                .Options;

            using var context = new ApplicationContext(options);

            var teamA = new Team { Name = "Команда А" };
            var teamB = new Team { Name = "Команда Б" };
            context.Teams.AddRange(teamA, teamB);
            context.SaveChanges();

            context.Matches.Add(new Match
            {
                Team1Id = teamA.Id,
                Team2Id = teamB.Id,
                Team1Goals = 1,
                Team2Goals = 1,
                MatchDate = System.DateTime.Now,
                ScorersInfo = "Тест"
            });
            context.SaveChanges();

            var stats = SpainFootball_Part4.Program.GetTeamStats(context);

            var statA = stats.First(s => s.TeamName == "Команда А");
            var statB = stats.First(s => s.TeamName == "Команда Б");

            Assert.Equal(1, statA.Points);
            Assert.Equal(1, statB.Points);
        }
    }
}