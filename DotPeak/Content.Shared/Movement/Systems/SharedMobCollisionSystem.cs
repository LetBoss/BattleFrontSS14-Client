// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedMobCollisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Movement.Systems;

public abstract class SharedMobCollisionSystem : EntitySystem
{
  [Dependency]
  protected IConfigurationManager CfgManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private MovementSpeedModifierSystem _moveMod;
  [Dependency]
  protected SharedPhysicsSystem Physics;
  [Dependency]
  private SharedTransformSystem _xformSystem;
  protected Robust.Shared.GameObjects.EntityQuery<MobCollisionComponent> MobQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> PhysicsQuery;
  private float _pushingCap;
  private float _pushingDotProduct;
  private float _minimumPushSquared = 0.01f;
  private float _penCap;
  public const float BufferTime = 0.2f;
  private float _massDiffCap;
  [Dependency]
  private RMCSizeStunSystem _rmcSizeStun;
  private Robust.Shared.GameObjects.EntityQuery<RMCMobCollisionMassComponent> _rmcMobCollisionMassQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoComponent> _xenoQuery;
  private float _penCapSubtract;
  private bool _bigXenosCancelMovement;

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatePushCap();
    this.Subs.CVar<int>(this.CfgManager, CVars.NetTickrate, (Action<int>) (_ => this.UpdatePushCap()));
    this.Subs.CVar<float>(this.CfgManager, CCVars.MovementMinimumPush, (Action<float>) (val => this._minimumPushSquared = val * val), true);
    this.Subs.CVar<float>(this.CfgManager, CCVars.MovementPenetrationCap, (Action<float>) (val => this._penCap = val), true);
    this.Subs.CVar<float>(this.CfgManager, CCVars.MovementPushingCap, (Action<float>) (_ => this.UpdatePushCap()));
    this.Subs.CVar<float>(this.CfgManager, CCVars.MovementPushingVelocityProduct, (Action<float>) (value => this._pushingDotProduct = value), true);
    this.Subs.CVar<float>(this.CfgManager, CCVars.MovementPushMassCap, (Action<float>) (val => this._massDiffCap = val), true);
    this.MobQuery = this.GetEntityQuery<MobCollisionComponent>();
    this.PhysicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.SubscribeAllEvent<SharedMobCollisionSystem.MobCollisionMessage>(new EntitySessionEventHandler<SharedMobCollisionSystem.MobCollisionMessage>(this.OnCollision));
    this.SubscribeLocalEvent<MobCollisionComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<MobCollisionComponent, RefreshMovementSpeedModifiersEvent>(this.OnMoveModifier));
    this.UpdatesBefore.Add(typeof (SharedPhysicsSystem));
    this._rmcMobCollisionMassQuery = this.GetEntityQuery<RMCMobCollisionMassComponent>();
    this._xenoQuery = this.GetEntityQuery<XenoComponent>();
    this.Subs.CVar<float>(this.CfgManager, RMCCVars.RMCMovementPenCapSubtract, (Action<float>) (v => this._penCapSubtract = v), true);
    this.Subs.CVar<bool>(this.CfgManager, RMCCVars.RMCMovementBigXenosCancelMovement, (Action<bool>) (v => this._bigXenosCancelMovement = v), true);
  }

  private void UpdatePushCap()
  {
    this._pushingCap = 1f / (float) this.CfgManager.GetCVar<int>(CVars.NetTickrate) * this.CfgManager.GetCVar<float>(CCVars.MovementPushingCap);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    AllEntityQueryEnumerator<MobCollisionComponent> entityQueryEnumerator = this.AllEntityQuery<MobCollisionComponent>();
    EntityUid uid;
    MobCollisionComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Colliding)
      {
        comp1.BufferAccumulator -= frameTime;
        this.DirtyField<MobCollisionComponent>(uid, comp1, "BufferAccumulator");
        Vector2 vector2 = comp1.Direction;
        if ((double) comp1.BufferAccumulator <= 0.0)
        {
          this.SetColliding((Entity<MobCollisionComponent>) (uid, comp1), false, 1f);
        }
        else
        {
          PhysicsComponent component;
          if (vector2 != Vector2.Zero && this.PhysicsQuery.TryComp(uid, out component))
          {
            if ((double) vector2.Length() > (double) this._pushingCap)
              vector2 = Vector2Helpers.Normalized(vector2) * this._pushingCap;
            this.Physics.ApplyLinearImpulse(uid, vector2 * component.Mass, body: component);
            comp1.Direction = Vector2.Zero;
            this.DirtyField<MobCollisionComponent>(uid, comp1, "Direction");
          }
        }
      }
    }
  }

  private void OnMoveModifier(
    Entity<MobCollisionComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!ent.Comp.Colliding)
      return;
    args.ModifySpeed(ent.Comp.SpeedModifier);
  }

  private void SetColliding(Entity<MobCollisionComponent> entity, bool value, float speedMod)
  {
    if (value)
    {
      entity.Comp.BufferAccumulator = 0.2f;
      this.DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp, "BufferAccumulator");
    }
    if (entity.Comp.Colliding != value)
    {
      entity.Comp.Colliding = value;
      this.DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp, "Colliding");
    }
    if (entity.Comp.SpeedModifier.Equals(speedMod))
      return;
    entity.Comp.SpeedModifier = speedMod;
    this._moveMod.RefreshMovementSpeedModifiers(entity.Owner);
    this.DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp, "SpeedModifier");
  }

  private void OnCollision(
    SharedMobCollisionSystem.MobCollisionMessage msg,
    EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    MobCollisionComponent component;
    if (!this.MobQuery.TryComp(attachedEntity, out component))
      return;
    TransformComponent transformComponent = this.Transform(attachedEntity.Value);
    EntityUid parentUid1 = transformComponent.ParentUid;
    EntityUid? nullable = transformComponent.GridUid;
    if ((nullable.HasValue ? (parentUid1 != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
    {
      EntityUid parentUid2 = transformComponent.ParentUid;
      nullable = transformComponent.MapUid;
      if ((nullable.HasValue ? (parentUid2 != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        return;
    }
    Vector2 direction = msg.Direction;
    this.MoveMob((Entity<MobCollisionComponent, TransformComponent>) (attachedEntity.Value, component, transformComponent), direction, msg.SpeedModifier);
  }

  protected void MoveMob(
    Entity<MobCollisionComponent, TransformComponent> entity,
    Vector2 direction,
    float speedMod)
  {
    bool flag = true;
    if ((double) direction.LengthSquared() < (double) this._minimumPushSquared)
    {
      flag = false;
      direction = Vector2.Zero;
      speedMod = 1f;
    }
    else if (float.IsNaN(direction.X) || float.IsNaN(direction.Y))
      direction = Vector2.Zero;
    speedMod = Math.Clamp(speedMod, 0.0f, 1f);
    this.SetColliding((Entity<MobCollisionComponent>) entity, flag, speedMod);
    if (direction == entity.Comp1.Direction)
      return;
    entity.Comp1.Direction = direction;
    this.DirtyField<MobCollisionComponent>(entity.Owner, entity.Comp1, "Direction");
  }

  protected bool HandleCollisions(
    Entity<MobCollisionComponent, PhysicsComponent> entity,
    float frameTime)
  {
    PhysicsComponent comp2 = entity.Comp2;
    if (comp2.ContactCount == 0)
      return false;
    Vector2 linearVelocity = entity.Comp2.LinearVelocity;
    if (linearVelocity == Vector2.Zero && !this.CfgManager.GetCVar<bool>(CCVars.MovementPushingStatic))
      return false;
    TransformComponent component1 = this.Transform(entity.Owner);
    EntityUid parentUid1 = component1.ParentUid;
    EntityUid? nullable = component1.GridUid;
    if ((nullable.HasValue ? (parentUid1 != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
    {
      EntityUid parentUid2 = component1.ParentUid;
      nullable = component1.MapUid;
      if ((nullable.HasValue ? (parentUid2 != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        return false;
    }
    AttemptMobCollideEvent args1 = new AttemptMobCollideEvent();
    this.RaiseLocalEvent<AttemptMobCollideEvent>(entity.Owner, ref args1);
    if (args1.Cancelled)
      return false;
    (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._xformSystem.GetWorldPositionRotation(component1);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
    ContactEnumerator contacts = this.Physics.GetContacts((Entity<FixturesComponent>) entity.Owner);
    Vector2 zero1 = Vector2.Zero;
    int num1 = 0;
    float fixturesMass = comp2.FixturesMass;
    float speedmodifier = 1f;
    Vector2 zero2 = Vector2.Zero;
    bool flag = this._xenoQuery.HasComp((EntityUid) entity);
    RMCSizes size1 = RMCSizes.Small;
    if (flag)
      this._rmcSizeStun.TryGetSize((EntityUid) entity, out size1);
    Contact contact;
    while (contacts.MoveNext(out contact))
    {
      if (contact.IsTouching && !(contact.OurFixture(entity.Owner).Id != entity.Comp1.FixtureId))
      {
        EntityUid entityUid = contact.OtherEnt(entity.Owner);
        MobCollisionComponent component2;
        PhysicsComponent component3;
        if (this.MobQuery.TryComp(entityUid, out component2) && this.PhysicsQuery.TryComp(entityUid, out component3) && (double) Vector2.Dot(linearVelocity, component3.LinearVelocity) >= (double) this._pushingDotProduct)
        {
          AttemptMobTargetCollideEvent args2 = new AttemptMobTargetCollideEvent((EntityUid) entity);
          this.RaiseLocalEvent<AttemptMobTargetCollideEvent>(entityUid, ref args2);
          if (!args2.Cancelled)
          {
            Robust.Shared.Physics.Transform physicsTransform = this.Physics.GetPhysicsTransform(entityUid);
            Vector2 vector2_1 = transform.Position - physicsTransform.Position;
            if (vector2_1 == Vector2.Zero)
              vector2_1 = this._random.NextVector2(0.01f);
            float num2 = Math.Clamp(this._penCapSubtract - vector2_1.Length(), 0.0f, this._penCap);
            Vector2 vector2_2 = num2 * Vector2Helpers.Normalized(vector2_1) * (entity.Comp1.Strength + component2.Strength);
            if ((double) this._massDiffCap > 0.0)
            {
              float num3 = component3.FixturesMass;
              RMCMobCollisionMassComponent component4;
              if (this._rmcMobCollisionMassQuery.TryComp(entityUid, out component4))
                num3 = component4.Mass;
              float num4 = Math.Clamp(num3 / fixturesMass, 1f / this._massDiffCap, this._massDiffCap);
              vector2_2 *= num4;
              speedmodifier = MathF.Min(Math.Clamp((float) (1.0 - (double) ((1f - entity.Comp1.MinimumSpeedModifier) / (this._penCap / num2)) * (double) num4), entity.Comp1.MinimumSpeedModifier, 1f), 1f);
            }
            zero1 += vector2_2;
            ++num1;
            RMCSizes size2;
            if (this._bigXenosCancelMovement & flag && size1 >= RMCSizes.Big && this._xenoQuery.HasComp(entityUid) && this._rmcSizeStun.TryGetSize(entityUid, out size2) && size2 < RMCSizes.Big)
              zero2 += vector2_2;
          }
        }
      }
    }
    if (zero1 == Vector2.Zero)
      return num1 > 0;
    if (zero2 != Vector2.Zero)
      zero1 -= zero2;
    Vector2 direction = zero1 * frameTime;
    this.RaiseCollisionEvent(entity.Owner, direction, speedmodifier);
    return true;
  }

  protected abstract void RaiseCollisionEvent(
    EntityUid uid,
    Vector2 direction,
    float speedmodifier);

  [NetSerializable]
  [Serializable]
  protected sealed class MobCollisionMessage : EntityEventArgs
  {
    public Vector2 Direction;
    public float SpeedModifier;
  }
}
