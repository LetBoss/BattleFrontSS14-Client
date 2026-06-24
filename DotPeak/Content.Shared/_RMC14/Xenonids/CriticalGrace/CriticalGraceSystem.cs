// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.CriticalGrace.CriticalGraceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.CriticalGrace;

public sealed class CriticalGraceSystem : EntitySystem
{
  [Dependency]
  private MobThresholdSystem _mobThresholds;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CriticalGraceTimeComponent, UpdateMobStateEvent>(new EntityEventRefHandler<CriticalGraceTimeComponent, UpdateMobStateEvent>(this.OnCriticalGraceMobState), after: new Type[2]
    {
      typeof (MobThresholdSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<InCriticalGraceComponent, ComponentShutdown>(new EntityEventRefHandler<InCriticalGraceComponent, ComponentShutdown>(this.OnInCriticalGraceRemove));
  }

  private void OnCriticalGraceMobState(
    Entity<CriticalGraceTimeComponent> ent,
    ref UpdateMobStateEvent args)
  {
    if (args.State != MobState.Critical || !this._mobState.HasState((EntityUid) ent, MobState.Critical))
      return;
    InCriticalGraceComponent comp;
    if (this.TryComp<InCriticalGraceComponent>((EntityUid) ent, out comp))
    {
      if (comp.Over)
        return;
      args.State = MobState.Alive;
    }
    else
    {
      if (!this._mobState.IsAlive((EntityUid) ent))
        return;
      InCriticalGraceComponent criticalGraceComponent = this.EnsureComp<InCriticalGraceComponent>((EntityUid) ent);
      GetCriticalGraceTimeEvent args1 = new GetCriticalGraceTimeEvent(ent.Comp.GraceDuration);
      this.RaiseLocalEvent<GetCriticalGraceTimeEvent>((EntityUid) ent, ref args1);
      TimeSpan timeSpan = this._timing.CurTime + args1.Time;
      criticalGraceComponent.GraceEndsAt = timeSpan;
      args.State = MobState.Alive;
    }
  }

  private void OnInCriticalGraceRemove(
    Entity<InCriticalGraceComponent> ent,
    ref ComponentShutdown args)
  {
    MobThresholdsComponent comp;
    if (this.TerminatingOrDeleted((EntityUid) ent) || !this.TryComp<MobThresholdsComponent>((EntityUid) ent, out comp))
      return;
    this._mobThresholds.VerifyThresholds((EntityUid) ent, comp);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<InCriticalGraceComponent> entityQueryEnumerator = this.EntityQueryEnumerator<InCriticalGraceComponent>();
    EntityUid uid;
    InCriticalGraceComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.GraceEndsAt))
      {
        comp1.Over = true;
        this.RemCompDeferred<InCriticalGraceComponent>(uid);
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }
}
