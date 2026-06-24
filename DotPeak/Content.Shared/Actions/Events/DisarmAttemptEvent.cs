// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Events.DisarmAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Actions.Events;

[ByRefEvent]
public record struct DisarmAttemptEvent(
  EntityUid targetUid,
  EntityUid disarmerUid,
  EntityUid? targetItemInHandUid = null)
{
  public readonly EntityUid TargetUid = targetUid;
  public readonly EntityUid DisarmerUid = disarmerUid;
  public readonly EntityUid? TargetItemInHandUid = targetItemInHandUid;
  public bool Cancelled = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.TargetUid) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.DisarmerUid)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.TargetItemInHandUid)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Cancelled);
  }

  [CompilerGenerated]
  public readonly bool Equals(DisarmAttemptEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.TargetUid, other.TargetUid) && EqualityComparer<EntityUid>.Default.Equals(this.DisarmerUid, other.DisarmerUid) && EqualityComparer<EntityUid?>.Default.Equals(this.TargetItemInHandUid, other.TargetItemInHandUid) && EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled);
  }
}
