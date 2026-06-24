// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tools.RMCDeviceBreakerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Communications;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Sensor;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Tools;

public sealed class RMCDeviceBreakerSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doafter;
  [Dependency]
  private SharedRMCPowerSystem _power;
  [Dependency]
  private SharedDestructibleSystem _destroy;
  [Dependency]
  private SensorTowerSystem _sensor;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCDeviceBreakerComponent, RMCDeviceBreakerDoAfterEvent>(new EntityEventRefHandler<RMCDeviceBreakerComponent, RMCDeviceBreakerDoAfterEvent>(this.OnDeviceBreakerDoafter));
  }

  private void OnDeviceBreakerDoafter(
    Entity<RMCDeviceBreakerComponent> breaker,
    ref RMCDeviceBreakerDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    target = args.Target;
    if (!this.CanBreak(target.Value))
      return;
    args.Handled = true;
    target = args.Target;
    this.Break(target.Value, args.User);
    this._audio.PlayPredicted(breaker.Comp.UseSound, (EntityUid) breaker, new EntityUid?(args.User));
    if (!breaker.Comp.Repeat)
      return;
    target = args.Target;
    if (!this.CanBreak(target.Value))
      return;
    this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, breaker.Comp.DoAfterTime, (DoAfterEvent) new RMCDeviceBreakerDoAfterEvent(), args.Used, args.Target, args.Used)
    {
      BreakOnMove = true,
      RequireCanInteract = true,
      BreakOnHandChange = true,
      DuplicateCondition = DuplicateConditions.SameTool
    });
  }

  private bool CanBreak(EntityUid target)
  {
    RMCFusionReactorComponent comp1;
    CommunicationsTowerComponent comp2;
    SensorTowerComponent comp3;
    return this.TryComp<RMCFusionReactorComponent>(target, out comp1) && comp1.State != RMCFusionReactorState.Weld || this.TryComp<CommunicationsTowerComponent>(target, out comp2) && comp2.State != CommunicationsTowerState.Broken || this.TryComp<SensorTowerComponent>(target, out comp3) && comp3.State != SensorTowerState.Weld;
  }

  private void Break(EntityUid target, EntityUid user)
  {
    RMCFusionReactorComponent comp1;
    if (this.TryComp<RMCFusionReactorComponent>(target, out comp1) && comp1.State != RMCFusionReactorState.Weld)
      this._power.DestroyReactor((Entity<RMCFusionReactorComponent>) (target, comp1), new EntityUid?(user));
    CommunicationsTowerComponent comp2;
    if (this.TryComp<CommunicationsTowerComponent>(target, out comp2) && comp2.State != CommunicationsTowerState.Broken)
      this._destroy.BreakEntity(target);
    SensorTowerComponent comp3;
    if (!this.TryComp<SensorTowerComponent>(target, out comp3) || comp3.State == SensorTowerState.Weld)
      return;
    this._sensor.SensorTowerIncrementalDestroy((Entity<SensorTowerComponent>) (target, comp3));
  }
}
