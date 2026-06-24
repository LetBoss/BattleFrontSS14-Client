// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.PanelOverrideEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Wires;

[ByRefEvent]
public record struct PanelOverrideEvent
{
  public bool Allowed;

  public PanelOverrideEvent() => this.Allowed = true;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<bool>.Default.GetHashCode(this.Allowed);
  }

  [CompilerGenerated]
  public readonly bool Equals(PanelOverrideEvent other)
  {
    return EqualityComparer<bool>.Default.Equals(this.Allowed, other.Allowed);
  }
}
