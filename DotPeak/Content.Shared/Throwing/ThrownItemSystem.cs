// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.ThrownItemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Throwing;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.Movement.Pulling.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Throwing;

public sealed class ThrownItemSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private FixtureSystem _fixtures;
  [Dependency]
  private SharedBroadphaseSystem _broadphase;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedGravitySystem _gravity;
  private const string ThrowingFixture = "throw-fixture";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ThrownItemComponent, MapInitEvent>(new ComponentEventHandler<ThrownItemComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ThrownItemComponent, PhysicsSleepEvent>(new ComponentEventRefHandler<ThrownItemComponent, PhysicsSleepEvent>(this.OnSleep));
    this.SubscribeLocalEvent<ThrownItemComponent, StartCollideEvent>(new ComponentEventRefHandler<ThrownItemComponent, StartCollideEvent>(this.HandleCollision));
    this.SubscribeLocalEvent<ThrownItemComponent, PreventCollideEvent>(new ComponentEventRefHandler<ThrownItemComponent, PreventCollideEvent>(this.PreventCollision));
    this.SubscribeLocalEvent<ThrownItemComponent, ThrownEvent>(new ComponentEventRefHandler<ThrownItemComponent, ThrownEvent>(this.ThrowItem));
    this.SubscribeLocalEvent<PullStartedMessage>(new EntityEventHandler<PullStartedMessage>(this.HandlePullStarted));
  }

  private void OnMapInit(EntityUid uid, ThrownItemComponent component, MapInitEvent args)
  {
    ThrownItemComponent thrownItemComponent = component;
    thrownItemComponent.ThrownTime.GetValueOrDefault();
    if (thrownItemComponent.ThrownTime.HasValue)
      return;
    TimeSpan curTime = this._gameTiming.CurTime;
    thrownItemComponent.ThrownTime = new TimeSpan?(curTime);
  }

  private void ThrowItem(EntityUid uid, ThrownItemComponent component, ref ThrownEvent @event)
  {
    FixturesComponent comp1;
    PhysicsComponent comp2;
    if (!this.TryComp<FixturesComponent>(uid, out comp1) || comp1.Fixtures.Count != 1 || !this.TryComp<PhysicsComponent>(uid, out comp2))
      return;
    IPhysShape shape = comp1.Fixtures.Values.First<Fixture>().Shape;
    this._fixtures.TryCreateFixture(uid, shape, "throw-fixture", hard: false, collisionMask: 74, manager: comp1, body: comp2);
  }

  private void HandleCollision(
    EntityUid uid,
    ThrownItemComponent component,
    ref StartCollideEvent args)
  {
    if (!args.OtherFixture.Hard)
      return;
    EntityUid otherEntity = args.OtherEntity;
    EntityUid? thrower = component.Thrower;
    if ((thrower.HasValue ? (otherEntity == thrower.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    this.ThrowCollideInteraction(component, args.OurEntity, args.OtherEntity);
  }

  private void PreventCollision(
    EntityUid uid,
    ThrownItemComponent component,
    ref PreventCollideEvent args)
  {
    if (this.HasComp<ThrownHitUserComponent>(uid))
      return;
    EntityUid otherEntity = args.OtherEntity;
    EntityUid? thrower = component.Thrower;
    if ((thrower.HasValue ? (otherEntity == thrower.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  private void OnSleep(EntityUid uid, ThrownItemComponent thrownItem, ref PhysicsSleepEvent @event)
  {
    this.StopThrow(uid, thrownItem);
  }

  private void HandlePullStarted(PullStartedMessage message)
  {
    ThrownItemComponent comp;
    if (!this.TryComp<ThrownItemComponent>(message.PulledUid, out comp))
      return;
    this.StopThrow(message.PulledUid, comp);
  }

  public void StopThrow(EntityUid uid, ThrownItemComponent thrownItemComponent)
  {
    PhysicsComponent comp1;
    if (this.TryComp<PhysicsComponent>(uid, out comp1))
    {
      this._physics.SetBodyStatus(uid, comp1, BodyStatus.OnGround);
      if (comp1.Awake)
        this._broadphase.RegenerateContacts((Entity<PhysicsComponent, FixturesComponent, TransformComponent>) (uid, comp1));
    }
    FixturesComponent comp2;
    if (this.TryComp<FixturesComponent>(uid, out comp2))
    {
      Fixture fixtureOrNull = this._fixtures.GetFixtureOrNull(uid, "throw-fixture", comp2);
      if (fixtureOrNull != null)
        this._fixtures.DestroyFixture(uid, "throw-fixture", fixtureOrNull, manager: comp2);
    }
    StopThrowEvent args = new StopThrowEvent(thrownItemComponent.Thrower);
    this.RaiseLocalEvent<StopThrowEvent>(uid, ref args);
    this.RemComp<ThrownItemComponent>(uid);
  }

  public void LandComponent(
    EntityUid uid,
    ThrownItemComponent thrownItem,
    PhysicsComponent physics,
    bool playSound)
  {
    if (thrownItem.Landed || thrownItem.Deleted || this._gravity.IsWeightless(uid) || this.Deleted(uid))
      return;
    thrownItem.Landed = true;
    if (thrownItem.Thrower.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(19, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "entity", "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" thrown by ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) thrownItem.Thrower.Value), "thrower", "ToPrettyString(thrownItem.Thrower.Value)");
      logStringHandler.AppendLiteral(" landed.");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Landed, LogImpact.Low, ref local);
    }
    this._broadphase.RegenerateContacts((Entity<PhysicsComponent, FixturesComponent, TransformComponent>) (uid, physics));
    LandEvent args = new LandEvent(thrownItem.Thrower, playSound);
    this.RaiseLocalEvent<LandEvent>(uid, ref args);
  }

  public void ThrowCollideInteraction(
    ThrownItemComponent component,
    EntityUid thrown,
    EntityUid target)
  {
    if (component.Thrower.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(17, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) thrown), nameof (thrown), "ToPrettyString(thrown)");
      logStringHandler.AppendLiteral(" thrown by ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) component.Thrower.Value), "thrower", "ToPrettyString(component.Thrower.Value)");
      logStringHandler.AppendLiteral(" hit ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.ThrowHit, LogImpact.Low, ref local);
    }
    this.RaiseLocalEvent<ThrowHitByEvent>(target, new ThrowHitByEvent(thrown, target, component), true);
    this.RaiseLocalEvent<ThrowDoHitEvent>(thrown, new ThrowDoHitEvent(thrown, target, component), true);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<ThrownItemComponent, PhysicsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ThrownItemComponent, PhysicsComponent>();
    EntityUid uid;
    ThrownItemComponent comp1;
    PhysicsComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!this._netMan.IsClient || comp2.Predict)
      {
        TimeSpan? nullable = comp1.LandTime;
        TimeSpan curTime1 = this._gameTiming.CurTime;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() <= curTime1 ? 1 : 0) : 0) != 0)
          this.LandComponent(uid, comp1, comp2, comp1.PlayLandSound);
        nullable = comp1.LandTime;
        nullable = nullable ?? comp1.ThrownTime;
        TimeSpan curTime2 = this._gameTiming.CurTime;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() <= curTime2 ? 1 : 0) : 0) != 0)
          this.StopThrow(uid, comp1);
      }
    }
  }
}
