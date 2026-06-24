using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems;

[Serializable]
[NetSerializable]
public sealed class AgentIDCardNameChangedMessage : BoundUserInterfaceMessage
{
	public string Name { get; }

	public AgentIDCardNameChangedMessage(string name)
	{
		Name = name;
	}
}
