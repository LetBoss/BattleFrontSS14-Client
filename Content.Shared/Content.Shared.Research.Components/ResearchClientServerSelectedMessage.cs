using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components;

[Serializable]
[NetSerializable]
public sealed class ResearchClientServerSelectedMessage : BoundUserInterfaceMessage
{
	public int ServerId;

	public ResearchClientServerSelectedMessage(int serverId)
	{
		ServerId = serverId;
	}
}
