using System;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.TacticalMap;

public sealed class TacMapXenoAlertSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TacMapXenoAlertComponent, MapInitEvent>((EntityEventRefHandler<TacMapXenoAlertComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TacMapXenoAlertComponent, ComponentRemove>((EntityEventRefHandler<TacMapXenoAlertComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<TacMapXenoAlertComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ShowAlert(Entity<TacMapXenoAlertComponent>.op_Implicit(ent), ent.Comp.Alert);
	}

	private void OnRemove(Entity<TacMapXenoAlertComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<TacMapXenoAlertComponent>.op_Implicit(ent), ent.Comp.Alert);
	}
}
