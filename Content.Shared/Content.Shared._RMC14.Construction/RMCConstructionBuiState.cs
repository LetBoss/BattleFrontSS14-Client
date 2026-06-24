using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Construction;

[Serializable]
[NetSerializable]
public sealed class RMCConstructionBuiState(string stackAmount) : BoundUserInterfaceState
{
	public readonly string StackAmount = stackAmount;
}
