// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.TeslaCoil.TeslaCoilSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Sentry.TeslaCoil;

public sealed class TeslaCoilSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private RMCDazedSystem _dazed;
  [Dependency]
  private SentrySystem _sentrySystem;
  [Dependency]
  private SharedSentryTargetingSystem _targeting;
  private readonly HashSet<EntityUid> _potentialTargets = new HashSet<EntityUid>();
  private readonly List<EntityUid> _validTargets = new List<EntityUid>();

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCTeslaCoilComponent, SentryComponent, TransformComponent, SentryTargetingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCTeslaCoilComponent, SentryComponent, TransformComponent, SentryTargetingComponent>();
    EntityUid uid;
    RMCTeslaCoilComponent comp1;
    SentryComponent comp2;
    TransformComponent comp3;
    SentryTargetingComponent comp4;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3, out comp4))
    {
      if (comp2.Mode == SentryMode.On && comp3.Anchored && !(curTime < comp1.LastFired + comp1.FireDelay))
      {
        this._potentialTargets.Clear();
        this._validTargets.Clear();
        this._entityLookup.GetEntitiesInRange(comp3.Coordinates, comp1.Range, this._potentialTargets, LookupFlags.Uncontained);
        int num = 0;
        foreach (EntityUid potentialTarget in this._potentialTargets)
        {
          if (num < comp1.MaxTargets)
          {
            if (!(potentialTarget == uid) && this._interaction.InRangeUnobstructed((Entity<TransformComponent>) uid, (Entity<TransformComponent>) potentialTarget, comp1.Range) && this._targeting.IsValidTarget((Entity<SentryTargetingComponent>) (uid, comp4), potentialTarget))
            {
              bool flag = false;
              SentryComponent comp5;
              if (this.TryComp<SentryComponent>(potentialTarget, out comp5))
              {
                if (comp5.Mode == SentryMode.On)
                  flag = true;
              }
              else
              {
                MobStateComponent comp6;
                if (this.TryComp<MobStateComponent>(potentialTarget, out comp6) && this._mobState.IsAlive(potentialTarget, comp6))
                  flag = true;
              }
              if (flag)
              {
                this._validTargets.Add(potentialTarget);
                ++num;
              }
            }
          }
          else
            break;
        }
        comp1.LastFired = curTime;
        if (this._validTargets.Count > 0)
        {
          this.Dirty(uid, (IComponent) comp1);
          foreach (EntityUid validTarget in this._validTargets)
            this.ApplyTeslaEffects((Entity<RMCTeslaCoilComponent>) (uid, comp1), validTarget);
        }
      }
    }
  }

  private void ApplyTeslaEffects(Entity<RMCTeslaCoilComponent> tesla, EntityUid target)
  {
    RMCTeslaCoilComponent comp1 = tesla.Comp;
    SentryComponent comp2;
    if (this.TryComp<SentryComponent>(target, out comp2) && comp2.Mode == SentryMode.On)
    {
      this._sentrySystem.TrySetMode((Entity<SentryComponent>) (target, comp2), SentryMode.Off);
    }
    else
    {
      RMCSizes size;
      if (comp1.StunDuration > TimeSpan.Zero && this._sizeStun.TryGetSize(target, out size) && size <= RMCSizes.Xeno)
        this._stun.TryParalyze(target, comp1.StunDuration, true);
      if (comp1.SlowDuration > TimeSpan.Zero)
        this._slow.TrySuperSlowdown(target, comp1.SlowDuration);
      if (comp1.DazeDuration > TimeSpan.Zero)
        this._dazed.TryDaze(target, comp1.DazeDuration, true);
    }
    SharedColorFlashEffectSystem colorFlash = this._colorFlash;
    Color cyan = Color.Cyan;
    List<EntityUid> entities = new List<EntityUid>();
    entities.Add(target);
    Filter filter = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager);
    colorFlash.RaiseEffect(cyan, entities, filter);
    if (string.IsNullOrEmpty((string) comp1.TeslaBeamProto))
      return;
    this._line.TryCreateLine(tesla.Owner, target, (string) comp1.TeslaBeamProto, out List<EntityUid> _);
  }
}
