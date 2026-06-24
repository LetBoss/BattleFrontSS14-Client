// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Movement.SharedRMCLagCompensationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared.Coordinates;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Movement;

public abstract class SharedRMCLagCompensationSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<ActorComponent> _actorQuery;
  private readonly Dictionary<NetUserId, GameTick> _lastRealTicks = new Dictionary<NetUserId, GameTick>();
  private GameTick _lastSentRealTick;

  public float MarginTiles { get; private set; }

  public override void Initialize()
  {
    base.Initialize();
    this._actorQuery = this.GetEntityQuery<ActorComponent>();
    this.SubscribeNetworkEvent<RMCSetLastRealTickEvent>(new EntitySessionEventHandler<RMCSetLastRealTickEvent>(this.OnSetLastRealTick));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCLagCompensationMarginTiles, (Action<float>) (v => this.MarginTiles = v), true);
  }

  private void OnSetLastRealTick(RMCSetLastRealTickEvent msg, EntitySessionEventArgs args)
  {
    this.SetLastRealTick(args.SenderSession.UserId, msg.Tick - 1U);
  }

  public virtual (EntityCoordinates Coordinates, Angle Angle) GetCoordinatesAngle(
    EntityUid uid,
    ICommonSession? pSession,
    TransformComponent? xform = null)
  {
    return !this.Resolve(uid, ref xform) ? (EntityCoordinates.Invalid, Angle.Zero) : (xform.Coordinates, xform.LocalRotation);
  }

  public virtual Angle GetAngle(EntityUid uid, ICommonSession? session, TransformComponent? xform = null)
  {
    return this.GetCoordinatesAngle(uid, session, xform).Angle;
  }

  public virtual EntityCoordinates GetCoordinates(
    EntityUid uid,
    ICommonSession? session,
    TransformComponent? xform = null)
  {
    return this.GetCoordinatesAngle(uid, session, xform).Coordinates;
  }

  public EntityCoordinates GetCoordinates(
    EntityUid uid,
    EntityUid? session,
    TransformComponent? xform = null)
  {
    ActorComponent component;
    return !this._actorQuery.TryComp(session, out component) ? this.GetCoordinates(uid, (ICommonSession) null, xform) : this.GetCoordinates(uid, component.PlayerSession, xform);
  }

  public bool IsWithinMargin(
    Entity<TransformComponent?> sessionEnt,
    Entity<TransformComponent?> lagCompensatedTarget,
    ICommonSession? session,
    float range)
  {
    EntityCoordinates coordinates1 = this.GetCoordinates((EntityUid) lagCompensatedTarget, session);
    if (this._net.IsServer)
    {
      EntityCoordinates coordinates2 = lagCompensatedTarget.Owner.ToCoordinates();
      if (!this._transform.InRange(coordinates1, coordinates2, 0.01f))
        range += this.MarginTiles;
    }
    return this._transform.InRange(sessionEnt.Owner.ToCoordinates(), coordinates1, range);
  }

  public virtual GameTick GetLastRealTick(NetUserId? session)
  {
    return session.HasValue ? this._lastRealTicks.GetValueOrDefault<NetUserId, GameTick>(session.Value, this._timing.CurTick) : this._timing.CurTick;
  }

  public void SetLastRealTick(NetUserId session, GameTick tick)
  {
    GameTick gameTick;
    if (this._net.IsClient || this._lastRealTicks.TryGetValue(session, out gameTick) && tick <= gameTick)
      return;
    this._lastRealTicks[session] = tick;
  }

  public void SendLastRealTick()
  {
    if (this._net.IsServer)
      return;
    GameTick lastRealTick = this.GetLastRealTick(new NetUserId?());
    if (lastRealTick == this._lastSentRealTick)
      return;
    this._lastSentRealTick = lastRealTick;
    this.RaiseNetworkEvent((EntityEventArgs) new RMCSetLastRealTickEvent(lastRealTick));
  }

  public bool Collides(
    Entity<FixturesComponent?> target,
    Entity<PhysicsComponent?> projectile,
    MapCoordinates targetCoordinates,
    Angle targetAngle = default (Angle))
  {
    if (!this.Resolve<FixturesComponent>((EntityUid) target, ref target.Comp, false) || !this.Resolve<PhysicsComponent>((EntityUid) projectile, ref projectile.Comp, false))
      return false;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) projectile);
    Vector2 position = mapCoordinates.Position;
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(targetCoordinates.Position, targetAngle);
    Box2 box2_1;
    // ISSUE: explicit constructor call
    ((Box2) ref box2_1).\u002Ector(transform.Position, transform.Position);
    foreach (Fixture fixture in target.Comp.Fixtures.Values)
    {
      if ((fixture.CollisionLayer & projectile.Comp.CollisionMask) != 0)
      {
        for (int childIndex = 0; childIndex < fixture.Shape.ChildCount; ++childIndex)
        {
          Box2 aabb = fixture.Shape.ComputeAABB(transform, childIndex);
          box2_1 = ((Box2) ref box2_1).Union(ref aabb);
        }
      }
    }
    if (((Box2) ref box2_1).Contains(position, true))
      return true;
    Vector2 linearVelocity = this._physics.GetLinearVelocity((EntityUid) projectile, projectile.Comp.LocalCenter);
    Vector2 vector2_1 = mapCoordinates.Position + linearVelocity / (float) this._timing.TickRate / 1.5f;
    if (((Box2) ref box2_1).Contains(vector2_1, true))
      return true;
    Vector2 vector2_2 = Vector2.Min(position, vector2_1);
    Vector2 vector2_3 = Vector2.Max(position, vector2_1);
    Box2 box2_2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2_2).\u002Ector(vector2_2, vector2_3);
    return ((Box2) ref box2_1).Intersects(ref box2_2) || (double) (((Box2) ref box2_1).ClosestPoint(ref vector2_1) - vector2_1).LengthSquared() <= (double) this.MarginTiles * (double) this.MarginTiles;
  }

  public bool Collides(
    Entity<FixturesComponent?> target,
    Entity<PhysicsComponent?> projectile,
    ICommonSession session)
  {
    (EntityCoordinates entityCoordinates, Angle angle) = this.GetCoordinatesAngle((EntityUid) target, session);
    return this.Collides(target, projectile, this._transform.ToMapCoordinates(entityCoordinates), angle);
  }
}
