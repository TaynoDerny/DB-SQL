using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;

public class Student
{
    public string Name { get; set; }
    public string Group { get; set; }
    public int Age { get; set; }
    public double GPA { get; set; }
    public int PassedCredits { get; set; }

    public override string ToString()
    {
        return $"{Name,-10} | {Group,-6} | {Age,2} | {GPA,4:F1} | {PassedCredits,2}";
    }
}

class Program
{
    private static List<Student> GetStudents()
    {
        return new List<Student>
        {
            new Student { Name = "Алексей", Group = "ИТ-41", Age = 20, GPA = 4.2, PassedCredits = 40 },
            new Student { Name = "Борис", Group = "ИТ-41", Age = 19, GPA = 3.1, PassedCredits = 35 },
            new Student { Name = "Виктория", Group = "ПИ-42", Age = 21, GPA = 4.8, PassedCredits = 45 },
            new Student { Name = "Григорий", Group = "ИБ-43", Age = 20, GPA = 2.9, PassedCredits = 20 },
            new Student { Name = "Дарья", Group = "ИТ-41", Age = 22, GPA = 4.5, PassedCredits = 42 },
            new Student { Name = "Елена", Group = "ПИ-42", Age = 20, GPA = 3.8, PassedCredits = 38 },
            new Student { Name = "Жанна", Group = "ИБ-43", Age = 23, GPA = 4.9, PassedCredits = 48 },
            new Student { Name = "Игорь", Group = "ИТ-41", Age = 19, GPA = 2.5, PassedCredits = 15 }
        };
    }

    // Задача 1
    private static void Task1_QuerySyntax()
    {
        List<Student> students = GetStudents();

        var result = from s in students
                     where s.Group == "ИТ-41" && s.GPA > 3.5
                     orderby s.Age ascending
                     select s;

        Console.WriteLine("--- Задание 1: Студенты ИТ-41 с баллом > 3.5 ---");
        Console.WriteLine("Имя | Группа | Возраст | Балл | Зачеты");

        foreach (var student in result)
        {
            Console.WriteLine(student.ToString());
        }
    }

    // Задача 2
    private static void Task2_MethodSyntax()
    {
        List<Student> students = GetStudents();

        var result = students
            .Where(s => s.GPA >= 4.0)
            .OrderByDescending(s => s.GPA)
            .Select(s => new
            {
                FullName = s.Name,
                Scholarship = s.GPA >= 4.7 ? "Ректорская" :
                              s.GPA >= 4.5 ? "Повышенная" :
                              s.GPA >= 4.0 ? "Стандартная" : "Нет"
            });

        Console.WriteLine("--- Задание 2: Стипендии (синтаксис методов) ---");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.FullName}: {item.Scholarship}");
        }
    }

    // Задача 3
    private static void Task3_Agreement()
    {
        List<Student> students = GetStudents();

        var groups = students
            .GroupBy(s => s.Group)
            .Select(g => new
            {
                GroupName = g.Key,
                AvgGPA = g.Average(s => s.GPA)
            })
            .OrderBy(g => g.GroupName);

        Console.WriteLine("--- Задание 3: Статистика ---");
        foreach (var group in groups)
        {
            Console.WriteLine($"Группа {group.GroupName}: Средний балл {group.AvgGPA:F2}");
        }

        var oldestWith45 = students
            .Where(s => s.PassedCredits == 45)
            .OrderByDescending(s => s.Age)
            .FirstOrDefault();

        if (oldestWith45 != null)
        {
            Console.WriteLine($"\nСтудент с 45 зачетами: {oldestWith45.Name} ({oldestWith45.Age} год)");
        }
        else
        {
            Console.WriteLine("\nСтудент с 45 зачетами не найден.");
        }
    }

    // Задача 4
    private static void Task4_Xml()
    {
        XDocument xmlDoc = new XDocument(
            new XElement("university",
                new XElement("course", new XAttribute("name", "Программирование"),
                    new XElement("student", new XAttribute("name", "Кирилл"), new XAttribute("gra", "3.9")),
                    new XElement("student", new XAttribute("name", "Лариса"), new XAttribute("gra", "4.7")),
                    new XElement("student", new XAttribute("name", "Михаил"), new XAttribute("gra", "3.2")),
                    new XElement("student", new XAttribute("name", "Наталья"), new XAttribute("gra", "4.1"))
                )
            )
        );
        xmlDoc.Save("uni.xml");

        XDocument loadedDoc = XDocument.Load("uni.xml");

        var excellentStudents = loadedDoc
            .Descendants("student")
            .Where(s => double.Parse(s.Attribute("gra").Value, CultureInfo.InvariantCulture) > 4.0)
            .Select(s => s.Attribute("name").Value);

        Console.WriteLine("--- Задание 4: Отличники из XML ---");
        foreach (var name in excellentStudents)
        {
            Console.WriteLine(name);
        }
    }

    static void Main(string[] args)
    {
        Task1_QuerySyntax();
        Console.WriteLine();

        Task2_MethodSyntax();
        Console.WriteLine();

        Task3_Agreement();
        Console.WriteLine();

        Task4_Xml();

        Console.WriteLine("\nНажмите Enter для выхода...");
        Console.ReadLine();
    }
}