// Decompiled with JetBrains decompiler
// Type: Content.Shared.StepTrigger.Systems.StepTriggerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Gravity;
using Content.Shared.StepTrigger.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using System;

#nullable enable
namespace Content.Shared.StepTrigger.Systems;

public sealed class StepTriggerSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    this.UpdatesOutsidePrediction = true;
    this.SubscribeLocalEvent<StepTriggerComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<StepTriggerComponent, AfterAutoHandleStateEvent>(this.TriggerHandleState));
    this.SubscribeLocalEvent<StepTriggerComponent, StartCollideEvent>(new ComponentEventRefHandler<StepTriggerComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<StepTriggerComponent, EndCollideEvent>(new ComponentEventRefHandler<StepTriggerComponent, EndCollideEvent>(this.OnEndCollide));
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> entityQuery = this.GetEntityQuery<PhysicsComponent>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<StepTriggerActiveComponent, StepTriggerComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<StepTriggerActiveComponent, StepTriggerComponent, TransformComponent>();
    EntityUid uid;
    StepTriggerActiveComponent comp1;
    StepTriggerComponent comp2;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      if (this.Update(uid, comp2, comp3, entityQuery))
        this.RemCompDeferred(uid, (IComponent) comp1);
    }
  }

  private bool Update(
    EntityUid uid,
    StepTriggerComponent component,
    TransformComponent transform,
    Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> query)
  {
    if (!component.Active || component.Colliding.Count == 0)
      return true;
    MapGridComponent comp;
    if (component.Blacklist != null && this.TryComp<MapGridComponent>(transform.GridUid, out comp))
    {
      Vector2i tile = this._map.LocalToTile(transform.GridUid.Value, comp, transform.Coordinates);
      AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(uid, comp, tile);
      EntityUid? uid1;
      while (entitiesEnumerator.MoveNext(out uid1))
      {
        EntityUid? nullable = uid1;
        EntityUid entityUid = uid;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0 && this._whitelistSystem.IsBlacklistPass(component.Blacklist, uid1.Value))
          return false;
      }
    }
    foreach (EntityUid otherUid in component.Colliding)
      this.UpdateColliding(uid, component, transform, otherUid, query);
    return false;
  }

  private void UpdateColliding(
    EntityUid uid,
    StepTriggerComponent component,
    TransformComponent ownerXform,
    EntityUid otherUid,
    Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> query)
  {
    PhysicsComponent component1;
    if (!query.TryGetComponent(otherUid, out component1))
      return;
    TransformComponent transformComponent = this.Transform(otherUid);
    Box2 aabbNoContainer1 = this._entityLookup.GetAABBNoContainer(uid, ownerXform.LocalPosition, ownerXform.LocalRotation);
    Box2 aabbNoContainer2 = this._entityLookup.GetAABBNoContainer(otherUid, transformComponent.LocalPosition, transformComponent.LocalRotation);
    if (!((Box2) ref aabbNoContainer1).Intersects(ref aabbNoContainer2))
    {
      if (!component.CurrentlySteppedOn.Remove(otherUid))
        return;
      this.Dirty(uid, (IComponent) component);
    }
    else
    {
      Box2 box2 = ((Box2) ref aabbNoContainer2).Intersect(ref aabbNoContainer1);
      float num1 = Box2.Area(ref box2);
      float num2 = Math.Max(num1 / Box2.Area(ref aabbNoContainer2), num1 / Box2.Area(ref aabbNoContainer1));
      if ((double) component1.LinearVelocity.Length() < (double) component.RequiredTriggeredSpeed || component.CurrentlySteppedOn.Contains(otherUid) || (double) num2 < (double) component.IntersectRatio || !this.CanTrigger(uid, otherUid, component))
        return;
      if (component.StepOn)
      {
        StepTriggeredOnEvent args = new StepTriggeredOnEvent(uid, otherUid);
        this.RaiseLocalEvent<StepTriggeredOnEvent>(uid, ref args);
      }
      else
      {
        StepTriggeredOffEvent args = new StepTriggeredOffEvent(uid, otherUid);
        this.RaiseLocalEvent<StepTriggeredOffEvent>(uid, ref args);
      }
      component.CurrentlySteppedOn.Add(otherUid);
      this.Dirty(uid, (IComponent) component);
    }
  }

  private bool CanTrigger(EntityUid uid, EntityUid otherUid, StepTriggerComponent component)
  {
    PhysicsComponent comp;
    if (!component.Active || component.CurrentlySteppedOn.Contains(otherUid) || !component.IgnoreWeightless && this.TryComp<PhysicsComponent>(otherUid, out comp) && (comp.BodyStatus == BodyStatus.InAir || this._gravity.IsWeightless(otherUid, comp)))
      return false;
    StepTriggerAttemptEvent args = new StepTriggerAttemptEvent()
    {
      Source = uid,
      Tripper = otherUid
    };
    this.RaiseLocalEvent<StepTriggerAttemptEvent>(uid, ref args);
    return args.Continue && !args.Cancelled;
  }

  private void OnStartCollide(
    EntityUid uid,
    StepTriggerComponent component,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (!args.OtherFixture.Hard || !this.CanTrigger(uid, otherEntity, component))
      return;
    this.EnsureComp<StepTriggerActiveComponent>(uid);
    if (!component.Colliding.Add(otherEntity))
      return;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnEndCollide(
    EntityUid uid,
    StepTriggerComponent component,
    ref EndCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (!component.Colliding.Remove(otherEntity))
      return;
    component.CurrentlySteppedOn.Remove(otherEntity);
    this.Dirty(uid, (IComponent) component);
    if (component.StepOn)
    {
      StepTriggeredOffEvent args1 = new StepTriggeredOffEvent(uid, otherEntity);
      this.RaiseLocalEvent<StepTriggeredOffEvent>(uid, ref args1);
    }
    if (component.Colliding.Count != 0)
      return;
    this.RemCompDeferred<StepTriggerActiveComponent>(uid);
  }

  private void TriggerHandleState(
    EntityUid uid,
    StepTriggerComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    if (component.Colliding.Count > 0)
      this.EnsureComp<StepTriggerActiveComponent>(uid);
    else
      this.RemCompDeferred<StepTriggerActiveComponent>(uid);
  }

  public void SetIntersectRatio(EntityUid uid, float ratio, StepTriggerComponent? component = null)
  {
    if (!this.Resolve<StepTriggerComponent>(uid, ref component) || MathHelper.CloseToPercent(component.IntersectRatio, ratio, 1E-05))
      return;
    component.IntersectRatio = ratio;
    this.Dirty(uid, (IComponent) component);
  }

  public void SetRequiredTriggerSpeed(EntityUid uid, float speed, StepTriggerComponent? component = null)
  {
    if (!this.Resolve<StepTriggerComponent>(uid, ref component) || MathHelper.CloseToPercent(component.RequiredTriggeredSpeed, speed, 1E-05))
      return;
    component.RequiredTriggeredSpeed = speed;
    this.Dirty(uid, (IComponent) component);
  }

  public void SetActive(EntityUid uid, bool active, StepTriggerComponent? component = null)
  {
    if (!this.Resolve<StepTriggerComponent>(uid, ref component) || active == component.Active)
      return;
    component.Active = active;
    this.Dirty(uid, (IComponent) component);
  }
}
