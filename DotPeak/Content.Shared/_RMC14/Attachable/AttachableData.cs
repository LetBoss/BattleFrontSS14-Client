// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.AttachableSlot
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Attachable;

[DataDefinition]
[NetSerializable]
[Serializable]
public struct AttachableSlot : ISerializationGenerated<AttachableSlot>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Locked;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<AttachableComponent>? StartingAttachable;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId<AttachableComponent>>? Random;
  [DataField(null, false, 1, false, false, null)]
  public float RandomChance;

  public AttachableSlot()
  {
    this.Locked = false;
    this.Whitelist = (EntityWhitelist) null;
    this.StartingAttachable = new EntProtoId<AttachableComponent>?();
    this.Random = (List<EntProtoId<AttachableComponent>>) null;
    this.RandomChance = 1f;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableSlot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AttachableSlot>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target1, hookCtx, false, context))
      target1 = this.Locked;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    EntProtoId<AttachableComponent>? target3 = new EntProtoId<AttachableComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<AttachableComponent>?>(this.StartingAttachable, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId<AttachableComponent>?>(this.StartingAttachable, hookCtx, context);
    List<EntProtoId<AttachableComponent>> target4 = (List<EntProtoId<AttachableComponent>>) null;
    if (!serialization.TryCustomCopy<List<EntProtoId<AttachableComponent>>>(this.Random, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId<AttachableComponent>>>(this.Random, hookCtx, context);
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RandomChance, ref target5, hookCtx, false, context))
      target5 = this.RandomChance;
    target = target with
    {
      Locked = target1,
      Whitelist = target2,
      StartingAttachable = target3,
      Random = target4,
      RandomChance = target5
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableSlot target,
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
    AttachableSlot target1 = (AttachableSlot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AttachableSlot Instantiate() => new AttachableSlot();
}
