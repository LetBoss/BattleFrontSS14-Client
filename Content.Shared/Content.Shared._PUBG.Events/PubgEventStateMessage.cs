using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventStateMessage : EntityEventArgs
{
	public DateTime ServerNowUtc { get; }

	public PubgEventDetailInfo State { get; }

	public PubgEventStateMessage(DateTime serverNowUtc, PubgEventDetailInfo state)
	{
		ServerNowUtc = serverNowUtc;
		State = state;
	}
}
