-- Script per aggiornamento gestione Tours (Cover, Experience, VehicleId)

IF COL_LENGTH('dbo.Tours', 'CoverImageUrl') IS NULL
BEGIN
    ALTER TABLE dbo.Tours ADD CoverImageUrl NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Tours', 'ExperienceImageUrl') IS NULL
BEGIN
    ALTER TABLE dbo.Tours ADD ExperienceImageUrl NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Tours', 'VehicleId') IS NULL
BEGIN
    ALTER TABLE dbo.Tours ADD VehicleId UNIQUEIDENTIFIER NULL;
END
GO

-- Aggiunta FK verso Vehicles se non esiste
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tours_Vehicles' AND parent_object_id = OBJECT_ID('dbo.Tours'))
BEGIN
    ALTER TABLE dbo.Tours
    ADD CONSTRAINT FK_Tours_Vehicles
    FOREIGN KEY (VehicleId) REFERENCES dbo.Vehicles(Id);
END
GO

PRINT 'Aggiornamento Database Tours completato con successo.';
