// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.Sources.DummyAudioSource
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Effects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Audio.Sources;

[Virtual]
[DataDefinition]
internal class DummyAudioSource : 
  IAudioSource,
  IDisposable,
  ISerializationGenerated<DummyAudioSource>,
  ISerializationGenerated
{
  public static DummyAudioSource Instance { get; } = new DummyAudioSource();

  public void Dispose()
  {
  }

  public void Pause()
  {
  }

  public void StartPlaying()
  {
  }

  public void StopPlaying()
  {
  }

  public void Restart()
  {
  }

  public bool Playing { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public bool Looping { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public bool Global { get; set; }

  public Vector2 Position { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float Pitch { get; set; }

  public float Volume { get; set; }

  public float Gain { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float MaxDistance { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float RolloffFactor { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public float ReferenceDistance { get; set; }

  public float Occlusion { get; set; }

  public float PlaybackPosition { get; set; }

  public Vector2 Velocity { get; set; }

  public void SetAuxiliary(IAuxiliaryAudio? audio)
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref DummyAudioSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DummyAudioSource>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Looping, ref target1, hookCtx, false, context))
      target1 = this.Looping;
    target.Looping = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Global, ref target2, hookCtx, false, context))
      target2 = this.Global;
    target.Global = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Pitch, ref target3, hookCtx, false, context))
      target3 = this.Pitch;
    target.Pitch = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDistance, ref target4, hookCtx, false, context))
      target4 = this.MaxDistance;
    target.MaxDistance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RolloffFactor, ref target5, hookCtx, false, context))
      target5 = this.RolloffFactor;
    target.RolloffFactor = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReferenceDistance, ref target6, hookCtx, false, context))
      target6 = this.ReferenceDistance;
    target.ReferenceDistance = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref DummyAudioSource target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DummyAudioSource target1 = (DummyAudioSource) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual DummyAudioSource Instantiate() => new DummyAudioSource();
}
