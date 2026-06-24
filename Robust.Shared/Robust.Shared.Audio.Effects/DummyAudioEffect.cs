using System.Numerics;

namespace Robust.Shared.Audio.Effects;

internal sealed class DummyAudioEffect : IAudioEffect
{
	public float Density { get; set; }

	public float Diffusion { get; set; }

	public float Gain { get; set; }

	public float GainHF { get; set; }

	public float GainLF { get; set; }

	public float DecayTime { get; set; }

	public float DecayHFRatio { get; set; }

	public float DecayLFRatio { get; set; }

	public float ReflectionsGain { get; set; }

	public float ReflectionsDelay { get; set; }

	public Vector3 ReflectionsPan { get; set; }

	public float LateReverbGain { get; set; }

	public float LateReverbDelay { get; set; }

	public Vector3 LateReverbPan { get; set; }

	public float EchoTime { get; set; }

	public float EchoDepth { get; set; }

	public float ModulationTime { get; set; }

	public float ModulationDepth { get; set; }

	public float AirAbsorptionGainHF { get; set; }

	public float HFReference { get; set; }

	public float LFReference { get; set; }

	public float RoomRolloffFactor { get; set; }

	public int DecayHFLimit { get; set; }

	public void Dispose()
	{
	}
}
