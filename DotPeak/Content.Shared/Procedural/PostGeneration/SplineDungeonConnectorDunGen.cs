// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.SplineDungeonConnectorDunGen
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

public sealed class SplineDungeonConnectorDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<SplineDungeonConnectorDunGen>
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ContentTileDefinition>? WidenTile;
  [DataField(null, false, 1, false, false, null)]
  public int DivisionDistance = 10;
  [DataField(null, false, 1, false, false, null)]
  public float VarianceMax = 0.35f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SplineDungeonConnectorDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SplineDungeonConnectorDunGen>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ContentTileDefinition> target1 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target1;
    ProtoId<ContentTileDefinition>? target2 = new ProtoId<ContentTileDefinition>?();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(this.WidenTile, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(this.WidenTile, hookCtx, context);
    target.WidenTile = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.DivisionDistance, ref target3, hookCtx, false, context))
      target3 = this.DivisionDistance;
    target.DivisionDistance = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VarianceMax, ref target4, hookCtx, false, context))
      target4 = this.VarianceMax;
    target.VarianceMax = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SplineDungeonConnectorDunGen target,
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
    SplineDungeonConnectorDunGen target1 = (SplineDungeonConnectorDunGen) target;
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
    SplineDungeonConnectorDunGen target1 = (SplineDungeonConnectorDunGen) target;
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
  public SplineDungeonConnectorDunGen Instantiate() => new SplineDungeonConnectorDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
