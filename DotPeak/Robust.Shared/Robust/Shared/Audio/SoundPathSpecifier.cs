// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.SoundPathSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Audio;

[NetSerializable]
[Serializable]
public sealed class SoundPathSpecifier : 
  SoundSpecifier,
  ISerializationGenerated<SoundPathSpecifier>,
  ISerializationGenerated
{
  public const string Node = "path";

  [DataField("path", false, 1, true, false, typeof (ResPathSerializer))]
  public ResPath Path { get; private set; }

  public override string ToString() => $"SoundPathSpecifier({this.Path})";

  private SoundPathSpecifier()
  {
  }

  public SoundPathSpecifier(string path, AudioParams? @params = null)
    : this(new ResPath(path), @params)
  {
  }

  public SoundPathSpecifier(ResPath path, AudioParams? @params = null)
  {
    this.Path = path;
    if (!@params.HasValue)
      return;
    this.Params = @params.Value;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SoundPathSpecifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SoundSpecifier target1 = (SoundSpecifier) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SoundPathSpecifier) target1;
    if (serialization.TryCustomCopy<SoundPathSpecifier>(this, ref target, hookCtx, false, context))
      return;
    ResPath resPath = new ResPath();
    ResPath copy = serialization.CreateCopy<ResPath, ResPathSerializer>(this.Path, hookCtx, context);
    target.Path = copy;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SoundPathSpecifier target,
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
    SoundPathSpecifier target1 = (SoundPathSpecifier) target;
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
    SoundPathSpecifier target1 = (SoundPathSpecifier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SoundPathSpecifier SoundSpecifier.Instantiate() => new SoundPathSpecifier();
}
