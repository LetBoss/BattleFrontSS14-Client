using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle.Weapons;

[Serializable]
[NetSerializable]
public sealed class ShellSelectionUiState : BoundUserInterfaceState
{
	public readonly List<ShellTypeEntry> AvailableShells;

	public readonly EntProtoId CurrentSelection;

	public ShellSelectionUiState(List<ShellTypeEntry> availableShells, EntProtoId currentSelection)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		AvailableShells = availableShells;
		CurrentSelection = currentSelection;
	}
}
