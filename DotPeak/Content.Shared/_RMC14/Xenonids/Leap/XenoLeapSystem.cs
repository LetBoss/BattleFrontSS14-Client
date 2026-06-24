// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Leap.XenoLeapSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Barricade;
using Content.Shared._RMC14.Barricade.Components;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Invisibility;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Spray;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.ActionBlocker;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Leap;

public sealed class XenoLeapSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private BlindableSystem _blindable;
  [Dependency]
  private SharedBroadphaseSystem _broadphase;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCLagCompensationSystem _rmcLagCompensation;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private DamageableSystem _damagable;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private RMCCameraShakeSystem _cameraShake;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCObstacleSlammingSystem _obstacleSlamming;
  [Dependency]
  private SharedDirectionalAttackBlockSystem _directionalBlock;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixturesQuery;

  public override void Initialize()
  {
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    this.SubscribeAllEvent<XenoLeapPredictedHitEvent>(new EntitySessionEventHandler<XenoLeapPredictedHitEvent>(this.OnPredictedHit));
    this.SubscribeLocalEvent<XenoLeapComponent, XenoLeapActionEvent>(new EntityEventRefHandler<XenoLeapComponent, XenoLeapActionEvent>(this.OnXenoLeapAction));
    this.SubscribeLocalEvent<XenoLeapComponent, XenoLeapDoAfterEvent>(new EntityEventRefHandler<XenoLeapComponent, XenoLeapDoAfterEvent>(this.OnXenoLeapDoAfter));
    this.SubscribeLocalEvent<XenoLeapComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoLeapComponent, MeleeHitEvent>(this.OnXenoLeapMelee));
    this.SubscribeLocalEvent<XenoLeapComponent, RMCMeleeUserGetRangeEvent>(new EntityEventRefHandler<XenoLeapComponent, RMCMeleeUserGetRangeEvent>(this.OnXenoLeapingMeleeGetRange));
    this.SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotEquippedHandEvent>(new EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotEquippedHandEvent>(this.OnEquippedHand));
    this.SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotUnequippedHandEvent>(this.OnUnequippedHand));
    this.SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotEquippedEvent>(new EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<RMCGrantLeapProtectionComponent, GotUnequippedEvent>(new EntityEventRefHandler<RMCGrantLeapProtectionComponent, GotUnequippedEvent>(this.OnGotUnequipped));
    this.SubscribeLocalEvent<RMCLeapProtectionComponent, MapInitEvent>(new EntityEventRefHandler<RMCLeapProtectionComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<RMCLeapProtectionComponent, XenoLeapHitAttempt>(new EntityEventRefHandler<RMCLeapProtectionComponent, XenoLeapHitAttempt>(this.OnXenoLeapHitAttempt));
    this.SubscribeLocalEvent<XenoLeapingComponent, StartCollideEvent>(new EntityEventRefHandler<XenoLeapingComponent, StartCollideEvent>(this.OnXenoLeapingDoHit));
    this.SubscribeLocalEvent<XenoLeapingComponent, ComponentRemove>(new EntityEventRefHandler<XenoLeapingComponent, ComponentRemove>(this.OnXenoLeapingRemove));
    this.SubscribeLocalEvent<XenoLeapingComponent, PhysicsSleepEvent>(new EntityEventRefHandler<XenoLeapingComponent, PhysicsSleepEvent>(this.OnXenoLeapingPhysicsSleep));
    this.SubscribeLocalEvent<XenoLeapingComponent, StartPullAttemptEvent>(new EntityEventRefHandler<XenoLeapingComponent, StartPullAttemptEvent>(this.OnXenoLeapingStartPullAttempt));
    this.SubscribeLocalEvent<XenoLeapingComponent, PullAttemptEvent>(new EntityEventRefHandler<XenoLeapingComponent, PullAttemptEvent>(this.OnXenoLeapingPullAttempt));
  }

  private void OnPredictedHit(XenoLeapPredictedHitEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    XenoLeapingComponent comp;
    if (!this.TryComp<XenoLeapingComponent>(valueOrDefault, out comp))
      return;
    EntityUid entity = this.GetEntity(msg.Target);
    if (!entity.Valid)
      return;
    if (this._net.IsServer)
    {
      if (!this.HasComp<XenoLeapComponent>(valueOrDefault) || !comp.Running)
        return;
      this._rmcLagCompensation.SetLastRealTick(args.SenderSession.UserId, msg.LastRealTick);
      if (!this._rmcLagCompensation.Collides((Entity<FixturesComponent>) entity, (Entity<PhysicsComponent>) valueOrDefault, args.SenderSession))
        return;
    }
    this.ApplyLeapingHitEffects((Entity<XenoLeapingComponent>) (valueOrDefault, comp), entity);
  }

  private void OnXenoLeapAction(Entity<XenoLeapComponent> xeno, ref XenoLeapActionEvent args)
  {
    if (args.Handled)
      return;
    XenoLeapAttemptEvent args1 = new XenoLeapAttemptEvent();
    this.RaiseLocalEvent<XenoLeapAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled || xeno.Comp.PlasmaCost > FixedPoint2.Zero && !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    XenoLeapDoAfterEvent @event = new XenoLeapDoAfterEvent(this.GetNetCoordinates(args.Target));
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      DamageThreshold = FixedPoint2.New(10)
    });
  }

  private void OnXenoLeapDoAfter(Entity<XenoLeapComponent> xeno, ref XenoLeapDoAfterEvent args)
  {
    if (args.Handled)
      return;
    if (args.Cancelled)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-leap-cancelled"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    }
    else
    {
      PhysicsComponent component;
      XenoLeapingComponent comp1;
      if (!this._physicsQuery.TryGetComponent((EntityUid) xeno, out component) || this.EnsureComp<XenoLeapingComponent>((EntityUid) xeno, out comp1))
        return;
      args.Handled = true;
      comp1.KnockdownRequiresInvisibility = xeno.Comp.KnockdownRequiresInvisibility;
      comp1.DestroyObjects = xeno.Comp.DestroyObjects;
      comp1.MoveDelayTime = xeno.Comp.MoveDelayTime;
      comp1.Damage = xeno.Comp.Damage;
      comp1.HitEffect = xeno.Comp.HitEffect;
      comp1.TargetJitterTime = xeno.Comp.TargetJitterTime;
      comp1.TargetCameraShakeStrength = xeno.Comp.TargetCameraShakeStrength;
      comp1.IgnoredCollisionGroupLarge = xeno.Comp.IgnoredCollisionGroupLarge;
      comp1.IgnoredCollisionGroupSmall = xeno.Comp.IgnoredCollisionGroupSmall;
      if (xeno.Comp.PlasmaCost > FixedPoint2.Zero && !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
        return;
      this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) xeno);
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
      Vector2 vector2_1 = this._transform.ToMapCoordinates(args.Coordinates).Position - mapCoordinates.Position;
      if (vector2_1 == Vector2.Zero)
        return;
      float num1 = vector2_1.Length();
      float num2 = Math.Clamp(num1, 0.1f, xeno.Comp.Range.Float());
      Vector2 vector2_2 = vector2_1 * (num2 / num1);
      Vector2 impulse = Vector2Helpers.Normalized(vector2_2) * (float) xeno.Comp.Strength * component.Mass;
      comp1.Origin = this._transform.GetMoverCoordinates((EntityUid) xeno);
      comp1.ParalyzeTime = xeno.Comp.KnockdownTime;
      comp1.LeapSound = xeno.Comp.LeapSound;
      comp1.LeapEndTime = this._timing.CurTime + TimeSpan.FromSeconds((double) vector2_2.Length() / (double) xeno.Comp.Strength);
      this._obstacleSlamming.MakeImmune((EntityUid) xeno, 0.5f);
      this._physics.ApplyLinearImpulse((EntityUid) xeno, impulse, body: component);
      this._physics.SetBodyStatus((EntityUid) xeno, component, BodyStatus.InAir);
      FixturesComponent comp2;
      if (this.TryComp<FixturesComponent>((EntityUid) xeno, out comp2))
      {
        int num3 = (int) comp1.IgnoredCollisionGroupSmall;
        RMCSizes size;
        if (this._size.TryGetSize((EntityUid) xeno, out size) && size > RMCSizes.SmallXeno)
          num3 = (int) comp1.IgnoredCollisionGroupLarge;
        KeyValuePair<string, Fixture> keyValuePair = comp2.Fixtures.First<KeyValuePair<string, Fixture>>();
        this._physics.SetCollisionMask((EntityUid) xeno, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionMask ^ num3);
      }
      foreach (EntityUid contactingEntity in this._physics.GetContactingEntities(xeno.Owner, component))
      {
        if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) contactingEntity) && this.ApplyLeapingHitEffects((Entity<XenoLeapingComponent>) ((EntityUid) xeno, comp1), contactingEntity))
          break;
      }
    }
  }

  private void OnXenoLeapMelee(Entity<XenoLeapComponent> xeno, ref MeleeHitEvent args)
  {
    if (!xeno.Comp.UnrootOnMelee || !args.IsHit || args.HitEntities.Count == 0)
      return;
    using (IEnumerator<EntityUid> enumerator = args.HitEntities.GetEnumerator())
    {
      if (!enumerator.MoveNext())
        return;
      EntityUid current = enumerator.Current;
      if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, current))
        return;
      SlowedDownComponent comp;
      if (this.TryComp<SlowedDownComponent>((EntityUid) xeno, out comp) && (double) comp.SprintSpeedModifier == 0.0)
      {
        this.RemComp<SlowedDownComponent>((EntityUid) xeno);
        this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
      }
      xeno.Comp.LastHit = new EntityUid?();
      xeno.Comp.LastHitAt = new TimeSpan?();
      this.Dirty<XenoLeapComponent>(xeno);
    }
  }

  private void OnXenoLeapingMeleeGetRange(
    Entity<XenoLeapComponent> ent,
    ref RMCMeleeUserGetRangeEvent args)
  {
    if (!ent.Comp.LastHit.HasValue)
      return;
    EntityUid? lastHit = ent.Comp.LastHit;
    EntityUid? target = args.Target;
    if ((lastHit.HasValue == target.HasValue ? (lastHit.HasValue ? (lastHit.GetValueOrDefault() != target.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      return;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? lastHitAt = ent.Comp.LastHitAt;
    TimeSpan moveDelayTime = ent.Comp.MoveDelayTime;
    TimeSpan? nullable = lastHitAt.HasValue ? new TimeSpan?(lastHitAt.GetValueOrDefault() + moveDelayTime) : new TimeSpan?();
    if ((nullable.HasValue ? (curTime > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    args.Range = ent.Comp.LastHitRange;
  }

  private void OnXenoLeapingDoHit(Entity<XenoLeapingComponent> xeno, ref StartCollideEvent args)
  {
    this.ApplyLeapingHitEffects(xeno, args.OtherEntity);
  }

  private void OnXenoLeapingRemove(Entity<XenoLeapingComponent> ent, ref ComponentRemove args)
  {
    XenoLeapStoppedEvent args1 = new XenoLeapStoppedEvent();
    this.RaiseLocalEvent<XenoLeapStoppedEvent>((EntityUid) ent, ref args1);
    this.StopLeap(ent);
  }

  private void OnXenoLeapingPhysicsSleep(
    Entity<XenoLeapingComponent> ent,
    ref PhysicsSleepEvent args)
  {
    this.StopLeap(ent);
  }

  private void OnXenoLeapingStartPullAttempt(
    Entity<XenoLeapingComponent> ent,
    ref StartPullAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnXenoLeapingPullAttempt(Entity<XenoLeapingComponent> ent, ref PullAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnXenoLeapHitAttempt(
    Entity<RMCLeapProtectionComponent> ent,
    ref XenoLeapHitAttempt args)
  {
    XenoLeapingComponent comp;
    if (args.Cancelled || !this.TryComp<XenoLeapingComponent>(args.Leaper, out comp))
      return;
    args.Cancelled = this.AttemptBlockLeap(ent.Owner, ent.Comp.StunDuration, ent.Comp.BlockSound, args.Leaper, comp.Origin, ent.Comp.FullProtection);
  }

  private void OnGotEquipped(Entity<RMCGrantLeapProtectionComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    this.ApplyLeapProtection(args.Equipee, ent);
  }

  private void OnGotUnequipped(
    Entity<RMCGrantLeapProtectionComponent> ent,
    ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE || !this.RemoveLeapProtection(args.Equipee, ent))
      return;
    this.RemCompDeferred<RMCLeapProtectionComponent>(args.Equipee);
  }

  private void OnEquippedHand(
    Entity<RMCGrantLeapProtectionComponent> ent,
    ref GotEquippedHandEvent args)
  {
    if (!ent.Comp.ProtectsInHand)
      return;
    this.ApplyLeapProtection(args.User, ent);
  }

  private void OnUnequippedHand(
    Entity<RMCGrantLeapProtectionComponent> ent,
    ref GotUnequippedHandEvent args)
  {
    if (!ent.Comp.ProtectsInHand || !this.RemoveLeapProtection(args.User, ent))
      return;
    this.RemCompDeferred<RMCLeapProtectionComponent>(args.User);
  }

  private void OnMapInit(Entity<RMCLeapProtectionComponent> ent, ref MapInitEvent args)
  {
    if (!ent.Comp.InherentStunDuration.HasValue)
      return;
    ent.Comp.StunDuration = ent.Comp.InherentStunDuration.Value;
  }

  private void ApplyLeapProtection(
    EntityUid receiver,
    Entity<RMCGrantLeapProtectionComponent> protection)
  {
    RMCLeapProtectionComponent protectionComponent = this.EnsureComp<RMCLeapProtectionComponent>(receiver);
    protectionComponent.ProtectionProviders.Add((EntityUid) protection);
    if (protection.Comp.StunDuration >= protectionComponent.StunDuration)
    {
      protectionComponent.StunDuration = protection.Comp.StunDuration;
      protectionComponent.BlockSound = protection.Comp.BlockSound;
    }
    this.Dirty(receiver, (IComponent) protectionComponent);
  }

  private bool RemoveLeapProtection(
    EntityUid user,
    Entity<RMCGrantLeapProtectionComponent> protection)
  {
    RMCLeapProtectionComponent comp1;
    if (!this.TryComp<RMCLeapProtectionComponent>(user, out comp1))
      return true;
    TimeSpan timeSpan = new TimeSpan();
    comp1.ProtectionProviders.Remove((EntityUid) protection);
    if (comp1.InherentStunDuration.HasValue)
    {
      timeSpan = comp1.InherentStunDuration.Value;
      comp1.BlockSound = comp1.InherentBlockSound;
    }
    foreach (EntityUid protectionProvider in comp1.ProtectionProviders)
    {
      RMCGrantLeapProtectionComponent comp2;
      if (this.TryComp<RMCGrantLeapProtectionComponent>(protectionProvider, out comp2) && !(comp2.StunDuration < timeSpan))
      {
        timeSpan = comp2.StunDuration;
        comp1.BlockSound = comp2.BlockSound;
      }
    }
    if (!(timeSpan != TimeSpan.Zero))
      return true;
    comp1.StunDuration = timeSpan;
    this.Dirty(user, (IComponent) comp1);
    return false;
  }

  public bool AttemptBlockLeap(
    EntityUid blocker,
    TimeSpan stunDuration,
    SoundSpecifier blockSound,
    EntityUid leaper,
    EntityCoordinates leapOrigin,
    bool omnidirectionalProtection = false)
  {
    BarbedComponent comp;
    RMCSizes size;
    if (!this._directionalBlock.IsFacingTarget(blocker, leaper, new EntityCoordinates?(leapOrigin)) && !omnidirectionalProtection || this.HasComp<BarricadeComponent>(blocker) && (!this.TryComp<BarbedComponent>(blocker, out comp) || !comp.IsBarbed) || this._size.TryGetSize(leaper, out size) && size >= RMCSizes.Big && !this.HasComp<BarbedComponent>(blocker))
      return false;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(blocker, this.Transform(blocker));
    if (size < RMCSizes.Big)
      this._stun.TryParalyze(leaper, stunDuration, true);
    this._size.KnockBack(leaper, new MapCoordinates?(mapCoordinates), ignoreSize: true);
    this._audio.PlayPredicted(blockSound, leaper, new EntityUid?(leaper));
    this._popup.PopupClient(this.Loc.GetString("rmc-obstacle-slam-self", ("object", (object) Identity.Name(blocker, (IEntityManager) this.EntityManager, new EntityUid?(leaper)))), leaper, new EntityUid?(leaper), PopupType.MediumCaution);
    foreach (ICommonSession recipient in Filter.PvsExcept(leaper).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        this._popup.PopupEntity(this.Loc.GetString("rmc-obstacle-slam-others", ("ent", (object) Identity.Name(leaper, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), ("object", (object) Identity.Name(blocker, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), leaper, valueOrDefault, PopupType.MediumCaution);
      }
    }
    return true;
  }

  private bool IsValidLeapHit(Entity<XenoLeapingComponent> xeno, EntityUid target)
  {
    if (xeno.Comp.KnockedDown)
      return false;
    XenoLeapDestroyOnPassComponent comp;
    if (xeno.Comp.DestroyObjects && this.TryComp<XenoLeapDestroyOnPassComponent>(target, out comp))
    {
      if (this._net.IsServer)
      {
        for (int index = 0; index < comp.Amount; ++index)
        {
          if (comp.SpawnPrototype.HasValue)
          {
            EntProtoId? spawnPrototype = comp.SpawnPrototype;
            this.SpawnAtPosition(spawnPrototype.HasValue ? (string) spawnPrototype.GetValueOrDefault() : (string) null, target.ToCoordinates());
          }
        }
        this.QueueDel(new EntityUid?(target));
      }
      this._physics.SetCanCollide(target, false, force: true);
      return false;
    }
    RMCSizes size;
    return !this.HasComp<XenoParasiteComponent>(target) && !this.HasComp<XenoFruitComponent>(target) && !this.HasComp<XenoEggComponent>(target) && !this.HasComp<XenoAcidSplatterComponent>(target) && !this._standing.IsDown(target) && !this.HasComp<LeapIncapacitatedComponent>(target) && (!this._size.TryGetSize(target, out size) || size < RMCSizes.Big) && size != RMCSizes.VerySmallXeno && !this.HasComp<XenoWeedsComponent>(target) && !this.HasComp<XenoConstructComponent>(target);
  }

  private bool ApplyLeapingHitEffects(Entity<XenoLeapingComponent> xeno, EntityUid target)
  {
    if (!this.IsValidLeapHit(xeno, target))
      return false;
    if (this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) target))
    {
      this.StopLeap(xeno);
      return true;
    }
    XenoLeapHitAttempt args1 = new XenoLeapHitAttempt(xeno.Owner);
    this.RaiseLocalEvent<XenoLeapHitAttempt>(target, ref args1);
    if (args1.Cancelled)
    {
      xeno.Comp.KnockedDown = true;
      this.StopLeap(xeno);
      this.Dirty<XenoLeapingComponent>(xeno);
      return true;
    }
    if (!this.HasComp<MobStateComponent>(target) || this._mobState.IsIncapacitated(target))
      return false;
    xeno.Comp.KnockedDown = true;
    this.Dirty<XenoLeapingComponent>(xeno);
    XenoLeapComponent comp;
    if (this.TryComp<XenoLeapComponent>((EntityUid) xeno, out comp))
    {
      comp.LastHit = new EntityUid?(target);
      comp.LastHitAt = new TimeSpan?(this._timing.CurTime);
      this.Dirty((EntityUid) xeno, (IComponent) comp);
    }
    PhysicsComponent component;
    if (this._physicsQuery.TryGetComponent((EntityUid) xeno, out component))
    {
      this._physics.SetBodyStatus((EntityUid) xeno, component, BodyStatus.OnGround);
      if (component.Awake)
        this._broadphase.RegenerateContacts((EntityUid) xeno, component);
    }
    if (!xeno.Comp.KnockdownRequiresInvisibility || this.HasComp<XenoActiveInvisibleComponent>((EntityUid) xeno))
    {
      LeapIncapacitatedComponent incapacitatedComponent = this.EnsureComp<LeapIncapacitatedComponent>(target);
      incapacitatedComponent.RecoverAt = this._timing.CurTime + xeno.Comp.ParalyzeTime;
      this.Dirty(target, (IComponent) incapacitatedComponent);
      this._stun.TrySlowdown((EntityUid) xeno, xeno.Comp.MoveDelayTime, true, 0.0f, 0.0f);
      if (this._net.IsServer)
        this._stun.TryParalyze(target, this._xeno.TryApplyXenoDebuffMultiplier(target, xeno.Comp.ParalyzeTime), true);
    }
    if (xeno.Comp.HitEffect.HasValue && this._net.IsServer)
    {
      EntProtoId? hitEffect = xeno.Comp.HitEffect;
      this.SpawnAttachedTo(hitEffect.HasValue ? (string) hitEffect.GetValueOrDefault() : (string) null, target.ToCoordinates(), rotation: new Angle());
    }
    FixedPoint2? total = this._damagable.TryChangeDamage(new EntityUid?(target), this._xeno.TryApplyXenoSlashDamageMultiplier(target, xeno.Comp.Damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
    }
    this._jitter.DoJitter(target, xeno.Comp.TargetJitterTime, false);
    this._cameraShake.ShakeCamera(target, 2, xeno.Comp.TargetCameraShakeStrength);
    XenoLeapHitEvent args2 = new XenoLeapHitEvent((XenoLeapingComponent) xeno, target);
    this.RaiseLocalEvent<XenoLeapHitEvent>((EntityUid) xeno, ref args2);
    if (!xeno.Comp.PlayedSound && this._net.IsServer)
    {
      xeno.Comp.PlayedSound = true;
      this._audio.PlayPvs(xeno.Comp.LeapSound, (EntityUid) xeno);
    }
    if (this._net.IsClient)
    {
      XenoLeapPredictedHitEvent predictedHitEvent = new XenoLeapPredictedHitEvent(this.GetNetEntity(target), this._rmcLagCompensation.GetLastRealTick(new NetUserId?()));
      this.RaiseNetworkEvent((EntityEventArgs) predictedHitEvent);
      if (this._timing.InPrediction && this._timing.IsFirstTimePredicted)
        this.RaisePredictiveEvent<XenoLeapPredictedHitEvent>(predictedHitEvent);
    }
    this.StopLeap(xeno);
    return true;
  }

  private void StopLeap(Entity<XenoLeapingComponent> leaping)
  {
    PhysicsComponent component1;
    if (this._physicsQuery.TryGetComponent((EntityUid) leaping, out component1))
    {
      this._physics.SetLinearVelocity((EntityUid) leaping, Vector2.Zero, body: component1);
      this._physics.SetBodyStatus((EntityUid) leaping, component1, BodyStatus.OnGround);
    }
    FixturesComponent component2;
    if (this._fixturesQuery.TryGetComponent((EntityUid) leaping, out component2))
    {
      int num = (int) leaping.Comp.IgnoredCollisionGroupSmall;
      RMCSizes size;
      if (this._size.TryGetSize((EntityUid) leaping, out size) && size > RMCSizes.SmallXeno)
        num = (int) leaping.Comp.IgnoredCollisionGroupLarge;
      if (size >= RMCSizes.SmallXeno)
      {
        KeyValuePair<string, Fixture> keyValuePair = component2.Fixtures.First<KeyValuePair<string, Fixture>>();
        this._physics.SetCollisionMask((EntityUid) leaping, keyValuePair.Key, keyValuePair.Value, keyValuePair.Value.CollisionMask | num);
      }
    }
    this.RemCompDeferred<XenoLeapingComponent>((EntityUid) leaping);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoLeapingComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoLeapingComponent>();
    EntityUid uid1;
    XenoLeapingComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(curTime < comp1_1.LeapEndTime))
        this.StopLeap((Entity<XenoLeapingComponent>) (uid1, comp1_1));
    }
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<LeapIncapacitatedComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<LeapIncapacitatedComponent>();
    EntityUid uid2;
    LeapIncapacitatedComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(comp1_2.RecoverAt > curTime))
      {
        this.RemCompDeferred<LeapIncapacitatedComponent>(uid2);
        this._blindable.UpdateIsBlind((Entity<BlindableComponent>) uid2);
        this._actionBlocker.UpdateCanMove(uid2);
      }
    }
  }
}
