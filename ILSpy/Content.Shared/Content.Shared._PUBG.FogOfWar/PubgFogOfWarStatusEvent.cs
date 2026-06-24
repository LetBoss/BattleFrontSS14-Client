using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.FogOfWar;

[Serializable]
[NetSerializable]
public sealed class PubgFogOfWarStatusEvent : EntityEventArgs
{
	public bool Active { get; }

	public PubgFogOfWarStatusEvent(bool active)
	{
		Active = active;
	}
}
