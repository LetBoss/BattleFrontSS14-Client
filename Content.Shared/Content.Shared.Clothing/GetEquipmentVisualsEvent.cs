using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing;

public sealed class GetEquipmentVisualsEvent : EntityEventArgs
{
	public readonly EntityUid Equipee;

	public readonly string Slot;

	public List<(string, PrototypeLayerData)> Layers = new List<(string, PrototypeLayerData)>();

	public GetEquipmentVisualsEvent(EntityUid equipee, string slot)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Equipee = equipee;
		Slot = slot;
	}
}
