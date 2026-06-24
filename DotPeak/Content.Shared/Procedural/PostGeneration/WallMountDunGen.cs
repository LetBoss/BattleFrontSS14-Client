// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.WallMountDunGen
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

public sealed class WallMountDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<WallMountDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public double Prob = 0.1;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<EntityTablePrototype> Contents;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WallMountDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<WallMountDunGen>(this, ref target, hookCtx, false, context))
      return;
    double target1 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Prob, ref target1, hookCtx, false, context))
      target1 = this.Prob;
    target.Prob = target1;
    ProtoId<ContentTileDefinition> target2 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target2;
    ProtoId<EntityTablePrototype> target3 = new ProtoId<EntityTablePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(this.Contents, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(this.Contents, hookCtx, context);
    target.Contents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WallMountDunGen target,
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
    WallMountDunGen target1 = (WallMountDunGen) target;
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
    WallMountDunGen target1 = (WallMountDunGen) target;
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
  public WallMountDunGen Instantiate() => new WallMountDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
