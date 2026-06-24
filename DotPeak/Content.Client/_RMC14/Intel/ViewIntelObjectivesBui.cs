// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Intel.ViewIntelObjectivesBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Intel;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Intel;

public sealed class ViewIntelObjectivesBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private ViewIntelObjectivesWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ViewIntelObjectivesWindow>((BoundUserInterface) this);
    this.Refresh();
  }

  public void Refresh()
  {
    ViewIntelObjectivesWindow window = this._window;
    ViewIntelObjectivesComponent objectivesComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<ViewIntelObjectivesComponent>(this.Owner, ref objectivesComponent))
      return;
    IntelTechTree tree = objectivesComponent.Tree;
    RichTextLabel currentPointsLabel = this._window.CurrentPointsLabel;
    (string, object)[] valueTupleArray1 = new (string, object)[1];
    double num = tree.Points.Double();
    valueTupleArray1[0] = ("value", (object) num.ToString("F1"));
    string str1 = Loc.GetString("rmc-ui-intel-points-value", valueTupleArray1);
    currentPointsLabel.Text = str1;
    this._window.CurrentTierLabel.Text = Loc.GetString("rmc-ui-intel-tier-value", new (string, object)[1]
    {
      ("value", (object) tree.Tier)
    });
    Label totalPointsLabel = this._window.TotalPointsLabel;
    (string, object)[] valueTupleArray2 = new (string, object)[1];
    num = tree.TotalEarned.Double();
    valueTupleArray2[0] = ("value", (object) num.ToString("F1"));
    string str2 = Loc.GetString("rmc-ui-intel-total-credits", valueTupleArray2);
    totalPointsLabel.Text = str2;
    this._window.DocumentsLabel.Text = Loc.GetString("rmc-ui-intel-progress", new (string, object)[2]
    {
      ("current", (object) tree.Documents.Current),
      ("total", (object) tree.Documents.Total)
    });
    this._window.RetrieveItemsLabel.Text = Loc.GetString("rmc-ui-intel-progress", new (string, object)[2]
    {
      ("current", (object) tree.RetrieveItems.Current),
      ("total", (object) tree.RetrieveItems.Total)
    });
    this._window.RescueSurvivorsLabel.Text = Loc.GetString("rmc-ui-intel-infinite-progress", new (string, object)[1]
    {
      ("current", (object) tree.RescueSurvivors)
    });
    this._window.RecoverCorpsesLabel.Text = Loc.GetString("rmc-ui-intel-infinite-progress", new (string, object)[1]
    {
      ("current", (object) tree.RecoverCorpses)
    });
    this._window.ColonyCommunicationsLabel.Text = Loc.GetString("rmc-ui-intel-colony-status", new (string, object)[1]
    {
      ("online", (object) tree.ColonyCommunications)
    });
    this._window.ColonyPowerLabel.Text = Loc.GetString("rmc-ui-intel-colony-status", new (string, object)[1]
    {
      ("online", (object) tree.ColonyPower)
    });
    ((Control) this._window.CluesContainer).DisposeAllChildren();
    foreach ((LocId key, Dictionary<NetEntity, string> dictionary) in objectivesComponent.Tree.Clues)
    {
      ScrollContainer scrollContainer = new ScrollContainer()
      {
        HScrollEnabled = false,
        VScrollEnabled = true
      };
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer1).Margin = new Thickness(4f);
      BoxContainer boxContainer2 = boxContainer1;
      foreach ((NetEntity _, string str3) in dictionary)
      {
        BoxContainer boxContainer3 = boxContainer2;
        Label label = new Label();
        label.Text = str3;
        ((Control) label).Margin = new Thickness(2f, 1f, 2f, 1f);
        ((Control) label).StyleClasses.Add("Label");
        ((Control) boxContainer3).AddChild((Control) label);
      }
      ((Control) scrollContainer).AddChild((Control) boxContainer2);
      ((Control) this._window.CluesContainer).AddChild((Control) scrollContainer);
      TabContainer.SetTabTitle((Control) scrollContainer, Loc.GetString(LocId.op_Implicit(key)));
    }
  }
}
