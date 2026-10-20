-- 1. Удаляем старую базу данных (если она есть)
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'CountriesDB')
BEGIN
    ALTER DATABASE CountriesDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CountriesDB;
END
GO

-- 2. Создаем базу заново
CREATE DATABASE CountriesDB;
GO

USE CountriesDB;
GO

-- 3. Создаем таблицы
CREATE TABLE Countries (
    CountryID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Population INT NOT NULL,
    Area FLOAT NOT NULL,
    PartOfWorld NVARCHAR(100) NOT NULL
);

CREATE TABLE Capitals (
    CapitalID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Population INT NOT NULL,
    CountryID INT FOREIGN KEY REFERENCES Countries(CountryID)
);

CREATE TABLE Cities (
    CityID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Population INT NOT NULL,
    CountryID INT FOREIGN KEY REFERENCES Countries(CountryID)
);

-- 4. Заполняем таблицы правильными данными
-- Обратите внимание на букву N перед одинарными кавычками: N'Россия'. 
-- Она НЕ сохранится в базе, а просто говорит SQL, что это русский текст.

INSERT INTO Countries (Name, Population, Area, PartOfWorld) VALUES 
(N'Россия', 146000000, 17098246, N'Европа/Азия'),
(N'Франция', 67390000, 551695, N'Европа'),
(N'Германия', 83240000, 357022, N'Европа'),
(N'Китай', 1402112000, 9596961, N'Азия'),
(N'Египет', 100000000, 1002450, N'Африка'),
(N'США', 331000000, 9833517, N'Северная Америка'),
(N'Бразилия', 211000000, 8515767, N'Южная Америка');

INSERT INTO Capitals (Name, Population, CountryID) VALUES 
(N'Москва', 12600000, 1),
(N'Париж', 2100000, 2),
(N'Берлин', 3600000, 3),
(N'Пекин', 21500000, 4),
(N'Каир', 9500000, 5),
(N'Вашингтон', 700000, 6),
(N'Бразилиа', 3000000, 7);

INSERT INTO Cities (Name, Population, CountryID) VALUES 
(N'Санкт-Петербург', 5600000, 1),
(N'Новосибирск', 1600000, 1),
(N'Марсель', 870000, 2),
(N'Лион', 520000, 2),
(N'Гамбург', 1800000, 3),
(N'Мюнхен', 1500000, 3),
(N'Шанхай', 24800000, 4),
(N'Гуанчжоу', 15300000, 4),
(N'Александрия', 5200000, 5),
(N'Гиза', 4300000, 5),
(N'Нью-Йорк', 8400000, 6),
(N'Лос-Анджелес', 3900000, 6),
(N'Сан-Паулу', 12300000, 7),
(N'Рио-де-Жанейро', 6700000, 7);
GO