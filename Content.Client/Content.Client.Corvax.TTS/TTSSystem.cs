using System;
using System.Collections.Generic;
using Content.Shared.Corvax.CCCVars;
using Content.Shared.Corvax.TTS;
using Content.Shared.DeadSpace.CCCCVars;
using Robust.Client.Audio;
using Robust.Client.ResourceManagement;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Client.Corvax.TTS;

public sealed class TTSSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IResourceManager _res;

	[Dependency]
	private AudioSystem _audio;

	private ISawmill _sawmill;

	private static readonly MemoryContentRoot _contentRoot = new MemoryContentRoot();

	private static bool _rootAdded;

	private static readonly ResPath Prefix = ResPath.Root / "TTS";

	private const float WhisperFade = 3f;

	private const float MinimalVolume = -6f;

	private float _volume;

	private float _volumeRadio;

	private float _voiceRange = 10f;

	private float _whisperRange = 5f;

	private bool _playRadio = true;

	private int _fileIdx;

	private bool _ttsOptEnabled = true;

	private int _maxConcurrent = 5;

	private readonly List<EntityUid> _playingTts = new List<EntityUid>();

	public override void Initialize()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		_sawmill = Logger.GetSawmill("tts");
		if (!_rootAdded)
		{
			_res.AddRoot(Prefix, (IContentRoot)(object)_contentRoot);
			_rootAdded = true;
		}
		_cfg.OnValueChanged<float>(CCCVars.TTSVolume, (Action<float>)OnTtsVolumeChanged, true);
		_cfg.OnValueChanged<float>(CCCVars.TTSVoiceRange, (Action<float>)OnTtsVoiceRangeChanged, true);
		_cfg.OnValueChanged<float>(CCCVars.TTSWhisperRange, (Action<float>)OnTtsWhisperRangeChanged, true);
		_cfg.OnValueChanged<int>(CCCVars.TTSMaxConcurrent, (Action<int>)OnTtsMaxConcurrentChanged, true);
		_cfg.OnValueChanged<float>(CCCCVars.TTSVolumeRadio, (Action<float>)OnTtsRadioVolumeChanged, true);
		_cfg.OnValueChanged<bool>(CCCCVars.RadioTTSSoundsEnabled, (Action<bool>)OnTtsPlayRadioChanged, true);
		((EntitySystem)this).SubscribeNetworkEvent<PlayTTSEvent>((EntityEventHandler<PlayTTSEvent>)OnPlayTTS, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_cfg.UnsubValueChanged<float>(CCCVars.TTSVolume, (Action<float>)OnTtsVolumeChanged);
		_cfg.UnsubValueChanged<float>(CCCVars.TTSVoiceRange, (Action<float>)OnTtsVoiceRangeChanged);
		_cfg.UnsubValueChanged<float>(CCCVars.TTSWhisperRange, (Action<float>)OnTtsWhisperRangeChanged);
		_cfg.UnsubValueChanged<int>(CCCVars.TTSMaxConcurrent, (Action<int>)OnTtsMaxConcurrentChanged);
		_cfg.UnsubValueChanged<float>(CCCCVars.TTSVolumeRadio, (Action<float>)OnTtsRadioVolumeChanged);
		_cfg.UnsubValueChanged<bool>(CCCCVars.RadioTTSSoundsEnabled, (Action<bool>)OnTtsPlayRadioChanged);
		_contentRoot.Clear();
		_playingTts.Clear();
	}

	public void RequestPreviewTTS(string voiceId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestPreviewTTSEvent(voiceId));
	}

	private void OnTtsVolumeChanged(float volume)
	{
		_volume = volume;
		UpdateTtsOption();
	}

	private void OnTtsVoiceRangeChanged(float range)
	{
		_voiceRange = range;
	}

	private void OnTtsWhisperRangeChanged(float range)
	{
		_whisperRange = range;
	}

	private void OnTtsRadioVolumeChanged(float volume)
	{
		_volumeRadio = volume;
		UpdateTtsOption();
	}

	private void OnTtsPlayRadioChanged(bool radio)
	{
		_playRadio = radio;
		UpdateTtsOption();
	}

	private void UpdateTtsOption()
	{
		bool flag = _volume > 0f || (_playRadio && _volumeRadio > 0f);
		if (flag != _ttsOptEnabled)
		{
			_ttsOptEnabled = flag;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new ClientOptionTTSEvent(flag));
		}
	}

	private void OnTtsMaxConcurrentChanged(int value)
	{
		_maxConcurrent = value;
	}

	private void OnPlayTTS(PlayTTSEvent ev)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		if (ev.IsRadio && !_playRadio)
		{
			return;
		}
		byte[] data = ev.Data;
		if (data == null || data.Length <= 0)
		{
			_sawmill.Warning("TTS event has no audio data");
			return;
		}
		_playingTts.RemoveAll((EntityUid e) => ((EntitySystem)this).Deleted(e, (MetaDataComponent)null));
		if (_playingTts.Count >= _maxConcurrent)
		{
			return;
		}
		EntityUid? val = null;
		if (ev.SourceUid.HasValue)
		{
			EntityUid? val2 = default(EntityUid?);
			if (!((EntitySystem)this).TryGetEntity(ev.SourceUid.Value, ref val2))
			{
				return;
			}
			val = val2;
		}
		_sawmill.Verbose($"Play TTS audio {ev.Data.Length} bytes from {ev.SourceUid} entity");
		AudioParams val3 = ((AudioParams)(ref AudioParams.Default)).WithVolume(AdjustVolume(ev.IsWhisper, ev.IsRadio));
		AudioParams value = ((AudioParams)(ref val3)).WithMaxDistance(AdjustDistance(ev.IsWhisper));
		ResPath val4 = default(ResPath);
		((ResPath)(ref val4))._002Ector($"{_fileIdx++}.ogg");
		_contentRoot.AddOrUpdateFile(val4, ev.Data);
		AudioResource val5 = new AudioResource();
		((BaseResource)val5).Load(IoCManager.Instance, Prefix / val4);
		ResolvedPathSpecifier val6 = new ResolvedPathSpecifier(Prefix / val4);
		(EntityUid, AudioComponent)? tuple = (val.HasValue ? _audio.PlayEntity(val5.AudioStream, val.Value, (ResolvedSoundSpecifier)(object)val6, (AudioParams?)value) : _audio.PlayGlobal(val5.AudioStream, (ResolvedSoundSpecifier)(object)val6, (AudioParams?)value));
		if (tuple.HasValue)
		{
			_playingTts.Add(tuple.Value.Item1);
		}
		_contentRoot.RemoveFile(val4);
	}

	private float AdjustVolume(bool isWhisper, bool isRadio)
	{
		float num = -6f + SharedAudioSystem.GainToVolume(_volume);
		if (isWhisper && !isRadio)
		{
			num -= SharedAudioSystem.GainToVolume(3f);
		}
		else if (isRadio)
		{
			num = -6f + SharedAudioSystem.GainToVolume(_volumeRadio);
		}
		return num;
	}

	private float AdjustDistance(bool isWhisper)
	{
		if (!isWhisper)
		{
			return _voiceRange;
		}
		return _whisperRange;
	}
}
