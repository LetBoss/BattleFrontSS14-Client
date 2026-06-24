using System;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._CIV14merka.GlobalMap;

public sealed class CivGlobalMapAlertSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CivGlobalMapAlertComponent, MapInitEvent>((EntityEventRefHandler<CivGlobalMapAlertComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivGlobalMapAlertComponent, ComponentRemove>((EntityEventRefHandler<CivGlobalMapAlertComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<CivGlobalMapAlertComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ShowAlert(Entity<CivGlobalMapAlertComponent>.op_Implicit(ent), ent.Comp.Alert);
	}

	private void OnRemove(Entity<CivGlobalMapAlertComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<CivGlobalMapAlertComponent>.op_Implicit(ent), ent.Comp.Alert);
	}
}
