// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioParams
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[NetSerializable]
[DataDefinition]
[Serializable]
public struct AudioParams : ISerializationGenerated<AudioParams>, ISerializationGenerated
{
  private float _volume;
  private float _pitch;
  public static readonly AudioParams Default = new AudioParams(0.0f, 1f, 15f, 1f, 1f, false, 0.0f);

  [DataField(null, false, 1, false, false, null)]
  public float Volume
  {
    get => this._volume;
    set
    {
      if (float.IsNaN(value))
        value = float.NegativeInfinity;
      this._volume = value;
    }
  }

  [DataField(null, false, 1, false, false, null)]
  public float Pitch
  {
    get => this._pitch;
    set => this._pitch = MathF.Max(0.0f, value);
  }

  [DataField(null, false, 1, false, false, null)]
  public float MaxDistance { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float RolloffFactor { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float ReferenceDistance { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public bool Loop { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float PlayOffsetSeconds { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float? Variation { get; set; }

  public AudioParams()
  {
    this._volume = AudioParams.Default.Volume;
    this._pitch = AudioParams.Default.Pitch;
    // ISSUE: reference to a compiler-generated field
    this.\u003CMaxDistance\u003Ek__BackingField = AudioParams.Default.MaxDistance;
    // ISSUE: reference to a compiler-generated field
    this.\u003CRolloffFactor\u003Ek__BackingField = AudioParams.Default.RolloffFactor;
    // ISSUE: reference to a compiler-generated field
    this.\u003CReferenceDistance\u003Ek__BackingField = AudioParams.Default.ReferenceDistance;
    // ISSUE: reference to a compiler-generated field
    this.\u003CLoop\u003Ek__BackingField = AudioParams.Default.Loop;
    // ISSUE: reference to a compiler-generated field
    this.\u003CPlayOffsetSeconds\u003Ek__BackingField = AudioParams.Default.PlayOffsetSeconds;
    // ISSUE: reference to a compiler-generated field
    this.\u003CVariation\u003Ek__BackingField = new float?();
  }

  public AudioParams(
    float volume,
    float pitch,
    float maxDistance,
    float refDistance,
    bool loop,
    float playOffsetSeconds,
    float? variation = null)
    : this(volume, pitch, maxDistance, 1f, refDistance, loop, playOffsetSeconds, variation)
  {
  }

  public AudioParams(
    float volume,
    float pitch,
    float maxDistance,
    float rolloffFactor,
    float refDistance,
    bool loop,
    float playOffsetSeconds,
    float? variation = null)
  {
    this = new AudioParams();
    this.Volume = volume;
    this.Pitch = pitch;
    this.MaxDistance = maxDistance;
    this.RolloffFactor = rolloffFactor;
    this.ReferenceDistance = refDistance;
    this.Loop = loop;
    this.PlayOffsetSeconds = playOffsetSeconds;
    this.Variation = variation;
  }

  public readonly AudioParams WithVolume(float volume)
  {
    return this with { Volume = volume };
  }

  public readonly AudioParams AddVolume(float volume)
  {
    AudioParams audioParams = this;
    audioParams.Volume += volume;
    return audioParams;
  }

  public readonly AudioParams WithVariation(float? variation)
  {
    return this with { Variation = variation };
  }

  public readonly AudioParams WithPitchScale(float pitch)
  {
    return this with { Pitch = pitch };
  }

  public readonly AudioParams WithMaxDistance(float dist)
  {
    return this with { MaxDistance = dist };
  }

  public readonly AudioParams WithRolloffFactor(float rolloffFactor)
  {
    return this with { RolloffFactor = rolloffFactor };
  }

  public readonly AudioParams WithReferenceDistance(float refDistance)
  {
    return this with { ReferenceDistance = refDistance };
  }

  public readonly AudioParams WithLoop(bool loop)
  {
    return this with { Loop = loop };
  }

  public readonly AudioParams WithPlayOffset(float offset)
  {
    return this with { PlayOffsetSeconds = offset };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AudioParams target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AudioParams>(this, ref target, hookCtx, false, context))
      return;
    float target1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Volume, ref target1, hookCtx, false, context))
      target1 = this.Volume;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Pitch, ref target2, hookCtx, false, context))
      target2 = this.Pitch;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDistance, ref target3, hookCtx, false, context))
      target3 = this.MaxDistance;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RolloffFactor, ref target4, hookCtx, false, context))
      target4 = this.RolloffFactor;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReferenceDistance, ref target5, hookCtx, false, context))
      target5 = this.ReferenceDistance;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Loop, ref target6, hookCtx, false, context))
      target6 = this.Loop;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PlayOffsetSeconds, ref target7, hookCtx, false, context))
      target7 = this.PlayOffsetSeconds;
    float? target8 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Variation, ref target8, hookCtx, false, context))
      target8 = this.Variation;
    target = target with
    {
      Volume = target1,
      Pitch = target2,
      MaxDistance = target3,
      RolloffFactor = target4,
      ReferenceDistance = target5,
      Loop = target6,
      PlayOffsetSeconds = target7,
      Variation = target8
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AudioParams target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AudioParams target1 = (AudioParams) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AudioParams Instantiate() => new AudioParams();
}
