using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

[ByRefEvent]
public struct GetAmmoCountEvent
{
	public int Count;

	public int Capacity;
}
