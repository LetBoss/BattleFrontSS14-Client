using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems;

[Serializable]
[NetSerializable]
public sealed class AgentIDCardBoundUserInterfaceState : BoundUserInterfaceState
{
	public string CurrentName { get; }

	public string CurrentJob { get; }

	public string CurrentJobIconId { get; }

	public AgentIDCardBoundUserInterfaceState(string currentName, string currentJob, string currentJobIconId)
	{
		CurrentName = currentName;
		CurrentJob = currentJob;
		CurrentJobIconId = currentJobIconId;
	}
}
