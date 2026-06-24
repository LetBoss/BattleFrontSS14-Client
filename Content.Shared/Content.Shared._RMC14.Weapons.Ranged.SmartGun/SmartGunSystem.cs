using System;
using Content.Shared.PowerCell.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Weapons.Ranged.SmartGun;

public sealed class SmartGunSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SmartGunBatteryComponent, ContainerGettingInsertedAttemptEvent>((EntityEventRefHandler<SmartGunBatteryComponent, ContainerGettingInsertedAttemptEvent>)OnBatteryInsertedAttempt, (Type[])null, (Type[])null);
	}

	private void OnBatteryInsertedAttempt(Entity<SmartGunBatteryComponent> ent, ref ContainerGettingInsertedAttemptEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			BaseContainer container = ((ContainerAttemptEventBase)args).Container;
			PowerCellSlotComponent slot = default(PowerCellSlotComponent);
			if (((EntitySystem)this).TryComp<PowerCellSlotComponent>(container.Owner, ref slot) && container.ID == slot.CellSlotId && !((EntitySystem)this).HasComp<SmartGunComponent>(container.Owner))
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}
}
