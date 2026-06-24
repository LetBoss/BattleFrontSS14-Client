using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components;

[Serializable]
[NetSerializable]
public sealed class ResearchClientBoundInterfaceState : BoundUserInterfaceState
{
	public int ServerCount;

	public string[] ServerNames;

	public int[] ServerIds;

	public int SelectedServerId;

	public ResearchClientBoundInterfaceState(int serverCount, string[] serverNames, int[] serverIds, int selectedServerId = -1)
	{
		ServerCount = serverCount;
		ServerNames = serverNames;
		ServerIds = serverIds;
		SelectedServerId = selectedServerId;
	}
}
