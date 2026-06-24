// Decompiled with JetBrains decompiler
// Type: Content.Shared.ReagentSpeed.ReagentSpeedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.ReagentSpeed;

public sealed class ReagentSpeedSystem : EntitySystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solution;

  public TimeSpan ApplySpeed(Entity<ReagentSpeedComponent?> ent, TimeSpan time)
  {
    Solution solution;
    if (!this.Resolve<ReagentSpeedComponent>((EntityUid) ent, ref ent.Comp, false) || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.Solution, out Entity<SolutionComponent>? _, out solution))
      return time;
    foreach ((ProtoId<ReagentPrototype> protoId, float num1) in ent.Comp.Modifiers)
    {
      float num2 = 1f - (1f - num1) * (solution.RemoveReagent((string) protoId, ent.Comp.Cost) / ent.Comp.Cost).Float();
      time *= (double) num2;
    }
    return time;
  }
}
