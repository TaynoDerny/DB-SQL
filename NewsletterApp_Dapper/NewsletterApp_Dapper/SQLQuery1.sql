CREATE DATABASE NewsletterDB;
GO
USE NewsletterDB;
GO

CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    BirthDate DATE NOT NULL,
    Gender NVARCHAR(10),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Country NVARCHAR(50),
    City NVARCHAR(50)
);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE CustomerCategories (
    CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    PRIMARY KEY (CustomerId, CategoryId)
);

CREATE TABLE PromoItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    Country NVARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL
);

-- ВОТ ЗДЕСЬ ДОБАВЛЕНА БУКВА N ПЕРЕД КАЖДОЙ СТРОКОЙ С РУССКИМ ТЕКСТОМ
INSERT INTO Categories (Name) VALUES 
(N'Мобильные телефоны'), 
(N'Наушники'), 
(N'Кухонная техника');

INSERT INTO Customers (FullName, BirthDate, Gender, Email, Country, City) VALUES 
(N'Иванов Иван', '1990-05-15', N'М', 'ivan@mail.ru', N'Россия', N'Москва'),
(N'Петрова Анна', '1985-11-20', N'Ж', 'anna@mail.ru', N'Россия', N'Санкт-Петербург'),
('Smith John', '1992-01-10', N'М', 'john@mail.com', N'США', N'Нью-Йорк');

INSERT INTO CustomerCategories (CustomerId, CategoryId) VALUES 
(1, 1), 
(1, 2), 
(2, 3), 
(3, 1);

INSERT INTO PromoItems (Name, CategoryId, Country, StartDate, EndDate) VALUES 
(N'Скидка 20% на iPhone', 1, N'Россия', '2026-10-01', '2026-11-01'),
(N'Black Friday наушники', 2, N'США', '2026-11-20', '2026-11-30');