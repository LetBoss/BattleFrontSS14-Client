// Decompiled with JetBrains decompiler
// Type: Content.Shared.Friction.TileFrictionController
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.Gravity;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Friction;

public sealed class TileFrictionController : VirtualController
{
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private ITileDefinitionManager _tileDefinitionManager;
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private SharedMapSystem _map;
  private Robust.Shared.GameObjects.EntityQuery<TileFrictionModifierComponent> _frictionQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  private Robust.Shared.GameObjects.EntityQuery<PullerComponent> _pullerQuery;
  private Robust.Shared.GameObjects.EntityQuery<PullableComponent> _pullableQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private float _frictionModifier;
  private float _minDamping;
  private float _airDamping;
  private float _offGridDamping;

  public override void Initialize()
  {
    base.Initialize();
    this.Subs.CVar<float>(this._configManager, CCVars.TileFrictionModifier, (Action<float>) (value => this._frictionModifier = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.MinFriction, (Action<float>) (value => this._minDamping = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.AirFriction, (Action<float>) (value => this._airDamping = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.OffgridFriction, (Action<float>) (value => this._offGridDamping = value), true);
    this._frictionQuery = this.GetEntityQuery<TileFrictionModifierComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this._pullerQuery = this.GetEntityQuery<PullerComponent>();
    this._pullableQuery = this.GetEntityQuery<PullableComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
  }

  public override void UpdateBeforeSolve(bool prediction, float frameTime)
  {
    base.UpdateBeforeSolve(prediction, frameTime);
    foreach (Entity<PhysicsComponent, TransformComponent> awakeBody in this.PhysicsSystem.AwakeBodies)
    {
      EntityUid owner = awakeBody.Owner;
      PhysicsComponent comp1 = awakeBody.Comp1;
      if ((!prediction || comp1.Predict) && !this._mover.UseMobMovement(owner) && (!comp1.LinearVelocity.Equals(Vector2.Zero) || !comp1.AngularVelocity.Equals(0.0f)))
      {
        TransformComponent comp2 = awakeBody.Comp2;
        float num = comp1.BodyStatus == BodyStatus.InAir || this._gravity.IsWeightless(owner, comp1, comp2) || !comp2.Coordinates.IsValid((IEntityManager) this.EntityManager) ? (!comp2.GridUid.HasValue || !this._gridQuery.HasComp(comp2.GridUid) ? this._offGridDamping : this._airDamping) : this._frictionModifier * this.GetTileFriction(owner, comp1, comp2);
        float modifier1 = 1f;
        TileFrictionModifierComponent component1;
        if (this._frictionQuery.TryGetComponent(owner, out component1))
          modifier1 = component1.Modifier;
        TileFrictionEvent args = new TileFrictionEvent(modifier1);
        this.RaiseLocalEvent<TileFrictionEvent>(owner, ref args);
        float modifier2 = args.Modifier;
        PullerComponent component2;
        PullableComponent component3;
        if (this._pullerQuery.TryGetComponent(owner, out component2) && component2.Pulling.HasValue && this._pullableQuery.TryGetComponent(owner, out component3) && component3.BeingPulled)
          modifier2 *= 0.2f;
        float friction = Math.Max(this._minDamping, num * modifier2);
        this.PhysicsSystem.SetLinearDamping(owner, comp1, friction);
        this.PhysicsSystem.SetAngularDamping(owner, comp1, friction);
        if (comp1.BodyType == BodyType.KinematicController)
        {
          Vector2 linearVelocity = comp1.LinearVelocity;
          float angularVelocity = comp1.AngularVelocity;
          this._mover.Friction(0.0f, frameTime, friction, ref linearVelocity);
          this._mover.Friction(0.0f, frameTime, friction, ref angularVelocity);
          this.PhysicsSystem.SetLinearVelocity(owner, linearVelocity, body: comp1);
          this.PhysicsSystem.SetAngularVelocity(owner, angularVelocity, body: comp1);
        }
      }
    }
  }

  private float GetTileFriction(EntityUid uid, PhysicsComponent body, TransformComponent xform)
  {
    float tileFriction = 1f;
    MapGridComponent component1;
    if (!this._gridQuery.TryGetComponent(xform.GridUid, out component1))
    {
      TileFrictionModifierComponent component2;
      return !this._frictionQuery.TryGetComponent(xform.MapUid, out component2) ? tileFriction : component2.Modifier;
    }
    TileRef tileRef = this._map.GetTileRef(xform.GridUid.Value, component1, xform.Coordinates);
    GravityComponent comp;
    if (tileRef.Tile.IsEmpty && this.HasComp<MapComponent>(xform.GridUid) && (!this.TryComp<GravityComponent>(xform.GridUid, out comp) || comp.Enabled))
      return tileFriction;
    AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(xform.GridUid.Value, component1, tileRef.GridIndices);
    EntityUid? uid1;
    while (entitiesEnumerator.MoveNext(out uid1))
    {
      TileFrictionModifierComponent component3;
      if (this._frictionQuery.TryGetComponent(uid1, out component3))
        tileFriction *= component3.Modifier;
    }
    return this._tileDefinitionManager[tileRef.Tile.TypeId].Friction * tileFriction;
  }

  public void SetModifier(EntityUid entityUid, float value, TileFrictionModifierComponent? friction = null)
  {
    if (!this.Resolve<TileFrictionModifierComponent>(entityUid, ref friction) || value.Equals(friction.Modifier))
      return;
    friction.Modifier = value;
    this.Dirty(entityUid, (IComponent) friction);
  }
}
