-- Script idempotente per l'aggiunta dei nuovi campi sulle tabelle esistenti

-- --------------------------------------------------
-- 1. Tabella Services
-- Aggiungiamo CoverImageUrl e CoverImageMediaId
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Services') AND name = 'CoverImageUrl')
BEGIN
    ALTER TABLE Services ADD CoverImageUrl NVARCHAR(1000) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Services') AND name = 'CoverImageMediaId')
BEGIN
    ALTER TABLE Services ADD CoverImageMediaId UNIQUEIDENTIFIER NULL;
END

-- --------------------------------------------------
-- 2. Tabella Vehicles
-- Aggiungiamo Title, IsBookable, Description
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Vehicles') AND name = 'Title')
BEGIN
    ALTER TABLE Vehicles ADD Title NVARCHAR(150) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Vehicles') AND name = 'IsBookable')
BEGIN
    ALTER TABLE Vehicles ADD IsBookable BIT NOT NULL DEFAULT 1;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Vehicles') AND name = 'Description')
BEGIN
    ALTER TABLE Vehicles ADD Description NVARCHAR(MAX) NULL;
END

-- --------------------------------------------------
-- 3. Tabella Tours
-- Aggiungiamo CoverImageUrl, Description, HeroTitle, HeroSubtitle,
-- ExperienceDescription, ExperienceImageUrl, DurationDays,
-- DurationHours, MeetingPoint, VehicleId
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'CoverImageUrl')
BEGIN
    ALTER TABLE Tours ADD CoverImageUrl NVARCHAR(1000) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'Description')
BEGIN
    ALTER TABLE Tours ADD Description NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'HeroTitle')
BEGIN
    ALTER TABLE Tours ADD HeroTitle NVARCHAR(200) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'HeroSubtitle')
BEGIN
    ALTER TABLE Tours ADD HeroSubtitle NVARCHAR(300) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'ExperienceDescription')
BEGIN
    ALTER TABLE Tours ADD ExperienceDescription NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'ExperienceImageUrl')
BEGIN
    ALTER TABLE Tours ADD ExperienceImageUrl NVARCHAR(1000) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'DurationDays')
BEGIN
    ALTER TABLE Tours ADD DurationDays INT NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'DurationHours')
BEGIN
    ALTER TABLE Tours ADD DurationHours INT NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'MeetingPoint')
BEGIN
    ALTER TABLE Tours ADD MeetingPoint NVARCHAR(300) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Tours') AND name = 'VehicleId')
BEGIN
    ALTER TABLE Tours ADD VehicleId UNIQUEIDENTIFIER NULL;
    -- Aggiungiamo Foreign Key verso Vehicles, se possibile e logico
    ALTER TABLE Tours ADD CONSTRAINT FK_Tours_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE SET NULL;
END

-- --------------------------------------------------
-- 4. Tabella Bookings
-- Aggiungiamo MaxSeats
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'MaxSeats')
BEGIN
    ALTER TABLE Bookings ADD MaxSeats INT NULL;
END

-- --------------------------------------------------
-- 5. Tabella Partners (Solo check/creazione, LogoUrl potrebbe esserci già)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Partners') AND name = 'LogoUrl')
BEGIN
    ALTER TABLE Partners ADD LogoUrl NVARCHAR(1000) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Partners') AND name = 'LogoMediaId')
BEGIN
    ALTER TABLE Partners ADD LogoMediaId UNIQUEIDENTIFIER NULL;
END
