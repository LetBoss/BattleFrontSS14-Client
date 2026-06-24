// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Effects.DummyAudioEffect
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Numerics;

#nullable disable
namespace Robust.Shared.Audio.Effects;

internal sealed class DummyAudioEffect : IAudioEffect
{
  public void Dispose()
  {
  }

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
}
