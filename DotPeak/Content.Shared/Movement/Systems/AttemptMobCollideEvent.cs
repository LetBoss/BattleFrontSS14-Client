// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.AttemptMobCollideEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct AttemptMobCollideEvent
{
  public bool Cancelled;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<bool>.Default.GetHashCode(this.Cancelled);
  }

  [CompilerGenerated]
  public readonly bool Equals(AttemptMobCollideEvent other)
  {
    return EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled);
  }
}
