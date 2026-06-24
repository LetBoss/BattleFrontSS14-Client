// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.MenuVisibilityEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Verbs;

[ByRefEvent]
public record struct MenuVisibilityEvent
{
  public MapCoordinates TargetPos;
  public MenuVisibility Visibility;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<MapCoordinates>.Default.GetHashCode(this.TargetPos) * -1521134295 + EqualityComparer<MenuVisibility>.Default.GetHashCode(this.Visibility);
  }

  [CompilerGenerated]
  public readonly bool Equals(MenuVisibilityEvent other)
  {
    return EqualityComparer<MapCoordinates>.Default.Equals(this.TargetPos, other.TargetPos) && EqualityComparer<MenuVisibility>.Default.Equals(this.Visibility, other.Visibility);
  }
}
