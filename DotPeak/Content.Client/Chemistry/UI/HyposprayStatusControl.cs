// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.HyposprayStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class HyposprayStatusControl : Control
{
  private readonly Entity<HyposprayComponent> _parent;
  private readonly RichTextLabel _label;
  private readonly SharedSolutionContainerSystem _solutionContainers;
  private FixedPoint2 PrevVolume;
  private FixedPoint2 PrevMaxVolume;
  private bool PrevOnlyAffectsMobs;

  public HyposprayStatusControl(
    Entity<HyposprayComponent> parent,
    SharedSolutionContainerSystem solutionContainers)
  {
    this._parent = parent;
    this._solutionContainers = solutionContainers;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    Solution solution;
    if (!this._solutionContainers.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(this._parent.Owner), this._parent.Comp.SolutionName, out Entity<SolutionComponent>? _, out solution) || this.PrevVolume == solution.Volume && this.PrevMaxVolume == solution.MaxVolume && this.PrevOnlyAffectsMobs == this._parent.Comp.OnlyAffectsMobs)
      return;
    this.PrevVolume = solution.Volume;
    this.PrevMaxVolume = solution.MaxVolume;
    this.PrevOnlyAffectsMobs = this._parent.Comp.OnlyAffectsMobs;
    string str = Loc.GetString((!this._parent.Comp.OnlyAffectsMobs ? 0 : (this._parent.Comp.CanContainerDraw ? 1 : 0)) != 0 ? "hypospray-mobs-only-mode-text" : "hypospray-all-mode-text");
    this._label.SetMarkup(Loc.GetString("hypospray-volume-label", new (string, object)[3]
    {
      ("currentVolume", (object) solution.Volume),
      ("totalVolume", (object) solution.MaxVolume),
      ("modeString", (object) str)
    }));
  }
}
