using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components;

[Serializable]
[NetSerializable]
public sealed class ConsoleUnlockTechnologyMessage : BoundUserInterfaceMessage
{
	public string Id;

	public ConsoleUnlockTechnologyMessage(string id)
	{
		Id = id;
	}
}
