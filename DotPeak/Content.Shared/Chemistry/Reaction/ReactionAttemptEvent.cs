// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ReactionAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

[ByRefEvent]
public record struct ReactionAttemptEvent(
  ReactionPrototype Reaction,
  Entity<SolutionComponent> Solution)
{
  public readonly ReactionPrototype Reaction = Reaction;
  public readonly Entity<SolutionComponent> Solution = Solution;
  public bool Cancelled = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<ReactionPrototype>.Default.GetHashCode(this.Reaction) * -1521134295 + EqualityComparer<Entity<SolutionComponent>>.Default.GetHashCode(this.Solution)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Cancelled);
  }

  [CompilerGenerated]
  public readonly bool Equals(ReactionAttemptEvent other)
  {
    return EqualityComparer<ReactionPrototype>.Default.Equals(this.Reaction, other.Reaction) && EqualityComparer<Entity<SolutionComponent>>.Default.Equals(this.Solution, other.Solution) && EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(
    out ReactionPrototype Reaction,
    out Entity<SolutionComponent> Solution)
  {
    Reaction = this.Reaction;
    Solution = this.Solution;
  }
}
