USE DeviceManagementDb;
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'test@user.com')
BEGIN
    INSERT INTO Users (Email, PasswordHash, FullName, Role, Location)
    VALUES ('test@user.com', 'hashedpassword', 'Test User', 'Employee', 'London');
END
GO

IF NOT EXISTS (SELECT 1 FROM Devices WHERE Name = 'iPhone 14')
BEGIN
    INSERT INTO Devices (Name, Manufacturer, Type, OperatingSystem, OsVersion, Processor, RamAmount, Description, Location)
    VALUES ('iPhone 14', 'Apple', 'phone', 'iOS', '16', 'A15', '6GB', 'Apple smartphone with A15 chip, 6GB RAM, iOS 16.', 'London');
END
GO