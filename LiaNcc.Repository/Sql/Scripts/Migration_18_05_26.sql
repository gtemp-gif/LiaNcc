-- Migration 18-05-2026:
/* Localization
Languages
- Id UNIQUEIDENTIFIER PK
- Code NVARCHAR(10) -- it, en, fr
- Name NVARCHAR(100)
- IsDefault BIT
- IsActive BIT

LocalizedContents
- Id UNIQUEIDENTIFIER PK
- EntityName NVARCHAR(100) -- Tours, Vehicles, Services, SitePages
- EntityId UNIQUEIDENTIFIER
- LanguageId UNIQUEIDENTIFIER FK
- FieldName NVARCHAR(100) -- Title, Description, ButtonText, ecc.
- FieldValue NVARCHAR(MAX)

*/

/*
Media / immagini

MediaAssets
- Id UNIQUEIDENTIFIER PK
- Url NVARCHAR(1000)
- AltText NVARCHAR(300)
- Title NVARCHAR(200)
- MediaType NVARCHAR(50) -- Image, Video
- IsExternal BIT
- CreatedAt DATETIME2

EntityMedia
- Id UNIQUEIDENTIFIER PK
- EntityName NVARCHAR(100)
- EntityId UNIQUEIDENTIFIER
- MediaAssetId UNIQUEIDENTIFIER FK
- UsageType NVARCHAR(100) -- Hero, Gallery, Card, Thumbnail
- SortOrder INT

*/

/*
Services
- Id UNIQUEIDENTIFIER PK
- Name NVARCHAR(150)
- ShortDescription NVARCHAR(500)
- FullDescription NVARCHAR(MAX)
- BasePrice DECIMAL(18,2) NULL
- PriceLabel NVARCHAR(100) -- "da 50€"
- ImageUrl NVARCHAR(500)
- ServiceType NVARCHAR(100)
- IsBookable BIT
- IsActive BIT
- SortOrder INT


Esempi dai file:

1. Transfer aeroportuali
2. Navetta eventi
3. Tour personalizzati
4. Transfer privati
5. Collaborazioni hotel

*/

/*
FLOTTA 

VehicleCategories
- Id UNIQUEIDENTIFIER PK
- Name NVARCHAR(100)
- Description NVARCHAR(500)
- SortOrder INT

Vehicles -- da ampliare
- Id
- VehicleCategoryId UNIQUEIDENTIFIER FK
- Name
- Description
- Category -- eventualmente eliminabile se usi VehicleCategoryId
- ImageUrl
- Seats
- Luggages
- IsFeatured BIT
- SortOrder INT
- IsActive


VehicleFeatures
- Id UNIQUEIDENTIFIER PK
- VehicleId UNIQUEIDENTIFIER FK
- Icon NVARCHAR(100) -- person, luggage, wifi
- Name NVARCHAR(150)
- Value NVARCHAR(150)
- SortOrder INT

Serve per elementi come:

3 passeggeri
2 valigie grandi
Chauffeur professionista
Wi-Fi a bordo
Layout conferenza

*/

/*
TourCategories
- Id UNIQUEIDENTIFIER PK
- Name NVARCHAR(150)
- Description NVARCHAR(500)
- SortOrder INT

Tours -- da ampliare
- Id
- TourCategoryId UNIQUEIDENTIFIER NULL FK
- Slug NVARCHAR(150)
- Name
- Subtitle NVARCHAR(300)
- ShortDescription
- FullDescription
- ImageUrl
- HeroImageUrl NVARCHAR(500)
- Price
- PriceLabel NVARCHAR(100)
- Duration NVARCHAR(100)
- MeetingPoint NVARCHAR(300)
- VehicleInfo NVARCHAR(300)
- Region NVARCHAR(100)
- IsFeatured BIT
- IsActive
- SortOrder INT

*/

