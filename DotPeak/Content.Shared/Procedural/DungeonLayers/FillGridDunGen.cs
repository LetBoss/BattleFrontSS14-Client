// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonLayers.FillGridDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.DungeonLayers;

public sealed class FillGridDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<FillGridDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<ContentTileDefinition>>? AllowedTiles;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Entity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FillGridDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<FillGridDunGen>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<ContentTileDefinition>> target1 = (HashSet<ProtoId<ContentTileDefinition>>) null;
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ContentTileDefinition>>>(this.AllowedTiles, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<HashSet<ProtoId<ContentTileDefinition>>>(this.AllowedTiles, hookCtx, context);
    target.AllowedTiles = target1;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Entity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Entity, hookCtx, context);
    target.Entity = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FillGridDunGen target,
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
    FillGridDunGen target1 = (FillGridDunGen) target;
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
    FillGridDunGen target1 = (FillGridDunGen) target;
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
  public FillGridDunGen Instantiate() => new FillGridDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
