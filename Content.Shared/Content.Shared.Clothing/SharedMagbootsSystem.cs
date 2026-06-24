using System;
using Content.Shared.Alert;
using Content.Shared.Atmos.Components;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Clothing;

public sealed class SharedMagbootsSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedGravitySystem _gravity;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MagbootsComponent, ItemToggledEvent>((EntityEventRefHandler<MagbootsComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagbootsComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<MagbootsComponent, ClothingGotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagbootsComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<MagbootsComponent, ClothingGotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagbootsComponent, IsWeightlessEvent>((EntityEventRefHandler<MagbootsComponent, IsWeightlessEvent>)OnIsWeightless, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagbootsComponent, InventoryRelayedEvent<IsWeightlessEvent>>((EntityEventRefHandler<MagbootsComponent, InventoryRelayedEvent<IsWeightlessEvent>>)OnIsWeightless, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<MagbootsComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Entity<MagbootsComponent> val = ent;
		MagbootsComponent magbootsComponent = default(MagbootsComponent);
		EntityUid val2 = default(EntityUid);
		val.Deconstruct(ref val2, ref magbootsComponent);
		EntityUid uid = val2;
		MagbootsComponent comp = magbootsComponent;
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container) && _inventory.TryGetSlotEntity(container.Owner, comp.Slot, out var worn))
		{
			val2 = uid;
			EntityUid? val3 = worn;
			if (val3.HasValue && val2 == val3.GetValueOrDefault())
			{
				UpdateMagbootEffects(container.Owner, ent, args.Activated);
			}
		}
	}

	private void OnGotUnequipped(Entity<MagbootsComponent> ent, ref ClothingGotUnequippedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateMagbootEffects(args.Wearer, ent, state: false);
	}

	private void OnGotEquipped(Entity<MagbootsComponent> ent, ref ClothingGotEquippedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		UpdateMagbootEffects(args.Wearer, ent, _toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)));
	}

	public void UpdateMagbootEffects(EntityUid user, Entity<MagbootsComponent> ent, bool state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		MovedByPressureComponent moved = default(MovedByPressureComponent);
		if (((EntitySystem)this).TryComp<MovedByPressureComponent>(user, ref moved))
		{
			moved.Enabled = !state;
		}
		if (state)
		{
			_alerts.ShowAlert(user, ent.Comp.MagbootsAlert);
		}
		else
		{
			_alerts.ClearAlert(user, ent.Comp.MagbootsAlert);
		}
	}

	private void OnIsWeightless(Entity<MagbootsComponent> ent, ref IsWeightlessEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && _toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)) && (!ent.Comp.RequiresGrid || _gravity.EntityOnGravitySupportingGridOrMap(Entity<TransformComponent>.op_Implicit(ent.Owner))))
		{
			args.IsWeightless = false;
			args.Handled = true;
		}
	}

	private void OnIsWeightless(Entity<MagbootsComponent> ent, ref InventoryRelayedEvent<IsWeightlessEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnIsWeightless(ent, ref args.Args);
	}
}
