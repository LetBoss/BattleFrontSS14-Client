using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Atmos;

[ByRefEvent]
public sealed class GetIgnitionImmunityEvent : EntityEventArgs, IInventoryRelayEvent
{
	public bool Ignite;

	public bool DirectHit;

	public int Intensity;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public GetIgnitionImmunityEvent(int intensity, bool directHit)
	{
		Ignite = true;
		DirectHit = directHit;
		Intensity = intensity;
	}
}
