USE [Respobot]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrackedMembers] (
    [IRacingMemberId]     INT            NOT NULL,
    [DiscordMemberId]     BIGINT         NOT NULL,
    [Name]                NVARCHAR (255) NULL,
    [LastCheckedHosted]   DATETIME       NULL,
    [LastCheckedOfficial] DATETIME       NULL,
    [LastCheckedLicense]  DATETIME       NULL,
    [MemberSince]         INT            NULL,
    CONSTRAINT [PK_TrackedMembers] PRIMARY KEY CLUSTERED ([IRacingMemberId] ASC, [DiscordMemberId] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [Idx_TrackedMembers__IRacingMemberId]
    ON [dbo].[TrackedMembers]([IRacingMemberId] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [Idx_TrackedMembers_DiscordMemberId]
    ON [dbo].[TrackedMembers]([DiscordMemberId] ASC);
GO
