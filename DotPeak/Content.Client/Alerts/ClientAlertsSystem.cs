// Decompiled with JetBrains decompiler
// Type: Content.Client.Alerts.ClientAlertsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Alert;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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

  public event EventHandler? ClearAlerts;

  public event EventHandler<IReadOnlyDictionary<AlertKey, AlertState>>? SyncAlerts;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertsComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<AlertsComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertsComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<AlertsComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertsComponent, ComponentHandleState>(new EntityEventRefHandler<AlertsComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  protected override void HandledAlert() => this._ui.ClickSound();

  protected override void LoadPrototypes()
  {
    base.LoadPrototypes();
    this.AlertOrder = this._prototypeManager.EnumeratePrototypes<AlertOrderPrototype>().FirstOrDefault<AlertOrderPrototype>();
    if (this.AlertOrder != null)
      return;
    this.Log.Error("No alertOrder prototype found, alerts will be in random order");
  }

  public IReadOnlyDictionary<AlertKey, AlertState>? ActiveAlerts
  {
    get
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
      return !localEntity.HasValue ? (IReadOnlyDictionary<AlertKey, AlertState>) null : this.GetActiveAlerts(localEntity.Value);
    }
  }

  private void OnHandleState(Entity<AlertsComponent> alerts, ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is AlertComponentState current))
      return;
    Dictionary<AlertKey, AlertState> dictionary = new Dictionary<AlertKey, AlertState>();
    foreach (KeyValuePair<AlertKey, AlertState> alert1 in alerts.Comp.Alerts)
    {
      AlertKey key = alert1.Key;
      ProtoId<AlertPrototype>? alertType = key.AlertType;
      if (alertType.HasValue)
      {
        key = alert1.Key;
        alertType = key.AlertType;
        AlertPrototype alert2;
        if (this.TryGet(alertType.Value, out alert2) && alert2.ClientHandled)
          dictionary[alert1.Key] = alert1.Value;
      }
    }
    alerts.Comp.Alerts = new Dictionary<AlertKey, AlertState>((IDictionary<AlertKey, AlertState>) current.Alerts);
    foreach (KeyValuePair<AlertKey, AlertState> keyValuePair in dictionary)
      alerts.Comp.Alerts[keyValuePair.Key] = keyValuePair.Value;
    this.UpdateHud(alerts);
  }

  protected override void AfterShowAlert(Entity<AlertsComponent> alerts) => this.UpdateHud(alerts);

  protected override void AfterClearAlert(Entity<AlertsComponent> alerts) => this.UpdateHud(alerts);

  private void UpdateHud(Entity<AlertsComponent> entity)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid owner = entity.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 0) == 0)
      return;
    EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> syncAlerts = this.SyncAlerts;
    if (syncAlerts == null)
      return;
    syncAlerts((object) this, (IReadOnlyDictionary<AlertKey, AlertState>) entity.Comp.Alerts);
  }

  private void OnPlayerAttached(
    EntityUid uid,
    AlertsComponent component,
    LocalPlayerAttachedEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
      return;
    EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> syncAlerts = this.SyncAlerts;
    if (syncAlerts == null)
      return;
    syncAlerts((object) this, (IReadOnlyDictionary<AlertKey, AlertState>) component.Alerts);
  }

  protected override void HandleComponentShutdown(
    EntityUid uid,
    AlertsComponent component,
    ComponentShutdown args)
  {
    base.HandleComponentShutdown(uid, component, args);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
      return;
    EventHandler clearAlerts = this.ClearAlerts;
    if (clearAlerts == null)
      return;
    clearAlerts((object) this, EventArgs.Empty);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    AlertsComponent component,
    LocalPlayerDetachedEvent args)
  {
    EventHandler clearAlerts = this.ClearAlerts;
    if (clearAlerts == null)
      return;
    clearAlerts((object) this, EventArgs.Empty);
  }

  public void AlertClicked(ProtoId<AlertPrototype> alertType)
  {
    this.RaisePredictiveEvent<ClickAlertEvent>(new ClickAlertEvent(alertType));
  }

  public void AlertClickedAlt(ProtoId<AlertPrototype> alertType)
  {
    this.RaisePredictiveEvent<ClickAlertAltEvent>(new ClickAlertAltEvent(alertType));
  }
}
