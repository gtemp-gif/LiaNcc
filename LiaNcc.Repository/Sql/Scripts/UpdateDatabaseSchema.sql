USE [LiaNcc]
GO

-- SITE PAGES
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SitePages]') AND name = N'MetaTitle')
    ALTER TABLE [dbo].[SitePages] ADD [MetaTitle] NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SitePages]') AND name = N'MetaDescription')
    ALTER TABLE [dbo].[SitePages] ADD [MetaDescription] NVARCHAR(500) NULL;

-- PAGE SECTIONS
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PageSections]') AND name = N'Title')
    ALTER TABLE [dbo].[PageSections] ADD [Title] NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PageSections]') AND name = N'Description')
    ALTER TABLE [dbo].[PageSections] ADD [Description] NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PageSections]') AND name = N'ImageUrl')
    ALTER TABLE [dbo].[PageSections] ADD [ImageUrl] NVARCHAR(1000) NULL;

-- CALL TO ACTIONS
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CallToActions]') AND name = N'Label')
    ALTER TABLE [dbo].[CallToActions] ADD [Label] NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CallToActions]') AND name = N'Title')
    ALTER TABLE [dbo].[CallToActions] ADD [Title] NVARCHAR(200) NULL;

-- SERVICES
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND name = N'Description')
    ALTER TABLE [dbo].[Services] ADD [Description] NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND name = N'CoverImageUrl')
    ALTER TABLE [dbo].[Services] ADD [CoverImageUrl] NVARCHAR(1000) NULL;

-- VEHICLES
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vehicles]') AND name = N'Title')
    ALTER TABLE [dbo].[Vehicles] ADD [Title] NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vehicles]') AND name = N'Description')
    ALTER TABLE [dbo].[Vehicles] ADD [Description] NVARCHAR(MAX) NULL;

-- TOURS
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'CoverImageUrl')
    ALTER TABLE [dbo].[Tours] ADD [CoverImageUrl] NVARCHAR(1000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'Description')
    ALTER TABLE [dbo].[Tours] ADD [Description] NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'HeroTitle')
    ALTER TABLE [dbo].[Tours] ADD [HeroTitle] NVARCHAR(200) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'HeroSubtitle')
    ALTER TABLE [dbo].[Tours] ADD [HeroSubtitle] NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'ExperienceDescription')
    ALTER TABLE [dbo].[Tours] ADD [ExperienceDescription] NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'ExperienceImageUrl')
    ALTER TABLE [dbo].[Tours] ADD [ExperienceImageUrl] NVARCHAR(1000) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'DurationDays')
    ALTER TABLE [dbo].[Tours] ADD [DurationDays] INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'DurationHours')
    ALTER TABLE [dbo].[Tours] ADD [DurationHours] INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'MeetingPoint')
    ALTER TABLE [dbo].[Tours] ADD [MeetingPoint] NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'VehicleId')
    ALTER TABLE [dbo].[Tours] ADD [VehicleId] UNIQUEIDENTIFIER NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tours]') AND name = N'IsBookable')
    ALTER TABLE [dbo].[Tours] ADD [IsBookable] BIT NOT NULL DEFAULT 1;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND name = N'IsActive')
    ALTER TABLE [dbo].[Services] ADD [IsActive] BIT NOT NULL DEFAULT 1;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND name = N'IsBookable')
    ALTER TABLE [dbo].[Services] ADD [IsBookable] BIT NOT NULL DEFAULT 1;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vehicles]') AND name = N'IsActive')
    ALTER TABLE [dbo].[Vehicles] ADD [IsActive] BIT NOT NULL DEFAULT 1;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vehicles]') AND name = N'IsBookable')
    ALTER TABLE [dbo].[Vehicles] ADD [IsBookable] BIT NOT NULL DEFAULT 1;

GO
