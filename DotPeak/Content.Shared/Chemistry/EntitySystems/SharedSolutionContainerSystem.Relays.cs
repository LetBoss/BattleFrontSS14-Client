// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionContainerChangedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionContainerChangedEvent(Solution solution, string solutionId)
{
  public readonly Solution Solution = solution;
  public readonly string SolutionId = solutionId;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<Solution>.Default.GetHashCode(this.Solution) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.SolutionId);
  }

  [CompilerGenerated]
  public readonly bool Equals(SolutionContainerChangedEvent other)
  {
    return EqualityComparer<Solution>.Default.Equals(this.Solution, other.Solution) && EqualityComparer<string>.Default.Equals(this.SolutionId, other.SolutionId);
  }
}
