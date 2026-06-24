using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivCommanderCandidateEntry
{
	public NetUserId UserId { get; }

	public string Name { get; }

	public int PlaytimeMinutes { get; }

	public bool IsSelf { get; }

	public int Priority { get; }

	public CivCommanderCandidateEntry(NetUserId userId, string name, int playtimeMinutes, bool isSelf, int priority = 0)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Name = name;
		PlaytimeMinutes = playtimeMinutes;
		IsSelf = isSelf;
		Priority = priority;
	}
}
