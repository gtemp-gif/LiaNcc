/*
    Script di migrazione per la tabella ApplicationLogs.
    Obiettivo: Aggiornare la tabella alla nuova struttura mantenendo i dati esistenti.
*/

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApplicationLogs')
BEGIN
    CREATE TABLE [ApplicationLogs] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [Timestamp] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [Level] NVARCHAR(50) NOT NULL,
        [ProjectName] NVARCHAR(150) NOT NULL,
        [Area] NVARCHAR(150) NULL,
        [Controller] NVARCHAR(150) NULL,
        [Action] NVARCHAR(150) NULL,
        [UserId] NVARCHAR(100) NULL,
        [UserName] NVARCHAR(256) NULL,
        [TenantId] UNIQUEIDENTIFIER NULL,
        [EntityName] NVARCHAR(150) NULL,
        [EntityId] NVARCHAR(100) NULL,
        [EventType] NVARCHAR(100) NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Exception] NVARCHAR(MAX) NULL,
        [StackTrace] NVARCHAR(MAX) NULL,
        [InnerException] NVARCHAR(MAX) NULL,
        [RequestPath] NVARCHAR(500) NULL,
        [HttpMethod] NVARCHAR(20) NULL,
        [StatusCode] INT NULL,
        [IpAddress] NVARCHAR(100) NULL,
        [UserAgent] NVARCHAR(1000) NULL,
        [CorrelationId] NVARCHAR(100) NULL,
        [AdditionalDataJson] NVARCHAR(MAX) NULL,
        [QueryString] NVARCHAR(MAX) NULL
    );
END
ELSE
BEGIN
    DECLARE @IdType NVARCHAR(128);
    SELECT @IdType = TYPE_NAME(system_type_id) FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'Id';

    IF @IdType = 'uniqueidentifier'
    BEGIN
        EXEC sp_rename 'ApplicationLogs', 'ApplicationLogs_Old';

        CREATE TABLE [ApplicationLogs] (
            [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
            [Timestamp] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
            [Level] NVARCHAR(50) NOT NULL,
            [ProjectName] NVARCHAR(150) NOT NULL,
            [Area] NVARCHAR(150) NULL,
            [Controller] NVARCHAR(150) NULL,
            [Action] NVARCHAR(150) NULL,
            [UserId] NVARCHAR(100) NULL,
            [UserName] NVARCHAR(256) NULL,
            [TenantId] UNIQUEIDENTIFIER NULL,
            [EntityName] NVARCHAR(150) NULL,
            [EntityId] NVARCHAR(100) NULL,
            [EventType] NVARCHAR(100) NULL,
            [Message] NVARCHAR(MAX) NOT NULL,
            [Exception] NVARCHAR(MAX) NULL,
            [StackTrace] NVARCHAR(MAX) NULL,
            [InnerException] NVARCHAR(MAX) NULL,
            [RequestPath] NVARCHAR(500) NULL,
            [HttpMethod] NVARCHAR(20) NULL,
            [StatusCode] INT NULL,
            [IpAddress] NVARCHAR(100) NULL,
            [UserAgent] NVARCHAR(1000) NULL,
            [CorrelationId] NVARCHAR(100) NULL,
            [AdditionalDataJson] NVARCHAR(MAX) NULL,
            [QueryString] NVARCHAR(MAX) NULL
        );

        INSERT INTO [ApplicationLogs] (
            [Timestamp], [Level], [ProjectName], [Area], [Action], [UserId], [UserName],
            [EntityName], [EntityId], [Message], [Exception], [StackTrace],
            [RequestPath], [HttpMethod], [StatusCode], [IpAddress], [UserAgent],
            [CorrelationId], [AdditionalDataJson]
        )
        SELECT
            COALESCE([CreatedAt], SYSUTCDATETIME()), [Level], COALESCE([Source], 'Unknown'), [Area], [Action], [UserId], [UserEmail],
            [EntityName], CAST([EntityId] AS NVARCHAR(100)), [Message], [ExceptionMessage], [StackTrace],
            [RequestPath], [HttpMethod], [StatusCode], [IpAddress], [UserAgent],
            [CorrelationId], [AdditionalData]
        FROM [ApplicationLogs_Old];
    END
    ELSE
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'Controller')
            ALTER TABLE [ApplicationLogs] ADD [Controller] NVARCHAR(150) NULL;

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'UserName')
            ALTER TABLE [ApplicationLogs] ADD [UserName] NVARCHAR(256) NULL;

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'TenantId')
            ALTER TABLE [ApplicationLogs] ADD [TenantId] UNIQUEIDENTIFIER NULL;

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'EventType')
            ALTER TABLE [ApplicationLogs] ADD [EventType] NVARCHAR(100) NULL;

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'InnerException')
            ALTER TABLE [ApplicationLogs] ADD [InnerException] NVARCHAR(MAX) NULL;

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'AdditionalDataJson')
            ALTER TABLE [ApplicationLogs] ADD [AdditionalDataJson] NVARCHAR(MAX) NULL;

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationLogs') AND name = 'QueryString')
            ALTER TABLE [ApplicationLogs] ADD [QueryString] NVARCHAR(MAX) NULL;
    END
END

-- Indici idempotenti
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_Timestamp' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_Timestamp] ON [ApplicationLogs] ([Timestamp] DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_Level' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_Level] ON [ApplicationLogs] ([Level]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_ProjectName' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_ProjectName] ON [ApplicationLogs] ([ProjectName]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_Area' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_Area] ON [ApplicationLogs] ([Area]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_EventType' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_EventType] ON [ApplicationLogs] ([EventType]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_CorrelationId' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_CorrelationId] ON [ApplicationLogs] ([CorrelationId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationLogs_StatusCode' AND object_id = OBJECT_ID('ApplicationLogs'))
    CREATE INDEX [IX_ApplicationLogs_StatusCode] ON [ApplicationLogs] ([StatusCode]);
