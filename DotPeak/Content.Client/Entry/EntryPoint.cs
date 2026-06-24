// Decompiled with JetBrains decompiler
// Type: Content.Client.Entry.EntryPoint
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
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

  public virtual void Init()
  {
    ClientContentIoC.Register();
    foreach (ClientModuleTestingCallbacks testingCallback in ((GameShared) this).TestingCallbacks)
    {
      Action clientBeforeIoC = testingCallback.ClientBeforeIoC;
      if (clientBeforeIoC != null)
        clientBeforeIoC();
    }
    IoCManager.BuildGraph();
    IoCManager.InjectDependencies<EntryPoint>(this);
    this._contentLoc.Initialize();
    this._componentFactory.DoAutoRegistrations();
    this._componentFactory.IgnoreMissingComponents("");
    this._componentFactory.RegisterClass<SharedGravityGeneratorComponent>(false);
    this._componentFactory.RegisterClass<SharedAmeControllerComponent>(false);
    this._prototypeManager.RegisterIgnore("utilityQuery");
    this._prototypeManager.RegisterIgnore("utilityCurvePreset");
    this._prototypeManager.RegisterIgnore("accent");
    this._prototypeManager.RegisterIgnore("gasReaction");
    this._prototypeManager.RegisterIgnore("seed");
    this._prototypeManager.RegisterIgnore("objective");
    this._prototypeManager.RegisterIgnore("holiday");
    this._prototypeManager.RegisterIgnore("htnCompound");
    this._prototypeManager.RegisterIgnore("htnPrimitive");
    this._prototypeManager.RegisterIgnore("gameMap");
    this._prototypeManager.RegisterIgnore("gameMapPool");
    this._prototypeManager.RegisterIgnore("lobbyBackground");
    this._prototypeManager.RegisterIgnore("gamePreset");
    this._prototypeManager.RegisterIgnore("noiseChannel");
    this._prototypeManager.RegisterIgnore("playerConnectionWhitelist");
    this._prototypeManager.RegisterIgnore("spaceBiome");
    this._prototypeManager.RegisterIgnore("worldgenConfig");
    this._prototypeManager.RegisterIgnore("gameRule");
    this._prototypeManager.RegisterIgnore("worldSpell");
    this._prototypeManager.RegisterIgnore("entitySpell");
    this._prototypeManager.RegisterIgnore("instantSpell");
    this._prototypeManager.RegisterIgnore("roundAnnouncement");
    this._prototypeManager.RegisterIgnore("wireLayout");
    this._prototypeManager.RegisterIgnore("alertLevels");
    this._prototypeManager.RegisterIgnore("nukeopsRole");
    this._prototypeManager.RegisterIgnore("ghostRoleRaffleDecider");
    this._prototypeManager.RegisterIgnore("codewordGenerator");
    this._prototypeManager.RegisterIgnore("codewordFaction");
    this._componentFactory.GenerateNetIds();
    this._adminManager.Initialize();
    this._screenshotHook.Initialize();
    this._fullscreenHook.Initialize();
    this._changelogManager.Initialize();
    this._viewportManager.Initialize();
    this._ghostKick.Initialize();
    this._extendedDisconnectInformation.Initialize();
    this._jobRequirements.Initialize();
    this._playbackMan.Initialize();
    this._clientsidePlaytimeManager.Initialize();
    this._configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffX", (object) 1080, false);
    this._configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffY", (object) 720, false);
    this._configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffX", (object) 520, false);
    this._configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffY", (object) 240 /*0xF0*/, false);
    this._configManager.SetCVar("interface.resolutionAutoScaleMinimum", (object) 0.5f, false);
  }

  public virtual void Shutdown()
  {
    ((GameShared) this).Shutdown();
    this._titleWindowManager.Shutdown();
  }

  public virtual void PostInit()
  {
    ((GameShared) this).PostInit();
    this._stylesheetManager.Initialize();
    ContentContexts.SetupContexts(this._inputManager.Contexts);
    this._parallaxManager.LoadDefaultParallax();
    this._overlayManager.AddOverlay((Overlay) new SingularityOverlay());
    this._overlayManager.AddOverlay((Overlay) new RMCExplosionShockWaveOverlay());
    this._overlayManager.AddOverlay((Overlay) new RMCXenoScreechShockWaveOverlay());
    this._overlayManager.AddOverlay((Overlay) new RadiationPulseOverlay());
    this._chatManager.Initialize();
    this._clientPreferencesManager.Initialize();
    this._euiManager.Initialize();
    this._voteManager.Initialize();
    this._userInterfaceManager.SetDefaultTheme("SS14DefaultTheme");
    this._userInterfaceManager.SetActiveTheme(this._configManager.GetCVar<string>(CVars.InterfaceTheme));
    this._documentParsingManager.Initialize();
    this._titleWindowManager.Initialize();
    this._baseClient.RunLevelChanged += (EventHandler<RunLevelChangedEventArgs>) ((_, args) =>
    {
      if (args.NewLevel != 1)
        return;
      this.SwitchToDefaultState(args.OldLevel == 3 || args.OldLevel == 4);
    });
    ((Control) this._userInterfaceManager.MainViewport).Visible = false;
    this.SwitchToDefaultState();
    this._languageSelectManager.Initialize();
  }

  private void SwitchToDefaultState(bool disconnected = false)
  {
    if (this._configManager.GetCVar<bool>(CVars.LaunchContentBundle) && this._resourceManager.ContentFileExists(ResPath.op_Division(((ResPath) ref ReplayConstants.ReplayZipFolder).ToRootedPath(), ReplayConstants.FileMeta)))
    {
      this._logManager.GetSawmill("entry").Info("Loading content bundle replay from VFS!");
      ReplayFileReaderResources fileReaderResources = new ReplayFileReaderResources(this._resourceManager, ((ResPath) ref ReplayConstants.ReplayZipFolder).ToRootedPath());
      this._playbackMan.LastLoad = new (ResPath?, ResPath)?((new ResPath?(), ((ResPath) ref ReplayConstants.ReplayZipFolder).ToRootedPath()));
      this._replayLoad.LoadAndStartReplay((IReplayFileReader) fileReaderResources);
    }
    else if (this._gameController.LaunchState.FromLauncher)
    {
      this._stateManager.RequestStateChange<LauncherConnecting>();
      LauncherConnecting currentState = (LauncherConnecting) this._stateManager.CurrentState;
      if (!disconnected)
        return;
      currentState.SetDisconnected();
    }
    else
      this._stateManager.RequestStateChange<MainScreen>();
  }

  public virtual void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
  {
    if (level == 1)
      this._debugMonitorManager.FrameUpdate();
    if (level != null || this._baseClient.RunLevel - 4 > 1)
      return;
    this._entitySystemManager.GetEntitySystem<BuiPreTickUpdateSystem>().RunUpdates();
  }
}
