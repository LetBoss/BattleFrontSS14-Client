// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Effects.IAudioEffect
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Audio.Effects;

[NotContentImplementable]
public interface IAudioEffect
{
  float Density { get; set; }

  float Diffusion { get; set; }

  float Gain { get; set; }

  float GainHF { get; set; }

  float GainLF { get; set; }

  float DecayTime { get; set; }

  float DecayHFRatio { get; set; }

  float DecayLFRatio { get; set; }

  float ReflectionsGain { get; set; }

  float ReflectionsDelay { get; set; }

  Vector3 ReflectionsPan { get; set; }

  float LateReverbGain { get; set; }

  float LateReverbDelay { get; set; }

  Vector3 LateReverbPan { get; set; }

  float EchoTime { get; set; }

  float EchoDepth { get; set; }

  float ModulationTime { get; set; }

  float ModulationDepth { get; set; }

  float AirAbsorptionGainHF { get; set; }

  float HFReference { get; set; }

  float LFReference { get; set; }

  float RoomRolloffFactor { get; set; }

  int DecayHFLimit { get; set; }
}
