// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Events.OnIrradiatedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Radiation.Events;

public readonly record struct OnIrradiatedEvent(
  float FrameTime,
  float RadsPerSecond,
  EntityUid Origin)
{
  public readonly float FrameTime = FrameTime;
  public readonly float RadsPerSecond = RadsPerSecond;
  public readonly EntityUid Origin = Origin;

  public float TotalRads => this.RadsPerSecond * this.FrameTime;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<float>.Default.GetHashCode(this.FrameTime) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.RadsPerSecond)) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Origin);
  }

  [CompilerGenerated]
  public bool Equals(OnIrradiatedEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.FrameTime, other.FrameTime) && EqualityComparer<float>.Default.Equals(this.RadsPerSecond, other.RadsPerSecond) && EqualityComparer<EntityUid>.Default.Equals(this.Origin, other.Origin);
  }

  [CompilerGenerated]
  public void Deconstruct(out float FrameTime, out float RadsPerSecond, out EntityUid Origin)
  {
    FrameTime = this.FrameTime;
    RadsPerSecond = this.RadsPerSecond;
    Origin = this.Origin;
  }
}
