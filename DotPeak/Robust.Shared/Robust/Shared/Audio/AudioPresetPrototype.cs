// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioPresetPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Audio;

[Prototype(null, 1)]
public sealed class AudioPresetPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool CreateAuxiliary;
  [DataField(null, false, 1, false, false, null)]
  public float Density;
  [DataField(null, false, 1, false, false, null)]
  public float Diffusion;
  [DataField(null, false, 1, false, false, null)]
  public float Gain;
  [DataField("gainHf", false, 1, false, false, null)]
  public float GainHF;
  [DataField("gainLf", false, 1, false, false, null)]
  public float GainLF;
  [DataField(null, false, 1, false, false, null)]
  public float DecayTime;
  [DataField("decayHfRatio", false, 1, false, false, null)]
  public float DecayHFRatio;
  [DataField("decayLfRatio", false, 1, false, false, null)]
  public float DecayLFRatio;
  [DataField(null, false, 1, false, false, null)]
  public float ReflectionsGain;
  [DataField(null, false, 1, false, false, null)]
  public float ReflectionsDelay;
  [DataField(null, false, 1, false, false, null)]
  public Vector3 ReflectionsPan;
  [DataField(null, false, 1, false, false, null)]
  public float LateReverbGain;
  [DataField(null, false, 1, false, false, null)]
  public float LateReverbDelay;
  [DataField(null, false, 1, false, false, null)]
  public Vector3 LateReverbPan;
  [DataField(null, false, 1, false, false, null)]
  public float EchoTime;
  [DataField(null, false, 1, false, false, null)]
  public float EchoDepth;
  [DataField(null, false, 1, false, false, null)]
  public float ModulationTime;
  [DataField(null, false, 1, false, false, null)]
  public float ModulationDepth;
  [DataField("airAbsorptionGainHf", false, 1, false, false, null)]
  public float AirAbsorptionGainHF;
  [DataField("hfReference", false, 1, false, false, null)]
  public float HFReference;
  [DataField("lfReference", false, 1, false, false, null)]
  public float LFReference;
  [DataField(null, false, 1, false, false, null)]
  public float RoomRolloffFactor;
  [DataField("decayHfLimit", false, 1, false, false, null)]
  public int DecayHFLimit;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
