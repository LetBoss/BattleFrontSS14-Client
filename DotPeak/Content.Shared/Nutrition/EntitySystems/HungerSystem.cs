// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.HungerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class HungerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;
  [Dependency]
  private SharedJetpackSystem _jetpack;
  private static readonly ProtoId<SatiationIconPrototype> HungerIconOverfedId = (ProtoId<SatiationIconPrototype>) "HungerIconOverfed";
  private static readonly ProtoId<SatiationIconPrototype> HungerIconPeckishId = (ProtoId<SatiationIconPrototype>) "HungerIconPeckish";
  private static readonly ProtoId<SatiationIconPrototype> HungerIconStarvingId = (ProtoId<SatiationIconPrototype>) "HungerIconStarving";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HungerComponent, MapInitEvent>(new ComponentEventHandler<HungerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<HungerComponent, ComponentShutdown>(new ComponentEventHandler<HungerComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<HungerComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<HungerComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed));
    this.SubscribeLocalEvent<HungerComponent, RejuvenateEvent>(new ComponentEventHandler<HungerComponent, RejuvenateEvent>(this.OnRejuvenate));
  }

  private void OnMapInit(EntityUid uid, HungerComponent component, MapInitEvent args)
  {
    int amount = this._random.Next((int) component.Thresholds[HungerThreshold.Peckish] + 10, (int) component.Thresholds[HungerThreshold.Okay]);
    this.SetHunger(uid, (float) amount, component);
  }

  private void OnShutdown(EntityUid uid, HungerComponent component, ComponentShutdown args)
  {
    this._alerts.ClearAlertCategory(uid, component.HungerAlertCategory);
  }

  private void OnRefreshMovespeed(
    EntityUid uid,
    HungerComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    if (component.CurrentThreshold > HungerThreshold.Starving || this._jetpack.IsUserFlying(uid))
      return;
    args.ModifySpeed(component.StarvingSlowdownModifier, component.StarvingSlowdownModifier);
  }

  private void OnRejuvenate(EntityUid uid, HungerComponent component, RejuvenateEvent args)
  {
    this.SetHunger(uid, component.Thresholds[HungerThreshold.Okay], component);
  }

  public float GetHunger(HungerComponent component)
  {
    TimeSpan timeSpan = this._timing.CurTime - component.LastAuthoritativeHungerChangeTime;
    float hungerValue = component.LastAuthoritativeHungerValue - (float) timeSpan.TotalSeconds * component.ActualDecayRate;
    return HungerSystem.ClampHungerWithinThresholds(component, hungerValue);
  }

  public void ModifyHunger(EntityUid uid, float amount, HungerComponent? component = null)
  {
    if (!this.Resolve<HungerComponent>(uid, ref component))
      return;
    this.SetHunger(uid, this.GetHunger(component) + amount, component);
  }

  public void SetHunger(EntityUid uid, float amount, HungerComponent? component = null)
  {
    if (!this.Resolve<HungerComponent>(uid, ref component))
      return;
    this.SetAuthoritativeHungerValue((Entity<HungerComponent>) (uid, component), amount);
    this.UpdateCurrentThreshold(uid, component);
  }

  private void SetAuthoritativeHungerValue(Entity<HungerComponent> entity, float value)
  {
    entity.Comp.LastAuthoritativeHungerChangeTime = this._timing.CurTime;
    entity.Comp.LastAuthoritativeHungerValue = HungerSystem.ClampHungerWithinThresholds(entity.Comp, value);
    this.DirtyField<HungerComponent>(entity.Owner, entity.Comp, "LastAuthoritativeHungerChangeTime");
    this.DirtyField<HungerComponent>(entity.Owner, entity.Comp, "LastAuthoritativeHungerValue");
  }

  private void UpdateCurrentThreshold(EntityUid uid, HungerComponent? component = null)
  {
    if (!this.Resolve<HungerComponent>(uid, ref component))
      return;
    HungerThreshold hungerThreshold = this.GetHungerThreshold(component);
    if (hungerThreshold == component.CurrentThreshold)
      return;
    component.CurrentThreshold = hungerThreshold;
    this.DirtyField<HungerComponent>(uid, component, "CurrentThreshold");
    this.DoHungerThresholdEffects(uid, component);
  }

  private void DoHungerThresholdEffects(EntityUid uid, HungerComponent? component = null, bool force = false)
  {
    if (!this.Resolve<HungerComponent>(uid, ref component) || component.CurrentThreshold == component.LastThreshold && !force)
      return;
    if (this.GetMovementThreshold(component.CurrentThreshold) != this.GetMovementThreshold(component.LastThreshold))
      this._movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
    ProtoId<AlertPrototype> alertType;
    if (component.HungerThresholdAlerts.TryGetValue(component.CurrentThreshold, out alertType))
      this._alerts.ShowAlert(uid, alertType);
    else
      this._alerts.ClearAlertCategory(uid, component.HungerAlertCategory);
    float num;
    if (component.HungerThresholdDecayModifiers.TryGetValue(component.CurrentThreshold, out num))
    {
      component.ActualDecayRate = component.BaseDecayRate * num;
      this.DirtyField<HungerComponent>(uid, component, "ActualDecayRate");
      this.SetAuthoritativeHungerValue((Entity<HungerComponent>) (uid, component), this.GetHunger(component));
    }
    component.LastThreshold = component.CurrentThreshold;
    this.DirtyField<HungerComponent>(uid, component, "LastThreshold");
  }

  private void DoContinuousHungerEffects(EntityUid uid, HungerComponent? component = null)
  {
    if (!this.Resolve<HungerComponent>(uid, ref component) || component.CurrentThreshold > HungerThreshold.Starving)
      return;
    DamageSpecifier starvationDamage = component.StarvationDamage;
    if (starvationDamage == null || this._mobState.IsDead(uid))
      return;
    this._damageable.TryChangeDamage(new EntityUid?(uid), starvationDamage, true, false);
  }

  public HungerThreshold GetHungerThreshold(HungerComponent component, float? food = null)
  {
    food.GetValueOrDefault();
    if (!food.HasValue)
      food = new float?(this.GetHunger(component));
    HungerThreshold hungerThreshold = HungerThreshold.Dead;
    float threshold1 = component.Thresholds[HungerThreshold.Overfed];
    foreach (KeyValuePair<HungerThreshold, float> threshold2 in component.Thresholds)
    {
      if ((double) threshold2.Value <= (double) threshold1)
      {
        double num = (double) threshold2.Value;
        float? nullable = food;
        double valueOrDefault = (double) nullable.GetValueOrDefault();
        if (num >= valueOrDefault & nullable.HasValue)
        {
          hungerThreshold = threshold2.Key;
          threshold1 = threshold2.Value;
        }
      }
    }
    return hungerThreshold;
  }

  public bool IsHungerBelowState(
    EntityUid uid,
    HungerThreshold threshold,
    float? food = null,
    HungerComponent? comp = null)
  {
    return this.Resolve<HungerComponent>(uid, ref comp) && this.GetHungerThreshold(comp, food) < threshold;
  }

  private bool GetMovementThreshold(HungerThreshold threshold)
  {
    switch (threshold)
    {
      case HungerThreshold.Dead:
      case HungerThreshold.Starving:
      case HungerThreshold.Peckish:
        return false;
      case HungerThreshold.Okay:
      case HungerThreshold.Overfed:
        return true;
      default:
        throw new ArgumentOutOfRangeException(nameof (threshold), (object) threshold, (string) null);
    }
  }

  public bool TryGetStatusIconPrototype(
    HungerComponent component,
    [NotNullWhen(true)] out SatiationIconPrototype? prototype)
  {
    switch (component.CurrentThreshold)
    {
      case HungerThreshold.Starving:
        this._prototype.TryIndex<SatiationIconPrototype>(HungerSystem.HungerIconStarvingId, out prototype);
        break;
      case HungerThreshold.Peckish:
        this._prototype.TryIndex<SatiationIconPrototype>(HungerSystem.HungerIconPeckishId, out prototype);
        break;
      case HungerThreshold.Overfed:
        this._prototype.TryIndex<SatiationIconPrototype>(HungerSystem.HungerIconOverfedId, out prototype);
        break;
      default:
        prototype = (SatiationIconPrototype) null;
        break;
    }
    return prototype != null;
  }

  private static float ClampHungerWithinThresholds(HungerComponent component, float hungerValue)
  {
    return Math.Clamp(hungerValue, component.Thresholds[HungerThreshold.Dead], component.Thresholds[HungerThreshold.Overfed]);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<HungerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HungerComponent>();
    EntityUid uid;
    HungerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(this._timing.CurTime < comp1.NextThresholdUpdateTime))
      {
        comp1.NextThresholdUpdateTime = this._timing.CurTime + comp1.ThresholdUpdateRate;
        this.UpdateCurrentThreshold(uid, comp1);
        this.DoContinuousHungerEffects(uid, comp1);
      }
    }
  }
}
