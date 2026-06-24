// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.SharedMapLayerData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Storage.Components;

[DataDefinition]
[Serializable]
public sealed class SharedMapLayerData : 
  ISerializationGenerated<SharedMapLayerData>,
  ISerializationGenerated
{
  public string Layer = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public int MinCount = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxCount = int.MaxValue;

  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist? Whitelist { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SharedMapLayerData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SharedMapLayerData>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target1, hookCtx, context);
    }
    target.Whitelist = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinCount, ref target2, hookCtx, false, context))
      target2 = this.MinCount;
    target.MinCount = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxCount, ref target3, hookCtx, false, context))
      target3 = this.MaxCount;
    target.MaxCount = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SharedMapLayerData target,
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
    SharedMapLayerData target1 = (SharedMapLayerData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public SharedMapLayerData Instantiate() => new SharedMapLayerData();
}
