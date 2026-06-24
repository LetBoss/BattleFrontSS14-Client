// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.ContentReplayPlaybackManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.IO.Compression;

#nullable enable
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
    if (this._initialized)
      return;
    this._initialized = true;
    // ISSUE: method pointer
    this._playback.HandleReplayMessage += new IReplayPlaybackManager.HandleReplayMessageDelegate((object) this, __methodptr(OnHandleReplayMessage));
    this._playback.ReplayPlaybackStopped += new Action(this.OnReplayPlaybackStopped);
    this._playback.ReplayPlaybackStarted += new Action<MappingDataNode, List<object>>(this.OnReplayPlaybackStarted);
    this._playback.ReplayCheckpointReset += new Action(this.OnCheckpointReset);
    this._loadMan.LoadOverride += new Action<IReplayFileReader>(this.LoadOverride);
  }

  private void LoadOverride(IReplayFileReader fileReader)
  {
    LoadingScreen<bool> screen = this._stateMan.RequestStateChange<LoadingScreen<bool>>();
    screen.Job = (Job<bool>) new ContentLoadReplayJob(0.0166666675f, fileReader, this._loadMan, screen);
    screen.OnJobFinished += (Action<bool, Exception>) ((_, e) => this.OnFinishedLoading(e));
  }

  private void OnFinishedLoading(Exception? exception)
  {
    if (exception == null)
    {
      this.LastLoad = new (ResPath?, ResPath)?();
    }
    else
    {
      if (this._client.RunLevel == 5)
        this._client.StopSinglePlayer();
      Action retryPressed = (Action) null;
      Action cancelPressed = (Action) null;
      if (!this._cfg.GetCVar<bool>(CVars.ReplayIgnoreErrors))
      {
        (ResPath? Zip, ResPath Folder)? lastLoad = this.LastLoad;
        if (lastLoad.HasValue)
        {
          (ResPath? Zip, ResPath Folder) last = lastLoad.GetValueOrDefault();
          retryPressed = (Action) (() =>
          {
            this._cfg.SetCVar<bool>(CVars.ReplayIgnoreErrors, true, false);
            this._loadMan.LoadAndStartReplay(!last.Zip.HasValue ? (IReplayFileReader) new ReplayFileReaderResources(this._resMan, last.Folder) : (IReplayFileReader) new ReplayFileReaderZip(new ZipArchive(WritableDirProviderExt.OpenRead(this._resMan.UserData, last.Zip.Value)), last.Folder));
          });
        }
      }
      if (this.DefaultState != (Type) null)
        cancelPressed = (Action) (() => this._stateMan.RequestStateChange(this.DefaultState));
      this._stateMan.RequestStateChange<ReplayLoadingFailed>().SetData(exception, cancelPressed, retryPressed);
    }
  }

  public void ReturnToDefaultState()
  {
    if (this.DefaultState != (Type) null)
      this._stateMan.RequestStateChange(this.DefaultState);
    else if (this._controller.LaunchState.FromLauncher)
      this._stateMan.RequestStateChange<LauncherConnecting>().SetDisconnected();
    else
      this._stateMan.RequestStateChange<MainScreen>();
    if (this._client.RunLevel != 5)
      return;
    this._client.StopSinglePlayer();
  }

  private void OnCheckpointReset()
  {
    this._uiMan.GetUIController<ChatUIController>().History.RemoveAll((Predicate<(GameTick, ChatMessage)>) (x => GameTick.op_GreaterThan(x.Tick, ((IGameTiming) this._timing).CurTick)));
    this._uiMan.GetUIController<ChatUIController>().Repopulate();
  }

  private bool OnHandleReplayMessage(object message, bool skipEffects)
  {
    switch (message)
    {
      case BoundUserInterfaceMessage _:
      case RequestWindowAttentionEvent _:
        return true;
      case TickerJoinGameEvent _:
        if (!((IEntityManager) this._entMan).EntityExists(((ISharedPlayerManager) this._player).LocalEntity))
          ((IEntityManager) this._entMan).System<ReplaySpectatorSystem>().SetSpectatorPosition(new ReplaySpectatorSystem.SpectatorData());
        return true;
      case ChatMessage msg:
        this._uiMan.GetUIController<ChatUIController>().ProcessChatMessage(msg, !skipEffects);
        return true;
      default:
        if (!skipEffects)
          return false;
        switch (message)
        {
          case RoundEndMessageEvent _:
          case PopupEvent _:
          case PickupAnimationEvent _:
          case MeleeLungeEvent _:
          case SharedGunSystem.HitscanEvent _:
          case ImpactEffectEvent _:
          case MuzzleFlashEvent _:
          case ColorFlashEffectEvent _:
          case InstrumentStartMidiEvent _:
          case InstrumentMidiEventEvent _:
          case InstrumentStopMidiEvent _:
            return true;
          default:
            return false;
        }
    }
  }

  private void OnReplayPlaybackStarted(MappingDataNode metadata, List<object> objects)
  {
    this._conGrp.Implementation = (IClientConGroupImplementation) new ReplayConGroup();
  }

  private void OnReplayPlaybackStopped()
  {
    this._conGrp.Implementation = (IClientConGroupImplementation) this._adminMan;
    this.ReturnToDefaultState();
  }
}
