// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionContainerOverflowEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionContainerOverflowEvent
{
  public readonly EntityUid SolutionEnt;
  public readonly Solution SolutionHolder;
  public readonly Solution Overflow;
  public readonly FixedPoint2 OverflowVol;
  public bool Handled;

  public SolutionContainerOverflowEvent(
    EntityUid SolutionEnt,
    Solution SolutionHolder,
    Solution Overflow)
  {
    this.SolutionEnt = SolutionEnt;
    this.SolutionHolder = SolutionHolder;
    this.Overflow = Overflow;
    this.OverflowVol = Overflow.Volume;
    this.Handled = false;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (((EqualityComparer<EntityUid>.Default.GetHashCode(this.SolutionEnt) * -1521134295 + EqualityComparer<Solution>.Default.GetHashCode(this.SolutionHolder)) * -1521134295 + EqualityComparer<Solution>.Default.GetHashCode(this.Overflow)) * -1521134295 + EqualityComparer<FixedPoint2>.Default.GetHashCode(this.OverflowVol)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled);
  }

  [CompilerGenerated]
  public readonly bool Equals(SolutionContainerOverflowEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.SolutionEnt, other.SolutionEnt) && EqualityComparer<Solution>.Default.Equals(this.SolutionHolder, other.SolutionHolder) && EqualityComparer<Solution>.Default.Equals(this.Overflow, other.Overflow) && EqualityComparer<FixedPoint2>.Default.Equals(this.OverflowVol, other.OverflowVol) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(
    out EntityUid SolutionEnt,
    out Solution SolutionHolder,
    out Solution Overflow)
  {
    SolutionEnt = this.SolutionEnt;
    SolutionHolder = this.SolutionHolder;
    Overflow = this.Overflow;
  }
}
