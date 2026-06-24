// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivBotControlPanel
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivBotControlPanel : Control
{
  private readonly CivCommanderBotControlSystem _control;
  private readonly CivGlobalMapSystem _globalMap;
  private readonly BoxContainer _root;
  private readonly Label _selectionLabel;
  private readonly Label _orderModeLabel;
  private readonly GridContainer _orderButtons;
  private readonly GridContainer _squadButtons;
  private readonly Button _clearSelectionButton;

  public CivBotControlPanel(CivCommanderBotControlSystem control, CivGlobalMapSystem globalMap)
  {
    IoCManager.InjectDependencies<CivBotControlPanel>(this);
    this._control = control;
    this._globalMap = globalMap;
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer.SeparationOverride = new int?(6);
    ((Control) boxContainer).Margin = new Thickness(8f);
    ((Control) boxContainer).HorizontalExpand = true;
    this._root = boxContainer;
    this.AddChild((Control) this._root);
    BoxContainer root = this._root;
    Label label = new Label();
    label.Text = Loc.GetString("civ-cmd-bot-panel-title");
    label.FontColorOverride = new Color?(Color.White);
    ((Control) label).HorizontalExpand = true;
    ((Control) root).AddChild((Control) label);
    this._selectionLabel = new Label()
    {
      Text = Loc.GetString("civ-cmd-bot-panel-selected", new (string, object)[1]
      {
        ("count", (object) 0)
      }),
      FontColorOverride = new Color?(Color.LightGray)
    };
    ((Control) this._root).AddChild((Control) this._selectionLabel);
    this._orderModeLabel = new Label()
    {
      Text = Loc.GetString("civ-cmd-bot-panel-mode", new (string, object)[1]
      {
        ("mode", (object) CivBotControlPanel.GetOrderName(CivBotOrderType.Move))
      }),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?()))
    };
    ((Control) this._root).AddChild((Control) this._orderModeLabel);
    ((Control) this._root).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-cmd-bot-panel-orders-header")
    });
    GridContainer gridContainer1 = new GridContainer();
    gridContainer1.Columns = 3;
    ((Control) gridContainer1).HorizontalExpand = true;
    this._orderButtons = gridContainer1;
    ((Control) this._root).AddChild((Control) this._orderButtons);
    this.AddOrderButton(Loc.GetString("civ-cmd-bot-btn-move"), CivBotOrderType.Move, "#4caf50");
    this.AddOrderButton(Loc.GetString("civ-cmd-bot-btn-attack-move"), CivBotOrderType.AttackMove, "#f44336");
    this.AddOrderButton(Loc.GetString("civ-cmd-bot-btn-hold"), CivBotOrderType.HoldPosition, "#2196f3");
    this.AddOrderButton(Loc.GetString("civ-cmd-bot-btn-follow"), CivBotOrderType.Follow, "#9c27b0");
    this.AddOrderButton(Loc.GetString("civ-cmd-bot-btn-defend"), CivBotOrderType.Defend, "#00bcd4");
    this.AddOrderButton(Loc.GetString("civ-cmd-bot-btn-patrol"), CivBotOrderType.Patrol, "#ff9800");
    ((Control) this._root).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-cmd-bot-panel-squads-header")
    });
    GridContainer gridContainer2 = new GridContainer();
    gridContainer2.Columns = 5;
    ((Control) gridContainer2).HorizontalExpand = true;
    this._squadButtons = gridContainer2;
    ((Control) this._root).AddChild((Control) this._squadButtons);
    for (int index = 1; index <= 9; ++index)
    {
      int squadId = index;
      Button button1 = new Button();
      button1.Text = index.ToString();
      ((Control) button1).MinWidth = 30f;
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._control.SelectSquad(squadId));
      ((Control) this._squadButtons).AddChild((Control) button2);
    }
    Button button3 = new Button();
    button3.Text = Loc.GetString("civ-cmd-bot-clear-selection");
    ((Control) button3).HorizontalExpand = true;
    this._clearSelectionButton = button3;
    ((BaseButton) this._clearSelectionButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._control.ClearSelection());
    ((Control) this._root).AddChild((Control) this._clearSelectionButton);
    Button button4 = new Button();
    button4.Text = Loc.GetString("civ-cmd-bot-cancel-patrol");
    ((Control) button4).HorizontalExpand = true;
    Button button5 = button4;
    ((BaseButton) button5).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._control.CancelPatrol());
    ((Control) this._root).AddChild((Control) button5);
  }

  private void AddOrderButton(string text, CivBotOrderType orderType, string colorHex)
  {
    Color color = Color.FromHex((ReadOnlySpan<char>) colorHex, new Color?());
    Button button1 = new Button();
    button1.Text = text;
    ((Control) button1).HorizontalExpand = true;
    ((Control) button1).Modulate = color;
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._control.SetOrderMode(orderType));
    ((Control) this._orderButtons).AddChild((Control) button2);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    this._selectionLabel.Text = Loc.GetString("civ-cmd-bot-panel-selected", new (string, object)[1]
    {
      ("count", (object) this._control.GetSelectedCount())
    });
    CivBotOrderType currentOrderMode = this._control.CurrentOrderMode;
    Color orderColor = CivBotControlPanel.GetOrderColor(currentOrderMode);
    this._orderModeLabel.Text = Loc.GetString("civ-cmd-bot-panel-mode", new (string, object)[1]
    {
      ("mode", (object) CivBotControlPanel.GetOrderName(currentOrderMode))
    });
    this._orderModeLabel.FontColorOverride = new Color?(orderColor);
    if (!this._control.IsPatrolMode)
      return;
    this._orderModeLabel.Text += Loc.GetString("civ-cmd-bot-panel-patrol-points", new (string, object)[1]
    {
      ("count", (object) this._control.PatrolPoints.Count)
    });
  }

  private static Color GetOrderColor(CivBotOrderType order)
  {
    Color orderColor;
    switch (order)
    {
      case CivBotOrderType.Idle:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#9e9e9e", new Color?());
        break;
      case CivBotOrderType.Move:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?());
        break;
      case CivBotOrderType.AttackMove:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#f44336", new Color?());
        break;
      case CivBotOrderType.HoldPosition:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#2196f3", new Color?());
        break;
      case CivBotOrderType.Follow:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#9c27b0", new Color?());
        break;
      case CivBotOrderType.Defend:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?());
        break;
      case CivBotOrderType.Patrol:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
        break;
      default:
        orderColor = Color.White;
        break;
    }
    return orderColor;
  }

  private static string GetOrderName(CivBotOrderType order)
  {
    string orderName;
    switch (order)
    {
      case CivBotOrderType.Idle:
        orderName = Loc.GetString("civ-cmd-bot-mode-idle");
        break;
      case CivBotOrderType.Move:
        orderName = Loc.GetString("civ-cmd-bot-mode-move");
        break;
      case CivBotOrderType.AttackMove:
        orderName = Loc.GetString("civ-cmd-bot-mode-attack-move");
        break;
      case CivBotOrderType.HoldPosition:
        orderName = Loc.GetString("civ-cmd-bot-mode-hold");
        break;
      case CivBotOrderType.Follow:
        orderName = Loc.GetString("civ-cmd-bot-mode-follow");
        break;
      case CivBotOrderType.Defend:
        orderName = Loc.GetString("civ-cmd-bot-mode-defend");
        break;
      case CivBotOrderType.Patrol:
        orderName = Loc.GetString("civ-cmd-bot-mode-patrol");
        break;
      default:
        orderName = Loc.GetString("civ-cmd-bot-mode-unknown");
        break;
    }
    return orderName;
  }
}
