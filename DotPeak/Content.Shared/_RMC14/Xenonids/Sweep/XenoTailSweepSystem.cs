// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Sweep.XenoTailSweepSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Sweep;

public sealed class XenoTailSweepSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private RotateToFaceSystem _rotateTo;
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
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedInteractionSystem _interact;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCObstacleSlammingSystem _obstacleSlamming;
  private readonly HashSet<Entity<MobStateComponent>> _hit = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoTailSweepComponent, XenoTailSweepActionEvent>(new EntityEventRefHandler<XenoTailSweepComponent, XenoTailSweepActionEvent>(this.OnXenoTailSweepAction));
  }

  private void OnXenoTailSweepAction(
    Entity<XenoTailSweepComponent> xeno,
    ref XenoTailSweepActionEvent args)
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) xeno, out comp))
      return;
    XenoTailSweepAttemptEvent args1 = new XenoTailSweepAttemptEvent();
    this.RaiseLocalEvent<XenoTailSweepAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this.EnsureComp<XenoSweepingComponent>((EntityUid) xeno);
    if (this._net.IsClient)
      return;
    this._hit.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(comp.Coordinates, xeno.Comp.Range, this._hit);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    foreach (Entity<MobStateComponent> entity in this._hit)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) entity) && this._interact.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entity.Owner, xeno.Comp.Range))
      {
        this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) entity);
        DamageSpecifier damage = xeno.Comp.Damage;
        if (damage != null)
          this._damageable.TryChangeDamage(new EntityUid?((EntityUid) entity), this._xeno.TryApplyXenoSlashDamageMultiplier((EntityUid) entity, damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
        Filter filter1 = Filter.Pvs((EntityUid) entity, entityManager: (IEntityManager) this.EntityManager);
        SharedColorFlashEffectSystem colorFlash = this._colorFlash;
        Color red = Color.Red;
        List<EntityUid> entities = new List<EntityUid>();
        entities.Add((EntityUid) entity);
        Filter filter2 = filter1;
        colorFlash.RaiseEffect(red, entities, filter2);
        this._obstacleSlamming.MakeImmune((EntityUid) entity);
        this._size.KnockBack((EntityUid) entity, new MapCoordinates?(mapCoordinates), xeno.Comp.KnockBackDistance, xeno.Comp.KnockBackDistance);
        RMCSizes size;
        if (!this._size.TryGetSize((EntityUid) entity, out size) || size < RMCSizes.Big)
          this._stun.TryParalyze((EntityUid) entity, this._xeno.TryApplyXenoDebuffMultiplier((EntityUid) entity, xeno.Comp.ParalyzeTime), true);
        this._audio.PlayPvs(xeno.Comp.HitSound, (EntityUid) entity);
        this.SpawnAttachedTo((string) xeno.Comp.HitEffect, entity.Owner.ToCoordinates(), rotation: new Angle());
      }
    }
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoSweepingComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoSweepingComponent, TransformComponent>();
    EntityUid uid;
    XenoSweepingComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp1.NextRotation > this._timing.CurTime))
      {
        if (comp1.TotalRotations >= comp1.MaxRotations)
        {
          this.RemCompDeferred<XenoSweepingComponent>(uid);
        }
        else
        {
          ++comp1.TotalRotations;
          comp1.NextRotation = this._timing.CurTime + comp1.Delay;
          XenoSweepingComponent sweepingComponent = comp1;
          sweepingComponent.LastDirection.GetValueOrDefault();
          if (!sweepingComponent.LastDirection.HasValue)
          {
            Angle worldRotation = this._transform.GetWorldRotation(comp2);
            Direction dir = ((Angle) ref worldRotation).GetDir();
            sweepingComponent.LastDirection = new Direction?(dir);
          }
          Angle diffAngle = Angle.op_Addition(DirectionExtensions.ToAngle(comp1.LastDirection.Value), Angle.FromDegrees(90.0));
          comp1.LastDirection = new Direction?(((Angle) ref diffAngle).GetDir());
          this.Dirty(uid, (IComponent) comp1);
          this._rotateTo.TryFaceAngle(uid, diffAngle, comp2);
        }
      }
    }
  }

  public override void FrameUpdate(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoSweepingComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoSweepingComponent, TransformComponent>();
    EntityUid uid;
    XenoSweepingComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      Direction? lastDirection = comp1.LastDirection;
      if (lastDirection.HasValue)
      {
        Direction valueOrDefault = lastDirection.GetValueOrDefault();
        this._rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(valueOrDefault), comp2);
      }
    }
  }
}
