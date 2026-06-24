using Content.Client._PUBG.UserInterface.LanguageSelect;
using Content.Client._RMC14.Explosion;
using Content.Client._RMC14.Xenonids.Screech;
using Content.Client.Administration.Managers;
using Content.Client.Changelog;
using Content.Client.Chat.Managers;
using Content.Client.DebugMon;
using Content.Client.Eui;
using Content.Client.Fullscreen;
using Content.Client.GameTicking.Managers;
using Content.Client.GhostKick;
using Content.Client.Guidebook;
using Content.Client.Input;
using Content.Client.IoC;
using Content.Client.Launcher;
using Content.Client.Lobby;
using Content.Client.MainMenu;
using Content.Client.Parallax.Managers;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.Playtime;
using Content.Client.Radiation.Overlays;
using Content.Client.Replay;
using Content.Client.Screenshot;
using Content.Client.Singularity;
using Content.Client.Stylesheets;
using Content.Client.UserInterface;
using Content.Client.Viewport;
using Content.Client.Voting;
using Content.Shared.Ame.Components;
using Content.Shared.Gravity;
using Content.Shared.Localizations;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Replays.Loading;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Entry;

public sealed class EntryPoint : GameClient
{
	[Dependency]
	private IBaseClient _baseClient;

	[Dependency]
	private IGameController _gameController;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IComponentFactory _componentFactory;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IParallaxManager _parallaxManager;

	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	private IStylesheetManager _stylesheetManager;

	[Dependency]
	private IScreenshotHook _screenshotHook;

	[Dependency]
	private FullscreenHook _fullscreenHook;

	[Dependency]
	private ChangelogManager _changelogManager;

	[Dependency]
	private ViewportManager _viewportManager;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IChatManager _chatManager;

	[Dependency]
	private IClientPreferencesManager _clientPreferencesManager;

	[Dependency]
	private EuiManager _euiManager;

	[Dependency]
	private IVoteManager _voteManager;

	[Dependency]
	private DocumentParsingManager _documentParsingManager;

	[Dependency]
	private GhostKickManager _ghostKick;

	[Dependency]
	private ExtendedDisconnectInformationManager _extendedDisconnectInformation;

	[Dependency]
	private JobRequirementsManager _jobRequirements;

	[Dependency]
	private ContentLocalizationManager _contentLoc;

	[Dependency]
	private ContentReplayPlaybackManager _playbackMan;

	[Dependency]
	private IResourceManager _resourceManager;

	[Dependency]
	private IReplayLoadManager _replayLoad;

	[Dependency]
	private ILogManager _logManager;

	[Dependency]
	private DebugMonitorManager _debugMonitorManager;

	[Dependency]
	private TitleWindowManager _titleWindowManager;

	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	[Dependency]
	private ClientsidePlaytimeTrackingManager _clientsidePlaytimeManager;

	[Dependency]
	private LanguageSelectManager _languageSelectManager;

	public override void Init()
	{
		ClientContentIoC.Register();
		foreach (ClientModuleTestingCallbacks testingCallback in ((GameShared)this).TestingCallbacks)
		{
			testingCallback.ClientBeforeIoC?.Invoke();
		}
		IoCManager.BuildGraph();
		IoCManager.InjectDependencies<EntryPoint>(this);
		_contentLoc.Initialize();
		_componentFactory.DoAutoRegistrations();
		_componentFactory.IgnoreMissingComponents("");
		_componentFactory.RegisterClass<SharedGravityGeneratorComponent>(false);
		_componentFactory.RegisterClass<SharedAmeControllerComponent>(false);
		_prototypeManager.RegisterIgnore("utilityQuery");
		_prototypeManager.RegisterIgnore("utilityCurvePreset");
		_prototypeManager.RegisterIgnore("accent");
		_prototypeManager.RegisterIgnore("gasReaction");
		_prototypeManager.RegisterIgnore("seed");
		_prototypeManager.RegisterIgnore("objective");
		_prototypeManager.RegisterIgnore("holiday");
		_prototypeManager.RegisterIgnore("htnCompound");
		_prototypeManager.RegisterIgnore("htnPrimitive");
		_prototypeManager.RegisterIgnore("gameMap");
		_prototypeManager.RegisterIgnore("gameMapPool");
		_prototypeManager.RegisterIgnore("lobbyBackground");
		_prototypeManager.RegisterIgnore("gamePreset");
		_prototypeManager.RegisterIgnore("noiseChannel");
		_prototypeManager.RegisterIgnore("playerConnectionWhitelist");
		_prototypeManager.RegisterIgnore("spaceBiome");
		_prototypeManager.RegisterIgnore("worldgenConfig");
		_prototypeManager.RegisterIgnore("gameRule");
		_prototypeManager.RegisterIgnore("worldSpell");
		_prototypeManager.RegisterIgnore("entitySpell");
		_prototypeManager.RegisterIgnore("instantSpell");
		_prototypeManager.RegisterIgnore("roundAnnouncement");
		_prototypeManager.RegisterIgnore("wireLayout");
		_prototypeManager.RegisterIgnore("alertLevels");
		_prototypeManager.RegisterIgnore("nukeopsRole");
		_prototypeManager.RegisterIgnore("ghostRoleRaffleDecider");
		_prototypeManager.RegisterIgnore("codewordGenerator");
		_prototypeManager.RegisterIgnore("codewordFaction");
		_componentFactory.GenerateNetIds();
		_adminManager.Initialize();
		_screenshotHook.Initialize();
		_fullscreenHook.Initialize();
		_changelogManager.Initialize();
		_viewportManager.Initialize();
		_ghostKick.Initialize();
		_extendedDisconnectInformation.Initialize();
		_jobRequirements.Initialize();
		_playbackMan.Initialize();
		_clientsidePlaytimeManager.Initialize();
		_configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffX", (object)1080, false);
		_configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffY", (object)720, false);
		_configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffX", (object)520, false);
		_configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffY", (object)240, false);
		_configManager.SetCVar("interface.resolutionAutoScaleMinimum", (object)0.5f, false);
	}

