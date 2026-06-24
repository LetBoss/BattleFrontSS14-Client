using System;
using Content.Shared.Alert;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.TacticalMap;

public sealed class TacMapMarineAlertSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private InventorySystem _inv;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GrantTacMapAlertComponent, GotEquippedEvent>((EntityEventRefHandler<GrantTacMapAlertComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantTacMapAlertComponent, GotUnequippedEvent>((EntityEventRefHandler<GrantTacMapAlertComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacMapMarineAlertComponent, MapInitEvent>((EntityEventRefHandler<TacMapMarineAlertComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacMapMarineAlertComponent, ComponentRemove>((EntityEventRefHandler<TacMapMarineAlertComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
	}

	private void OnGotEquipped(Entity<GrantTacMapAlertComponent> ent, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			((EntitySystem)this).EnsureComp<TacMapMarineAlertComponent>(args.Equipee);
		}
	}

	private void OnGotUnequipped(Entity<GrantTacMapAlertComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE && !_inv.TryGetInventoryEntity<GrantTacMapAlertComponent>(Entity<InventoryComponent>.op_Implicit(args.Equipee), out Entity<GrantTacMapAlertComponent> _))
		{
			((EntitySystem)this).RemCompDeferred<TacMapMarineAlertComponent>(args.Equipee);
		}
	}

	private void OnMapInit(Entity<TacMapMarineAlertComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ShowAlert(Entity<TacMapMarineAlertComponent>.op_Implicit(ent), ent.Comp.Alert);
	}

	private void OnRemove(Entity<TacMapMarineAlertComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<TacMapMarineAlertComponent>.op_Implicit(ent), ent.Comp.Alert);
	}
}
