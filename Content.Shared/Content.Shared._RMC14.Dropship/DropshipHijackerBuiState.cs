using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[Serializable]
[NetSerializable]
public sealed class DropshipHijackerBuiState(List<(NetEntity Id, string Name)> destinations) : BoundUserInterfaceState
{
	public List<(NetEntity Id, string Name)> Destinations = destinations;
}
