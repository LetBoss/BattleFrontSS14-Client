// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Commander.UI;
using Content.Client._CIV14merka.GlobalMap;
using Content.Client._CIV14merka.Supply;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [Dependency]
  private IPlayerManager _player;
  [UISystemDependency]
  private readonly CivCommanderPurchasePlacementSystem? _placement;
  [UISystemDependency]
  private readonly CivCommanderBotControlSystem? _botControl;
  [UISystemDependency]
  private readonly CivCommanderTeleportSystem? _teleport;
  [UISystemDependency]
  private readonly CivGlobalMapSystem? _globalMap;
  [UISystemDependency]
  private readonly CivAirstrikeSystem? _airstrike;
  [UISystemDependency]
  private readonly CivCommanderLinesSystem? _lines;
  [UISystemDependency]
  private readonly CivSupplyRefillSystem? _supplyRefill;
  private bool _loaded;
  private CivCommanderLinesGui? _linesPanel;
  private CivCommanderLabelInputWindow? _labelWindow;

  private CivCommanderGui? Gui => this.UIManager.GetActiveUIWidgetOrNull<CivCommanderGui>();

  private CivCommanderInfoGui? GuiInfo
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<CivCommanderInfoGui>();
  }

  public void OnStateEntered(GameplayState state)
  {
    this.LoadGui();
    this.EnsureLinesPanel();
  }

  public void OnStateExited(GameplayState state)
  {
    this.UnloadGui();
    this.DisposeLinesPanel();
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._loaded || this.Gui == null)
      return;
    bool flag = this.IsCommander();
    ((Control) this.Gui).Visible = flag;
    CivCommanderInfoGui guiInfo = this.GuiInfo;
    if (guiInfo != null)
    {
      ((Control) guiInfo).Visible = flag;
      CivCommanderInfoGui commanderInfoGui1 = guiInfo;
      CivGlobalMapSystem globalMap1 = this._globalMap;
      int commanderCurrency = globalMap1 != null ? globalMap1.GetCommanderCurrency() : 0;
      commanderInfoGui1.SetCurrency(commanderCurrency);
      CivCommanderInfoGui commanderInfoGui2 = guiInfo;
      CivGlobalMapSystem globalMap2 = this._globalMap;
      double seconds1 = globalMap2 != null ? (double) globalMap2.GetAirstrikeCooldown() : 0.0;
      commanderInfoGui2.SetAirstrikeCooldown((float) seconds1);
      CivCommanderInfoGui commanderInfoGui3 = guiInfo;
      CivGlobalMapSystem globalMap3 = this._globalMap;
      double seconds2 = globalMap3 != null ? (double) globalMap3.GetArtilleryCooldown() : 0.0;
      commanderInfoGui3.SetArtilleryCooldown((float) seconds2);
    }
    this.EnsureLinesPanel();
    if (this._linesPanel != null)
      ((Control) this._linesPanel).Visible = flag;
    if (flag)
      return;
    this._placement?.CancelPlacement();
    this._botControl?.ClearSelection();
  }

  private void EnsureLinesPanel()
  {
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
      return;
    if (activeScreen.GetWidget<MainViewport>() == null)
      return;
    LayoutContainer control;
    try
    {
      control = ((Control) activeScreen).FindControl<LayoutContainer>("ViewportContainer");
    }
    catch (ArgumentException ex)
    {
      return;
    }
    if (this._linesPanel == null)
    {
      this._linesPanel = new CivCommanderLinesGui();
      this._linesPanel.LineColorSelected += new Action<CivCommanderLineColor>(this.OnLineColorSelected);
      this._linesPanel.ClearLinesPressed += new Action(this.OnClearLinesPressed);
      this._linesPanel.LabelPressed += new Action(this.OnLabelPressed);
      this._linesPanel.PresetLabelPressed += new Action<string>(this.OnPresetLabelPressed);
      if (this._lines != null)
        this._linesPanel.UpdateSelectedColor(this._lines.SelectedColor);
    }
    if (((Control) this._linesPanel).Parent != control)
    {
      ((Control) this._linesPanel).Orphan();
      ((Control) control).AddChild((Control) this._linesPanel);
      LayoutContainer.SetAnchorAndMarginPreset((Control) this._linesPanel, (LayoutContainer.LayoutPreset) 2, (LayoutContainer.LayoutPresetMode) 0, 8);
      LayoutContainer.SetGrowHorizontal((Control) this._linesPanel, (LayoutContainer.GrowDirection) 0);
      LayoutContainer.SetGrowVertical((Control) this._linesPanel, (LayoutContainer.GrowDirection) 1);
    }
    ((Control) this._linesPanel).SetPositionLast();
  }

  private void DisposeLinesPanel()
  {
    if (this._linesPanel == null)
      return;
    this._linesPanel.LineColorSelected -= new Action<CivCommanderLineColor>(this.OnLineColorSelected);
    this._linesPanel.ClearLinesPressed -= new Action(this.OnClearLinesPressed);
    this._linesPanel.LabelPressed -= new Action(this.OnLabelPressed);
    this._linesPanel.PresetLabelPressed -= new Action<string>(this.OnPresetLabelPressed);
    ((Control) this._linesPanel).Orphan();
    this._linesPanel = (CivCommanderLinesGui) null;
  }

  private void LoadGui()
  {
    if (this.Gui == null || this._loaded)
      return;
    this.Gui.HeadquartersPressed += new Action(this.OnHeadquartersPressed);
    this.Gui.ShopPressed += new Action(this.OnShopPressed);
    this.Gui.AlliesPressed += new Action(this.OnAlliesPressed);
    this.Gui.FireSupportPressed += new Action(this.OnFireSupportPressed);
    this.Gui.HeliPressed += new Action(this.OnHeliPressed);
    this.Gui.SupplyPressed += new Action(this.OnSupplyPressed);
    ((Control) this.Gui).Visible = this.IsCommander();
    CivCommanderInfoGui guiInfo = this.GuiInfo;
    if (guiInfo != null)
    {
      guiInfo.BomPressed += new Action(this.OnBomPressed);
      ((Control) guiInfo).Visible = this.IsCommander();
    }
    this._loaded = true;
  }

  private void UnloadGui()
  {
    if (this.Gui != null && this._loaded)
    {
      this.Gui.HeadquartersPressed -= new Action(this.OnHeadquartersPressed);
      this.Gui.ShopPressed -= new Action(this.OnShopPressed);
      this.Gui.AlliesPressed -= new Action(this.OnAlliesPressed);
      this.Gui.FireSupportPressed -= new Action(this.OnFireSupportPressed);
      this.Gui.HeliPressed -= new Action(this.OnHeliPressed);
      this.Gui.SupplyPressed -= new Action(this.OnSupplyPressed);
      ((Control) this.Gui).Visible = false;
      CivCommanderInfoGui guiInfo = this.GuiInfo;
      if (guiInfo != null)
      {
        guiInfo.BomPressed -= new Action(this.OnBomPressed);
        ((Control) guiInfo).Visible = false;
      }
      this._loaded = false;
    }
    this._placement?.CancelPlacement();
    this._botControl?.ClearSelection();
    this._globalMap?.CloseCommanderShopWindow();
    this._teleport?.CloseWindow();
    this._supplyRefill?.CloseWindow();
  }

  private void OnHeadquartersPressed() => this._globalMap?.OpenCommanderWindow();

  private void OnAlliesPressed() => this._teleport?.OpenWindow();

  private void OnShopPressed() => this._globalMap?.OpenCommanderShopWindow();

  private void OnSupplyPressed() => this._supplyRefill?.OpenWindow();

  private void OnFireSupportPressed()
  {
    CivAirstrikeSystem airstrike = this._airstrike;
    if (airstrike == null)
      return;
    CivGlobalMapSystem globalMap1 = this._globalMap;
    double airstrikeCooldown = globalMap1 != null ? (double) globalMap1.GetAirstrikeCooldown() : 0.0;
    CivGlobalMapSystem globalMap2 = this._globalMap;
    double artilleryCooldown = globalMap2 != null ? (double) globalMap2.GetArtilleryCooldown() : 0.0;
    CivGlobalMapSystem globalMap3 = this._globalMap;
    double smokeCooldown = globalMap3 != null ? (double) globalMap3.GetSmokeSupportCooldown() : 0.0;
    Vector2? initialCoords = new Vector2?();
    airstrike.OpenWindow((float) airstrikeCooldown, (float) artilleryCooldown, (float) smokeCooldown, initialCoords);
  }

  private void OnHeliPressed()
  {
    if (!this.IsCommander())
      return;
    this._globalMap?.RequestCommanderCallHeli();
  }

  private void OnBomPressed()
  {
    if (!this.IsCommander())
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    MapCoordinates mapCoordinates = this.EntityManager.System<SharedTransformSystem>().GetMapCoordinates(valueOrDefault, (TransformComponent) null);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    this._airstrike?.RequestArtillery(mapCoordinates.Position);
  }

  private void OnLineColorSelected(CivCommanderLineColor color)
  {
    if (this._lines == null)
      return;
    this._lines.SelectedColor = color;
  }

  private void OnClearLinesPressed() => this._lines?.RequestClearAll();

  private void OnLabelPressed()
  {
    if (this._lines == null || !this.IsCommander())
      return;
    if (this._labelWindow != null)
    {
      ((BaseWindow) this._labelWindow).Close();
      this._labelWindow = (CivCommanderLabelInputWindow) null;
    }
    this._labelWindow = new CivCommanderLabelInputWindow();
    this._labelWindow.TextConfirmed += (Action<string>) (text => this._lines.StartLabelPlacement(text));
    ((BaseWindow) this._labelWindow).OnClose += (Action) (() => this._labelWindow = (CivCommanderLabelInputWindow) null);
    ((BaseWindow) this._labelWindow).OpenCentered();
  }

  private void OnPresetLabelPressed(string text)
  {
    if (this._lines == null || !this.IsCommander())
      return;
    this._lines.StartLabelPlacement(text);
  }

  private bool IsCommander()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    return localEntity.HasValue && this.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) && teamMemberComponent.IsCommander;
  }
}
