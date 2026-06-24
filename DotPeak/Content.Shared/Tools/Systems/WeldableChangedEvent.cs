// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Systems.WeldableChangedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Tools.Systems;

[ByRefEvent]
public readonly record struct WeldableChangedEvent(bool IsWelded)
{
  public readonly bool IsWelded = IsWelded;

  [CompilerGenerated]
  public override int GetHashCode() => EqualityComparer<bool>.Default.GetHashCode(this.IsWelded);

  [CompilerGenerated]
  public bool Equals(WeldableChangedEvent other)
  {
    return EqualityComparer<bool>.Default.Equals(this.IsWelded, other.IsWelded);
  }

  [CompilerGenerated]
  public void Deconstruct(out bool IsWelded) => IsWelded = this.IsWelded;
}
