// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonLayers.EntityTableDunGen
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Procedural.DungeonLayers;

public sealed class EntityTableDunGen : 
  IDunGenLayer,
  ISerializationGenerated<IDunGenLayer>,
  ISerializationGenerated,
  ISerializationGenerated<EntityTableDunGen>
{
  [DataField(null, false, 1, false, false, null)]
  public int MinCount = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxCount = 1;
  [DataField(null, false, 1, true, false, null)]
  public EntityTableSelector Table;
  [DataField(null, false, 1, false, false, null)]
  public bool PerDungeon;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityTableDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntityTableDunGen>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinCount, ref target1, hookCtx, false, context))
      target1 = this.MinCount;
    target.MinCount = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxCount, ref target2, hookCtx, false, context))
      target2 = this.MaxCount;
    target.MaxCount = target2;
    EntityTableSelector target3 = (EntityTableSelector) null;
    if (this.Table == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityTableSelector>(this.Table, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<EntityTableSelector>(this.Table, hookCtx, context);
    target.Table = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.PerDungeon, ref target4, hookCtx, false, context))
      target4 = this.PerDungeon;
    target.PerDungeon = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityTableDunGen target,
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
    EntityTableDunGen target1 = (EntityTableDunGen) target;
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
    EntityTableDunGen target1 = (EntityTableDunGen) target;
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
  public EntityTableDunGen Instantiate() => new EntityTableDunGen();

  IDunGenLayer IDunGenLayer.Instantiate() => (IDunGenLayer) this.Instantiate();

  IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
  {
    return (IDunGenLayer) this.Instantiate();
  }
}
