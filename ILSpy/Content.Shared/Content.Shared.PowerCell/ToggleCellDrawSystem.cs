using System;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.PowerCell.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PowerCell;

public sealed class ToggleCellDrawSystem : EntitySystem
{
	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedPowerCellSystem _cell;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToggleCellDrawComponent, MapInitEvent>((EntityEventRefHandler<ToggleCellDrawComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleCellDrawComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<ToggleCellDrawComponent, ItemToggleActivateAttemptEvent>)OnActivateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleCellDrawComponent, ItemToggledEvent>((EntityEventRefHandler<ToggleCellDrawComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleCellDrawComponent, PowerCellSlotEmptyEvent>((EntityEventRefHandler<ToggleCellDrawComponent, PowerCellSlotEmptyEvent>)OnEmpty, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ToggleCellDrawComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_cell.SetDrawEnabled(Entity<PowerCellDrawComponent>.op_Implicit(ent.Owner), _toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)));
	}

	private void OnActivateAttempt(Entity<ToggleCellDrawComponent> ent, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!_cell.HasDrawCharge(Entity<ToggleCellDrawComponent>.op_Implicit(ent), null, null, args.User) || !_cell.HasActivatableCharge(Entity<ToggleCellDrawComponent>.op_Implicit(ent), null, null, args.User))
		{
			args.Cancelled = true;
		}
	}

	private void OnToggled(Entity<ToggleCellDrawComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = ent.Owner;
		PowerCellDrawComponent draw = ((EntitySystem)this).Comp<PowerCellDrawComponent>(uid);
		_cell.SetDrawEnabled(Entity<PowerCellDrawComponent>.op_Implicit((uid, draw)), args.Activated);
	}

	private void OnEmpty(Entity<ToggleCellDrawComponent> ent, ref PowerCellSlotEmptyEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(ent.Owner));
	}
}
