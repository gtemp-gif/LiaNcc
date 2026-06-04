USE [LiaNcc]
GO

-- 1. CLEANUP (Optional, depends on how you want to use the script)
-- DELETE FROM LocalizedContents;
-- DELETE FROM CallToActions;
-- DELETE FROM PageSections;
-- DELETE FROM SitePages;

-- 2. FIXED GUIDs FOR CONSISTENCY
DECLARE @HomePageId UNIQUEIDENTIFIER = 'A1B2C3D4-E5F6-4789-8012-34567890ABCD';
DECLARE @CompanyProfileId UNIQUEIDENTIFIER = '7705ee36-bc18-45ee-b5a0-6da6ab542a1c';
DECLARE @HeroSectionId UNIQUEIDENTIFIER = 'B2C3D4E5-F6A7-4890-9123-4567890ABCDE';
DECLARE @AboutSectionId UNIQUEIDENTIFIER = 'C3D4E5F6-A7B8-4901-0234-567890ABCDEF';
DECLARE @ServicesSectionId UNIQUEIDENTIFIER = 'D4E5F6A7-B8C9-4012-1345-67890ABCDEFG';
DECLARE @ToursSectionId UNIQUEIDENTIFIER = 'E5F6A7B8-C9D0-4123-2456-7890ABCDEFGH';
DECLARE @FleetSectionId UNIQUEIDENTIFIER = 'F6A7B8C9-D0E1-4234-3567-890ABCDEFGHI';
DECLARE @ContactSectionId UNIQUEIDENTIFIER = 'A7B8C9D0-E1F2-4345-4678-90ABCDEFGHIJ';

-- 3. SITE PAGES
IF NOT EXISTS (SELECT 1 FROM SitePages WHERE Id = @HomePageId)
BEGIN
    INSERT INTO SitePages (Id, Slug, Name, MetaTitle, MetaDescription, IsActive)
    VALUES (@HomePageId, 'home', 'Home Page', 'LIA NCC - Eccellenza in Movimento', 'Servizi di noleggio con conducente, transfer e tour personalizzati in Basilicata e Puglia.', 1);
END

-- 4. PAGE SECTIONS
-- Hero
IF NOT EXISTS (SELECT 1 FROM PageSections WHERE Id = @HeroSectionId)
BEGIN
    INSERT INTO PageSections (Id, PageId, Name, Title, Description, SortOrder, IsActive, ImageUrl)
    VALUES (@HeroSectionId, @HomePageId, 'Hero', 'Esperienza Chauffeur Premium', 'Il Vertice del Trasporto Privato', 1, 1, 'https://lh3.googleusercontent.com/aida-public/AB6AXuABLx3pL6IWe1fmg1Mytk9zyigB_MJ_fExwdSaSwqMknbOfTcmaf0rpIm6WXV22AKYSA_iMCq5mE70DVqPkx4m6W1pjJtCnWbUwFFx35kKDyE4L5I4-k8I2TL6qpmBO9dDbaAz7DzAUeg0qeggH_Oi78bo9_NgfTSADO71wr0LX-5RPPhcLImPcJAs52atpzLpoDUeWEwBkaZ9DwWrDSqYdeoQOXfoKMODMHiq7ngFLLfQ_orO75Nzfe64ssA2_sjT_XD-YtW6g13q3');
END

-- About
IF NOT EXISTS (SELECT 1 FROM PageSections WHERE Id = @AboutSectionId)
BEGIN
    INSERT INTO PageSections (Id, PageId, Name, Title, Description, SortOrder, IsActive)
    VALUES (@AboutSectionId, @HomePageId, 'About', 'Chi Siamo', 'LIA NCC è un’azienda specializzata nel servizio di noleggio con conducente (NCC) e nell’organizzazione di transfer e tour personalizzati, operativa principalmente tra Basilicata e Puglia. Operativi dal 2009, mettiamo a disposizione dei nostri clienti anni di experience nel settore del trasporto persone, garantendo sempre servizi affidabili, puntuali e confortevoli.', 2, 1);
END

