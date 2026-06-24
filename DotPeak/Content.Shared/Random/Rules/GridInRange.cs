// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.GridInRangeRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Random.Rules;

public sealed class GridInRangeRule : 
  RulesRule,
  ISerializationGenerated<GridInRangeRule>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Range = 10f;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

  public override bool Check(EntityManager entManager, EntityUid uid)
  {
    TransformComponent component1;
    if (!entManager.TryGetComponent<TransformComponent>(uid, out component1))
      return false;
    if (component1.GridUid.HasValue)
      return !this.Inverted;
    SharedTransformSystem sharedTransformSystem = entManager.System<SharedTransformSystem>();
    IMapManager mapManager = IoCManager.Resolve<IMapManager>();
    TransformComponent component2 = component1;
    Vector2 worldPosition = sharedTransformSystem.GetWorldPosition(component2);
    Vector2 vector2 = new Vector2(this.Range, this.Range);
    this._grids.Clear();
    mapManager.FindGridsIntersecting(component1.MapID, new Box2(worldPosition - vector2, worldPosition + vector2), ref this._grids);
    return this._grids.Count > 0 ? !this.Inverted : this.Inverted;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GridInRangeRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RulesRule target1 = (RulesRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GridInRangeRule) target1;
    if (serialization.TryCustomCopy<GridInRangeRule>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GridInRangeRule target,
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
    GridInRangeRule target1 = (GridInRangeRule) target;
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
    GridInRangeRule target1 = (GridInRangeRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GridInRangeRule RulesRule.Instantiate() => new GridInRangeRule();
}
