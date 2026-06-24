using System;
using System.Numerics;
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

	public override void Initialize()
	{
		((UIController)this).Initialize();
		((UIController)this).SubscribeNetworkEvent<LobbyStatusEvent>((EntitySessionEventHandler<LobbyStatusEvent>)OnLobbyStatus, (Type[])null, (Type[])null);
		((UIController)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntitySessionEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, (Action)delegate
		{
			if (_inLobby)
			{
				EnsureUI();
			}
		});
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
	}

	public void OnStateEntered(GameplayState state)
	{
		if (_inLobby)
		{
			EnsureUI();
		}
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		if (_inLobby && _readyPanel == null)
		{
			EnsureUI();
		}
		if (_infoViewport != null && _infoStack != null)
		{
			PositionInfoStack(_infoViewport, (Control)(object)_infoStack, GetScreenType());
		}
	}

	private void EnsureUI()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen == null || activeScreen.GetWidget<MainViewport>() == null)
		{
			return;
		}
		LayoutContainer viewportContainer;
		try
		{
			viewportContainer = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
		}
		catch (ArgumentException)
		{
			return;
		}
		ScreenType screenType = GetScreenType();
		Control infoContainer = GetInfoContainer(viewportContainer, screenType);
		if (_readyPanel == null)
		{
			(_readyPanel, _readyLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#28a745", (Color?)null), "✓", "READY: 0/0");
			infoContainer.AddChild((Control)(object)_readyPanel);
			if (screenType != ScreenType.Default)
			{
				ApplyPanelLayout(_readyPanel, 60f, -520f, 15);
			}
		}
		if (_totalPanel == null)
		{
			(_totalPanel, _totalLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#17a2b8", (Color?)null), "◉", "TOTAL: 0");
			infoContainer.AddChild((Control)(object)_totalPanel);
			if (screenType != ScreenType.Default)
			{
				ApplyPanelLayout(_totalPanel, 105f, -475f, 15);
			}
		}
		if (_timePanel == null)
		{
			(_timePanel, _timeLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#ffc107", (Color?)null), "◷", "TIME: 00:00");
			infoContainer.AddChild((Control)(object)_timePanel);
			if (screenType != ScreenType.Default)
			{
				ApplyPanelLayout(_timePanel, 150f, -430f, 15);
			}
		}
		PositionInfoStack(viewportContainer, infoContainer, screenType);
		ApplyStatusText();
	}

	private void ApplyStatusText()
	{
		if (_readyLabel != null)
		{
			_readyLabel.Text = $"READY: {_lastReady}/{_lastTotal}";
		}
		if (_totalLabel != null)
		{
			_totalLabel.Text = $"TOTAL: {_lastTotal}";
		}
		if (_timeLabel != null)
		{
			int value = _lastTimeRemaining / 60;
			int value2 = _lastTimeRemaining % 60;
			_timeLabel.Text = $"TIME: {value:D2}:{value2:D2}";
		}
	}

	private (PanelContainer panel, Label label) CreateStyledPanel(Color accentColor, string icon, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		PanelContainer val = new PanelContainer();
		StyleBoxFlat val2 = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a1aDD", (Color?)null),
			BorderColor = accentColor,
			BorderThickness = new Thickness(0f, 0f, 3f, 0f)
		};
		((StyleBox)val2).SetContentMarginOverride((Margin)15, 8f);
		val.PanelOverride = (StyleBox)(object)val2;
		((Control)val).MinWidth = 150f;
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		Label val4 = new Label
		{
			Text = icon,
			FontColorOverride = accentColor,
			MinWidth = 20f
		};
		Label val5 = new Label
		{
			Text = text,
			FontColorOverride = Color.White
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val).AddChild((Control)(object)val3);
		return (panel: val, label: val5);
	}

	private void OnLobbyStatus(LobbyStatusEvent msg, EntitySessionEventArgs args)
	{
		_inLobby = msg.InLobby;
		if (_inLobby)
		{
			_lastReady = msg.ReadyPlayers;
			_lastTotal = msg.TotalPlayers;
			if (msg.TimeRemaining > 0)
			{
				_lastTimeRemaining = msg.TimeRemaining;
			}
			EnsureUI();
			ApplyStatusText();
		}
		else
		{
			ClearUI();
		}
	}

	private void OnRoundRestart(RoundRestartCleanupEvent ev, EntitySessionEventArgs args)
	{
		_inLobby = false;
		_lastReady = 0;
		_lastTotal = 0;
		_lastTimeRemaining = 0;
		ClearUI();
	}

	private ScreenType GetScreenType()
	{
		if (!Enum.TryParse<ScreenType>(_cfg.GetCVar<string>(CCVars.UILayout), out var result))
		{
			return ScreenType.Default;
		}
		return result;
	}

	private Control GetInfoContainer(LayoutContainer viewportContainer, ScreenType screenType)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (screenType != ScreenType.Default)
		{
			return (Control)(object)viewportContainer;
		}
		bool flag = true;
		BoxContainer val;
		try
		{
			val = ((Control)viewportContainer).FindControl<BoxContainer>("PubgHudInfoStack");
		}
		catch (ArgumentException)
		{
			flag = false;
			val = new BoxContainer
			{
				Name = "PubgHudInfoStack",
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 8,
				MinSize = new Vector2(260f, 0f),
				HorizontalAlignment = (HAlignment)3
			};
			((Control)viewportContainer).AddChild((Control)(object)val);
		}
		if (flag)
		{
			((Control)val).Orphan();
			((Control)viewportContainer).AddChild((Control)(object)val);
		}
		_infoStack = val;
		EnsureInfoViewport(viewportContainer);
		return (Control)(object)val;
	}

	private unsafe void PositionInfoStack(LayoutContainer viewportContainer, Control infoContainer, ScreenType screenType)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (screenType != ScreenType.Default)
		{
			return;
		}
		BoxContainer val = (BoxContainer)(object)((infoContainer is BoxContainer) ? infoContainer : null);
		if (val == null)
		{
			return;
		}
		Enumerator enumerator = ((Control)val).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				current.ForceRunStyleUpdate();
				current.Measure(Vector2Helpers.Infinity);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		((Control)val).ForceRunStyleUpdate();
		((Control)val).Measure(Vector2Helpers.Infinity);
		Vector2 desiredSize = ((Control)val).DesiredSize;
		Vector2 viewportSize = GetViewportSize(viewportContainer);
		if (!(viewportSize.X <= 0f) && !(viewportSize.Y <= 0f))
		{
			float num = viewportSize.X - desiredSize.X - 15f;
			float num2 = viewportSize.Y - 220f - 200f - 10f - desiredSize.Y + 200f - 30f;
			if (num < 15f)
			{
				num = 15f;
			}
			if (num2 < 15f)
			{
				num2 = 15f;
			}
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)val, (LayoutPreset)0, (LayoutPresetMode)0, 0);
			LayoutContainer.SetPosition((Control)(object)val, new Vector2(num, num2));
		}
	}

	private Vector2 GetViewportSize(LayoutContainer viewportContainer)
	{
		Vector2 result = ((Control)base.UIManager.RootControl).Size;
		if (result.X <= 0f || result.Y <= 0f)
		{
			result = ((Control)base.UIManager.StateRoot).Size;
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			result = ((Control)viewportContainer).Size;
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			UIScreen activeScreen = base.UIManager.ActiveScreen;
			if (activeScreen != null)
			{
				MainViewport widget = activeScreen.GetWidget<MainViewport>();
				result = ((widget != null) ? ((Control)widget).Size : Vector2.Zero);
			}
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			Control parent = ((Control)viewportContainer).Parent;
			result = ((parent != null) ? parent.Size : Vector2.Zero);
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			UIScreen activeScreen2 = base.UIManager.ActiveScreen;
			result = ((activeScreen2 != null) ? ((Control)activeScreen2).Size : Vector2.Zero);
		}
		return result;
	}

	private void EnsureInfoViewport(LayoutContainer viewportContainer)
	{
		if (_infoViewport != viewportContainer || !_infoViewportSubscribed)
		{
			UnsubscribeInfoViewport();
			_infoViewport = viewportContainer;
			((Control)_infoViewport).OnResized += OnInfoViewportResized;
			_infoViewportSubscribed = true;
		}
	}

	private void UnsubscribeInfoViewport()
	{
		if (_infoViewport != null && _infoViewportSubscribed)
		{
			((Control)_infoViewport).OnResized -= OnInfoViewportResized;
		}
		_infoViewport = null;
		_infoViewportSubscribed = false;
	}

	private void OnInfoViewportResized()
	{
		if (_infoViewport != null && _infoStack != null)
		{
			PositionInfoStack(_infoViewport, (Control)(object)_infoStack, GetScreenType());
		}
	}

	private void ApplyPanelLayout(PanelContainer panel, float topOffset, float defaultBottomOffset, int margin)
	{
		if (GetScreenType() == ScreenType.Default)
		{
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)3, (LayoutPresetMode)0, margin);
			LayoutContainer.SetMarginBottom((Control)(object)panel, defaultBottomOffset);
		}
		else
		{
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)1, (LayoutPresetMode)0, margin);
			LayoutContainer.SetMarginTop((Control)(object)panel, topOffset);
		}
	}

	private void OnScreenUnload()
	{
		ClearUI();
		UnsubscribeInfoViewport();
		BoxContainer? infoStack = _infoStack;
		if (infoStack != null)
		{
			((Control)infoStack).Orphan();
		}
		_infoStack = null;
	}

	private void ClearUI()
	{
		PanelContainer? readyPanel = _readyPanel;
		if (readyPanel != null)
		{
			((Control)readyPanel).Orphan();
		}
		_readyPanel = null;
		_readyLabel = null;
		PanelContainer? totalPanel = _totalPanel;
		if (totalPanel != null)
		{
			((Control)totalPanel).Orphan();
		}
		_totalPanel = null;
		_totalLabel = null;
		PanelContainer? timePanel = _timePanel;
		if (timePanel != null)
		{
			((Control)timePanel).Orphan();
		}
		_timePanel = null;
		_timeLabel = null;
	}
}
