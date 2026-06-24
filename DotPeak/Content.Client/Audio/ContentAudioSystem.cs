// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.ContentAudioSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Lobby;
using Content.Client._PUBG.Lobby;
using Content.Client.Gameplay;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Content.Shared.Audio;
using Content.Shared.Audio.Events;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.Random.Rules;
using Robust.Client;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Audio;

public sealed class ContentAudioSystem : SharedContentAudioSystem
{
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IStateManager _state;
  [Dependency]
  private RulesSystem _rules;
  [Dependency]
  private SharedAudioSystem _audio;
  private readonly TimeSpan _minAmbienceTime = TimeSpan.FromSeconds(30L);
  private readonly TimeSpan _maxAmbienceTime = TimeSpan.FromSeconds(60L);
  private const float AmbientMusicFadeTime = 10f;
  private static float _volumeSlider;
  private TimeSpan _nextAudio;
  private EntityUid? _ambientMusicStream;
  private AmbientMusicPrototype? _musicProto;
  private bool _interruptable;
  private readonly Dictionary<string, List<ResPath>> _ambientSounds = new Dictionary<string, List<ResPath>>();
  private ISawmill _sawmill;
  private readonly Dictionary<EntityUid, float> _fadingOut = new Dictionary<EntityUid, float>();
  private readonly Dictionary<EntityUid, (float VolumeChange, float TargetVolume)> _fadingIn = new Dictionary<EntityUid, (float, float)>();
  private readonly List<EntityUid> _fadeToRemove = new List<EntityUid>();
  private const float MinVolume = -32f;
  private const float DefaultDuration = 2f;
  public const float MasterVolumeMultiplier = 3f;
  public const float MidiVolumeMultiplier = 0.25f;
  public const float AmbienceMultiplier = 3f;
  public const float AmbientMusicMultiplier = 3f;
  public const float LobbyMultiplier = 3f;
  public const float InterfaceMultiplier = 2f;
  [Dependency]
  private IBaseClient _client;
  [Dependency]
  private ClientGameTicker _gameTicker;
  [Dependency]
  private IResourceCache _resourceCache;
  private readonly AudioParams _lobbySoundtrackParams = new AudioParams(-5f, 1f, 0.0f, 0.0f, 0.0f, false, 0.0f, new float?());
  private readonly AudioParams _roundEndSoundEffectParams = new AudioParams(-5f, 1f, 0.0f, 0.0f, 0.0f, false, 0.0f, new float?());
  private EntityUid? _lobbyRoundRestartAudioStream;
  private string[]? _lobbyPlaylist;
  private ContentAudioSystem.LobbySoundtrackInfo? _lobbySoundtrackInfo;
  private Action<LobbySoundtrackChangedEvent>? _lobbySoundtrackChanged;

