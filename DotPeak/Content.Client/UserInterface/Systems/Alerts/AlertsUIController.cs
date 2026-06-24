// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Alerts.AlertsUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Alerts;

public sealed class AlertsUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnSystemChanged<ClientAlertsSystem>,
  IOnSystemLoaded<ClientAlertsSystem>,
  IOnSystemUnloaded<ClientAlertsSystem>
{
  [Dependency]
  private IPlayerManager _player;
  [UISystemDependency]
  private readonly ClientAlertsSystem? _alertsSystem;

  private AlertsUI? UI => this.UIManager.GetActiveUIWidgetOrNull<AlertsUI>();

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.OnScreenLoad);
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
  }

  private void OnScreenUnload()
  {
    AlertsUI ui = this.UI;
    if (ui == null)
      return;
    ui.AlertPressed -= new EventHandler<ProtoId<AlertPrototype>>(this.OnAlertPressed);
    ui.AlertAltPressed += new EventHandler<ProtoId<AlertPrototype>>(this.OnAlertAltPressed);
  }

  private void OnScreenLoad()
  {
    AlertsUI ui = this.UI;
    if (ui != null)
    {
      ui.AlertPressed += new EventHandler<ProtoId<AlertPrototype>>(this.OnAlertPressed);
      ui.AlertAltPressed += new EventHandler<ProtoId<AlertPrototype>>(this.OnAlertAltPressed);
    }
    this.SyncAlerts();
  }

  private void OnAlertPressed(object? sender, ProtoId<AlertPrototype> e)
  {
    this._alertsSystem?.AlertClicked(e);
  }

  private void OnAlertAltPressed(object? sender, ProtoId<AlertPrototype> e)
  {
    this._alertsSystem?.AlertClickedAlt(e);
  }

  private void SystemOnClearAlerts(object? sender, EventArgs e) => this.UI?.ClearAllControls();

  private void SystemOnSyncAlerts(object? sender, IReadOnlyDictionary<AlertKey, AlertState> e)
  {
    if (!(sender is ClientAlertsSystem clientAlertsSystem))
      return;
    this.UI?.SyncControls((AlertsSystem) clientAlertsSystem, clientAlertsSystem.AlertOrder, e);
  }

  public void OnSystemLoaded(ClientAlertsSystem system)
  {
    system.SyncAlerts += new EventHandler<IReadOnlyDictionary<AlertKey, AlertState>>(this.SystemOnSyncAlerts);
    system.ClearAlerts += new EventHandler(this.SystemOnClearAlerts);
  }

  public void OnSystemUnloaded(ClientAlertsSystem system)
  {
    system.SyncAlerts -= new EventHandler<IReadOnlyDictionary<AlertKey, AlertState>>(this.SystemOnSyncAlerts);
    system.ClearAlerts -= new EventHandler(this.SystemOnClearAlerts);
  }

  public void OnStateEntered(GameplayState state) => this.SyncAlerts();

  public void SyncAlerts()
  {
    IReadOnlyDictionary<AlertKey, AlertState> activeAlerts = this._alertsSystem?.ActiveAlerts;
    if (activeAlerts == null)
      return;
    this.SystemOnSyncAlerts((object) this._alertsSystem, activeAlerts);
  }

  public void UpdateAlertSpriteEntity(EntityUid spriteViewEnt, AlertPrototype alert)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    SpriteComponent spriteComponent;
    if (!this.EntityManager.TryGetComponent<SpriteComponent>(spriteViewEnt, ref spriteComponent))
      return;
    UpdateAlertSpriteEvent alertSpriteEvent = new UpdateAlertSpriteEvent(Entity<SpriteComponent>.op_Implicit((spriteViewEnt, spriteComponent)), valueOrDefault, alert);
    ((IDirectedEventBus) this.EntityManager.EventBus).RaiseLocalEvent<UpdateAlertSpriteEvent>(valueOrDefault, ref alertSpriteEvent, false);
    ((IDirectedEventBus) this.EntityManager.EventBus).RaiseLocalEvent<UpdateAlertSpriteEvent>(spriteViewEnt, ref alertSpriteEvent, false);
  }
}
