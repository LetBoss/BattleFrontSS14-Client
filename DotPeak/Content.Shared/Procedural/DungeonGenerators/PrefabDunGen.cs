// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonGenerators.PrefabDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class PrefabDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<PrefabDunGen>
{
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<DungeonPresetPrototype>> Presets = new List<ProtoId<DungeonPresetPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? RoomWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ContentTileDefinition>? FallbackTile;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PrefabDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PrefabDunGen>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<DungeonPresetPrototype>> target1 = (List<ProtoId<DungeonPresetPrototype>>) null;
    if (this.Presets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DungeonPresetPrototype>>>(this.Presets, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<List<ProtoId<DungeonPresetPrototype>>>(this.Presets, hookCtx, context);
    target.Presets = target1;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.RoomWhitelist, ref target2, hookCtx, false, context))
    {
      if (this.RoomWhitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.RoomWhitelist, ref target2, hookCtx, context);
    }
    target.RoomWhitelist = target2;
    ProtoId<ContentTileDefinition>? target3 = new ProtoId<ContentTileDefinition>?();
    if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>?>(this.FallbackTile, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<ContentTileDefinition>?>(this.FallbackTile, hookCtx, context);
    target.FallbackTile = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PrefabDunGen target,
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
    PrefabDunGen target1 = (PrefabDunGen) target;
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
    PrefabDunGen target1 = (PrefabDunGen) target;
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
  public PrefabDunGen Instantiate() => new PrefabDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
