using System;
using System.Collections.Generic;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Audio;

public sealed class ClientGlobalSoundSystem : SharedGlobalSoundSystem
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private SharedAudioSystem _audio;

	private bool _adminAudioEnabled = true;

	private List<EntityUid?> _adminAudio = new List<EntityUid?>(1);

	private bool _eventAudioEnabled = true;

	private Dictionary<StationEventMusicType, EntityUid?> _eventAudio = new Dictionary<StationEventMusicType, EntityUid?>(1);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<AdminSoundEvent>((EntityEventHandler<AdminSoundEvent>)PlayAdminSound, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _cfg, CCVars.AdminSoundsEnabled, (Action<bool>)ToggleAdminSound, true);
		((EntitySystem)this).SubscribeNetworkEvent<StationEventMusicEvent>((EntityEventHandler<StationEventMusicEvent>)PlayStationEventMusic, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<StopStationEventMusic>((EntityEventHandler<StopStationEventMusic>)StopStationEventMusic, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _cfg, CCVars.EventMusicEnabled, (Action<bool>)ToggleStationEventMusic, true);
		((EntitySystem)this).SubscribeNetworkEvent<GameGlobalSoundEvent>((EntityEventHandler<GameGlobalSoundEvent>)PlayGameSound, (Type[])null, (Type[])null);
	}

	private void OnRoundRestart(RoundRestartCleanupEvent ev)
	{
		ClearAudio();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ClearAudio();
	}

	private void ClearAudio()
	{
		foreach (EntityUid? item in _adminAudio)
		{
			_audio.Stop(item, (AudioComponent)null);
		}
		_adminAudio.Clear();
		foreach (EntityUid? value in _eventAudio.Values)
		{
			_audio.Stop(value, (AudioComponent)null);
		}
		_eventAudio.Clear();
	}

	private void PlayAdminSound(AdminSoundEvent soundEvent)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (_adminAudioEnabled)
		{
			(EntityUid, AudioComponent)? tuple = _audio.PlayGlobal(soundEvent.Specifier, Filter.Local(), false, soundEvent.AudioParams);
			_adminAudio.Add(tuple?.Item1);
		}
	}

	private void PlayStationEventMusic(StationEventMusicEvent soundEvent)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (_eventAudioEnabled && !_eventAudio.ContainsKey(soundEvent.Type))
		{
			(EntityUid, AudioComponent)? tuple = _audio.PlayGlobal(soundEvent.Specifier, Filter.Local(), false, soundEvent.AudioParams);
			_eventAudio.Add(soundEvent.Type, tuple?.Item1);
		}
	}

	private void PlayGameSound(GameGlobalSoundEvent soundEvent)
	{
		_audio.PlayGlobal(soundEvent.Specifier, Filter.Local(), false, soundEvent.AudioParams);
	}

	private void StopStationEventMusic(StopStationEventMusic soundEvent)
	{
		if (_eventAudio.TryGetValue(soundEvent.Type, out var value))
		{
			_audio.Stop(value, (AudioComponent)null);
			_eventAudio.Remove(soundEvent.Type);
		}
	}

	private void ToggleAdminSound(bool enabled)
	{
		_adminAudioEnabled = enabled;
		if (_adminAudioEnabled)
		{
			return;
		}
		foreach (EntityUid? item in _adminAudio)
		{
			_audio.Stop(item, (AudioComponent)null);
		}
		_adminAudio.Clear();
	}

	private void ToggleStationEventMusic(bool enabled)
	{
		_eventAudioEnabled = enabled;
		if (_eventAudioEnabled)
		{
			return;
		}
		foreach (KeyValuePair<StationEventMusicType, EntityUid?> item in _eventAudio)
		{
			_audio.Stop(item.Value, (AudioComponent)null);
		}
		_eventAudio.Clear();
	}
}
