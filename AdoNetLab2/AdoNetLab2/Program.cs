using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AdoNetLab2
{
    class Program
    {
        static string connString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Task1_ManualDataTable();
            Task2_FillDataSet();
            Task3_AutoUpdate();
            Task4_CustomUpdate();
            Task5_RowStateOrder();
            Task6_TableMappings();

            Console.WriteLine("\nГотово. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static void PrintTable(DataTable table, string title = "")
        {
            if (!string.IsNullOrEmpty(title))
                Console.WriteLine($"\n  {title} ");

            foreach (DataColumn col in table.Columns)
                Console.Write(col.ColumnName.PadRight(15));
            Console.WriteLine();
            Console.WriteLine(new string('-', table.Columns.Count * 15));

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    object val = row[col];
                    string s = val == DBNull.Value ? "NULL" : val.ToString();
                    Console.Write(s.PadRight(15));
                }
                Console.WriteLine();
            }
        }

        // Задача 1
        static void Task1_ManualDataTable()
        {
            DataTable table = new DataTable("Authors");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("LastName", typeof(string));

            DataRow row1 = table.NewRow();
            row1["Id"] = 1;
            row1["FirstName"] = "Isaac";
            row1["LastName"] = "Asimov";
            table.Rows.Add(row1);

            table.Rows.Add(2, "Ray", "Bradbury");
            table.Rows.Add(3, "Clifford", "Simak");
            table.Rows.Add(4, "Roger", "Zelazny");

            PrintTable(table, "Authors (manual)");

            Console.WriteLine($"\nЧтение row[0]: {table.Rows[0][0]}");
            Console.WriteLine($"Чтение row[\"FirstName\"]: {table.Rows[0]["FirstName"]}");
        }

        // Задача 2
        static void Task2_FillDataSet()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
                DataSet ds = new DataSet();

                da.Fill(ds, "Books");

                PrintTable(ds.Tables["Books"], "Books (DataSet)");
            }
        }

        // Задача 3
        static void Task3_AutoUpdate()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds, "Books");

                Console.WriteLine("\n Задача 3: ");
                Console.WriteLine("До изменения:");
                Console.WriteLine($"Цена первой книги: {ds.Tables["Books"].Rows[0]["Price"]}");

                ds.Tables["Books"].Rows[0]["Price"] = 999;

                Console.WriteLine("После локального изменения:");
                Console.WriteLine($"Цена первой книги: {ds.Tables["Books"].Rows[0]["Price"]}");

                da.Update(ds, "Books");
                Console.WriteLine("Update выполнен.");

                Console.WriteLine("\nСгенерированные команды:");
                Console.WriteLine("INSERT: " + cmdBuilder.GetInsertCommand().CommandText);
                Console.WriteLine("UPDATE: " + cmdBuilder.GetUpdateCommand().CommandText);
                Console.WriteLine("DELETE: " + cmdBuilder.GetDeleteCommand().CommandText);

                DataSet dsCheck = new DataSet();
                da.Fill(dsCheck, "Books");
                PrintTable(dsCheck.Tables["Books"], "Books после Update");
            }
        }

        // Задача 4
        static void Task4_CustomUpdate()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "Books");

                SqlCommand updateCmd = new SqlCommand(
                    "UPDATE Books SET Price = @pPrice WHERE Id = @pId", conn);

                updateCmd.Parameters.Add(new SqlParameter("@pPrice", SqlDbType.Int));
                updateCmd.Parameters["@pPrice"].SourceVersion = DataRowVersion.Current;
                updateCmd.Parameters["@pPrice"].SourceColumn = "Price";

                updateCmd.Parameters.Add(new SqlParameter("@pId", SqlDbType.Int));
                updateCmd.Parameters["@pId"].SourceVersion = DataRowVersion.Original;
                updateCmd.Parameters["@pId"].SourceColumn = "Id";

                da.UpdateCommand = updateCmd;

                ds.Tables["Books"].Rows[0]["Price"] = 1500;
                ds.Tables["Books"].Rows[1]["Price"] = 2000;

                Console.WriteLine("\n Задача 4: ");
                Console.WriteLine("Локальные изменения: Book 1 = 1500, Book 2 = 2000");

                da.Update(ds, "Books");
                Console.WriteLine("Update выполнен.");

                DataSet dsCheck = new DataSet();
                da.Fill(dsCheck, "Books");
                PrintTable(dsCheck.Tables["Books"], "Books после Custom Update");
            }
        }

        // Задача 5
        static void Task5_RowStateOrder()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", conn);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
                DataSet ds = new DataSet();
                da.Fill(ds, "Books");
                DataTable books = ds.Tables["Books"];

                Console.WriteLine("\n Задача 5: ");

                books.Rows[0].Delete();
                books.Rows[1]["Price"] = 777;

                DataRow newRow = books.NewRow();
                newRow["Author"] = "New Author";
                newRow["Title"] = "New Book";
                newRow["Price"] = 500;
                newRow["Pages"] = 100;
                books.Rows.Add(newRow);

                Console.WriteLine("Состояния строк:");
                foreach (DataRow row in books.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                        Console.WriteLine($"  {row["Title"],-20} -> {row.RowState}");
                    else
                        Console.WriteLine($"  [Удалённая строка] -> {row.RowState}");
                }

                da.Update(books.Select(null, null, DataViewRowState.Deleted));
                da.Update(books.Select(null, null, DataViewRowState.ModifiedCurrent));
                da.Update(books.Select(null, null, DataViewRowState.Added));

                Console.WriteLine("Update выполнен в правильном порядке.");

                DataSet dsCheck = new DataSet();
                da.Fill(dsCheck, "Books");
                PrintTable(dsCheck.Tables["Books"], "Books после Task 5");
            }
        }

        // Задача 6
        static void Task6_TableMappings()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = "SELECT * FROM Authors; SELECT * FROM Books;";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();

                da.Fill(ds);
                Console.WriteLine("\n Задача 6: ");
                Console.WriteLine("Без маппинга:");
                foreach (DataTable t in ds.Tables)
                    Console.WriteLine($"Таблица: {t.TableName}");

                ds.Clear();


                DataTableMapping dtm = da.TableMappings.Add("Table", "Authors");
                dtm.ColumnMappings.Add("Id", "Num");
                dtm.ColumnMappings.Add("FirstName", "Name");
                dtm.ColumnMappings.Add("LastName", "Surname");


                da.TableMappings.Add("Table1", "Books");

                da.Fill(ds);
                Console.WriteLine("\nС маппингом:");
                foreach (DataTable t in ds.Tables)
                    Console.WriteLine($"Таблица: {t.TableName}");

                PrintTable(ds.Tables["Authors"], "Authors (с ColumnMappings)");
                PrintTable(ds.Tables["Books"], "Books (с TableMappings)");
            }
        }
    }
}