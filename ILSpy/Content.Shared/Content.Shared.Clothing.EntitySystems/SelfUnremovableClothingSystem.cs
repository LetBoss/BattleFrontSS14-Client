using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class SelfUnremovableClothingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SelfUnremovableClothingComponent, BeingUnequippedAttemptEvent>((EntityEventRefHandler<SelfUnremovableClothingComponent, BeingUnequippedAttemptEvent>)OnUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SelfUnremovableClothingComponent, ExaminedEvent>((EntityEventRefHandler<SelfUnremovableClothingComponent, ExaminedEvent>)OnUnequipMarkup, (Type[])null, (Type[])null);
	}

	private void OnUnequip(Entity<SelfUnremovableClothingComponent> selfUnremovableClothing, ref BeingUnequippedAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		ClothingComponent clothing = default(ClothingComponent);
		if ((!((EntitySystem)this).TryComp<ClothingComponent>(Entity<SelfUnremovableClothingComponent>.op_Implicit(selfUnremovableClothing), ref clothing) || (clothing.Slots & args.SlotFlags) != SlotFlags.NONE) && args.UnEquipTarget == args.Unequipee)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUnequipMarkup(Entity<SelfUnremovableClothingComponent> selfUnremovableClothing, ref ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("comp-self-unremovable-clothing"));
	}
}
