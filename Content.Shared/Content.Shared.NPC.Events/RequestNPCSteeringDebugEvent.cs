using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events;

[Serializable]
[NetSerializable]
public sealed class RequestNPCSteeringDebugEvent : EntityEventArgs
{
	public bool Enabled;
}
