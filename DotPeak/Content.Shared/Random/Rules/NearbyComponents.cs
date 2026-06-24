// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.NearbyComponentsRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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

public sealed class NearbyComponentsRule : 
  RulesRule,
  ISerializationGenerated<NearbyComponentsRule>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Anchored;
  [DataField(null, false, 1, false, false, null)]
  public int Count;
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry Components;
  [DataField(null, false, 1, false, false, null)]
  public float Range = 10f;

  public override bool Check(EntityManager entManager, EntityUid uid)
  {
    HashSet<Entity<IComponent>> entities = new HashSet<Entity<IComponent>>();
    EntityQuery<TransformComponent> entityQuery = entManager.GetEntityQuery<TransformComponent>();
    TransformComponent component1;
    if (!entityQuery.TryGetComponent(uid, out component1) || !component1.MapUid.HasValue)
      return false;
    SharedTransformSystem sharedTransformSystem = entManager.System<SharedTransformSystem>();
    EntityLookupSystem entityLookupSystem = entManager.System<EntityLookupSystem>();
    bool flag = false;
    TransformComponent component2 = component1;
    Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(component2);
    int num = 0;
    foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in this.Components.Values)
    {
      entities.Clear();
      entityLookupSystem.GetEntitiesInRange(componentRegistryEntry.Component.GetType(), component1.MapID, worldPosition, this.Range, entities);
      foreach (Entity<IComponent> uid1 in entities)
      {
        TransformComponent component3;
        if (!this.Anchored || entityQuery.TryGetComponent((EntityUid) uid1, out component3) && component3.Anchored)
        {
          ++num;
          if (num >= this.Count)
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
        break;
    }
    return !flag ? this.Inverted : !this.Inverted;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NearbyComponentsRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RulesRule target1 = (RulesRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NearbyComponentsRule) target1;
    if (serialization.TryCustomCopy<NearbyComponentsRule>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Anchored, ref target2, hookCtx, false, context))
      target2 = this.Anchored;
    target.Anchored = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target3, hookCtx, false, context))
      target3 = this.Count;
    target.Count = target3;
    ComponentRegistry target4 = (ComponentRegistry) null;
    if (this.Components == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Components, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ComponentRegistry>(this.Components, hookCtx, context);
    target.Components = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NearbyComponentsRule target,
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
    NearbyComponentsRule target1 = (NearbyComponentsRule) target;
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
    NearbyComponentsRule target1 = (NearbyComponentsRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NearbyComponentsRule RulesRule.Instantiate() => new NearbyComponentsRule();
}