  private void InitializeAmbientMusic()
  {
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._configManager, CCVars.AmbientMusicVolume, new Action<float>(this.AmbienceCVarChanged), true);
    this._sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("audio.ambience");
    this._nextAudio = TimeSpan.MaxValue;
    this.SetupAmbientSounds();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnProtoReload), (Type[]) null, (Type[]) null);
    this._state.OnStateChanged += new Action<StateChangedEventArgs>(this.OnStateChange);
    this.SubscribeNetworkEvent<RoundEndMessageEvent>(new EntityEventHandler<RoundEndMessageEvent>(this.OnRoundEndMessage), (Type[]) null, (Type[]) null);
  }

  private void AmbienceCVarChanged(float obj)
  {
    ContentAudioSystem._volumeSlider = SharedAudioSystem.GainToVolume(obj);
    if (!this._ambientMusicStream.HasValue || this._musicProto == null)
      return;
    SharedAudioSystem audio = this._audio;
    EntityUid? ambientMusicStream = this._ambientMusicStream;
    AudioParams audioParams = this._musicProto.Sound.Params;
    double num = (double) ((AudioParams) ref audioParams).Volume + (double) ContentAudioSystem._volumeSlider;
    audio.SetVolume(ambientMusicStream, (float) num, (AudioComponent) null);
  }

  private void ShutdownAmbientMusic()
  {
    this._state.OnStateChanged -= new Action<StateChangedEventArgs>(this.OnStateChange);
    this._ambientMusicStream = this._audio.Stop(this._ambientMusicStream, (AudioComponent) null);
  }

  private void OnProtoReload(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<AmbientMusicPrototype>() && !obj.WasModified<RulesPrototype>())
      return;
    this.SetupAmbientSounds();
  }

  private void OnStateChange(StateChangedEventArgs obj)
  {
    if (!(obj.NewState is GameplayState))
      return;
    this._nextAudio = this._timing.CurTime + this._random.Next(this._minAmbienceTime, this._maxAmbienceTime);
  }

  private void SetupAmbientSounds()
  {
    this._ambientSounds.Clear();
    foreach (AmbientMusicPrototype enumeratePrototype in this._proto.EnumeratePrototypes<AmbientMusicPrototype>())
    {
      List<ResPath> orNew = Extensions.GetOrNew<string, List<ResPath>>(this._ambientSounds, enumeratePrototype.ID);
      this.RefreshTracks(enumeratePrototype.Sound, orNew, new ResPath?());
      this._random.Shuffle<ResPath>((IList<ResPath>) orNew);
    }
  }

  private void OnRoundEndMessage(RoundEndMessageEvent ev)
  {
    this._ambientMusicStream = this._audio.Stop(this._ambientMusicStream, (AudioComponent) null);
    this._nextAudio = TimeSpan.FromMinutes(3L);
  }

  private void RefreshTracks(SoundSpecifier sound, List<ResPath> tracks, ResPath? lastPlayed)
  {
    if (!(sound is SoundCollectionSpecifier collectionSpecifier))
    {
      if (sound is SoundPathSpecifier soundPathSpecifier)
        tracks.Add(soundPathSpecifier.Path);
    }
    else if (collectionSpecifier.Collection != null)
    {
      SoundCollectionPrototype collectionPrototype = this._proto.Index<SoundCollectionPrototype>(collectionSpecifier.Collection);
      tracks.AddRange((IEnumerable<ResPath>) collectionPrototype.PickFiles);
    }
    if (tracks.Count <= 1)
      return;
    List<ResPath> resPathList1 = tracks;
    ResPath resPath1 = resPathList1[resPathList1.Count - 1];
    ResPath? nullable = lastPlayed;
    if ((nullable.HasValue ? (ResPath.op_Equality(resPath1, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    List<ResPath> resPathList2 = tracks;
    List<ResPath> resPathList3 = tracks;
    int index = resPathList3.Count - 1;
    List<ResPath> resPathList4 = tracks;
    ResPath resPath2 = resPathList4[resPathList4.Count - 1];
    ResPath track = tracks[0];
    resPathList2[0] = resPath2;
    resPathList3[index] = track;
  }

  private void UpdateAmbientMusic()
  {
    if (!(this._state.CurrentState is GameplayState))
    {
      this._ambientMusicStream = this.Audio.Stop(this._ambientMusicStream, (AudioComponent) null);
      this._musicProto = (AmbientMusicPrototype) null;
    }
    else
    {
      bool? nullable1 = new bool?();
      AudioComponent audioComponent;
      if (this.TryComp<AudioComponent>(this._ambientMusicStream, ref audioComponent))
        nullable1 = new bool?(!audioComponent.Playing);
      if (this._interruptable)
      {
        EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity;
        if (!attachedEntity.HasValue || this._musicProto == null || !this._rules.IsTrue(attachedEntity.Value, this._proto.Index<RulesPrototype>(this._musicProto.Rules)))
        {
          this.FadeOut(this._ambientMusicStream, duration: 10f);
          this._musicProto = (AmbientMusicPrototype) null;
          this._interruptable = false;
          nullable1 = new bool?(true);
        }
      }
      bool? nullable2 = nullable1;
      bool flag = false;
      if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
        return;
      if (nullable1.GetValueOrDefault())
        this._nextAudio = this._timing.CurTime + this._random.Next(this._minAmbienceTime, this._maxAmbienceTime);
      this._ambientMusicStream = new EntityUid?();
      if (this._nextAudio > this._timing.CurTime)
        return;
      this._musicProto = this.GetAmbience();
      if (this._musicProto == null)
      {
        this._interruptable = false;
      }
      else
      {
        this._interruptable = this._musicProto.Interruptable;
        List<ResPath> ambientSound = this._ambientSounds[this._musicProto.ID];
        List<ResPath> resPathList = ambientSound;
        ResPath resPath = resPathList[resPathList.Count - 1];
        ambientSound.RemoveAt(ambientSound.Count - 1);
        SharedAudioSystem audio = this._audio;
        ResolvedPathSpecifier resolvedPathSpecifier = new ResolvedPathSpecifier(resPath);
        Filter filter = Filter.Local();
        ref AudioParams local = ref AudioParams.Default;
        AudioParams audioParams = this._musicProto.Sound.Params;
        double num = (double) ((AudioParams) ref audioParams).Volume + (double) ContentAudioSystem._volumeSlider;
        AudioParams? nullable3 = new AudioParams?(((AudioParams) ref local).WithVolume((float) num));
        (EntityUid, AudioComponent)? nullable4 = audio.PlayGlobal((ResolvedSoundSpecifier) resolvedPathSpecifier, filter, false, nullable3);
        this._ambientMusicStream = nullable4.Item1;
        if (this._musicProto.FadeIn && nullable4.HasValue)
          this.FadeIn(this._ambientMusicStream, nullable4.Value.Item2, 10f);
        if (ambientSound.Count != 0)
          return;
        this.RefreshTracks(this._musicProto.Sound, ambientSound, new ResPath?(resPath));
      }
    }
  }

  private AmbientMusicPrototype? GetAmbience()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return (AmbientMusicPrototype) null;
    PlayAmbientMusicEvent ambientMusicEvent = new PlayAmbientMusicEvent();
    this.RaiseLocalEvent<PlayAmbientMusicEvent>(ref ambientMusicEvent);
    if (ambientMusicEvent.Cancelled)
      return (AmbientMusicPrototype) null;
    List<AmbientMusicPrototype> list = this._proto.EnumeratePrototypes<AmbientMusicPrototype>().ToList<AmbientMusicPrototype>();
    list.Sort((Comparison<AmbientMusicPrototype>) ((x, y) => y.Priority.CompareTo(x.Priority)));
    foreach (AmbientMusicPrototype ambience in list)
    {
      if (this._rules.IsTrue(localEntity.Value, this._proto.Index<RulesPrototype>(ambience.Rules)))
        return ambience;
    }
    this._sawmill.Warning("Unable to find fallback ambience track");
    return (AmbientMusicPrototype) null;
  }

  public void DisableAmbientMusic()
  {
    this.FadeOut(this._ambientMusicStream);
    this._ambientMusicStream = new EntityUid?();
  }

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this.InitializeAmbientMusic();
    this.InitializeLobbyMusic();
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundCleanup), (Type[]) null, (Type[]) null);
  }

  private void OnRoundCleanup(RoundRestartCleanupEvent ev)
  {
    this._fadingOut.Clear();
    EntityUid? musicStreamEntityUid = this._lobbySoundtrackInfo?.MusicStreamEntityUid;
    AudioComponent audioComponent1;
    this.TryComp<AudioComponent>(musicStreamEntityUid, ref audioComponent1);
    float? gain1 = audioComponent1?.Gain;
    EntityUid? restartAudioStream = this._lobbyRoundRestartAudioStream;
    AudioComponent audioComponent2;
    this.TryComp<AudioComponent>(restartAudioStream, ref audioComponent2);
    float? gain2 = audioComponent2?.Gain;
    this.SilenceAudio();
    if (gain1.HasValue)
      this.Audio.SetGain(musicStreamEntityUid, gain1.Value, audioComponent1);
    if (gain2.HasValue)
      this.Audio.SetGain(restartAudioStream, gain2.Value, audioComponent2);
    this.PlayRestartSound(ev);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.ShutdownAmbientMusic();
    this.ShutdownLobbyMusic();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    this.UpdateAmbientMusic();
    this.UpdateLobbyMusic();
    this.UpdateFades(frameTime);
  }

  public void FadeOut(EntityUid? stream, AudioComponent? component = null, float duration = 2f)
  {
    if (!stream.HasValue || (double) duration <= 0.0 || !this.Resolve<AudioComponent>(stream.Value, ref component, true))
      return;
    this._fadingIn.Remove(stream.Value);
    float num = component.Volume - -32f;
    this._fadingOut.Add(stream.Value, num / duration);
  }

  public void FadeIn(EntityUid? stream, AudioComponent? component = null, float duration = 2f)
  {
    if (!stream.HasValue || (double) duration <= 0.0 || !this.Resolve<AudioComponent>(stream.Value, ref component, true) || (double) component.Volume < -32.0)
      return;
    this._fadingOut.Remove(stream.Value);
    float num = (-32f - component.Volume) / duration;
    this._fadingIn.Add(stream.Value, (num, component.Volume));
    component.Volume = -32f;
  }

  private void UpdateFades(float frameTime)
  {
    this._fadeToRemove.Clear();
    foreach ((EntityUid key, float volume) in this._fadingOut)
    {
      EntityUid entityUid = key;
      float num1 = volume;
      AudioComponent audioComponent;
      if (!this.TryComp<AudioComponent>(entityUid, ref audioComponent))
      {
        this._fadeToRemove.Add(entityUid);
      }
      else
      {
        float num2 = MathF.Max(-32f, audioComponent.Volume - num1 * frameTime);
        this._audio.SetVolume(new EntityUid?(entityUid), num2, audioComponent);
        volume = audioComponent.Volume;
        if (volume.Equals(-32f))
        {
          this._audio.Stop(new EntityUid?(entityUid), (AudioComponent) null);
          this._fadeToRemove.Add(entityUid);
        }
      }
    }
    foreach (EntityUid key in this._fadeToRemove)
      this._fadingOut.Remove(key);
    this._fadeToRemove.Clear();
    (float VolumeChange, float TargetVolume) tuple2;
    foreach ((key, tuple2) in this._fadingIn)
    {
      EntityUid entityUid = key;
      float volumeChange = tuple2.VolumeChange;
      float targetVolume = tuple2.TargetVolume;
      AudioComponent audioComponent;
      if (!this.TryComp<AudioComponent>(entityUid, ref audioComponent))
      {
        this._fadeToRemove.Add(entityUid);
      }
      else
      {
        float y = audioComponent.Volume - volumeChange * frameTime;
        float num = MathF.Min(targetVolume, y);
        this._audio.SetVolume(new EntityUid?(entityUid), num, audioComponent);
        if (audioComponent.Volume.Equals(targetVolume))
          this._fadeToRemove.Add(entityUid);
      }
    }
    foreach (EntityUid key in this._fadeToRemove)
      this._fadingIn.Remove(key);
  }

  public event Action<LobbySoundtrackChangedEvent>? LobbySoundtrackChanged
  {
    add
    {
      if (value == null)
        return;
      if (this._lobbySoundtrackInfo != (ContentAudioSystem.LobbySoundtrackInfo) null)
        value(new LobbySoundtrackChangedEvent(this._lobbySoundtrackInfo.Filename));
      this._lobbySoundtrackChanged += value;
    }
    remove => this._lobbySoundtrackChanged -= value;
  }

  private void InitializeLobbyMusic()
  {
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configManager, CCVars.LobbyMusicEnabled, new Action<bool>(this.LobbyMusicCVarChanged), false);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._configManager, CCVars.LobbyMusicVolume, new Action<float>(this.LobbyMusicVolumeCVarChanged), false);
    this._state.OnStateChanged += new Action<StateChangedEventArgs>(this.StateManagerOnStateChanged);
    this._client.PlayerLeaveServer += new EventHandler<PlayerEventArgs>(this.OnLeave);
    this.SubscribeNetworkEvent<LobbyMusicStopEvent>(new EntityEventHandler<LobbyMusicStopEvent>(this.OnLobbySongStopped), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<LobbyPlaylistChangedEvent>(new EntityEventHandler<LobbyPlaylistChangedEvent>(this.OnLobbySongChanged), (Type[]) null, (Type[]) null);
  }

  private void OnLobbySongStopped(LobbyMusicStopEvent ev) => this.EndLobbyMusic();

  private void StateManagerOnStateChanged(StateChangedEventArgs args)
  {
    switch (args.NewState)
    {
      case LobbyState _:
      case PubgPreLobbyHubState _:
      case CivLobbyState _:
        this.StartLobbyMusic();
        break;
      default:
        this.EndLobbyMusic();
        break;
    }
  }

  private void OnLeave(object? sender, PlayerEventArgs args) => this.EndLobbyMusic();

  private void LobbyMusicVolumeCVarChanged(float volume)
  {
    if (!(this._lobbySoundtrackInfo != (ContentAudioSystem.LobbySoundtrackInfo) null))
      return;
    SharedAudioSystem audio = this._audio;
    EntityUid? nullable = new EntityUid?(this._lobbySoundtrackInfo.MusicStreamEntityUid);
    AudioParams soundtrackParams = this._lobbySoundtrackParams;
    double num = (double) ((AudioParams) ref soundtrackParams).Volume + (double) SharedAudioSystem.GainToVolume(this._configManager.GetCVar<float>(CCVars.LobbyMusicVolume));
    audio.SetVolume(nullable, (float) num, (AudioComponent) null);
  }

  private void LobbyMusicCVarChanged(bool musicEnabled)
  {
    if (musicEnabled && (this._state.CurrentState is LobbyState || this._state.CurrentState is PubgPreLobbyHubState || this._state.CurrentState is CivLobbyState))
      this.StartLobbyMusic();
    else
      this.EndLobbyMusic();
  }

  private void OnLobbySongChanged(LobbyPlaylistChangedEvent playlistChangedEvent)
  {
    string[] playlist = playlistChangedEvent.Playlist;
    if (this._lobbySoundtrackInfo != (ContentAudioSystem.LobbySoundtrackInfo) null && this._lobbyPlaylist != null && ((IEnumerable<string>) this._lobbyPlaylist).SequenceEqual<string>((IEnumerable<string>) playlist))
      return;
    this.EndLobbyMusic();
    this.StartLobbyMusic(playlistChangedEvent.Playlist);
  }

  private void StartLobbyMusic()
  {
    if (this._lobbyPlaylist == null || this._lobbyPlaylist.Length == 0)
      return;
    this.StartLobbyMusic(this._lobbyPlaylist);
  }

  private void StartLobbyMusic(string[] playlist)
  {
    if (this._lobbySoundtrackInfo != (ContentAudioSystem.LobbySoundtrackInfo) null || !this._configManager.GetCVar<bool>(CCVars.LobbyMusicEnabled))
      return;
    this._lobbyPlaylist = playlist;
    if (this._lobbyPlaylist.Length == 0)
      return;
    this.PlaySoundtrack(playlist[0]);
  }

  private void PlaySoundtrack(string soundtrackFilename)
  {
    AudioResource audioResource;
    if (!this._resourceCache.TryGetResource<AudioResource>(new ResPath(soundtrackFilename), ref audioResource))
      return;
    SharedAudioSystem audio = this._audio;
    ResolvedPathSpecifier resolvedPathSpecifier = new ResolvedPathSpecifier(soundtrackFilename);
    Filter filter = Filter.Local();
    ref readonly AudioParams local = ref this._lobbySoundtrackParams;
    AudioParams soundtrackParams = this._lobbySoundtrackParams;
    double num = (double) ((AudioParams) ref soundtrackParams).Volume + (double) SharedAudioSystem.GainToVolume(this._configManager.GetCVar<float>(CCVars.LobbyMusicVolume));
    AudioParams? nullable1 = new AudioParams?(((AudioParams) ref local).WithVolume((float) num));
    (EntityUid, AudioComponent)? nullable2 = audio.PlayGlobal((ResolvedSoundSpecifier) resolvedPathSpecifier, filter, false, nullable1);
    if (!nullable2.HasValue)
    {
      this._sawmill.Warning("Tried to play lobby soundtrack '{Filename}' using SharedAudioSystem.PlayGlobal but it returned default value of EntityUid!", new object[1]
      {
        (object) soundtrackFilename
      });
    }
    else
    {
      TimeSpan NextTrackOn = this._timing.CurTime + audioResource.AudioStream.Length;
      this._lobbySoundtrackInfo = new ContentAudioSystem.LobbySoundtrackInfo(soundtrackFilename, NextTrackOn, nullable2.Value.Item1);
      LobbySoundtrackChangedEvent soundtrackChangedEvent = new LobbySoundtrackChangedEvent(soundtrackFilename);
      Action<LobbySoundtrackChangedEvent> soundtrackChanged = this._lobbySoundtrackChanged;
      if (soundtrackChanged == null)
        return;
      soundtrackChanged(soundtrackChangedEvent);
    }
  }

  private void EndLobbyMusic()
  {
    if (this._lobbySoundtrackInfo == (ContentAudioSystem.LobbySoundtrackInfo) null)
      return;
    this._audio.Stop(new EntityUid?(this._lobbySoundtrackInfo.MusicStreamEntityUid), (AudioComponent) null);
    this._lobbySoundtrackInfo = (ContentAudioSystem.LobbySoundtrackInfo) null;
    LobbySoundtrackChangedEvent soundtrackChangedEvent = new LobbySoundtrackChangedEvent();
    Action<LobbySoundtrackChangedEvent> soundtrackChanged = this._lobbySoundtrackChanged;
    if (soundtrackChanged == null)
      return;
    soundtrackChanged(soundtrackChangedEvent);
  }

  private void PlayRestartSound(RoundRestartCleanupEvent ev)
  {
    if (!this._configManager.GetCVar<bool>(CCVars.RestartSoundsEnabled))
      return;
    ResolvedSoundSpecifier restartSound = this._gameTicker.RestartSound;
    if (ResolvedSoundSpecifier.IsNullOrEmpty(restartSound))
      return;
    SharedAudioSystem audio = this._audio;
    ResolvedSoundSpecifier resolvedSoundSpecifier = restartSound;
    Filter filter = Filter.Local();
    ref readonly AudioParams local1 = ref this._roundEndSoundEffectParams;
    AudioParams soundEffectParams = this._roundEndSoundEffectParams;
    double num = (double) ((AudioParams) ref soundEffectParams).Volume + (double) SharedAudioSystem.GainToVolume(this._configManager.GetCVar<float>(CCVars.LobbyMusicVolume));
    AudioParams? nullable1 = new AudioParams?(((AudioParams) ref local1).WithVolume((float) num));
    (EntityUid, AudioComponent)? nullable2 = audio.PlayGlobal(resolvedSoundSpecifier, filter, false, nullable1);
    ref (EntityUid, AudioComponent)? local2 = ref nullable2;
    this._lobbyRoundRestartAudioStream = local2.HasValue ? new EntityUid?(local2.GetValueOrDefault().Item1) : new EntityUid?();
  }

  private void ShutdownLobbyMusic()
  {
    this._state.OnStateChanged -= new Action<StateChangedEventArgs>(this.StateManagerOnStateChanged);
    this._client.PlayerLeaveServer -= new EventHandler<PlayerEventArgs>(this.OnLeave);
    this.EndLobbyMusic();
  }

  private void UpdateLobbyMusic()
  {
    if (!(this._lobbySoundtrackInfo != (ContentAudioSystem.LobbySoundtrackInfo) null) || !(this._timing.CurTime >= this._lobbySoundtrackInfo.NextTrackOn))
      return;
    string[] lobbyPlaylist = this._lobbyPlaylist;
    if ((lobbyPlaylist != null ? (lobbyPlaylist.Length != 0 ? 1 : 0) : 0) == 0)
      return;
    this.PlaySoundtrack(ContentAudioSystem.GetNextSoundtrackFromPlaylist(this._lobbySoundtrackInfo.Filename, this._lobbyPlaylist));
  }

  private static string GetNextSoundtrackFromPlaylist(
    string currentSoundtrackFilename,
    string[] playlist)
  {
    int index = Array.IndexOf<string>(playlist, currentSoundtrackFilename) + 1;
    if (index > playlist.Length - 1)
      index = 0;
    return playlist[index];
  }

  private sealed record LobbySoundtrackInfo(
    string Filename,
    TimeSpan NextTrackOn,
    EntityUid MusicStreamEntityUid)
  ;
}
