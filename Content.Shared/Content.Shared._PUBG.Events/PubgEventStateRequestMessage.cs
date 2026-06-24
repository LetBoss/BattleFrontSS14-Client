using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventStateRequestMessage : EntityEventArgs
{
	public string EventKey { get; }

	public PubgEventStateRequestMessage(string eventKey)
	{
		EventKey = eventKey;
	}
}
