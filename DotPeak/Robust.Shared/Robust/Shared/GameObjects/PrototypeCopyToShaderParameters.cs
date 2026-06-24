// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.PrototypeCopyToShaderParameters
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class PrototypeCopyToShaderParameters : 
  ISerializationGenerated<PrototypeCopyToShaderParameters>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string LayerKey;
  [DataField(null, false, 1, false, false, null)]
  public string? ParameterTexture;
  [DataField(null, false, 1, false, false, null)]
  public string? ParameterUV;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PrototypeCopyToShaderParameters target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PrototypeCopyToShaderParameters>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.LayerKey == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LayerKey, ref target1, hookCtx, false, context))
      target1 = this.LayerKey;
    target.LayerKey = target1;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ParameterTexture, ref target2, hookCtx, false, context))
      target2 = this.ParameterTexture;
    target.ParameterTexture = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ParameterUV, ref target3, hookCtx, false, context))
      target3 = this.ParameterUV;
    target.ParameterUV = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PrototypeCopyToShaderParameters target,
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
    PrototypeCopyToShaderParameters target1 = (PrototypeCopyToShaderParameters) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PrototypeCopyToShaderParameters Instantiate() => new PrototypeCopyToShaderParameters();
}
