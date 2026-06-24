// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.MiddleConnectionDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.PostGeneration;

public sealed class MiddleConnectionDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<MiddleConnectionDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public int OverlapCount = -1;
  [DataField(null, false, 1, false, false, null)]
  public int Count = 1;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<EntityTablePrototype> Contents;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EntityTablePrototype>? Flank;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MiddleConnectionDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MiddleConnectionDunGen>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.OverlapCount, ref target1, hookCtx, false, context))
      target1 = this.OverlapCount;
    target.OverlapCount = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target2, hookCtx, false, context))
      target2 = this.Count;
    target.Count = target2;
    ProtoId<ContentTileDefinition> target3 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target3;
    ProtoId<EntityTablePrototype> target4 = new ProtoId<EntityTablePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(this.Contents, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(this.Contents, hookCtx, context);
    target.Contents = target4;
    ProtoId<EntityTablePrototype>? target5 = new ProtoId<EntityTablePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>?>(this.Flank, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<EntityTablePrototype>?>(this.Flank, hookCtx, context);
    target.Flank = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MiddleConnectionDunGen target,
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
    MiddleConnectionDunGen target1 = (MiddleConnectionDunGen) target;
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
    MiddleConnectionDunGen target1 = (MiddleConnectionDunGen) target;
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
  public MiddleConnectionDunGen Instantiate() => new MiddleConnectionDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
