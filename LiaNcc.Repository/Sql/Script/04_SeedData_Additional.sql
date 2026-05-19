-- Inserimento Lingue
INSERT INTO Languages (Id, Code, Name, IsDefault, IsActive, CreatedAt)
VALUES
(NEWID(), 'it', 'Italiano', 1, 1, GETUTCDATE()),
(NEWID(), 'en', 'English', 0, 1, GETUTCDATE());

-- Inserimento Profilo Aziendale
DECLARE @CompanyId UNIQUEIDENTIFIER = NEWID();
INSERT INTO CompanyProfile (Id, Name, VatNumber, Address, City, ZipCode, Country, CreatedAt)
VALUES
(@CompanyId, 'LIA NCC', '12345678901', 'Via Roma 1', 'Matera', '75100', 'Italia', GETUTCDATE());

-- Inserimento Contatti Aziendali
INSERT INTO CompanyContacts (Id, CompanyProfileId, ContactType, ContactValue, IsPrimary, CreatedAt)
VALUES
(NEWID(), @CompanyId, 'Phone', '+39 333 715 9308', 1, GETUTCDATE()),
(NEWID(), @CompanyId, 'Phone', '+39 350 576 3433', 0, GETUTCDATE()),
(NEWID(), @CompanyId, 'Email', 'liamoto@alice.it', 1, GETUTCDATE());

-- Inserimento Pagine Sito (CMS)
DECLARE @HomePageId UNIQUEIDENTIFIER = NEWID();
DECLARE @FleetPageId UNIQUEIDENTIFIER = NEWID();
DECLARE @ToursPageId UNIQUEIDENTIFIER = NEWID();

INSERT INTO SitePages (Id, Slug, Name, IsActive, CreatedAt)
VALUES
(@HomePageId, 'home', 'Home Page', 1, GETUTCDATE()),
(@FleetPageId, 'fleet', 'Flotta', 1, GETUTCDATE()),
(@ToursPageId, 'tours', 'Tour', 1, GETUTCDATE());

-- Inserimento Sezioni Pagina (Home)
INSERT INTO PageSections (Id, PageId, Name, SortOrder, IsActive, CreatedAt)
VALUES
(NEWID(), @HomePageId, 'Hero', 1, 1, GETUTCDATE()),
(NEWID(), @HomePageId, 'Chi Siamo', 2, 1, GETUTCDATE()),
(NEWID(), @HomePageId, 'I Nostri Servizi', 3, 1, GETUTCDATE());

-- Inserimento Media Assets (Immagini da FE)
DECLARE @HeroMediaId UNIQUEIDENTIFIER = NEWID();
DECLARE @MateraMediaId UNIQUEIDENTIFIER = NEWID();
DECLARE @ItriaMediaId UNIQUEIDENTIFIER = NEWID();
DECLARE @AmalfiMediaId UNIQUEIDENTIFIER = NEWID();

INSERT INTO MediaAssets (Id, FileName, FileUrl, MimeType, FileSize, CreatedAt)
VALUES
(@HeroMediaId, 'hero-bg.jpg', 'https://lh3.googleusercontent.com/aida-public/AB6AXuABLx3pL6IWe1fmg1Mytk9zyigB_MJ_fExwdSaSwqMknbOfTcmaf0rpIm6WXV22AKYSA_iMCq5mE70DVqPkx4m6W1pjJtCnWbUwFFx35kKDyE4L5I4-k8I2TL6qpmBO9dDbaAz7DzAUeg0qeggH_Oi78bo9_NgfTSADO71wr0LX-5RPPhcLImPcJAs52atpzLpoDUeWEwBkaZ9DwWrDSqYdeoQOXfoKMODMHiq7ngFLLfQ_orO75Nzfe64ssA2_sjT_XD-YtW6g13q3', 'image/jpeg', 0, GETUTCDATE()),
(@MateraMediaId, 'matera.jpg', 'https://lh3.googleusercontent.com/aida-public/AB6AXuB_26FKFmffDMRQjZlBs2q7aOfiViMvBFXUH_caO-scku4mrbyHlmW_CLlTvmvYylgeoQI3ocah3I_3EvDK5YPANBs_ZQ_WkEUGXregJhxY9GMegst8K0RyWIpOiFdDeNWMzMAGigJE5siXmq6ANAlgo6whHBCSIWOrVxXBuZ08443Koo757QfRYhM73UtaLCygsN8jSNFwpuc5k-EJgB4oGsgIXBdkhjHnfcchWho1WDrhEFERdUEo-ZV1EtFwVM47BfvhfzWKgn5j', 'image/jpeg', 0, GETUTCDATE()),
(@ItriaMediaId, 'itria.jpg', 'https://lh3.googleusercontent.com/aida-public/AB6AXuCtShf6qOrX_gwJmojpPrAkKucl4CiG2uuAx-nsaZx8MvipdnZGcubfe_rYH2N5QkKxYsUhXnPOEdmJYv-mraQ2oC3ssSVSeqYJYuay4TFmfbn68B92A2VClChKatdB_lcSNU29-uwgxXJkO02H7qcyroDRsaNyl8ueMia2nkvijmLEuX2XT_Pe5M883pZL7u7YkyEEhcwv5hBEJxz1NY3dkke8ljRVJtlXUnaAXrdVjRBAx_pNf1_-ZsMY3iL9lacaqjjR2aIaapXM', 'image/jpeg', 0, GETUTCDATE()),
(@AmalfiMediaId, 'amalfi.jpg', 'https://lh3.googleusercontent.com/aida-public/AB6AXuBk3sSERBsPJTlbahW5ygl7_87dc-SdRDE-yqn0Bd7KDW3dmGVerBTenWJpfMvJN0suCLPXGfyLptQuEi5gIAYNMmTyBuZxNojKvugZDzigPnYqh5pzBF0xj6IbWZhBlqbxQ1filaKNjhWEh0nqut9nY5-8efM3DbCpPI0WvX9w8WCnvOk2dqOYMMTN1RohBtWJAXTjeA1FHbs9AISOeYCCa3gf5gbqyA3Yv6eI0ThNDV5ImXBLoeFApvFE05ANwq7qwExMxusG5qa-', 'image/jpeg', 0, GETUTCDATE());

