// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Effects.SharedGravityAnomalySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Anomaly.Effects;

public abstract class SharedGravityAnomalySystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private SharedMapSystem _mapSystem;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GravityAnomalyComponent, AnomalyPulseEvent>(new ComponentEventRefHandler<GravityAnomalyComponent, AnomalyPulseEvent>((object) this, __methodptr(OnAnomalyPulse)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GravityAnomalyComponent, AnomalySupercriticalEvent>(new ComponentEventRefHandler<GravityAnomalyComponent, AnomalySupercriticalEvent>((object) this, __methodptr(OnSupercritical)), (Type[]) null, (Type[]) null);
  }

  private void OnAnomalyPulse(
    EntityUid uid,
    GravityAnomalyComponent component,
    ref AnomalyPulseEvent args)
  {
    TransformComponent transformComponent = this.Transform(uid);
    float num = component.MaxThrowRange * args.Severity * args.PowerModifier;
    float baseThrowSpeed = component.MaxThrowStrength * args.Severity * args.PowerModifier;
    HashSet<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(uid, num, (LookupFlags) 10);
    EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    Vector2 worldPosition = this._xform.GetWorldPosition(transformComponent, entityQuery1);
    EntityQuery<PhysicsComponent> entityQuery2 = this.GetEntityQuery<PhysicsComponent>();
    foreach (EntityUid uid1 in entitiesInRange)
    {
      PhysicsComponent physicsComponent;
      if (!entityQuery2.TryGetComponent(uid1, ref physicsComponent) || (physicsComponent.CollisionMask & 32 /*0x20*/) == 0)
      {
        Vector2 vector2 = this._xform.GetWorldPosition(uid1, entityQuery1) - worldPosition;
        this._throwing.TryThrow(uid1, vector2 * 10f, baseThrowSpeed, new EntityUid?(uid), 0.0f);
      }
    }
  }

  private void OnSupercritical(
    EntityUid uid,
    GravityAnomalyComponent component,
    ref AnomalySupercriticalEvent args)
  {
    TransformComponent transformComponent = this.Transform(uid);
    MapGridComponent mapGridComponent1;
    if (!this.TryComp<MapGridComponent>(transformComponent.GridUid, ref mapGridComponent1))
      return;
    Vector2 worldPosition = this._xform.GetWorldPosition(transformComponent);
    SharedMapSystem mapSystem1 = this._mapSystem;
    EntityUid? gridUid = transformComponent.GridUid;
    EntityUid entityUid1 = gridUid.Value;
    MapGridComponent mapGridComponent2 = mapGridComponent1;
    Circle circle = new Circle(worldPosition, component.SpaceRange);
    List<(Vector2i, Tile)> list = ((IEnumerable<TileRef>) mapSystem1.GetTilesIntersecting(entityUid1, mapGridComponent2, circle, true, (Predicate<TileRef>) null).ToArray<TileRef>()).Select<TileRef, (Vector2i, Tile)>((Func<TileRef, (Vector2i, Tile)>) (t => (t.GridIndices, Tile.Empty))).ToList<(Vector2i, Tile)>();
    SharedMapSystem mapSystem2 = this._mapSystem;
    gridUid = transformComponent.GridUid;
    EntityUid entityUid2 = gridUid.Value;
    MapGridComponent mapGridComponent3 = mapGridComponent1;
    List<(Vector2i, Tile)> valueTupleList = list;
    mapSystem2.SetTiles(entityUid2, mapGridComponent3, valueTupleList);
    float num = component.MaxThrowRange * 2f * args.PowerModifier;
    float baseThrowSpeed = component.MaxThrowStrength * 2f * args.PowerModifier;
    HashSet<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(uid, num, (LookupFlags) 10);
    EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    EntityQuery<PhysicsComponent> entityQuery2 = this.GetEntityQuery<PhysicsComponent>();
    foreach (EntityUid uid1 in entitiesInRange)
    {
      PhysicsComponent physicsComponent;
      if (!entityQuery2.TryGetComponent(uid1, ref physicsComponent) || (physicsComponent.CollisionMask & 32 /*0x20*/) == 0)
      {
        Vector2 vector2 = this._xform.GetWorldPosition(uid1, entityQuery1) - worldPosition;
        this._throwing.TryThrow(uid1, vector2 * 5f, baseThrowSpeed, new EntityUid?(uid), 0.0f);
      }
    }
  }
}
