// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.SoundSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[ImplicitDataDefinitionForInheritors]
[NetSerializable]
[Serializable]
public abstract class SoundSpecifier : 
  ISerializationGenerated<SoundSpecifier>,
  ISerializationGenerated
{
  [DataField("params", false, 1, false, false, null)]
  public AudioParams Params { get; set; } = AudioParams.Default;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SoundSpecifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SoundSpecifier>(this, ref target, hookCtx, false, context))
      return;
    AudioParams target1 = new AudioParams();
    if (!serialization.TryCustomCopy<AudioParams>(this.Params, ref target1, hookCtx, false, context))
      serialization.CopyTo<AudioParams>(this.Params, ref target1, hookCtx, context);
    target.Params = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SoundSpecifier target,
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
    SoundSpecifier target1 = (SoundSpecifier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual SoundSpecifier Instantiate() => throw new NotImplementedException();
}
