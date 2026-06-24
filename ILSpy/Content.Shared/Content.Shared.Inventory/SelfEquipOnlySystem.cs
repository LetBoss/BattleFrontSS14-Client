using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Inventory;

public sealed class SelfEquipOnlySystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SelfEquipOnlyComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<SelfEquipOnlyComponent, BeingEquippedAttemptEvent>)OnBeingEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SelfEquipOnlyComponent, BeingUnequippedAttemptEvent>((EntityEventRefHandler<SelfEquipOnlyComponent, BeingUnequippedAttemptEvent>)OnBeingUnequipped, (Type[])null, (Type[])null);
	}

	private void OnBeingEquipped(Entity<SelfEquipOnlyComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ClothingComponent clothing = default(ClothingComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && (!((EntitySystem)this).TryComp<ClothingComponent>(Entity<SelfEquipOnlyComponent>.op_Implicit(ent), ref clothing) || (clothing.Slots & args.SlotFlags) != SlotFlags.NONE) && args.Equipee != args.EquipTarget)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeingUnequipped(Entity<SelfEquipOnlyComponent> ent, ref BeingUnequippedAttemptEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		ClothingComponent clothing = default(ClothingComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && !(args.Unequipee == args.UnEquipTarget) && (!((EntitySystem)this).TryComp<ClothingComponent>(Entity<SelfEquipOnlyComponent>.op_Implicit(ent), ref clothing) || (clothing.Slots & args.SlotFlags) != SlotFlags.NONE) && (!ent.Comp.UnequipRequireConscious || _actionBlocker.CanConsciouslyPerformAction(args.UnEquipTarget)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
