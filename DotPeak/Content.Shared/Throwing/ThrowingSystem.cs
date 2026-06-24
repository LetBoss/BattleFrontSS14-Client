// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.ThrowingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Construction.Components;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Throwing;

public sealed class ThrowingSystem : EntitySystem
{
  public const float ThrowAngularImpulse = 5f;
  public const float PushbackDefault = 2f;
  public const float FlyTimePercentage = 0.8f;
  private const float TileFrictionMod = 1.5f;
  private float _frictionModifier;
  private float _airDamping;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private ThrownItemSystem _thrownSystem;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedMeleeWeaponSystem _melee;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RotateToFaceSystem _rotateToFace;
  private readonly SoundSpecifier _throwSound = (SoundSpecifier) new SoundCollectionSpecifier("RMCThrowing");

  public override void Initialize()
  {
    base.Initialize();
    this.Subs.CVar<float>(this._configManager, CCVars.TileFrictionModifier, (Action<float>) (value => this._frictionModifier = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.AirFriction, (Action<float>) (value => this._airDamping = value), true);
  }

  public void TryThrow(
    EntityUid uid,
    EntityCoordinates coordinates,
    float baseThrowSpeed = 10f,
    EntityUid? user = null,
    float pushbackRatio = 2f,
    float? friction = null,
    bool compensateFriction = false,
    bool recoil = true,
    bool animated = true,
    bool playSound = true,
    bool doSpin = true,
    bool unanchor = false)
  {
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(uid);
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(coordinates);
    if (mapCoordinates2.MapId != mapCoordinates1.MapId)
      return;
    this.TryThrow(uid, mapCoordinates2.Position - mapCoordinates1.Position, baseThrowSpeed, user, pushbackRatio, friction, compensateFriction, recoil, animated, playSound, doSpin, unanchor);
  }

  public void TryThrow(
    EntityUid uid,
    Vector2 direction,
    float baseThrowSpeed = 10f,
    EntityUid? user = null,
    float pushbackRatio = 2f,
    float? friction = null,
    bool compensateFriction = false,
    bool recoil = true,
    bool animated = true,
    bool playSound = true,
    bool doSpin = true,
    bool unanchor = false,
    bool rotate = true)
  {
    PhysicsComponent component;
    if (!this.GetEntityQuery<PhysicsComponent>().TryGetComponent(uid, out component))
      return;
    Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> entityQuery = this.GetEntityQuery<ProjectileComponent>();
    this.TryThrow(uid, direction, component, this.Transform(uid), entityQuery, baseThrowSpeed, user, pushbackRatio, friction, compensateFriction, recoil, animated, playSound, doSpin, unanchor, rotate);
  }

  public void TryThrow(
    EntityUid uid,
    Vector2 direction,
    PhysicsComponent physics,
    TransformComponent transform,
    Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> projectileQuery,
    float baseThrowSpeed = 10f,
    EntityUid? user = null,
    float pushbackRatio = 2f,
    float? friction = null,
    bool compensateFriction = false,
    bool recoil = true,
    bool animated = true,
    bool playSound = true,
    bool doSpin = true,
    bool unanchor = false,
    bool rotate = true)
  {
    if ((double) baseThrowSpeed <= 0.0 || direction == Vector2Helpers.Infinity || direction == Vector2Helpers.NaN || direction == Vector2.Zero)
      return;
    float? nullable1 = friction;
    float num1 = 0.0f;
    if ((double) nullable1.GetValueOrDefault() < (double) num1 & nullable1.HasValue)
      return;
    if (unanchor && this.HasComp<AnchorableComponent>(uid))
      this._transform.Unanchor(uid);
    ProjectileComponent component;
    if ((physics.BodyType & (BodyType.KinematicController | BodyType.Dynamic)) == BodyType.Kinematic || projectileQuery.TryGetComponent(uid, out component) && !component.OnlyCollideWhenShot)
      return;
    ThrownItemComponent thrownItemComponent1 = new ThrownItemComponent()
    {
      Thrower = user,
      Animate = animated
    };
    float num2 = (float) ((double) friction ?? (double) this._frictionModifier * 1.5);
    if ((double) num2 == 0.0)
      compensateFriction = false;
    float num3 = direction.Length() / baseThrowSpeed;
    if (compensateFriction)
      num3 *= 0.8f;
    thrownItemComponent1.ThrownTime = new TimeSpan?(this._gameTiming.CurTime);
    ThrownItemComponent thrownItemComponent2 = thrownItemComponent1;
    TimeSpan? nullable2 = thrownItemComponent1.ThrownTime;
    TimeSpan timeSpan = TimeSpan.FromSeconds((double) num3);
    TimeSpan? nullable3 = nullable2.HasValue ? new TimeSpan?(nullable2.GetValueOrDefault() + timeSpan) : new TimeSpan?();
    thrownItemComponent2.LandTime = nullable3;
    thrownItemComponent1.PlayLandSound = playSound;
    this.AddComp<ThrownItemComponent>(uid, thrownItemComponent1, true);
    ThrowingAngleComponent comp1 = (ThrowingAngleComponent) null;
    if (doSpin)
    {
      if ((double) physics.InvI > 0.0 && (!this.TryComp<ThrowingAngleComponent>(uid, out comp1) || comp1.AngularVelocity))
      {
        this._physics.ApplyAngularImpulse(uid, 5f / physics.InvI, body: physics);
      }
      else
      {
        this.Resolve<ThrowingAngleComponent>(uid, ref comp1, false);
        Angle worldRotation = this._transform.GetWorldRotation(transform.ParentUid);
        Angle angle1 = Angle.op_Subtraction(DirectionExtensions.ToWorldAngle(direction), worldRotation);
        Angle angle2 = comp1 != null ? comp1.Angle : Angle.Zero;
        this._transform.SetLocalRotation(uid, Angle.op_Addition(angle1, angle2));
      }
    }
    ThrownEvent args1 = new ThrownEvent(user, uid);
    this.RaiseLocalEvent<ThrownEvent>(uid, ref args1, true);
    if (user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(7, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user.Value), nameof (user), "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" threw ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "entity", "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Throw, LogImpact.Low, ref local);
    }
    float num4 = compensateFriction ? direction.Length() / (num3 + 1f / num2) : baseThrowSpeed;
    Vector2 impulse = Vector2Helpers.Normalized(direction) * num4 * physics.Mass;
    this._physics.ApplyLinearImpulse(uid, impulse, body: physics);
    if (thrownItemComponent1.LandTime.HasValue)
    {
      nullable2 = thrownItemComponent1.LandTime;
      TimeSpan zero = TimeSpan.Zero;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() <= zero ? 1 : 0) : 0) == 0)
      {
        this._physics.SetBodyStatus(uid, physics, BodyStatus.InAir);
        goto label_21;
      }
    }
    this._thrownSystem.LandComponent(uid, thrownItemComponent1, physics, playSound);
