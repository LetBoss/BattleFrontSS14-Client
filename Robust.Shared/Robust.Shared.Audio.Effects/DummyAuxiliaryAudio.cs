using System;

namespace Robust.Shared.Audio.Effects;

internal sealed class DummyAuxiliaryAudio : IAuxiliaryAudio, IDisposable
{
	public void Dispose()
	{
	}

	public void SetEffect(IAudioEffect? effect)
	{
	}
}
