// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.SolutionStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Chemistry.Components;
using Content.Client.Items.UI;
using Content.Client.Message;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class SolutionStatusControl : PollingItemStatusControl<SolutionStatusControl.Data>
{
  private readonly Entity<SolutionItemStatusComponent> _parent;
  private readonly IEntityManager _entityManager;
  private readonly SharedSolutionContainerSystem _solutionContainers;
  private readonly RichTextLabel _label;

  public SolutionStatusControl(
    Entity<SolutionItemStatusComponent> parent,
    IEntityManager entityManager,
    SharedSolutionContainerSystem solutionContainers)
  {
    this._parent = parent;
    this._entityManager = entityManager;
    this._solutionContainers = solutionContainers;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
  }

  protected override SolutionStatusControl.Data PollData()
  {
    Solution solution;
    if (!this._solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(this._parent.Owner), this._parent.Comp.Solution, out Entity<SolutionComponent>? _, out solution))
      return new SolutionStatusControl.Data();
    FixedPoint2? TransferVolume = new FixedPoint2?();
    SolutionTransferComponent transferComponent;
    if (this._entityManager.TryGetComponent<SolutionTransferComponent>(this._parent.Owner, ref transferComponent))
      TransferVolume = new FixedPoint2?(transferComponent.TransferAmount);
    return new SolutionStatusControl.Data(solution.Volume, solution.MaxVolume, TransferVolume);
  }

  protected override void Update(in SolutionStatusControl.Data data)
  {
    string markup = Loc.GetString("solution-status-volume", new (string, object)[2]
    {
      ("currentVolume", (object) data.Volume),
      ("maxVolume", (object) data.MaxVolume)
    });
    FixedPoint2? transferVolume = data.TransferVolume;
    if (transferVolume.HasValue)
    {
      FixedPoint2 valueOrDefault = transferVolume.GetValueOrDefault();
      markup = $"{markup}\n{Loc.GetString("solution-status-transfer", new (string, object)[1]
      {
        ("volume", (object) valueOrDefault)
      })}";
    }
    this._label.SetMarkup(markup);
  }

  public readonly record struct Data(
    FixedPoint2 Volume,
    FixedPoint2 MaxVolume,
    FixedPoint2? TransferVolume)
  ;
}
