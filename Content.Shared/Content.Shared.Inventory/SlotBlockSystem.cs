using System;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory;

public sealed class SlotBlockSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SlotBlockComponent, InventoryRelayedEvent<IsEquippingTargetAttemptEvent>>((EntityEventRefHandler<SlotBlockComponent, InventoryRelayedEvent<IsEquippingTargetAttemptEvent>>)OnEquipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlotBlockComponent, InventoryRelayedEvent<IsUnequippingTargetAttemptEvent>>((EntityEventRefHandler<SlotBlockComponent, InventoryRelayedEvent<IsUnequippingTargetAttemptEvent>>)OnUnequipAttempt, (Type[])null, (Type[])null);
	}

	private void OnEquipAttempt(Entity<SlotBlockComponent> ent, ref InventoryRelayedEvent<IsEquippingTargetAttemptEvent> args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args.Args).Cancelled && (args.Args.SlotFlags & ent.Comp.Slots) != SlotFlags.NONE)
		{
			args.Args.Reason = base.Loc.GetString("slot-block-component-blocked", (ValueTuple<string, object>)("item", ent));
			((CancellableEntityEventArgs)args.Args).Cancel();
		}
	}

	private void OnUnequipAttempt(Entity<SlotBlockComponent> ent, ref InventoryRelayedEvent<IsUnequippingTargetAttemptEvent> args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args.Args).Cancelled && (args.Args.SlotFlags & ent.Comp.Slots) != SlotFlags.NONE)
		{
			args.Args.Reason = base.Loc.GetString("slot-block-component-blocked", (ValueTuple<string, object>)("item", ent));
			((CancellableEntityEventArgs)args.Args).Cancel();
		}
	}
}
