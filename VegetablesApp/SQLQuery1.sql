USE VegetablesAndFruits;
GO

DELETE FROM Vegetables;
DELETE FROM Fruits;
GO

DBCC CHECKIDENT ('Vegetables', RESEED, 0);
DBCC CHECKIDENT ('Fruits', RESEED, 0);
GO

INSERT INTO Vegetables (Name, Color, Calories, Quantity) VALUES
(N'Морковь',   N'Оранжевый',  41, 150),
(N'Картофель', N'Коричневый', 77, 300),
(N'Огурец',    N'Зелёный',    15, 120),
(N'Помидор',   N'Красный',    20, 90),
(N'Капуста',   N'Зелёный',    25, 200);
GO

INSERT INTO Fruits (Name, Color, Calories, Quantity) VALUES
(N'Яблоко',   N'Красный',    52, 180),
(N'Банан',    N'Жёлтый',     89, 140),
(N'Апельсин', N'Оранжевый',  47, 110),
(N'Груша',    N'Зелёный',    57, 75),
(N'Виноград', N'Фиолетовый', 69, 60);
GO

SELECT * FROM Vegetables;
SELECT * FROM Fruits;