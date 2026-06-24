using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events;

[Serializable]
[NetSerializable]
public sealed class NPCSteeringDebugEvent : EntityEventArgs
{
	public List<NPCSteeringDebugData> Data;

	public NPCSteeringDebugEvent(List<NPCSteeringDebugData> data)
	{
		Data = data;
	}
}
