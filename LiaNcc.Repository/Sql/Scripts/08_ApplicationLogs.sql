USE [LiaNcc]
GO

IF OBJECT_ID(N'[dbo].[ApplicationLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApplicationLogs (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ApplicationLogs PRIMARY KEY DEFAULT NEWID(),
        Source NVARCHAR(100) NOT NULL,
        Area NVARCHAR(100) NULL,
        Action NVARCHAR(150) NULL,
        Level NVARCHAR(50) NOT NULL,
        Message NVARCHAR(MAX) NOT NULL,
        ExceptionMessage NVARCHAR(MAX) NULL,
        StackTrace NVARCHAR(MAX) NULL,
        InnerException NVARCHAR(MAX) NULL,
        UserId NVARCHAR(100) NULL,
        UserEmail NVARCHAR(256) NULL,
        RequestPath NVARCHAR(500) NULL,
        QueryString NVARCHAR(1000) NULL,
        HttpMethod NVARCHAR(20) NULL,
        StatusCode INT NULL,
        EntityName NVARCHAR(100) NULL,
        EntityId UNIQUEIDENTIFIER NULL,
        IpAddress NVARCHAR(100) NULL,
        UserAgent NVARCHAR(1000) NULL,
        CorrelationId NVARCHAR(100) NULL,
        AdditionalData NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_ApplicationLogs_CreatedAt DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_ApplicationLogs_CreatedAt ON dbo.ApplicationLogs(CreatedAt);
    CREATE INDEX IX_ApplicationLogs_Level ON dbo.ApplicationLogs(Level);
    CREATE INDEX IX_ApplicationLogs_Source ON dbo.ApplicationLogs(Source);
    CREATE INDEX IX_ApplicationLogs_Area ON dbo.ApplicationLogs(Area);
    CREATE INDEX IX_ApplicationLogs_CorrelationId ON dbo.ApplicationLogs(CorrelationId);
END
GO
