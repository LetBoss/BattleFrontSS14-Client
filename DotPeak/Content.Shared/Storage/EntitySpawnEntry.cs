// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySpawnEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Storage;

[DataDefinition]
[Serializable]
public struct EntitySpawnEntry : ISerializationGenerated<EntitySpawnEntry>, ISerializationGenerated
{
  [DataField("id", false, 1, false, false, null)]
  public EntProtoId? PrototypeId;
  [DataField("prob", false, 1, false, false, null)]
  public float SpawnProbability;
  [DataField("orGroup", false, 1, false, false, null)]
  public string? GroupId;
  [DataField(null, false, 1, false, false, null)]
  public int Amount;
  [DataField(null, false, 1, false, false, null)]
  public int MaxAmount;

  public EntitySpawnEntry()
  {
    this.PrototypeId = new EntProtoId?();
    this.SpawnProbability = 1f;
    this.GroupId = (string) null;
    this.Amount = 1;
    this.MaxAmount = 1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntitySpawnEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntitySpawnEntry>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target1 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.PrototypeId, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId?>(this.PrototypeId, hookCtx, context);
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpawnProbability, ref target2, hookCtx, false, context))
      target2 = this.SpawnProbability;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.GroupId, ref target3, hookCtx, false, context))
      target3 = this.GroupId;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target4, hookCtx, false, context))
      target4 = this.Amount;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxAmount, ref target5, hookCtx, false, context))
      target5 = this.MaxAmount;
    target = target with
    {
      PrototypeId = target1,
      SpawnProbability = target2,
      GroupId = target3,
      Amount = target4,
      MaxAmount = target5
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntitySpawnEntry target,
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
    EntitySpawnEntry target1 = (EntitySpawnEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public EntitySpawnEntry Instantiate() => new EntitySpawnEntry();
}
