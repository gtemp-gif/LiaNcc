USE [LiaNcc]
GO

-- ABOUT SECTION
IF COL_LENGTH('dbo.CompanyProfile', 'AboutTitle') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyProfile ADD AboutTitle NVARCHAR(200) NULL;
END
GO

IF COL_LENGTH('dbo.CompanyProfile', 'AboutDescription') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyProfile ADD AboutDescription NVARCHAR(MAX) NULL;
END
GO

IF COL_LENGTH('dbo.CompanyProfile', 'AboutImageUrl') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyProfile ADD AboutImageUrl NVARCHAR(1000) NULL;
END
GO

-- POSITION
IF COL_LENGTH('dbo.CompanyProfile', 'Latitude') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyProfile ADD Latitude DECIMAL(10,7) NULL;
END
GO

IF COL_LENGTH('dbo.CompanyProfile', 'Longitude') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyProfile ADD Longitude DECIMAL(10,7) NULL;
END
GO

IF COL_LENGTH('dbo.CompanyProfile', 'GoogleMapsUrl') IS NULL
BEGIN
    ALTER TABLE dbo.CompanyProfile ADD GoogleMapsUrl NVARCHAR(1000) NULL;
END
GO
