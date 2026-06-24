// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.Loot.RandomSpawnLootEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Content.Shared.Procedural.Loot;

[DataDefinition]
public record struct RandomSpawnLootEntry : 
  IBudgetEntry,
  IProbEntry,
  ISerializationGenerated<RandomSpawnLootEntry>,
  ISerializationGenerated
{
  public RandomSpawnLootEntry()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CProto\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CCost\u003Ek__BackingField = 1f;
    // ISSUE: reference to a compiler-generated field
    this.\u003CProb\u003Ek__BackingField = 1f;
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("proto", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Proto { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("cost", false, 1, false, false, null)]
  public float Cost { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("prob", false, 1, false, false, null)]
  public float Prob { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RandomSpawnLootEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RandomSpawnLootEntry>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Proto == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Proto, ref target1, hookCtx, false, context))
      target1 = this.Proto;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Cost, ref target2, hookCtx, false, context))
      target2 = this.Cost;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Prob, ref target3, hookCtx, false, context))
      target3 = this.Prob;
    target = target with
    {
      Proto = target1,
      Cost = target2,
      Prob = target3
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RandomSpawnLootEntry target,
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
    RandomSpawnLootEntry target1 = (RandomSpawnLootEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RandomSpawnLootEntry Instantiate() => new RandomSpawnLootEntry();
}
