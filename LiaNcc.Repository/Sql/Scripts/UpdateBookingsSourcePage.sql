USE [LiaNcc]
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND name = N'SourcePage')
    ALTER TABLE [dbo].[Bookings] ADD [SourcePage] NVARCHAR(100) NULL;
GO
