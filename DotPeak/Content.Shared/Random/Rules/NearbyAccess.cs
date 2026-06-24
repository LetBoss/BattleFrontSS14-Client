// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.NearbyAccessRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Random.Rules;

public sealed class NearbyAccessRule : 
  RulesRule,
  ISerializationGenerated<NearbyAccessRule>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Anchored = true;
  [DataField(null, false, 1, false, false, null)]
  public int Count = 1;
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<AccessLevelPrototype>> Access = new List<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public float Range = 10f;

  public override bool Check(EntityManager entManager, EntityUid uid)
  {
    EntityQuery<TransformComponent> entityQuery = entManager.GetEntityQuery<TransformComponent>();
    TransformComponent component1;
    if (!entityQuery.TryGetComponent(uid, out component1) || !component1.MapUid.HasValue)
      return false;
    SharedTransformSystem sharedTransformSystem = entManager.System<SharedTransformSystem>();
    EntityLookupSystem entityLookupSystem = entManager.System<EntityLookupSystem>();
    AccessReaderSystem accessReaderSystem = entManager.System<AccessReaderSystem>();
    bool flag = false;
    TransformComponent component2 = component1;
    EntityQuery<TransformComponent> xformQuery = entityQuery;
    Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(component2, xformQuery);
    int num = 0;
    HashSet<Entity<AccessReaderComponent>> entities = new HashSet<Entity<AccessReaderComponent>>();
    entityLookupSystem.GetEntitiesInRange<AccessReaderComponent>(component1.MapID, worldPosition, this.Range, entities);
    foreach (Entity<AccessReaderComponent> entity in entities)
    {
      TransformComponent component3;
      if (accessReaderSystem.AreAccessTagsAllowed((ICollection<ProtoId<AccessLevelPrototype>>) this.Access, (AccessReaderComponent) entity) && (!this.Anchored || entityQuery.TryGetComponent((EntityUid) entity, out component3) && component3.Anchored))
      {
        ++num;
        if (num >= this.Count)
        {
          flag = true;
          break;
        }
      }
    }
    return !flag ? this.Inverted : !this.Inverted;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NearbyAccessRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RulesRule target1 = (RulesRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NearbyAccessRule) target1;
    if (serialization.TryCustomCopy<NearbyAccessRule>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Anchored, ref target2, hookCtx, false, context))
      target2 = this.Anchored;
    target.Anchored = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target3, hookCtx, false, context))
      target3 = this.Count;
    target.Count = target3;
    List<ProtoId<AccessLevelPrototype>> target4 = (List<ProtoId<AccessLevelPrototype>>) null;
    if (this.Access == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<AccessLevelPrototype>>>(this.Access, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<ProtoId<AccessLevelPrototype>>>(this.Access, hookCtx, context);
    target.Access = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NearbyAccessRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref RulesRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NearbyAccessRule target1 = (NearbyAccessRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (RulesRule) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NearbyAccessRule target1 = (NearbyAccessRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NearbyAccessRule RulesRule.Instantiate() => new NearbyAccessRule();
}
