using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterSetAllowAutoLeaderRequestEvent : EntityEventArgs
{
	public bool Allow { get; }

	public CivRosterSetAllowAutoLeaderRequestEvent(bool allow)
	{
		Allow = allow;
	}
}
