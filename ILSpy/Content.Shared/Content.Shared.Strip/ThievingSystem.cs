using System;
using Content.Shared.Alert;
using Content.Shared.Inventory;
using Content.Shared.Strip.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Strip;

public sealed class ThievingSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alertsSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ThievingComponent, BeforeStripEvent>((ComponentEventHandler<ThievingComponent, BeforeStripEvent>)OnBeforeStrip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThievingComponent, InventoryRelayedEvent<BeforeStripEvent>>((ComponentEventHandler<ThievingComponent, InventoryRelayedEvent<BeforeStripEvent>>)delegate(EntityUid e, ThievingComponent c, InventoryRelayedEvent<BeforeStripEvent> ev)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			OnBeforeStrip(e, c, ev.Args);
		}, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThievingComponent, ToggleThievingEvent>((EntityEventRefHandler<ThievingComponent, ToggleThievingEvent>)OnToggleStealthy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThievingComponent, ComponentInit>((EntityEventRefHandler<ThievingComponent, ComponentInit>)OnCompInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ThievingComponent, ComponentRemove>((EntityEventRefHandler<ThievingComponent, ComponentRemove>)OnCompRemoved, (Type[])null, (Type[])null);
	}

	private void OnBeforeStrip(EntityUid uid, ThievingComponent component, BeforeStripEvent args)
	{
		args.Stealth |= component.Stealthy;
		if (args.Stealth)
		{
			args.Additive -= component.StripTimeReduction;
		}
	}

	private void OnCompInit(Entity<ThievingComponent> entity, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alertsSystem.ShowAlert(Entity<ThievingComponent>.op_Implicit(entity), entity.Comp.StealthyAlertProtoId, (short)1);
	}

	private void OnCompRemoved(Entity<ThievingComponent> entity, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alertsSystem.ClearAlert(Entity<ThievingComponent>.op_Implicit(entity), entity.Comp.StealthyAlertProtoId);
	}

	private void OnToggleStealthy(Entity<ThievingComponent> ent, ref ToggleThievingEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			ent.Comp.Stealthy = !ent.Comp.Stealthy;
			_alertsSystem.ShowAlert(ent.Owner, ent.Comp.StealthyAlertProtoId, ent.Comp.Stealthy ? ((short)1) : ((short)0));
			((EntitySystem)this).DirtyField<ThievingComponent>(ent.AsNullable(), "Stealthy", (MetaDataComponent)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
