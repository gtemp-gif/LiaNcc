-- Script per ottimizzazione tabella LocalizedContents
-- Aggiunta indici per migliorare le performance delle query di localizzazione

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_LocalizedContents_Entity'
    AND object_id = OBJECT_ID('dbo.LocalizedContents')
)
BEGIN
    CREATE INDEX IX_LocalizedContents_Entity
    ON dbo.LocalizedContents(EntityName, EntityId, LanguageCode, ContentKey);
END
GO

-- Indice univoco per prevenire duplicati della stessa chiave di traduzione
-- Nota: da eseguire solo se non ci sono duplicati esistenti.
-- CREATE UNIQUE INDEX UX_LocalizedContents_Key
-- ON dbo.LocalizedContents(EntityName, EntityId, LanguageCode, ContentKey);
-- GO

PRINT 'Indici per LocalizedContents creati con successo.';
