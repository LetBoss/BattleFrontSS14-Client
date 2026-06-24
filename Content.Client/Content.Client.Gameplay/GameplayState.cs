using System;
using System.Numerics;
using Content.Client.Changelog;
using Content.Client.Hands;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Gameplay;

[Virtual]
public class GameplayState : GameplayStateBase, IMainViewportState
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[Dependency]
	private ChangelogManager _changelog;

	[Dependency]
	private IConfigurationManager _configurationManager;

	private FpsCounter _fpsCounter;

	private Label _version;

	private readonly GameplayStateLoadController _loadController;

	public MainViewport Viewport => _uiManager.ActiveScreen.GetWidget<MainViewport>();

	public GameplayState()
	{
		IoCManager.InjectDependencies<GameplayState>(this);
		_loadController = _uiManager.GetUIController<GameplayStateLoadController>();
	}

	protected override void Startup()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.Startup();
		LoadMainScreen();
		_configurationManager.OnValueChanged<string>(CCVars.UILayout, (Action<string>)ReloadMainScreenValueChange, false);
		_overlayManager.AddOverlay((Overlay)(object)new ShowHandItemOverlay());
		_fpsCounter = new FpsCounter(_gameTiming);
		((Control)UserInterfaceManager.PopupRoot).AddChild((Control)(object)_fpsCounter);
		((Control)_fpsCounter).Visible = _configurationManager.GetCVar<bool>(CCVars.HudFpsCounterVisible);
		_configurationManager.OnValueChanged<bool>(CCVars.HudFpsCounterVisible, (Action<bool>)delegate(bool show)
		{
			((Control)_fpsCounter).Visible = show;
		}, false);
		_version = new Label();
		_version.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFFFFF20", (Color?)null);
		_version.Text = _changelog.GetClientVersion();
		((Control)UserInterfaceManager.PopupRoot).AddChild((Control)(object)_version);
		_configurationManager.OnValueChanged<bool>(CCVars.HudVersionWatermark, (Action<bool>)delegate
		{
			((Control)_version).Visible = VersionVisible();
		}, true);
		_configurationManager.OnValueChanged<bool>(CCVars.ForceClientHudVersionWatermark, (Action<bool>)delegate
		{
			((Control)_version).Visible = VersionVisible();
		}, true);
		LayoutContainer.SetPosition((Control)(object)_version, new Vector2(70f, 0f));
	}

	private bool VersionVisible()
	{
		bool cVar = _configurationManager.GetCVar<bool>(CCVars.HudVersionWatermark);
		bool cVar2 = _configurationManager.GetCVar<bool>(CCVars.ForceClientHudVersionWatermark);
		return cVar || cVar2;
	}

	protected override void Shutdown()
	{
		_overlayManager.RemoveOverlay<ShowHandItemOverlay>();
		base.Shutdown();
		_eyeManager.MainViewport = (IViewportControl)(object)UserInterfaceManager.MainViewport;
		((Control)_fpsCounter).Orphan();
		((Control)_version).Orphan();
		_uiManager.ClearWindows();
		_configurationManager.UnsubValueChanged<string>(CCVars.UILayout, (Action<string>)ReloadMainScreenValueChange);
		UnloadMainScreen();
	}

	private void ReloadMainScreenValueChange(string _)
	{
		ReloadMainScreen();
	}

	public void ReloadMainScreen()
	{
		UIScreen activeScreen = _uiManager.ActiveScreen;
		if (((activeScreen != null) ? activeScreen.GetWidget<MainViewport>() : null) != null)
		{
			UnloadMainScreen();
			LoadMainScreen();
		}
	}

	private void UnloadMainScreen()
	{
		_loadController.UnloadScreen();
		_uiManager.UnloadScreen();
	}

	private void LoadMainScreen()
	{
		if (!Enum.TryParse<ScreenType>(_configurationManager.GetCVar<string>(CCVars.UILayout), out var result))
		{
			result = ScreenType.Default;
		}
		switch (result)
		{
		case ScreenType.Default:
			_uiManager.LoadScreen<DefaultGameScreen>();
			break;
		case ScreenType.Separated:
			_uiManager.LoadScreen<SeparatedChatGameScreen>();
			break;
		case ScreenType.Battlefront:
			_uiManager.LoadScreen<BattlefrontGameScreen>();
			break;
		}
		_loadController.LoadScreen();
	}

	protected override void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (args.Viewport == null)
		{
			base.OnKeyBindStateChanged(new ViewportBoundKeyEventArgs(args.KeyEventArgs, (Control)(object)Viewport.Viewport));
		}
		else
		{
			base.OnKeyBindStateChanged(args);
		}
	}
}
