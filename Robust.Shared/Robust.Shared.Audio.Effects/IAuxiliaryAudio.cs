using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Audio.Effects;

[NotContentImplementable]
public interface IAuxiliaryAudio : IDisposable
{
	void SetEffect(IAudioEffect? effect);
}
