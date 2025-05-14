CREATE TABLE [dbo].[WeatherData] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Temperature] DECIMAL(18,2) NULL,
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