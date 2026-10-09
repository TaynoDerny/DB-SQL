using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WarehouseApp
{
    class Program
    {
        // Строка подключения
        static string connectionString = ConfigurationManager.ConnectionStrings["Warehouse"].ConnectionString;

        static SqlConnection connection = new SqlConnection(connectionString);

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n===== МЕНЮ =====");
                Console.WriteLine("Состояние подключения: " + connection.State);
                Console.WriteLine("  1  - Подключиться к базе");
                Console.WriteLine("  2  - Отключиться от базы");
                Console.WriteLine("----------------------------");
                Console.WriteLine("ЗАДАНИЕ 3:");
                Console.WriteLine("  3  - Вся информация о товарах");
                Console.WriteLine("  4  - Все типы товаров");
                Console.WriteLine("  5  - Все поставщики");
                Console.WriteLine("  6  - Товар с максимальным количеством");
                Console.WriteLine("  7  - Товар с минимальным количеством");
                Console.WriteLine("  8  - Товар с минимальной себестоимостью");
                Console.WriteLine("  9  - Товар с максимальной себестоимостью");
                Console.WriteLine("ЗАДАНИЕ 4:");
                Console.WriteLine("  10 - Товары заданной категории");
                Console.WriteLine("  11 - Товары заданного поставщика");
                Console.WriteLine("  12 - Самый старый товар на складе");
                Console.WriteLine("  13 - Среднее количество по каждому типу");
                Console.WriteLine("  0  - Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": Connect(); break;
                    case "2": Disconnect(); break;

                    case "3": if (EnsureConnected()) ShowAllProducts(); break;
                    case "4": if (EnsureConnected()) ShowAllTypes(); break;
                    case "5": if (EnsureConnected()) ShowAllSuppliers(); break;
                    case "6": if (EnsureConnected()) ShowMaxQuantity(); break;
                    case "7": if (EnsureConnected()) ShowMinQuantity(); break;
                    case "8": if (EnsureConnected()) ShowMinCost(); break;
                    case "9": if (EnsureConnected()) ShowMaxCost(); break;
                    case "10": if (EnsureConnected()) ShowProductsByCategory(); break;
                    case "11": if (EnsureConnected()) ShowProductsBySupplier(); break;
                    case "12": if (EnsureConnected()) ShowOldestProduct(); break;
                    case "13": if (EnsureConnected()) ShowAvgQuantityByType(); break;

                    case "0":
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        return;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
            }
        }

        // ЗАДАНИЕ 2

        static void Connect()
        {
            if (connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Уже подключено.");
                return;
            }

            try
            {
                connection.Open();
                Console.WriteLine("Подключение к базе 'Warehouse' успешно установлено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения!");
                Console.WriteLine("Причина: " + ex.Message);
            }
        }

        // ЗАДАНИЕ 2
        static void Disconnect()
        {
            if (connection.State == ConnectionState.Closed)
            {
                Console.WriteLine("Соединение и так закрыто.");
                return;
            }

            try
            {
                connection.Close();
                Console.WriteLine("Отключение от базы выполнено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отключении: " + ex.Message);
            }
        }

        // Проверка
        static bool EnsureConnected()
        {
            if (connection.State != ConnectionState.Open)
            {
                Console.WriteLine("Нет подключения к базе. Сначала выберите пункт 1 - Подключиться.");
                return false;
            }
            return true;
        }


        // ЗАДАНИЕ 3.1

        static void ShowAllProducts()
        {
            ExecuteAndPrint("SELECT * FROM Products", "ВСЕ ТОВАРЫ");
        }

        // ЗАДАНИЕ 3.2
        static void ShowAllTypes()
        {
            ExecuteAndPrint("SELECT * FROM ProductTypes", "ВСЕ ТИПЫ ТОВАРОВ");
        }

        // ЗАДАНИЕ 3.3
        static void ShowAllSuppliers()
        {
            ExecuteAndPrint("SELECT * FROM Suppliers", "ВСЕ ПОСТАВЩИКИ");
        }

        // ЗАДАНИЕ 3.4
        static void ShowMaxQuantity()
        {
            ExecuteAndPrint("SELECT TOP 1 * FROM Products ORDER BY Quantity DESC",
                "ТОВАР С МАКСИМАЛЬНЫМ КОЛИЧЕСТВОМ");
        }

        // ЗАДАНИЕ 3.5
        static void ShowMinQuantity()
        {
            ExecuteAndPrint("SELECT TOP 1 * FROM Products ORDER BY Quantity ASC",
                "ТОВАР С МИНИМАЛЬНЫМ КОЛИЧЕСТВОМ");
        }

        // ЗАДАНИЕ 3.6
        static void ShowMinCost()
        {
            ExecuteAndPrint("SELECT TOP 1 * FROM Products ORDER BY Cost ASC",
                "ТОВАР С МИНИМАЛЬНОЙ СЕБЕСТОИМОСТЬЮ");
        }

        // ЗАДАНИЕ 3.7
        static void ShowMaxCost()
        {
            ExecuteAndPrint("SELECT TOP 1 * FROM Products ORDER BY Cost DESC",
                "ТОВАР С МАКСИМАЛЬНОЙ СЕБЕСТОИМОСТЬЮ");
        }


        // ЗАДАНИЕ 4.1

        static void ShowProductsByCategory()
        {
            Console.Write("Введите ID категории (1-Electronics, 2-Stationery, 3-Furniture): ");
            int typeId = int.Parse(Console.ReadLine());

            string query = @"SELECT p.ProductID, p.ProductName, t.TypeName, p.Quantity, p.Cost
                             FROM Products p
                             JOIN ProductTypes t ON p.TypeID = t.TypeID
                             WHERE p.TypeID = @TypeID";
            ExecuteAndPrint(query, "ТОВАРЫ КАТЕГОРИИ ID=" + typeId,
                new SqlParameter("@TypeID", typeId));
        }


        // ЗАДАНИЕ 4.2

        static void ShowProductsBySupplier()
        {
            Console.Write("Введите ID поставщика (1, 2 или 3): ");
            int supplierId = int.Parse(Console.ReadLine());

            string query = @"SELECT p.ProductID, p.ProductName, s.SupplierName, p.Quantity, p.Cost
                             FROM Products p
                             JOIN Suppliers s ON p.SupplierID = s.SupplierID
                             WHERE p.SupplierID = @SupplierID";
            ExecuteAndPrint(query, "ТОВАРЫ ПОСТАВЩИКА ID=" + supplierId,
                new SqlParameter("@SupplierID", supplierId));
        }


        // ЗАДАНИЕ 4.3

        static void ShowOldestProduct()
        {
            ExecuteAndPrint("SELECT TOP 1 * FROM Products ORDER BY DeliveryDate ASC",
                "САМЫЙ СТАРЫЙ ТОВАР");
        }

        // ЗАДАНИЕ 4.4

        static void ShowAvgQuantityByType()
        {
            string query = @"SELECT t.TypeName, AVG(p.Quantity) AS AvgQuantity
                             FROM Products p
                             JOIN ProductTypes t ON p.TypeID = t.TypeID
                             GROUP BY t.TypeName";
            ExecuteAndPrint(query, "СРЕДНЕЕ КОЛИЧЕСТВО ПО ТИПАМ");
        }

        static void ExecuteAndPrint(string query, string title, params SqlParameter[] parameters)
        {
            Console.WriteLine("--- " + title + " ---");

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Заголовки
                        for (int i = 0; i < reader.FieldCount; i++)
                            Console.Write(reader.GetName(i).PadRight(20));
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 20 * reader.FieldCount));

                        // Строки
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                                Console.Write(reader.GetValue(i).ToString().PadRight(20));
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка запроса: " + ex.Message);
            }
        }
    }
}