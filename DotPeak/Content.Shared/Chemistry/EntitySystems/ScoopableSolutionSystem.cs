// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.ScoopableSolutionSystem
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
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class ScoopableSolutionSystem : EntitySystem
{
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private SolutionTransferSystem _solutionTransfer;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ScoopableSolutionComponent, InteractUsingEvent>(new EntityEventRefHandler<ScoopableSolutionComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
  }

  private void OnInteractUsing(Entity<ScoopableSolutionComponent> ent, ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryScoop(ent, args.Used, args.User);
  }

  public bool TryScoop(Entity<ScoopableSolutionComponent> ent, EntityUid beaker, EntityUid user)
  {
    Entity<SolutionComponent>? entity;
    Solution solution;
    Entity<SolutionComponent>? soln;
    if (!this._solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out entity, out solution) || !this._solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(beaker), out soln, out Solution _) || this._solutionTransfer.Transfer(new EntityUid?(user), Entity<ScoopableSolutionComponent>.op_Implicit(ent), entity.Value, beaker, soln.Value, solution.Volume) == 0)
      return false;
    this._popup.PopupClient(this.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup), ("scooped", (object) ent.Owner), (nameof (beaker), (object) beaker)), user, new EntityUid?(user));
    if (solution.Volume == 0 && ent.Comp.Delete)
    {
      this.RemCompDeferred<ScoopableSolutionComponent>(Entity<ScoopableSolutionComponent>.op_Implicit(ent));
      if (!this._netManager.IsClient)
        this.QueueDel(new EntityUid?(Entity<ScoopableSolutionComponent>.op_Implicit(ent)));
    }
    return true;
  }
}
