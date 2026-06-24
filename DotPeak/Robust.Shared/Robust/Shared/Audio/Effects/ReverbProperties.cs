// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Effects.ReverbProperties
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Audio.Effects;

public record struct ReverbProperties(
  float density,
  float diffusion,
  float gain,
  float gainHF,
  float gainLF,
  float decayTime,
  float decayHFRatio,
  float decayLFRatio,
  float reflectionsGain,
  float reflectionsDelay,
  Vector3 reflectionsPan,
  float lateReverbGain,
  float lateReverbDelay,
  Vector3 lateReverbPan,
  float echoTime,
  float echoDepth,
  float modulationTime,
  float modulationDepth,
  float airAbsorptionGainHF,
  float hfReference,
  float lfReference,
  float roomRolloffFactor,
  int decayHFLimit)
{
  public float Density = density;
  public float Diffusion = diffusion;
  public float Gain = gain;
  public float GainHF = gainHF;
  public float GainLF = gainLF;
  public float DecayTime = decayTime;
  public float DecayHFRatio = decayHFRatio;
  public float DecayLFRatio = decayLFRatio;
  public float ReflectionsGain = reflectionsGain;
  public float ReflectionsDelay = reflectionsDelay;
  public Vector3 ReflectionsPan = reflectionsPan;
  public float LateReverbGain = lateReverbGain;
  public float LateReverbDelay = lateReverbDelay;
  public Vector3 LateReverbPan = lateReverbPan;
  public float EchoTime = echoTime;
  public float EchoDepth = echoDepth;
  public float ModulationTime = modulationTime;
  public float ModulationDepth = modulationDepth;
  public float AirAbsorptionGainHF = airAbsorptionGainHF;
  public float HFReference = hfReference;
  public float LFReference = lfReference;
  public float RoomRolloffFactor = roomRolloffFactor;
  public int DecayHFLimit = decayHFLimit;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (((((((((((((((((((((EqualityComparer<float>.Default.GetHashCode(this.Density) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Diffusion)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Gain)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.GainHF)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.GainLF)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.DecayTime)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.DecayHFRatio)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.DecayLFRatio)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ReflectionsGain)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ReflectionsDelay)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.ReflectionsPan)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.LateReverbGain)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.LateReverbDelay)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.LateReverbPan)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.EchoTime)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.EchoDepth)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ModulationTime)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ModulationDepth)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.AirAbsorptionGainHF)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.HFReference)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.LFReference)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.RoomRolloffFactor)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.DecayHFLimit);
  }

  [CompilerGenerated]
  public readonly bool Equals(ReverbProperties other)
  {
    return EqualityComparer<float>.Default.Equals(this.Density, other.Density) && EqualityComparer<float>.Default.Equals(this.Diffusion, other.Diffusion) && EqualityComparer<float>.Default.Equals(this.Gain, other.Gain) && EqualityComparer<float>.Default.Equals(this.GainHF, other.GainHF) && EqualityComparer<float>.Default.Equals(this.GainLF, other.GainLF) && EqualityComparer<float>.Default.Equals(this.DecayTime, other.DecayTime) && EqualityComparer<float>.Default.Equals(this.DecayHFRatio, other.DecayHFRatio) && EqualityComparer<float>.Default.Equals(this.DecayLFRatio, other.DecayLFRatio) && EqualityComparer<float>.Default.Equals(this.ReflectionsGain, other.ReflectionsGain) && EqualityComparer<float>.Default.Equals(this.ReflectionsDelay, other.ReflectionsDelay) && EqualityComparer<Vector3>.Default.Equals(this.ReflectionsPan, other.ReflectionsPan) && EqualityComparer<float>.Default.Equals(this.LateReverbGain, other.LateReverbGain) && EqualityComparer<float>.Default.Equals(this.LateReverbDelay, other.LateReverbDelay) && EqualityComparer<Vector3>.Default.Equals(this.LateReverbPan, other.LateReverbPan) && EqualityComparer<float>.Default.Equals(this.EchoTime, other.EchoTime) && EqualityComparer<float>.Default.Equals(this.EchoDepth, other.EchoDepth) && EqualityComparer<float>.Default.Equals(this.ModulationTime, other.ModulationTime) && EqualityComparer<float>.Default.Equals(this.ModulationDepth, other.ModulationDepth) && EqualityComparer<float>.Default.Equals(this.AirAbsorptionGainHF, other.AirAbsorptionGainHF) && EqualityComparer<float>.Default.Equals(this.HFReference, other.HFReference) && EqualityComparer<float>.Default.Equals(this.LFReference, other.LFReference) && EqualityComparer<float>.Default.Equals(this.RoomRolloffFactor, other.RoomRolloffFactor) && EqualityComparer<int>.Default.Equals(this.DecayHFLimit, other.DecayHFLimit);
  }
}
