// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Energy.XenoEnergySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.TrainingDummy;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Content.Shared.Standing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Energy;

public sealed class XenoEnergySystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private StandingStateSystem _stand;

  private void OnXenoPlasmaMapInit(Entity<XenoEnergyComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAlert(ent);
  }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoEnergyComponent, MapInitEvent>(new EntityEventRefHandler<XenoEnergyComponent, MapInitEvent>(this.OnXenoEnergyMapInit));
    this.SubscribeLocalEvent<XenoEnergyComponent, ComponentRemove>(new EntityEventRefHandler<XenoEnergyComponent, ComponentRemove>(this.OnXenoEnergyRemove));
    this.SubscribeLocalEvent<XenoEnergyComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoEnergyComponent, MeleeHitEvent>(this.OnMeleeHit));
    this.SubscribeLocalEvent<XenoEnergyComponent, XenoProjectileHitUserEvent>(new EntityEventRefHandler<XenoEnergyComponent, XenoProjectileHitUserEvent>(this.OnXenoProjectileHitUser));
    this.SubscribeLocalEvent<XenoEnergyComponent, RejuvenateEvent>(new EntityEventRefHandler<XenoEnergyComponent, RejuvenateEvent>(this.OnRejuvenate));
    this.SubscribeLocalEvent<XenoActionEnergyComponent, RMCActionUseAttemptEvent>(new EntityEventRefHandler<XenoActionEnergyComponent, RMCActionUseAttemptEvent>(this.OnXenoActionEnergyUseAttempt));
    this.SubscribeLocalEvent<XenoActionEnergyComponent, RMCActionUseEvent>(new EntityEventRefHandler<XenoActionEnergyComponent, RMCActionUseEvent>(this.OnXenoActionEnergyUse));
  }

  private void OnXenoEnergyMapInit(Entity<XenoEnergyComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAlert(ent);
  }

  private void OnXenoEnergyRemove(Entity<XenoEnergyComponent> ent, ref ComponentRemove args)
  {
    this._alerts.ClearAlert((EntityUid) ent, ent.Comp.Alert);
  }

  private void OnMeleeHit(Entity<XenoEnergyComponent> xeno, ref MeleeHitEvent args)
  {
    if (!args.IsHit)
      return;
    bool flag1 = false;
    bool flag2 = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      VictimInfectedComponent comp;
      if (this._xeno.CanAbilityAttackTarget(xeno.Owner, hitEntity) && (!xeno.Comp.IgnoreLateInfected || !this.TryComp<VictimInfectedComponent>(hitEntity, out comp) || comp.CurrentStage < comp.FinalSymptomsStart))
      {
        if (this.HasComp<RMCTrainingDummyComponent>(hitEntity))
          return;
        flag1 = true;
        if (this._stand.IsDown(hitEntity))
        {
          flag2 = true;
          break;
        }
        break;
      }
    }
    if (!flag1)
      return;
    this.AddEnergy(xeno, flag2 ? xeno.Comp.GainAttackDowned : xeno.Comp.GainAttack);
    this.UpdateAlert(xeno);
  }

  private void OnXenoProjectileHitUser(
    Entity<XenoEnergyComponent> xeno,
    ref XenoProjectileHitUserEvent args)
  {
    if (!xeno.Comp.GainOnProjectiles || !this._xeno.CanAbilityAttackTarget((EntityUid) xeno, args.Hit))
      return;
    this.AddEnergy(xeno, xeno.Comp.GainAttack);
    this.UpdateAlert(xeno);
  }

  private void OnRejuvenate(Entity<XenoEnergyComponent> ent, ref RejuvenateEvent args)
  {
    this.AddEnergy(ent, ent.Comp.Max);
    this.UpdateAlert(ent);
  }

  private void UpdateAlert(Entity<XenoEnergyComponent> xeno)
  {
    float actual = MathF.Max(0.0f, (float) xeno.Comp.Current);
    short maxSeverity = this._alerts.GetMaxSeverity(xeno.Comp.Alert);
    int num = (int) maxSeverity - ContentHelpers.RoundToLevels((double) actual, (double) xeno.Comp.Max, (int) maxSeverity + 1);
    string str1 = $"{xeno.Comp.Current.ToString()} / {xeno.Comp.Max.ToString()}";
    AlertsSystem alerts = this._alerts;
    EntityUid euid = (EntityUid) xeno;
    ProtoId<AlertPrototype> alert = xeno.Comp.Alert;
    short? severity = new short?((short) num);
    string str2 = str1;
    (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
    string dynamicMessage = str2;
    alerts.ShowAlert(euid, alert, severity, cooldown, dynamicMessage: dynamicMessage);
  }

  private void OnXenoActionEnergyUseAttempt(
    Entity<XenoActionEnergyComponent> action,
    ref RMCActionUseAttemptEvent args)
  {
    if (args.Cancelled || this.HasEnergyPopup((Entity<XenoEnergyComponent>) args.User, action.Comp.Cost))
      return;
    args.Cancelled = true;
  }

  private void OnXenoActionEnergyUse(
    Entity<XenoActionEnergyComponent> action,
    ref RMCActionUseEvent args)
  {
    XenoEnergyComponent comp;
    if (!this.TryComp<XenoEnergyComponent>(args.User, out comp))
      return;
    this.RemoveEnergy((Entity<XenoEnergyComponent>) (args.User, comp), action.Comp.Cost);
  }

  public void AddEnergy(Entity<XenoEnergyComponent> xeno, int energy, bool popup = true)
  {
    XenoEnergyGainAttemptEvent args1 = new XenoEnergyGainAttemptEvent();
    this.RaiseLocalEvent<XenoEnergyGainAttemptEvent>((EntityUid) xeno, args1);
    if (args1.Cancelled)
      return;
    if (popup && xeno.Comp.Current < xeno.Comp.Max && energy > 0)
      this._popup.PopupClient(this.Loc.GetString(xeno.Comp.PopupGain), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    xeno.Comp.Current = Math.Min(xeno.Comp.Max, xeno.Comp.Current + energy);
    this.Dirty<XenoEnergyComponent>(xeno);
    this.UpdateAlert(xeno);
    XenoEnergyChangedEvent args2 = new XenoEnergyChangedEvent((FixedPoint2) xeno.Comp.Current);
    this.RaiseLocalEvent<XenoEnergyChangedEvent>((EntityUid) xeno, ref args2);
  }

  public bool HasEnergy(Entity<XenoEnergyComponent> xeno, int energy)
  {
    return xeno.Comp.Current >= energy;
  }

  public bool HasEnergyPopup(Entity<XenoEnergyComponent?> xeno, int energy, bool predicted = true)
  {
    if (!this.Resolve<XenoEnergyComponent>((EntityUid) xeno, ref xeno.Comp, false))
    {
      DoPopup();
      return false;
    }
    if (this.HasEnergy((Entity<XenoEnergyComponent>) ((EntityUid) xeno, xeno.Comp), energy))
      return true;
    DoPopup();
    return false;

    void DoPopup()
    {
      string message = this.Loc.GetString(xeno.Comp != null ? xeno.Comp.PopupNotEnough : "rmc-xeno-not-enough-energy");
      if (predicted)
        this._popup.PopupClient(message, (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      else
        this._popup.PopupEntity(message, (EntityUid) xeno, (EntityUid) xeno, PopupType.SmallCaution);
    }
  }

  public void RemoveEnergy(Entity<XenoEnergyComponent?> xeno, int plasma)
  {
    if (!this.Resolve<XenoEnergyComponent>((EntityUid) xeno, ref xeno.Comp, false))
      return;
    xeno.Comp.Current = int.Max(0, xeno.Comp.Current - plasma);
    this.UpdateAlert((Entity<XenoEnergyComponent>) ((EntityUid) xeno, xeno.Comp));
    XenoEnergyChangedEvent args = new XenoEnergyChangedEvent((FixedPoint2) xeno.Comp.Current);
    this.RaiseLocalEvent<XenoEnergyChangedEvent>((EntityUid) xeno, ref args);
    this.Dirty<XenoEnergyComponent>(xeno);
  }

  public bool TryRemoveEnergy(Entity<XenoEnergyComponent?> xeno, int energy)
  {
    if (!this.Resolve<XenoEnergyComponent>((EntityUid) xeno, ref xeno.Comp, false) || !this.HasEnergy((Entity<XenoEnergyComponent>) ((EntityUid) xeno, xeno.Comp), energy))
      return false;
    this.RemoveEnergy((Entity<XenoEnergyComponent>) ((EntityUid) xeno, xeno.Comp), energy);
    return true;
  }

  public bool TryRemoveEnergyPopup(Entity<XenoEnergyComponent?> xeno, int energy)
  {
    if (!this.Resolve<XenoEnergyComponent>((EntityUid) xeno, ref xeno.Comp, false))
      return false;
    if (this.TryRemoveEnergy((Entity<XenoEnergyComponent>) ((EntityUid) xeno, xeno.Comp), energy))
      return true;
    this._popup.PopupClient(this.Loc.GetString(xeno.Comp.PopupNotEnough), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    return false;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoEnergyComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoEnergyComponent>();
    EntityUid uid;
    XenoEnergyComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!this._mobState.IsDead(uid) && !(curTime < comp1.NextGain))
      {
        comp1.NextGain = curTime + comp1.GainEvery;
        if (comp1.GenerationCap.HasValue)
        {
          int current = comp1.Current;
          int? generationCap = comp1.GenerationCap;
          int valueOrDefault = generationCap.GetValueOrDefault();
          if (!(current < valueOrDefault & generationCap.HasValue))
            goto label_6;
        }
        this.AddEnergy((Entity<XenoEnergyComponent>) (uid, comp1), comp1.Gain, false);
label_6:
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }
}
