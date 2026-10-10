using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace VegetablesApp
{
    class Program
    {
        static ConnectionStringSettings currentSettings;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ChooseDatabase();
            if (currentSettings == null) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Овощи и фрукты");
                Console.WriteLine("Активное подключение: " + currentSettings.Name +
                                  " (" + currentSettings.ProviderName + ")");
                Console.WriteLine();
                Console.WriteLine("1. Показать овощи");
                Console.WriteLine("2. Показать фрукты");
                Console.WriteLine("3. Показать всё");
                Console.WriteLine("4. Обновить количество");
                Console.WriteLine("5. Добавить запись");
                Console.WriteLine("6. Удалить запись");
                Console.WriteLine("7. Сменить СУБД");
                Console.WriteLine("0. Выход");
                Console.Write("\n> ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1": await ShowTableAsync("Vegetables"); break;
                        case "2": await ShowTableAsync("Fruits"); break;
                        case "3":
                            await ShowTableAsync("Vegetables");
                            await ShowTableAsync("Fruits");
                            break;
                        case "4": await UpdateQuantityAsync(); break;
                        case "5": await InsertRowAsync(); break;
                        case "6": await DeleteRowAsync(); break;
                        case "7": ChooseDatabase(); break;
                        case "0": return;
                        default: Console.WriteLine("Нет такого пункта."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ОШИБКА: " + ex.Message);
                }

                Console.WriteLine("\nНажмите Enter...");
                Console.ReadLine();
            }
        }

        static void ChooseDatabase()
        {
            Console.Clear();
            Console.WriteLine("Выбор СУБД");
            Console.WriteLine();

            var list = new System.Collections.Generic.List<ConnectionStringSettings>();
            foreach (ConnectionStringSettings s in ConfigurationManager.ConnectionStrings)
            {
                if (s.Name == "LocalSqlServer") continue;
                list.Add(s);
            }

            if (list.Count == 0)
            {
                Console.WriteLine("В App.config нет строк подключения.");
                return;
            }

            for (int i = 0; i < list.Count; i++)
                Console.WriteLine("  " + (i + 1) + ". " + list[i].Name +
                                  "  [" + list[i].ProviderName + "]");

            Console.Write("\n> ");
            if (!int.TryParse(Console.ReadLine(), out int n) ||
                n < 1 || n > list.Count)
            {
                Console.WriteLine("Некорректный выбор.");
                return;
            }

            currentSettings = list[n - 1];
            Console.WriteLine("\nВыбрано: " + currentSettings.Name);
        }

        static async Task ShowTableAsync(string tableName)
        {
            if (currentSettings == null) { Console.WriteLine("Подключение не выбрано."); return; }

            DbProviderFactory factory = DbProviderFactories.GetFactory(currentSettings.ProviderName);

            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = currentSettings.ConnectionString;

                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM " + tableName;

                    await conn.OpenAsync();

                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        Console.WriteLine();
                        Console.WriteLine(currentSettings.Name + " :: " + tableName);
                        Console.WriteLine();

                        for (int i = 0; i < reader.FieldCount; i++)
                            Console.Write(reader.GetName(i).PadRight(15) + "| ");
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 17 * reader.FieldCount));

                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                object v = reader.GetValue(i);
                                Console.Write((v?.ToString() ?? "").PadRight(15) + "| ");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        static async Task UpdateQuantityAsync()
        {
            if (currentSettings == null) return;

            Console.Write("Таблица (1 = Vegetables, 2 = Fruits): ");
            string table = Console.ReadLine().Trim() == "1" ? "Vegetables" : "Fruits";

            Console.Write("Id записи: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Некорректный Id."); return; }

            Console.Write("Новое количество: ");
            if (!int.TryParse(Console.ReadLine(), out int qty)) { Console.WriteLine("Некорректное количество."); return; }

            DbProviderFactory factory = DbProviderFactories.GetFactory(currentSettings.ProviderName);

            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = currentSettings.ConnectionString;

                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE " + table + " SET Quantity = " + Ph("@qty") +
                                      " WHERE Id = " + Ph("@id");

                    AddParam(cmd, factory, "@qty", qty);
                    AddParam(cmd, factory, "@id", id);

                    await conn.OpenAsync();
                    int affected = await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine(affected > 0
                        ? "\nОбновлено строк: " + affected
                        : "\nЗапись с Id = " + id + " не найдена.");
                }
            }
        }

        static async Task InsertRowAsync()
        {
            if (currentSettings == null) return;

            Console.Write("Таблица (1 = Vegetables, 2 = Fruits): ");
            string table = Console.ReadLine().Trim() == "1" ? "Vegetables" : "Fruits";

            Console.Write("Название: ");
            string name = Console.ReadLine();

            Console.Write("Цвет: ");
            string color = Console.ReadLine();

            Console.Write("Калорийность: ");
            if (!int.TryParse(Console.ReadLine(), out int calories)) { Console.WriteLine("Некорректно."); return; }

            Console.Write("Количество: ");
            if (!int.TryParse(Console.ReadLine(), out int qty)) { Console.WriteLine("Некорректно."); return; }

            DbProviderFactory factory = DbProviderFactories.GetFactory(currentSettings.ProviderName);

            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = currentSettings.ConnectionString;

                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO " + table +
                        " (Name, Color, Calories, Quantity) VALUES (" +
                        Ph("@name") + ", " + Ph("@color") + ", " + Ph("@cal") + ", " + Ph("@qty") + ")";

                    AddParam(cmd, factory, "@name", name);
                    AddParam(cmd, factory, "@color", color);
                    AddParam(cmd, factory, "@cal", calories);
                    AddParam(cmd, factory, "@qty", qty);

                    await conn.OpenAsync();
                    int affected = await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine("\nДобавлено строк: " + affected);
                }
            }
        }

        static async Task DeleteRowAsync()
        {
            if (currentSettings == null) return;

            Console.Write("Таблица (1 = Vegetables, 2 = Fruits): ");
            string table = Console.ReadLine().Trim() == "1" ? "Vegetables" : "Fruits";

            Console.Write("Id для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Некорректный Id."); return; }

            Console.Write("Подтвердить? (y/n): ");
            string c = Console.ReadLine().Trim().ToLower();
            if (c != "y" && c != "yes" && c != "д" && c != "да") { Console.WriteLine("Отменено."); return; }

            DbProviderFactory factory = DbProviderFactories.GetFactory(currentSettings.ProviderName);

            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = currentSettings.ConnectionString;

                using (DbCommand cmd = factory.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM " + table + " WHERE Id = " + Ph("@id");

                    AddParam(cmd, factory, "@id", id);

                    await conn.OpenAsync();
                    int affected = await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine(affected > 0
                        ? "\nУдалено строк: " + affected
                        : "\nЗапись с Id = " + id + " не найдена.");
                }
            }
        }

        static string Ph(string name)
        {
            if (currentSettings != null &&
                currentSettings.ProviderName == "System.Data.OleDb")
                return "?";
            return name;
        }

        static void AddParam(DbCommand cmd, DbProviderFactory factory, string name, object value)
        {
            DbParameter p = factory.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }
    }
}