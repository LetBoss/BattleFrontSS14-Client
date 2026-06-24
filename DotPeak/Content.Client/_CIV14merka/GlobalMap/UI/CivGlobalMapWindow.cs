// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.UI.CivGlobalMapWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivGlobalMapWindow : DefaultWindow
{
  private readonly CivGlobalMapSystem _system;
  private readonly CivGlobalMapCanvas _canvas;
  private readonly Label _roleLabel;
  private readonly Label _roundStatusLabel;
  private readonly Label _hintLabel;
  private readonly PanelContainer _commanderSidebar;
  private readonly Label _commanderSidebarSummaryLabel;
  private readonly BoxContainer _commanderSquadListContainer;
  private readonly Button _commanderSidebarAttackButton;
  private readonly Button _commanderSidebarDefenseButton;
  private readonly Button _commanderSidebarArtilleryOrderButton;
  private readonly Button _commanderSidebarClearOrderButton;
  private readonly Button _commanderSidebarRosterButton;
  private readonly PanelContainer _commanderPanel;
  private readonly Button _openCommanderButton;
  private readonly Label _commanderSummaryLabel;
  private readonly BoxContainer _commanderRosterContainer;
  private readonly OptionButton _commanderSquadSelector;
  private readonly Button _commanderAttackButton;
  private readonly Button _commanderDefenseButton;
  private readonly Button _commanderArtilleryOrderButton;
  private readonly Button _commanderClearOrderButton;
  private readonly OptionButton _commanderPlayerSelector;
  private readonly OptionButton _commanderDestinationSelector;
  private readonly Button _commanderMoveButton;
  private readonly Dictionary<CivGlobalMapMarkerType, Button> _markerButtons = new Dictionary<CivGlobalMapMarkerType, Button>();
  private readonly Button _removeModeButton;
  private CivGlobalMapMarkerType? _selectedMarkerType;
  private bool _isSquadLeader;
  private bool _isCommander;
  private CivCommanderState? _commanderState;
  private readonly Dictionary<int, int> _squadSelectorToSquadId = new Dictionary<int, int>();
  private readonly Dictionary<int, NetUserId> _playerSelectorToUserId = new Dictionary<int, NetUserId>();
  private readonly Dictionary<int, CommanderDestination> _destinationSelectorMap = new Dictionary<int, CommanderDestination>();
  private int? _pendingOrderSquadId;
  private CivCommanderOrderType? _pendingOrderType;

  public Control CommanderPanel => (Control) this._commanderPanel;

  public CivGlobalMapWindow(CivGlobalMapSystem system)
  {
    this._system = system;
    this.Title = Loc.GetString("civ-gmap-window-title");
    ((Control) this).MinSize = new Vector2(900f, 700f);
    ((Control) this).SetSize = new Vector2(1100f, 800f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).Margin = new Thickness(8f);
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    this.Contents.AddChild((Control) boxContainer2);
    Label label1 = new Label();
    label1.Text = Loc.GetString("civ-gmap-role-prefix");
    ((Control) label1).HorizontalExpand = true;
    this._roleLabel = label1;
    ((Control) boxContainer2).AddChild((Control) this._roleLabel);
    Label label2 = new Label();
    label2.Text = Loc.GetString("civ-gmap-round-status-prefix");
    label2.FontColorOverride = new Color?(Color.White);
    ((Control) label2).HorizontalExpand = true;
    this._roundStatusLabel = label2;
    ((Control) boxContainer2).AddChild((Control) this._roundStatusLabel);
    Label label3 = new Label();
    label3.Text = Loc.GetString("civ-gmap-hint-default");
    label3.FontColorOverride = new Color?(Color.LightGray);
    ((Control) label3).HorizontalExpand = true;
    this._hintLabel = label3;
    ((Control) boxContainer2).AddChild((Control) this._hintLabel);
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(8);
    ((Control) boxContainer3).HorizontalExpand = true;
    ((Control) boxContainer3).VerticalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 420f;
    ((Control) panelContainer1).HorizontalExpand = false;
    ((Control) panelContainer1).VerticalExpand = true;
    ((Control) panelContainer1).Visible = false;
    this._commanderSidebar = panelContainer1;
    ((Control) boxContainer4).AddChild((Control) this._commanderSidebar);
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer5.SeparationOverride = new int?(6);
    ((Control) boxContainer5).Margin = new Thickness(8f);
    ((Control) boxContainer5).HorizontalExpand = true;
    ((Control) boxContainer5).VerticalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    ((Control) this._commanderSidebar).AddChild((Control) boxContainer6);
    BoxContainer boxContainer7 = boxContainer6;
    Label label4 = new Label();
    label4.Text = Loc.GetString("civ-gmap-sidebar-squads");
    ((Control) label4).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer7).AddChild((Control) label4);
    Label label5 = new Label();
    label5.Text = Loc.GetString("civ-gmap-sidebar-no-data");
    ((Control) label5).HorizontalExpand = true;
    this._commanderSidebarSummaryLabel = label5;
    ((Control) boxContainer6).AddChild((Control) this._commanderSidebarSummaryLabel);
    ((Control) this._commanderSidebarSummaryLabel).Visible = false;
    GridContainer gridContainer1 = new GridContainer();
    gridContainer1.Columns = 2;
    ((Control) gridContainer1).HorizontalExpand = true;
    GridContainer gridContainer2 = gridContainer1;
    ((Control) boxContainer6).AddChild((Control) gridContainer2);
    ((Control) gridContainer2).Visible = false;
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-gmap-sidebar-order-attack");
    ((Control) button1).HorizontalExpand = true;
    this._commanderSidebarAttackButton = button1;
    ((BaseButton) this._commanderSidebarAttackButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.BeginCommanderOrderPlacement(CivCommanderOrderType.Attack));
    ((Control) gridContainer2).AddChild((Control) this._commanderSidebarAttackButton);
    Button button2 = new Button();
    button2.Text = Loc.GetString("civ-gmap-sidebar-order-defense");
    ((Control) button2).HorizontalExpand = true;
    this._commanderSidebarDefenseButton = button2;
    ((BaseButton) this._commanderSidebarDefenseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.BeginCommanderOrderPlacement(CivCommanderOrderType.Defense));
    ((Control) gridContainer2).AddChild((Control) this._commanderSidebarDefenseButton);
    Button button3 = new Button();
    button3.Text = Loc.GetString("civ-gmap-sidebar-order-artillery");
    ((Control) button3).HorizontalExpand = true;
    ((Control) button3).Visible = false;
    this._commanderSidebarArtilleryOrderButton = button3;
    ((BaseButton) this._commanderSidebarArtilleryOrderButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.BeginCommanderOrderPlacement(CivCommanderOrderType.Artillery));
    ((Control) gridContainer2).AddChild((Control) this._commanderSidebarArtilleryOrderButton);
    Button button4 = new Button();
    button4.Text = Loc.GetString("civ-gmap-sidebar-order-clear");
    ((Control) button4).HorizontalExpand = true;
    this._commanderSidebarClearOrderButton = button4;
    ((BaseButton) this._commanderSidebarClearOrderButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ClearCommanderOrder());
    ((Control) gridContainer2).AddChild((Control) this._commanderSidebarClearOrderButton);
    Button button5 = new Button();
    button5.Text = Loc.GetString("civ-gmap-sidebar-roster");
    ((Control) button5).HorizontalExpand = true;
    this._commanderSidebarRosterButton = button5;
    ((BaseButton) this._commanderSidebarRosterButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      int squadId;
      if (!this.TryGetSelectedCommanderSquadId(out squadId))
        return;
      this._system.OpenCommanderWindow(new int?(squadId));
    });
    ((Control) boxContainer6).AddChild((Control) this._commanderSidebarRosterButton);
    ((Control) this._commanderSidebarRosterButton).Visible = false;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    ((Control) boxContainer6).AddChild((Control) scrollContainer2);
    BoxContainer boxContainer8 = new BoxContainer();
    boxContainer8.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer8.SeparationOverride = new int?(6);
    ((Control) boxContainer8).HorizontalExpand = true;
    ((Control) boxContainer8).VerticalExpand = true;
    this._commanderSquadListContainer = boxContainer8;
    ((Control) scrollContainer2).AddChild((Control) this._commanderSquadListContainer);
    PanelContainer panelContainer2 = new PanelContainer();
    ((Control) panelContainer2).HorizontalExpand = true;
    ((Control) panelContainer2).VerticalExpand = true;
    ((Control) panelContainer2).SizeFlagsStretchRatio = 1.8f;
    PanelContainer panelContainer3 = panelContainer2;
    ((Control) boxContainer4).AddChild((Control) panelContainer3);
    CivGlobalMapCanvas civGlobalMapCanvas = new CivGlobalMapCanvas(this._system);
    civGlobalMapCanvas.HorizontalExpand = true;
    civGlobalMapCanvas.VerticalExpand = true;
    this._canvas = civGlobalMapCanvas;
    ((Control) panelContainer3).AddChild((Control) this._canvas);
    this._canvas.CommanderOrderPlaced += new Action(this.OnCommanderOrderPlaced);
    PanelContainer panelContainer4 = new PanelContainer();
    ((Control) panelContainer4).MinWidth = 260f;
    ((Control) panelContainer4).HorizontalExpand = false;
    ((Control) panelContainer4).VerticalExpand = true;
    ((Control) panelContainer4).Visible = false;
    this._commanderPanel = panelContainer4;
    BoxContainer boxContainer9 = new BoxContainer();
    boxContainer9.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer9.SeparationOverride = new int?(6);
    ((Control) boxContainer9).Margin = new Thickness(8f);
    ((Control) boxContainer9).HorizontalExpand = true;
    ((Control) boxContainer9).VerticalExpand = true;
    BoxContainer boxContainer10 = boxContainer9;
    ((Control) this._commanderPanel).AddChild((Control) boxContainer10);
    BoxContainer boxContainer11 = boxContainer10;
    Label label6 = new Label();
    label6.Text = Loc.GetString("civ-gmap-commander-title");
    ((Control) label6).StyleClasses.Add("LabelHeading");
    ((Control) boxContainer11).AddChild((Control) label6);
    Label label7 = new Label();
    label7.Text = Loc.GetString("civ-gmap-commander-no-data");
    ((Control) label7).HorizontalExpand = true;
    this._commanderSummaryLabel = label7;
    ((Control) boxContainer10).AddChild((Control) this._commanderSummaryLabel);
    ((Control) boxContainer10).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-gmap-commander-squad-control")
    });
    OptionButton optionButton1 = new OptionButton();
    ((Control) optionButton1).HorizontalExpand = true;
    this._commanderSquadSelector = optionButton1;
    this._commanderSquadSelector.OnItemSelected += (Action<OptionButton.ItemSelectedEventArgs>) (args =>
    {
      this._commanderSquadSelector.SelectId(args.Id);
      int squadId;
      if (!this._squadSelectorToSquadId.TryGetValue(args.Id, out squadId))
        return;
      this.SelectCommanderSquad(squadId);
    });
    ((Control) boxContainer10).AddChild((Control) this._commanderSquadSelector);
    GridContainer gridContainer3 = new GridContainer();
    gridContainer3.Columns = 2;
    ((Control) gridContainer3).HorizontalExpand = true;
    GridContainer gridContainer4 = gridContainer3;
    ((Control) boxContainer10).AddChild((Control) gridContainer4);
    Button button6 = new Button();
    button6.Text = Loc.GetString("civ-gmap-commander-order-attack");
    ((Control) button6).HorizontalExpand = true;
    this._commanderAttackButton = button6;
    ((BaseButton) this._commanderAttackButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.BeginCommanderOrderPlacement(CivCommanderOrderType.Attack));
    ((Control) gridContainer4).AddChild((Control) this._commanderAttackButton);
    Button button7 = new Button();
    button7.Text = Loc.GetString("civ-gmap-commander-order-defense");
    ((Control) button7).HorizontalExpand = true;
    this._commanderDefenseButton = button7;
    ((BaseButton) this._commanderDefenseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.BeginCommanderOrderPlacement(CivCommanderOrderType.Defense));
    ((Control) gridContainer4).AddChild((Control) this._commanderDefenseButton);
    Button button8 = new Button();
    button8.Text = Loc.GetString("civ-gmap-commander-order-artillery");
    ((Control) button8).HorizontalExpand = true;
    ((Control) button8).Visible = false;
    this._commanderArtilleryOrderButton = button8;
    ((BaseButton) this._commanderArtilleryOrderButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.BeginCommanderOrderPlacement(CivCommanderOrderType.Artillery));
    ((Control) gridContainer4).AddChild((Control) this._commanderArtilleryOrderButton);
    Button button9 = new Button();
    button9.Text = Loc.GetString("civ-gmap-commander-order-clear");
    ((Control) button9).HorizontalExpand = true;
    this._commanderClearOrderButton = button9;
    ((BaseButton) this._commanderClearOrderButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ClearCommanderOrder());
    ((Control) gridContainer4).AddChild((Control) this._commanderClearOrderButton);
    ((Control) boxContainer10).AddChild((Control) new Label()
    {
      Text = Loc.GetString("civ-gmap-commander-transfer-player")
    });
    OptionButton optionButton2 = new OptionButton();
    ((Control) optionButton2).HorizontalExpand = true;
    this._commanderPlayerSelector = optionButton2;
    this._commanderPlayerSelector.OnItemSelected += (Action<OptionButton.ItemSelectedEventArgs>) (_ => this.RefreshCommanderControls());
    ((Control) boxContainer10).AddChild((Control) this._commanderPlayerSelector);
    OptionButton optionButton3 = new OptionButton();
    ((Control) optionButton3).HorizontalExpand = true;
    this._commanderDestinationSelector = optionButton3;
    this._commanderDestinationSelector.OnItemSelected += (Action<OptionButton.ItemSelectedEventArgs>) (_ => this.RefreshCommanderControls());
    ((Control) boxContainer10).AddChild((Control) this._commanderDestinationSelector);
    Button button10 = new Button();
    button10.Text = Loc.GetString("civ-gmap-commander-move");
    ((Control) button10).HorizontalExpand = true;
    this._commanderMoveButton = button10;
    ((BaseButton) this._commanderMoveButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ApplyCommanderMove());
    ((Control) boxContainer10).AddChild((Control) this._commanderMoveButton);
    ScrollContainer scrollContainer3 = new ScrollContainer();
    ((Control) scrollContainer3).HorizontalExpand = true;
    ((Control) scrollContainer3).VerticalExpand = true;
    ScrollContainer scrollContainer4 = scrollContainer3;
    ((Control) boxContainer10).AddChild((Control) scrollContainer4);
    BoxContainer boxContainer12 = new BoxContainer();
    boxContainer12.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer12.SeparationOverride = new int?(4);
    ((Control) boxContainer12).HorizontalExpand = true;
    ((Control) boxContainer12).VerticalExpand = true;
    this._commanderRosterContainer = boxContainer12;
    ((Control) scrollContainer4).AddChild((Control) this._commanderRosterContainer);
    BoxContainer boxContainer13 = new BoxContainer();
    boxContainer13.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer13.SeparationOverride = new int?(6);
    ((Control) boxContainer13).HorizontalExpand = true;
    BoxContainer boxContainer14 = boxContainer13;
    ((Control) boxContainer2).AddChild((Control) boxContainer14);
    Button button11 = new Button();
    button11.Text = Loc.GetString("civ-gmap-tool-center-self");
    ((Control) button11).HorizontalExpand = true;
    Button button12 = button11;
    ((BaseButton) button12).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._canvas.CenterOnSelf());
    ((Control) boxContainer14).AddChild((Control) button12);
    Button button13 = new Button();
    button13.Text = Loc.GetString("civ-gmap-tool-commander");
    ((Control) button13).HorizontalExpand = true;
    ((Control) button13).Visible = false;
    this._openCommanderButton = button13;
    ((BaseButton) this._openCommanderButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._system.OpenCommanderWindow());
    ((Control) boxContainer14).AddChild((Control) this._openCommanderButton);
    GridContainer gridContainer5 = new GridContainer();
    gridContainer5.Columns = 3;
    ((Control) gridContainer5).HorizontalExpand = true;
    GridContainer grid = gridContainer5;
    ((Control) boxContainer2).AddChild((Control) grid);
    this.AddMarkerButton(grid, CivGlobalMapMarkerType.Attack, Loc.GetString("civ-gmap-marker-attack"));
    this.AddMarkerButton(grid, CivGlobalMapMarkerType.Defense, Loc.GetString("civ-gmap-marker-defense"));
    this.AddMarkerButton(grid, CivGlobalMapMarkerType.Enemy, Loc.GetString("civ-gmap-marker-enemy"));
    this.AddMarkerButton(grid, CivGlobalMapMarkerType.Help, Loc.GetString("civ-gmap-marker-help"));
    this.AddMarkerButton(grid, CivGlobalMapMarkerType.Allies, Loc.GetString("civ-gmap-marker-allies"));
    Button button14 = new Button();
    button14.Text = Loc.GetString("civ-gmap-tool-remove-markers");
    ((BaseButton) button14).ToggleMode = true;
    ((Control) button14).HorizontalExpand = true;
    this._removeModeButton = button14;
    ((BaseButton) this._removeModeButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnRemoveModeToggled);
    ((Control) grid).AddChild((Control) this._removeModeButton);
    this.UpdateCommanderPanel();
    this.UpdateCommanderSidebar();
  }

  public void UpdateState(
    MapId mapId,
    bool hasBounds,
    Vector2 boundsMin,
    Vector2 boundsMax,
    int teamId,
    int squadId,
    bool isSquadLeader,
    bool isCommander,
    string statusLabel,
    float roundTimeLeftSeconds,
    int team1AliveCount,
    int team2AliveCount,
    int team1Score,
    int team2Score,
    IReadOnlyList<CivGlobalMapMarkerState> markers,
    IReadOnlyList<CivGlobalMapPlayerState> players,
    IReadOnlyList<CivPointCapturePointState> points,
    IReadOnlyList<CivCommanderOrderState> orders,
    IReadOnlyList<CivGlobalMapDeathState> deaths,
    IReadOnlyList<CivFobMarkerState> fobs,
    CivCommanderState? commanderState)
  {
    this._isSquadLeader = isSquadLeader;
    this._isCommander = isCommander;
    this._commanderState = commanderState;
    string str1 = Loc.GetString(teamId == 2 ? "civ-team-short-rf" : "civ-team-short-usa");
    Label roleLabel = this._roleLabel;
    string str2;
    if (!isSquadLeader)
      str2 = Loc.GetString("civ-gmap-role-squad-member", new (string, object)[2]
      {
        ("squad", (object) squadId),
        ("team", (object) str1)
      });
    else
      str2 = Loc.GetString("civ-gmap-role-squad-leader", new (string, object)[2]
      {
        ("squad", (object) squadId),
        ("team", (object) str1)
      });
    roleLabel.Text = str2;
    if (squadId == 0 && !isCommander)
      this._roleLabel.Text = Loc.GetString("civ-gmap-role-reserve", new (string, object)[1]
      {
        ("team", (object) str1)
      });
    if (isCommander)
      this._roleLabel.Text = Loc.GetString("civ-gmap-role-commander", new (string, object)[1]
      {
        ("team", (object) str1)
      });
    int num1 = teamId == 2 ? team2Score : team1Score;
    int num2 = teamId == 2 ? team1Score : team2Score;
    this._roundStatusLabel.Text = Loc.GetString("civ-gmap-round-status", new (string, object)[6]
    {
      ("status", (object) statusLabel),
      ("time", (object) CivGlobalMapWindow.FormatTime(roundTimeLeftSeconds)),
      ("usaAlive", (object) team1AliveCount),
      ("rfAlive", (object) team2AliveCount),
      ("ownScore", (object) num1),
      ("enemyScore", (object) num2)
    });
    this._canvas.UpdateData(mapId, hasBounds, boundsMin, boundsMax, teamId, squadId, markers, players, points, orders, deaths, fobs, isCommander ? this._system.GetCommanderSelectedSquadId() : new int?());
    this.UpdateCommanderPanel();
    this.UpdateCommanderSidebar();
    this.UpdateButtonsAvailability();
    CivGlobalMapMarkerType? selectedMarkerType = this._selectedMarkerType;
    if (!selectedMarkerType.HasValue || !selectedMarkerType.GetValueOrDefault().IsGlobal() || this._isSquadLeader || this._isCommander)
      return;
    this.SelectMarkerType(new CivGlobalMapMarkerType?());
  }

  private void AddMarkerButton(GridContainer grid, CivGlobalMapMarkerType markerType, string text)
  {
    Button button1 = new Button();
    button1.Text = text;
    ((BaseButton) button1).ToggleMode = true;
    ((Control) button1).HorizontalExpand = true;
    Button button2 = button1;
    ((BaseButton) button2).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (args =>
    {
      if (args.Pressed)
      {
        this.SelectMarkerType(new CivGlobalMapMarkerType?(markerType));
      }
      else
      {
        CivGlobalMapMarkerType? selectedMarkerType = this._selectedMarkerType;
        CivGlobalMapMarkerType globalMapMarkerType = markerType;
        if (!(selectedMarkerType.GetValueOrDefault() == globalMapMarkerType & selectedMarkerType.HasValue))
          return;
        this.SelectMarkerType(new CivGlobalMapMarkerType?());
      }
    });
    this._markerButtons[markerType] = button2;
    ((Control) grid).AddChild((Control) button2);
  }

  private void SelectMarkerType(CivGlobalMapMarkerType? markerType)
  {
    if (this._isCommander && markerType.HasValue)
    {
      CivGlobalMapMarkerType valueOrDefault = markerType.GetValueOrDefault();
      CivCommanderOrderType orderType;
      if (this.TryGetSelectedCommanderSquadId(out int _) && CivGlobalMapWindow.TryGetCommanderOrderType(valueOrDefault, out orderType))
      {
        this.BeginCommanderOrderPlacement(orderType);
        return;
      }
    }
    if (markerType.HasValue && this._pendingOrderType.HasValue)
      this.CancelCommanderOrderPlacement();
    this._selectedMarkerType = markerType;
    foreach ((CivGlobalMapMarkerType key, Button button1) in this._markerButtons)
    {
      CivGlobalMapMarkerType globalMapMarkerType = key;
      Button button2 = button1;
      int num;
      if (markerType.HasValue)
      {
        CivGlobalMapMarkerType? nullable = markerType;
        key = globalMapMarkerType;
        num = nullable.GetValueOrDefault() == key & nullable.HasValue ? 1 : 0;
      }
      else
        num = 0;
      bool flag = num != 0;
      if (((BaseButton) button2).Pressed != flag)
        ((BaseButton) button2).Pressed = flag;
    }
    if (markerType.HasValue && ((BaseButton) this._removeModeButton).Pressed)
      ((BaseButton) this._removeModeButton).Pressed = false;
    this._canvas.SelectedMarkerType = markerType;
    this._canvas.RemoveMode = ((BaseButton) this._removeModeButton).Pressed;
    this.UpdateHint();
  }

  private void OnRemoveModeToggled(BaseButton.ButtonToggledEventArgs args)
  {
    if (args.Pressed)
    {
      if (this._pendingOrderType.HasValue)
        this.CancelCommanderOrderPlacement();
      this.SelectMarkerType(new CivGlobalMapMarkerType?());
    }
    this._canvas.RemoveMode = args.Pressed;
    this.UpdateHint();
  }

  private void UpdateButtonsAvailability()
  {
    foreach ((CivGlobalMapMarkerType globalMapMarkerType, Button button) in this._markerButtons)
      ((BaseButton) button).Disabled = globalMapMarkerType.IsGlobal() && !this._isSquadLeader && !this._isCommander;
    ((BaseButton) this._removeModeButton).Disabled = !this._isSquadLeader && !this._isCommander;
    ((Control) this._openCommanderButton).Visible = false;
    ((Control) this._commanderSidebar).Visible = this._isCommander;
  }

  private void UpdateHint()
  {
    if (this._canvas.RemoveMode)
    {
      this._hintLabel.Text = Loc.GetString("civ-gmap-hint-remove-mode");
    }
    else
    {
      int? pendingOrderSquadId = this._pendingOrderSquadId;
      if (pendingOrderSquadId.HasValue)
      {
        int valueOrDefault = pendingOrderSquadId.GetValueOrDefault();
        CivCommanderOrderType? pendingOrderType = this._pendingOrderType;
        if (pendingOrderType.HasValue)
        {
          this._hintLabel.Text = Loc.GetString("civ-gmap-hint-pending-order", new (string, object)[2]
          {
            ("order", (object) CivGlobalMapWindow.GetCommanderOrderDisplayName(pendingOrderType.GetValueOrDefault())),
            ("squad", (object) valueOrDefault)
          });
          return;
        }
      }
      if (!this._selectedMarkerType.HasValue)
        this._hintLabel.Text = Loc.GetString("civ-gmap-hint-default");
      else
        this._hintLabel.Text = Loc.GetString("civ-gmap-hint-place-marker", new (string, object)[1]
        {
          ("marker", (object) (this._markerButtons[this._selectedMarkerType.Value].Text ?? string.Empty))
        });
    }
  }

  private void BeginCommanderOrderPlacement(CivCommanderOrderType orderType)
  {
    int squadId;
    if (!this._isCommander || !this.TryGetSelectedCommanderSquadId(out squadId))
      return;
    this._pendingOrderSquadId = new int?(squadId);
    this._pendingOrderType = new CivCommanderOrderType?(orderType);
    this._canvas.PendingCommanderOrderSquadId = new int?(squadId);
    this._canvas.PendingCommanderOrderType = new CivCommanderOrderType?(orderType);
    this.SelectMarkerType(new CivGlobalMapMarkerType?());
    if (((BaseButton) this._removeModeButton).Pressed)
      ((BaseButton) this._removeModeButton).Pressed = false;
    this.UpdateCommanderPanel();
    this.UpdateCommanderSidebar();
    this.UpdateHint();
  }

  private void ClearCommanderOrder()
  {
    int squadId;
    if (!this._isCommander || !this.TryGetSelectedCommanderSquadId(out squadId))
      return;
    this._system.RequestCommanderSetOrder(squadId, CivCommanderOrderType.None, MapId.Nullspace, Vector2.Zero);
    this.CancelCommanderOrderPlacement();
  }

  private void ApplyCommanderMove()
  {
    NetUserId userId;
    CommanderDestination commanderDestination;
    if (!this._isCommander || !this.TryGetSelectedCommanderPlayerId(out userId) || !this._destinationSelectorMap.TryGetValue(this._commanderDestinationSelector.SelectedId, out commanderDestination))
      return;
    this._system.RequestCommanderMovePlayer(userId, commanderDestination.SquadId, commanderDestination.CreateNewSquad);
  }

  private void CancelCommanderOrderPlacement()
  {
    this._pendingOrderSquadId = new int?();
    this._pendingOrderType = new CivCommanderOrderType?();
    this._canvas.PendingCommanderOrderSquadId = new int?();
    this._canvas.PendingCommanderOrderType = new CivCommanderOrderType?();
    this.UpdateCommanderPanel();
    this.UpdateCommanderSidebar();
    this.UpdateHint();
  }

  private void OnCommanderOrderPlaced() => this.CancelCommanderOrderPlacement();

  private void UpdateCommanderPanel()
  {
    ((Control) this._commanderPanel).Visible = this._isCommander;
    if (!this._isCommander)
    {
      this._pendingOrderSquadId = new int?();
      this._pendingOrderType = new CivCommanderOrderType?();
      this._canvas.PendingCommanderOrderSquadId = new int?();
      this._canvas.PendingCommanderOrderType = new CivCommanderOrderType?();
    }
    else
    {
      this.RebuildCommanderSelectors();
      this.RebuildCommanderRosterList();
      this.RefreshCommanderControls();
    }
  }

  private void UpdateCommanderSidebar()
  {
    ((Control) this._commanderSidebar).Visible = this._isCommander;
    if (!this._isCommander)
    {
      this._commanderSidebarSummaryLabel.Text = Loc.GetString("civ-gmap-sidebar-no-data");
      ((Control) this._commanderSquadListContainer).DisposeAllChildren();
    }
    else
    {
      this.RebuildCommanderSquadList();
      this.RefreshCommanderSidebarControls();
    }
  }

  private void RefreshCommanderSidebarControls()
  {
    bool selectedCommanderSquad = this.TryGetSelectedCommanderSquad(out CivCommanderSquadState _);
    ((BaseButton) this._commanderSidebarAttackButton).Disabled = !selectedCommanderSquad;
    ((BaseButton) this._commanderSidebarDefenseButton).Disabled = !selectedCommanderSquad;
    ((BaseButton) this._commanderSidebarArtilleryOrderButton).Disabled = !selectedCommanderSquad;
    ((BaseButton) this._commanderSidebarClearOrderButton).Disabled = !selectedCommanderSquad;
    ((BaseButton) this._commanderSidebarRosterButton).Disabled = !selectedCommanderSquad;
    this._commanderSidebarSummaryLabel.Text = this.BuildCommanderSummary();
  }

  private void RebuildCommanderSquadList()
  {
    ((Control) this._commanderSquadListContainer).DisposeAllChildren();
    if (this._commanderState == null)
      return;
    int? commanderSelectedSquadId = this._system.GetCommanderSelectedSquadId();
    foreach (CivCommanderSquadState commanderSquadState in (IEnumerable<CivCommanderSquadState>) this._commanderState.Squads.OrderBy<CivCommanderSquadState, int>((Func<CivCommanderSquadState, int>) (entry => entry.SquadId)))
    {
      CivCommanderSquadState squad = commanderSquadState;
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
      boxContainer1.SeparationOverride = new int?(4);
      ((Control) boxContainer1).HorizontalExpand = true;
      BoxContainer boxContainer2 = boxContainer1;
      BoxContainer boxContainer3 = new BoxContainer();
      boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
      boxContainer3.SeparationOverride = new int?(4);
      ((Control) boxContainer3).HorizontalExpand = false;
      BoxContainer boxContainer4 = boxContainer3;
      Button button1 = new Button();
      button1.Text = Loc.GetString("civ-gmap-squad-button", new (string, object)[1]
      {
        ("squad", (object) squad.SquadId)
      });
      ((BaseButton) button1).ToggleMode = true;
      Button button2 = button1;
      int? nullable = commanderSelectedSquadId;
      int squadId = squad.SquadId;
      int num = nullable.GetValueOrDefault() == squadId & nullable.HasValue ? 1 : 0;
      ((BaseButton) button2).Pressed = num != 0;
      ((Control) button1).HorizontalExpand = true;
      Button button3 = button1;
      ((BaseButton) button3).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SelectCommanderSquad(squad.SquadId));
      ((Control) boxContainer4).AddChild((Control) button3);
      Button button4 = new Button()
      {
        Text = Loc.GetString("civ-gmap-sidebar-roster")
      };
      ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._system.OpenCommanderWindow(new int?(squad.SquadId)));
      ((Control) boxContainer4).AddChild((Control) button4);
      ((Control) boxContainer2).AddChild((Control) boxContainer4);
      BoxContainer boxContainer5 = boxContainer2;
      Label label1 = new Label();
      label1.Text = Loc.GetString("civ-gmap-squad-info", new (string, object)[3]
      {
        ("leader", (object) squad.LeaderName),
        ("members", (object) squad.Members.Count),
        ("order", (object) CivGlobalMapWindow.GetCommanderOrderDisplayName(squad.Order))
      });
      ((Control) label1).HorizontalExpand = true;
      Label label2 = label1;
      ((Control) boxContainer5).AddChild((Control) label2);
      ((Control) this._commanderSquadListContainer).AddChild((Control) boxContainer2);
    }
    BoxContainer squadListContainer = this._commanderSquadListContainer;
    Label label3 = new Label();
    label3.Text = Loc.GetString("civ-gmap-reserve-count", new (string, object)[1]
    {
      ("count", (object) this._commanderState.ReservePlayers.Count)
    });
    label3.FontColorOverride = new Color?(Color.LightGray);
    ((Control) label3).HorizontalExpand = true;
    Label label4 = label3;
    ((Control) squadListContainer).AddChild((Control) label4);
  }

  private void SelectCommanderSquad(int squadId)
  {
    if (!this._isCommander)
      return;
    this._system.SetCommanderSelectedSquad(squadId);
    this._canvas.CommanderSelectedSquadId = new int?(squadId);
    this.UpdateCommanderPanel();
    this.UpdateCommanderSidebar();
  }

  private void SelectCommanderPlayer(NetUserId userId)
  {
    int key = this._playerSelectorToUserId.FirstOrDefault<KeyValuePair<int, NetUserId>>((Func<KeyValuePair<int, NetUserId>, bool>) (pair => NetUserId.op_Equality(pair.Value, userId))).Key;
    if (key == 0)
      return;
    this._commanderPlayerSelector.SelectId(key);
    this.RefreshCommanderControls();
  }

  private string BuildCommanderSummary()
  {
    if (this._commanderState == null)
      return Loc.GetString("civ-gmap-sidebar-no-snapshot");
    string str = Loc.GetString("civ-gmap-sidebar-summary", new (string, object)[3]
    {
      ("team", (object) this._commanderState.TeamId),
      ("squads", (object) this._commanderState.Squads.Count),
      ("reserve", (object) this._commanderState.ReservePlayers.Count)
    });
    CivCommanderSquadState squad;
    if (this.TryGetSelectedCommanderSquad(out squad))
      str = $"{str}\n{Loc.GetString("civ-gmap-sidebar-summary-squad", new (string, object)[2]
      {
        ("squad", (object) squad.SquadId),
        ("order", (object) CivGlobalMapWindow.GetCommanderOrderDisplayName(squad.Order))
      })}";
    int? pendingOrderSquadId = this._pendingOrderSquadId;
    if (pendingOrderSquadId.HasValue)
    {
      int valueOrDefault1 = pendingOrderSquadId.GetValueOrDefault();
      CivCommanderOrderType? pendingOrderType = this._pendingOrderType;
      if (pendingOrderType.HasValue)
      {
        CivCommanderOrderType valueOrDefault2 = pendingOrderType.GetValueOrDefault();
        str = $"{str}\n{Loc.GetString("civ-gmap-sidebar-summary-pending", new (string, object)[2]
        {
          ("order", (object) CivGlobalMapWindow.GetCommanderOrderDisplayName(valueOrDefault2)),
          ("squad", (object) valueOrDefault1)
        })}";
      }
    }
    return str;
  }

  private bool TryGetSelectedCommanderSquad([NotNullWhen(true)] out CivCommanderSquadState? squad)
  {
    squad = (CivCommanderSquadState) null;
    int squadId;
    if (this._commanderState == null || !this.TryGetSelectedCommanderSquadId(out squadId))
      return false;
    squad = this._commanderState.Squads.FirstOrDefault<CivCommanderSquadState>((Func<CivCommanderSquadState, bool>) (entry => entry.SquadId == squadId));
    return squad != null;
  }

  private void RefreshCommanderControls()
  {
    int squadId;
    bool commanderSquadId = this.TryGetSelectedCommanderSquadId(out squadId);
    bool commanderPlayerId = this.TryGetSelectedCommanderPlayerId(out NetUserId _);
    bool flag = this._destinationSelectorMap.ContainsKey(this._commanderDestinationSelector.SelectedId);
    ((BaseButton) this._commanderAttackButton).Disabled = !commanderSquadId;
    ((BaseButton) this._commanderDefenseButton).Disabled = !commanderSquadId;
    ((BaseButton) this._commanderArtilleryOrderButton).Disabled = !commanderSquadId;
    ((BaseButton) this._commanderClearOrderButton).Disabled = !commanderSquadId;
    ((BaseButton) this._commanderMoveButton).Disabled = !commanderPlayerId || !flag;
    if (this._commanderState == null)
    {
      this._commanderSummaryLabel.Text = Loc.GetString("civ-gmap-sidebar-no-snapshot");
    }
    else
    {
      string str = Loc.GetString("civ-gmap-sidebar-summary", new (string, object)[3]
      {
        ("team", (object) this._commanderState.TeamId),
        ("squads", (object) this._commanderState.Squads.Count),
        ("reserve", (object) this._commanderState.ReservePlayers.Count)
      });
      if (commanderSquadId)
      {
        CivCommanderSquadState commanderSquadState = this._commanderState.Squads.FirstOrDefault<CivCommanderSquadState>((Func<CivCommanderSquadState, bool>) (entry => entry.SquadId == squadId));
        if (commanderSquadState != null)
          str = $"{str}\n{Loc.GetString("civ-gmap-sidebar-summary-squad", new (string, object)[2]
          {
            ("squad", (object) commanderSquadState.SquadId),
            ("order", (object) CivGlobalMapWindow.GetCommanderOrderDisplayName(commanderSquadState.Order))
          })}";
      }
      int? pendingOrderSquadId = this._pendingOrderSquadId;
      if (pendingOrderSquadId.HasValue)
      {
        int valueOrDefault1 = pendingOrderSquadId.GetValueOrDefault();
        CivCommanderOrderType? pendingOrderType = this._pendingOrderType;
        if (pendingOrderType.HasValue)
        {
          CivCommanderOrderType valueOrDefault2 = pendingOrderType.GetValueOrDefault();
          str = $"{str}\n{Loc.GetString("civ-gmap-sidebar-summary-pending", new (string, object)[2]
          {
            ("order", (object) CivGlobalMapWindow.GetCommanderOrderDisplayName(valueOrDefault2)),
            ("squad", (object) valueOrDefault1)
          })}";
        }
      }
      this._commanderSummaryLabel.Text = str;
    }
  }

  private void RebuildCommanderSelectors()
  {
    int? nullable1 = this._system.GetCommanderSelectedSquadId() ?? this._pendingOrderSquadId;
    NetUserId netUserId1;
    NetUserId? selectedPlayerId = this._playerSelectorToUserId.TryGetValue(this._commanderPlayerSelector.SelectedId, out netUserId1) ? new NetUserId?(netUserId1) : new NetUserId?();
    int selectedId = this._commanderDestinationSelector.SelectedId;
    this._squadSelectorToSquadId.Clear();
    this._commanderSquadSelector.Clear();
    this._playerSelectorToUserId.Clear();
    this._commanderPlayerSelector.Clear();
    this._destinationSelectorMap.Clear();
    this._commanderDestinationSelector.Clear();
    if (this._commanderState == null)
      return;
    foreach (CivCommanderSquadState commanderSquadState in (IEnumerable<CivCommanderSquadState>) this._commanderState.Squads.OrderBy<CivCommanderSquadState, int>((Func<CivCommanderSquadState, int>) (entry => entry.SquadId)))
    {
      this._squadSelectorToSquadId[commanderSquadState.SquadId] = commanderSquadState.SquadId;
      this._commanderSquadSelector.AddItem(Loc.GetString("civ-gmap-squad-button", new (string, object)[1]
      {
        ("squad", (object) commanderSquadState.SquadId)
      }), new int?(commanderSquadState.SquadId));
      this._destinationSelectorMap[commanderSquadState.SquadId] = new CommanderDestination(commanderSquadState.SquadId, false);
      this._commanderDestinationSelector.AddItem(Loc.GetString("civ-gmap-commander-destination-squad", new (string, object)[1]
      {
        ("squad", (object) commanderSquadState.SquadId)
      }), new int?(commanderSquadState.SquadId));
    }
    if (this._commanderState.Squads.Count > 0)
    {
      int num;
      if (nullable1.HasValue)
      {
        int valueOrDefault = nullable1.GetValueOrDefault();
        if (this._squadSelectorToSquadId.ContainsKey(valueOrDefault))
        {
          num = valueOrDefault;
          goto label_14;
        }
      }
      num = this._commanderState.Squads[0].SquadId;
label_14:
      int squadId = num;
      this._canvas.CommanderSelectedSquadId = new int?(squadId);
      this._system.SetCommanderSelectedSquad(squadId);
      this._commanderSquadSelector.SelectId(squadId);
    }
    this._destinationSelectorMap[-1] = new CommanderDestination(0, false);
    this._destinationSelectorMap[-2] = new CommanderDestination(0, true);
    this._commanderDestinationSelector.AddItem(Loc.GetString("civ-gmap-commander-destination-reserve"), new int?(-1));
    this._commanderDestinationSelector.AddItem(Loc.GetString("civ-gmap-commander-destination-new-squad"), new int?(-2));
    List<CivCommanderPlayerState> list = this._commanderState.Squads.SelectMany<CivCommanderSquadState, CivCommanderPlayerState>((Func<CivCommanderSquadState, IEnumerable<CivCommanderPlayerState>>) (entry => (IEnumerable<CivCommanderPlayerState>) entry.Members)).Concat<CivCommanderPlayerState>((IEnumerable<CivCommanderPlayerState>) this._commanderState.ReservePlayers).Where<CivCommanderPlayerState>((Func<CivCommanderPlayerState, bool>) (player => !player.IsCommander)).OrderBy<CivCommanderPlayerState, string>((Func<CivCommanderPlayerState, string>) (player => player.Name)).ToList<CivCommanderPlayerState>();
    foreach (CivCommanderPlayerState commanderPlayerState in list)
    {
      int key = this._playerSelectorToUserId.Count + 1;
      this._playerSelectorToUserId[key] = commanderPlayerState.UserId;
      string str1;
      if (commanderPlayerState.SquadId != 0)
        str1 = Loc.GetString("civ-gmap-commander-player-squad-num", new (string, object)[1]
        {
          ("squad", (object) commanderPlayerState.SquadId)
        });
      else
        str1 = Loc.GetString("civ-gmap-commander-player-squad-reserve");
      string str2 = str1;
      this._commanderPlayerSelector.AddItem(Loc.GetString("civ-gmap-commander-player-option", new (string, object)[2]
      {
        ("name", (object) commanderPlayerState.Name),
        ("squad", (object) str2)
      }), new int?(key));
    }
    if (list.Count > 0)
    {
      int key = this._playerSelectorToUserId.FirstOrDefault<KeyValuePair<int, NetUserId>>((Func<KeyValuePair<int, NetUserId>, bool>) (pair =>
      {
        NetUserId netUserId2 = pair.Value;
        NetUserId? nullable2 = selectedPlayerId;
        return nullable2.HasValue && NetUserId.op_Equality(netUserId2, nullable2.GetValueOrDefault());
      })).Key;
      this._commanderPlayerSelector.SelectId(key != 0 ? key : this._playerSelectorToUserId.Keys.Min());
    }
    if (this._destinationSelectorMap.Count <= 0)
      return;
    this._commanderDestinationSelector.SelectId(this._destinationSelectorMap.ContainsKey(selectedId) ? selectedId : -1);
  }

  private void RebuildCommanderRosterList()
  {
    ((Control) this._commanderRosterContainer).DisposeAllChildren();
    if (this._commanderState == null)
      return;
    foreach (CivCommanderSquadState commanderSquadState in (IEnumerable<CivCommanderSquadState>) this._commanderState.Squads.OrderBy<CivCommanderSquadState, int>((Func<CivCommanderSquadState, int>) (entry => entry.SquadId)))
    {
      string orderDisplayName = CivGlobalMapWindow.GetCommanderOrderDisplayName(commanderSquadState.Order);
      BoxContainer commanderRosterContainer1 = this._commanderRosterContainer;
      Label label1 = new Label();
      label1.Text = Loc.GetString("civ-gmap-roster-squad", new (string, object)[3]
      {
        ("squad", (object) commanderSquadState.SquadId),
        ("leader", (object) commanderSquadState.LeaderName),
        ("order", (object) orderDisplayName)
      });
      ((Control) label1).HorizontalExpand = true;
      Label label2 = label1;
      ((Control) commanderRosterContainer1).AddChild((Control) label2);
      foreach (CivCommanderPlayerState commanderPlayerState in (IEnumerable<CivCommanderPlayerState>) commanderSquadState.Members.OrderByDescending<CivCommanderPlayerState, bool>((Func<CivCommanderPlayerState, bool>) (player => player.IsSquadLeader)).ThenBy<CivCommanderPlayerState, string>((Func<CivCommanderPlayerState, string>) (player => player.Name)))
      {
        string str = commanderPlayerState.IsSquadLeader ? Loc.GetString("civ-gmap-roster-leader-mark") : string.Empty;
        BoxContainer commanderRosterContainer2 = this._commanderRosterContainer;
        Label label3 = new Label();
        label3.Text = Loc.GetString("civ-gmap-roster-member", new (string, object)[2]
        {
          ("mark", (object) str),
          ("name", (object) commanderPlayerState.Name)
        });
        ((Control) label3).HorizontalExpand = true;
        Label label4 = label3;
        ((Control) commanderRosterContainer2).AddChild((Control) label4);
      }
    }
    BoxContainer commanderRosterContainer3 = this._commanderRosterContainer;
    Label label5 = new Label();
    label5.Text = Loc.GetString("civ-gmap-roster-reserve");
    ((Control) label5).StyleClasses.Add("LabelHeading");
    ((Control) commanderRosterContainer3).AddChild((Control) label5);
    if (this._commanderState.ReservePlayers.Count == 0)
    {
      ((Control) this._commanderRosterContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("civ-gmap-roster-reserve-empty")
      });
    }
    else
    {
      foreach (CivCommanderPlayerState commanderPlayerState in (IEnumerable<CivCommanderPlayerState>) this._commanderState.ReservePlayers.OrderBy<CivCommanderPlayerState, string>((Func<CivCommanderPlayerState, string>) (player => player.Name)))
      {
        BoxContainer commanderRosterContainer4 = this._commanderRosterContainer;
        Label label6 = new Label();
        label6.Text = Loc.GetString("civ-gmap-roster-reserve-member", new (string, object)[1]
        {
          ("name", (object) commanderPlayerState.Name)
        });
        ((Control) label6).HorizontalExpand = true;
        Label label7 = label6;
        ((Control) commanderRosterContainer4).AddChild((Control) label7);
      }
    }
  }

  private bool TryGetSelectedCommanderSquadId(out int squadId)
  {
    return this._squadSelectorToSquadId.TryGetValue(this._commanderSquadSelector.SelectedId, out squadId);
  }

  private bool TryGetSelectedCommanderPlayerId(out NetUserId userId)
  {
    return this._playerSelectorToUserId.TryGetValue(this._commanderPlayerSelector.SelectedId, out userId);
  }

  private static string GetCommanderOrderDisplayName(CivCommanderOrderType orderType)
  {
    string orderDisplayName;
    switch (orderType)
    {
      case CivCommanderOrderType.Attack:
        orderDisplayName = Loc.GetString("civ-gmap-order-attack");
        break;
      case CivCommanderOrderType.Defense:
        orderDisplayName = Loc.GetString("civ-gmap-order-defense");
        break;
      case CivCommanderOrderType.Artillery:
        orderDisplayName = Loc.GetString("civ-gmap-order-artillery");
        break;
      default:
        orderDisplayName = Loc.GetString("civ-gmap-order-none");
        break;
    }
    return orderDisplayName;
  }

  private static bool TryGetCommanderOrderType(
    CivGlobalMapMarkerType markerType,
    out CivCommanderOrderType orderType)
  {
    CivCommanderOrderType commanderOrderType;
    switch (markerType)
    {
      case CivGlobalMapMarkerType.Attack:
        commanderOrderType = CivCommanderOrderType.Attack;
        break;
      case CivGlobalMapMarkerType.Defense:
        commanderOrderType = CivCommanderOrderType.Defense;
        break;
      default:
        commanderOrderType = CivCommanderOrderType.None;
        break;
    }
    orderType = commanderOrderType;
    return orderType != 0;
  }

  private static string FormatTime(float totalSeconds)
  {
    if (!float.IsFinite(totalSeconds) || (double) totalSeconds <= 0.0)
      return "00:00";
    TimeSpan timeSpan = TimeSpan.FromSeconds((double) totalSeconds);
    if (timeSpan.TotalHours >= 1.0)
      return $"{(int) timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
  }
}
