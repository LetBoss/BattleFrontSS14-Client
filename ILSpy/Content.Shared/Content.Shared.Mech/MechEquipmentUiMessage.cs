using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech;

[Serializable]
[NetSerializable]
public abstract class MechEquipmentUiMessage : BoundUserInterfaceMessage
{
	public NetEntity Equipment;
}
