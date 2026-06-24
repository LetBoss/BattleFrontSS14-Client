using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Content.Client.Audio;

public sealed class ContentAudioSystem : SharedContentAudioSystem
{
	private sealed record LobbySoundtrackInfo(string Filename, TimeSpan NextTrackOn, EntityUid MusicStreamEntityUid);

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

	private readonly AudioParams _lobbySoundtrackParams = new AudioParams(-5f, 1f, 0f, 0f, 0f, false, 0f, (float?)null);

	private readonly AudioParams _roundEndSoundEffectParams = new AudioParams(-5f, 1f, 0f, 0f, 0f, false, 0f, (float?)null);

	private EntityUid? _lobbyRoundRestartAudioStream;

	private string[]? _lobbyPlaylist;

	private LobbySoundtrackInfo? _lobbySoundtrackInfo;

	private Action<LobbySoundtrackChangedEvent>? _lobbySoundtrackChanged;

	public event Action<LobbySoundtrackChangedEvent>? LobbySoundtrackChanged
	{
		add
		{
			if (value != null)
			{
				if (_lobbySoundtrackInfo != null)
				{
					value(new LobbySoundtrackChangedEvent(_lobbySoundtrackInfo.Filename));
				}
				_lobbySoundtrackChanged = (Action<LobbySoundtrackChangedEvent>)Delegate.Combine(_lobbySoundtrackChanged, value);
			}
		}
		remove
		{
			_lobbySoundtrackChanged = (Action<LobbySoundtrackChangedEvent>)Delegate.Remove(_lobbySoundtrackChanged, value);
		}
	}

