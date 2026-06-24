using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
public sealed class TickerLateJoinStatusEvent : EntityEventArgs
{
	public bool Disallowed { get; }

	public TickerLateJoinStatusEvent(bool disallowed)
	{
		Disallowed = disallowed;
	}
}
