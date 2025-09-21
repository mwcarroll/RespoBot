using System;

namespace RespoBot.Events.Args;

public class NewTrackedMemberEventArgs(int iRacingMemberId, long discordMemberId) : EventArgs
{
    // ReSharper disable once InconsistentNaming
    public int IRacingMemberId { get; private set; } = iRacingMemberId;
    public long DiscordMemberId { get; private set; } = discordMemberId;
}