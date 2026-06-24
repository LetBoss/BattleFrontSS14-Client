using System;
using System.Numerics;
using Robust.Shared.Audio.Effects;

namespace Robust.Shared.Audio.Sources;

public interface IAudioSource : IDisposable
{
	bool Playing { get; set; }

	bool Looping { get; set; }

	bool Global { get; set; }

	Vector2 Position { get; set; }

	float Pitch { get; set; }

	float Volume { get; set; }

	float Gain { get; set; }

	float MaxDistance { get; set; }

	float RolloffFactor { get; set; }

	float ReferenceDistance { get; set; }

	float Occlusion { get; set; }

	float PlaybackPosition { get; set; }

	Vector2 Velocity { get; set; }

	void Pause();

	void StartPlaying();

	void StopPlaying();

	void Restart();

	void SetAuxiliary(IAuxiliaryAudio? audio);
}
