// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Lobby.LobbyUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG.Lobby;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Lobby;

public sealed class LobbyUIController : UIController, IOnStateEntered<GameplayState>
{
  private const string PubgHudInfoStackName = "PubgHudInfoStack";
  private const float MinimapSize = 200f;
  private const float MinimapBottomOffset = 220f;
  private const float StackSpacing = 10f;
  private const float StackRightMargin = 15f;
  private const float StackMinTop = 15f;
  private const float StackDownOffset = 200f;
  private const float InfoPanelWidth = 260f;
  [Dependency]
  private IConfigurationManager _cfg;
  private bool _inLobby;
  private int _lastReady;
  private int _lastTotal;
  private int _lastTimeRemaining;
  private LayoutContainer? _infoViewport;
  private BoxContainer? _infoStack;
  private bool _infoViewportSubscribed;
  private PanelContainer? _readyPanel;
  private PanelContainer? _totalPanel;
  private PanelContainer? _timePanel;
  private Label? _readyLabel;
  private Label? _totalLabel;
  private Label? _timeLabel;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<LobbyStatusEvent>(new EntitySessionEventHandler<LobbyStatusEvent>(this.OnLobbyStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntitySessionEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), (Type[]) null, (Type[]) null);
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += (Action) (() =>
    {
      if (!this._inLobby)
        return;
      this.EnsureUI();
    });
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
  }

  public void OnStateEntered(GameplayState state)
  {
    if (!this._inLobby)
      return;
    this.EnsureUI();
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this._inLobby && this._readyPanel == null)
      this.EnsureUI();
    if (this._infoViewport == null || this._infoStack == null)
      return;
    this.PositionInfoStack(this._infoViewport, (Control) this._infoStack, this.GetScreenType());
  }

  private void EnsureUI()
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
    ScreenType screenType = this.GetScreenType();
    Control infoContainer = this.GetInfoContainer(control, screenType);
    if (this._readyPanel == null)
    {
      (this._readyPanel, this._readyLabel) = this.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#28a745", new Color?()), "✓", "READY: 0/0");
      infoContainer.AddChild((Control) this._readyPanel);
      if (screenType != ScreenType.Default)
        this.ApplyPanelLayout(this._readyPanel, 60f, -520f, 15);
    }
    if (this._totalPanel == null)
    {
      (this._totalPanel, this._totalLabel) = this.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#17a2b8", new Color?()), "◉", "TOTAL: 0");
      infoContainer.AddChild((Control) this._totalPanel);
      if (screenType != ScreenType.Default)
        this.ApplyPanelLayout(this._totalPanel, 105f, -475f, 15);
    }
    if (this._timePanel == null)
    {
      (this._timePanel, this._timeLabel) = this.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#ffc107", new Color?()), "◷", "TIME: 00:00");
      infoContainer.AddChild((Control) this._timePanel);
      if (screenType != ScreenType.Default)
        this.ApplyPanelLayout(this._timePanel, 150f, -430f, 15);
    }
    this.PositionInfoStack(control, infoContainer, screenType);
    this.ApplyStatusText();
  }

  private void ApplyStatusText()
  {
    if (this._readyLabel != null)
      this._readyLabel.Text = $"READY: {this._lastReady}/{this._lastTotal}";
    if (this._totalLabel != null)
      this._totalLabel.Text = $"TOTAL: {this._lastTotal}";
    if (this._timeLabel == null)
      return;
    this._timeLabel.Text = $"TIME: {this._lastTimeRemaining / 60:D2}:{this._lastTimeRemaining % 60:D2}";
  }

  private (PanelContainer panel, Label label) CreateStyledPanel(
    Color accentColor,
    string icon,
    string text)
  {
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a1a1aDD", new Color?()),
      BorderColor = accentColor,
      BorderThickness = new Thickness(0.0f, 0.0f, 3f, 0.0f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer).MinWidth = 150f;
    BoxContainer boxContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    Label label1 = new Label();
    label1.Text = icon;
    label1.FontColorOverride = new Color?(accentColor);
    ((Control) label1).MinWidth = 20f;
    Label label2 = label1;
    Label label3 = new Label()
    {
      Text = text,
      FontColorOverride = new Color?(Color.White)
    };
    ((Control) boxContainer).AddChild((Control) label2);
    ((Control) boxContainer).AddChild((Control) label3);
    ((Control) panelContainer).AddChild((Control) boxContainer);
    return (panelContainer, label3);
  }

  private void OnLobbyStatus(LobbyStatusEvent msg, EntitySessionEventArgs args)
  {
    this._inLobby = msg.InLobby;
    if (this._inLobby)
    {
      this._lastReady = msg.ReadyPlayers;
      this._lastTotal = msg.TotalPlayers;
      if (msg.TimeRemaining > 0)
        this._lastTimeRemaining = msg.TimeRemaining;
      this.EnsureUI();
      this.ApplyStatusText();
    }
    else
      this.ClearUI();
  }

  private void OnRoundRestart(RoundRestartCleanupEvent ev, EntitySessionEventArgs args)
  {
    this._inLobby = false;
    this._lastReady = 0;
    this._lastTotal = 0;
    this._lastTimeRemaining = 0;
    this.ClearUI();
  }

  private ScreenType GetScreenType()
  {
    ScreenType result;
    return !Enum.TryParse<ScreenType>(this._cfg.GetCVar<string>(CCVars.UILayout), out result) ? ScreenType.Default : result;
  }

  private Control GetInfoContainer(LayoutContainer viewportContainer, ScreenType screenType)
  {
    if (screenType != ScreenType.Default)
      return (Control) viewportContainer;
    bool flag = true;
    BoxContainer infoContainer;
    try
    {
      infoContainer = ((Control) viewportContainer).FindControl<BoxContainer>("PubgHudInfoStack");
    }
    catch (ArgumentException ex)
    {
      flag = false;
      BoxContainer boxContainer = new BoxContainer();
      ((Control) boxContainer).Name = "PubgHudInfoStack";
      boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
      boxContainer.SeparationOverride = new int?(8);
      ((Control) boxContainer).MinSize = new Vector2(260f, 0.0f);
      ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 3;
      infoContainer = boxContainer;
      ((Control) viewportContainer).AddChild((Control) infoContainer);
    }
    if (flag)
    {
      ((Control) infoContainer).Orphan();
      ((Control) viewportContainer).AddChild((Control) infoContainer);
    }
    this._infoStack = infoContainer;
    this.EnsureInfoViewport(viewportContainer);
    return (Control) infoContainer;
  }

  private void PositionInfoStack(
    LayoutContainer viewportContainer,
    Control infoContainer,
    ScreenType screenType)
  {
    if (screenType != ScreenType.Default || !(infoContainer is BoxContainer boxContainer))
      return;
    foreach (Control child in ((Control) boxContainer).Children)
    {
      child.ForceRunStyleUpdate();
      child.Measure(Vector2Helpers.Infinity);
    }
    ((Control) boxContainer).ForceRunStyleUpdate();
    ((Control) boxContainer).Measure(Vector2Helpers.Infinity);
    Vector2 desiredSize = ((Control) boxContainer).DesiredSize;
    Vector2 viewportSize = this.GetViewportSize(viewportContainer);
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
      return;
    float x = (float) ((double) viewportSize.X - (double) desiredSize.X - 15.0);
    float y = (float) ((double) viewportSize.Y - 220.0 - 200.0 - 10.0 - (double) desiredSize.Y + 200.0 - 30.0);
    if ((double) x < 15.0)
      x = 15f;
    if ((double) y < 15.0)
      y = 15f;
    LayoutContainer.SetAnchorAndMarginPreset((Control) boxContainer, (LayoutContainer.LayoutPreset) 0, (LayoutContainer.LayoutPresetMode) 0, 0);
    LayoutContainer.SetPosition((Control) boxContainer, new Vector2(x, y));
  }

  private Vector2 GetViewportSize(LayoutContainer viewportContainer)
  {
    Vector2 viewportSize = ((Control) this.UIManager.RootControl).Size;
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
      viewportSize = ((Control) this.UIManager.StateRoot).Size;
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
      viewportSize = ((Control) viewportContainer).Size;
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      if (activeScreen != null)
      {
        MainViewport widget = activeScreen.GetWidget<MainViewport>();
        viewportSize = widget != null ? ((Control) widget).Size : Vector2.Zero;
      }
    }
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
    {
      Control parent = ((Control) viewportContainer).Parent;
      viewportSize = parent != null ? parent.Size : Vector2.Zero;
    }
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      viewportSize = activeScreen != null ? ((Control) activeScreen).Size : Vector2.Zero;
    }
    return viewportSize;
  }

  private void EnsureInfoViewport(LayoutContainer viewportContainer)
  {
    if (this._infoViewport == viewportContainer && this._infoViewportSubscribed)
      return;
    this.UnsubscribeInfoViewport();
    this._infoViewport = viewportContainer;
    ((Control) this._infoViewport).OnResized += new Action(this.OnInfoViewportResized);
    this._infoViewportSubscribed = true;
  }

  private void UnsubscribeInfoViewport()
  {
    if (this._infoViewport != null && this._infoViewportSubscribed)
      ((Control) this._infoViewport).OnResized -= new Action(this.OnInfoViewportResized);
    this._infoViewport = (LayoutContainer) null;
    this._infoViewportSubscribed = false;
  }

  private void OnInfoViewportResized()
  {
    if (this._infoViewport == null || this._infoStack == null)
      return;
    this.PositionInfoStack(this._infoViewport, (Control) this._infoStack, this.GetScreenType());
  }

  private void ApplyPanelLayout(
    PanelContainer panel,
    float topOffset,
    float defaultBottomOffset,
    int margin)
  {
    if (this.GetScreenType() == ScreenType.Default)
    {
      LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 3, (LayoutContainer.LayoutPresetMode) 0, margin);
      LayoutContainer.SetMarginBottom((Control) panel, defaultBottomOffset);
    }
    else
    {
      LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 1, (LayoutContainer.LayoutPresetMode) 0, margin);
      LayoutContainer.SetMarginTop((Control) panel, topOffset);
    }
  }

  private void OnScreenUnload()
  {
    this.ClearUI();
    this.UnsubscribeInfoViewport();
    ((Control) this._infoStack)?.Orphan();
    this._infoStack = (BoxContainer) null;
  }

  private void ClearUI()
  {
    ((Control) this._readyPanel)?.Orphan();
    this._readyPanel = (PanelContainer) null;
    this._readyLabel = (Label) null;
    ((Control) this._totalPanel)?.Orphan();
    this._totalPanel = (PanelContainer) null;
    this._totalLabel = (Label) null;
    ((Control) this._timePanel)?.Orphan();
    this._timePanel = (PanelContainer) null;
    this._timeLabel = (Label) null;
  }
}
