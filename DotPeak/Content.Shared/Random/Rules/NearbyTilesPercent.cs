// Decompiled with JetBrains decompiler
// Type: Content.Shared.Random.Rules.NearbyTilesPercentRule
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Random.Rules;

public sealed class NearbyTilesPercentRule : 
  RulesRule,
  ISerializationGenerated<NearbyTilesPercentRule>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreAnchored;
  [DataField(null, false, 1, true, false, null)]
  public float Percent;
  [DataField(null, false, 1, true, false, null)]
  public List<ProtoId<ContentTileDefinition>> Tiles = new List<ProtoId<ContentTileDefinition>>();
  [DataField(null, false, 1, false, false, null)]
  public float Range = 10f;

  public override bool Check(EntityManager entManager, EntityUid uid)
  {
    TransformComponent component1;
    MapGridComponent component2;
    if (!entManager.TryGetComponent<TransformComponent>(uid, out component1) || !entManager.TryGetComponent<MapGridComponent>(component1.GridUid, out component2))
      return false;
    SharedTransformSystem sharedTransformSystem = entManager.System<SharedTransformSystem>();
    SharedMapSystem sharedMapSystem = entManager.System<SharedMapSystem>();
    ITileDefinitionManager definitionManager = IoCManager.Resolve<ITileDefinitionManager>();
    EntityQuery<PhysicsComponent> entityQuery = entManager.GetEntityQuery<PhysicsComponent>();
    int num1 = 0;
    int num2 = 0;
    foreach (TileRef tileRef in sharedMapSystem.GetTilesIntersecting(component1.GridUid.Value, component2, new Circle(sharedTransformSystem.GetWorldPosition(component1), this.Range)))
    {
      if (this.IgnoreAnchored)
      {
        AnchoredEntitiesEnumerator entitiesEnumerator = sharedMapSystem.GetAnchoredEntitiesEnumerator(component1.GridUid.Value, component2, tileRef.GridIndices);
        bool flag = false;
        EntityUid? uid1;
        while (entitiesEnumerator.MoveNext(out uid1))
        {
          PhysicsComponent component3;
          if (entityQuery.TryGetComponent(uid1, out component3) && component3.CanCollide)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          continue;
      }
      ++num1;
      if (this.Tiles.Contains((ProtoId<ContentTileDefinition>) definitionManager[tileRef.Tile.TypeId].ID))
        ++num2;
    }
    return num1 == 0 || (double) num2 / (double) num1 < (double) this.Percent ? this.Inverted : !this.Inverted;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NearbyTilesPercentRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RulesRule target1 = (RulesRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NearbyTilesPercentRule) target1;
    if (serialization.TryCustomCopy<NearbyTilesPercentRule>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreAnchored, ref target2, hookCtx, false, context))
      target2 = this.IgnoreAnchored;
    target.IgnoreAnchored = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Percent, ref target3, hookCtx, false, context))
      target3 = this.Percent;
    target.Percent = target3;
    List<ProtoId<ContentTileDefinition>> target4 = (List<ProtoId<ContentTileDefinition>>) null;
    if (this.Tiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<ContentTileDefinition>>>(this.Tiles, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<ProtoId<ContentTileDefinition>>>(this.Tiles, hookCtx, context);
    target.Tiles = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NearbyTilesPercentRule target,
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
    NearbyTilesPercentRule target1 = (NearbyTilesPercentRule) target;
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
    NearbyTilesPercentRule target1 = (NearbyTilesPercentRule) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NearbyTilesPercentRule RulesRule.Instantiate() => new NearbyTilesPercentRule();
}
