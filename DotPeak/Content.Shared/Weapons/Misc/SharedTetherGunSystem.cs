// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Misc.SharedTetherGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Throwing;
using Content.Shared.Toggleable;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Misc;

public abstract class SharedTetherGunSystem : EntitySystem
{
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedJointSystem _joints;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected SharedTransformSystem TransformSystem;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private ThrownItemSystem _thrown;
  private const string TetherJoint = "tether";
  private const float SpinVelocity = 3.14159274f;
  private const float AngularChange = 1f;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TetherGunComponent, ActivateInWorldEvent>(new ComponentEventHandler<TetherGunComponent, ActivateInWorldEvent>(this.OnTetherActivate));
    this.SubscribeLocalEvent<TetherGunComponent, AfterInteractEvent>(new ComponentEventHandler<TetherGunComponent, AfterInteractEvent>(this.OnTetherRanged));
    this.SubscribeAllEvent<SharedTetherGunSystem.RequestTetherMoveEvent>(new EntitySessionEventHandler<SharedTetherGunSystem.RequestTetherMoveEvent>(this.OnTetherMove));
    this.SubscribeLocalEvent<TetheredComponent, BuckleAttemptEvent>(new ComponentEventRefHandler<TetheredComponent, BuckleAttemptEvent>(this.OnTetheredBuckleAttempt));
    this.SubscribeLocalEvent<TetheredComponent, UpdateCanMoveEvent>(new ComponentEventHandler<TetheredComponent, UpdateCanMoveEvent>(this.OnTetheredUpdateCanMove));
    this.SubscribeLocalEvent<TetheredComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<TetheredComponent, EntGotInsertedIntoContainerMessage>(this.OnTetheredContainerInserted));
    this.InitializeForce();
  }

  private void OnTetheredContainerInserted(
    EntityUid uid,
    TetheredComponent component,
    EntGotInsertedIntoContainerMessage args)
  {
    TetherGunComponent comp1;
    if (this.TryComp<TetherGunComponent>(component.Tetherer, out comp1))
    {
      this.StopTether(component.Tetherer, (BaseForceGunComponent) comp1);
    }
    else
    {
      ForceGunComponent comp2;
      if (!this.TryComp<ForceGunComponent>(component.Tetherer, out comp2))
        return;
      this.StopTether(component.Tetherer, (BaseForceGunComponent) comp2);
    }
  }

  private void OnTetheredBuckleAttempt(
    EntityUid uid,
    TetheredComponent component,
    ref BuckleAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnTetheredUpdateCanMove(
    EntityUid uid,
    TetheredComponent component,
    UpdateCanMoveEvent args)
  {
    args.Cancel();
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<TetheredComponent, PhysicsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TetheredComponent, PhysicsComponent>();
    EntityUid uid;
    PhysicsComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out TetheredComponent _, out comp2))
    {
      int num = Math.Sign(comp2.AngularVelocity);
      if (num == 0)
        num = 1;
      float impulse = Math.Clamp(3.14159274f * (float) num - comp2.AngularVelocity, -3.14159274f, 3.14159274f) * (frameTime * 1f);
      this._physics.ApplyAngularImpulse(uid, impulse, body: comp2);
    }
  }

  private void OnTetherMove(
    SharedTetherGunSystem.RequestTetherMoveEvent msg,
    EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    EntityUid? gunUid;
    TetherGunComponent gun;
    if (!attachedEntity.HasValue || !this.TryGetTetherGun(attachedEntity.Value, out gunUid, out gun))
      return;
    EntityUid? tetherEntity = gun.TetherEntity;
    if (!tetherEntity.HasValue)
      return;
    EntityCoordinates coordinates = this.GetCoordinates(msg.Coordinates);
    float distance;
    if (!coordinates.TryDistance((IEntityManager) this.EntityManager, this.TransformSystem, this.Transform(gunUid.Value).Coordinates, out distance) || (double) distance > (double) gun.MaxDistance)
      return;
    SharedTransformSystem transformSystem = this.TransformSystem;
    tetherEntity = gun.TetherEntity;
    EntityUid uid = tetherEntity.Value;
    EntityCoordinates entityCoordinates = coordinates;
    transformSystem.SetCoordinates(uid, entityCoordinates);
  }

  private void OnTetherRanged(EntityUid uid, TetherGunComponent component, AfterInteractEvent args)
  {
    if (!args.Target.HasValue || args.Handled)
      return;
    this.TryTether(uid, args.Target.Value, new EntityUid?(args.User), (BaseForceGunComponent) component);
  }

  protected bool TryGetTetherGun(EntityUid user, [NotNullWhen(true)] out EntityUid? gunUid, [NotNullWhen(true)] out TetherGunComponent? gun)
  {
    gunUid = new EntityUid?();
    gun = (TetherGunComponent) null;
    EntityUid? uid;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out uid) || !this.TryComp<TetherGunComponent>(uid, out gun) || this._container.IsEntityInContainer(user))
      return false;
    gunUid = new EntityUid?(uid.Value);
    return true;
  }

  private void OnTetherActivate(
    EntityUid uid,
    TetherGunComponent component,
    ActivateInWorldEvent args)
  {
    if (!args.Complex)
      return;
    this.StopTether(uid, (BaseForceGunComponent) component);
  }

  public bool TryTether(
    EntityUid gun,
    EntityUid target,
    EntityUid? user,
    BaseForceGunComponent? component = null)
  {
    if (!this.Resolve<BaseForceGunComponent>(gun, ref component) || !this.CanTether(gun, component, target, user))
      return false;
    this.StartTether(gun, component, target, user);
    return true;
  }

  protected virtual bool CanTether(
    EntityUid uid,
    BaseForceGunComponent component,
    EntityUid target,
    EntityUid? user)
  {
    PhysicsComponent comp1;
    StrapComponent comp2;
    return !this.HasComp<TetheredComponent>(target) && this.TryComp<PhysicsComponent>(target, out comp1) && (comp1.BodyType != BodyType.Static || component.CanUnanchor) && !this._container.IsEntityInContainer(target) && (double) comp1.Mass <= (double) component.MassLimit && (component.CanTetherAlive || !this._mob.IsAlive(target)) && (!this.TryComp<StrapComponent>(target, out comp2) || comp2.BuckledEntities.Count <= 0);
  }

  protected virtual void StartTether(
    EntityUid gunUid,
    BaseForceGunComponent component,
    EntityUid target,
    EntityUid? user,
    PhysicsComponent? targetPhysics = null,
    TransformComponent? targetXform = null)
  {
    if (!this.Resolve<PhysicsComponent, TransformComponent>(target, ref targetPhysics, ref targetXform))
      return;
    if (component.Tethered.HasValue)
      this.StopTether(gunUid, component);
    AppearanceComponent comp;
    this.TryComp<AppearanceComponent>(gunUid, out comp);
    this._appearance.SetData(gunUid, (Enum) SharedTetherGunSystem.TetherVisualsStatus.Key, (object) true, comp);
    this._appearance.SetData(gunUid, (Enum) ToggleableVisuals.Enabled, (object) true, comp);
    this.TransformSystem.Unanchor(target, targetXform);
    component.Tethered = new EntityUid?(target);
    TetheredComponent tetheredComponent = this.EnsureComp<TetheredComponent>(target);
    this._physics.SetBodyStatus(target, targetPhysics, BodyStatus.InAir, false);
    this._physics.SetSleepingAllowed(target, targetPhysics, false);
    tetheredComponent.Tetherer = gunUid;
    tetheredComponent.OriginalAngularDamping = targetPhysics.AngularDamping;
    this._physics.SetAngularDamping(target, targetPhysics, 0.0f);
    this._physics.SetLinearDamping(target, targetPhysics, 0.0f);
    this._physics.SetAngularVelocity(target, 3.14159274f, body: targetPhysics);
    this._physics.WakeBody(target, body: targetPhysics);
    this.EnsureComp<ThrownItemComponent>(component.Tethered.Value).Thrower = new EntityUid?(gunUid);
    this._blocker.UpdateCanMove(target);
    EntityUid entityUid = this.Spawn("TetherEntity", this.TransformSystem.GetMapCoordinates(target), rotation: new Angle());
    PhysicsComponent physicsComponent = this.Comp<PhysicsComponent>(entityUid);
    component.TetherEntity = new EntityUid?(entityUid);
    this._physics.WakeBody(entityUid);
    MouseJoint mouseJoint = this._joints.CreateMouseJoint(entityUid, target, id: "tether");
    float stiffness;
    float damping;
    SharedJointSystem.LinearStiffness(component.Frequency, component.DampingRatio, physicsComponent.Mass, targetPhysics.Mass, out stiffness, out damping);
    mouseJoint.Stiffness = stiffness;
    mouseJoint.Damping = damping;
    mouseJoint.MaxForce = component.MaxForce;
    if (this._netManager.IsServer && !component.Stream.HasValue)
    {
      BaseForceGunComponent forceGunComponent = component;
      (EntityUid, AudioComponent)? nullable1 = this._audio.PlayPredicted(component.Sound, gunUid, new EntityUid?());
      ref (EntityUid, AudioComponent)? local = ref nullable1;
      EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      forceGunComponent.Stream = nullable2;
    }
    this.Dirty(target, (IComponent) tetheredComponent);
    this.Dirty(gunUid, (IComponent) component);
  }

  protected virtual void StopTether(
    EntityUid gunUid,
    BaseForceGunComponent component,
    bool land = true,
    bool transfer = false)
  {
    if (!component.Tethered.HasValue)
      return;
    EntityUid? nullable1;
    if (component.TetherEntity.HasValue)
    {
      SharedJointSystem joints = this._joints;
      nullable1 = component.TetherEntity;
      EntityUid uid = nullable1.Value;
      joints.RemoveJoint(uid, "tether");
      if (this._netManager.IsServer)
      {
        nullable1 = component.TetherEntity;
        this.QueueDel(new EntityUid?(nullable1.Value));
      }
      BaseForceGunComponent forceGunComponent = component;
      nullable1 = new EntityUid?();
      EntityUid? nullable2 = nullable1;
      forceGunComponent.TetherEntity = nullable2;
    }
    PhysicsComponent comp1;
    if (this.TryComp<PhysicsComponent>(component.Tethered, out comp1))
    {
      if (land)
      {
        nullable1 = component.Tethered;
        ThrownItemComponent thrownItemComponent1 = this.EnsureComp<ThrownItemComponent>(nullable1.Value);
        ThrownItemSystem thrown1 = this._thrown;
        nullable1 = component.Tethered;
        EntityUid uid1 = nullable1.Value;
        ThrownItemComponent thrownItem = thrownItemComponent1;
        PhysicsComponent physics = comp1;
        thrown1.LandComponent(uid1, thrownItem, physics, true);
        ThrownItemSystem thrown2 = this._thrown;
        nullable1 = component.Tethered;
        EntityUid uid2 = nullable1.Value;
        ThrownItemComponent thrownItemComponent2 = thrownItemComponent1;
        thrown2.StopThrow(uid2, thrownItemComponent2);
      }
      SharedPhysicsSystem physics1 = this._physics;
      nullable1 = component.Tethered;
      EntityUid uid3 = nullable1.Value;
      PhysicsComponent body1 = comp1;
      physics1.SetBodyStatus(uid3, body1, BodyStatus.OnGround);
      SharedPhysicsSystem physics2 = this._physics;
      nullable1 = component.Tethered;
      EntityUid uid4 = nullable1.Value;
      PhysicsComponent body2 = comp1;
      physics2.SetSleepingAllowed(uid4, body2, true);
      SharedPhysicsSystem physics3 = this._physics;
      nullable1 = component.Tethered;
      EntityUid uid5 = nullable1.Value;
      PhysicsComponent body3 = comp1;
      nullable1 = component.Tethered;
      double originalAngularDamping = (double) this.Comp<TetheredComponent>(nullable1.Value).OriginalAngularDamping;
      physics3.SetAngularDamping(uid5, body3, (float) originalAngularDamping);
    }
    if (!transfer)
    {
      this._audio.Stop(component.Stream);
      component.Stream = new EntityUid?();
    }
    AppearanceComponent comp2;
    this.TryComp<AppearanceComponent>(gunUid, out comp2);
    this._appearance.SetData(gunUid, (Enum) SharedTetherGunSystem.TetherVisualsStatus.Key, (object) false, comp2);
    this._appearance.SetData(gunUid, (Enum) ToggleableVisuals.Enabled, (object) false, comp2);
    nullable1 = component.Tethered;
    this.RemComp<TetheredComponent>(nullable1.Value);
    ActionBlockerSystem blocker = this._blocker;
    nullable1 = component.Tethered;
    EntityUid uid6 = nullable1.Value;
    blocker.UpdateCanMove(uid6);
    BaseForceGunComponent forceGunComponent1 = component;
    nullable1 = new EntityUid?();
    EntityUid? nullable3 = nullable1;
    forceGunComponent1.Tethered = nullable3;
    this.Dirty(gunUid, (IComponent) component);
  }

  private void InitializeForce()
  {
    this.SubscribeLocalEvent<ForceGunComponent, AfterInteractEvent>(new ComponentEventHandler<ForceGunComponent, AfterInteractEvent>(this.OnForceRanged));
    this.SubscribeLocalEvent<ForceGunComponent, ActivateInWorldEvent>(new ComponentEventHandler<ForceGunComponent, ActivateInWorldEvent>(this.OnForceActivate));
  }

  private void OnForceActivate(
    EntityUid uid,
    ForceGunComponent component,
    ActivateInWorldEvent args)
  {
    if (!args.Complex)
      return;
    this.StopTether(uid, (BaseForceGunComponent) component);
  }

  private void OnForceRanged(EntityUid uid, ForceGunComponent component, AfterInteractEvent args)
  {
    if (this.IsTethered(component))
    {
      float distance;
      if (!args.ClickLocation.TryDistance((IEntityManager) this.EntityManager, this.TransformSystem, this.Transform(uid).Coordinates, out distance) || (double) distance > (double) component.ThrowDistance || !this._netManager.IsServer)
        return;
      EntityUid? tethered = component.Tethered;
      this.StopTether(uid, (BaseForceGunComponent) component, false);
      this._throwing.TryThrow(tethered.Value, args.ClickLocation, component.ThrowForce, playSound: false);
      this._audio.PlayPredicted(component.LaunchSound, uid, new EntityUid?());
    }
    else
    {
      EntityUid? nullable = args.Target;
      if (!nullable.HasValue)
        return;
      EntityUid gun = uid;
      nullable = args.Target;
      EntityUid target = nullable.Value;
      EntityUid? user = new EntityUid?(args.User);
      ForceGunComponent component1 = component;
      if (!this.TryTether(gun, target, user, (BaseForceGunComponent) component1))
        return;
      SharedTransformSystem transformSystem = this.TransformSystem;
      nullable = component.TetherEntity;
      EntityUid uid1 = nullable.Value;
      EntityCoordinates entityCoordinates = new EntityCoordinates(uid, new Vector2(0.0f, 0.0f));
      transformSystem.SetCoordinates(uid1, entityCoordinates);
    }
  }

  private bool IsTethered(ForceGunComponent component) => component.Tethered.HasValue;

  [NetSerializable]
  [Serializable]
  protected sealed class RequestTetherMoveEvent : EntityEventArgs
  {
    public NetCoordinates Coordinates;
  }

  [NetSerializable]
  [Serializable]
  public enum TetherVisualsStatus : byte
  {
    Key,
  }
}
