// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Stomp.XenoStompSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Stomp;

public sealed class XenoStompSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  private readonly HashSet<Entity<MobStateComponent>> _receivers = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoStompComponent, XenoStompActionEvent>(new EntityEventRefHandler<XenoStompComponent, XenoStompActionEvent>(this.OnXenoStompAction));
    this.SubscribeLocalEvent<XenoStompComponent, XenoStompDoAfterEvent>(new EntityEventRefHandler<XenoStompComponent, XenoStompDoAfterEvent>(this.OnXenoStompDoAfter));
  }

  private void OnXenoStompAction(Entity<XenoStompComponent> xeno, ref XenoStompActionEvent args)
  {
    XenoStompAttemptEvent args1 = new XenoStompAttemptEvent();
    this.RaiseLocalEvent<XenoStompAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled || this._mobState.IsDead((EntityUid) xeno) || !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    XenoStompDoAfterEvent @event = new XenoStompDoAfterEvent();
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true
    });
  }

  private void OnXenoStompDoAfter(Entity<XenoStompComponent> xeno, ref XenoStompDoAfterEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (args.Cancelled)
    {
      foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoStompActionEvent>((EntityUid) xeno))
        this._actions.ClearCooldown(new Entity<ActionComponent>?(entity.AsNullable()));
    }
    else
    {
      TransformComponent comp;
      if (!this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost) || !this.TryComp((EntityUid) xeno, out comp))
        return;
      this._receivers.Clear();
      this._entityLookup.GetEntitiesInRange<MobStateComponent>(comp.Coordinates, xeno.Comp.Range, this._receivers);
      if (this._net.IsServer)
        this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
      foreach (Entity<MobStateComponent> receiver in this._receivers)
      {
        if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) receiver))
        {
          RMCSizes size;
          if (xeno.Comp.SlowBigInsteadOfStun && this._size.TryGetSize((EntityUid) receiver, out size) && size >= RMCSizes.Big)
            this._slow.TrySlowdown((EntityUid) receiver, xeno.Comp.DebuffsHurtXenosMore ? this._xeno.TryApplyXenoDebuffMultiplier((EntityUid) receiver, xeno.Comp.ParalyzeTime) : xeno.Comp.ParalyzeTime);
          else if (!xeno.Comp.ParalyzeUnderOnly)
            this._stun.TryParalyze((EntityUid) receiver, xeno.Comp.DebuffsHurtXenosMore ? this._xeno.TryApplyXenoDebuffMultiplier((EntityUid) receiver, xeno.Comp.ParalyzeTime) : xeno.Comp.ParalyzeTime, true);
          if (xeno.Comp.Slows)
            this._slow.TrySuperSlowdown((EntityUid) receiver, xeno.Comp.SlowTime);
          float distance;
          if (comp.Coordinates.TryDistance((IEntityManager) this.EntityManager, receiver.Owner.ToCoordinates(), out distance) && (double) distance <= (double) xeno.Comp.ShortRange && this._standing.IsDown((EntityUid) receiver))
          {
            FixedPoint2? total = this._damageable.TryChangeDamage(new EntityUid?((EntityUid) receiver), this._xeno.TryApplyXenoSlashDamageMultiplier((EntityUid) receiver, xeno.Comp.Damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
            FixedPoint2 zero = FixedPoint2.Zero;
            if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
            {
              Filter filter1 = Filter.Pvs((EntityUid) receiver, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
              SharedColorFlashEffectSystem colorFlash = this._colorFlash;
              Color red = Color.Red;
              List<EntityUid> entities = new List<EntityUid>();
              entities.Add((EntityUid) receiver);
              Filter filter2 = filter1;
              colorFlash.RaiseEffect(red, entities, filter2);
            }
            if (xeno.Comp.ParalyzeUnderOnly && this._size.TryGetSize((EntityUid) receiver, out size) && size < RMCSizes.Big)
              this._stun.TryParalyze((EntityUid) receiver, xeno.Comp.DebuffsHurtXenosMore ? this._xeno.TryApplyXenoDebuffMultiplier((EntityUid) receiver, xeno.Comp.ParalyzeTime) : xeno.Comp.ParalyzeTime, true);
          }
        }
      }
      if (!this._net.IsServer)
        return;
      EntProtoId? selfEffect = xeno.Comp.SelfEffect;
      if (!selfEffect.HasValue)
        return;
      selfEffect = xeno.Comp.SelfEffect;
      this.SpawnAttachedTo(selfEffect.HasValue ? (string) selfEffect.GetValueOrDefault() : (string) null, xeno.Owner.ToCoordinates(), rotation: new Angle());
    }
  }
}
