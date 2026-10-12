using System;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StudentGradesApp
{
    class Program
    {
        static string currentProvider = "System.Data.SqlClient";
        static string currentConnectionString = ConfigurationManager.ConnectionStrings["SqlServerDB"].ConnectionString;

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Текущая СУБД: {currentProvider} ===");
                Console.WriteLine("1. Показать оценки (Задание 3)");
                Console.WriteLine("2. Обновить оценку (Задание 4)");
                Console.WriteLine("3. Удалить оценку (Задание 5)");
                Console.WriteLine("4. Сменить СУБД (Задание 6)");
                Console.WriteLine("0. Выход");
                Console.Write("Выбери действие: ");

                string choice = Console.ReadLine();
                if (choice == "0") break;

                // Задание 7: Запускаем секундомер
                Stopwatch sw = Stopwatch.StartNew();

                try
                {
                    switch (choice)
                    {
                        case "1": await ReadGradesAsync(); break;
                        case "2": await UpdateGradeAsync(); break;
                        case "3": await DeleteGradeAsync(); break;
                        case "4": SwitchDbms(); break;
                        default: Console.WriteLine("Неверный выбор."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nОшибка: {ex.Message}");
                }

                sw.Stop();
                // Задание 7: Вывод времени
                Console.WriteLine($"\n[Время выполнения: {sw.Elapsed.TotalSeconds:F4} сек.]");
                Console.WriteLine("Нажми Enter для продолжения...");
                Console.ReadLine();
            }
        }

        // Задание 3: Асинхронное чтение
        static async Task ReadGradesAsync()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(currentProvider);
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = currentConnectionString;
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT s.LastName, s.FirstName, sub.SubjectName, g.GradeValue 
                        FROM Grades g 
                        JOIN Students s ON g.StudentId = s.StudentId 
                        JOIN Subjects sub ON g.SubjectId = sub.SubjectId
                        ORDER BY s.LastName";

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("\nОценки студентов:");
                        Console.WriteLine("----------------------------------------");
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine($"{reader["LastName"]} {reader["FirstName"]} | {reader["SubjectName"]}: {reader["GradeValue"]}");
                        }
                    }
                }
            }
        }

        // Задание 4: Асинхронное обновление
        static async Task UpdateGradeAsync()
        {
            Console.Write("Введи ID оценки для изменения: ");
            int gradeId = int.Parse(Console.ReadLine());
            Console.Write("Введи новую оценку (1-5): ");
            int newValue = int.Parse(Console.ReadLine());

            DbProviderFactory factory = DbProviderFactories.GetFactory(currentProvider);
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = currentConnectionString;
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Grades SET GradeValue = @val WHERE GradeId = @id";

                    DbParameter pVal = factory.CreateParameter();
                    pVal.ParameterName = "@val";
                    pVal.Value = newValue;
                    command.Parameters.Add(pVal);

                    DbParameter pId = factory.CreateParameter();
                    pId.ParameterName = "@id";
                    pId.Value = gradeId;
                    command.Parameters.Add(pId);

                    int rows = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"\nОбновлено строк: {rows}");
                }
            }
        }

        // Задание 5: Асинхронное удаление
        static async Task DeleteGradeAsync()
        {
            Console.Write("Введи ID оценки для удаления: ");
            int gradeId = int.Parse(Console.ReadLine());

            DbProviderFactory factory = DbProviderFactories.GetFactory(currentProvider);
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = currentConnectionString;
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Grades WHERE GradeId = @id";

                    DbParameter pId = factory.CreateParameter();
                    pId.ParameterName = "@id";
                    pId.Value = gradeId;
                    command.Parameters.Add(pId);

                    int rows = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"\nУдалено строк: {rows}");
                }
            }
        }

        // Задание 6: Смена СУБД
        static void SwitchDbms()
        {
            Console.WriteLine("\nВыбери СУБД:");
            Console.WriteLine("1. SQL Server");
            Console.WriteLine("2. SQLite (требует установки NuGet-пакета System.Data.SQLite)");
            Console.Write("Выбор: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                currentProvider = "System.Data.SqlClient";
                currentConnectionString = ConfigurationManager.ConnectionStrings["SqlServerDB"].ConnectionString;
                Console.WriteLine("Переключено на SQL Server.");
            }
            else if (choice == "2")
            {
                currentProvider = "System.Data.SQLite";
                currentConnectionString = ConfigurationManager.ConnectionStrings["SqliteDB"].ConnectionString;
                Console.WriteLine("Переключено на SQLite. Внимание: структура таблиц в файле StudentGrades.db должна совпадать!");
            }
        }
    }
}