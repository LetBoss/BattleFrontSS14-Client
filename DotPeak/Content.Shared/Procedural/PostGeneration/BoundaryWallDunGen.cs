// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.BoundaryWallDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.PostGeneration;

public sealed class BoundaryWallDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<BoundaryWallDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public BoundaryWallFlags Flags = BoundaryWallFlags.Rooms | BoundaryWallFlags.Corridors;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Wall;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? CornerWall;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BoundaryWallDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<BoundaryWallDunGen>(this, ref target, hookCtx, false, context))
      return;
    BoundaryWallFlags target1 = (BoundaryWallFlags) 0;
    if (!serialization.TryCustomCopy<BoundaryWallFlags>(this.Flags, ref target1, hookCtx, false, context))
      target1 = this.Flags;
    target.Flags = target1;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Wall, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Wall, hookCtx, context);
    target.Wall = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.CornerWall, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.CornerWall, hookCtx, context);
    target.CornerWall = target3;
    ProtoId<ContentTileDefinition> target4 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BoundaryWallDunGen target,
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
    BoundaryWallDunGen target1 = (BoundaryWallDunGen) target;
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
    BoundaryWallDunGen target1 = (BoundaryWallDunGen) target;
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
  public BoundaryWallDunGen Instantiate() => new BoundaryWallDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
