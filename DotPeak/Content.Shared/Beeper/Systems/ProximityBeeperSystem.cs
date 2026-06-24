// Decompiled with JetBrains decompiler
// Type: Content.Shared.Beeper.Systems.ProximityBeeperSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Beeper.Components;
using Content.Shared.FixedPoint;
using Content.Shared.ProximityDetection;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Beeper.Systems;

public sealed class ProximityBeeperSystem : EntitySystem
{
  [Dependency]
  private BeeperSystem _beeper;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ProximityBeeperComponent, NewProximityTargetEvent>(new ComponentEventRefHandler<ProximityBeeperComponent, NewProximityTargetEvent>((object) this, __methodptr(OnNewProximityTarget)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ProximityBeeperComponent, ProximityTargetUpdatedEvent>(new ComponentEventRefHandler<ProximityBeeperComponent, ProximityTargetUpdatedEvent>((object) this, __methodptr(OnProximityTargetUpdate)), (Type[]) null, (Type[]) null);
  }

  private void OnProximityTargetUpdate(
    EntityUid owner,
    ProximityBeeperComponent proxBeeper,
    ref ProximityTargetUpdatedEvent args)
  {
    BeeperComponent beeper;
    if (!this.TryComp<BeeperComponent>(owner, ref beeper))
      return;
    this._beeper.SetIntervalScaling(owner, (FixedPoint2) (args.Distance / args.Detector.Comp.Range), beeper);
  }

  private void OnNewProximityTarget(
    EntityUid owner,
    ProximityBeeperComponent proxBeeper,
    ref NewProximityTargetEvent args)
  {
    this._beeper.SetMute(owner, !args.Target.HasValue);
  }
}
