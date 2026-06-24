using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterWithdrawCommanderRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public CivRosterWithdrawCommanderRequestEvent(int teamId)
	{
		TeamId = teamId;
	}
}
