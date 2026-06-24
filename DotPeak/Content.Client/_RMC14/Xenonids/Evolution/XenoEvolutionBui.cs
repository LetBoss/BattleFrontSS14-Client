// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Evolution.XenoEvolutionBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Client.Message;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Evolution;

public sealed class XenoEvolutionBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly SpriteSystem _sprite;
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoEvolutionWindow? _window;
  private readonly Dictionary<EntProtoId, XenoChoiceControl> _evolutionControls = new Dictionary<EntProtoId, XenoChoiceControl>();
  private readonly Dictionary<EntProtoId, XenoChoiceControl> _strainControls = new Dictionary<EntProtoId, XenoChoiceControl>();

  public XenoEvolutionBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoEvolutionWindow>((BoundUserInterface) this);
    ((Control) this._window.OvipositorNeededLabel).Visible = false;
    XenoEvolutionComponent evolutionComponent;
    if (this.EntMan.TryGetComponent<XenoEvolutionComponent>(this.Owner, ref evolutionComponent))
    {
      foreach (EntProtoId strain in evolutionComponent.Strains)
        this.AddStrain(strain);
    }
    ((Control) this._window.StrainsLabel).Visible = ((Control) this._window.StrainsContainer).ChildCount > 0;
    this.Refresh();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state) => this.Refresh();

  private void AddEvolution(EntProtoId evolutionId)
  {
    EntityPrototype entityPrototype;
    if (!this._prototype.TryIndex(evolutionId, ref entityPrototype))
      return;
    XenoChoiceControl xenoChoiceControl;
    if (!this._evolutionControls.TryGetValue(evolutionId, out xenoChoiceControl))
    {
      xenoChoiceControl = new XenoChoiceControl();
      xenoChoiceControl.Set(entityPrototype.Name, this._sprite.Frame0(entityPrototype));
      ((BaseButton) xenoChoiceControl.Button).Disabled = false;
      ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoEvolveBuiMsg(evolutionId));
        this.Close();
      });
      this._evolutionControls[evolutionId] = xenoChoiceControl;
      ((Control) this._window?.EvolutionsContainer).AddChild((Control) xenoChoiceControl);
    }
    xenoChoiceControl.Visible = true;
    ((BaseButton) xenoChoiceControl.Button).Disabled = false;
  }

  private void AddStrain(EntProtoId strainId)
  {
    XenoEvolutionWindow window = this._window;
    EntityPrototype strain;
    if (window == null || !((BaseWindow) window).IsOpen || !this._prototype.TryIndex(strainId, ref strain))
      return;
    XenoChoiceControl xenoChoiceControl;
    if (!this._strainControls.TryGetValue(strainId, out xenoChoiceControl))
    {
      xenoChoiceControl = new XenoChoiceControl();
      string name = strain.Name;
      string description = (string) null;
      XenoStrainComponent xenoStrainComponent;
      if (strain.TryGetComponent<XenoStrainComponent>(ref xenoStrainComponent, this.EntMan.ComponentFactory))
      {
        name = $"{Loc.GetString(LocId.op_Implicit(xenoStrainComponent.Name))} {name}";
        LocId? description1 = xenoStrainComponent.Description;
        description = description1.HasValue ? LocId.op_Implicit(description1.GetValueOrDefault()) : (string) null;
      }
      xenoChoiceControl.Set(name, this._sprite.Frame0(strain));
      ((BaseButton) xenoChoiceControl.Button).Disabled = false;
      ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        XenoStrainConfirmWindow confirmWindow = new XenoStrainConfirmWindow();
        confirmWindow.SetInfo(name, this._sprite.Frame0(strain), description);
        confirmWindow.OnConfirm += (Action) (() =>
        {
          this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoStrainBuiMsg(strainId));
          ((BaseWindow) confirmWindow).Close();
          this.Close();
        });
        ((BaseWindow) confirmWindow).OpenCentered();
      });
      this._strainControls[strainId] = xenoChoiceControl;
      ((Control) this._window.StrainsContainer).AddChild((Control) xenoChoiceControl);
    }
    xenoChoiceControl.Visible = true;
    ((BaseButton) xenoChoiceControl.Button).Disabled = false;
  }

  public void Refresh()
  {
    XenoEvolutionComponent evolutionComponent;
    if (this._window == null || !this.EntMan.TryGetComponent<XenoEvolutionComponent>(this.Owner, ref evolutionComponent))
      return;
    ((Control) this._window.PointsLabel).Visible = evolutionComponent.Max > FixedPoint2.Zero;
    foreach (Control control in this._evolutionControls.Values)
      control.Visible = false;
    foreach (EntProtoId evolvesToWithoutPoint in evolutionComponent.EvolvesToWithoutPoints)
      this.AddEvolution(evolvesToWithoutPoint);
    if (evolutionComponent.Points >= evolutionComponent.Max)
    {
      foreach (EntProtoId evolutionId in evolutionComponent.EvolvesTo)
        this.AddEvolution(evolutionId);
    }
    this._window.Separator.Visible = ((IEnumerable<Control>) ((Control) this._window.EvolutionsContainer).Children).Any<Control>((Func<Control, bool>) (child => child.Visible)) && ((IEnumerable<Control>) ((Control) this._window.StrainsContainer).Children).Any<Control>((Func<Control, bool>) (child => child.Visible));
    bool flag = this.State is XenoEvolveBuiState state && state.LackingOvipositor;
    this._window.PointsLabel.Text = Loc.GetString("rmc-xeno-ui-evolution-points", new (string, object)[2]
    {
      ("points", (object) (int) Math.Floor(evolutionComponent.Points.Double())),
      ("maxPoints", (object) evolutionComponent.Max)
    });
    if (flag && evolutionComponent.Max > FixedPoint2.Zero)
    {
      if (((Control) this._window.OvipositorNeededLabel).Visible)
        return;
      this._window.OvipositorNeededLabel.SetMarkupPermissive(Loc.GetString("rmc-xeno-ui-ovi-needed-label"));
      ((Control) this._window.OvipositorNeededLabel).Visible = true;
    }
    else
    {
      if (!((Control) this._window.OvipositorNeededLabel).Visible)
        return;
      ((Control) this._window.OvipositorNeededLabel).Visible = false;
    }
  }
}