	private void InitializeAmbientMusic()
	{
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.AmbientMusicVolume, (Action<float>)AmbienceCVarChanged, true);
		_sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("audio.ambience");
		_nextAudio = TimeSpan.MaxValue;
		SetupAmbientSounds();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnProtoReload, (Type[])null, (Type[])null);
		_state.OnStateChanged += OnStateChange;
		((EntitySystem)this).SubscribeNetworkEvent<RoundEndMessageEvent>((EntityEventHandler<RoundEndMessageEvent>)OnRoundEndMessage, (Type[])null, (Type[])null);
	}

	private void AmbienceCVarChanged(float obj)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_volumeSlider = SharedAudioSystem.GainToVolume(obj);
		if (_ambientMusicStream.HasValue && _musicProto != null)
		{
			SharedAudioSystem audio = _audio;
			EntityUid? ambientMusicStream = _ambientMusicStream;
			AudioParams val = _musicProto.Sound.Params;
			audio.SetVolume(ambientMusicStream, ((AudioParams)(ref val)).Volume + _volumeSlider, (AudioComponent)null);
		}
	}

	private void ShutdownAmbientMusic()
	{
		_state.OnStateChanged -= OnStateChange;
		_ambientMusicStream = _audio.Stop(_ambientMusicStream, (AudioComponent)null);
	}

	private void OnProtoReload(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<AmbientMusicPrototype>() || obj.WasModified<RulesPrototype>())
		{
			SetupAmbientSounds();
		}
	}

	private void OnStateChange(StateChangedEventArgs obj)
	{
		if (obj.NewState is GameplayState)
		{
			_nextAudio = _timing.CurTime + _random.Next(_minAmbienceTime, _maxAmbienceTime);
		}
	}

	private void SetupAmbientSounds()
	{
		_ambientSounds.Clear();
		foreach (AmbientMusicPrototype item in _proto.EnumeratePrototypes<AmbientMusicPrototype>())
		{
			List<ResPath> orNew = Extensions.GetOrNew<string, List<ResPath>>(_ambientSounds, item.ID);
			RefreshTracks(item.Sound, orNew, null);
			_random.Shuffle<ResPath>((IList<ResPath>)orNew);
		}
	}

	private void OnRoundEndMessage(RoundEndMessageEvent ev)
	{
		_ambientMusicStream = _audio.Stop(_ambientMusicStream, (AudioComponent)null);
		_nextAudio = TimeSpan.FromMinutes(3L);
	}

	private void RefreshTracks(SoundSpecifier sound, List<ResPath> tracks, ResPath? lastPlayed)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		SoundCollectionSpecifier val = (SoundCollectionSpecifier)(object)((sound is SoundCollectionSpecifier) ? sound : null);
		if (val == null)
		{
			SoundPathSpecifier val2 = (SoundPathSpecifier)(object)((sound is SoundPathSpecifier) ? sound : null);
			if (val2 != null)
			{
				tracks.Add(val2.Path);
			}
		}
		else if (val.Collection != null)
		{
			SoundCollectionPrototype val3 = _proto.Index<SoundCollectionPrototype>(val.Collection);
			tracks.AddRange(val3.PickFiles);
		}
		if (tracks.Count > 1)
		{
			ResPath val4 = tracks[tracks.Count - 1];
			ResPath? val5 = lastPlayed;
			if (val5.HasValue && val4 == val5.GetValueOrDefault())
			{
				int index = tracks.Count - 1;
				val4 = tracks[tracks.Count - 1];
				ResPath value = tracks[0];
				tracks[0] = val4;
				tracks[index] = value;
			}
		}
	}

	private void UpdateAmbientMusic()
	{
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if (!(_state.CurrentState is GameplayState))
		{
			_ambientMusicStream = Audio.Stop(_ambientMusicStream, (AudioComponent)null);
			_musicProto = null;
			return;
		}
		bool? flag = null;
		AudioComponent val = default(AudioComponent);
		if (((EntitySystem)this).TryComp<AudioComponent>(_ambientMusicStream, ref val))
		{
			flag = !val.Playing;
		}
		if (_interruptable)
		{
			ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
			EntityUid? val2 = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
			if (!val2.HasValue || _musicProto == null || !_rules.IsTrue(val2.Value, _proto.Index<RulesPrototype>(_musicProto.Rules)))
			{
				FadeOut(_ambientMusicStream, null, 10f);
				_musicProto = null;
				_interruptable = false;
				flag = true;
			}
		}
		if (flag == false)
		{
			return;
		}
		if (flag == true)
		{
			_nextAudio = _timing.CurTime + _random.Next(_minAmbienceTime, _maxAmbienceTime);
		}
		_ambientMusicStream = null;
		if (_nextAudio > _timing.CurTime)
		{
			return;
		}
		_musicProto = GetAmbience();
		if (_musicProto == null)
		{
			_interruptable = false;
			return;
		}
		_interruptable = _musicProto.Interruptable;
		List<ResPath> list = _ambientSounds[_musicProto.ID];
		ResPath val3 = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		SharedAudioSystem audio = _audio;
		ResolvedPathSpecifier val4 = new ResolvedPathSpecifier(val3);
		Filter obj = Filter.Local();
		AudioParams val5 = _musicProto.Sound.Params;
		(EntityUid, AudioComponent)? tuple = audio.PlayGlobal((ResolvedSoundSpecifier)val4, obj, false, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(((AudioParams)(ref val5)).Volume + _volumeSlider));
		_ambientMusicStream = tuple?.Item1;
		if (_musicProto.FadeIn && tuple.HasValue)
		{
			FadeIn(_ambientMusicStream, tuple.Value.Item2, 10f);
		}
		if (list.Count == 0)
		{
			RefreshTracks(_musicProto.Sound, list, val3);
		}
	}

	private AmbientMusicPrototype? GetAmbience()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return null;
		}
		PlayAmbientMusicEvent playAmbientMusicEvent = default(PlayAmbientMusicEvent);
		((EntitySystem)this).RaiseLocalEvent<PlayAmbientMusicEvent>(ref playAmbientMusicEvent);
		if (playAmbientMusicEvent.Cancelled)
		{
			return null;
		}
		List<AmbientMusicPrototype> list = _proto.EnumeratePrototypes<AmbientMusicPrototype>().ToList();
		list.Sort((AmbientMusicPrototype x, AmbientMusicPrototype y) => y.Priority.CompareTo(x.Priority));
		foreach (AmbientMusicPrototype item in list)
		{
			if (_rules.IsTrue(localEntity.Value, _proto.Index<RulesPrototype>(item.Rules)))
			{
				return item;
			}
		}
		_sawmill.Warning("Unable to find fallback ambience track");
		return null;
	}

	public void DisableAmbientMusic()
	{
		FadeOut(_ambientMusicStream);
		_ambientMusicStream = null;
	}

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		InitializeAmbientMusic();
		InitializeLobbyMusic();
		((EntitySystem)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundCleanup, (Type[])null, (Type[])null);
	}

	private void OnRoundCleanup(RoundRestartCleanupEvent ev)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_fadingOut.Clear();
		EntityUid? val = _lobbySoundtrackInfo?.MusicStreamEntityUid;
		AudioComponent val2 = default(AudioComponent);
		((EntitySystem)this).TryComp<AudioComponent>(val, ref val2);
		float? num = ((val2 != null) ? new float?(val2.Gain) : ((float?)null));
		EntityUid? lobbyRoundRestartAudioStream = _lobbyRoundRestartAudioStream;
		AudioComponent val3 = default(AudioComponent);
		((EntitySystem)this).TryComp<AudioComponent>(lobbyRoundRestartAudioStream, ref val3);
		float? num2 = ((val3 != null) ? new float?(val3.Gain) : ((float?)null));
		SilenceAudio();
		if (num.HasValue)
		{
			Audio.SetGain(val, num.Value, val2);
		}
		if (num2.HasValue)
		{
			Audio.SetGain(lobbyRoundRestartAudioStream, num2.Value, val3);
		}
		PlayRestartSound(ev);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ShutdownAmbientMusic();
		ShutdownLobbyMusic();
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_timing.IsFirstTimePredicted)
		{
			UpdateAmbientMusic();
			UpdateLobbyMusic();
			UpdateFades(frameTime);
		}
	}

	public void FadeOut(EntityUid? stream, AudioComponent? component = null, float duration = 2f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (stream.HasValue && !(duration <= 0f) && ((EntitySystem)this).Resolve<AudioComponent>(stream.Value, ref component, true))
		{
			_fadingIn.Remove(stream.Value);
			float num = component.Volume - -32f;
			_fadingOut.Add(stream.Value, num / duration);
		}
	}

	public void FadeIn(EntityUid? stream, AudioComponent? component = null, float duration = 2f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (stream.HasValue && !(duration <= 0f) && ((EntitySystem)this).Resolve<AudioComponent>(stream.Value, ref component, true) && !(component.Volume < -32f))
		{
			_fadingOut.Remove(stream.Value);
			float volume = component.Volume;
			float item = (-32f - volume) / duration;
			_fadingIn.Add(stream.Value, (item, component.Volume));
			component.Volume = -32f;
		}
	}

	private void UpdateFades(float frameTime)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		_fadeToRemove.Clear();
		EntityUid key;
		AudioComponent val2 = default(AudioComponent);
		foreach (KeyValuePair<EntityUid, float> item in _fadingOut)
		{
			item.Deconstruct(out key, out var value);
			EntityUid val = key;
			float num = value;
			if (!((EntitySystem)this).TryComp<AudioComponent>(val, ref val2))
			{
				_fadeToRemove.Add(val);
				continue;
			}
			float y = val2.Volume - num * frameTime;
			y = MathF.Max(-32f, y);
			_audio.SetVolume((EntityUid?)val, y, val2);
			value = val2.Volume;
			if (value.Equals(-32f))
			{
				_audio.Stop((EntityUid?)val, (AudioComponent)null);
				_fadeToRemove.Add(val);
			}
		}
		foreach (EntityUid item2 in _fadeToRemove)
		{
			_fadingOut.Remove(item2);
		}
		_fadeToRemove.Clear();
		AudioComponent val4 = default(AudioComponent);
		foreach (KeyValuePair<EntityUid, (float, float)> item3 in _fadingIn)
		{
			item3.Deconstruct(out key, out var value2);
			(float, float) tuple = value2;
			EntityUid val3 = key;
			var (num2, num3) = tuple;
			if (!((EntitySystem)this).TryComp<AudioComponent>(val3, ref val4))
			{
				_fadeToRemove.Add(val3);
				continue;
			}
			float y2 = val4.Volume - num2 * frameTime;
			y2 = MathF.Min(num3, y2);
			_audio.SetVolume((EntityUid?)val3, y2, val4);
			if (val4.Volume.Equals(num3))
			{
				_fadeToRemove.Add(val3);
			}
		}
		foreach (EntityUid item4 in _fadeToRemove)
		{
			_fadingIn.Remove(item4);
		}
	}

	private void InitializeLobbyMusic()
	{
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configManager, CCVars.LobbyMusicEnabled, (Action<bool>)LobbyMusicCVarChanged, false);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.LobbyMusicVolume, (Action<float>)LobbyMusicVolumeCVarChanged, false);
		_state.OnStateChanged += StateManagerOnStateChanged;
		_client.PlayerLeaveServer += OnLeave;
		((EntitySystem)this).SubscribeNetworkEvent<LobbyMusicStopEvent>((EntityEventHandler<LobbyMusicStopEvent>)OnLobbySongStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<LobbyPlaylistChangedEvent>((EntityEventHandler<LobbyPlaylistChangedEvent>)OnLobbySongChanged, (Type[])null, (Type[])null);
	}

	private void OnLobbySongStopped(LobbyMusicStopEvent ev)
	{
		EndLobbyMusic();
	}

	private void StateManagerOnStateChanged(StateChangedEventArgs args)
	{
		State newState = args.NewState;
		if (newState is LobbyState || newState is PubgPreLobbyHubState || newState is CivLobbyState)
		{
			StartLobbyMusic();
		}
		else
		{
			EndLobbyMusic();
		}
	}

	private void OnLeave(object? sender, PlayerEventArgs args)
	{
		EndLobbyMusic();
	}

	private void LobbyMusicVolumeCVarChanged(float volume)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (_lobbySoundtrackInfo != null)
		{
			SharedAudioSystem audio = _audio;
			EntityUid? val = _lobbySoundtrackInfo.MusicStreamEntityUid;
			AudioParams lobbySoundtrackParams = _lobbySoundtrackParams;
			audio.SetVolume(val, ((AudioParams)(ref lobbySoundtrackParams)).Volume + SharedAudioSystem.GainToVolume(_configManager.GetCVar<float>(CCVars.LobbyMusicVolume)), (AudioComponent)null);
		}
	}

	private void LobbyMusicCVarChanged(bool musicEnabled)
	{
		if (musicEnabled && (_state.CurrentState is LobbyState || _state.CurrentState is PubgPreLobbyHubState || _state.CurrentState is CivLobbyState))
		{
			StartLobbyMusic();
		}
		else
		{
			EndLobbyMusic();
		}
	}

	private void OnLobbySongChanged(LobbyPlaylistChangedEvent playlistChangedEvent)
	{
		string[] playlist = playlistChangedEvent.Playlist;
		if (!(_lobbySoundtrackInfo != null) || _lobbyPlaylist == null || !Enumerable.SequenceEqual(_lobbyPlaylist, playlist))
		{
			EndLobbyMusic();
			StartLobbyMusic(playlistChangedEvent.Playlist);
		}
	}

	private void StartLobbyMusic()
	{
		if (_lobbyPlaylist != null && _lobbyPlaylist.Length != 0)
		{
			StartLobbyMusic(_lobbyPlaylist);
		}
	}

	private void StartLobbyMusic(string[] playlist)
	{
		if (!(_lobbySoundtrackInfo != null) && _configManager.GetCVar<bool>(CCVars.LobbyMusicEnabled))
		{
			_lobbyPlaylist = playlist;
			if (_lobbyPlaylist.Length != 0)
			{
				PlaySoundtrack(playlist[0]);
			}
		}
	}

	private void PlaySoundtrack(string soundtrackFilename)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		AudioResource val = default(AudioResource);
		if (_resourceCache.TryGetResource<AudioResource>(new ResPath(soundtrackFilename), ref val))
		{
			SharedAudioSystem audio = _audio;
			ResolvedPathSpecifier val2 = new ResolvedPathSpecifier(soundtrackFilename);
			Filter obj = Filter.Local();
			ref readonly AudioParams lobbySoundtrackParams = ref _lobbySoundtrackParams;
			AudioParams lobbySoundtrackParams2 = _lobbySoundtrackParams;
			(EntityUid, AudioComponent)? tuple = audio.PlayGlobal((ResolvedSoundSpecifier)val2, obj, false, (AudioParams?)((AudioParams)(ref lobbySoundtrackParams)).WithVolume(((AudioParams)(ref lobbySoundtrackParams2)).Volume + SharedAudioSystem.GainToVolume(_configManager.GetCVar<float>(CCVars.LobbyMusicVolume))));
			if (!tuple.HasValue)
			{
				_sawmill.Warning("Tried to play lobby soundtrack '{Filename}' using SharedAudioSystem.PlayGlobal but it returned default value of EntityUid!", new object[1] { soundtrackFilename });
				return;
			}
			TimeSpan nextTrackOn = _timing.CurTime + val.AudioStream.Length;
			_lobbySoundtrackInfo = new LobbySoundtrackInfo(soundtrackFilename, nextTrackOn, tuple.Value.Item1);
			LobbySoundtrackChangedEvent obj2 = new LobbySoundtrackChangedEvent(soundtrackFilename);
			_lobbySoundtrackChanged?.Invoke(obj2);
		}
	}

	private void EndLobbyMusic()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!(_lobbySoundtrackInfo == null))
		{
			_audio.Stop((EntityUid?)_lobbySoundtrackInfo.MusicStreamEntityUid, (AudioComponent)null);
			_lobbySoundtrackInfo = null;
			LobbySoundtrackChangedEvent obj = new LobbySoundtrackChangedEvent();
			_lobbySoundtrackChanged?.Invoke(obj);
		}
	}

	private void PlayRestartSound(RoundRestartCleanupEvent ev)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (_configManager.GetCVar<bool>(CCVars.RestartSoundsEnabled))
		{
			ResolvedSoundSpecifier restartSound = _gameTicker.RestartSound;
			if (!ResolvedSoundSpecifier.IsNullOrEmpty(restartSound))
			{
				SharedAudioSystem audio = _audio;
				Filter obj = Filter.Local();
				ref readonly AudioParams roundEndSoundEffectParams = ref _roundEndSoundEffectParams;
				AudioParams roundEndSoundEffectParams2 = _roundEndSoundEffectParams;
				_lobbyRoundRestartAudioStream = audio.PlayGlobal(restartSound, obj, false, (AudioParams?)((AudioParams)(ref roundEndSoundEffectParams)).WithVolume(((AudioParams)(ref roundEndSoundEffectParams2)).Volume + SharedAudioSystem.GainToVolume(_configManager.GetCVar<float>(CCVars.LobbyMusicVolume))))?.Item1;
			}
		}
	}

	private void ShutdownLobbyMusic()
	{
		_state.OnStateChanged -= StateManagerOnStateChanged;
		_client.PlayerLeaveServer -= OnLeave;
		EndLobbyMusic();
	}

	private void UpdateLobbyMusic()
	{
		if (_lobbySoundtrackInfo != null && _timing.CurTime >= _lobbySoundtrackInfo.NextTrackOn)
		{
			string[]? lobbyPlaylist = _lobbyPlaylist;
			if (lobbyPlaylist != null && lobbyPlaylist.Length != 0)
			{
				string nextSoundtrackFromPlaylist = GetNextSoundtrackFromPlaylist(_lobbySoundtrackInfo.Filename, _lobbyPlaylist);
				PlaySoundtrack(nextSoundtrackFromPlaylist);
			}
		}
	}

	private static string GetNextSoundtrackFromPlaylist(string currentSoundtrackFilename, string[] playlist)
	{
		int num = Array.IndexOf(playlist, currentSoundtrackFilename) + 1;
		if (num > playlist.Length - 1)
		{
			num = 0;
		}
		return playlist[num];
	}
}
