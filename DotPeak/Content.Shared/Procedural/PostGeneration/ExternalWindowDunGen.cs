// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.PostGeneration.ExternalWindowDunGen
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

public sealed class ExternalWindowDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<ExternalWindowDunGen>
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ContentTileDefinition> Tile;
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<EntityTablePrototype> Contents;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExternalWindowDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ExternalWindowDunGen>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ContentTileDefinition> target1 = new ProtoId<ContentTileDefinition>();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(this.Tile, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(this.Tile, hookCtx, context);
    target.Tile = target1;
    ProtoId<EntityTablePrototype> target2 = new ProtoId<EntityTablePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(this.Contents, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(this.Contents, hookCtx, context);
    target.Contents = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExternalWindowDunGen target,
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
    ExternalWindowDunGen target1 = (ExternalWindowDunGen) target;
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
    ExternalWindowDunGen target1 = (ExternalWindowDunGen) target;
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
  public ExternalWindowDunGen Instantiate() => new ExternalWindowDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
