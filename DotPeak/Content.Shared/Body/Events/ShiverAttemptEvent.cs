// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Events.ShiverAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Body.Events;

[ByRefEvent]
public record struct ShiverAttemptEvent(EntityUid Uid)
{
  public readonly EntityUid Uid = Uid;
  public bool Cancelled = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.Uid) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Cancelled);
  }

  [CompilerGenerated]
  public readonly bool Equals(ShiverAttemptEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Uid, other.Uid) && EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid Uid) => Uid = this.Uid;
}
