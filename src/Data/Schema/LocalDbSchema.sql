IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ORBIT9000Local')

BEGIN
    CREATE DATABASE [ORBIT9000Local];
END

DECLARE @sql NVARCHAR(MAX) = N'USE [ORBIT9000Local];';
EXEC(@sql);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'WeatherData')
BEGIN
    CREATE TABLE [dbo].[WeatherData] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Temperature] DECIMAL(18,6) NULL,
        [City] NVARCHAR(255) NULL,
        [Longitude] REAL NULL,
        [Lattitude] REAL NULL,
        [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
        [CreatedOn] DATETIME2 NOT NULL,
        [ModifiedBy] UNIQUEIDENTIFIER NOT NULL,
        [ModifiedOn] DATETIME2 NULL,
        [DeletedBy] UNIQUEIDENTIFIER NOT NULL,
        [DeletedOn] DATETIME2 NULL
    );
END

