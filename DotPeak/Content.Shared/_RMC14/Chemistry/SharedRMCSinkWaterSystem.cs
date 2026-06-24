// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.SharedRMCSinkWaterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Chemistry;

public sealed class SharedRMCSinkWaterSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solution;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCSinkWaterComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCSinkWaterComponent, InteractUsingEvent>(this.OnSinkInteractUsing));
  }

  private void OnSinkInteractUsing(Entity<RMCSinkWaterComponent> sink, ref InteractUsingEvent args)
  {
    RefillableSolutionComponent comp1;
    Entity<SolutionComponent>? soln;
    Solution solution;
    if (args.Handled || !this.TryComp<RefillableSolutionComponent>(args.Used, out comp1) || !this._solution.TryGetRefillableSolution((Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>) (args.Used, comp1, (SolutionContainerManagerComponent) null), out soln, out solution))
      return;
    args.Handled = true;
    FixedPoint2 availableVolume = solution.AvailableVolume;
    if (availableVolume <= FixedPoint2.Zero)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-sink-container-full", ("container", (object) args.Used)), (EntityUid) sink, new EntityUid?(args.User));
    }
    else
    {
      FixedPoint2 quantity = availableVolume;
      SolutionTransferComponent comp2;
      if (this.TryComp<SolutionTransferComponent>(args.Used, out comp2))
        quantity = FixedPoint2.Min(comp2.TransferAmount, availableVolume);
      Solution toAdd = new Solution();
      toAdd.AddReagent((string) sink.Comp.Reagent, quantity);
      this._solution.TryAddSolution(soln.Value, toAdd);
      this._popup.PopupPredicted(this.Loc.GetString("rmc-sink-fill-container", ("user", (object) args.User), ("container", (object) args.Used), (nameof (sink), (object) sink)), args.User, new EntityUid?(args.User));
    }
  }
}
