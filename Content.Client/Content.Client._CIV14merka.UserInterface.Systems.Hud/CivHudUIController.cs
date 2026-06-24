using System;
using System.Collections.Generic;
using System.Numerics;
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

namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivHudUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
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
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Expected O, but got Unknown
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Expected O, but got Unknown
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Expected O, but got Unknown
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Expected O, but got Unknown
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Expected O, but got Unknown
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Expected O, but got Unknown
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Expected O, but got Unknown
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Expected O, but got Unknown
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Expected O, but got Unknown
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Expected O, but got Unknown
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Expected O, but got Unknown
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Expected O, but got Unknown
			IsSquadLeaderTemplate = isSquadLeaderTemplate;
			((DefaultWindow)this).Title = (isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-title-sl") : Loc.GetString("civ-ui-hud-briefing-title-soldier"));
			((Control)this).MinSize = (isSquadLeaderTemplate ? new Vector2(960f, 720f) : new Vector2(700f, 580f));
			((Control)this).SetSize = (isSquadLeaderTemplate ? new Vector2(1040f, 760f) : new Vector2(740f, 620f));
			((BaseWindow)this).Resizable = false;
			Color borderColor = (isSquadLeaderTemplate ? Color.FromHex((ReadOnlySpan<char>)"#f1c550", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#57b9ff", (Color?)null));
			string text = (isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-badge-sl") : Loc.GetString("civ-ui-hud-briefing-badge-soldier"));
			PanelContainer val = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#10161d", (Color?)null),
					BorderColor = Color.FromHex((ReadOnlySpan<char>)"#3f4957", (Color?)null),
					BorderThickness = new Thickness(1f)
				},
				HorizontalExpand = true,
				VerticalExpand = true
			};
			val.PanelOverride.SetContentMarginOverride((Margin)15, 14f);
			((DefaultWindow)this).Contents.AddChild((Control)(object)val);
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 10,
				HorizontalExpand = true,
				VerticalExpand = true
			};
			((Control)val).AddChild((Control)(object)val2);
			PanelContainer val3 = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#151b23", (Color?)null),
					BorderColor = borderColor,
					BorderThickness = new Thickness(0f, 0f, 0f, 4f)
				},
				HorizontalExpand = true
			};
			val3.PanelOverride.SetContentMarginOverride((Margin)15, 16f);
			((Control)val2).AddChild((Control)(object)val3);
			BoxContainer val4 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 6,
				HorizontalExpand = true
			};
			((Control)val3).AddChild((Control)(object)val4);
			BoxContainer val5 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 8,
				HorizontalExpand = true
			};
			((Control)val4).AddChild((Control)(object)val5);
			((Control)val5).AddChild((Control)(object)CreateBadge(text));
			_eyebrowLabel = new Label
			{
				Text = Loc.GetString("civ-ui-hud-briefing-eyebrow"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#d3dde8", (Color?)null),
				StyleClasses = { "LabelSecondaryColor" }
			};
			((Control)val4).AddChild((Control)(object)_eyebrowLabel);
			_modeLabel = new RichTextLabel
			{
				HorizontalExpand = true,
				HorizontalAlignment = (HAlignment)1
			};
			((Control)val4).AddChild((Control)(object)_modeLabel);
			_subtitleLabel = new Label
			{
				Text = (isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-subtitle-sl") : Loc.GetString("civ-ui-hud-briefing-subtitle-soldier")),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#b9c4d1", (Color?)null),
				HorizontalExpand = true
			};
			((Control)val4).AddChild((Control)(object)_subtitleLabel);
			PanelContainer val6 = CreateInfoCard(Loc.GetString("civ-ui-hud-briefing-objective-title"), Color.FromHex((ReadOnlySpan<char>)"#ff6b57", (Color?)null), out _objectiveLabel);
			((Control)val2).AddChild((Control)(object)val6);
			PanelContainer val7 = CreateInfoCard(isSquadLeaderTemplate ? Loc.GetString("civ-ui-hud-briefing-guidance-title-sl") : Loc.GetString("civ-ui-hud-briefing-guidance-title-soldier"), Color.FromHex((ReadOnlySpan<char>)"#72df8f", (Color?)null), out _guidanceLabel);
			((Control)val2).AddChild((Control)(object)val7);
			((Control)val2).AddChild((Control)(object)new HLine
			{
				Color = Color.FromHex((ReadOnlySpan<char>)"#344150", (Color?)null),
				Thickness = 1f
			});
			BoxContainer val8 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalExpand = true,
				SeparationOverride = 8
			};
			((Control)val2).AddChild((Control)(object)val8);
			((Control)val8).AddChild(new Control
			{
				HorizontalExpand = true
			});
			_confirmButton = new Button
			{
				Text = Loc.GetString("civ-ui-hud-briefing-confirm"),
				MinWidth = 180f,
				HorizontalAlignment = (HAlignment)2,
				StyleClasses = { "ButtonBig" },
				StyleClasses = { "ButtonColorGreen" }
			};
			((BaseButton)_confirmButton).OnPressed += delegate
			{
				((BaseWindow)this).Close();
			};
			((Control)val8).AddChild((Control)(object)_confirmButton);
			((Control)val8).AddChild(new Control
			{
				HorizontalExpand = true
			});
		}

		public void UpdateContent(string modeName, string objectiveText, string guidanceText, bool isSquadLeader)
		{
			string text = (string.IsNullOrWhiteSpace(modeName) ? "TDM" : modeName.Trim().ToUpperInvariant());
			_modeLabel.SetMarkup("[font size=18][bold][color=#f4d16f]" + text + "[/color][/bold][/font]");
			_objectiveLabel.Text = (string.IsNullOrWhiteSpace(objectiveText) ? Loc.GetString("civ-ui-hud-briefing-objective-fallback") : objectiveText.Trim());
			_guidanceLabel.Text = ((!string.IsNullOrWhiteSpace(guidanceText)) ? guidanceText.Trim() : (isSquadLeader ? Loc.GetString("civ-ui-hud-briefing-guidance-fallback-sl") : Loc.GetString("civ-ui-hud-briefing-guidance-fallback-soldier")));
		}

		private static PanelContainer CreateInfoCard(string title, Color accentColor, out Label contentLabel)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			//IL_0110: Expected O, but got Unknown
			PanelContainer val = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#19212a", (Color?)null),
					BorderColor = accentColor,
					BorderThickness = new Thickness(4f, 0f, 0f, 0f)
				},
				HorizontalExpand = true,
				VerticalExpand = true
			};
			val.PanelOverride.SetContentMarginOverride((Margin)15, 14f);
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 6,
				HorizontalExpand = true,
				VerticalExpand = true
			};
			((Control)val).AddChild((Control)(object)val2);
			Label val3 = new Label
			{
				Text = title,
				FontColorOverride = accentColor,
				StyleClasses = { "LabelSecondaryColor" }
			};
			((Control)val2).AddChild((Control)(object)val3);
			contentLabel = new Label
			{
				FontColorOverride = Color.White,
				HorizontalExpand = true,
				VerticalExpand = true,
				StyleClasses = { "LabelBig" }
			};
			((Control)val2).AddChild((Control)(object)contentLabel);
			return val;
		}

		private static PanelContainer CreateBadge(string text)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected O, but got Unknown
			PanelContainer val = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#40351b", (Color?)null),
					BorderColor = Color.FromHex((ReadOnlySpan<char>)"#caa84f", (Color?)null),
					BorderThickness = new Thickness(1f)
				}
			};
			val.PanelOverride.SetContentMarginOverride((Margin)15, 6f);
			Label val2 = new Label
			{
				Text = text,
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#f5d476", (Color?)null),
				StyleClasses = { "LabelSmall" }
			};
			((Control)val).AddChild((Control)(object)val2);
			return val;
		}
	}

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

	private CivBriefingWindow? _briefingWindow;

	private string? _lastScoreMarkup;

	public void OnStateEntered(GameplayState state)
	{
		EnsureSystemSubscribed();
		UpdateHudVisibility();
	}

	public void OnStateExited(GameplayState state)
	{
		UnsubscribeFromSystem();
		_enabled = false;
		ClearPanels();
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		if (_enabled && _phaseTimeLeftSeconds > 0f)
		{
			_phaseTimeLeftSeconds = MathF.Max(0f, _phaseTimeLeftSeconds - ((FrameEventArgs)(ref args)).DeltaSeconds);
		}
		if (_enabled && _waveRespawnSecondsLeft > 0f)
		{
			_waveRespawnSecondsLeft = MathF.Max(0f, _waveRespawnSecondsLeft - ((FrameEventArgs)(ref args)).DeltaSeconds);
		}
		UpdateHudVisibility();
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			CivHudEventsSystem civHudEventsSystem = base.EntityManager.System<CivHudEventsSystem>();
			civHudEventsSystem.OnStatusReceived += OnStatus;
			_systemSubscribed = true;
			if (civHudEventsSystem.LastStatus != null)
			{
				OnStatus(civHudEventsSystem.LastStatus);
			}
		}
	}

	private void UnsubscribeFromSystem()
	{
		if (_systemSubscribed)
		{
			CivHudEventsSystem civHudEventsSystem = base.EntityManager.SystemOrNull<CivHudEventsSystem>();
			if (civHudEventsSystem != null)
			{
				civHudEventsSystem.OnStatusReceived -= OnStatus;
			}
			_systemSubscribed = false;
		}
	}

	private void OnStatus(CivHudStatusEvent msg)
	{
		if (_phase != CivHudPhase.Briefing && msg.Phase == CivHudPhase.Briefing)
		{
			_briefingWindowDismissed = false;
		}
		if (_phase == CivHudPhase.Briefing && msg.Phase != CivHudPhase.Briefing)
		{
			base.UIManager.GetUIController<ChatUIController>().ReplaceSelectedChannel(ChatSelectChannel.Radio, ChatSelectChannel.Local);
		}
		_enabled = msg.Enabled;
		_viewerTeamId = msg.ViewerTeamId;
		_phase = msg.Phase;
		_phaseTimeLeftSeconds = msg.PhaseTimeLeftSeconds;
		_team1AliveCount = msg.Team1AliveCount;
		_team2AliveCount = msg.Team2AliveCount;
		_team1Score = msg.Team1Score;
		_team2Score = msg.Team2Score;
		_waveRespawnSecondsLeft = msg.WaveRespawnSecondsLeft;
		_waveConfirmActive = msg.WaveConfirmActive;
		_modeName = msg.ModeName;
		_objectiveText = msg.ObjectiveText;
		_guidanceText = msg.GuidanceText;
		_isSquadLeader = msg.IsSquadLeader;
		_pointStates.Clear();
		_pointStates.AddRange(msg.PointStates);
		UpdateHudVisibility();
	}

	private void UpdateHudVisibility()
	{
		if (!ShouldShowHud())
		{
			ClearPanels();
		}
		else if (EnsurePanels())
		{
			UpdateLabels();
			UpdateBriefingWindow();
		}
	}

	private bool ShouldShowHud()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!_enabled)
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return false;
		}
		if (base.EntityManager.HasComponent<CivTeamMemberComponent>(localEntity.Value))
		{
			return true;
		}
		if (_viewerTeamId > 0)
		{
			return base.EntityManager.HasComponent<GhostComponent>(localEntity.Value);
		}
		return false;
	}

	private bool EnsurePanels()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetViewportContainer(out LayoutContainer viewportContainer))
		{
			ClearPanels();
			return false;
		}
		if (_timerPanel == null || ((Control)_timerPanel).Disposed)
		{
			(_timerPanel, _timerLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#6c757d", (Color?)null), Loc.GetString("civ-ui-hud-timer-default"));
			((Control)viewportContainer).AddChild((Control)(object)_timerPanel);
		}
		else if ((object)((Control)_timerPanel).Parent != viewportContainer)
		{
			((Control)_timerPanel).Orphan();
			((Control)viewportContainer).AddChild((Control)(object)_timerPanel);
		}
		if (_scorePanel == null || ((Control)_scorePanel).Disposed)
		{
			(_scorePanel, _scoreLabel) = CreateStyledRichPanel(Color.FromHex((ReadOnlySpan<char>)"#4b6378", (Color?)null));
			((Control)viewportContainer).AddChild((Control)(object)_scorePanel);
		}
		else if ((object)((Control)_scorePanel).Parent != viewportContainer)
		{
			((Control)_scorePanel).Orphan();
			((Control)viewportContainer).AddChild((Control)(object)_scorePanel);
		}
		ApplyPanelLayout(_timerPanel, 130f, -540f, 25);
		ApplyScorePanelLayout(_scorePanel);
		return true;
	}

	private bool TryGetViewportContainer(out LayoutContainer viewportContainer)
	{
		viewportContainer = null;
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen == null || activeScreen.GetWidget<MainViewport>() == null)
		{
			return false;
		}
		try
		{
			viewportContainer = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
			return true;
		}
		catch (ArgumentException)
		{
			return false;
		}
	}

	private void UpdateBriefingWindow()
	{
		if (_phase != CivHudPhase.Briefing || _briefingWindowDismissed)
		{
			CloseBriefingWindow();
			return;
		}
		if (_briefingWindow == null || ((Control)_briefingWindow).Disposed || _briefingWindow.IsSquadLeaderTemplate != _isSquadLeader)
		{
			CloseBriefingWindow();
			_briefingWindow = new CivBriefingWindow(_isSquadLeader);
			((BaseWindow)_briefingWindow).OnClose += delegate
			{
				_briefingWindowDismissed = true;
				_briefingWindow = null;
			};
			((BaseWindow)_briefingWindow).OpenCentered();
		}
		_briefingWindow.UpdateContent(string.IsNullOrWhiteSpace(_modeName) ? "TDM" : _modeName, _objectiveText, _guidanceText, _isSquadLeader);
	}

	private void UpdateLabels()
	{
		if (_scoreLabel != null)
		{
			string text = BuildScoreMarkup();
			if (text != _lastScoreMarkup)
			{
				_lastScoreMarkup = text;
				_scoreLabel.SetMarkup(text);
			}
		}
		if (_timerLabel != null)
		{
			string text2 = _phase switch
			{
				CivHudPhase.Briefing => Loc.GetString("civ-ui-hud-timer-briefing", new(string, object)[1] { ("time", FormatTime(_phaseTimeLeftSeconds)) }), 
				CivHudPhase.InRound => Loc.GetString("civ-ui-hud-timer-inround", new(string, object)[1] { ("time", FormatTime(_phaseTimeLeftSeconds)) }), 
				_ => Loc.GetString("civ-ui-hud-timer-waiting"), 
			};
			if (_phase == CivHudPhase.InRound && _waveRespawnSecondsLeft > 0f && (IsLocalGhostViewer() || IsLocalCommanderViewer()))
			{
				string text3 = (_waveConfirmActive ? "civ-ui-hud-wave-confirm" : "civ-ui-hud-wave-timer");
				text2 = text2 + "\n" + Loc.GetString(text3, new(string, object)[1] { ("time", FormatTime(_waveRespawnSecondsLeft)) });
			}
			if (text2 != _timerLabel.Text)
			{
				_timerLabel.Text = text2;
			}
		}
	}

	private bool IsLocalGhostViewer()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			return base.EntityManager.HasComponent<GhostComponent>(localEntity.Value);
		}
		return false;
	}

	private bool IsLocalCommanderViewer()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (localEntity.HasValue && base.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent))
		{
			return civTeamMemberComponent.IsCommander;
		}
		return false;
	}

	private string BuildScoreMarkup()
	{
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		int viewerTeamId = GetViewerTeamId();
		int num = ((viewerTeamId != 2) ? 1 : 2);
		int num2 = ((num == 2) ? 1 : 2);
		int value = ((num == 2) ? _team2Score : _team1Score);
		int value2 = ((num2 == 2) ? _team2Score : _team1Score);
		int value3 = ((num == 2) ? _team2AliveCount : _team1AliveCount);
		int value4 = ((num2 == 2) ? _team2AliveCount : _team1AliveCount);
		string value5 = ((Color)(ref CivPointCaptureColorResolver.FriendlyColor)).ToHex();
		string value6 = ((Color)(ref CivPointCaptureColorResolver.EnemyColor)).ToHex();
		string value7 = ((Color)(ref CivPointCaptureColorResolver.NeutralColor)).ToHex();
		string value8 = $"[bold][color={value5}]{GetTeamShortName(num)} {value3}[/color][/bold]";
		string value9 = $"[bold][color={value6}]{GetTeamShortName(num2)} {value4}[/color][/bold]";
		string value10 = $"[bold][color={value5}]{value}[/color][/bold]";
		string value11 = $"[bold][color={value6}]{value2}[/color][/bold]";
		if (_pointStates.Count != 0)
		{
			string text = string.Empty;
			foreach (CivPointCapturePointState pointState in _pointStates)
			{
				string value12 = (string.IsNullOrWhiteSpace(pointState.Label) ? "P" : pointState.Label);
				float timeSeconds = MathF.Floor((float)_timing.CurTime.TotalSeconds * 10f) / 10f;
				Color val = ((pointState.CapturingTeamId != 0 && pointState.CaptureProgress > 0f) ? CivPointCaptureColorResolver.GetCapturePulseColor(viewerTeamId, pointState.OwnerTeamId, pointState.CapturingTeamId, timeSeconds) : CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, pointState.OwnerTeamId));
				string value13 = ((Color)(ref val)).ToHex();
				text += $"[bold][color={value13}]{value12}[/color][/bold] ";
			}
			text = text.TrimEnd();
			return $"[font size=13]{value8}   [color={value7}]|[/color]   {value10}   [color={value7}]|[/color]   {text}   [color={value7}]|[/color]   {value11}   [color={value7}]|[/color]   {value9}[/font]";
		}
		return $"[font size=13]{value8}   [color={value7}]|[/color]   {value10}   [color={value7}]|[/color]   {value11}   [color={value7}]|[/color]   {value9}[/font]";
	}

	private int GetViewerTeamId()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (localEntity.HasValue && base.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent))
		{
			return civTeamMemberComponent.TeamId;
		}
		return _viewerTeamId;
	}

	private static string GetTeamShortName(int teamId)
	{
		return Loc.GetString((teamId == 2) ? "civ-team-short-rf" : "civ-team-short-usa");
	}

	private static string FormatTime(float totalSeconds)
	{
		if (!float.IsFinite(totalSeconds) || totalSeconds <= 0f)
		{
			return "00:00";
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
		if (!(timeSpan.TotalHours >= 1.0))
		{
			return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		}
		return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
	}

	private static (PanelContainer panel, Label label) CreateStyledPanel(Color accentColor, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		PanelContainer val = new PanelContainer();
		StyleBoxFlat val2 = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a1aDD", (Color?)null),
			BorderColor = accentColor,
			BorderThickness = new Thickness(0f, 0f, 3f, 0f)
		};
		((StyleBox)val2).SetContentMarginOverride((Margin)15, 8f);
		val.PanelOverride = (StyleBox)(object)val2;
		((Control)val).MinWidth = 190f;
		Label val3 = new Label
		{
			Text = text,
			FontColorOverride = Color.White
		};
		((Control)val).AddChild((Control)(object)val3);
		return (panel: val, label: val3);
	}

	private static (PanelContainer panel, RichTextLabel label) CreateStyledRichPanel(Color accentColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		PanelContainer val = new PanelContainer();
		StyleBoxFlat val2 = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#13181fE8", (Color?)null),
			BorderColor = accentColor,
			BorderThickness = new Thickness(0f, 0f, 0f, 2f)
		};
		((StyleBox)val2).SetContentMarginOverride((Margin)15, 8f);
		val.PanelOverride = (StyleBox)(object)val2;
		((Control)val).MinWidth = 340f;
		RichTextLabel val3 = new RichTextLabel
		{
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val).AddChild((Control)(object)val3);
		return (panel: val, label: val3);
	}

	private ScreenType GetScreenType()
	{
		if (!Enum.TryParse<ScreenType>(_cfg.GetCVar<string>(CCVars.UILayout), out var result))
		{
			return ScreenType.Default;
		}
		return result;
	}

	private void ApplyPanelLayout(PanelContainer panel, float topOffset, float defaultBottomOffset, int margin)
	{
		switch (GetScreenType())
		{
		case ScreenType.Default:
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)3, (LayoutPresetMode)0, margin);
			LayoutContainer.SetMarginBottom((Control)(object)panel, defaultBottomOffset);
			break;
		case ScreenType.Battlefront:
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)1, (LayoutPresetMode)0, 6);
			LayoutContainer.SetMarginTop((Control)(object)panel, 6f);
			break;
		default:
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)1, (LayoutPresetMode)0, margin);
			LayoutContainer.SetMarginTop((Control)(object)panel, topOffset);
			break;
		}
	}

	private static void ApplyScorePanelLayout(PanelContainer panel)
	{
		LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)10, (LayoutPresetMode)0, 0);
		LayoutContainer.SetMarginTop((Control)(object)panel, 12f);
		((Control)panel).HorizontalAlignment = (HAlignment)2;
	}

	private void CloseBriefingWindow()
	{
		if (_briefingWindow == null || ((Control)_briefingWindow).Disposed)
		{
			_briefingWindow = null;
			return;
		}
		((BaseWindow)_briefingWindow).Close();
		_briefingWindow = null;
	}

	private void ClearPanels()
	{
		PanelContainer scorePanel = _scorePanel;
		if (scorePanel != null && !((Control)scorePanel).Disposed)
		{
			((Control)scorePanel).Orphan();
		}
		PanelContainer timerPanel = _timerPanel;
		if (timerPanel != null && !((Control)timerPanel).Disposed)
		{
			((Control)timerPanel).Orphan();
		}
		CloseBriefingWindow();
		_scorePanel = null;
		_timerPanel = null;
		_scoreLabel = null;
		_timerLabel = null;
		_briefingWindowDismissed = false;
	}
}
