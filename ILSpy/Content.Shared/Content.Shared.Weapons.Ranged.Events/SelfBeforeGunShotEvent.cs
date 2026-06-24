using System.Collections.Generic;
using Content.Shared.Inventory;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events;

public sealed class SelfBeforeGunShotEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
	public readonly EntityUid Shooter;

	public readonly Entity<GunComponent> Gun;

	public readonly List<(EntityUid? Entity, IShootable Shootable)> Ammo;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public SelfBeforeGunShotEvent(EntityUid shooter, Entity<GunComponent> gun, List<(EntityUid? Entity, IShootable Shootable)> ammo)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Shooter = shooter;
		Gun = gun;
		Ammo = ammo;
	}
}
