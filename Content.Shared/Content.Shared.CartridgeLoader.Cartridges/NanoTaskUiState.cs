using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public sealed class NanoTaskUiState : BoundUserInterfaceState
{
	public List<NanoTaskItemAndId> Tasks;

	public NanoTaskUiState(List<NanoTaskItemAndId> tasks)
	{
		Tasks = tasks;
	}
}
