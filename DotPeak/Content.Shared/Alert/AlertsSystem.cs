// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.AlertsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Alert;

public abstract class AlertsSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private FrozenDictionary<ProtoId<AlertPrototype>, AlertPrototype> _typeToAlert;

  public IReadOnlyDictionary<AlertKey, AlertState>? GetActiveAlerts(EntityUid euid)
  {
    AlertsComponent alertsComponent;
    return !this.TryComp<AlertsComponent>(euid, ref alertsComponent) ? (IReadOnlyDictionary<AlertKey, AlertState>) null : (IReadOnlyDictionary<AlertKey, AlertState>) alertsComponent.Alerts;
  }

  public short GetSeverityRange(ProtoId<AlertPrototype> alertType)
  {
    short minSeverity = this._typeToAlert[alertType].MinSeverity;
    return (short) MathF.Max((float) minSeverity, (float) ((int) this._typeToAlert[alertType].MaxSeverity - (int) minSeverity));
  }

  public short GetMaxSeverity(ProtoId<AlertPrototype> alertType)
  {
    return this._typeToAlert[alertType].MaxSeverity;
  }

  public short GetMinSeverity(ProtoId<AlertPrototype> alertType)
  {
    return this._typeToAlert[alertType].MinSeverity;
  }

  public bool IsShowingAlert(EntityUid euid, ProtoId<AlertPrototype> alertType)
  {
    AlertsComponent alertsComponent;
    if (!this.TryComp<AlertsComponent>(euid, ref alertsComponent))
      return false;
    AlertPrototype alert;
    if (this.TryGet(alertType, out alert))
      return alertsComponent.Alerts.ContainsKey(alert.AlertKey);
    this.Log.Debug("Unknown alert type {0}", new object[1]
    {
      (object) alertType
    });
    return false;
  }

  public bool IsShowingAlertCategory(EntityUid euid, ProtoId<AlertCategoryPrototype> alertCategory)
  {
    AlertsComponent alertsComponent;
    return this.TryComp<AlertsComponent>(euid, ref alertsComponent) && alertsComponent.Alerts.ContainsKey(AlertKey.ForCategory(alertCategory));
  }

  public bool TryGetAlertState(EntityUid euid, AlertKey key, out AlertState alertState)
  {
    AlertsComponent alertsComponent;
    if (this.TryComp<AlertsComponent>(euid, ref alertsComponent))
      return alertsComponent.Alerts.TryGetValue(key, out alertState);
    alertState = new AlertState();
    return false;
  }

  public void ShowAlert(
    EntityUid euid,
    ProtoId<AlertPrototype> alertType,
    short? severity = null,
    (TimeSpan, TimeSpan)? cooldown = null,
    bool autoRemove = false,
    bool showCooldown = true,
    string? dynamicMessage = null)
  {
    AlertsComponent alertsComponent;
    if (this._timing.ApplyingState || !this.TryComp<AlertsComponent>(euid, ref alertsComponent))
      return;
    AlertPrototype alert;
    if (this.TryGet(alertType, out alert))
    {
      AlertState alertState1;
      if (alertsComponent.Alerts.TryGetValue(alert.AlertKey, out alertState1) && ProtoId<AlertPrototype>.op_Equality(alertState1.Type, alertType))
      {
        short? nullable1 = alertState1.Severity;
        int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        nullable1 = severity;
        int? nullable3 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        if (nullable2.GetValueOrDefault() == nullable3.GetValueOrDefault() & nullable2.HasValue == nullable3.HasValue)
        {
          (TimeSpan, TimeSpan)? cooldown1 = alertState1.Cooldown;
          (TimeSpan, TimeSpan)? nullable4 = cooldown;
          bool hasValue = cooldown1.HasValue;
          int num;
          if (hasValue != nullable4.HasValue)
            num = 0;
          else if (!hasValue)
          {
            num = 1;
          }
          else
          {
            (TimeSpan, TimeSpan) valueOrDefault1 = cooldown1.GetValueOrDefault();
            (TimeSpan, TimeSpan) valueOrDefault2 = nullable4.GetValueOrDefault();
            num = !(valueOrDefault1.Item1 == valueOrDefault2.Item1) ? 0 : (valueOrDefault1.Item2 == valueOrDefault2.Item2 ? 1 : 0);
          }
          if (num != 0 && alertState1.AutoRemove == autoRemove && alertState1.ShowCooldown == showCooldown && alertState1.DynamicMessage == dynamicMessage)
            return;
        }
      }
      alertsComponent.Alerts.Remove(alert.AlertKey);
      AlertState alertState2 = new AlertState()
      {
        Cooldown = cooldown,
        Severity = severity,
        Type = alertType,
        AutoRemove = autoRemove,
        ShowCooldown = showCooldown,
        DynamicMessage = dynamicMessage
      };
      alertsComponent.Alerts[alert.AlertKey] = alertState2;
      if (autoRemove)
      {
        AlertAutoRemoveComponent autoRemoveComponent = this.EnsureComp<AlertAutoRemoveComponent>(euid);
        if (!autoRemoveComponent.AlertKeys.Contains(alert.AlertKey))
          autoRemoveComponent.AlertKeys.Add(alert.AlertKey);
      }
      this.AfterShowAlert(Entity<AlertsComponent>.op_Implicit((euid, alertsComponent)));
      this.Dirty(euid, (IComponent) alertsComponent, (MetaDataComponent) null);
    }
    else
      this.Log.Error("Unable to show alert {0}, please ensure this alertType has a corresponding YML alert prototype", new object[1]
      {
        (object) alertType
      });
  }

  public void ClearAlertCategory(EntityUid euid, ProtoId<AlertCategoryPrototype> category)
  {
    AlertsComponent alertsComponent;
    if (!this.TryComp<AlertsComponent>(euid, ref alertsComponent))
      return;
    AlertKey key = AlertKey.ForCategory(category);
    if (!alertsComponent.Alerts.Remove(key))
      return;
    this.AfterClearAlert(Entity<AlertsComponent>.op_Implicit((euid, alertsComponent)));
    this.Dirty(euid, (IComponent) alertsComponent, (MetaDataComponent) null);
  }

  public void ClearAlert(EntityUid euid, ProtoId<AlertPrototype> alertType)
  {
    AlertsComponent alertsComponent;
    if (this._timing.ApplyingState || !this.TryComp<AlertsComponent>(euid, ref alertsComponent))
      return;
    AlertPrototype alert;
    if (this.TryGet(alertType, out alert))
    {
      if (!alertsComponent.Alerts.Remove(alert.AlertKey))
        return;
      this.AfterClearAlert(Entity<AlertsComponent>.op_Implicit((euid, alertsComponent)));
      this.Dirty(euid, (IComponent) alertsComponent, (MetaDataComponent) null);
    }
    else
      this.Log.Error("Unable to clear alert, unknown alertType {0}", new object[1]
      {
        (object) alertType
      });
  }

  protected virtual void AfterShowAlert(Entity<AlertsComponent> alerts)
  {
  }

  protected virtual void AfterClearAlert(Entity<AlertsComponent> alerts)
  {
  }

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertsComponent, ComponentStartup>(new ComponentEventHandler<AlertsComponent, ComponentStartup>((object) this, __methodptr(HandleComponentStartup)), (Type[]) null, (Type[]) null);
    AlertsSystem alertsSystem = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<AlertsComponent, ComponentShutdown>(new ComponentEventHandler<AlertsComponent, ComponentShutdown>((object) alertsSystem, __vmethodptr(alertsSystem, HandleComponentShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertsComponent, PlayerAttachedEvent>(new ComponentEventHandler<AlertsComponent, PlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertAutoRemoveComponent, EntityUnpausedEvent>(new ComponentEventHandler<AlertAutoRemoveComponent, EntityUnpausedEvent>((object) this, __methodptr(OnAutoRemoveUnPaused)), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<ClickAlertEvent>(new EntitySessionEventHandler<ClickAlertEvent>(this.HandleClickAlert), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<ClickAlertAltEvent>(new EntitySessionEventHandler<ClickAlertAltEvent>(this.HandleClickAlertAlt), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.HandlePrototypesReloaded), (Type[]) null, (Type[]) null);
    this.LoadPrototypes();
  }

  private void OnAutoRemoveUnPaused(
    EntityUid uid,
    AlertAutoRemoveComponent comp,
    EntityUnpausedEvent args)
  {
    AlertsComponent alertsComponent;
    if (!this.TryComp<AlertsComponent>(uid, ref alertsComponent))
      return;
    bool flag = false;
    foreach (KeyValuePair<AlertKey, AlertState> alert in alertsComponent.Alerts)
    {
      if (alert.Value.Cooldown.HasValue)
      {
        (TimeSpan, TimeSpan) valueTuple;
        ref (TimeSpan, TimeSpan) local = ref valueTuple;
        AlertState alertState1 = alert.Value;
        TimeSpan timeSpan1 = alertState1.Cooldown.Value.Item1;
        alertState1 = alert.Value;
        TimeSpan timeSpan2 = alertState1.Cooldown.Value.Item2 + args.PausedTime;
        local = (timeSpan1, timeSpan2);
        alertState1 = new AlertState();
        alertState1.Severity = alert.Value.Severity;
        alertState1.Cooldown = new (TimeSpan, TimeSpan)?(valueTuple);
        alertState1.ShowCooldown = alert.Value.ShowCooldown;
        alertState1.AutoRemove = alert.Value.AutoRemove;
        alertState1.Type = alert.Value.Type;
        AlertState alertState2 = alertState1;
        alertsComponent.Alerts[alert.Key] = alertState2;
        flag = true;
      }
    }
    if (!flag)
      return;
    this.Dirty(uid, (IComponent) comp, (MetaDataComponent) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<AlertAutoRemoveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AlertAutoRemoveComponent>();
    EntityUid entityUid;
    AlertAutoRemoveComponent autoRemoveComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref autoRemoveComponent))
    {
      bool flag = false;
      AlertsComponent alertsComponent;
      if (autoRemoveComponent.AlertKeys.Count <= 0 || !this.TryComp<AlertsComponent>(entityUid, ref alertsComponent))
      {
        this.RemCompDeferred(entityUid, (IComponent) autoRemoveComponent);
      }
      else
      {
        List<AlertKey> alertKeyList = new List<AlertKey>();
        foreach (AlertKey alertKey in autoRemoveComponent.AlertKeys)
        {
          AlertState alertState;
          alertsComponent.Alerts.TryGetValue(alertKey, out alertState);
          if (alertState.Cooldown.HasValue && !(alertState.Cooldown.Value.Item2 >= this._timing.CurTime))
          {
            alertKeyList.Add(alertKey);
            alertsComponent.Alerts.Remove(alertKey);
            flag = true;
          }
        }
        foreach (AlertKey alertKey in alertKeyList)
          autoRemoveComponent.AlertKeys.Remove(alertKey);
        if (flag)
          this.Dirty(entityUid, (IComponent) alertsComponent, (MetaDataComponent) null);
      }
    }
  }

  protected virtual void HandleComponentShutdown(
    EntityUid uid,
    AlertsComponent component,
    ComponentShutdown args)
  {
    this.RaiseLocalEvent<AlertSyncEvent>(uid, new AlertSyncEvent(uid), true);
  }

  private void HandleComponentStartup(
    EntityUid uid,
    AlertsComponent component,
    ComponentStartup args)
  {
    this.RaiseLocalEvent<AlertSyncEvent>(uid, new AlertSyncEvent(uid), true);
  }

  private void HandlePrototypesReloaded(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<AlertPrototype>())
      return;
    this.LoadPrototypes();
  }

  protected virtual void LoadPrototypes()
  {
    Dictionary<ProtoId<AlertPrototype>, AlertPrototype> source = new Dictionary<ProtoId<AlertPrototype>, AlertPrototype>();
    foreach (AlertPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<AlertPrototype>())
    {
      if (!source.TryAdd(ProtoId<AlertPrototype>.op_Implicit(enumeratePrototype.ID), enumeratePrototype))
        this.Log.Error("Found alert with duplicate alertType {0} - all alerts must have a unique alertType, this one will be skipped", new object[1]
        {
          (object) enumeratePrototype.ID
        });
    }
    this._typeToAlert = source.ToFrozenDictionary<ProtoId<AlertPrototype>, AlertPrototype>();
  }

  public bool TryGet(ProtoId<AlertPrototype> alertType, [NotNullWhen(true)] out AlertPrototype? alert)
  {
    return this._typeToAlert.TryGetValue(alertType, out alert);
  }

  private bool TryGetAlert(
    ProtoId<AlertPrototype> alertType,
    EntityUid? player,
    out AlertPrototype? alert,
    bool activate = true)
  {
    alert = (AlertPrototype) null;
    if (!player.HasValue || !this.HasComp<AlertsComponent>(player))
      return false;
    if (!this.IsShowingAlert(player.Value, alertType))
    {
      this.Log.Debug("User {0} attempted to click alert {1} which is not currently showing for them", new object[2]
      {
        (object) this.Comp<MetaDataComponent>(player.Value).EntityName,
        (object) alertType
      });
      return false;
    }
    if (!this.TryGet(alertType, out alert))
    {
      this.Log.Warning("Unrecognized encoded alert {0}", new object[1]
      {
        (object) alert
      });
      return false;
    }
    if (!activate || !this.ActivateAlert(player.Value, alert) || !this._timing.IsFirstTimePredicted)
      return true;
    this.HandledAlert();
    return true;
  }

  protected virtual void HandledAlert()
  {
  }

  private void HandleClickAlert(ClickAlertEvent ev, EntitySessionEventArgs args)
  {
    this.TryGetAlert(ev.Type, (EntityUid?) ((EntitySessionEventArgs) ref args).SenderSession?.AttachedEntity, out AlertPrototype _);
  }

  private void HandleClickAlertAlt(ClickAlertAltEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = ((EntitySessionEventArgs) ref args).SenderSession.AttachedEntity;
    AlertPrototype alert;
    if (!this.TryGetAlert(msg.Type, attachedEntity, out alert, false) || alert == null || !attachedEntity.HasValue)
      return;
    this.ActivateAlertAlt(attachedEntity.Value, alert);
  }

  public bool ActivateAlert(EntityUid user, AlertPrototype alert)
  {
    BaseAlertEvent clickEvent = alert.ClickEvent;
    if (clickEvent == null)
      return false;
    clickEvent.Handled = false;
    clickEvent.User = user;
    clickEvent.AlertId = ProtoId<AlertPrototype>.op_Implicit(alert.ID);
    this.RaiseLocalEvent(user, (object) clickEvent, true);
    return clickEvent.Handled;
  }

  public bool ActivateAlertAlt(EntityUid user, AlertPrototype alert)
  {
    BaseAlertEvent altClickEvent = alert.AltClickEvent;
    if (altClickEvent == null)
      return false;
    altClickEvent.Handled = false;
    altClickEvent.User = user;
    altClickEvent.AlertId = ProtoId<AlertPrototype>.op_Implicit(alert.ID);
    this.RaiseLocalEvent(user, (object) altClickEvent, true);
    return altClickEvent.Handled;
  }

  private void OnPlayerAttached(EntityUid uid, AlertsComponent component, PlayerAttachedEvent args)
  {
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }
}
