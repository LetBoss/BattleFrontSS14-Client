using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing;

public sealed class EquipmentVisualsUpdatedEvent : EntityEventArgs
{
	public readonly EntityUid Equipee;

	public readonly string Slot;

	public HashSet<string> RevealedLayers;

	public EquipmentVisualsUpdatedEvent(EntityUid equipee, string slot, HashSet<string> revealedLayers)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Equipee = equipee;
		Slot = slot;
		RevealedLayers = revealedLayers;
	}
}
