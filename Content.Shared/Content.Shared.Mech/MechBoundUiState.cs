using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech;

[Serializable]
[NetSerializable]
public sealed class MechBoundUiState : BoundUserInterfaceState
{
	public Dictionary<NetEntity, BoundUserInterfaceState> EquipmentStates = new Dictionary<NetEntity, BoundUserInterfaceState>();
}
