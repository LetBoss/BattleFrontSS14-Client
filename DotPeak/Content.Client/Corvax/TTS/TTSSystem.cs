// Decompiled with JetBrains decompiler
// Type: Content.Client.Corvax.TTS.TTSSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Corvax.TTS;
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
using System;
using System.Collections.Generic;

#nullable enable
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
  private static readonly ResPath Prefix = ResPath.op_Division(ResPath.Root, "TTS");
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

  public virtual void Initialize()
  {
    this._sawmill = Logger.GetSawmill("tts");
    if (!TTSSystem._rootAdded)
    {
      this._res.AddRoot(TTSSystem.Prefix, (IContentRoot) TTSSystem._contentRoot);
      TTSSystem._rootAdded = true;
    }
    this._cfg.OnValueChanged<float>(Content.Shared.Corvax.CCCVars.CCCVars.TTSVolume, new Action<float>(this.OnTtsVolumeChanged), true);
    this._cfg.OnValueChanged<float>(Content.Shared.Corvax.CCCVars.CCCVars.TTSVoiceRange, new Action<float>(this.OnTtsVoiceRangeChanged), true);
    this._cfg.OnValueChanged<float>(Content.Shared.Corvax.CCCVars.CCCVars.TTSWhisperRange, new Action<float>(this.OnTtsWhisperRangeChanged), true);
    this._cfg.OnValueChanged<int>(Content.Shared.Corvax.CCCVars.CCCVars.TTSMaxConcurrent, new Action<int>(this.OnTtsMaxConcurrentChanged), true);
    this._cfg.OnValueChanged<float>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.TTSVolumeRadio, new Action<float>(this.OnTtsRadioVolumeChanged), true);
    this._cfg.OnValueChanged<bool>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.RadioTTSSoundsEnabled, new Action<bool>(this.OnTtsPlayRadioChanged), true);
    this.SubscribeNetworkEvent<PlayTTSEvent>(new EntityEventHandler<PlayTTSEvent>(this.OnPlayTTS), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._cfg.UnsubValueChanged<float>(Content.Shared.Corvax.CCCVars.CCCVars.TTSVolume, new Action<float>(this.OnTtsVolumeChanged));
    this._cfg.UnsubValueChanged<float>(Content.Shared.Corvax.CCCVars.CCCVars.TTSVoiceRange, new Action<float>(this.OnTtsVoiceRangeChanged));
    this._cfg.UnsubValueChanged<float>(Content.Shared.Corvax.CCCVars.CCCVars.TTSWhisperRange, new Action<float>(this.OnTtsWhisperRangeChanged));
    this._cfg.UnsubValueChanged<int>(Content.Shared.Corvax.CCCVars.CCCVars.TTSMaxConcurrent, new Action<int>(this.OnTtsMaxConcurrentChanged));
    this._cfg.UnsubValueChanged<float>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.TTSVolumeRadio, new Action<float>(this.OnTtsRadioVolumeChanged));
    this._cfg.UnsubValueChanged<bool>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.RadioTTSSoundsEnabled, new Action<bool>(this.OnTtsPlayRadioChanged));
    TTSSystem._contentRoot.Clear();
    this._playingTts.Clear();
  }

  public void RequestPreviewTTS(string voiceId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new RequestPreviewTTSEvent(voiceId));
  }

  private void OnTtsVolumeChanged(float volume)
  {
    this._volume = volume;
    this.UpdateTtsOption();
  }

  private void OnTtsVoiceRangeChanged(float range) => this._voiceRange = range;

  private void OnTtsWhisperRangeChanged(float range) => this._whisperRange = range;

  private void OnTtsRadioVolumeChanged(float volume)
  {
    this._volumeRadio = volume;
    this.UpdateTtsOption();
  }

  private void OnTtsPlayRadioChanged(bool radio)
  {
    this._playRadio = radio;
    this.UpdateTtsOption();
  }

  private void UpdateTtsOption()
  {
    bool enabled = (double) this._volume > 0.0 || this._playRadio && (double) this._volumeRadio > 0.0;
    if (enabled == this._ttsOptEnabled)
      return;
    this._ttsOptEnabled = enabled;
    this.RaiseNetworkEvent((EntityEventArgs) new ClientOptionTTSEvent(enabled));
  }

  private void OnTtsMaxConcurrentChanged(int value) => this._maxConcurrent = value;

  private void OnPlayTTS(PlayTTSEvent ev)
  {
    if (ev.IsRadio && !this._playRadio)
      return;
    byte[] data = ev.Data;
    if ((data == null ? 0 : (data.Length > 0 ? 1 : 0)) == 0)
    {
      this._sawmill.Warning("TTS event has no audio data");
    }
    else
    {
      this._playingTts.RemoveAll((Predicate<EntityUid>) (e => this.Deleted(e, (MetaDataComponent) null)));
      if (this._playingTts.Count >= this._maxConcurrent)
        return;
      EntityUid? nullable1 = new EntityUid?();
      NetEntity? sourceUid = ev.SourceUid;
      if (sourceUid.HasValue)
      {
        sourceUid = ev.SourceUid;
        EntityUid? nullable2;
        if (!this.TryGetEntity(sourceUid.Value, ref nullable2))
          return;
        nullable1 = nullable2;
      }
      this._sawmill.Verbose($"Play TTS audio {ev.Data.Length} bytes from {ev.SourceUid} entity");
      AudioParams audioParams1 = ((AudioParams) ref AudioParams.Default).WithVolume(this.AdjustVolume(ev.IsWhisper, ev.IsRadio));
      AudioParams audioParams2 = ((AudioParams) ref audioParams1).WithMaxDistance(this.AdjustDistance(ev.IsWhisper));
      ResPath resPath;
      // ISSUE: explicit constructor call
      ((ResPath) ref resPath).\u002Ector($"{this._fileIdx++}.ogg");
      TTSSystem._contentRoot.AddOrUpdateFile(resPath, ev.Data);
      AudioResource audioResource = new AudioResource();
      ((BaseResource) audioResource).Load(IoCManager.Instance, ResPath.op_Division(TTSSystem.Prefix, resPath));
      ResolvedPathSpecifier resolvedPathSpecifier = new ResolvedPathSpecifier(ResPath.op_Division(TTSSystem.Prefix, resPath));
      (EntityUid, AudioComponent)? nullable3 = nullable1.HasValue ? this._audio.PlayEntity(audioResource.AudioStream, nullable1.Value, (ResolvedSoundSpecifier) resolvedPathSpecifier, new AudioParams?(audioParams2)) : this._audio.PlayGlobal(audioResource.AudioStream, (ResolvedSoundSpecifier) resolvedPathSpecifier, new AudioParams?(audioParams2));
      if (nullable3.HasValue)
        this._playingTts.Add(nullable3.Value.Item1);
      TTSSystem._contentRoot.RemoveFile(resPath);
    }
  }

  private float AdjustVolume(bool isWhisper, bool isRadio)
  {
    float num = SharedAudioSystem.GainToVolume(this._volume) - 6f;
    if (isWhisper && !isRadio)
      num -= SharedAudioSystem.GainToVolume(3f);
    else if (isRadio)
      num = SharedAudioSystem.GainToVolume(this._volumeRadio) - 6f;
    return num;
  }

  private float AdjustDistance(bool isWhisper)
  {
    return !isWhisper ? this._voiceRange : this._whisperRange;
  }
}
