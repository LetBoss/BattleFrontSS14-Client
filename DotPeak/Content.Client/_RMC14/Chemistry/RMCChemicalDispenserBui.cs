// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Chemistry.RMCChemicalDispenserBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Chemistry.Containers.EntitySystems;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable enable
namespace Content.Client._RMC14.Chemistry;

public sealed class RMCChemicalDispenserBui : BoundUserInterface
{
  [Dependency]
  private RMCReagentSystem _reagent;
  private RMCChemicalDispenserWindow? _window;
  private readonly ContainerSystem _container;
  private readonly SolutionContainerSystem _solution;
  private readonly List<(Button Button, FixedPoint2 Amount)> _dispenseButtons = new List<(Button, FixedPoint2)>();

  public RMCChemicalDispenserBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._container = this.EntMan.System<ContainerSystem>();
    this._solution = this.EntMan.System<SolutionContainerSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RMCChemicalDispenserWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.EjectBeakerButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemicalDispenserEjectBeakerBuiMsg()));
    RMCChemicalDispenserComponent dispenserComponent;
    if (this.EntMan.TryGetComponent<RMCChemicalDispenserComponent>(this.Owner, ref dispenserComponent))
    {
      for (int index1 = 0; index1 < dispenserComponent.Reagents.Length; index1 += 3)
      {
        BoxContainer row = new BoxContainer();
        for (int index2 = index1; index2 < index1 + 3; ++index2)
        {
          ProtoId<ReagentPrototype> reagentId;
          if (Extensions.TryGetValue<ProtoId<ReagentPrototype>>((IList<ProtoId<ReagentPrototype>>) dispenserComponent.Reagents, index2, ref reagentId))
            AddButton(reagentId);
        }
        ((Control) this._window.ChemicalsContainer).AddChild((Control) row);

        void AddButton(ProtoId<ReagentPrototype> reagentId)
        {
          Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
          if (!this._reagent.TryIndex(reagentId, out reagent))
            return;
          Button button1 = new Button();
          button1.Text = "▼ " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reagent.LocalizedName);
          ((Control) button1).HorizontalExpand = true;
          ((Control) button1).StyleClasses.Add("OpenBoth");
          Button button2 = button1;
          ((Control) button2.Label).AddStyleClass("CMAlignLeft");
          ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemicalDispenserDispenseBuiMsg(reagentId)));
          ((Control) row).AddChild((Control) button2);
        }
      }
      foreach (FixedPoint2 setting1 in dispenserComponent.Settings)
      {
        FixedPoint2 setting = setting1;
        Button button1 = new Button();
        button1.Text = $"+ {setting.Int()}";
        ((Control) button1).StyleClasses.Add("OpenBoth");
        ((Control) button1).SetWidth = 45f;
        ((Control) button1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 3f);
        ((BaseButton) button1).Pressed = dispenserComponent.DispenseSetting == setting;
        Button button2 = button1;
        ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemicalDispenserDispenseSettingBuiMsg(setting)));
        ((Control) this._window.DispenseContainer).AddChild((Control) button2);
        this._dispenseButtons.Add((button2, setting));
        Button button3 = new Button();
        button3.Text = $"- {setting.Int()}";
        ((Control) button3).StyleClasses.Add("OpenBoth");
        ((Control) button3).SetWidth = 45f;
        ((Control) button3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 3f);
        Button button4 = button3;
        ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemicalDispenserBeakerBuiMsg(setting)));
        ((Control) this._window.BeakerContainer).AddChild((Control) button4);
      }
    }
    this.Refresh();
  }

  public void Refresh()
  {
    RMCChemicalDispenserWindow window = this._window;
    RMCChemicalDispenserComponent dispenserComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<RMCChemicalDispenserComponent>(this.Owner, ref dispenserComponent))
      return;
    ((Range) this._window.EnergyBar).MaxValue = dispenserComponent.MaxEnergy.Float();
    FixedPoint2 energy = dispenserComponent.Energy;
    ((Range) this._window.EnergyBar).Value = energy.Float();
    this._window.EnergyLabel.Text = $"{energy.Int()} energy";
    BaseContainer baseContainer;
    EntityUid? nullable;
    if (!((SharedContainerSystem) this._container).TryGetContainer(this.Owner, dispenserComponent.ContainerSlotId, ref baseContainer, (ContainerManagerComponent) null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>) baseContainer.ContainedEntities, ref nullable))
    {
      this._window.BeakerStatus.Text = "No beaker loaded!";
      ((Control) this._window.EjectBeakerButton).Visible = false;
      ((Control) this._window.ContentsNone).Visible = true;
      ((Control) this._window.BeakerContents).Visible = false;
      ((Control) this._window.BeakerContents).DisposeAllChildren();
      foreach (BaseButton baseButton in ((Control) this._window.ChemicalsContainer).GetControlOfType<Button>())
        baseButton.Disabled = true;
    }
    else
    {
      ((Control) this._window.EjectBeakerButton).Visible = true;
      ((Control) this._window.ContentsNone).Visible = false;
      ((Control) this._window.BeakerContents).Visible = true;
      ((Control) this._window.BeakerContents).DisposeAllChildren();
      foreach (BaseButton baseButton in ((Control) this._window.ChemicalsContainer).GetControlOfType<Button>())
        baseButton.Disabled = false;
      FixedPoint2 fixedPoint2_1 = FixedPoint2.Zero;
      FixedPoint2 fixedPoint2_2 = FixedPoint2.Zero;
      Solution solution;
      if (this._solution.TryGetMixableSolution(Entity<MixableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(nullable.Value), out Entity<SolutionComponent>? _, out solution))
      {
        fixedPoint2_1 = solution.Volume;
        fixedPoint2_2 = solution.MaxVolume;
        foreach (ReagentQuantity content in solution.Contents)
        {
          string str = content.Reagent.Prototype;
          Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
          if (this._reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(str), out reagent))
            str = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reagent.LocalizedName);
          BoxContainer beakerContents = this._window.BeakerContents;
          Label label = new Label();
          label.Text = $"{content.Quantity} units of {str}";
          ((Control) beakerContents).AddChild((Control) label);
        }
      }
      this._window.BeakerStatus.Text = $"{fixedPoint2_1}/{fixedPoint2_2} units";
    }
    foreach ((Button Button, FixedPoint2 Amount) in this._dispenseButtons)
      ((BaseButton) Button).Pressed = dispenserComponent.DispenseSetting == Amount;
  }
}
