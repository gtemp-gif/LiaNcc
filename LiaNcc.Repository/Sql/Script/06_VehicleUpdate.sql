-- Script per aggiornamento gestione flotta (Veicoli, Categorie, Features, Gallery)

-- 1. Verifica/Aggiornamento Vehicles
IF COL_LENGTH('dbo.Vehicles', 'Title') IS NULL
BEGIN
    ALTER TABLE dbo.Vehicles ADD Title NVARCHAR(150) NULL;
END
GO

IF COL_LENGTH('dbo.Vehicles', 'Description') IS NULL
BEGIN
    ALTER TABLE dbo.Vehicles ADD Description NVARCHAR(MAX) NULL;
END
GO

IF COL_LENGTH('dbo.Vehicles', 'Seats') IS NULL
BEGIN
    -- Se SeatsCount esisteva e Seats no, rinominiamo o aggiungiamo
    IF COL_LENGTH('dbo.Vehicles', 'SeatsCount') IS NOT NULL
    BEGIN
        EXEC sp_rename 'dbo.Vehicles.SeatsCount', 'Seats', 'COLUMN';
    END
    ELSE
    BEGIN
        ALTER TABLE dbo.Vehicles ADD Seats INT NOT NULL DEFAULT 0;
    END
END
GO

IF COL_LENGTH('dbo.Vehicles', 'Luggages') IS NULL
BEGIN
    IF COL_LENGTH('dbo.Vehicles', 'LuggageCount') IS NOT NULL
    BEGIN
        EXEC sp_rename 'dbo.Vehicles.LuggageCount', 'Luggages', 'COLUMN';
    END
    ELSE
    BEGIN
        ALTER TABLE dbo.Vehicles ADD Luggages INT NOT NULL DEFAULT 0;
    END
END
GO

IF COL_LENGTH('dbo.Vehicles', 'IsBookable') IS NULL
BEGIN
    ALTER TABLE dbo.Vehicles ADD IsBookable BIT NOT NULL DEFAULT 1;
END
GO

IF COL_LENGTH('dbo.Vehicles', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Vehicles ADD UpdatedAt DATETIME2 NULL;
END
GO

-- 2. Verifica/Aggiornamento VehicleCategories
IF COL_LENGTH('dbo.VehicleCategories', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.VehicleCategories ADD UpdatedAt DATETIME2 NULL;
END
GO

-- 3. VehicleFeatures (Dovrebbe già essere a posto da 01_CreateDatabase.sql se presente)
-- ma assicuriamoci dei vincoli se necessario.

-- 4. VehicleGalleryImages (Se non usiamo EntityMedia)
-- In questo progetto stiamo usando EntityMedia per la gallery, quindi non creiamo la tabella dedicata
-- a meno che non si voglia passare a tabella dedicata.
-- Dato che FilesController salva in MediaAssets e associa tramite EntityMedia, continuiamo così.

PRINT 'Aggiornamento Database Flotta completato con successo.';
