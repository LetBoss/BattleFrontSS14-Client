// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.Hypospray.RMCHyposprayStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client._RMC14.Medical.Hypospray;

public sealed class RMCHyposprayStatusControl : Control
{
  private readonly Entity<RMCHyposprayComponent> _parent;
  private readonly SharedSolutionContainerSystem _solutionContainers;
  private readonly RichTextLabel _label;
  private readonly SharedContainerSystem _container;
  private EntityUid? PrevVial;
  private FixedPoint2 PrevVolume;
  private FixedPoint2 PrevMaxVolume;
  private FixedPoint2 PrevTransferAmount;

  public RMCHyposprayStatusControl(
    Entity<RMCHyposprayComponent> parent,
    SharedSolutionContainerSystem solutionContainers,
    SharedContainerSystem containers)
  {
    this._parent = parent;
    this._solutionContainers = solutionContainers;
    this._container = containers;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    BaseContainer baseContainer;
    if (!this._container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(this._parent), this._parent.Comp.SlotId, ref baseContainer, (ContainerManagerComponent) null))
      return;
    if (baseContainer.ContainedEntities.Count == 0)
    {
      if (this.PrevTransferAmount == this._parent.Comp.TransferAmount && !this.PrevVial.HasValue)
        return;
      this.PrevVial = new EntityUid?();
      this.PrevTransferAmount = this._parent.Comp.TransferAmount;
      this._label.SetMarkup(Loc.GetString("rmc-hypospray-label-novial", new (string, object)[1]
      {
        ("transferVolume", (object) this._parent.Comp.TransferAmount)
      }));
    }
    else
    {
      EntityUid containedEntity = baseContainer.ContainedEntities[0];
      Solution solution;
      if (!this._solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(containedEntity), this._parent.Comp.VialName, out Entity<SolutionComponent>? _, out solution))
        return;
      if (this.PrevVolume == solution.Volume && this.PrevMaxVolume == solution.MaxVolume && this.PrevTransferAmount == this._parent.Comp.TransferAmount)
      {
        EntityUid? prevVial = this.PrevVial;
        EntityUid entityUid = containedEntity;
        if ((prevVial.HasValue ? (EntityUid.op_Equality(prevVial.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
          return;
      }
      this.PrevVolume = solution.Volume;
      this.PrevMaxVolume = solution.MaxVolume;
      this.PrevTransferAmount = this._parent.Comp.TransferAmount;
      this.PrevVial = new EntityUid?(containedEntity);
      this._label.SetMarkup(Loc.GetString("rmc-hypospray-label", new (string, object)[3]
      {
        ("currentVolume", (object) solution.Volume),
        ("totalVolume", (object) solution.MaxVolume),
        ("transferVolume", (object) this._parent.Comp.TransferAmount)
      }));
    }
  }
}
