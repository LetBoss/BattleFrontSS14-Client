// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Misc.SharedGrapplingGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CombatMode;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Misc;

public abstract class SharedGrapplingGunSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedJointSystem _joints;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedPhysicsSystem _physics;
  public const string GrapplingJoint = "grappling";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GrapplingProjectileComponent, ProjectileEmbedEvent>(new ComponentEventRefHandler<GrapplingProjectileComponent, ProjectileEmbedEvent>(this.OnGrappleCollide));
    this.SubscribeLocalEvent<GrapplingProjectileComponent, JointRemovedEvent>(new ComponentEventHandler<GrapplingProjectileComponent, JointRemovedEvent>(this.OnGrappleJointRemoved));
    this.SubscribeLocalEvent<CanWeightlessMoveEvent>(new EntityEventRefHandler<CanWeightlessMoveEvent>(this.OnWeightlessMove));
    this.SubscribeAllEvent<SharedGrapplingGunSystem.RequestGrapplingReelMessage>(new EntitySessionEventHandler<SharedGrapplingGunSystem.RequestGrapplingReelMessage>(this.OnGrapplingReel));
    this.SubscribeLocalEvent<GrapplingGunComponent, GunShotEvent>(new ComponentEventRefHandler<GrapplingGunComponent, GunShotEvent>(this.OnGrapplingShot));
    this.SubscribeLocalEvent<GrapplingGunComponent, ActivateInWorldEvent>(new ComponentEventHandler<GrapplingGunComponent, ActivateInWorldEvent>(this.OnGunActivate));
    this.SubscribeLocalEvent<GrapplingGunComponent, HandDeselectedEvent>(new ComponentEventHandler<GrapplingGunComponent, HandDeselectedEvent>(this.OnGrapplingDeselected));
  }

  private void OnGrappleJointRemoved(
    EntityUid uid,
    GrapplingProjectileComponent component,
    JointRemovedEvent args)
  {
    if (!this._netManager.IsServer)
      return;
    this.QueueDel(new EntityUid?(uid));
  }

  private void OnGrapplingShot(
    EntityUid uid,
    GrapplingGunComponent component,
    ref GunShotEvent args)
  {
    foreach ((EntityUid? nullable, IShootable _) in args.Ammo)
    {
      if (this.HasComp<GrapplingProjectileComponent>(nullable))
      {
        component.Projectile = new EntityUid?(nullable.Value);
        this.Dirty(uid, (IComponent) component);
        JointVisualsComponent visualsComponent = this.EnsureComp<JointVisualsComponent>(nullable.Value);
        visualsComponent.Sprite = component.RopeSprite;
        visualsComponent.OffsetA = new Vector2(0.0f, 0.5f);
        visualsComponent.Target = new NetEntity?(this.GetNetEntity(uid));
        this.Dirty(nullable.Value, (IComponent) visualsComponent);
      }
    }
    AppearanceComponent comp;
    this.TryComp<AppearanceComponent>(uid, out comp);
    this._appearance.SetData(uid, (Enum) SharedTetherGunSystem.TetherVisualsStatus.Key, (object) false, comp);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnGrapplingDeselected(
    EntityUid uid,
    GrapplingGunComponent component,
    HandDeselectedEvent args)
  {
    this.SetReeling(uid, component, false, new EntityUid?(args.User));
  }

  private void OnGrapplingReel(
    SharedGrapplingGunSystem.RequestGrapplingReelMessage msg,
    EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid? uid;
    GrapplingGunComponent comp1;
    CombatModeComponent comp2;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) valueOrDefault, out uid) || !this.TryComp<GrapplingGunComponent>(uid, out comp1) || msg.Reeling && (!this.TryComp<CombatModeComponent>(valueOrDefault, out comp2) || !comp2.IsInCombatMode))
      return;
    this.SetReeling(uid.Value, comp1, msg.Reeling, new EntityUid?(valueOrDefault));
  }

  private void OnWeightlessMove(ref CanWeightlessMoveEvent ev)
  {
    JointRelayTargetComponent comp1;
    if (ev.CanMove || !this.TryComp<JointRelayTargetComponent>(ev.Uid, out comp1))
      return;
    foreach (EntityUid uid in comp1.Relayed)
    {
      JointComponent comp2;
      if (this.TryComp<JointComponent>(uid, out comp2) && comp2.GetJoints.ContainsKey("grappling"))
      {
        ev.CanMove = true;
        break;
      }
    }
  }

  private void OnGunActivate(
    EntityUid uid,
    GrapplingGunComponent component,
    ActivateInWorldEvent args)
  {
    if (!this.Timing.IsFirstTimePredicted || args.Handled || !args.Complex)
      return;
    EntityUid? projectile = component.Projectile;
    if (!projectile.HasValue)
      return;
    EntityUid valueOrDefault = projectile.GetValueOrDefault();
    this._audio.PlayPredicted(component.CycleSound, uid, new EntityUid?(args.User));
    this._appearance.SetData(uid, (Enum) SharedTetherGunSystem.TetherVisualsStatus.Key, (object) true);
    if (this._netManager.IsServer)
      this.QueueDel(new EntityUid?(valueOrDefault));
    component.Projectile = new EntityUid?();
    this.SetReeling(uid, component, false, new EntityUid?(args.User));
    this._gun.ChangeBasicEntityAmmoCount(uid, 1);
    args.Handled = true;
  }

  private void SetReeling(
    EntityUid uid,
    GrapplingGunComponent component,
    bool value,
    EntityUid? user)
  {
    if (component.Reeling == value)
      return;
    if (value)
    {
      if (this.Timing.IsFirstTimePredicted)
      {
        GrapplingGunComponent grapplingGunComponent = component;
        (EntityUid, AudioComponent)? nullable1 = this._audio.PlayPredicted(component.ReelSound, uid, user);
        ref (EntityUid, AudioComponent)? local = ref nullable1;
        EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
        grapplingGunComponent.Stream = nullable2;
      }
    }
    else if (this.Timing.IsFirstTimePredicted)
      component.Stream = this._audio.Stop(component.Stream);
    component.Reeling = value;
    this.Dirty(uid, (IComponent) component);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<GrapplingGunComponent> entityQueryEnumerator = this.EntityQueryEnumerator<GrapplingGunComponent>();
    EntityUid uid;
    GrapplingGunComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!comp1.Reeling)
      {
        if (this.Timing.IsFirstTimePredicted)
          comp1.Stream = this._audio.Stop(comp1.Stream);
      }
      else
      {
        JointComponent comp;
        Joint joint;
        if (!this.TryComp<JointComponent>(uid, out comp) || !comp.GetJoints.TryGetValue("grappling", out joint) || !(joint is DistanceJoint distanceJoint))
        {
          this.SetReeling(uid, comp1, false, new EntityUid?());
        }
        else
        {
          distanceJoint.MaxLength = MathF.Max(distanceJoint.MinLength, distanceJoint.MaxLength - comp1.ReelRate * frameTime);
          distanceJoint.Length = MathF.Min(distanceJoint.MaxLength, distanceJoint.Length);
          this._physics.WakeBody(joint.BodyAUid);
          this._physics.WakeBody(joint.BodyBUid);
          if (comp.Relay.HasValue)
            this._physics.WakeBody(comp.Relay.Value);
          this.Dirty(uid, (IComponent) comp);
          if (distanceJoint.MaxLength.Equals(distanceJoint.MinLength))
            this.SetReeling(uid, comp1, false, new EntityUid?());
        }
      }
    }
  }

  private void OnGrappleCollide(
    EntityUid uid,
    GrapplingProjectileComponent component,
    ref ProjectileEmbedEvent args)
  {
    if (!this.Timing.IsFirstTimePredicted)
      return;
    JointComponent jointComponent = this.EnsureComp<JointComponent>(uid);
    DistanceJoint distanceJoint = this._joints.CreateDistanceJoint(uid, args.Weapon, new Vector2?(new Vector2(0.0f, 0.5f)), id: "grappling");
    distanceJoint.MaxLength = distanceJoint.Length + 0.2f;
    distanceJoint.Stiffness = 1f;
    distanceJoint.MinLength = 0.35f;
    this.Dirty(uid, (IComponent) jointComponent);
  }

  [NetSerializable]
  [Serializable]
  protected sealed class RequestGrapplingReelMessage : EntityEventArgs
  {
    public bool Reeling;

    public RequestGrapplingReelMessage(bool reeling) => this.Reeling = reeling;
  }
}
