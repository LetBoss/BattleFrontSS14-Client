// Decompiled with JetBrains decompiler
// Type: Content.Shared.DisplacementMap.DisplacementData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DisplacementMap;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class DisplacementData : 
  ISerializationGenerated<DisplacementData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<int, PrototypeLayerData> SizeMaps = new Dictionary<int, PrototypeLayerData>();
  [DataField(null, false, 1, false, false, null)]
  public string? ShaderOverride = "DisplacedDraw";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DisplacementData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DisplacementData>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<int, PrototypeLayerData> dictionary = (Dictionary<int, PrototypeLayerData>) null;
    if (this.SizeMaps == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, PrototypeLayerData>>(this.SizeMaps, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<int, PrototypeLayerData>>(this.SizeMaps, hookCtx, context, false);
    target.SizeMaps = dictionary;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ShaderOverride, ref str, hookCtx, false, context))
      str = this.ShaderOverride;
    target.ShaderOverride = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DisplacementData target,
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
    DisplacementData target1 = (DisplacementData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public DisplacementData Instantiate() => new DisplacementData();
}
