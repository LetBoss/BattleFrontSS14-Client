using System.Numerics;

namespace Robust.Shared.Audio.Effects;

public record struct ReverbProperties
{
	public float Density;

	public float Diffusion;

	public float Gain;

	public float GainHF;

	public float GainLF;

	public float DecayTime;

	public float DecayHFRatio;

	public float DecayLFRatio;

	public float ReflectionsGain;

	public float ReflectionsDelay;

	public Vector3 ReflectionsPan;

	public float LateReverbGain;

	public float LateReverbDelay;

	public Vector3 LateReverbPan;

	public float EchoTime;

	public float EchoDepth;

	public float ModulationTime;

	public float ModulationDepth;

	public float AirAbsorptionGainHF;

	public float HFReference;

	public float LFReference;

	public float RoomRolloffFactor;

	public int DecayHFLimit;

	public ReverbProperties(float density, float diffusion, float gain, float gainHF, float gainLF, float decayTime, float decayHFRatio, float decayLFRatio, float reflectionsGain, float reflectionsDelay, Vector3 reflectionsPan, float lateReverbGain, float lateReverbDelay, Vector3 lateReverbPan, float echoTime, float echoDepth, float modulationTime, float modulationDepth, float airAbsorptionGainHF, float hfReference, float lfReference, float roomRolloffFactor, int decayHFLimit)
	{
		Density = density;
		Diffusion = diffusion;
		Gain = gain;
		GainHF = gainHF;
		GainLF = gainLF;
		DecayTime = decayTime;
		DecayHFRatio = decayHFRatio;
		DecayLFRatio = decayLFRatio;
		ReflectionsGain = reflectionsGain;
		ReflectionsDelay = reflectionsDelay;
		ReflectionsPan = reflectionsPan;
		LateReverbGain = lateReverbGain;
		LateReverbDelay = lateReverbDelay;
		LateReverbPan = lateReverbPan;
		EchoTime = echoTime;
		EchoDepth = echoDepth;
		ModulationTime = modulationTime;
		ModulationDepth = modulationDepth;
		AirAbsorptionGainHF = airAbsorptionGainHF;
		HFReference = hfReference;
		LFReference = lfReference;
		RoomRolloffFactor = roomRolloffFactor;
		DecayHFLimit = decayHFLimit;
	}
}