-- Services
IF NOT EXISTS (SELECT 1 FROM PageSections WHERE Id = @ServicesSectionId)
BEGIN
    INSERT INTO PageSections (Id, PageId, Name, Title, Description, SortOrder, IsActive)
    VALUES (@ServicesSectionId, @HomePageId, 'Services', 'Servizi Esclusivi', 'Offriamo soluzioni di trasporto su misura per ogni esigenza.', 3, 1);
END

-- Tours
IF NOT EXISTS (SELECT 1 FROM PageSections WHERE Id = @ToursSectionId)
BEGIN
    INSERT INTO PageSections (Id, PageId, Name, Title, Description, SortOrder, IsActive)
    VALUES (@ToursSectionId, @HomePageId, 'Tours', 'Destinazioni d''Autore', 'Scopri i tesori del Sud Italia con i nostri tour guidati.', 4, 1);
END

-- Fleet
IF NOT EXISTS (SELECT 1 FROM PageSections WHERE Id = @FleetSectionId)
BEGIN
    INSERT INTO PageSections (Id, PageId, Name, Title, Description, SortOrder, IsActive)
    VALUES (@FleetSectionId, @HomePageId, 'Fleet', 'FLOTTA D''ECCELLENZA', 'Veicoli di lusso per un comfort senza compromessi.', 5, 1);
END

-- Contact
IF NOT EXISTS (SELECT 1 FROM PageSections WHERE Id = @ContactSectionId)
BEGIN
    INSERT INTO PageSections (Id, PageId, Name, Title, Description, SortOrder, IsActive)
    VALUES (@ContactSectionId, @HomePageId, 'Contact', 'Richiesta Prenotazione VIP', 'Compila il modulo per ricevere un preventivo personalizzato immediato.', 6, 1);
END

-- 5. CALL TO ACTIONS
INSERT INTO CallToActions (PageId, SectionId, Label, Url, SortOrder, IsActive)
SELECT @HomePageId, @HeroSectionId, 'Prenota il tuo Viaggio', '#booking-form', 1, 1
WHERE NOT EXISTS (SELECT 1 FROM CallToActions WHERE SectionId = @HeroSectionId AND Label = 'Prenota il tuo Viaggio');

INSERT INTO CallToActions (PageId, SectionId, Label, Url, SortOrder, IsActive)
SELECT @HomePageId, @HeroSectionId, 'Scopri la Flotta', '/Home/Fleet', 2, 1
WHERE NOT EXISTS (SELECT 1 FROM CallToActions WHERE SectionId = @HeroSectionId AND Label = 'Scopri la Flotta');

-- 6. TRANSLATIONS (English)

-- SitePage Home
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'SitePage', @HomePageId, 'MetaTitle', 'en', 'LIA NCC - Excellence in Motion'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @HomePageId AND ContentKey = 'MetaTitle' AND LanguageCode = 'en');

INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'SitePage', @HomePageId, 'MetaDescription', 'en', 'Chauffeur services, transfers and personalized tours in Basilicata and Puglia.'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @HomePageId AND ContentKey = 'MetaDescription' AND LanguageCode = 'en');

-- Section Hero
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @HeroSectionId, 'Title', 'en', 'Premium Chauffeur Experience'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @HeroSectionId AND ContentKey = 'Title' AND LanguageCode = 'en');

INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @HeroSectionId, 'Description', 'en', 'The Pinnacle of Private Transport'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @HeroSectionId AND ContentKey = 'Description' AND LanguageCode = 'en');

-- Section About
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @AboutSectionId, 'Title', 'en', 'About Us'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @AboutSectionId AND ContentKey = 'Title' AND LanguageCode = 'en');

INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @AboutSectionId, 'Description', 'en', 'LIA NCC is a company specialized in the chauffeur service (NCC) and in the organization of personalized transfers and tours, operating mainly between Basilicata and Puglia. Operating since 2009, we provide our customers with years of experience in the passenger transport sector, always guaranteeing reliable, punctual and comfortable services.'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @AboutSectionId AND ContentKey = 'Description' AND LanguageCode = 'en');

