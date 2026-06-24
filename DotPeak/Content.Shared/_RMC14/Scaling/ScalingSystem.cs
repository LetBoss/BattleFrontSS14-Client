// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Scaling.ScalingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Requisitions.Components;
using Content.Shared._RMC14.Vendors;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Scaling;

public sealed class ScalingSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedJobSystem _job;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedRequisitionsSystem _requisitions;
  [Dependency]
  private SharedCMAutomatedVendorSystem _rmcAutomatedVendor;
  [Dependency]
  private GunIFFSystem _gunIFF;
  private float _marineScalingBonus;

  public float MarineScalingNormal { get; private set; }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawnComplete));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCMarineScalingNormal, (Action<float>) (v => this.MarineScalingNormal = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCMarineScalingBonus, (Action<float>) (v => this._marineScalingBonus = v), true);
  }

  private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
  {
    if (!ev.LateJoin || !this.HasComp<MarineComponent>(ev.Mob) || this.HasComp<RMCAdminSpawnedComponent>(ev.Mob))
      return;
    string jobId = ev.JobId;
    JobPrototype prototype;
    if (jobId == null || !this._prototypes.TryIndex<JobPrototype>(jobId, out prototype) || (double) prototype.RoleWeight <= 0.0)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<MarineScalingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MarineScalingComponent>();
    EntityUid uid;
    MarineScalingComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      double num = this._gameTicker.RoundDuration().TotalSeconds * 10.0;
      comp1.Scale += (double) prototype.RoleWeight * (0.25 + 0.75 / (1.0 + num / 20000.0)) / (double) this.MarineScalingNormal;
      double Delta = comp1.Scale - comp1.MaxScale;
      if (Delta > 0.0)
      {
        comp1.MaxScale = comp1.Scale;
        MarineScaleChangedEvent message = new MarineScaleChangedEvent(comp1.MaxScale, Delta);
        this.RaiseLocalEvent<MarineScaleChangedEvent>(ref message);
      }
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  public bool TryGetScaling(out Entity<MarineScalingComponent> scaling)
  {
    EntityUid uid;
    MarineScalingComponent comp1;
    if (this.EntityQueryEnumerator<MarineScalingComponent>().MoveNext(out uid, out comp1))
    {
      scaling = (Entity<MarineScalingComponent>) (uid, comp1);
      return true;
    }
    scaling = new Entity<MarineScalingComponent>();
    return false;
  }

  private Entity<MarineScalingComponent> EnsureScaling()
  {
    EntityUid uid1;
    MarineScalingComponent comp1;
    if (this.EntityQueryEnumerator<MarineScalingComponent>().MoveNext(out uid1, out comp1))
      return (Entity<MarineScalingComponent>) (uid1, comp1);
    EntityUid uid2 = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
    MarineScalingComponent scalingComponent = this.EnsureComp<MarineScalingComponent>(uid2);
    return (Entity<MarineScalingComponent>) (uid2, scalingComponent);
  }

  public void TryStartScaling(EntProtoId<IFFFactionComponent> faction)
  {
    Entity<MarineScalingComponent> ent = this.EnsureScaling();
    if (ent.Comp.Started)
      return;
    ent.Comp.Started = true;
    this.Dirty<MarineScalingComponent>(ent);
    float marineScalingBonus = this._marineScalingBonus;
    Robust.Shared.GameObjects.EntityQueryEnumerator<UserIFFComponent, ActorComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<UserIFFComponent, ActorComponent>();
    EntityUid uid1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out UserIFFComponent _, out ActorComponent _))
    {
      EntityUid mindId;
      JobPrototype prototype;
      if (this._gunIFF.IsInFaction(uid1, faction) && this._mind.TryGetMind(uid1, out mindId, out MindComponent _) && this._job.MindTryGetJob(new EntityUid?(mindId), out prototype))
        marineScalingBonus += prototype.RoleWeight;
    }
    ent.Comp.Scale = (double) Math.Max(1f, marineScalingBonus / this.MarineScalingNormal);
    ent.Comp.MaxScale = ent.Comp.Scale;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RequisitionsAccountComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RequisitionsAccountComponent>();
    EntityUid uid2;
    RequisitionsAccountComponent comp1_1;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_1))
      this._requisitions.StartAccount((Entity<RequisitionsAccountComponent>) (uid2, comp1_1), ent.Comp.Scale, marineScalingBonus);
    Robust.Shared.GameObjects.EntityQueryEnumerator<CMAutomatedVendorComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<CMAutomatedVendorComponent>();
    EntityUid uid3;
    CMAutomatedVendorComponent comp1_2;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_2))
    {
      double num1 = ent.Comp.Scale;
      if (!comp1_2.Scaling)
        num1 = 1.0;
      foreach (CMVendorSection section in comp1_2.Sections)
      {
        for (int index = 0; index < section.Entries.Count; ++index)
        {
          CMVendorEntry entry = section.Entries[index];
          int? amount = entry.Amount;
          if (amount.HasValue)
          {
            int valueOrDefault = amount.GetValueOrDefault();
            if (!entry.Box.HasValue)
            {
              int num2 = (int) Math.Round((double) valueOrDefault * num1);
              section.Entries[index] = entry with
              {
                Amount = new int?(num2),
                Max = new int?(num2)
              };
              this._rmcAutomatedVendor.AmountUpdated((Entity<CMAutomatedVendorComponent>) (uid3, comp1_2), entry);
            }
          }
        }
      }
      this.Dirty(uid3, (IComponent) comp1_2);
    }
  }

  public int GetAliveHumanoids()
  {
    int aliveHumanoids = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<MarineComponent, MobStateComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MarineComponent, MobStateComponent>();
    MobStateComponent comp2;
    while (entityQueryEnumerator.MoveNext(out MarineComponent _, out comp2))
    {
      if (comp2.CurrentState != MobState.Dead)
        ++aliveHumanoids;
    }
    return aliveHumanoids;
  }
}
