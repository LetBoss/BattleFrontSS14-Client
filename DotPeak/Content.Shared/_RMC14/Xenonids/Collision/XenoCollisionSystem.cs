// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Collision.XenoCollisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Collision;

public sealed class XenoCollisionSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private StandingStateSystem _standingState;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoRestSystem _xenoRest;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  private Robust.Shared.GameObjects.EntityQuery<MobCollisionComponent> _mobCollisionQuery;
  private Robust.Shared.GameObjects.EntityQuery<StunFriendlyXenoOnStepComponent> _stunFriendlyXenoOnStepQuery;
  private Robust.Shared.GameObjects.EntityQuery<StunHostilesOnStepComponent> _stunHostileOnStepQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoFortifyComponent> _xenoFortifyQuery;
  private readonly HashSet<EntityUid> _contacts = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this._mobCollisionQuery = this.GetEntityQuery<MobCollisionComponent>();
    this._stunFriendlyXenoOnStepQuery = this.GetEntityQuery<StunFriendlyXenoOnStepComponent>();
    this._stunHostileOnStepQuery = this.GetEntityQuery<StunHostilesOnStepComponent>();
    this._xenoFortifyQuery = this.GetEntityQuery<XenoFortifyComponent>();
    this.SubscribeLocalEvent<XenoComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<XenoComponent, AttemptMobTargetCollideEvent>(this.OnXenoAttemptMobTargetCollide));
    this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, MobStateChangedEvent>(new EntityEventRefHandler<StunFriendlyXenoOnStepComponent, MobStateChangedEvent>(this.OnStunUpdated<MobStateChangedEvent>));
    this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, XenoRestEvent>(new EntityEventRefHandler<StunFriendlyXenoOnStepComponent, XenoRestEvent>(this.OnStunUpdated<XenoRestEvent>));
    this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, StunnedEvent>(new EntityEventRefHandler<StunFriendlyXenoOnStepComponent, StunnedEvent>(this.OnStunUpdated<StunnedEvent>));
    this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<StunFriendlyXenoOnStepComponent, StatusEffectEndedEvent>(this.OnStunUpdated<StatusEffectEndedEvent>));
    this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, XenoOvipositorChangedEvent>(new EntityEventRefHandler<StunFriendlyXenoOnStepComponent, XenoOvipositorChangedEvent>(this.OnStunUpdated<XenoOvipositorChangedEvent>));
    this.SubscribeLocalEvent<StunFriendlyXenoOnStepComponent, PreventCollideEvent>(new EntityEventRefHandler<StunFriendlyXenoOnStepComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<StunHostilesOnStepComponent, MobStateChangedEvent>(new EntityEventRefHandler<StunHostilesOnStepComponent, MobStateChangedEvent>(this.OnStunUpdatedHostile<MobStateChangedEvent>));
    this.SubscribeLocalEvent<StunHostilesOnStepComponent, XenoRestEvent>(new EntityEventRefHandler<StunHostilesOnStepComponent, XenoRestEvent>(this.OnStunUpdatedHostile<XenoRestEvent>));
    this.SubscribeLocalEvent<StunHostilesOnStepComponent, StunnedEvent>(new EntityEventRefHandler<StunHostilesOnStepComponent, StunnedEvent>(this.OnStunUpdatedHostile<StunnedEvent>));
    this.SubscribeLocalEvent<StunHostilesOnStepComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<StunHostilesOnStepComponent, StatusEffectEndedEvent>(this.OnStunUpdatedHostile<StatusEffectEndedEvent>));
    this.SubscribeLocalEvent<StunHostilesOnStepComponent, XenoOvipositorChangedEvent>(new EntityEventRefHandler<StunHostilesOnStepComponent, XenoOvipositorChangedEvent>(this.OnStunUpdatedHostile<XenoOvipositorChangedEvent>));
  }

  private void OnXenoAttemptMobTargetCollide(
    Entity<XenoComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    if (args.Cancelled || !this._stunFriendlyXenoOnStepQuery.HasComp(args.Entity))
      return;
    args.Cancelled = true;
  }

  private void OnStunUpdated<T>(Entity<StunFriendlyXenoOnStepComponent> ent, ref T args)
  {
    StunFriendlyXenoOnStepComponent comp = ent.Comp;
    int num;
    if (this._mobState.IsAlive((EntityUid) ent) && !this.HasComp<XenoRestingComponent>((EntityUid) ent) && !this._statusEffects.HasStatusEffect((EntityUid) ent, (string) ent.Comp.DisableStatus))
    {
      XenoAttachedOvipositorComponent ovipositorComponent = this.CompOrNull<XenoAttachedOvipositorComponent>((EntityUid) ent);
      num = (ovipositorComponent == null ? 0 : (ovipositorComponent.Running ? 1 : 0)) == 0 ? 1 : 0;
    }
    else
      num = 0;
    comp.Enabled = num != 0;
    this.Dirty<StunFriendlyXenoOnStepComponent>(ent);
  }

  private void OnStunUpdatedHostile<T>(Entity<StunHostilesOnStepComponent> ent, ref T args)
  {
    StunHostilesOnStepComponent comp = ent.Comp;
    int num;
    if (this._mobState.IsAlive((EntityUid) ent) && !this.HasComp<XenoRestingComponent>((EntityUid) ent) && !this._statusEffects.HasStatusEffect((EntityUid) ent, (string) ent.Comp.DisableStatus))
    {
      XenoAttachedOvipositorComponent ovipositorComponent = this.CompOrNull<XenoAttachedOvipositorComponent>((EntityUid) ent);
      num = (ovipositorComponent == null ? 0 : (ovipositorComponent.Running ? 1 : 0)) == 0 ? 1 : 0;
    }
    else
      num = 0;
    comp.Enabled = num != 0;
    this.Dirty<StunHostilesOnStepComponent>(ent);
  }

  private void OnPreventCollide(
    Entity<StunFriendlyXenoOnStepComponent> ent,
    ref PreventCollideEvent args)
  {
    XenoFortifyComponent component;
    if (!this._xenoFortifyQuery.TryComp(args.OtherEntity, out component) || !component.Fortified)
      return;
    args.Cancelled = true;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<StunFriendlyXenoOnStepComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<StunFriendlyXenoOnStepComponent, TransformComponent>();
    EntityUid uid1;
    StunFriendlyXenoOnStepComponent comp1_1;
    TransformComponent comp2_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2_1))
    {
      if (comp1_1.Enabled)
      {
        this._contacts.Clear();
        this._physics.GetContactingEntities((Entity<PhysicsComponent>) uid1, this._contacts);
        AttemptMobCollideEvent args1 = new AttemptMobCollideEvent();
        this.RaiseLocalEvent<AttemptMobCollideEvent>(uid1, ref args1);
        if (!args1.Cancelled)
        {
          foreach (EntityUid contact in this._contacts)
          {
            if (!this._mobState.IsDead(contact) && !this._xenoRest.IsResting((Entity<XenoRestingComponent>) contact) && !this._standingState.IsDown(contact) && this._mobCollisionQuery.HasComp(contact))
            {
              TransformComponent transformComponent = this.Transform(contact);
              Box2 aabbNoContainer1 = this._entityLookup.GetAABBNoContainer(uid1, comp2_1.LocalPosition, comp2_1.LocalRotation);
              Box2 aabbNoContainer2 = this._entityLookup.GetAABBNoContainer(contact, transformComponent.LocalPosition, transformComponent.LocalRotation);
              if (((Box2) ref aabbNoContainer1).Intersects(ref aabbNoContainer2))
              {
                Box2 box2 = ((Box2) ref aabbNoContainer2).Intersect(ref aabbNoContainer1);
                float num = Box2.Area(ref box2);
                if ((double) Math.Max(num / Box2.Area(ref aabbNoContainer2), num / Box2.Area(ref aabbNoContainer1)) >= (double) comp1_1.Ratio)
                {
                  AttemptMobTargetCollideEvent args2 = new AttemptMobTargetCollideEvent();
                  this.RaiseLocalEvent<AttemptMobTargetCollideEvent>(contact, ref args2);
                  if (!args2.Cancelled && this._hive.FromSameHive((Entity<HiveMemberComponent>) uid1, (Entity<HiveMemberComponent>) contact))
                  {
                    RecentlyStunnedByFriendlyXenoComponent friendlyXenoComponent = this.EnsureComp<RecentlyStunnedByFriendlyXenoComponent>(contact);
                    if (!(curTime < friendlyXenoComponent.At + comp1_1.Cooldown))
                    {
                      friendlyXenoComponent.At = curTime;
                      this.Dirty(contact, (IComponent) friendlyXenoComponent);
                      this._stun.TryParalyze(contact, comp1_1.Duration, true, force: true);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<StunHostilesOnStepComponent, TransformComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<StunHostilesOnStepComponent, TransformComponent>();
label_15:
    StunHostilesOnStepComponent comp1_2;
    TransformComponent comp2_2;
    AttemptMobCollideEvent args3;
    EntityUid uid;
    do
    {
      do
      {
        if (!entityQueryEnumerator2.MoveNext(out uid, out comp1_2, out comp2_2))
          goto label_29;
      }
      while (!comp1_2.Enabled);
      this._contacts.Clear();
      this._physics.GetContactingEntities((Entity<PhysicsComponent>) uid, this._contacts);
      args3 = new AttemptMobCollideEvent();
      this.RaiseLocalEvent<AttemptMobCollideEvent>(uid, ref args3);
    }
    while (args3.Cancelled);
    goto label_18;
label_29:
    return;
label_18:
    using (HashSet<EntityUid>.Enumerator enumerator = this._contacts.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        EntityUid current = enumerator.Current;
        if (!this._mobState.IsDead(current) && !this._xenoRest.IsResting((Entity<XenoRestingComponent>) current) && !this._standingState.IsDown(current) && this._mobCollisionQuery.HasComp(current))
        {
          TransformComponent transformComponent = this.Transform(current);
          Box2 aabbNoContainer3 = this._entityLookup.GetAABBNoContainer(uid, comp2_2.LocalPosition, comp2_2.LocalRotation);
          Box2 aabbNoContainer4 = this._entityLookup.GetAABBNoContainer(current, transformComponent.LocalPosition, transformComponent.LocalRotation);
          if (((Box2) ref aabbNoContainer3).Intersects(ref aabbNoContainer4))
          {
            Box2 box2 = ((Box2) ref aabbNoContainer4).Intersect(ref aabbNoContainer3);
            float num = Box2.Area(ref box2);
            if ((double) Math.Max(num / Box2.Area(ref aabbNoContainer4), num / Box2.Area(ref aabbNoContainer3)) >= (double) comp1_2.Ratio)
            {
              AttemptMobTargetCollideEvent args4 = new AttemptMobTargetCollideEvent();
              this.RaiseLocalEvent<AttemptMobTargetCollideEvent>(current, ref args4);
              if (!args4.Cancelled && this._xeno.CanAbilityAttackTarget(uid, current))
              {
                RecentlyStunnedByHostileXenoComponent hostileXenoComponent = this.EnsureComp<RecentlyStunnedByHostileXenoComponent>(current);
                if (!(curTime < hostileXenoComponent.At + comp1_2.Cooldown))
                {
                  hostileXenoComponent.At = curTime;
                  this.Dirty(current, (IComponent) hostileXenoComponent);
                  this._stun.TryParalyze(current, comp1_2.Duration, true, force: true);
                  if (!this.HasComp<XenoComponent>(current))
                  {
                    FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?(current), comp1_2.Damage, origin: new EntityUid?(uid), tool: new EntityUid?(uid))?.GetTotal();
                    FixedPoint2 zero = FixedPoint2.Zero;
                    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
                    {
                      Filter filter1 = Filter.Pvs(current, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == uid));
                      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
                      Color red = Color.Red;
                      List<EntityUid> entities = new List<EntityUid>();
                      entities.Add(current);
                      Filter filter2 = filter1;
                      colorFlash.RaiseEffect(red, entities, filter2);
                    }
                  }
                }
              }
            }
          }
        }
      }
      goto label_15;
    }
  }
}
