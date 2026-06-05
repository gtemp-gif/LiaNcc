/*
    Script di creazione/verifica per la tabella EmailMessages.
    Obiettivo: Assicurarsi che la tabella esista e sia allineata al model C#.
*/

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailMessages')
BEGIN
    CREATE TABLE [EmailMessages] (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [ToEmail] NVARCHAR(256) NOT NULL,
        [FromEmail] NVARCHAR(256) NULL,
        [Subject] NVARCHAR(300) NOT NULL,
        [Body] NVARCHAR(MAX) NOT NULL,
        [IsHtml] BIT NOT NULL DEFAULT 1,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [SentAt] DATETIME2 NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [RelatedEntityName] NVARCHAR(100) NULL,
        [RelatedEntityId] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
ELSE
BEGIN
    -- Verifica colonne mancanti
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmailMessages') AND name = 'ErrorMessage')
        ALTER TABLE [EmailMessages] ADD [ErrorMessage] NVARCHAR(MAX) NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmailMessages') AND name = 'RelatedEntityName')
        ALTER TABLE [EmailMessages] ADD [RelatedEntityName] NVARCHAR(100) NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmailMessages') AND name = 'RelatedEntityId')
        ALTER TABLE [EmailMessages] ADD [RelatedEntityId] UNIQUEIDENTIFIER NULL;
END

-- Indici
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailMessages_CreatedAt' AND object_id = OBJECT_ID('EmailMessages'))
    CREATE INDEX [IX_EmailMessages_CreatedAt] ON [EmailMessages] ([CreatedAt] DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailMessages_Status' AND object_id = OBJECT_ID('EmailMessages'))
    CREATE INDEX [IX_EmailMessages_Status] ON [EmailMessages] ([Status]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmailMessages_RelatedEntity' AND object_id = OBJECT_ID('EmailMessages'))
    CREATE INDEX [IX_EmailMessages_RelatedEntity] ON [EmailMessages] ([RelatedEntityName], [RelatedEntityId]);