label_21:
    if (!user.HasValue)
      return;
    if (recoil)
    {
      Vector2 vector2 = Vector2.Transform(transform.LocalPosition + direction, this._transform.GetInvWorldMatrix(transform));
      if (rotate)
      {
        this._rotateToFace.TryFaceCoordinates(user.Value, this._transform.ToMapCoordinates(transform.Coordinates.Offset(direction)).Position);
        if (this._net.IsServer)
          this._audio.PlayPvs(this._throwSound, user.Value);
      }
      Angle localRotation = transform.LocalRotation;
      Vector2 localPos = ((Angle) ref localRotation).RotateVec(ref vector2);
      this._melee.DoLunge(user.Value, user.Value, Angle.Zero, localPos, (string) null, false);
    }
    PhysicsComponent comp2;
    if ((double) pushbackRatio != 0.0 && (double) physics.Mass > 0.0 && this.TryComp<PhysicsComponent>(user.Value, out comp2) && this._gravity.IsWeightless(user.Value, comp2))
    {
      ThrowPushbackAttemptEvent args2 = new ThrowPushbackAttemptEvent();
      this.RaiseLocalEvent<ThrowPushbackAttemptEvent>(uid, args2);
      if (!args2.Cancelled)
        this._physics.ApplyLinearImpulse(user.Value, -impulse / physics.Mass * pushbackRatio * MathF.Min(5f, physics.Mass), body: comp2);
    }
    foreach (ICommonSession recipient in Filter.PvsExcept(user.Value).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("throwing-user-threw-others", (nameof (user), (object) Identity.Name(user.Value, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), ("thrown", (object) Identity.Name(uid, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), user.Value, valueOrDefault, PopupType.SmallCaution);
      }
    }
  }
}
