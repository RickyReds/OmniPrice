-- Script per creare il database locale per WebApiPrezzi
-- Eseguire questo script in SQL Server Management Studio

-- 1. Creazione Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PrezziDB')
BEGIN
    CREATE DATABASE PrezziDB;
END
GO

USE PrezziDB;
GO

-- 2. Tabella principale per le regole di prezzo
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PriceRules')
BEGIN
    CREATE TABLE PriceRules (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerNo VARCHAR(50) NOT NULL,
        ProductCategory VARCHAR(100),
        BasePrice DECIMAL(10,2) NOT NULL DEFAULT 0,
        DiscountPercentage DECIMAL(5,2) DEFAULT 0,
        ValidFrom DATETIME DEFAULT GETDATE(),
        ValidTo DATETIME,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME
    );
END
GO

-- 3. Tabella per i fattori di calcolo prezzo
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PriceFactors')
BEGIN
    CREATE TABLE PriceFactors (
        Id INT PRIMARY KEY IDENTITY(1,1),
        FactorName VARCHAR(100) NOT NULL,
        FactorType VARCHAR(50), -- 'Dimensioni', 'Cliente', 'Materiale', etc.
        MultiplierValue DECIMAL(10,4) DEFAULT 1.0,
        AdditiveValue DECIMAL(10,2) DEFAULT 0,
        IsActive BIT DEFAULT 1
    );
END
GO

-- 4. Tabella per storico prezzi calcolati
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PriceHistory')
BEGIN
    CREATE TABLE PriceHistory (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerNo VARCHAR(50) NOT NULL,
        ItemCode VARCHAR(100),
        CalculatedPrice DECIMAL(10,2) NOT NULL,
        RequestData NVARCHAR(MAX), -- JSON del ConfigItem
        CalculationDate DATETIME DEFAULT GETDATE(),
        IPAddress VARCHAR(50)
    );
END
GO

-- 5. Inserimento dati di esempio
-- Regole prezzo per clienti
INSERT INTO PriceRules (CustomerNo, ProductCategory, BasePrice, DiscountPercentage)
VALUES 
    ('CUST001', 'VASCHE', 1000.00, 10),
    ('CUST001', 'LAVABI', 500.00, 5),
    ('TEST123', 'VASCHE', 800.00, 0),
    ('PREMIUM001', 'LAVABI', 1500.00, 15);

-- Fattori di prezzo
INSERT INTO PriceFactors (FactorName, FactorType, MultiplierValue, AdditiveValue)
VALUES 
    ('Contributo Dimensioni', 'Dimensioni', 1.0, 0),
    ('Contributo Cliente', 'Cliente', 1.0, 0),
    ('Contributo XY', 'Altro', 1.0, 0);

-- 6. Stored Procedure per calcolare il prezzo
GO
CREATE OR ALTER PROCEDURE sp_CalculatePrice
    @CustomerNo VARCHAR(50),
    @ProductCategory VARCHAR(100) = NULL,
    @BasePrice DECIMAL(10,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Ottieni il prezzo base per il cliente
    SELECT TOP 1 
        @BasePrice = BasePrice * (1 - DiscountPercentage/100.0)
    FROM PriceRules
    WHERE CustomerNo = @CustomerNo
        AND IsActive = 1
        AND (ProductCategory = @ProductCategory OR ProductCategory IS NULL)
        AND (ValidFrom <= GETDATE() OR ValidFrom IS NULL)
        AND (ValidTo >= GETDATE() OR ValidTo IS NULL)
    ORDER BY 
        CASE WHEN ProductCategory = @ProductCategory THEN 0 ELSE 1 END,
        ValidFrom DESC;
    
    -- Se non trovato, usa prezzo default
    IF @BasePrice IS NULL
        SET @BasePrice = 100.00;
END
GO

-- 7. Vista per monitoraggio prezzi
CREATE OR ALTER VIEW vw_PriceMonitor
AS
SELECT 
    ph.Id,
    ph.CustomerNo,
    pr.ProductCategory,
    ph.CalculatedPrice,
    ph.CalculationDate,
    pr.BasePrice as ConfiguredBasePrice,
    pr.DiscountPercentage
FROM PriceHistory ph
LEFT JOIN PriceRules pr ON ph.CustomerNo = pr.CustomerNo
WHERE ph.CalculationDate >= DATEADD(day, -30, GETDATE());
GO

PRINT 'Database PrezziDB creato con successo!';
PRINT 'Connection String da usare: Data Source=localhost\SQLEXPRESS;Initial Catalog=PrezziDB;Integrated Security=True;';