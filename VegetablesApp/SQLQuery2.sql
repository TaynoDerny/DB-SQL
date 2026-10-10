CREATE DATABASE VegetablesAndFruits2;
GO

USE VegetablesAndFruits2;
GO

CREATE TABLE Vegetables (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50),
    Color NVARCHAR(30),
    Calories INT,
    Quantity INT
);
GO

CREATE TABLE Fruits (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50),
    Color NVARCHAR(30),
    Calories INT,
    Quantity INT
);
GO

INSERT INTO Vegetables (Name, Color, Calories, Quantity) VALUES
(N'Морковь',   N'Оранжевый',  41, 150),
(N'Картофель', N'Коричневый', 77, 300);

INSERT INTO Fruits (Name, Color, Calories, Quantity) VALUES
(N'Яблоко', N'Красный', 52, 180),
(N'Банан',  N'Жёлтый',  89, 140);
GO