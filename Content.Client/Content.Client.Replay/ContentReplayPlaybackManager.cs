using System;
using System.Collections.Generic;
using System.IO.Compression;
using Content.Client.Administration.Managers;
using Content.Client.Launcher;
using Content.Client.MainMenu;
using Content.Client.Replay.Spectator;
using Content.Client.Replay.UI.Loading;
using Content.Client.UserInterface.Systems.Chat;
using Content.Shared.Chat;
using Content.Shared.Effects;
using Content.Shared.GameTicking;
using Content.Shared.GameWindow;
using Content.Shared.Hands;
using Content.Shared.Instruments;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.Replays.Loading;
using Robust.Client.Replays.Playback;
using Robust.Client.State;
using Robust.Client.Timing;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.CPUJob.JobQueues;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Replay;

public sealed class ContentReplayPlaybackManager
{
	[Dependency]
	private IStateManager _stateMan;

	[Dependency]
	private IClientGameTiming _timing;

	[Dependency]
	private IReplayLoadManager _loadMan;

	[Dependency]
	private IGameController _controller;

	[Dependency]
	private IClientEntityManager _entMan;

	[Dependency]
	private IUserInterfaceManager _uiMan;

	[Dependency]
	private IReplayPlaybackManager _playback;

	[Dependency]
	private IClientConGroupController _conGrp;

	[Dependency]
	private IClientAdminManager _adminMan;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IBaseClient _client;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IResourceManager _resMan;

	public Type? DefaultState;

	public bool IsScreenshotMode;

	private bool _initialized;

	public (ResPath? Zip, ResPath Folder)? LastLoad;

	public void Initialize()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (!_initialized)
		{
			_initialized = true;
			_playback.HandleReplayMessage += new HandleReplayMessageDelegate(OnHandleReplayMessage);
			_playback.ReplayPlaybackStopped += OnReplayPlaybackStopped;
			_playback.ReplayPlaybackStarted += OnReplayPlaybackStarted;
			_playback.ReplayCheckpointReset += OnCheckpointReset;
			_loadMan.LoadOverride += LoadOverride;
		}
	}

	private void LoadOverride(IReplayFileReader fileReader)
	{
		LoadingScreen<bool> loadingScreen = _stateMan.RequestStateChange<LoadingScreen<bool>>();
		loadingScreen.Job = (Job<bool>?)(object)new ContentLoadReplayJob(1f / 60f, fileReader, _loadMan, loadingScreen);
		loadingScreen.OnJobFinished += delegate(bool _, Exception? e)
		{
			OnFinishedLoading(e);
		};
	}

	private void OnFinishedLoading(Exception? exception)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		if (exception == null)
		{
			LastLoad = null;
			return;
		}
		if ((int)_client.RunLevel == 5)
		{
			_client.StopSinglePlayer();
		}
		Action retryPressed = null;
		Action cancelPressed = null;
		if (!_cfg.GetCVar<bool>(CVars.ReplayIgnoreErrors))
		{
			(ResPath?, ResPath)? lastLoad = LastLoad;
			if (lastLoad.HasValue)
			{
				(ResPath? Zip, ResPath Folder) last = lastLoad.GetValueOrDefault();
				retryPressed = delegate
				{
					//IL_0078: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0083: Expected O, but got Unknown
					//IL_0044: Unknown result type (might be due to invalid IL or missing references)
					//IL_0059: Unknown result type (might be due to invalid IL or missing references)
					//IL_005e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0064: Expected O, but got Unknown
					_cfg.SetCVar<bool>(CVars.ReplayIgnoreErrors, true, false);
					IReplayFileReader obj;
					if (last.Zip.HasValue)
					{
						IReplayFileReader val = (IReplayFileReader)new ReplayFileReaderZip(new ZipArchive(WritableDirProviderExt.OpenRead(_resMan.UserData, last.Zip.Value)), last.Folder);
						obj = val;
					}
					else
					{
						IReplayFileReader val = (IReplayFileReader)new ReplayFileReaderResources(_resMan, last.Folder);
						obj = val;
					}
					IReplayFileReader val2 = obj;
					_loadMan.LoadAndStartReplay(val2);
				};
			}
		}
		if (DefaultState != null)
		{
			cancelPressed = delegate
			{
				_stateMan.RequestStateChange(DefaultState);
			};
		}
		_stateMan.RequestStateChange<ReplayLoadingFailed>().SetData(exception, cancelPressed, retryPressed);
	}

	public void ReturnToDefaultState()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I4
		if (DefaultState != null)
		{
			_stateMan.RequestStateChange(DefaultState);
		}
		else if (_controller.LaunchState.FromLauncher)
		{
			_stateMan.RequestStateChange<LauncherConnecting>().SetDisconnected();
		}
		else
		{
			_stateMan.RequestStateChange<MainScreen>();
		}
		if ((int)_client.RunLevel == 5)
		{
			_client.StopSinglePlayer();
		}
	}

	private void OnCheckpointReset()
	{
		_uiMan.GetUIController<ChatUIController>().History.RemoveAll(((GameTick Tick, ChatMessage Msg) x) => x.Tick > ((IGameTiming)_timing).CurTick);
		_uiMan.GetUIController<ChatUIController>().Repopulate();
	}

	private bool OnHandleReplayMessage(object message, bool skipEffects)
	{
		if (!(message is BoundUserInterfaceMessage) && !(message is RequestWindowAttentionEvent))
		{
			if (!(message is TickerJoinGameEvent))
			{
				if (message is ChatMessage msg)
				{
					_uiMan.GetUIController<ChatUIController>().ProcessChatMessage(msg, !skipEffects);
					return true;
				}
				if (!skipEffects)
				{
					return false;
				}
				if (message is RoundEndMessageEvent || message is PopupEvent || message is PickupAnimationEvent || message is MeleeLungeEvent || message is SharedGunSystem.HitscanEvent || message is ImpactEffectEvent || message is MuzzleFlashEvent || message is ColorFlashEffectEvent || message is InstrumentStartMidiEvent || message is InstrumentMidiEventEvent || message is InstrumentStopMidiEvent)
				{
					return true;
				}
				return false;
			}
			if (!((IEntityManager)_entMan).EntityExists(((ISharedPlayerManager)_player).LocalEntity))
			{
				((IEntityManager)_entMan).System<ReplaySpectatorSystem>().SetSpectatorPosition(default(ReplaySpectatorSystem.SpectatorData));
			}
			return true;
		}
		return true;
	}

	private void OnReplayPlaybackStarted(MappingDataNode metadata, List<object> objects)
	{
		_conGrp.Implementation = (IClientConGroupImplementation)(object)new ReplayConGroup();
	}

	private void OnReplayPlaybackStopped()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_conGrp.Implementation = (IClientConGroupImplementation)_adminMan;
		ReturnToDefaultState();
	}
}
