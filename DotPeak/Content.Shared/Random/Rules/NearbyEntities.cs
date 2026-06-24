// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.NearbyEntitiesRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Random.Rules;

public sealed class NearbyEntitiesRule : 
  RulesRule,
  ISerializationGenerated<NearbyEntitiesRule>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Count = 1;
  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist Whitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  public float Range = 10f;

  public override bool Check(EntityManager entManager, EntityUid uid)
  {
    TransformComponent component1;
    if (!entManager.TryGetComponent<TransformComponent>(uid, out component1) || !component1.MapUid.HasValue)
      return false;
    SharedTransformSystem sharedTransformSystem = entManager.System<SharedTransformSystem>();
    EntityLookupSystem entityLookupSystem = entManager.System<EntityLookupSystem>();
    EntityWhitelistSystem entityWhitelistSystem = entManager.System<EntityWhitelistSystem>();
    bool flag = false;
    TransformComponent component2 = component1;
    Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(component2);
    int num = 0;
    foreach (EntityUid uid1 in entityLookupSystem.GetEntitiesInRange(component1.MapID, worldPosition, this.Range))
    {
      if (!entityWhitelistSystem.IsWhitelistFail(this.Whitelist, uid1))
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
    ref NearbyEntitiesRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RulesRule target1 = (RulesRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NearbyEntitiesRule) target1;
    if (serialization.TryCustomCopy<NearbyEntitiesRule>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target2, hookCtx, false, context))
      target2 = this.Count;
    target.Count = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context, true);
    }
    target.Whitelist = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target4, hookCtx, false, context))
      target4 = this.Range;
    target.Range = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NearbyEntitiesRule target,
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
    NearbyEntitiesRule target1 = (NearbyEntitiesRule) target;
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
    NearbyEntitiesRule target1 = (NearbyEntitiesRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NearbyEntitiesRule RulesRule.Instantiate() => new NearbyEntitiesRule();
}