/*
Dettagli e galleria tour

TourSections
- Id UNIQUEIDENTIFIER PK
- TourId UNIQUEIDENTIFIER FK
- SectionKey NVARCHAR(100) -- experience, details, gallery
- Title NVARCHAR(200)
- Subtitle NVARCHAR(300)
- Body NVARCHAR(MAX)
- ImageUrl NVARCHAR(500)
- SortOrder INT

TourGalleryImages
- Id UNIQUEIDENTIFIER PK
- TourId UNIQUEIDENTIFIER FK
- ImageUrl NVARCHAR(500)
- AltText NVARCHAR(300)
- Caption NVARCHAR(200)
- SortOrder INT


TourInfoItems
- Id UNIQUEIDENTIFIER PK
- TourId UNIQUEIDENTIFIER FK
- Icon NVARCHAR(100)
- Label NVARCHAR(150)
- Value NVARCHAR(300)
- SortOrder INT


*/


/*
PRENOTAZIONI

Bookings -- da ampliare
- Id
- FullName
- Email
- Phone NVARCHAR(50) NULL
- ServiceDate
- ServiceType
- ServiceId UNIQUEIDENTIFIER NULL
- TourId UNIQUEIDENTIFIER NULL
- VehicleId UNIQUEIDENTIFIER NULL
- PassengersRange NVARCHAR(100) NULL
- Message
- Status
- SourcePage NVARCHAR(100) -- Home, Fleet, TourDetail
- CreatedAt
- UpdatedAt

Tipologie servizio / opzioni form
BookingServiceTypes
- Id UNIQUEIDENTIFIER PK
- Name NVARCHAR(150)
- Code NVARCHAR(100)
- IsActive BIT
- SortOrder INT


Esempi:

Transfer Aeroportuale
Tour Personalizzato
Evento Speciale
Altro


BookingPassengerOptions
- Id UNIQUEIDENTIFIER PK
- Label NVARCHAR(100)
- MinPassengers INT NULL
- MaxPassengers INT NULL
- IsPrivate BIT
- SortOrder INT

*/

/*
Contatti
ContactMessages -- da ampliare
- Id
- FullName
- Email
- Phone NVARCHAR(50) NULL
- Subject NVARCHAR(200) NULL
- Message
- SourcePage NVARCHAR(100)
- IsRead
- CreatedAt

CompanyProfile
- Id UNIQUEIDENTIFIER PK
- CompanyName NVARCHAR(200)
- LegalName NVARCHAR(200) NULL
- Description NVARCHAR(MAX)
- Address NVARCHAR(300)
- City NVARCHAR(100)
- Province NVARCHAR(50)
- PostalCode NVARCHAR(20)
- Country NVARCHAR(100)
- Email NVARCHAR(256)
- PhonePrimary NVARCHAR(50)
- PhoneSecondary NVARCHAR(50)
- MapEmbedUrl NVARCHAR(MAX) NULL
- Latitude DECIMAL(10,7) NULL
- Longitude DECIMAL(10,7) NULL


CompanyContacts
- Id UNIQUEIDENTIFIER PK
- CompanyProfileId UNIQUEIDENTIFIER FK
- ContactType NVARCHAR(50) -- Phone, Email, WhatsApp
- Label NVARCHAR(100)
- Value NVARCHAR(300)
- Icon NVARCHAR(100)
- SortOrder INT

*/

/*
Partner / hotel

Partners
- Id UNIQUEIDENTIFIER PK
- Name NVARCHAR(150)
- LogoUrl NVARCHAR(500) NULL
- WebsiteUrl NVARCHAR(500) NULL
- PartnerType NVARCHAR(100) -- Hotel, Agency, Corporate
- IsActive BIT
- SortOrder INT
*/

USE [LiaNcc]
GO

