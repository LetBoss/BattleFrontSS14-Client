using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public sealed class NotekeeperUiState : BoundUserInterfaceState
{
	public List<string> Notes;

	public NotekeeperUiState(List<string> notes)
	{
		Notes = notes;
	}
}