-- Section Services
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @ServicesSectionId, 'Title', 'en', 'Exclusive Services'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @ServicesSectionId AND ContentKey = 'Title' AND LanguageCode = 'en');

-- Section Tours
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @ToursSectionId, 'Title', 'en', 'Signature Destinations'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @ToursSectionId AND ContentKey = 'Title' AND LanguageCode = 'en');

-- Section Fleet
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @FleetSectionId, 'Title', 'en', 'EXCELLENCE FLEET'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @FleetSectionId AND ContentKey = 'Title' AND LanguageCode = 'en');

-- Section Contact
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @ContactSectionId, 'Title', 'en', 'VIP Booking Request'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @ContactSectionId AND ContentKey = 'Title' AND LanguageCode = 'en');

INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'PageSection', @ContactSectionId, 'Description', 'en', 'Fill out the form to receive an immediate personalized quote.'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @ContactSectionId AND ContentKey = 'Description' AND LanguageCode = 'en');

-- Call To Actions
DECLARE @CtaBookId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM CallToActions WHERE SectionId = @HeroSectionId AND Label = 'Prenota il tuo Viaggio');
IF @CtaBookId IS NOT NULL
BEGIN
    INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
    SELECT 'CallToAction', @CtaBookId, 'Label', 'en', 'Book Your Trip'
    WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @CtaBookId AND ContentKey = 'Label' AND LanguageCode = 'en');
END

DECLARE @CtaFleetId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM CallToActions WHERE SectionId = @HeroSectionId AND Label = 'Scopri la Flotta');
IF @CtaFleetId IS NOT NULL
BEGIN
    INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
    SELECT 'CallToAction', @CtaFleetId, 'Label', 'en', 'Discover the Fleet'
    WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @CtaFleetId AND ContentKey = 'Label' AND LanguageCode = 'en');
END

-- 7. COMPANY PROFILE
IF NOT EXISTS (SELECT 1 FROM CompanyProfile WHERE Id = @CompanyProfileId)
BEGIN
    INSERT INTO CompanyProfile (Id, Name, VatNumber, Address, City, ZipCode, Country, Latitude, Longitude, GoogleMapsUrl, AboutTitle, AboutDescription)
    VALUES (@CompanyProfileId, 'LiaNcc Chauffeur Service', '01234567890', 'Via delle Rimembranze', 'Grassano', '75014', 'Italia', 40.6358178, 16.2798153, 'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3032.553259837731!2d16.2798153!3d40.6358178!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x1338dfd6e64c3c39%3A0x6b77c4f4a3e9c8e!2sVia%20delle%20Rimembranze%2C%2075014%20Grassano%20MT!5e0!3m2!1sit!2sit!4v1710000000000!5m2!1sit!2sit', 'L''Eccellenza in Movimento', 'LIA NCC è un’azienda specializzata nel servizio di noleggio con conducente (NCC) e nell’organizzazione di transfer e tour personalizzati, operativa principalmente tra Basilicata e Puglia. Operativi dal 2009, mettiamo a disposizione dei nostri clienti anni di experience nel settore del trasporto persone, garantendo sempre servizi affidabili, puntuali e confortevoli.');
END

-- Company Profile Translations
INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'CompanyProfile', @CompanyProfileId, 'AboutTitle', 'en', 'Excellence in Motion'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @CompanyProfileId AND ContentKey = 'AboutTitle' AND LanguageCode = 'en');

INSERT INTO LocalizedContents (EntityName, EntityId, ContentKey, LanguageCode, ContentValue)
SELECT 'CompanyProfile', @CompanyProfileId, 'AboutDescription', 'en', 'LIA NCC is a company specialized in the chauffeur service (NCC) and in the organization of personalized transfers and tours, operating mainly between Basilicata and Puglia. Operating since 2009, we provide our customers with years of experience in the passenger transport sector, always guaranteeing reliable, punctual and comfortable services.'
WHERE NOT EXISTS (SELECT 1 FROM LocalizedContents WHERE EntityId = @CompanyProfileId AND ContentKey = 'AboutDescription' AND LanguageCode = 'en');

GO
