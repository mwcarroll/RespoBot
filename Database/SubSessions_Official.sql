USE [Respobot]
GO

DROP TABLE IF EXISTS [dbo].[SubSessions_Official]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubSessions_Official](
	[SubSessionId]           [int]           NOT NULL,
	[SeriesName]             [nvarchar](512) NOT NULL,
	[TrackName]              [nvarchar](512) NOT NULL,
	[NumberOfDrivers]        [int]           NOT NULL,
	[StrengthOfField]        [int]           NOT NULL,
    CONSTRAINT [PK_SubSessions] PRIMARY KEY CLUSTERED ([SubSessionId] DESC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [Idx_SubSessions_IRacingMemberId]
    ON [dbo].[SubSessions_Official]([SubSessionId] DESC);
GO