/* ============================================================
   LANGUAGES & LOCALIZATION
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Languages')
BEGIN
    CREATE TABLE Languages (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Languages PRIMARY KEY DEFAULT NEWID(),
        Code NVARCHAR(10) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        IsDefault BIT NOT NULL CONSTRAINT DF_Languages_IsDefault DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_Languages_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Languages_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_Languages_Code ON Languages(Code);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LocalizedContents')
BEGIN
    CREATE TABLE LocalizedContents (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_LocalizedContents PRIMARY KEY DEFAULT NEWID(),
        EntityName NVARCHAR(100) NOT NULL,
        EntityId UNIQUEIDENTIFIER NOT NULL,
        LanguageId UNIQUEIDENTIFIER NOT NULL,
        FieldName NVARCHAR(100) NOT NULL,
        FieldValue NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_LocalizedContents_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_LocalizedContents_Languages FOREIGN KEY (LanguageId)
            REFERENCES Languages(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_LocalizedContents_Entity 
        ON LocalizedContents(EntityName, EntityId);

    CREATE UNIQUE INDEX IX_LocalizedContents_UniqueField
        ON LocalizedContents(EntityName, EntityId, LanguageId, FieldName);
END
GO

/* ============================================================
   MEDIA
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MediaAssets')
BEGIN
    CREATE TABLE MediaAssets (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_MediaAssets PRIMARY KEY DEFAULT NEWID(),
        Url NVARCHAR(1000) NOT NULL,
        AltText NVARCHAR(300) NULL,
        Title NVARCHAR(200) NULL,
        MediaType NVARCHAR(50) NOT NULL CONSTRAINT DF_MediaAssets_MediaType DEFAULT 'Image',
        IsExternal BIT NOT NULL CONSTRAINT DF_MediaAssets_IsExternal DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_MediaAssets_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_MediaAssets_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntityMedia')
BEGIN
    CREATE TABLE EntityMedia (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_EntityMedia PRIMARY KEY DEFAULT NEWID(),
        EntityName NVARCHAR(100) NOT NULL,
        EntityId UNIQUEIDENTIFIER NOT NULL,
        MediaAssetId UNIQUEIDENTIFIER NOT NULL,
        UsageType NVARCHAR(100) NOT NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_EntityMedia_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_EntityMedia_CreatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT FK_EntityMedia_MediaAssets FOREIGN KEY (MediaAssetId)
            REFERENCES MediaAssets(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_EntityMedia_Entity 
        ON EntityMedia(EntityName, EntityId);

    CREATE INDEX IX_EntityMedia_MediaAssetId 
        ON EntityMedia(MediaAssetId);
END
GO

/* ============================================================
   SERVICES
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Services')
BEGIN
    CREATE TABLE Services (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Services PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(150) NOT NULL,
        ShortDescription NVARCHAR(500) NULL,
        FullDescription NVARCHAR(MAX) NULL,
        BasePrice DECIMAL(18,2) NULL,
        PriceLabel NVARCHAR(100) NULL,
        ImageUrl NVARCHAR(500) NULL,
        ServiceType NVARCHAR(100) NULL,
        IsBookable BIT NOT NULL CONSTRAINT DF_Services_IsBookable DEFAULT 1,
        IsFeatured BIT NOT NULL CONSTRAINT DF_Services_IsFeatured DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_Services_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_Services_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Services_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

/* ============================================================
   VEHICLES
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VehicleCategories')
BEGIN
    CREATE TABLE VehicleCategories (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_VehicleCategories PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_VehicleCategories_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_VehicleCategories_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_VehicleCategories_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_VehicleCategories_Name ON VehicleCategories(Name);
END
GO

IF COL_LENGTH('Vehicles', 'VehicleCategoryId') IS NULL
BEGIN
    ALTER TABLE Vehicles ADD VehicleCategoryId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Vehicles', 'IsFeatured') IS NULL
BEGIN
    ALTER TABLE Vehicles ADD IsFeatured BIT NOT NULL 
        CONSTRAINT DF_Vehicles_IsFeatured DEFAULT 0;
END
GO

IF COL_LENGTH('Vehicles', 'SortOrder') IS NULL
BEGIN
    ALTER TABLE Vehicles ADD SortOrder INT NOT NULL 
        CONSTRAINT DF_Vehicles_SortOrder DEFAULT 0;
END
GO

IF NOT EXISTS (
    SELECT * 
    FROM sys.foreign_keys 
    WHERE name = 'FK_Vehicles_VehicleCategories'
)
BEGIN
    ALTER TABLE Vehicles
    ADD CONSTRAINT FK_Vehicles_VehicleCategories
    FOREIGN KEY (VehicleCategoryId)
    REFERENCES VehicleCategories(Id)
    ON DELETE SET NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VehicleFeatures')
BEGIN
    CREATE TABLE VehicleFeatures (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_VehicleFeatures PRIMARY KEY DEFAULT NEWID(),
        VehicleId UNIQUEIDENTIFIER NOT NULL,
        Icon NVARCHAR(100) NULL,
        Name NVARCHAR(150) NOT NULL,
        Value NVARCHAR(150) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_VehicleFeatures_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_VehicleFeatures_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_VehicleFeatures_Vehicles FOREIGN KEY (VehicleId)
            REFERENCES Vehicles(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_VehicleFeatures_VehicleId ON VehicleFeatures(VehicleId);
END
GO

/* ============================================================
   TOURS
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TourCategories')
BEGIN
    CREATE TABLE TourCategories (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_TourCategories PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(150) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_TourCategories_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_TourCategories_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_TourCategories_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_TourCategories_Name ON TourCategories(Name);
END
GO

IF COL_LENGTH('Tours', 'TourCategoryId') IS NULL
BEGIN
    ALTER TABLE Tours ADD TourCategoryId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Tours', 'Slug') IS NULL
BEGIN
    ALTER TABLE Tours ADD Slug NVARCHAR(150) NULL;
END
GO

IF COL_LENGTH('Tours', 'Subtitle') IS NULL
BEGIN
    ALTER TABLE Tours ADD Subtitle NVARCHAR(300) NULL;
END
GO

IF COL_LENGTH('Tours', 'HeroImageUrl') IS NULL
BEGIN
    ALTER TABLE Tours ADD HeroImageUrl NVARCHAR(500) NULL;
END
GO

IF COL_LENGTH('Tours', 'PriceLabel') IS NULL
BEGIN
    ALTER TABLE Tours ADD PriceLabel NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('Tours', 'Duration') IS NULL
BEGIN
    ALTER TABLE Tours ADD Duration NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('Tours', 'MeetingPoint') IS NULL
BEGIN
    ALTER TABLE Tours ADD MeetingPoint NVARCHAR(300) NULL;
END
GO

IF COL_LENGTH('Tours', 'VehicleInfo') IS NULL
BEGIN
    ALTER TABLE Tours ADD VehicleInfo NVARCHAR(300) NULL;
END
GO

IF COL_LENGTH('Tours', 'Region') IS NULL
BEGIN
    ALTER TABLE Tours ADD Region NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('Tours', 'IsFeatured') IS NULL
BEGIN
    ALTER TABLE Tours ADD IsFeatured BIT NOT NULL 
        CONSTRAINT DF_Tours_IsFeatured DEFAULT 0;
END
GO

IF COL_LENGTH('Tours', 'SortOrder') IS NULL
BEGIN
    ALTER TABLE Tours ADD SortOrder INT NOT NULL 
        CONSTRAINT DF_Tours_SortOrder DEFAULT 0;
END
GO

IF NOT EXISTS (
    SELECT * 
    FROM sys.foreign_keys 
    WHERE name = 'FK_Tours_TourCategories'
)
BEGIN
    ALTER TABLE Tours
    ADD CONSTRAINT FK_Tours_TourCategories
    FOREIGN KEY (TourCategoryId)
    REFERENCES TourCategories(Id)
    ON DELETE SET NULL;
END
GO

IF NOT EXISTS (
    SELECT * 
    FROM sys.indexes 
    WHERE name = 'IX_Tours_Slug'
)
BEGIN
    CREATE UNIQUE INDEX IX_Tours_Slug 
    ON Tours(Slug)
    WHERE Slug IS NOT NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TourSections')
BEGIN
    CREATE TABLE TourSections (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_TourSections PRIMARY KEY DEFAULT NEWID(),
        TourId UNIQUEIDENTIFIER NOT NULL,
        SectionKey NVARCHAR(100) NOT NULL,
        Title NVARCHAR(200) NULL,
        Subtitle NVARCHAR(300) NULL,
        Body NVARCHAR(MAX) NULL,
        ImageUrl NVARCHAR(500) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_TourSections_SortOrder DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_TourSections_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_TourSections_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_TourSections_Tours FOREIGN KEY (TourId)
            REFERENCES Tours(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_TourSections_TourId ON TourSections(TourId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TourGalleryImages')
BEGIN
    CREATE TABLE TourGalleryImages (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_TourGalleryImages PRIMARY KEY DEFAULT NEWID(),
        TourId UNIQUEIDENTIFIER NOT NULL,
        ImageUrl NVARCHAR(500) NOT NULL,
        AltText NVARCHAR(300) NULL,
        Caption NVARCHAR(200) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_TourGalleryImages_SortOrder DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_TourGalleryImages_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_TourGalleryImages_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_TourGalleryImages_Tours FOREIGN KEY (TourId)
            REFERENCES Tours(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_TourGalleryImages_TourId ON TourGalleryImages(TourId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TourInfoItems')
BEGIN
    CREATE TABLE TourInfoItems (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_TourInfoItems PRIMARY KEY DEFAULT NEWID(),
        TourId UNIQUEIDENTIFIER NOT NULL,
        Icon NVARCHAR(100) NULL,
        Label NVARCHAR(150) NOT NULL,
        Value NVARCHAR(300) NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_TourInfoItems_SortOrder DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_TourInfoItems_IsActive DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_TourInfoItems_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_TourInfoItems_Tours FOREIGN KEY (TourId)
            REFERENCES Tours(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_TourInfoItems_TourId ON TourInfoItems(TourId);
END
GO

/* ============================================================
   BOOKING OPTIONS
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingServiceTypes')
BEGIN
    CREATE TABLE BookingServiceTypes (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_BookingServiceTypes PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(150) NOT NULL,
        Code NVARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_BookingServiceTypes_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_BookingServiceTypes_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_BookingServiceTypes_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_BookingServiceTypes_Code ON BookingServiceTypes(Code);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingPassengerOptions')
BEGIN
    CREATE TABLE BookingPassengerOptions (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_BookingPassengerOptions PRIMARY KEY DEFAULT NEWID(),
        Label NVARCHAR(100) NOT NULL,
        MinPassengers INT NULL,
        MaxPassengers INT NULL,
        IsPrivate BIT NOT NULL CONSTRAINT DF_BookingPassengerOptions_IsPrivate DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_BookingPassengerOptions_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_BookingPassengerOptions_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_BookingPassengerOptions_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

/* ============================================================
   BOOKINGS EXTENSION
============================================================ */

