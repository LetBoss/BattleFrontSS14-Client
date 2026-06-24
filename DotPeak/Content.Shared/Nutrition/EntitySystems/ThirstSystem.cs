// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.ThirstSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Movement.Components;
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

public sealed class ThirstSystem : EntitySystem
{
  private static readonly bool DisableThirstMovespeedPenalty = true;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private MovementSpeedModifierSystem _movement;
  [Dependency]
  private SharedJetpackSystem _jetpack;
  private static readonly ProtoId<SatiationIconPrototype> ThirstIconOverhydratedId = (ProtoId<SatiationIconPrototype>) "ThirstIconOverhydrated";
  private static readonly ProtoId<SatiationIconPrototype> ThirstIconThirstyId = (ProtoId<SatiationIconPrototype>) "ThirstIconThirsty";
  private static readonly ProtoId<SatiationIconPrototype> ThirstIconParchedId = (ProtoId<SatiationIconPrototype>) "ThirstIconParched";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ThirstComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<ThirstComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed));
    this.SubscribeLocalEvent<ThirstComponent, MapInitEvent>(new ComponentEventHandler<ThirstComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ThirstComponent, RejuvenateEvent>(new ComponentEventHandler<ThirstComponent, RejuvenateEvent>(this.OnRejuvenate));
  }

  private void OnMapInit(EntityUid uid, ThirstComponent component, MapInitEvent args)
  {
    if ((double) component.CurrentThirst < 0.0)
    {
      component.CurrentThirst = (float) this._random.Next((int) component.ThirstThresholds[ThirstThreshold.Thirsty] + 10, (int) component.ThirstThresholds[ThirstThreshold.Okay] - 1);
      this.DirtyField<ThirstComponent>(uid, component, "CurrentThirst");
    }
    component.NextUpdateTime = this._timing.CurTime;
    component.CurrentThirstThreshold = this.GetThirstThreshold(component, component.CurrentThirst);
    component.LastThirstThreshold = ThirstThreshold.Okay;
    this.UpdateEffects(uid, component);
    this.DirtyFields<ThirstComponent>(uid, component, (MetaDataComponent) null, "NextUpdateTime", "CurrentThirstThreshold", "LastThirstThreshold");
    MovementSpeedModifierComponent comp;
    this.TryComp<MovementSpeedModifierComponent>(uid, out comp);
    this._movement.RefreshMovementSpeedModifiers(uid, comp);
  }

  private void OnRefreshMovespeed(
    EntityUid uid,
    ThirstComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    if (ThirstSystem.DisableThirstMovespeedPenalty || this._jetpack.IsUserFlying(uid))
      return;
    float num = component.CurrentThirstThreshold <= ThirstThreshold.Parched ? 0.75f : 1f;
    args.ModifySpeed(num, num);
  }

  private void OnRejuvenate(EntityUid uid, ThirstComponent component, RejuvenateEvent args)
  {
    this.SetThirst(uid, component, component.ThirstThresholds[ThirstThreshold.Okay]);
  }

  private ThirstThreshold GetThirstThreshold(ThirstComponent component, float amount)
  {
    ThirstThreshold thirstThreshold1 = ThirstThreshold.Dead;
    float thirstThreshold2 = component.ThirstThresholds[ThirstThreshold.OverHydrated];
    foreach (KeyValuePair<ThirstThreshold, float> thirstThreshold3 in component.ThirstThresholds)
    {
      if ((double) thirstThreshold3.Value <= (double) thirstThreshold2 && (double) thirstThreshold3.Value >= (double) amount)
      {
        thirstThreshold1 = thirstThreshold3.Key;
        thirstThreshold2 = thirstThreshold3.Value;
      }
    }
    return thirstThreshold1;
  }

  public void ModifyThirst(EntityUid uid, ThirstComponent component, float amount)
  {
    this.SetThirst(uid, component, component.CurrentThirst + amount);
  }

  public void SetThirst(EntityUid uid, ThirstComponent component, float amount)
  {
    component.CurrentThirst = Math.Clamp(amount, component.ThirstThresholds[ThirstThreshold.Dead], component.ThirstThresholds[ThirstThreshold.OverHydrated]);
    this.DirtyField<ThirstComponent>(uid, component, "CurrentThirst");
  }

  private bool IsMovementThreshold(ThirstThreshold threshold)
  {
    switch (threshold)
    {
      case ThirstThreshold.Dead:
      case ThirstThreshold.Parched:
        return true;
      case ThirstThreshold.Thirsty:
      case ThirstThreshold.Okay:
      case ThirstThreshold.OverHydrated:
        return false;
      default:
        throw new ArgumentOutOfRangeException(nameof (threshold), (object) threshold, (string) null);
    }
  }

  public bool TryGetStatusIconPrototype(
    ThirstComponent component,
    [NotNullWhen(true)] out SatiationIconPrototype? prototype)
  {
    switch (component.CurrentThirstThreshold)
    {
      case ThirstThreshold.Parched:
        this._prototype.TryIndex<SatiationIconPrototype>(ThirstSystem.ThirstIconParchedId, out prototype);
        break;
      case ThirstThreshold.Thirsty:
        this._prototype.TryIndex<SatiationIconPrototype>(ThirstSystem.ThirstIconThirstyId, out prototype);
        break;
      case ThirstThreshold.OverHydrated:
        this._prototype.TryIndex<SatiationIconPrototype>(ThirstSystem.ThirstIconOverhydratedId, out prototype);
        break;
      default:
        prototype = (SatiationIconPrototype) null;
        break;
    }
    return prototype != null;
  }

  private void UpdateEffects(EntityUid uid, ThirstComponent component)
  {
    MovementSpeedModifierComponent comp;
    if (this.IsMovementThreshold(component.LastThirstThreshold) != this.IsMovementThreshold(component.CurrentThirstThreshold) && this.TryComp<MovementSpeedModifierComponent>(uid, out comp))
      this._movement.RefreshMovementSpeedModifiers(uid, comp);
    ProtoId<AlertPrototype> alertType;
    if (ThirstComponent.ThirstThresholdAlertTypes.TryGetValue(component.CurrentThirstThreshold, out alertType))
      this._alerts.ShowAlert(uid, alertType);
    else
      this._alerts.ClearAlertCategory(uid, component.ThirstyCategory);
    this.DirtyField<ThirstComponent>(uid, component, "LastThirstThreshold");
    this.DirtyField<ThirstComponent>(uid, component, "ActualDecayRate");
    switch (component.CurrentThirstThreshold)
    {
      case ThirstThreshold.Dead:
        break;
      case ThirstThreshold.Parched:
        this._movement.RefreshMovementSpeedModifiers(uid);
        component.LastThirstThreshold = component.CurrentThirstThreshold;
        component.ActualDecayRate = component.BaseDecayRate * 0.6f;
        break;
      case ThirstThreshold.Thirsty:
        component.LastThirstThreshold = component.CurrentThirstThreshold;
        component.ActualDecayRate = component.BaseDecayRate * 0.8f;
        break;
      case ThirstThreshold.Okay:
        component.LastThirstThreshold = component.CurrentThirstThreshold;
        component.ActualDecayRate = component.BaseDecayRate;
        break;
      case ThirstThreshold.OverHydrated:
        component.LastThirstThreshold = component.CurrentThirstThreshold;
        component.ActualDecayRate = component.BaseDecayRate * 1.2f;
        break;
      default:
        this.Log.Error($"No thirst threshold found for {component.CurrentThirstThreshold}");
        throw new ArgumentOutOfRangeException($"No thirst threshold found for {component.CurrentThirstThreshold}");
    }
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<ThirstComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ThirstComponent>();
    EntityUid uid;
    ThirstComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(this._timing.CurTime < comp1.NextUpdateTime))
      {
        comp1.NextUpdateTime += comp1.UpdateRate;
        this.ModifyThirst(uid, comp1, -comp1.ActualDecayRate);
        ThirstThreshold thirstThreshold = this.GetThirstThreshold(comp1, comp1.CurrentThirst);
        if (thirstThreshold != comp1.CurrentThirstThreshold)
        {
          comp1.CurrentThirstThreshold = thirstThreshold;
          this.UpdateEffects(uid, comp1);
        }
      }
    }
  }
}
