using System;
using Content.Shared.CCVar;
using Robust.Client.Audio;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Audio.Sources;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

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

	public override void Initialize()
	{
		((UIController)this).Initialize();
		_configManager.OnValueChanged<string>(CCVars.UIClickSound, (Action<string>)SetClickSound, true);
		_configManager.OnValueChanged<string>(CCVars.UIHoverSound, (Action<string>)SetHoverSound, true);
		_configManager.OnValueChanged<float>(CCVars.InterfaceVolume, (Action<float>)SetInterfaceVolume, true);
	}

	private void SetInterfaceVolume(float obj)
	{
		_interfaceGain = obj;
		if (_clickSource != null)
		{
			_clickSource.Gain = 0.25f * _interfaceGain;
		}
		if (_hoverSource != null)
		{
			_hoverSource.Gain = 0.05f * _interfaceGain;
		}
	}

	private void SetClickSound(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			AudioResource soundOrFallback = GetSoundOrFallback(value, CCVars.UIClickSound.DefaultValue);
			IAudioSource val = _audioManager.CreateAudioSource(AudioResource.op_Implicit(soundOrFallback));
			if (val != null)
			{
				val.Gain = 0.25f * _interfaceGain;
				val.Global = true;
			}
			_clickSource = val;
			base.UIManager.SetClickSound(val);
		}
		else
		{
			base.UIManager.SetClickSound((IAudioSource)null);
		}
	}

	private void SetHoverSound(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			AudioResource soundOrFallback = GetSoundOrFallback(value, CCVars.UIHoverSound.DefaultValue);
			IAudioSource val = _audioManager.CreateAudioSource(AudioResource.op_Implicit(soundOrFallback));
			if (val != null)
			{
				val.Gain = 0.05f * _interfaceGain;
				val.Global = true;
			}
			_hoverSource = val;
			base.UIManager.SetHoverSound(val);
		}
		else
		{
			base.UIManager.SetHoverSound((IAudioSource)null);
		}
	}

	private AudioResource GetSoundOrFallback(string path, string fallback)
	{
		AudioResource result = default(AudioResource);
		if (!_cache.TryGetResource<AudioResource>(path, ref result))
		{
			return _cache.GetResource<AudioResource>(fallback, true);
		}
		return result;
	}
}
