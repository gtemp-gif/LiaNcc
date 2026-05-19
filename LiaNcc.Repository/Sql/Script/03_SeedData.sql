-- Inserimento categorie Tour
INSERT INTO TourCategories (Id, Name, Slug, Description, IsActive, CreatedAt)
VALUES
(NEWID(), 'Tour d''Autore', 'tour-dautore', 'Esperienza Chauffeur Premium', 1, GETUTCDATE());

DECLARE @CategoryId UNIQUEIDENTIFIER;
SELECT TOP 1 @CategoryId = Id FROM TourCategories WHERE Slug = 'tour-dautore';

-- Inserimento Tours (Matera, Valle d'Itria, Costiera Amalfitana)
INSERT INTO Tours (Id, CategoryId, Name, Slug, Price, IsFeatured, IsActive, SortOrder, CreatedAt)
VALUES
(NEWID(), @CategoryId, 'Matera', 'matera', NULL, 1, 1, 1, GETUTCDATE()),
(NEWID(), @CategoryId, 'Valle d''Itria', 'valle-ditria', NULL, 1, 1, 2, GETUTCDATE()),
(NEWID(), @CategoryId, 'Costiera Amalfitana', 'costiera-amalfitana', NULL, 1, 1, 3, GETUTCDATE());

-- Inserimento Categorie Veicoli
INSERT INTO VehicleCategories (Id, Name, Slug, Description, IsActive, CreatedAt)
VALUES
(NEWID(), 'Premium', 'premium', 'Berline di lusso', 1, GETUTCDATE()),
(NEWID(), 'Van', 'van', 'Van spaziosi e lussuosi', 1, GETUTCDATE());

DECLARE @PremiumId UNIQUEIDENTIFIER, @VanId UNIQUEIDENTIFIER;
SELECT TOP 1 @PremiumId = Id FROM VehicleCategories WHERE Slug = 'premium';
SELECT TOP 1 @VanId = Id FROM VehicleCategories WHERE Slug = 'van';

-- Inserimento Veicoli
INSERT INTO Vehicles (Id, CategoryId, Name, Slug, SeatsCount, LuggageCount, Description, IsFeatured, IsActive, SortOrder, CreatedAt)
VALUES
(NEWID(), @PremiumId, 'Mercedes Classe E', 'mercedes-classe-e', 3, 2, 'L''eleganza senza tempo incontra la tecnologia avanzata. Il rifugio perfetto per il viaggiatore d''affari o la coppia.', 1, 1, 1, GETUTCDATE()),
(NEWID(), @PremiumId, 'Maserati Ghibli', 'maserati-ghibli', 3, 2, 'Il lusso italiano nella sua forma più dinamica. Per chi desidera un ingresso indimenticabile.', 1, 1, 2, GETUTCDATE()),
(NEWID(), @VanId, 'Mercedes Classe V', 'mercedes-classe-v', 7, 7, 'Spazio senza compromessi sul lusso. La scelta definitiva per gruppi esclusivi e famiglie numerose.', 1, 1, 3, GETUTCDATE());

-- Inserimento Servizi
INSERT INTO Services (Id, Name, Icon, IsFeatured, IsBookable, IsActive, SortOrder, CreatedAt)
VALUES
(NEWID(), 'Transfer Aeroportuali Privati', 'bi-airplane', 1, 1, 1, 1, GETUTCDATE()),
(NEWID(), 'Tour Esclusivi in Basilicata & Puglia', 'bi-map', 1, 1, 1, 2, GETUTCDATE()),
(NEWID(), 'Disposizioni Orarie per Business & Eventi', 'bi-briefcase', 1, 1, 1, 3, GETUTCDATE()),
(NEWID(), 'Servizi Matrimoniali Luxury', 'bi-heart', 1, 1, 1, 4, GETUTCDATE());

-- Inserimento Utente Admin (credenziali provvisorie, password hash BCrypt per 'admin')
INSERT INTO Users (Id, FullName, Email, PasswordHash, IsActive, CreatedAt)
VALUES
(NEWID(), 'Amministratore', 'admin@liancc.it', '$2a$11$u.k6Z5.Z/X3P8H122v4P7.0l37D32E7o8Vl2E8/0A02778.6s9O', 1, GETUTCDATE());

-- Inserimento Ruolo Admin
INSERT INTO Roles (Id, Name, Description, IsActive, CreatedAt)
VALUES
(NEWID(), 'Admin', 'Amministratore del sistema', 1, GETUTCDATE()),
(NEWID(), 'Operator', 'Operatore', 1, GETUTCDATE());

DECLARE @UserId UNIQUEIDENTIFIER, @RoleId UNIQUEIDENTIFIER;
SELECT TOP 1 @UserId = Id FROM Users WHERE Email = 'admin@liancc.it';
SELECT TOP 1 @RoleId = Id FROM Roles WHERE Name = 'Admin';

INSERT INTO UserRoles (UserId, RoleId)
VALUES (@UserId, @RoleId);
