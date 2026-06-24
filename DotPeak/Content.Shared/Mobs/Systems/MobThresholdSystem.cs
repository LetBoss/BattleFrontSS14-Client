// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Systems.MobThresholdSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.CriticalGrace;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Mobs.Systems;

public sealed class MobThresholdSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobStateSystem;
  [Dependency]
  private AlertsSystem _alerts;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MobThresholdsComponent, ComponentGetState>(new ComponentEventRefHandler<MobThresholdsComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<MobThresholdsComponent, ComponentHandleState>(new ComponentEventRefHandler<MobThresholdsComponent, ComponentHandleState>(this.OnHandleState));
    this.SubscribeLocalEvent<MobThresholdsComponent, ComponentShutdown>(new ComponentEventHandler<MobThresholdsComponent, ComponentShutdown>(this.MobThresholdShutdown));
    this.SubscribeLocalEvent<MobThresholdsComponent, ComponentStartup>(new ComponentEventHandler<MobThresholdsComponent, ComponentStartup>(this.MobThresholdStartup));
    this.SubscribeLocalEvent<MobThresholdsComponent, DamageChangedEvent>(new ComponentEventHandler<MobThresholdsComponent, DamageChangedEvent>(this.OnDamaged));
    this.SubscribeLocalEvent<MobThresholdsComponent, UpdateMobStateEvent>(new ComponentEventRefHandler<MobThresholdsComponent, UpdateMobStateEvent>(this.OnUpdateMobState));
    this.SubscribeLocalEvent<MobThresholdsComponent, MobStateChangedEvent>(new EntityEventRefHandler<MobThresholdsComponent, MobStateChangedEvent>(this.OnThresholdsMobState));
  }

  private void OnGetState(
    EntityUid uid,
    MobThresholdsComponent component,
    ref ComponentGetState args)
  {
    Dictionary<FixedPoint2, MobState> unsortedThresholds = new Dictionary<FixedPoint2, MobState>();
    foreach ((FixedPoint2 key, MobState mobState) in component.Thresholds)
      unsortedThresholds.Add(key, mobState);
    args.State = (IComponentState) new MobThresholdsComponentState(unsortedThresholds, component.TriggersAlerts, component.CurrentThresholdState, component.StateAlertDict, component.ShowOverlays, component.AllowRevives, component.DisplayDamageInAlert);
  }

  private void OnHandleState(
    EntityUid uid,
    MobThresholdsComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is MobThresholdsComponentState current))
      return;
    component.Thresholds = new SortedDictionary<FixedPoint2, MobState>((IDictionary<FixedPoint2, MobState>) current.UnsortedThresholds);
    component.TriggersAlerts = current.TriggersAlerts;
    component.CurrentThresholdState = current.CurrentThresholdState;
    component.AllowRevives = current.AllowRevives;
  }

  public bool TryGetNextState(
    EntityUid target,
    MobState mobState,
    [NotNullWhen(true)] out MobState? nextState,
    MobThresholdsComponent? thresholdsComponent = null)
  {
    nextState = new MobState?();
    if (!this.Resolve<MobThresholdsComponent>(target, ref thresholdsComponent))
      return false;
    MobState? nullable1 = new MobState?();
    foreach (MobState mobState1 in thresholdsComponent.Thresholds.Values)
    {
      if (mobState1 > mobState)
      {
        if (nullable1.HasValue)
        {
          int num = (int) mobState1;
          MobState? nullable2 = nullable1;
          int valueOrDefault = (int) nullable2.GetValueOrDefault();
          if (!(num < valueOrDefault & nullable2.HasValue))
            continue;
        }
        nullable1 = new MobState?(mobState1);
      }
    }
    nextState = nullable1;
    return nextState.HasValue;
  }

  public FixedPoint2 GetThresholdForState(
    EntityUid target,
    MobState mobState,
    MobThresholdsComponent? thresholdComponent = null)
  {
    if (!this.Resolve<MobThresholdsComponent>(target, ref thresholdComponent))
      return FixedPoint2.Zero;
    foreach (KeyValuePair<FixedPoint2, MobState> threshold in thresholdComponent.Thresholds)
    {
      if (threshold.Value == mobState)
        return threshold.Key;
    }
    return FixedPoint2.Zero;
  }

  public bool TryGetThresholdForState(
    EntityUid target,
    MobState mobState,
    [NotNullWhen(true)] out FixedPoint2? threshold,
    MobThresholdsComponent? thresholdComponent = null)
  {
    threshold = new FixedPoint2?();
    if (!this.Resolve<MobThresholdsComponent>(target, ref thresholdComponent))
      return false;
    foreach (KeyValuePair<FixedPoint2, MobState> threshold1 in thresholdComponent.Thresholds)
    {
      if (threshold1.Value == mobState)
      {
        threshold = new FixedPoint2?(threshold1.Key);
        return true;
      }
    }
    return false;
  }

  public bool TryGetPercentageForState(
    EntityUid target,
    MobState mobState,
    FixedPoint2 damage,
    [NotNullWhen(true)] out FixedPoint2? percentage,
    MobThresholdsComponent? thresholdComponent = null)
  {
    percentage = new FixedPoint2?();
    FixedPoint2? threshold;
    if (!this.TryGetThresholdForState(target, mobState, out threshold, thresholdComponent))
      return false;
    ref FixedPoint2? local = ref percentage;
    FixedPoint2 fixedPoint2 = damage;
    FixedPoint2? nullable1 = threshold;
    FixedPoint2? nullable2 = nullable1.HasValue ? new FixedPoint2?(fixedPoint2 / nullable1.GetValueOrDefault()) : new FixedPoint2?();
    local = nullable2;
    return true;
  }

  public bool TryGetIncapThreshold(
    EntityUid target,
    [NotNullWhen(true)] out FixedPoint2? threshold,
    MobThresholdsComponent? thresholdComponent = null)
  {
    threshold = new FixedPoint2?();
    if (!this.Resolve<MobThresholdsComponent>(target, ref thresholdComponent))
      return false;
    return this.TryGetThresholdForState(target, MobState.Critical, out threshold, thresholdComponent) || this.TryGetThresholdForState(target, MobState.Dead, out threshold, thresholdComponent);
  }

  public bool TryGetIncapPercentage(
    EntityUid target,
    FixedPoint2 damage,
    [NotNullWhen(true)] out FixedPoint2? percentage,
    MobThresholdsComponent? thresholdComponent = null)
  {
    percentage = new FixedPoint2?();
    FixedPoint2? threshold;
    if (!this.TryGetIncapThreshold(target, out threshold, thresholdComponent))
      return false;
    if (damage == 0)
    {
      percentage = new FixedPoint2?((FixedPoint2) 0);
      return true;
    }
    percentage = new FixedPoint2?(FixedPoint2.Min((FixedPoint2) 1f, damage / threshold.Value));
    return true;
  }

  public bool TryGetDeadThreshold(
    EntityUid target,
    [NotNullWhen(true)] out FixedPoint2? threshold,
    MobThresholdsComponent? thresholdComponent = null)
  {
    threshold = new FixedPoint2?();
    return this.Resolve<MobThresholdsComponent>(target, ref thresholdComponent, false) && this.TryGetThresholdForState(target, MobState.Dead, out threshold, thresholdComponent);
  }

  public bool TryGetDeadPercentage(
    EntityUid target,
    FixedPoint2 damage,
    [NotNullWhen(true)] out FixedPoint2? percentage,
    MobThresholdsComponent? thresholdComponent = null)
  {
    percentage = new FixedPoint2?();
    FixedPoint2? threshold;
    if (!this.TryGetDeadThreshold(target, out threshold, thresholdComponent))
      return false;
    if (damage == 0)
    {
      percentage = new FixedPoint2?((FixedPoint2) 0);
      return true;
    }
    percentage = new FixedPoint2?(FixedPoint2.Min((FixedPoint2) 1f, damage / threshold.Value));
    return true;
  }

  public bool GetScaledDamage(EntityUid target1, EntityUid target2, out DamageSpecifier? damage)
  {
    damage = (DamageSpecifier) null;
    DamageableComponent comp1;
    MobThresholdsComponent comp2;
    MobThresholdsComponent comp3;
    if (!this.TryComp<DamageableComponent>(target1, out comp1) || !this.TryComp<MobThresholdsComponent>(target1, out comp2) || !this.TryComp<MobThresholdsComponent>(target2, out comp3))
      return false;
    FixedPoint2? threshold1;
    if (!this.TryGetThresholdForState(target1, MobState.Dead, out threshold1, comp2))
      threshold1 = new FixedPoint2?((FixedPoint2) 0);
    FixedPoint2? threshold2;
    if (!this.TryGetThresholdForState(target2, MobState.Dead, out threshold2, comp3))
      threshold2 = new FixedPoint2?((FixedPoint2) 0);
    damage = comp1.Damage / threshold1.Value * threshold2.Value;
    return true;
  }

  public void SetMobStateThreshold(
    EntityUid target,
    FixedPoint2 damage,
    MobState mobState,
    MobThresholdsComponent? threshold = null)
  {
    if (!this.Resolve<MobThresholdsComponent>(target, ref threshold))
      return;
    foreach ((FixedPoint2 key, MobState mobState1) in new Dictionary<FixedPoint2, MobState>((IDictionary<FixedPoint2, MobState>) threshold.Thresholds))
    {
      if (mobState1 == mobState)
        threshold.Thresholds.Remove(key);
    }
    threshold.Thresholds[damage] = mobState;
    this.Dirty(target, (IComponent) threshold);
    this.VerifyThresholds(target, threshold);
  }

  public void VerifyThresholds(
    EntityUid target,
    MobThresholdsComponent? threshold = null,
    MobStateComponent? mobState = null,
    DamageableComponent? damageable = null)
  {
    if (!this.Resolve<MobStateComponent, MobThresholdsComponent, DamageableComponent>(target, ref mobState, ref threshold, ref damageable))
      return;
    this.CheckThresholds(target, mobState, threshold, damageable);
    MobThresholdChecked args = new MobThresholdChecked(target, mobState, threshold, damageable);
    this.RaiseLocalEvent<MobThresholdChecked>(target, ref args, true);
    this.UpdateAlerts(target, mobState.CurrentState, threshold, damageable);
  }

  public void SetAllowRevives(EntityUid uid, bool val, MobThresholdsComponent? component = null)
  {
    if (!this.Resolve<MobThresholdsComponent>(uid, ref component, false))
      return;
    component.AllowRevives = val;
    this.Dirty(uid, (IComponent) component);
    this.VerifyThresholds(uid, component);
  }

  private void CheckThresholds(
    EntityUid target,
    MobStateComponent mobStateComponent,
    MobThresholdsComponent thresholdsComponent,
    DamageableComponent damageableComponent,
    EntityUid? origin = null)
  {
    foreach ((FixedPoint2 key, MobState newState) in thresholdsComponent.Thresholds.Reverse<KeyValuePair<FixedPoint2, MobState>>())
    {
      if (!(damageableComponent.TotalDamage < key))
      {
        this.TriggerThreshold(target, newState, mobStateComponent, thresholdsComponent, origin);
        break;
      }
    }
  }

  private void TriggerThreshold(
    EntityUid target,
    MobState newState,
    MobStateComponent? mobState = null,
    MobThresholdsComponent? thresholds = null,
    EntityUid? origin = null)
  {
    if (!this.Resolve<MobStateComponent, MobThresholdsComponent>(target, ref mobState, ref thresholds) || mobState.CurrentState == newState)
      return;
    if (mobState.CurrentState != MobState.Dead || thresholds.AllowRevives)
    {
      thresholds.CurrentThresholdState = newState;
      this.Dirty(target, (IComponent) thresholds);
    }
    this._mobStateSystem.UpdateMobState(target, mobState, origin);
  }

  private void UpdateAlerts(
    EntityUid target,
    MobState currentMobState,
    MobThresholdsComponent? threshold = null,
    DamageableComponent? damageable = null)
  {
    if (!this.Resolve<MobThresholdsComponent, DamageableComponent>(target, ref threshold, ref damageable) || !threshold.TriggersAlerts)
      return;
    FixedPoint2? threshold1;
    bool incapThreshold = this.TryGetIncapThreshold(target, out threshold1, threshold);
    MobState key = currentMobState;
    FixedPoint2? nullable;
    if (incapThreshold && this.HasComp<InCriticalGraceComponent>(target))
    {
      FixedPoint2 totalDamage = damageable.TotalDamage;
      nullable = threshold1;
      if ((nullable.HasValue ? (totalDamage > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        key = MobState.Critical;
    }
    ProtoId<AlertPrototype> protoId;
    if (!threshold.StateAlertDict.TryGetValue(key, out protoId))
    {
      this.Log.Error($"No alert alert for mob state {key} for entity {this.ToPrettyString((Entity<MetaDataComponent>) target)}");
    }
    else
    {
      AlertPrototype alert;
      if (!this._alerts.TryGet(protoId, out alert))
      {
        this.Log.Error($"Invalid alert type {protoId}");
      }
      else
      {
        string str1 = (string) null;
        if (threshold.DisplayDamageInAlert & incapThreshold && threshold1.HasValue)
        {
          string str2 = ((int) threshold1.Value - (int) damageable.TotalDamage).ToString();
          nullable = threshold1;
          string str3 = nullable.ToString();
          str1 = $"{str2} / {str3}";
        }
        if (alert.SupportsSeverity)
        {
          short severity1 = this._alerts.GetMinSeverity(protoId);
          BeforeAlertSeverityCheckEvent args = new BeforeAlertSeverityCheckEvent(protoId, severity1);
          this.RaiseLocalEvent<BeforeAlertSeverityCheckEvent>(target, args);
          if (args.CancelUpdate)
          {
            this._alerts.ShowAlert(target, args.CurrentAlert, new short?(args.Severity));
          }
          else
          {
            MobState? nextState;
            FixedPoint2? percentage;
            if (this.TryGetNextState(target, currentMobState, out nextState, threshold) && this.TryGetPercentageForState(target, nextState.Value, damageable.TotalDamage, out percentage))
            {
              percentage = new FixedPoint2?(FixedPoint2.Clamp(percentage.Value, (FixedPoint2) 0, (FixedPoint2) 1));
              severity1 = (short) MathF.Round(MathHelper.Lerp((float) this._alerts.GetMinSeverity(protoId), (float) this._alerts.GetMaxSeverity(protoId), percentage.Value.Float()));
            }
            AlertsSystem alerts = this._alerts;
            EntityUid euid = target;
            ProtoId<AlertPrototype> alertType = protoId;
            short? severity2 = new short?(severity1);
            string str4 = str1;
            (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
            string dynamicMessage = str4;
            alerts.ShowAlert(euid, alertType, severity2, cooldown, dynamicMessage: dynamicMessage);
          }
        }
        else
        {
          AlertsSystem alerts = this._alerts;
          EntityUid euid = target;
          ProtoId<AlertPrototype> alertType = protoId;
          string str5 = str1;
          short? severity = new short?();
          (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
          string dynamicMessage = str5;
          alerts.ShowAlert(euid, alertType, severity, cooldown, dynamicMessage: dynamicMessage);
        }
      }
    }
  }

  private void OnDamaged(
    EntityUid target,
    MobThresholdsComponent thresholds,
    DamageChangedEvent args)
  {
    MobStateComponent comp;
    if (!this.TryComp<MobStateComponent>(target, out comp))
      return;
    this.CheckThresholds(target, comp, thresholds, args.Damageable, args.Origin);
    MobThresholdChecked args1 = new MobThresholdChecked(target, comp, thresholds, args.Damageable);
    this.RaiseLocalEvent<MobThresholdChecked>(target, ref args1, true);
    this.UpdateAlerts(target, comp.CurrentState, thresholds, args.Damageable);
  }

  private void MobThresholdStartup(
    EntityUid target,
    MobThresholdsComponent thresholds,
    ComponentStartup args)
  {
    MobStateComponent comp1;
    DamageableComponent comp2;
    if (!this.TryComp<MobStateComponent>(target, out comp1) || !this.TryComp<DamageableComponent>(target, out comp2))
      return;
    this.CheckThresholds(target, comp1, thresholds, comp2);
    this.UpdateAllEffects((Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>) (target, thresholds, comp1, comp2), comp1.CurrentState);
  }

  private void MobThresholdShutdown(
    EntityUid target,
    MobThresholdsComponent component,
    ComponentShutdown args)
  {
    if (!component.TriggersAlerts)
      return;
    this._alerts.ClearAlertCategory(target, component.HealthAlertCategory);
  }

  private void OnUpdateMobState(
    EntityUid target,
    MobThresholdsComponent component,
    ref UpdateMobStateEvent args)
  {
    if (!component.AllowRevives && component.CurrentThresholdState == MobState.Dead)
    {
      args.State = MobState.Dead;
    }
    else
    {
      if (component.CurrentThresholdState == MobState.Invalid)
        return;
      args.State = component.CurrentThresholdState;
    }
  }

  private void UpdateAllEffects(
    Entity<MobThresholdsComponent, MobStateComponent?, DamageableComponent?> ent,
    MobState currentState)
  {
    (EntityUid _, MobThresholdsComponent comp1, MobStateComponent comp2, DamageableComponent comp3) = ent;
    if (this.Resolve<MobThresholdsComponent, MobStateComponent, DamageableComponent>((EntityUid) ent, ref comp1, ref comp2, ref comp3))
    {
      MobThresholdChecked args = new MobThresholdChecked((EntityUid) ent, comp2, comp1, comp3);
      this.RaiseLocalEvent<MobThresholdChecked>((EntityUid) ent, ref args, true);
    }
    this.UpdateAlerts((EntityUid) ent, currentState, comp1, comp3);
  }

  private void OnThresholdsMobState(
    Entity<MobThresholdsComponent> ent,
    ref MobStateChangedEvent args)
  {
    this.UpdateAllEffects((Entity<MobThresholdsComponent, MobStateComponent, DamageableComponent>) ((EntityUid) ent, (MobThresholdsComponent) ent, (MobStateComponent) null, (DamageableComponent) null), args.NewMobState);
  }
}
