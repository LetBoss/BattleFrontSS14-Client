// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SolutionSpikerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionSpikerSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solution;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RefillableSolutionComponent, InteractUsingEvent>(new EntityEventRefHandler<RefillableSolutionComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
  }

  private void OnInteractUsing(
    Entity<RefillableSolutionComponent> entity,
    ref InteractUsingEvent args)
  {
    if (!this.TrySpike(args.Used, args.Target, args.User, entity.Comp))
      return;
    args.Handled = true;
  }

  private bool TrySpike(
    EntityUid source,
    EntityUid target,
    EntityUid user,
    RefillableSolutionComponent? spikableTarget = null,
    SolutionSpikerComponent? spikableSource = null,
    SolutionContainerManagerComponent? managerSource = null,
    SolutionContainerManagerComponent? managerTarget = null)
  {
    Entity<SolutionComponent>? soln;
    Solution solution1;
    Solution solution2;
    if (!this.Resolve<SolutionSpikerComponent, SolutionContainerManagerComponent>(source, ref spikableSource, ref managerSource, false) || !this.Resolve<RefillableSolutionComponent, SolutionContainerManagerComponent>(target, ref spikableTarget, ref managerTarget, false) || !this._solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((target, spikableTarget, managerTarget)), out soln, out solution1) || !this._solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((source, managerSource)), spikableSource.SourceSolution, out Entity<SolutionComponent>? _, out solution2))
      return false;
    if (solution1.Volume == 0 && !spikableSource.IgnoreEmpty)
    {
      this._popup.PopupClient(this.Loc.GetString(LocId.op_Implicit(spikableSource.PopupEmpty), ("spiked-entity", (object) target), ("spike-entity", (object) source)), user, new EntityUid?(user));
      return false;
    }
    if (!this._solution.ForceAddSolution(soln.Value, solution2))
      return false;
    this._popup.PopupClient(this.Loc.GetString(LocId.op_Implicit(spikableSource.Popup), ("spiked-entity", (object) target), ("spike-entity", (object) source)), user, new EntityUid?(user));
    solution2.RemoveAllSolution();
    if (spikableSource.Delete)
      this.QueueDel(new EntityUid?(source));
    return true;
  }
}
