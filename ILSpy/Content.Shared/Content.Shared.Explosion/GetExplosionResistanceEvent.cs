using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Explosion;

[ByRefEvent]
public record struct GetExplosionResistanceEvent(string ExplosionPrototype) : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

	public float DamageCoefficient = 1f;

	public readonly string ExplosionPrototype = ExplosionPrototype;

	[CompilerGenerated]
	public readonly void Deconstruct(out string ExplosionPrototype)
	{
		ExplosionPrototype = this.ExplosionPrototype;
	}
}
