/*
    Script idempotente per la creazione della tabella EmailMessages.
*/

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailMessages')
BEGIN
    CREATE TABLE [EmailMessages] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [RelatedEntityName] NVARCHAR(100) NULL,
        [RelatedEntityId] UNIQUEIDENTIFIER NULL,
        [ToEmail] NVARCHAR(256) NOT NULL,
        [FromEmail] NVARCHAR(256) NULL,
        [Subject] NVARCHAR(300) NOT NULL,
        [Body] NVARCHAR(MAX) NULL,
        [IsHtml] BIT NOT NULL DEFAULT 1,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [SentAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX [IX_EmailMessages_CreatedAt] ON [EmailMessages] ([CreatedAt] DESC);
    CREATE INDEX [IX_EmailMessages_Status] ON [EmailMessages] ([Status]);
    CREATE INDEX [IX_EmailMessages_RelatedEntity] ON [EmailMessages] ([RelatedEntityName], [RelatedEntityId]);
END
GO
