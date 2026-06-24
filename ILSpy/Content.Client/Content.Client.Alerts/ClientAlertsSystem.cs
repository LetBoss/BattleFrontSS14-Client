using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Alert;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Alerts;

public sealed class ClientAlertsSystem : AlertsSystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IUserInterfaceManager _ui;

	public AlertOrderPrototype? AlertOrder { get; set; }

	public IReadOnlyDictionary<AlertKey, AlertState>? ActiveAlerts
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (!localEntity.HasValue)
			{
				return null;
			}
			return GetActiveAlerts(localEntity.Value);
		}
	}

	public event EventHandler? ClearAlerts;

	public event EventHandler<IReadOnlyDictionary<AlertKey, AlertState>>? SyncAlerts;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<AlertsComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<AlertsComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, ComponentHandleState>((EntityEventRefHandler<AlertsComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	protected override void HandledAlert()
	{
		_ui.ClickSound();
	}

	protected override void LoadPrototypes()
	{
		base.LoadPrototypes();
		AlertOrder = _prototypeManager.EnumeratePrototypes<AlertOrderPrototype>().FirstOrDefault();
		if (AlertOrder == null)
		{
			((EntitySystem)this).Log.Error("No alertOrder prototype found, alerts will be in random order");
		}
	}

	private void OnHandleState(Entity<AlertsComponent> alerts, ref ComponentHandleState args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is AlertComponentState alertComponentState))
		{
			return;
		}
		Dictionary<AlertKey, AlertState> dictionary = new Dictionary<AlertKey, AlertState>();
		foreach (KeyValuePair<AlertKey, AlertState> alert2 in alerts.Comp.Alerts)
		{
			if (alert2.Key.AlertType.HasValue && TryGet(alert2.Key.AlertType.Value, out AlertPrototype alert) && alert.ClientHandled)
			{
				dictionary[alert2.Key] = alert2.Value;
			}
		}
		alerts.Comp.Alerts = new Dictionary<AlertKey, AlertState>(alertComponentState.Alerts);
		foreach (KeyValuePair<AlertKey, AlertState> item in dictionary)
		{
			alerts.Comp.Alerts[item.Key] = item.Value;
		}
		UpdateHud(alerts);
	}

	protected override void AfterShowAlert(Entity<AlertsComponent> alerts)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateHud(alerts);
	}

	protected override void AfterClearAlert(Entity<AlertsComponent> alerts)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateHud(alerts);
	}

	private void UpdateHud(Entity<AlertsComponent> entity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		EntityUid owner = entity.Owner;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == owner)
		{
			this.SyncAlerts?.Invoke(this, entity.Comp.Alerts);
		}
	}

	private void OnPlayerAttached(EntityUid uid, AlertsComponent component, LocalPlayerAttachedEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != uid))
		{
			this.SyncAlerts?.Invoke(this, component.Alerts);
		}
	}

	protected override void HandleComponentShutdown(EntityUid uid, AlertsComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.HandleComponentShutdown(uid, component, args);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != uid))
		{
			this.ClearAlerts?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnPlayerDetached(EntityUid uid, AlertsComponent component, LocalPlayerDetachedEvent args)
	{
		this.ClearAlerts?.Invoke(this, EventArgs.Empty);
	}

	public void AlertClicked(ProtoId<AlertPrototype> alertType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaisePredictiveEvent<ClickAlertEvent>(new ClickAlertEvent(alertType));
	}

	public void AlertClickedAlt(ProtoId<AlertPrototype> alertType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaisePredictiveEvent<ClickAlertAltEvent>(new ClickAlertAltEvent(alertType));
	}
}