IF COL_LENGTH('Bookings', 'Phone') IS NULL
BEGIN
    ALTER TABLE Bookings ADD Phone NVARCHAR(50) NULL;
END
GO

IF COL_LENGTH('Bookings', 'ServiceId') IS NULL
BEGIN
    ALTER TABLE Bookings ADD ServiceId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Bookings', 'TourId') IS NULL
BEGIN
    ALTER TABLE Bookings ADD TourId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Bookings', 'VehicleId') IS NULL
BEGIN
    ALTER TABLE Bookings ADD VehicleId UNIQUEIDENTIFIER NULL;
END
GO

IF COL_LENGTH('Bookings', 'PassengersRange') IS NULL
BEGIN
    ALTER TABLE Bookings ADD PassengersRange NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('Bookings', 'SourcePage') IS NULL
BEGIN
    ALTER TABLE Bookings ADD SourcePage NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_Bookings_Services'
)
BEGIN
    ALTER TABLE Bookings
    ADD CONSTRAINT FK_Bookings_Services
    FOREIGN KEY (ServiceId)
    REFERENCES Services(Id)
    ON DELETE SET NULL;
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_Bookings_Tours'
)
BEGIN
    ALTER TABLE Bookings
    ADD CONSTRAINT FK_Bookings_Tours
    FOREIGN KEY (TourId)
    REFERENCES Tours(Id)
    ON DELETE SET NULL;
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_Bookings_Vehicles'
)
BEGIN
    ALTER TABLE Bookings
    ADD CONSTRAINT FK_Bookings_Vehicles
    FOREIGN KEY (VehicleId)
    REFERENCES Vehicles(Id)
    ON DELETE SET NULL;
