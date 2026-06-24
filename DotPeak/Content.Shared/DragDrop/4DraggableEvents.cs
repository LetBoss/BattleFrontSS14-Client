// Decompiled with JetBrains decompiler
// Type: Content.Shared.DragDrop.DragDropTargetEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.DragDrop;

[ByRefEvent]
public record struct DragDropTargetEvent(EntityUid User, EntityUid Dragged)
{
  public readonly EntityUid User = User;
  public readonly EntityUid Dragged = Dragged;
  public bool Handled = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Dragged)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled);
  }

  [CompilerGenerated]
  public readonly bool Equals(DragDropTargetEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid>.Default.Equals(this.Dragged, other.Dragged) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid User, out EntityUid Dragged)
  {
    User = this.User;
    Dragged = this.Dragged;
  }
}
