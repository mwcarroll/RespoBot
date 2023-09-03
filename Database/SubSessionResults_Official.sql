
USE [Respobot]
GO

DROP TABLE IF EXISTS [dbo].[SubSessionResults_Official]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubSessionResults_Official](
	[IRacingMemberId]        [int]           NOT NULL,
	[SubSessionId]           [int]           NOT NULL,
	[ClassName]              [nvarchar](512) NOT NULL,
	[CarNumber]              [int]           NOT NULL,
	[QualifyPosition]        [int]           NOT NULL,
	[FinishPosition]         [int]           NOT NULL,
	[NumberOfDriversInClass] [int]           NULL,
	[CarNumberInClass]       [int]           NULL,
	[StrengthOfFieldClass]   [int]           NULL,
	[QualifyPositionInClass] [int]           NULL,
	[FinishPositionInClass]  [int]           NULL,
	[ChampionshipPoints]     [int]           NULL,
	[IRatingNew]             [int]           NULL,
	[IRatingChange]          [int]           NULL,
	[SafetyRatingNew]        [decimal](3, 2) NULL,
	[SafetyRatingChange]     [decimal](3, 2) NULL,
	[IncidentPoints]         [int]           NOT NULL,
    CONSTRAINT [PK_SubSessionResults_Official] PRIMARY KEY CLUSTERED ([IRacingMemberId] ASC, [SubSessionId] DESC)
);
GO

CREATE NONCLUSTERED INDEX [Idx_SubSessionResults_IRacingMemberId]
    ON [dbo].[SubSessionResults_Official]([IRacingMemberId] ASC);
GO

CREATE NONCLUSTERED INDEX [Idx_SubSessionResults_SubSessionId]
    ON [dbo].[SubSessionResults_Official]([SubSessionId] DESC);
GO