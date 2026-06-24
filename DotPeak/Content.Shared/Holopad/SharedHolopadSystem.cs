// Decompiled with JetBrains decompiler
// Type: Content.Shared.Holopad.SharedHolopadSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Holopad;

public abstract class SharedHolopadSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public bool IsHolopadControlLocked(Entity<HolopadComponent> entity, EntityUid? user = null)
  {
    if (entity.Comp.ControlLockoutStartTime == TimeSpan.Zero || entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds((double) entity.Comp.ControlLockoutDuration) < this._timing.CurTime || !entity.Comp.ControlLockoutOwner.HasValue)
      return false;
    EntityUid? controlLockoutOwner = entity.Comp.ControlLockoutOwner;
    EntityUid? nullable = user;
    return (controlLockoutOwner.HasValue == nullable.HasValue ? (controlLockoutOwner.HasValue ? (controlLockoutOwner.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0;
  }

  public TimeSpan GetHolopadControlLockedPeriod(Entity<HolopadComponent> entity)
  {
    return entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds((double) entity.Comp.ControlLockoutDuration) - this._timing.CurTime;
  }

  public bool IsHolopadBroadcastOnCoolDown(Entity<HolopadComponent> entity)
  {
    return !(entity.Comp.ControlLockoutStartTime == TimeSpan.Zero) && !(entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds((double) entity.Comp.ControlLockoutCoolDown) < this._timing.CurTime);
  }

  public TimeSpan GetHolopadBroadcastCoolDown(Entity<HolopadComponent> entity)
  {
    return entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds((double) entity.Comp.ControlLockoutCoolDown) - this._timing.CurTime;
  }
}
