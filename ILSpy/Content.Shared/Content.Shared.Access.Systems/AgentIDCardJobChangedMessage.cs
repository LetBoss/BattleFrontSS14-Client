using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems;

[Serializable]
[NetSerializable]
public sealed class AgentIDCardJobChangedMessage : BoundUserInterfaceMessage
{
	public string Job { get; }

	public AgentIDCardJobChangedMessage(string job)
	{
		Job = job;
	}
}
