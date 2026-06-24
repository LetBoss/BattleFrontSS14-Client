// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonLayers.MobsDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.DungeonLayers;

public sealed class MobsDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<MobsDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public int MinCount = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxCount = 1;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<EntityTablePrototype> Contents;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobsDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MobsDunGen>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinCount, ref target1, hookCtx, false, context))
      target1 = this.MinCount;
    target.MinCount = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxCount, ref target2, hookCtx, false, context))
      target2 = this.MaxCount;
    target.MaxCount = target2;
    ProtoId<EntityTablePrototype> target3 = new ProtoId<EntityTablePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(this.Contents, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(this.Contents, hookCtx, context);
    target.Contents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobsDunGen target,
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
    MobsDunGen target1 = (MobsDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MobsDunGen target1 = (MobsDunGen) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDunGenLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MobsDunGen Instantiate() => new MobsDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
