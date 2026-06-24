// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.Loot.RandomSpawnsLoot
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural.Loot;

public sealed class RandomSpawnsLoot : 
  IDungeonLoot,
  ISerializationGenerated<IDungeonLoot>,
  ISerializationGenerated,
  ISerializationGenerated<RandomSpawnsLoot>
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("entries", false, 1, true, false, null)]
  public List<RandomSpawnLootEntry> Entries = new List<RandomSpawnLootEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomSpawnsLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RandomSpawnsLoot>(this, ref target, hookCtx, false, context))
      return;
    List<RandomSpawnLootEntry> target1 = (List<RandomSpawnLootEntry>) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RandomSpawnLootEntry>>(this.Entries, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<List<RandomSpawnLootEntry>>(this.Entries, hookCtx, context);
    target.Entries = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomSpawnsLoot target,
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
    RandomSpawnsLoot target1 = (RandomSpawnsLoot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IDungeonLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RandomSpawnsLoot target1 = (RandomSpawnsLoot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDungeonLoot) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IDungeonLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RandomSpawnsLoot Instantiate() => new RandomSpawnsLoot();

  IDungeonLoot IDungeonLoot.Instantiate() => (IDungeonLoot) this.Instantiate();

  IDungeonLoot ISerializationGenerated<IDungeonLoot>.Instantiate()
  {
    return (IDungeonLoot) this.Instantiate();
  }
}
