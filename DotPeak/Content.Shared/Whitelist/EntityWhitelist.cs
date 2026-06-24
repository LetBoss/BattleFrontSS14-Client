// Decompiled with JetBrains decompiler
// Type: Content.Shared.Whitelist.EntityWhitelist
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Stun;
using Content.Shared.Item;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Whitelist;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class EntityWhitelist : 
  ISerializationGenerated<EntityWhitelist>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string[]? Components;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ItemSizePrototype>>? Sizes;
  [Access(new Type[] {typeof (EntityWhitelistSystem)})]
  [NonSerialized]
  public List<ComponentRegistration>? Registrations;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<TagPrototype>>? Tags;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireAll;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int>? Skills;
  [DataField(null, false, 1, false, false, null)]
  public RMCSizes? MinMobSize;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityWhitelist target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EntityWhitelist>(this, ref target, hookCtx, false, context))
      return;
    string[] target1 = (string[]) null;
    if (!serialization.TryCustomCopy<string[]>(this.Components, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<string[]>(this.Components, hookCtx, context);
    target.Components = target1;
    List<ProtoId<ItemSizePrototype>> target2 = (List<ProtoId<ItemSizePrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<ItemSizePrototype>>>(this.Sizes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<ItemSizePrototype>>>(this.Sizes, hookCtx, context);
    target.Sizes = target2;
    List<ProtoId<TagPrototype>> target3 = (List<ProtoId<TagPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this.Tags, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this.Tags, hookCtx, context);
    target.Tags = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireAll, ref target4, hookCtx, false, context))
      target4 = this.RequireAll;
    target.RequireAll = target4;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target5 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target5;
    RMCSizes? target6 = new RMCSizes?();
    if (!serialization.TryCustomCopy<RMCSizes?>(this.MinMobSize, ref target6, hookCtx, false, context))
      target6 = this.MinMobSize;
    target.MinMobSize = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityWhitelist target,
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
    EntityWhitelist target1 = (EntityWhitelist) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public EntityWhitelist Instantiate() => new EntityWhitelist();
}
