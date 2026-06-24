// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusEffect.StatusEffectsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.StatusEffect;
using Content.Shared.Alert;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.StatusEffect;

[Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
public sealed class StatusEffectsSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private AlertsSystem _alertsSystem;
  private List<EntityUid> _toRemove = new List<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this.SubscribeLocalEvent<StatusEffectsComponent, ComponentGetState>(new ComponentEventRefHandler<StatusEffectsComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<StatusEffectsComponent, ComponentHandleState>(new ComponentEventRefHandler<StatusEffectsComponent, ComponentHandleState>(this.OnHandleState));
    this.SubscribeLocalEvent<StatusEffectsComponent, RejuvenateEvent>(new ComponentEventHandler<StatusEffectsComponent, RejuvenateEvent>(this.OnRejuvenate));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._gameTiming.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveStatusEffectsComponent, StatusEffectsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveStatusEffectsComponent, StatusEffectsComponent>();
    this._toRemove.Clear();
    EntityUid uid1;
    StatusEffectsComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid1, out ActiveStatusEffectsComponent _, out comp2))
    {
      if (comp2.ActiveEffects.Count == 0)
      {
        this._toRemove.Add(uid1);
      }
      else
      {
        foreach (KeyValuePair<string, StatusEffectState> activeEffect in comp2.ActiveEffects)
        {
          if (curTime > activeEffect.Value.Cooldown.Item2)
            this.TryRemoveStatusEffect(uid1, activeEffect.Key, comp2);
        }
      }
    }
    foreach (EntityUid uid2 in this._toRemove)
      this.RemComp<ActiveStatusEffectsComponent>(uid2);
  }

  private void OnGetState(
    EntityUid uid,
    StatusEffectsComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new StatusEffectsComponentState(new Dictionary<string, StatusEffectState>((IDictionary<string, StatusEffectState>) component.ActiveEffects), new List<string>((IEnumerable<string>) component.AllowedEffects));
  }

  private void OnHandleState(
    EntityUid uid,
    StatusEffectsComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is StatusEffectsComponentState current))
      return;
    component.AllowedEffects.Clear();
    component.AllowedEffects.AddRange((IEnumerable<string>) current.AllowedEffects);
    foreach (string key in component.ActiveEffects.Keys)
    {
      if (!current.ActiveEffects.ContainsKey(key))
        component.ActiveEffects.Remove(key);
    }
    foreach ((string key, StatusEffectState toCopy) in current.ActiveEffects)
      component.ActiveEffects[key] = new StatusEffectState(toCopy);
    if (component.ActiveEffects.Count == 0)
      this.RemComp<ActiveStatusEffectsComponent>(uid);
    else
      this.EnsureComp<ActiveStatusEffectsComponent>(uid);
  }

  private void OnRejuvenate(EntityUid uid, StatusEffectsComponent component, RejuvenateEvent args)
  {
    this.TryRemoveAllStatusEffects(uid, component);
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryAddStatusEffect<T>(
    EntityUid uid,
    string key,
    TimeSpan time,
    bool refresh,
    StatusEffectsComponent? status = null,
    bool force = false)
    where T : IComponent, new()
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false))
      return false;
    EntityUid uid1 = uid;
    string key1 = key;
    TimeSpan time1 = time;
    int num1 = refresh ? 1 : 0;
    StatusEffectsComponent status1 = status;
    bool flag = force;
    TimeSpan? startTime = new TimeSpan?();
    int num2 = flag ? 1 : 0;
    if (!this.TryAddStatusEffect(uid1, key1, time1, num1 != 0, status1, startTime, num2 != 0))
      return false;
    if (this.HasComp<T>(uid))
    {
      status.ActiveEffects[key].RelevantComponent = this.Factory.GetComponentName<T>();
      return true;
    }
    this.AddComp<T>(uid);
    status.ActiveEffects[key].RelevantComponent = this.Factory.GetComponentName<T>();
    return true;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryAddStatusEffect(
    EntityUid uid,
    string key,
    TimeSpan time,
    bool refresh,
    string component,
    StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.TryAddStatusEffect(uid, key, time, refresh, status))
      return false;
    if (!this.HasComp(uid, this.Factory.GetRegistration(component).Type))
    {
      Component component1 = (Component) this.Factory.GetComponent(component);
      this.AddComp<Component>(uid, component1);
      status.ActiveEffects[key].RelevantComponent = component;
    }
    return true;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryAddStatusEffect(
    EntityUid uid,
    string key,
    TimeSpan time,
    bool refresh,
    StatusEffectsComponent? status = null,
    TimeSpan? startTime = null,
    bool force = false)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.CanApplyEffect(uid, key, status, force))
      return false;
    RMCStatusEffectTimeEvent args = new RMCStatusEffectTimeEvent(key, time);
    this.RaiseLocalEvent<RMCStatusEffectTimeEvent>(uid, ref args);
    time = args.Duration;
    StatusEffectPrototype statusEffectPrototype = this._prototypeManager.Index<StatusEffectPrototype>(key);
    TimeSpan timeSpan = startTime ?? this._gameTiming.CurTime;
    (TimeSpan, TimeSpan) cooldown1 = (timeSpan, timeSpan + time);
    if (this.HasStatusEffect(uid, key, status))
    {
      status.ActiveEffects[key].CooldownRefresh = refresh;
      if (refresh)
      {
        if (status.ActiveEffects[key].Cooldown.Item2 - this._gameTiming.CurTime < time)
          status.ActiveEffects[key].Cooldown = cooldown1;
      }
      else
        status.ActiveEffects[key].Cooldown.Item2 += time;
    }
    else
    {
      status.ActiveEffects.Add(key, new StatusEffectState(cooldown1, refresh));
      this.EnsureComp<ActiveStatusEffectsComponent>(uid);
    }
    if (statusEffectPrototype.Alert.HasValue)
    {
      EntityUid uid1 = uid;
      ProtoId<AlertPrototype>? alert1 = statusEffectPrototype.Alert;
      ProtoId<AlertPrototype> alert2 = alert1.Value;
      StatusEffectsComponent status1 = status;
      (TimeSpan, TimeSpan)? alertCooldown = this.GetAlertCooldown(uid1, alert2, status1);
      AlertsSystem alertsSystem = this._alertsSystem;
      EntityUid euid = uid;
      alert1 = statusEffectPrototype.Alert;
      ProtoId<AlertPrototype> alertType = alert1.Value;
      short? severity = new short?();
      (TimeSpan, TimeSpan)? cooldown2 = alertCooldown;
      alertsSystem.ShowAlert(euid, alertType, severity, cooldown2);
    }
    this.Dirty(uid, (IComponent) status);
    this.RaiseLocalEvent<StatusEffectAddedEvent>(uid, new StatusEffectAddedEvent(uid, key));
    return true;
  }

  private (TimeSpan, TimeSpan)? GetAlertCooldown(
    EntityUid uid,
    ProtoId<AlertPrototype> alert,
    StatusEffectsComponent status)
  {
    (TimeSpan, TimeSpan)? alertCooldown = new (TimeSpan, TimeSpan)?();
    foreach (KeyValuePair<string, StatusEffectState> activeEffect in status.ActiveEffects)
    {
      ProtoId<AlertPrototype>? alert1 = this._prototypeManager.Index<StatusEffectPrototype>(activeEffect.Key).Alert;
      ProtoId<AlertPrototype> protoId = alert;
      if ((alert1.HasValue ? (alert1.GetValueOrDefault() == protoId ? 1 : 0) : 0) != 0 && (!alertCooldown.HasValue || activeEffect.Value.Cooldown.Item2 > alertCooldown.Value.Item2))
        alertCooldown = new (TimeSpan, TimeSpan)?(activeEffect.Value.Cooldown);
    }
    return alertCooldown;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryRemoveStatusEffect(
    EntityUid uid,
    string key,
    StatusEffectsComponent? status = null,
    bool remComp = true)
  {
    StatusEffectPrototype prototype;
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !status.ActiveEffects.ContainsKey(key) || !this._prototypeManager.TryIndex<StatusEffectPrototype>(key, out prototype))
      return false;
    StatusEffectState activeEffect = status.ActiveEffects[key];
    ComponentRegistration registration;
    if (remComp && activeEffect.RelevantComponent != null && this.Factory.TryGetRegistration(activeEffect.RelevantComponent, out registration))
    {
      Type type = registration.Type;
      this.RemComp(uid, type);
    }
    if (prototype.Alert.HasValue)
      this._alertsSystem.ClearAlert(uid, prototype.Alert.Value);
    status.ActiveEffects.Remove(key);
    if (status.ActiveEffects.Count == 0)
      this.RemComp<ActiveStatusEffectsComponent>(uid);
    this.Dirty(uid, (IComponent) status);
    this.RaiseLocalEvent<StatusEffectEndedEvent>(uid, new StatusEffectEndedEvent(uid, key));
    return true;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryRemoveAllStatusEffects(EntityUid uid, StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false))
      return false;
    bool flag = false;
    foreach (KeyValuePair<string, StatusEffectState> activeEffect in status.ActiveEffects)
    {
      if (!this.TryRemoveStatusEffect(uid, activeEffect.Key, status))
        flag = true;
    }
    this.Dirty(uid, (IComponent) status);
    return flag;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool HasStatusEffect(EntityUid uid, string key, StatusEffectsComponent? status = null)
  {
    return this.Resolve<StatusEffectsComponent>(uid, ref status, false) && status.ActiveEffects.ContainsKey(key);
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool CanApplyEffect(EntityUid uid, string key, StatusEffectsComponent? status = null, bool force = false)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false))
      return false;
    if (!force)
    {
      BeforeStatusEffectAddedEvent args = new BeforeStatusEffectAddedEvent((EntProtoId) key);
      this.RaiseLocalEvent<BeforeStatusEffectAddedEvent>(uid, ref args);
      if (args.Cancelled)
        return false;
    }
    StatusEffectPrototype prototype;
    return this._prototypeManager.TryIndex<StatusEffectPrototype>(key, out prototype) && (status.AllowedEffects.Contains(key) || prototype.AlwaysAllowed);
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryAddTime(EntityUid uid, string key, TimeSpan time, StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.HasStatusEffect(uid, key, status))
      return false;
    (TimeSpan, TimeSpan) cooldown1 = status.ActiveEffects[key].Cooldown;
    cooldown1.Item2 += time;
    status.ActiveEffects[key].Cooldown = cooldown1;
    StatusEffectPrototype prototype;
    if (this._prototypeManager.TryIndex<StatusEffectPrototype>(key, out prototype) && prototype.Alert.HasValue)
    {
      EntityUid uid1 = uid;
      ProtoId<AlertPrototype>? alert1 = prototype.Alert;
      ProtoId<AlertPrototype> alert2 = alert1.Value;
      StatusEffectsComponent status1 = status;
      (TimeSpan, TimeSpan)? alertCooldown = this.GetAlertCooldown(uid1, alert2, status1);
      AlertsSystem alertsSystem = this._alertsSystem;
      EntityUid euid = uid;
      alert1 = prototype.Alert;
      ProtoId<AlertPrototype> alertType = alert1.Value;
      short? severity = new short?();
      (TimeSpan, TimeSpan)? cooldown2 = alertCooldown;
      alertsSystem.ShowAlert(euid, alertType, severity, cooldown2);
    }
    this.Dirty(uid, (IComponent) status);
    return true;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryRemoveTime(
    EntityUid uid,
    string key,
    TimeSpan time,
    StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.HasStatusEffect(uid, key, status))
      return false;
    (TimeSpan, TimeSpan) cooldown1 = status.ActiveEffects[key].Cooldown;
    if (time > cooldown1.Item2)
      return false;
    cooldown1.Item2 -= time;
    status.ActiveEffects[key].Cooldown = cooldown1;
    StatusEffectPrototype prototype;
    if (this._prototypeManager.TryIndex<StatusEffectPrototype>(key, out prototype) && prototype.Alert.HasValue)
    {
      EntityUid uid1 = uid;
      ProtoId<AlertPrototype>? alert1 = prototype.Alert;
      ProtoId<AlertPrototype> alert2 = alert1.Value;
      StatusEffectsComponent status1 = status;
      (TimeSpan, TimeSpan)? alertCooldown = this.GetAlertCooldown(uid1, alert2, status1);
      AlertsSystem alertsSystem = this._alertsSystem;
      EntityUid euid = uid;
      alert1 = prototype.Alert;
      ProtoId<AlertPrototype> alertType = alert1.Value;
      short? severity = new short?();
      (TimeSpan, TimeSpan)? cooldown2 = alertCooldown;
      alertsSystem.ShowAlert(euid, alertType, severity, cooldown2);
    }
    this.Dirty(uid, (IComponent) status);
    return true;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TrySetTime(EntityUid uid, string key, TimeSpan time, StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.HasStatusEffect(uid, key, status))
      return false;
    status.ActiveEffects[key].Cooldown = (this._gameTiming.CurTime, this._gameTiming.CurTime + time);
    this.Dirty(uid, (IComponent) status);
    return true;
  }

  [Obsolete("Migration to Content.Shared.StatusEffectNew.SharedStatusEffectsSystem is required")]
  public bool TryGetTime(
    EntityUid uid,
    string key,
    [NotNullWhen(true)] out (TimeSpan, TimeSpan)? time,
    StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || !this.HasStatusEffect(uid, key, status))
    {
      time = new (TimeSpan, TimeSpan)?();
      return false;
    }
    time = new (TimeSpan, TimeSpan)?(status.ActiveEffects[key].Cooldown);
    return true;
  }
}