	public override void Shutdown()
	{
		((GameShared)this).Shutdown();
		_titleWindowManager.Shutdown();
	}

	public override void PostInit()
	{
		((GameShared)this).PostInit();
		_stylesheetManager.Initialize();
		ContentContexts.SetupContexts(_inputManager.Contexts);
		_parallaxManager.LoadDefaultParallax();
		_overlayManager.AddOverlay((Overlay)(object)new SingularityOverlay());
		_overlayManager.AddOverlay((Overlay)(object)new RMCExplosionShockWaveOverlay());
		_overlayManager.AddOverlay((Overlay)(object)new RMCXenoScreechShockWaveOverlay());
		_overlayManager.AddOverlay((Overlay)(object)new RadiationPulseOverlay());
		_chatManager.Initialize();
		_clientPreferencesManager.Initialize();
		_euiManager.Initialize();
		_voteManager.Initialize();
		_userInterfaceManager.SetDefaultTheme("SS14DefaultTheme");
		_userInterfaceManager.SetActiveTheme(_configManager.GetCVar<string>(CVars.InterfaceTheme));
		_documentParsingManager.Initialize();
		_titleWindowManager.Initialize();
		_baseClient.RunLevelChanged += delegate(object? _, RunLevelChangedEventArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Invalid comparison between Unknown and I4
			if ((int)args.NewLevel == 1)
			{
				SwitchToDefaultState((int)args.OldLevel == 3 || (int)args.OldLevel == 4);
			}
		};
		((Control)_userInterfaceManager.MainViewport).Visible = false;
		SwitchToDefaultState();
		_languageSelectManager.Initialize();
	}

	private void SwitchToDefaultState(bool disconnected = false)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (_configManager.GetCVar<bool>(CVars.LaunchContentBundle) && _resourceManager.ContentFileExists(((ResPath)(ref ReplayConstants.ReplayZipFolder)).ToRootedPath() / ReplayConstants.FileMeta))
		{
			_logManager.GetSawmill("entry").Info("Loading content bundle replay from VFS!");
			ReplayFileReaderResources val = new ReplayFileReaderResources(_resourceManager, ((ResPath)(ref ReplayConstants.ReplayZipFolder)).ToRootedPath());
			_playbackMan.LastLoad = (null, ((ResPath)(ref ReplayConstants.ReplayZipFolder)).ToRootedPath());
			_replayLoad.LoadAndStartReplay((IReplayFileReader)(object)val);
		}
		else if (_gameController.LaunchState.FromLauncher)
		{
			_stateManager.RequestStateChange<LauncherConnecting>();
			LauncherConnecting launcherConnecting = (LauncherConnecting)(object)_stateManager.CurrentState;
			if (disconnected)
			{
				launcherConnecting.SetDisconnected();
			}
		}
		else
		{
			_stateManager.RequestStateChange<MainScreen>();
		}
	}

	public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if ((int)level == 1)
		{
			_debugMonitorManager.FrameUpdate();
		}
		if ((int)level == 0)
		{
			ClientRunLevel runLevel = _baseClient.RunLevel;
			if (runLevel - 4 <= 1)
			{
				_entitySystemManager.GetEntitySystem<BuiPreTickUpdateSystem>().RunUpdates();
			}
		}
	}
}
