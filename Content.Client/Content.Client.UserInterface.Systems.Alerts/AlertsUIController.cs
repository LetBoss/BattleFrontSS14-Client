using System;
using System.Collections.Generic;
using Content.Client.Alerts;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Alerts.Widgets;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.Alerts;

public sealed class AlertsUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<ClientAlertsSystem>, IOnSystemLoaded<ClientAlertsSystem>, IOnSystemUnloaded<ClientAlertsSystem>
{
	[Dependency]
	private IPlayerManager _player;

	[UISystemDependency]
	private readonly ClientAlertsSystem? _alertsSystem;

	private AlertsUI? UI => base.UIManager.GetActiveUIWidgetOrNull<AlertsUI>();

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
	}

	private void OnScreenUnload()
	{
		AlertsUI uI = UI;
		if (uI != null)
		{
			uI.AlertPressed -= OnAlertPressed;
			uI.AlertAltPressed += OnAlertAltPressed;
		}
	}

	private void OnScreenLoad()
	{
		AlertsUI uI = UI;
		if (uI != null)
		{
			uI.AlertPressed += OnAlertPressed;
			uI.AlertAltPressed += OnAlertAltPressed;
		}
		SyncAlerts();
	}

	private void OnAlertPressed(object? sender, ProtoId<AlertPrototype> e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		_alertsSystem?.AlertClicked(e);
	}

	private void OnAlertAltPressed(object? sender, ProtoId<AlertPrototype> e)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		_alertsSystem?.AlertClickedAlt(e);
	}

	private void SystemOnClearAlerts(object? sender, EventArgs e)
	{
		UI?.ClearAllControls();
	}

	private void SystemOnSyncAlerts(object? sender, IReadOnlyDictionary<AlertKey, AlertState> e)
	{
		if (sender is ClientAlertsSystem clientAlertsSystem)
		{
			UI?.SyncControls(clientAlertsSystem, clientAlertsSystem.AlertOrder, e);
		}
	}

	public void OnSystemLoaded(ClientAlertsSystem system)
	{
		system.SyncAlerts += SystemOnSyncAlerts;
		system.ClearAlerts += SystemOnClearAlerts;
	}

	public void OnSystemUnloaded(ClientAlertsSystem system)
	{
		system.SyncAlerts -= SystemOnSyncAlerts;
		system.ClearAlerts -= SystemOnClearAlerts;
	}

	public void OnStateEntered(GameplayState state)
	{
		SyncAlerts();
	}

	public void SyncAlerts()
	{
		IReadOnlyDictionary<AlertKey, AlertState> readOnlyDictionary = _alertsSystem?.ActiveAlerts;
		if (readOnlyDictionary != null)
		{
			SystemOnSyncAlerts(_alertsSystem, readOnlyDictionary);
		}
	}

	public void UpdateAlertSpriteEntity(EntityUid spriteViewEnt, AlertPrototype alert)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			SpriteComponent item = default(SpriteComponent);
			if (base.EntityManager.TryGetComponent<SpriteComponent>(spriteViewEnt, ref item))
			{
				UpdateAlertSpriteEvent updateAlertSpriteEvent = new UpdateAlertSpriteEvent(Entity<SpriteComponent>.op_Implicit((spriteViewEnt, item)), valueOrDefault, alert);
				((IDirectedEventBus)base.EntityManager.EventBus).RaiseLocalEvent<UpdateAlertSpriteEvent>(valueOrDefault, ref updateAlertSpriteEvent, false);
				((IDirectedEventBus)base.EntityManager.EventBus).RaiseLocalEvent<UpdateAlertSpriteEvent>(spriteViewEnt, ref updateAlertSpriteEvent, false);
			}
		}
	}
}
