// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Eviscerate.XenoEviscerateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Rage;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Eviscerate;

public sealed class XenoEviscerateSystem : EntitySystem
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
  private SharedStunSystem _stun;
  [Dependency]
  private SharedXenoHealSystem _xenoHeal;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedInteractionSystem _interact;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private XenoRageSystem _rage;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  private readonly HashSet<Entity<MobStateComponent>> _hit = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoEviscerateComponent, XenoEviscerateActionEvent>(new EntityEventRefHandler<XenoEviscerateComponent, XenoEviscerateActionEvent>(this.OnXenoEviscerateAction));
    this.SubscribeLocalEvent<XenoEviscerateComponent, XenoEviscerateDoAfterEvent>(new EntityEventRefHandler<XenoEviscerateComponent, XenoEviscerateDoAfterEvent>(this.OnXenoEviscerateDoAfter));
  }

  private void OnXenoEviscerateAction(
    Entity<XenoEviscerateComponent> xeno,
    ref XenoEviscerateActionEvent args)
  {
    int rage = this._rage.GetRage(xeno.Owner);
    if (rage <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-eviscerate-fail"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      int num = rage - 1;
      TimeSpan reductionAtRageLevel = xeno.Comp.WindupReductionAtRageLevels[num];
      TimeSpan timeSpan = xeno.Comp.WindupTime - reductionAtRageLevel;
      XenoEviscerateDoAfterEvent @event = new XenoEviscerateDoAfterEvent(num);
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, timeSpan, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
      {
        BreakOnMove = true,
        Hidden = true,
        RootEntity = true,
        MovementThreshold = 0.5f
      }))
        return;
      args.Handled = true;
      this._stun.TrySlowdown((EntityUid) xeno, timeSpan, false, 0.0f, 0.0f);
      this._rage.IncrementRage((Entity<XenoRageComponent>) xeno.Owner, -1);
      if (rage > 1)
        this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-eviscerate-windup-self"), this.Loc.GetString("rmc-xeno-eviscerate-windup", (nameof (xeno), (object) xeno)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
      else
        this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-eviscerate-windup-small-self"), this.Loc.GetString("rmc-xeno-eviscerate-windup-small", (nameof (xeno), (object) xeno)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
    }
  }

  private void OnXenoEviscerateDoAfter(
    Entity<XenoEviscerateComponent> xeno,
    ref XenoEviscerateDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    this.EnsureComp<XenoSweepingComponent>((EntityUid) xeno);
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) xeno);
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.RoarEmote);
    DamageSpecifier damageAtRageLevel = xeno.Comp.DamageAtRageLevels[args.Rage];
    float rangeAtRageLevel = xeno.Comp.RangeAtRageLevels[args.Rage];
    TransformComponent transformComponent = this.Transform(xeno.Owner);
    if (this._net.IsClient)
      return;
    this._hit.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(transformComponent.Coordinates, rangeAtRageLevel, this._hit);
    int num = 0;
    this._transform.GetMapCoordinates((EntityUid) xeno);
    foreach (Entity<MobStateComponent> entity in this._hit)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) entity) && this._interact.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entity.Owner, rangeAtRageLevel))
      {
        this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) entity);
        this._damageable.TryChangeDamage(new EntityUid?((EntityUid) entity), this._xeno.TryApplyXenoSlashDamageMultiplier((EntityUid) entity, damageAtRageLevel), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
        Filter filter1 = Filter.Pvs((EntityUid) entity, entityManager: (IEntityManager) this.EntityManager);
        SharedColorFlashEffectSystem colorFlash = this._colorFlash;
        Color red = Color.Red;
        List<EntityUid> entities = new List<EntityUid>();
        entities.Add((EntityUid) entity);
        Filter filter2 = filter1;
        colorFlash.RaiseEffect(red, entities, filter2);
        if ((double) rangeAtRageLevel > 1.5)
        {
          this._audio.PlayPvs(xeno.Comp.RageHitSound, (EntityUid) entity);
          this._stun.TryParalyze((EntityUid) entity, xeno.Comp.StunTime, true);
        }
        else
          this._audio.PlayPvs(xeno.Comp.HitSound, (EntityUid) entity);
        ++num;
      }
    }
    int healAmount = Math.Clamp(num * xeno.Comp.LifeStealPerMarine, 0, xeno.Comp.MaxLifeSteal);
    this._xenoHeal.CreateHealStacks((EntityUid) xeno, (FixedPoint2) healAmount, xeno.Comp.HealDelay, 1, xeno.Comp.HealDelay);
  }
}