END
GO

/* ============================================================
   CONTACT MESSAGES EXTENSION
============================================================ */

IF COL_LENGTH('ContactMessages', 'Phone') IS NULL
BEGIN
    ALTER TABLE ContactMessages ADD Phone NVARCHAR(50) NULL;
END
GO

IF COL_LENGTH('ContactMessages', 'Subject') IS NULL
BEGIN
    ALTER TABLE ContactMessages ADD Subject NVARCHAR(200) NULL;
END
GO

IF COL_LENGTH('ContactMessages', 'SourcePage') IS NULL
BEGIN
    ALTER TABLE ContactMessages ADD SourcePage NVARCHAR(100) NULL;
END
GO

/* ============================================================
   COMPANY PROFILE
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanyProfile')
BEGIN
    CREATE TABLE CompanyProfile (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CompanyProfile PRIMARY KEY DEFAULT NEWID(),
        CompanyName NVARCHAR(200) NOT NULL,
        LegalName NVARCHAR(200) NULL,
        Description NVARCHAR(MAX) NULL,
        Address NVARCHAR(300) NULL,
        City NVARCHAR(100) NULL,
        Province NVARCHAR(50) NULL,
        PostalCode NVARCHAR(20) NULL,
        Country NVARCHAR(100) NULL,
        Email NVARCHAR(256) NULL,
        PhonePrimary NVARCHAR(50) NULL,
        PhoneSecondary NVARCHAR(50) NULL,
        MapEmbedUrl NVARCHAR(MAX) NULL,
        Latitude DECIMAL(10,7) NULL,
        Longitude DECIMAL(10,7) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CompanyProfile_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanyContacts')
BEGIN
    CREATE TABLE CompanyContacts (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CompanyContacts PRIMARY KEY DEFAULT NEWID(),
        CompanyProfileId UNIQUEIDENTIFIER NOT NULL,
        ContactType NVARCHAR(50) NOT NULL,
        Label NVARCHAR(100) NULL,
        Value NVARCHAR(300) NOT NULL,
        Icon NVARCHAR(100) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_CompanyContacts_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_CompanyContacts_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CompanyContacts_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT FK_CompanyContacts_CompanyProfile FOREIGN KEY (CompanyProfileId)
            REFERENCES CompanyProfile(Id)
            ON DELETE CASCADE
    );

    CREATE INDEX IX_CompanyContacts_CompanyProfileId 
        ON CompanyContacts(CompanyProfileId);
END
GO

/* ============================================================
   PARTNERS
============================================================ */

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Partners')
BEGIN
    CREATE TABLE Partners (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Partners PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(150) NOT NULL,
        LogoUrl NVARCHAR(500) NULL,
        WebsiteUrl NVARCHAR(500) NULL,
        PartnerType NVARCHAR(100) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Partners_IsActive DEFAULT 1,
        SortOrder INT NOT NULL CONSTRAINT DF_Partners_SortOrder DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Partners_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

/* ============================================================
   INITIAL SEED
============================================================ */

IF NOT EXISTS (SELECT 1 FROM Languages WHERE Code = 'it')
BEGIN
    INSERT INTO Languages (Code, Name, IsDefault, IsActive)
    VALUES ('it', 'Italiano', 1, 1);
END

IF NOT EXISTS (SELECT 1 FROM Languages WHERE Code = 'en')
BEGIN
    INSERT INTO Languages (Code, Name, IsDefault, IsActive)
    VALUES ('en', 'English', 0, 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM BookingServiceTypes WHERE Code = 'AIRPORT_TRANSFER')
BEGIN
    INSERT INTO BookingServiceTypes (Name, Code, SortOrder)
    VALUES 
    ('Transfer Aeroportuale', 'AIRPORT_TRANSFER', 1),
    ('Tour Personalizzato', 'CUSTOM_TOUR', 2),
    ('Evento Speciale', 'SPECIAL_EVENT', 3),
    ('Altro', 'OTHER', 99);
END
GO

IF NOT EXISTS (SELECT 1 FROM BookingPassengerOptions WHERE Label = '1 - 2 Persone')
BEGIN
    INSERT INTO BookingPassengerOptions (Label, MinPassengers, MaxPassengers, IsPrivate, SortOrder)
    VALUES
    ('1 - 2 Persone', 1, 2, 0, 1),
    ('3 - 5 Persone', 3, 5, 0, 2),
    ('Privato Exclusive', NULL, NULL, 1, 3);
END
GO