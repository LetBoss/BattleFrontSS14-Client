// Decompiled with JetBrains decompiler
// Type: Content.Shared.Temperature.Systems.SharedTemperatureSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Temperature.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Temperature.Systems;

public sealed class SharedTemperatureSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;
  private static readonly TimeSpan SlowdownApplicationDelay = TimeSpan.FromSeconds(1.0);

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TemperatureSpeedComponent, OnTemperatureChangeEvent>(new EntityEventRefHandler<TemperatureSpeedComponent, OnTemperatureChangeEvent>(this.OnTemperatureChanged));
    this.SubscribeLocalEvent<TemperatureSpeedComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<TemperatureSpeedComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovementSpeedModifiers));
  }

  private void OnTemperatureChanged(
    Entity<TemperatureSpeedComponent> ent,
    ref OnTemperatureChangeEvent args)
  {
    foreach ((float key, float num) in ent.Comp.Thresholds)
    {
      if ((double) args.CurrentTemperature < (double) key && (double) args.LastTemperature > (double) key || (double) args.CurrentTemperature > (double) key && (double) args.LastTemperature < (double) key)
      {
        ent.Comp.NextSlowdownUpdate = new TimeSpan?(this._timing.CurTime + SharedTemperatureSystem.SlowdownApplicationDelay);
        ent.Comp.CurrentSpeedModifier = new float?(num);
        this.Dirty<TemperatureSpeedComponent>(ent);
        break;
      }
    }
    float num1 = ent.Comp.Thresholds.Max<KeyValuePair<float, float>>((Func<KeyValuePair<float, float>, float>) (p => p.Key));
    if ((double) args.CurrentTemperature <= (double) num1 || (double) args.LastTemperature >= (double) num1)
      return;
    ent.Comp.NextSlowdownUpdate = new TimeSpan?(this._timing.CurTime + SharedTemperatureSystem.SlowdownApplicationDelay);
    ent.Comp.CurrentSpeedModifier = new float?();
    this.Dirty<TemperatureSpeedComponent>(ent);
  }

  private void OnRefreshMovementSpeedModifiers(
    Entity<TemperatureSpeedComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (ent.Comp.NextSlowdownUpdate.HasValue || !ent.Comp.CurrentSpeedModifier.HasValue)
      return;
    args.ModifySpeed(ent.Comp.CurrentSpeedModifier.Value, ent.Comp.CurrentSpeedModifier.Value);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<TemperatureSpeedComponent, MovementSpeedModifierComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TemperatureSpeedComponent, MovementSpeedModifierComponent>();
    EntityUid uid;
    TemperatureSpeedComponent comp1;
    MovementSpeedModifierComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.NextSlowdownUpdate.HasValue)
      {
        TimeSpan curTime = this._timing.CurTime;
        TimeSpan? nextSlowdownUpdate = comp1.NextSlowdownUpdate;
        if ((nextSlowdownUpdate.HasValue ? (curTime < nextSlowdownUpdate.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          comp1.NextSlowdownUpdate = new TimeSpan?();
          this._movementSpeedModifier.RefreshMovementSpeedModifiers(uid, comp2);
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }
}
