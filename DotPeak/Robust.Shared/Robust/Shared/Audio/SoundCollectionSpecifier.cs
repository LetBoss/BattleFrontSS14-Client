// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.SoundCollectionSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Audio;

[NetSerializable]
[Serializable]
public sealed class SoundCollectionSpecifier : 
  SoundSpecifier,
  ISerializationGenerated<SoundCollectionSpecifier>,
  ISerializationGenerated
{
  public const string Node = "collection";

  [DataField("collection", false, 1, true, false, typeof (PrototypeIdSerializer<SoundCollectionPrototype>))]
  public string? Collection { get; private set; }

  public override string ToString() => $"SoundCollectionSpecifier({this.Collection})";

  public SoundCollectionSpecifier()
  {
  }

  public SoundCollectionSpecifier(string collection, AudioParams? @params = null)
  {
    this.Collection = collection;
    if (!@params.HasValue)
      return;
    this.Params = @params.Value;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SoundCollectionSpecifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SoundSpecifier target1 = (SoundSpecifier) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SoundCollectionSpecifier) target1;
    if (serialization.TryCustomCopy<SoundCollectionSpecifier>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Collection, ref target2, hookCtx, false, context))
      target2 = this.Collection;
    target.Collection = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SoundCollectionSpecifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SoundSpecifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SoundCollectionSpecifier target1 = (SoundCollectionSpecifier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SoundSpecifier) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SoundCollectionSpecifier target1 = (SoundCollectionSpecifier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SoundCollectionSpecifier SoundSpecifier.Instantiate() => new SoundCollectionSpecifier();
}
