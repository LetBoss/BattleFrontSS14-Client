// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pointing.SharedPointingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared.Pointing;

public abstract class SharedPointingSystem : EntitySystem
{
  protected readonly TimeSpan PointDuration = TimeSpan.FromSeconds(4L);
  protected readonly float PointKeyTimeMove = 0.1f;
  protected readonly float PointKeyTimeHover = 0.5f;

  public bool CanPoint(EntityUid uid)
  {
    PointAttemptEvent args = new PointAttemptEvent(uid);
    this.RaiseLocalEvent<PointAttemptEvent>(uid, args, true);
    return !args.Cancelled;
  }

  [NetSerializable]
  [Serializable]
  public sealed class SharedPointingArrowComponentState : ComponentState
  {
    public Vector2 StartPosition { get; init; }

    public TimeSpan EndTime { get; init; }
  }
}
