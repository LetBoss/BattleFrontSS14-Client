// Decompiled with JetBrains decompiler
// Type: Content.Shared.Projectiles.SharedProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Projectiles.Penetration;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids.Damage;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Projectiles;

public abstract class SharedProjectileSystem : EntitySystem
{
  public const string ProjectileFixture = "projectile";
  [Robust.Shared.IoC.Dependency]
  private INetManager _net;
  [Robust.Shared.IoC.Dependency]
  private SharedAudioSystem _audio;
  [Robust.Shared.IoC.Dependency]
  private SharedDoAfterSystem _doAfter;
  [Robust.Shared.IoC.Dependency]
  private SharedHandsSystem _hands;
  [Robust.Shared.IoC.Dependency]
  private SharedContainerSystem _container;
  [Robust.Shared.IoC.Dependency]
  private SharedPhysicsSystem _physics;
  [Robust.Shared.IoC.Dependency]
  private SharedTransformSystem _transform;
  [Robust.Shared.IoC.Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Robust.Shared.IoC.Dependency]
  private SharedColorFlashEffectSystem _color;
  [Robust.Shared.IoC.Dependency]
  private DamageableSystem _damageableSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedGunSystem _guns;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ProjectileComponent, StartCollideEvent>(new ComponentEventRefHandler<ProjectileComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<ProjectileComponent, PreventCollideEvent>(new ComponentEventRefHandler<ProjectileComponent, PreventCollideEvent>(this.PreventCollision));
    this.SubscribeLocalEvent<EmbeddableProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<EmbeddableProjectileComponent, ProjectileHitEvent>(this.OnEmbedProjectileHit));
    this.SubscribeLocalEvent<EmbeddableProjectileComponent, ThrowDoHitEvent>(new EntityEventRefHandler<EmbeddableProjectileComponent, ThrowDoHitEvent>(this.OnEmbedThrowDoHit));
    this.SubscribeLocalEvent<EmbeddableProjectileComponent, ActivateInWorldEvent>(new EntityEventRefHandler<EmbeddableProjectileComponent, ActivateInWorldEvent>(this.OnEmbedActivate));
    this.SubscribeLocalEvent<EmbeddableProjectileComponent, SharedProjectileSystem.RemoveEmbeddedProjectileEvent>(new EntityEventRefHandler<EmbeddableProjectileComponent, SharedProjectileSystem.RemoveEmbeddedProjectileEvent>(this.OnEmbedRemove));
    this.SubscribeLocalEvent<EmbeddableProjectileComponent, ComponentShutdown>(new EntityEventRefHandler<EmbeddableProjectileComponent, ComponentShutdown>(this.OnEmbeddableCompShutdown));
    this.SubscribeLocalEvent<EmbeddedContainerComponent, EntityTerminatingEvent>(new EntityEventRefHandler<EmbeddedContainerComponent, EntityTerminatingEvent>(this.OnEmbeddableTermination));
  }

  private void OnStartCollide(
    EntityUid uid,
    ProjectileComponent component,
    ref StartCollideEvent args)
  {
    if (args.OurFixtureId != "projectile" || !args.OtherFixture.Hard || component.ProjectileSpent || component != null && !component.Weapon.HasValue && component.OnlyCollideWhenShot)
      return;
    bool predicted = this._net.IsServer && this._guns.GunPrediction && this.HasComp<PredictedProjectileServerComponent>(uid);
    this.ProjectileCollide((Entity<ProjectileComponent, PhysicsComponent>) (uid, component, args.OurBody), args.OtherEntity, predicted);
  }

  public void ProjectileCollide(
    Entity<ProjectileComponent, PhysicsComponent> projectile,
    EntityUid target,
    bool predicted = false)
  {
    (EntityUid entityUid1, ProjectileComponent projectileComponent, PhysicsComponent _) = projectile;
    if (projectile.Comp1.ProjectileSpent)
    {
      if (!this._net.IsServer || !projectileComponent.DeleteOnCollide)
        return;
      this.QueueDel(new EntityUid?(entityUid1));
    }
    else
    {
      ProjectileReflectAttemptEvent args1 = new ProjectileReflectAttemptEvent(entityUid1, projectileComponent, false);
      this.RaiseLocalEvent<ProjectileReflectAttemptEvent>(target, ref args1);
      if (args1.Cancelled)
      {
        this.SetShooter(entityUid1, projectileComponent, new EntityUid?(target));
      }
      else
      {
        ProjectileHitEvent args2 = new ProjectileHitEvent(projectileComponent.Damage * this._damageableSystem.UniversalProjectileDamageModifier, target, projectileComponent.Shooter);
        this.RaiseLocalEvent<ProjectileHitEvent>(entityUid1, ref args2);
        if (args2.Handled)
          return;
        EntityCoordinates coordinates = this.Transform((EntityUid) projectile).Coordinates;
        EntityStringRepresentation prettyString = this.ToPrettyString((Entity<MetaDataComponent>) target);
        DamageSpecifier damageSpecifier = this._net.IsServer ? this._damageableSystem.TryChangeDamage(new EntityUid?(target), args2.Damage, projectileComponent.IgnoreResistances, origin: projectileComponent.Shooter, tool: new EntityUid?(entityUid1)) : new DamageSpecifier(args2.Damage);
        bool flag = this.Deleted(target);
        DamageDealtEvent args3 = new DamageDealtEvent(projectileComponent.Shooter, damageSpecifier);
        this.RaiseLocalEvent<DamageDealtEvent>(target, ref args3);
        Filter filter1 = Filter.Pvs(coordinates, entityMan: (IEntityManager) this.EntityManager);
        if (this._guns.GunPrediction & predicted)
        {
          PredictedProjectileServerComponent comp1;
          if (this.TryComp<PredictedProjectileServerComponent>((EntityUid) projectile, out comp1))
          {
            ICommonSession shooter = comp1.Shooter;
            if (shooter != null)
              filter1 = filter1.RemovePlayer(shooter);
          }
          XenoProjectileShotComponent comp2;
          if (this._net.IsServer && this.TryComp<XenoProjectileShotComponent>((EntityUid) projectile, out comp2))
          {
            ICommonSession shooter = comp2.Shooter;
            if (shooter != null)
              filter1 = filter1.RemovePlayer(shooter);
          }
        }
        if (damageSpecifier != null && (this.Exists(projectileComponent.Shooter) || this.Exists(projectileComponent.Weapon)))
        {
          if (damageSpecifier.AnyPositive() && !flag)
          {
            Filter filter2 = filter1;
            if (this._net.IsServer && !predicted)
            {
              EntityUid? shooter1 = projectileComponent.Shooter;
              if (shooter1.HasValue)
              {
                EntityUid shooter = shooter1.GetValueOrDefault();
                filter2 = filter2.AddWhereAttachedEntity((Predicate<EntityUid>) (attached => attached == shooter));
              }
            }
            SharedColorFlashEffectSystem color = this._color;
            Color red = Color.Red;
            List<EntityUid> entities = new List<EntityUid>();
            entities.Add(target);
            Filter filter3 = filter2;
            color.RaiseEffect(red, entities, filter3);
          }
          EntityUid entityUid2 = this.Exists(projectileComponent.Shooter) ? projectileComponent.Shooter.Value : projectileComponent.Weapon.Value;
          ISharedAdminLogManager adminLogger = this._adminLogger;
          int impact = this.HasComp<ActorComponent>(target) ? 0 : -1;
          LogStringHandler logStringHandler = new LogStringHandler(43, 4);
          logStringHandler.AppendLiteral("Projectile ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid1), nameof (projectile), "ToPrettyString(uid)");
          logStringHandler.AppendLiteral(" shot by ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid2), "source", "ToPrettyString(shooterOrWeapon)");
          logStringHandler.AppendLiteral(" hit ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(prettyString, nameof (target), "otherName");
          logStringHandler.AppendLiteral(" and dealt ");
          logStringHandler.AppendFormatted<FixedPoint2>(damageSpecifier.GetTotal(), "damage", "modifiedDamage.GetTotal()");
          logStringHandler.AppendLiteral(" damage");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.BulletHit, (LogImpact) impact, ref local);
        }
        if (!flag && filter1.Count > 0)
          this._guns.PlayImpactSound(target, damageSpecifier, projectileComponent.SoundHit, projectileComponent.ForceSound, filter1, new EntityUid?((EntityUid) projectile));
        projectileComponent.ProjectileSpent = true;
        this.Dirty(entityUid1, (IComponent) projectileComponent);
        AfterProjectileHitEvent args4 = new AfterProjectileHitEvent((Entity<ProjectileComponent>) projectile, target);
        this.RaiseLocalEvent<AfterProjectileHitEvent>(entityUid1, ref args4);
        if (!predicted && projectileComponent.DeleteOnCollide && (this._net.IsServer || this.IsClientSide(entityUid1)))
          this.QueueDel(new EntityUid?(entityUid1));
        else if (this._net.IsServer && projectileComponent.DeleteOnCollide)
        {
          PredictedProjectileHitComponent projectileHitComponent = this.EnsureComp<PredictedProjectileHitComponent>(entityUid1);
          projectileHitComponent.Origin = this._transform.GetMoverCoordinates(coordinates);
          EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(target);
          float distance;
          if (projectileHitComponent.Origin.TryDistance((IEntityManager) this.EntityManager, this._transform, moverCoordinates, out distance))
            projectileHitComponent.Distance = distance;
          this.Dirty(entityUid1, (IComponent) projectileHitComponent);
        }
        if (!this._net.IsServer && !this.IsClientSide(entityUid1) || !projectileComponent.ImpactEffect.HasValue)
          return;
        EntProtoId? impactEffect = projectileComponent.ImpactEffect;
        ImpactEffectEvent message = new ImpactEffectEvent(impactEffect.HasValue ? (string) impactEffect.GetValueOrDefault() : (string) null, this.GetNetCoordinates(coordinates));
        if (this._net.IsServer)
          this.RaiseNetworkEvent((EntityEventArgs) message, filter1);
        else
          this.RaiseLocalEvent<ImpactEffectEvent>(message);
      }
    }
  }

  private void OnEmbedActivate(
    Entity<EmbeddableProjectileComponent> embeddable,
    ref ActivateInWorldEvent args)
  {
    PhysicsComponent comp;
    if (!embeddable.Comp.RemovalTime.HasValue || args.Handled || !args.Complex || !this.TryComp<PhysicsComponent>((EntityUid) embeddable, out comp) || comp.BodyType != BodyType.Static)
      return;
    args.Handled = true;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, embeddable.Comp.RemovalTime.Value, (DoAfterEvent) new SharedProjectileSystem.RemoveEmbeddedProjectileEvent(), new EntityUid?((EntityUid) embeddable), new EntityUid?((EntityUid) embeddable)));
  }

  private void OnEmbedRemove(
    Entity<EmbeddableProjectileComponent> embeddable,
    ref SharedProjectileSystem.RemoveEmbeddedProjectileEvent args)
  {
    if (args.Cancelled)
      return;
    this.EmbedDetach((EntityUid) embeddable, embeddable.Comp, new EntityUid?(args.User));
    this._hands.TryPickupAnyHand(args.User, (EntityUid) embeddable);
  }

  private void OnEmbeddableCompShutdown(
    Entity<EmbeddableProjectileComponent> embeddable,
    ref ComponentShutdown arg)
  {
    this.EmbedDetach((EntityUid) embeddable, embeddable.Comp);
  }

  private void OnEmbedThrowDoHit(
    Entity<EmbeddableProjectileComponent> embeddable,
    ref ThrowDoHitEvent args)
  {
    if (!embeddable.Comp.EmbedOnThrow)
      return;
    this.EmbedAttach((EntityUid) embeddable, args.Target, new EntityUid?(), embeddable.Comp);
  }

  private void OnEmbedProjectileHit(
    Entity<EmbeddableProjectileComponent> embeddable,
    ref ProjectileHitEvent args)
  {
    this.EmbedAttach((EntityUid) embeddable, args.Target, args.Shooter, embeddable.Comp);
    ProjectileComponent comp;
    if (!this.TryComp<ProjectileComponent>((EntityUid) embeddable, out comp))
      return;
    EntityUid? nullable = comp.Shooter;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    nullable = comp.Weapon;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
    ProjectileEmbedEvent args1 = new ProjectileEmbedEvent(new EntityUid?(valueOrDefault1), valueOrDefault2, args.Target);
    this.RaiseLocalEvent<ProjectileEmbedEvent>((EntityUid) embeddable, ref args1);
  }

  private void EmbedAttach(
    EntityUid uid,
    EntityUid target,
    EntityUid? user,
    EmbeddableProjectileComponent component)
  {
    PhysicsComponent comp1;
    this.TryComp<PhysicsComponent>(uid, out comp1);
    this._physics.SetLinearVelocity(uid, Vector2.Zero, body: comp1);
    this._physics.SetBodyType(uid, BodyType.Static, body: comp1);
    TransformComponent xform = this.Transform(uid);
    this._transform.SetParent(uid, xform, target);
    if (component.Offset != Vector2.Zero)
    {
      Angle angle = xform.LocalRotation;
      ThrowingAngleComponent comp2;
      if (this.TryComp<ThrowingAngleComponent>(uid, out comp2))
        angle = Angle.op_Addition(angle, comp2.Angle);
      this._transform.SetLocalPosition(uid, xform.LocalPosition + ((Angle) ref angle).RotateVec(ref component.Offset), xform);
    }
    this._audio.PlayPredicted(component.Sound, uid, new EntityUid?());
    component.EmbeddedIntoUid = new EntityUid?(target);
    EmbedEvent args = new EmbedEvent(user, target);
    this.RaiseLocalEvent<EmbedEvent>(uid, ref args);
    this.Dirty(uid, (IComponent) component);
    EmbeddedContainerComponent comp3;
    this.EnsureComp<EmbeddedContainerComponent>(target, out comp3);
    comp3.EmbeddedObjects.Add(uid);
  }

  public void EmbedDetach(EntityUid uid, EmbeddableProjectileComponent? component, EntityUid? user = null)
  {
    if (!this.Resolve<EmbeddableProjectileComponent>(uid, ref component))
      return;
    EmbeddedContainerComponent comp1;
    if (component.EmbeddedIntoUid.HasValue && this.TryComp<EmbeddedContainerComponent>(component.EmbeddedIntoUid.Value, out comp1))
    {
      comp1.EmbeddedObjects.Remove(uid);
      this.Dirty(component.EmbeddedIntoUid.Value, (IComponent) comp1);
      if (comp1.EmbeddedObjects.Count == 0)
        this.RemCompDeferred<EmbeddedContainerComponent>(component.EmbeddedIntoUid.Value);
    }
    if (component.DeleteOnRemove && this._net.IsServer)
    {
      this.QueueDel(new EntityUid?(uid));
    }
    else
    {
      TransformComponent xform = this.Transform(uid);
      if (this.TerminatingOrDeleted(xform.GridUid) && this.TerminatingOrDeleted(xform.MapUid))
        return;
      PhysicsComponent comp2;
      this.TryComp<PhysicsComponent>(uid, out comp2);
      this._physics.SetBodyType(uid, BodyType.Dynamic, body: comp2, xform: xform);
      this._transform.AttachToGridOrMap(uid, xform);
      component.EmbeddedIntoUid = new EntityUid?();
      this.Dirty(uid, (IComponent) component);
      ProjectileComponent comp3;
      if (this.TryComp<ProjectileComponent>(uid, out comp3))
      {
        comp3.Shooter = new EntityUid?();
        comp3.Weapon = new EntityUid?();
        comp3.ProjectileSpent = false;
        this.Dirty(uid, (IComponent) comp3);
      }
      if (user.HasValue)
      {
        LandEvent args = new LandEvent(user, true);
        this.RaiseLocalEvent<LandEvent>(uid, ref args);
      }
      this._physics.WakeBody(uid, body: comp2);
    }
  }

  private void OnEmbeddableTermination(
    Entity<EmbeddedContainerComponent> container,
    ref EntityTerminatingEvent args)
  {
    this.DetachAllEmbedded(container);
  }

  public void DetachAllEmbedded(Entity<EmbeddedContainerComponent> container)
  {
    foreach (EntityUid embeddedObject in container.Comp.EmbeddedObjects)
    {
      EmbeddableProjectileComponent comp;
      if (this.TryComp<EmbeddableProjectileComponent>(embeddedObject, out comp))
        this.EmbedDetach(embeddedObject, comp);
    }
  }

  private void PreventCollision(
    EntityUid uid,
    ProjectileComponent component,
    ref PreventCollideEvent args)
  {
    if (component.IgnoreShooter)
    {
      EntityUid otherEntity1 = args.OtherEntity;
      EntityUid? shooter = component.Shooter;
      if ((shooter.HasValue ? (otherEntity1 == shooter.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      {
        EntityUid otherEntity2 = args.OtherEntity;
        EntityUid? weapon = component.Weapon;
        if ((weapon.HasValue ? (otherEntity2 == weapon.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          goto label_4;
      }
      args.Cancelled = true;
      return;
    }
label_4:
    if (!component.Weapon.HasValue || !this.HasComp<GunIgnoreContainerOwnerCollisionComponent>(component.Weapon.Value))
      return;
    BaseContainer container;
    for (EntityUid owner = component.Weapon.Value; this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (owner, (TransformComponent) null), out container); owner = container.Owner)
    {
      if (args.OtherEntity == container.Owner)
      {
        args.Cancelled = true;
        break;
      }
    }
  }

  public void SetShooter(EntityUid id, ProjectileComponent component, EntityUid? shooterId = null)
  {
    EntityUid? shooter = component.Shooter;
    EntityUid? nullable = shooterId;
    if ((shooter.HasValue == nullable.HasValue ? (shooter.HasValue ? (shooter.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 || !shooterId.HasValue)
      return;
    component.Shooter = shooterId;
    this.Dirty(id, (IComponent) component);
  }

  [NetSerializable]
  [Serializable]
  private sealed class RemoveEmbeddedProjectileEvent : 
    DoAfterEvent,
    ISerializationGenerated<SharedProjectileSystem.RemoveEmbeddedProjectileEvent>,
    ISerializationGenerated
  {
    public override DoAfterEvent Clone() => (DoAfterEvent) this;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedProjectileSystem.RemoveEmbeddedProjectileEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DoAfterEvent target1 = (DoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedProjectileSystem.RemoveEmbeddedProjectileEvent) target1;
      serialization.TryCustomCopy<SharedProjectileSystem.RemoveEmbeddedProjectileEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedProjectileSystem.RemoveEmbeddedProjectileEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref DoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedProjectileSystem.RemoveEmbeddedProjectileEvent target1 = (SharedProjectileSystem.RemoveEmbeddedProjectileEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (DoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedProjectileSystem.RemoveEmbeddedProjectileEvent target1 = (SharedProjectileSystem.RemoveEmbeddedProjectileEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedProjectileSystem.RemoveEmbeddedProjectileEvent DoAfterEvent.Instantiate()
    {
      return new SharedProjectileSystem.RemoveEmbeddedProjectileEvent();
    }
  }
}
