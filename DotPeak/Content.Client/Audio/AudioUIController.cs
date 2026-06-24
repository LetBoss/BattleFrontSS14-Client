// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.AudioUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Client.Audio;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Audio.Sources;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Audio;

public sealed class AudioUIController : UIController
{
  [Dependency]
  private IAudioManager _audioManager;
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private IResourceCache _cache;
  private float _interfaceGain;
  private IAudioSource? _clickSource;
  private IAudioSource? _hoverSource;
  private const float ClickGain = 0.25f;
  private const float HoverGain = 0.05f;

  public virtual void Initialize()
  {
    base.Initialize();
    this._configManager.OnValueChanged<string>(CCVars.UIClickSound, new Action<string>(this.SetClickSound), true);
    this._configManager.OnValueChanged<string>(CCVars.UIHoverSound, new Action<string>(this.SetHoverSound), true);
    this._configManager.OnValueChanged<float>(CCVars.InterfaceVolume, new Action<float>(this.SetInterfaceVolume), true);
  }

  private void SetInterfaceVolume(float obj)
  {
    this._interfaceGain = obj;
    if (this._clickSource != null)
      this._clickSource.Gain = 0.25f * this._interfaceGain;
    if (this._hoverSource == null)
      return;
    this._hoverSource.Gain = 0.05f * this._interfaceGain;
  }

  private void SetClickSound(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      IAudioSource audioSource = this._audioManager.CreateAudioSource(AudioResource.op_Implicit(this.GetSoundOrFallback(value, CCVars.UIClickSound.DefaultValue)));
      if (audioSource != null)
      {
        audioSource.Gain = 0.25f * this._interfaceGain;
        audioSource.Global = true;
      }
      this._clickSource = audioSource;
      this.UIManager.SetClickSound(audioSource);
    }
    else
      this.UIManager.SetClickSound((IAudioSource) null);
  }

  private void SetHoverSound(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      IAudioSource audioSource = this._audioManager.CreateAudioSource(AudioResource.op_Implicit(this.GetSoundOrFallback(value, CCVars.UIHoverSound.DefaultValue)));
      if (audioSource != null)
      {
        audioSource.Gain = 0.05f * this._interfaceGain;
        audioSource.Global = true;
      }
      this._hoverSource = audioSource;
      this.UIManager.SetHoverSound(audioSource);
    }
    else
      this.UIManager.SetHoverSound((IAudioSource) null);
  }

  private AudioResource GetSoundOrFallback(string path, string fallback)
  {
    AudioResource audioResource;
    return !this._cache.TryGetResource<AudioResource>(path, ref audioResource) ? this._cache.GetResource<AudioResource>(fallback, true) : audioResource;
  }
}
