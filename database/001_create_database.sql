IF DB_ID('DeviceManagementDb') IS NULL
BEGIN
    CREATE DATABASE DeviceManagementDb;
END
GO

USE DeviceManagementDb;
GO

IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        FullName NVARCHAR(255) NOT NULL,
        Role NVARCHAR(100) NOT NULL,
        Location NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

IF OBJECT_ID('Devices', 'U') IS NULL
BEGIN
    CREATE TABLE Devices (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Manufacturer NVARCHAR(255) NOT NULL,
        Type NVARCHAR(50) NOT NULL,
        OperatingSystem NVARCHAR(100) NOT NULL,
        OsVersion NVARCHAR(100) NOT NULL,
        Processor NVARCHAR(100) NOT NULL,
        RamAmount NVARCHAR(50) NOT NULL,
        Description NVARCHAR(MAX),
        Location NVARCHAR(255) NOT NULL,
        AssignedUserId INT NULL,
        FOREIGN KEY (AssignedUserId) REFERENCES Users(Id)
    );
END
GO