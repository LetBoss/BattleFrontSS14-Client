// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.UserInterface.Systems.Hud.CivHudUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Capture;
using Content.Client.Gameplay;
using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Chat;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivHudUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IGameTiming _timing;
  private bool _systemSubscribed;
  private bool _enabled;
  private int _viewerTeamId;
  private int _team1AliveCount;
  private int _team2AliveCount;
  private int _team1Score;
  private int _team2Score;
  private CivHudPhase _phase;
  private float _phaseTimeLeftSeconds;
  private float _waveRespawnSecondsLeft;
  private bool _waveConfirmActive;
  private string _modeName = string.Empty;
  private string _objectiveText = string.Empty;
  private string _guidanceText = string.Empty;
  private bool _isSquadLeader;
  private bool _briefingWindowDismissed;
  private readonly List<CivPointCapturePointState> _pointStates = new List<CivPointCapturePointState>();
  private PanelContainer? _scorePanel;
  private PanelContainer? _timerPanel;
  private RichTextLabel? _scoreLabel;
  private Label? _timerLabel;
  private CivHudUIController.CivBriefingWindow? _briefingWindow;
  private string? _lastScoreMarkup;

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureSystemSubscribed();
    this.UpdateHudVisibility();
  }

  public void OnStateExited(GameplayState state)
  {
    this.UnsubscribeFromSystem();
    this._enabled = false;
    this.ClearPanels();
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this._enabled && (double) this._phaseTimeLeftSeconds > 0.0)
      this._phaseTimeLeftSeconds = MathF.Max(0.0f, this._phaseTimeLeftSeconds - ((FrameEventArgs) ref args).DeltaSeconds);
    if (this._enabled && (double) this._waveRespawnSecondsLeft > 0.0)
      this._waveRespawnSecondsLeft = MathF.Max(0.0f, this._waveRespawnSecondsLeft - ((FrameEventArgs) ref args).DeltaSeconds);
    this.UpdateHudVisibility();
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    CivHudEventsSystem civHudEventsSystem = this.EntityManager.System<CivHudEventsSystem>();
    civHudEventsSystem.OnStatusReceived += new Action<CivHudStatusEvent>(this.OnStatus);
    this._systemSubscribed = true;
    if (civHudEventsSystem.LastStatus == null)
      return;
    this.OnStatus(civHudEventsSystem.LastStatus);
  }

  private void UnsubscribeFromSystem()
  {
    if (!this._systemSubscribed)
      return;
    CivHudEventsSystem civHudEventsSystem = this.EntityManager.SystemOrNull<CivHudEventsSystem>();
    if (civHudEventsSystem != null)
      civHudEventsSystem.OnStatusReceived -= new Action<CivHudStatusEvent>(this.OnStatus);
    this._systemSubscribed = false;
  }

  private void OnStatus(CivHudStatusEvent msg)
  {
    if (this._phase != CivHudPhase.Briefing && msg.Phase == CivHudPhase.Briefing)
      this._briefingWindowDismissed = false;
    if (this._phase == CivHudPhase.Briefing && msg.Phase != CivHudPhase.Briefing)
      this.UIManager.GetUIController<ChatUIController>().ReplaceSelectedChannel(ChatSelectChannel.Radio, ChatSelectChannel.Local);
    this._enabled = msg.Enabled;
    this._viewerTeamId = msg.ViewerTeamId;
    this._phase = msg.Phase;
    this._phaseTimeLeftSeconds = msg.PhaseTimeLeftSeconds;
    this._team1AliveCount = msg.Team1AliveCount;
    this._team2AliveCount = msg.Team2AliveCount;
    this._team1Score = msg.Team1Score;
    this._team2Score = msg.Team2Score;
    this._waveRespawnSecondsLeft = msg.WaveRespawnSecondsLeft;
    this._waveConfirmActive = msg.WaveConfirmActive;
    this._modeName = msg.ModeName;
    this._objectiveText = msg.ObjectiveText;
    this._guidanceText = msg.GuidanceText;
    this._isSquadLeader = msg.IsSquadLeader;
    this._pointStates.Clear();
    this._pointStates.AddRange((IEnumerable<CivPointCapturePointState>) msg.PointStates);
    this.UpdateHudVisibility();
  }

  private void UpdateHudVisibility()
  {
    if (!this.ShouldShowHud())
    {
      this.ClearPanels();
    }
    else
    {
      if (!this.EnsurePanels())
        return;
      this.UpdateLabels();
      this.UpdateBriefingWindow();
    }
  }

  private bool ShouldShowHud()
  {
    if (!this._enabled)
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return false;
    if (this.EntityManager.HasComponent<CivTeamMemberComponent>(localEntity.Value))
      return true;
    return this._viewerTeamId > 0 && this.EntityManager.HasComponent<GhostComponent>(localEntity.Value);
  }

  private bool EnsurePanels()
  {
    LayoutContainer viewportContainer;
    if (!this.TryGetViewportContainer(out viewportContainer))
    {
      this.ClearPanels();
      return false;
    }
    if (this._timerPanel == null || ((Control) this._timerPanel).Disposed)
    {
      (this._timerPanel, this._timerLabel) = CivHudUIController.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#6c757d", new Color?()), Loc.GetString("civ-ui-hud-timer-default"));
      ((Control) viewportContainer).AddChild((Control) this._timerPanel);
    }
    else if (((Control) this._timerPanel).Parent != viewportContainer)
    {
      ((Control) this._timerPanel).Orphan();
      ((Control) viewportContainer).AddChild((Control) this._timerPanel);
    }
    if (this._scorePanel == null || ((Control) this._scorePanel).Disposed)
    {
      (this._scorePanel, this._scoreLabel) = CivHudUIController.CreateStyledRichPanel(Color.FromHex((ReadOnlySpan<char>) "#4b6378", new Color?()));
      ((Control) viewportContainer).AddChild((Control) this._scorePanel);
    }
    else if (((Control) this._scorePanel).Parent != viewportContainer)
    {
      ((Control) this._scorePanel).Orphan();
      ((Control) viewportContainer).AddChild((Control) this._scorePanel);
    }
    this.ApplyPanelLayout(this._timerPanel, 130f, -540f, 25);
    CivHudUIController.ApplyScorePanelLayout(this._scorePanel);
    return true;
  }

  private bool TryGetViewportContainer(out LayoutContainer viewportContainer)
  {
    viewportContainer = (LayoutContainer) null;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen != null)
    {
      if (activeScreen.GetWidget<MainViewport>() != null)
      {
        try
        {
          viewportContainer = ((Control) activeScreen).FindControl<LayoutContainer>("ViewportContainer");
          return true;
        }
        catch (ArgumentException ex)
        {
          return false;
        }
      }
    }
    return false;
  }

  private void UpdateBriefingWindow()
  {
    if (this._phase != CivHudPhase.Briefing || this._briefingWindowDismissed)
    {
      this.CloseBriefingWindow();
    }
    else
    {
      if (this._briefingWindow == null || ((Control) this._briefingWindow).Disposed || this._briefingWindow.IsSquadLeaderTemplate != this._isSquadLeader)
      {
        this.CloseBriefingWindow();
        this._briefingWindow = new CivHudUIController.CivBriefingWindow(this._isSquadLeader);
        ((BaseWindow) this._briefingWindow).OnClose += (Action) (() =>
        {
          this._briefingWindowDismissed = true;
          this._briefingWindow = (CivHudUIController.CivBriefingWindow) null;
        });
        ((BaseWindow) this._briefingWindow).OpenCentered();
      }
      this._briefingWindow.UpdateContent(string.IsNullOrWhiteSpace(this._modeName) ? "TDM" : this._modeName, this._objectiveText, this._guidanceText, this._isSquadLeader);
    }
  }

  private void UpdateLabels()
  {
    if (this._scoreLabel != null)
    {
      string markup = this.BuildScoreMarkup();
      if (markup != this._lastScoreMarkup)
      {
        this._lastScoreMarkup = markup;
        this._scoreLabel.SetMarkup(markup);
      }
    }
    if (this._timerLabel == null)
      return;
    string str1;
    switch (this._phase)
    {
      case CivHudPhase.Briefing:
        str1 = Loc.GetString("civ-ui-hud-timer-briefing", new (string, object)[1]
        {
          ("time", (object) CivHudUIController.FormatTime(this._phaseTimeLeftSeconds))
        });
        break;
      case CivHudPhase.InRound:
        str1 = Loc.GetString("civ-ui-hud-timer-inround", new (string, object)[1]
        {
          ("time", (object) CivHudUIController.FormatTime(this._phaseTimeLeftSeconds))
        });
        break;
      default:
        str1 = Loc.GetString("civ-ui-hud-timer-waiting");
        break;
    }
    string str2 = str1;
    if (this._phase == CivHudPhase.InRound && (double) this._waveRespawnSecondsLeft > 0.0 && (this.IsLocalGhostViewer() || this.IsLocalCommanderViewer()))
    {
      string str3 = this._waveConfirmActive ? "civ-ui-hud-wave-confirm" : "civ-ui-hud-wave-timer";
      str2 = $"{str2}\n{Loc.GetString(str3, new (string, object)[1]
      {
        ("time", (object) CivHudUIController.FormatTime(this._waveRespawnSecondsLeft))
      })}";
    }
    if (!(str2 != this._timerLabel.Text))
      return;
    this._timerLabel.Text = str2;
  }

  private bool IsLocalGhostViewer()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    return localEntity.HasValue && this.EntityManager.HasComponent<GhostComponent>(localEntity.Value);
  }

  private bool IsLocalCommanderViewer()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    return localEntity.HasValue && this.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent) && teamMemberComponent.IsCommander;
  }

  private string BuildScoreMarkup()
  {
    int viewerTeamId = this.GetViewerTeamId();
    int teamId1 = viewerTeamId == 2 ? 2 : 1;
    int teamId2 = teamId1 == 2 ? 1 : 2;
    int num1 = teamId1 == 2 ? this._team2Score : this._team1Score;
    int num2 = teamId2 == 2 ? this._team2Score : this._team1Score;
    int num3 = teamId1 == 2 ? this._team2AliveCount : this._team1AliveCount;
    int num4 = teamId2 == 2 ? this._team2AliveCount : this._team1AliveCount;
    string hex1 = ((Color) ref CivPointCaptureColorResolver.FriendlyColor).ToHex();
    string hex2 = ((Color) ref CivPointCaptureColorResolver.EnemyColor).ToHex();
    string hex3 = ((Color) ref CivPointCaptureColorResolver.NeutralColor).ToHex();
    string str1 = $"[bold][color={hex1}]{CivHudUIController.GetTeamShortName(teamId1)} {num3}[/color][/bold]";
    string str2 = $"[bold][color={hex2}]{CivHudUIController.GetTeamShortName(teamId2)} {num4}[/color][/bold]";
    string str3 = $"[bold][color={hex1}]{num1}[/color][/bold]";
    string str4 = $"[bold][color={hex2}]{num2}[/color][/bold]";
    if (this._pointStates.Count == 0)
      return $"[font size=13]{str1}   [color={hex3}]|[/color]   {str3}   [color={hex3}]|[/color]   {str4}   [color={hex3}]|[/color]   {str2}[/font]";
    string empty = string.Empty;
    foreach (CivPointCapturePointState pointState in this._pointStates)
    {
      string str5 = string.IsNullOrWhiteSpace(pointState.Label) ? "P" : pointState.Label;
      float timeSeconds = MathF.Floor((float) this._timing.CurTime.TotalSeconds * 10f) / 10f;
      Color color = pointState.CapturingTeamId == 0 || (double) pointState.CaptureProgress <= 0.0 ? CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, pointState.OwnerTeamId) : CivPointCaptureColorResolver.GetCapturePulseColor(viewerTeamId, pointState.OwnerTeamId, pointState.CapturingTeamId, timeSeconds);
      string hex4 = ((Color) ref color).ToHex();
      empty += $"[bold][color={hex4}]{str5}[/color][/bold] ";
    }
    string str6 = empty.TrimEnd();
    return $"[font size=13]{str1}   [color={hex3}]|[/color]   {str3}   [color={hex3}]|[/color]   {str6}   [color={hex3}]|[/color]   {str4}   [color={hex3}]|[/color]   {str2}[/font]";
  }

  private int GetViewerTeamId()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    return localEntity.HasValue && this.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent) ? teamMemberComponent.TeamId : this._viewerTeamId;
  }

  private static string GetTeamShortName(int teamId)
  {
    return Loc.GetString(teamId == 2 ? "civ-team-short-rf" : "civ-team-short-usa");
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

  private static (PanelContainer panel, Label label) CreateStyledPanel(
    Color accentColor,
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
    ((Control) panelContainer).MinWidth = 190f;
    Label label = new Label()
    {
      Text = text,
      FontColorOverride = new Color?(Color.White)
    };
    ((Control) panelContainer).AddChild((Control) label);
    return (panelContainer, label);
  }

  private static (PanelContainer panel, RichTextLabel label) CreateStyledRichPanel(Color accentColor)
  {
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#13181fE8", new Color?()),
      BorderColor = accentColor,
      BorderThickness = new Thickness(0.0f, 0.0f, 0.0f, 2f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer).MinWidth = 340f;
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).HorizontalExpand = true;
    ((Control) richTextLabel1).HorizontalAlignment = (Control.HAlignment) 2;
    RichTextLabel richTextLabel2 = richTextLabel1;
    ((Control) panelContainer).AddChild((Control) richTextLabel2);
    return (panelContainer, richTextLabel2);
  }

  private ScreenType GetScreenType()
  {
    ScreenType result;
    return !Enum.TryParse<ScreenType>(this._cfg.GetCVar<string>(CCVars.UILayout), out result) ? ScreenType.Default : result;
  }

  private void ApplyPanelLayout(
    PanelContainer panel,
    float topOffset,
    float defaultBottomOffset,
    int margin)
  {
    switch (this.GetScreenType())
    {
      case ScreenType.Default:
        LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 3, (LayoutContainer.LayoutPresetMode) 0, margin);
        LayoutContainer.SetMarginBottom((Control) panel, defaultBottomOffset);
        break;
      case ScreenType.Battlefront:
        LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 1, (LayoutContainer.LayoutPresetMode) 0, 6);
        LayoutContainer.SetMarginTop((Control) panel, 6f);
        break;
      default:
        LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 1, (LayoutContainer.LayoutPresetMode) 0, margin);
        LayoutContainer.SetMarginTop((Control) panel, topOffset);
        break;
    }
  }

  private static void ApplyScorePanelLayout(PanelContainer panel)
  {
    LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 10, (LayoutContainer.LayoutPresetMode) 0, 0);
    LayoutContainer.SetMarginTop((Control) panel, 12f);
    ((Control) panel).HorizontalAlignment = (Control.HAlignment) 2;
  }

  private void CloseBriefingWindow()
  {
    if (this._briefingWindow == null || ((Control) this._briefingWindow).Disposed)
    {
      this._briefingWindow = (CivHudUIController.CivBriefingWindow) null;
    }
    else
    {
      ((BaseWindow) this._briefingWindow).Close();
      this._briefingWindow = (CivHudUIController.CivBriefingWindow) null;
    }
  }

  private void ClearPanels()
  {
    PanelContainer scorePanel = this._scorePanel;
    if (scorePanel != null && !((Control) scorePanel).Disposed)
      ((Control) scorePanel).Orphan();
    PanelContainer timerPanel = this._timerPanel;
    if (timerPanel != null && !((Control) timerPanel).Disposed)
      ((Control) timerPanel).Orphan();
    this.CloseBriefingWindow();
    this._scorePanel = (PanelContainer) null;
    this._timerPanel = (PanelContainer) null;
    this._scoreLabel = (RichTextLabel) null;
    this._timerLabel = (Label) null;
    this._briefingWindowDismissed = false;
  }

  private sealed class CivBriefingWindow : DefaultWindow
  {
    private Label? _eyebrowLabel;
    private RichTextLabel? _modeLabel;
    private Label? _subtitleLabel;
    private Label? _objectiveLabel;
    private Label? _guidanceLabel;
    private Button? _confirmButton;

    public bool IsSquadLeaderTemplate { get; }

    public CivBriefingWindow(bool isSquadLeaderTemplate)
    {
      this.IsSquadLeaderTemplate = isSquadLeaderTemplate;
      this.Title = isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-title-sl") : Loc.GetString("civ-ui-hud-briefing-title-soldier");
      ((Control) this).MinSize = isSquadLeaderTemplate ? new Vector2(960f, 720f) : new Vector2(700f, 580f);
      ((Control) this).SetSize = isSquadLeaderTemplate ? new Vector2(1040f, 760f) : new Vector2(740f, 620f);
      ((BaseWindow) this).Resizable = false;
      Color color = isSquadLeaderTemplate ? Color.FromHex((ReadOnlySpan<char>) "#f1c550", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#57b9ff", new Color?());
      string text = isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-badge-sl") : Loc.GetString("civ-ui-hud-briefing-badge-soldier");
      PanelContainer panelContainer1 = new PanelContainer();
      panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#10161d", new Color?()),
        BorderColor = Color.FromHex((ReadOnlySpan<char>) "#3f4957", new Color?()),
        BorderThickness = new Thickness(1f)
      };
      ((Control) panelContainer1).HorizontalExpand = true;
      ((Control) panelContainer1).VerticalExpand = true;
      PanelContainer panelContainer2 = panelContainer1;
      panelContainer2.PanelOverride.SetContentMarginOverride((StyleBox.Margin) 15, 14f);
      this.Contents.AddChild((Control) panelContainer2);
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      boxContainer1.SeparationOverride = new int?(10);
      ((Control) boxContainer1).HorizontalExpand = true;
      ((Control) boxContainer1).VerticalExpand = true;
      BoxContainer boxContainer2 = boxContainer1;
      ((Control) panelContainer2).AddChild((Control) boxContainer2);
      PanelContainer panelContainer3 = new PanelContainer();
      panelContainer3.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#151b23", new Color?()),
        BorderColor = color,
        BorderThickness = new Thickness(0.0f, 0.0f, 0.0f, 4f)
      };
      ((Control) panelContainer3).HorizontalExpand = true;
      PanelContainer panelContainer4 = panelContainer3;
      panelContainer4.PanelOverride.SetContentMarginOverride((StyleBox.Margin) 15, 16f);
      ((Control) boxContainer2).AddChild((Control) panelContainer4);
      BoxContainer boxContainer3 = new BoxContainer();
      boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
      boxContainer3.SeparationOverride = new int?(6);
      ((Control) boxContainer3).HorizontalExpand = true;
      BoxContainer boxContainer4 = boxContainer3;
      ((Control) panelContainer4).AddChild((Control) boxContainer4);
      BoxContainer boxContainer5 = new BoxContainer();
      boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
      boxContainer5.SeparationOverride = new int?(8);
      ((Control) boxContainer5).HorizontalExpand = true;
      BoxContainer boxContainer6 = boxContainer5;
      ((Control) boxContainer4).AddChild((Control) boxContainer6);
      ((Control) boxContainer6).AddChild((Control) CivHudUIController.CivBriefingWindow.CreateBadge(text));
      Label label1 = new Label();
      label1.Text = Loc.GetString("civ-ui-hud-briefing-eyebrow");
      label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#d3dde8", new Color?()));
      ((Control) label1).StyleClasses.Add("LabelSecondaryColor");
      this._eyebrowLabel = label1;
      ((Control) boxContainer4).AddChild((Control) this._eyebrowLabel);
      RichTextLabel richTextLabel = new RichTextLabel();
      ((Control) richTextLabel).HorizontalExpand = true;
      ((Control) richTextLabel).HorizontalAlignment = (Control.HAlignment) 1;
      this._modeLabel = richTextLabel;
      ((Control) boxContainer4).AddChild((Control) this._modeLabel);
      Label label2 = new Label();
      label2.Text = isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-subtitle-sl") : Loc.GetString("civ-ui-hud-briefing-subtitle-soldier");
      label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#b9c4d1", new Color?()));
      ((Control) label2).HorizontalExpand = true;
      this._subtitleLabel = label2;
      ((Control) boxContainer4).AddChild((Control) this._subtitleLabel);
      PanelContainer infoCard1 = CivHudUIController.CivBriefingWindow.CreateInfoCard(Loc.GetString("civ-ui-hud-briefing-objective-title"), Color.FromHex((ReadOnlySpan<char>) "#ff6b57", new Color?()), out this._objectiveLabel);
      ((Control) boxContainer2).AddChild((Control) infoCard1);
      PanelContainer infoCard2 = CivHudUIController.CivBriefingWindow.CreateInfoCard(isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-guidance-title-sl") : Loc.GetString("civ-ui-hud-briefing-guidance-title-soldier"), Color.FromHex((ReadOnlySpan<char>) "#72df8f", new Color?()), out this._guidanceLabel);
      ((Control) boxContainer2).AddChild((Control) infoCard2);
      ((Control) boxContainer2).AddChild((Control) new HLine()
      {
        Color = new Color?(Color.FromHex((ReadOnlySpan<char>) "#344150", new Color?())),
        Thickness = new float?(1f)
      });
      BoxContainer boxContainer7 = new BoxContainer();
      boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer7).HorizontalExpand = true;
      boxContainer7.SeparationOverride = new int?(8);
      BoxContainer boxContainer8 = boxContainer7;
      ((Control) boxContainer2).AddChild((Control) boxContainer8);
      ((Control) boxContainer8).AddChild(new Control()
      {
        HorizontalExpand = true
      });
      Button button = new Button();
      button.Text = Loc.GetString("civ-ui-hud-briefing-confirm");
      ((Control) button).MinWidth = 180f;
      ((Control) button).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) button).StyleClasses.Add("ButtonBig");
      ((Control) button).StyleClasses.Add("ButtonColorGreen");
      this._confirmButton = button;
      ((BaseButton) this._confirmButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) this).Close());
      ((Control) boxContainer8).AddChild((Control) this._confirmButton);
      ((Control) boxContainer8).AddChild(new Control()
      {
        HorizontalExpand = true
      });
    }

    public void UpdateContent(
      string modeName,
      string objectiveText,
      string guidanceText,
      bool isSquadLeader)
    {
      this._modeLabel.SetMarkup($"[font size=18][bold][color=#f4d16f]{(string.IsNullOrWhiteSpace(modeName) ? "TDM" : modeName.Trim().ToUpperInvariant())}[/color][/bold][/font]");
      this._objectiveLabel.Text = string.IsNullOrWhiteSpace(objectiveText) ? Loc.GetString("civ-ui-hud-briefing-objective-fallback") : objectiveText.Trim();
      this._guidanceLabel.Text = string.IsNullOrWhiteSpace(guidanceText) ? (isSquadLeader ? Loc.GetString("civ-ui-hud-briefing-guidance-fallback-sl") : Loc.GetString("civ-ui-hud-briefing-guidance-fallback-soldier")) : guidanceText.Trim();
    }

    private static PanelContainer CreateInfoCard(
      string title,
      Color accentColor,
      out Label contentLabel)
    {
      PanelContainer infoCard = new PanelContainer();
      infoCard.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#19212a", new Color?()),
        BorderColor = accentColor,
        BorderThickness = new Thickness(4f, 0.0f, 0.0f, 0.0f)
      };
      ((Control) infoCard).HorizontalExpand = true;
      ((Control) infoCard).VerticalExpand = true;
      infoCard.PanelOverride.SetContentMarginOverride((StyleBox.Margin) 15, 14f);
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      boxContainer1.SeparationOverride = new int?(6);
      ((Control) boxContainer1).HorizontalExpand = true;
      ((Control) boxContainer1).VerticalExpand = true;
      BoxContainer boxContainer2 = boxContainer1;
      ((Control) infoCard).AddChild((Control) boxContainer2);
      Label label1 = new Label();
      label1.Text = title;
      label1.FontColorOverride = new Color?(accentColor);
      ((Control) label1).StyleClasses.Add("LabelSecondaryColor");
      Label label2 = label1;
      ((Control) boxContainer2).AddChild((Control) label2);
      ref Label local = ref contentLabel;
      Label label3 = new Label();
      label3.FontColorOverride = new Color?(Color.White);
      ((Control) label3).HorizontalExpand = true;
      ((Control) label3).VerticalExpand = true;
      ((Control) label3).StyleClasses.Add("LabelBig");
      local = label3;
      ((Control) boxContainer2).AddChild((Control) contentLabel);
      return infoCard;
    }

    private static PanelContainer CreateBadge(string text)
    {
      PanelContainer badge = new PanelContainer();
      badge.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#40351b", new Color?()),
        BorderColor = Color.FromHex((ReadOnlySpan<char>) "#caa84f", new Color?()),
        BorderThickness = new Thickness(1f)
      };
      badge.PanelOverride.SetContentMarginOverride((StyleBox.Margin) 15, 6f);
      Label label = new Label();
      label.Text = text;
      label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#f5d476", new Color?()));
      ((Control) label).StyleClasses.Add("LabelSmall");
      ((Control) badge).AddChild((Control) label);
      return badge;
    }
  }
}
