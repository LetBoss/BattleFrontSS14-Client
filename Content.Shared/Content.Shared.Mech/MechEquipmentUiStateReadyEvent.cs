using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech;

public sealed class MechEquipmentUiStateReadyEvent : EntityEventArgs
{
	public Dictionary<NetEntity, BoundUserInterfaceState> States = new Dictionary<NetEntity, BoundUserInterfaceState>();
}
