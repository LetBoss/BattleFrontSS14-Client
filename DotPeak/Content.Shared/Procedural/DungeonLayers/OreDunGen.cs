// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonLayers.OreDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.DungeonLayers;

[Virtual]
public class OreDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<OreDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Replacement;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Entity;
  [DataField(null, false, 1, false, false, null)]
  public int Count = 10;
  [DataField(null, false, 1, false, false, null)]
  public int MinGroupSize = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxGroupSize = 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref OreDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<OreDunGen>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target1 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Replacement, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId?>(this.Replacement, hookCtx, context);
    target.Replacement = target1;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Entity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Entity, hookCtx, context);
    target.Entity = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target3, hookCtx, false, context))
      target3 = this.Count;
    target.Count = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinGroupSize, ref target4, hookCtx, false, context))
      target4 = this.MinGroupSize;
    target.MinGroupSize = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxGroupSize, ref target5, hookCtx, false, context))
      target5 = this.MaxGroupSize;
    target.MaxGroupSize = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref OreDunGen target,
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
    OreDunGen target1 = (OreDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OreDunGen target1 = (OreDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDunGenLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual OreDunGen Instantiate() => new OreDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