-- Inserimento EntityMedia (Colleghiamo media alle entità)
-- Supponendo che i tour inseriti in 03_SeedData.sql esistano, recuperiamo gli ID:
DECLARE @MateraTourId UNIQUEIDENTIFIER;
SELECT TOP 1 @MateraTourId = Id FROM Tours WHERE Slug = 'matera';

IF @MateraTourId IS NOT NULL
BEGIN
    INSERT INTO EntityMedia (Id, EntityName, EntityId, MediaAssetId, MediaType, SortOrder, CreatedAt)
    VALUES (NEWID(), 'Tour', @MateraTourId, @MateraMediaId, 'Hero', 1, GETUTCDATE());
END

DECLARE @ItriaTourId UNIQUEIDENTIFIER;
SELECT TOP 1 @ItriaTourId = Id FROM Tours WHERE Slug = 'valle-ditria';

IF @ItriaTourId IS NOT NULL
BEGIN
    INSERT INTO EntityMedia (Id, EntityName, EntityId, MediaAssetId, MediaType, SortOrder, CreatedAt)
    VALUES (NEWID(), 'Tour', @ItriaTourId, @ItriaMediaId, 'Hero', 1, GETUTCDATE());
END

DECLARE @AmalfiTourId UNIQUEIDENTIFIER;
SELECT TOP 1 @AmalfiTourId = Id FROM Tours WHERE Slug = 'costiera-amalfitana';

IF @AmalfiTourId IS NOT NULL
BEGIN
    INSERT INTO EntityMedia (Id, EntityName, EntityId, MediaAssetId, MediaType, SortOrder, CreatedAt)
    VALUES (NEWID(), 'Tour', @AmalfiTourId, @AmalfiMediaId, 'Hero', 1, GETUTCDATE());
END

-- Localizzazioni (LocalizedContents)
-- Traduciamo il titolo di "Matera" in inglese
INSERT INTO LocalizedContents (Id, EntityName, EntityId, ContentKey, LanguageCode, ContentValue, CreatedAt)
VALUES (NEWID(), 'Tour', @MateraTourId, 'Name', 'en', 'Matera - The City of Stones', GETUTCDATE());

-- Traduciamo il Chi Siamo nella HomePage
DECLARE @ChiSiamoSectionId UNIQUEIDENTIFIER;
SELECT TOP 1 @ChiSiamoSectionId = Id FROM PageSections WHERE Name = 'Chi Siamo' AND PageId = @HomePageId;

INSERT INTO LocalizedContents (Id, EntityName, EntityId, ContentKey, LanguageCode, ContentValue, CreatedAt)
VALUES
(NEWID(), 'PageSection', @ChiSiamoSectionId, 'Title', 'it', 'Chi Siamo', GETUTCDATE()),
(NEWID(), 'PageSection', @ChiSiamoSectionId, 'Title', 'en', 'About Us', GETUTCDATE()),
(NEWID(), 'PageSection', @ChiSiamoSectionId, 'Description', 'it', 'LIA NCC è un''azienda specializzata nel servizio di noleggio con conducente (NCC) e nell''organizzazione di transfer e tour personalizzati...', GETUTCDATE()),
(NEWID(), 'PageSection', @ChiSiamoSectionId, 'Description', 'en', 'LIA NCC is a company specializing in chauffeur-driven car hire (NCC) and the organization of personalized transfers and tours...', GETUTCDATE());
