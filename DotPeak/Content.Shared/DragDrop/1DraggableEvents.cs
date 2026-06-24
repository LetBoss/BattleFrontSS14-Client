// Decompiled with JetBrains decompiler
// Type: Content.Shared.DragDrop.CanDropDraggedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.DragDrop;

[ByRefEvent]
public record struct CanDropDraggedEvent(EntityUid User, EntityUid Target)
{
  public readonly EntityUid User = User;
  public readonly EntityUid Target = Target;
  public bool Handled = false;
  public bool CanDrop = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Target)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.CanDrop);
  }

  [CompilerGenerated]
  public readonly bool Equals(CanDropDraggedEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid>.Default.Equals(this.Target, other.Target) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled) && EqualityComparer<bool>.Default.Equals(this.CanDrop, other.CanDrop);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid User, out EntityUid Target)
  {
    User = this.User;
    Target = this.Target;
  }
}
