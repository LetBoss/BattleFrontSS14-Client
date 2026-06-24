using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG.FogOfWar;

public sealed class GetPubgFovEvent : EntityEventArgs, IInventoryRelayEvent
{
	public float Modifier;

	public float BaseFov { get; }

	public SlotFlags TargetSlots => SlotFlags.HEAD;

	public GetPubgFovEvent(float baseFov)
	{
		BaseFov = baseFov;
	}
}
