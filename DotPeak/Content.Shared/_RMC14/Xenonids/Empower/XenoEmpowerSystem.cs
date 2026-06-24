// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Empower.XenoEmpowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Empower;

public sealed class XenoEmpowerSystem : EntitySystem
{
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private XenoShieldSystem _shield;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private DamageableSystem _damagable;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoEmpowerComponent, XenoEmpowerActionEvent>(new EntityEventRefHandler<XenoEmpowerComponent, XenoEmpowerActionEvent>(this.OnXenoEmpowerAction));
    this.SubscribeLocalEvent<XenoEmpowerComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<XenoEmpowerComponent, BeforeDamageChangedEvent>(this.OnXenoEmpowerBeforeDamageChanged));
    this.SubscribeLocalEvent<XenoEmpowerComponent, RemovedShieldEvent>(new EntityEventRefHandler<XenoEmpowerComponent, RemovedShieldEvent>(this.OnXenoEmpowerShieldRemoved));
    this.SubscribeLocalEvent<XenoSuperEmpoweredComponent, GetMeleeDamageEvent>(new EntityEventRefHandler<XenoSuperEmpoweredComponent, GetMeleeDamageEvent>(this.OnXenoSuperEmpoweredGetMeleeDamage));
    this.SubscribeLocalEvent<XenoSuperEmpoweredComponent, RMCGetTailStabBonusDamageEvent>(new EntityEventRefHandler<XenoSuperEmpoweredComponent, RMCGetTailStabBonusDamageEvent>(this.OnXenoSuperEmpoweredGetTailDamage));
    this.SubscribeLocalEvent<XenoSuperEmpoweredComponent, XenoLeapHitEvent>(new EntityEventRefHandler<XenoSuperEmpoweredComponent, XenoLeapHitEvent>(this.OnXenoSuperEmpoweredLeapHit));
  }

  private void OnXenoEmpowerBeforeDamageChanged(
    Entity<XenoEmpowerComponent> xeno,
    ref BeforeDamageChangedEvent args)
  {
    if (xeno.Comp.ShieldDecayAt.HasValue && args.Damage.GetTotal() <= 0)
      return;
    xeno.Comp.ShieldDecayAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.ShieldDecayTime);
  }

  private void OnXenoEmpowerShieldRemoved(
    Entity<XenoEmpowerComponent> xeno,
    ref RemovedShieldEvent args)
  {
    if (args.Type != XenoShieldSystem.ShieldType.Ravager || !this._net.IsServer)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-ravager-shield-end"), (EntityUid) xeno, (EntityUid) xeno, PopupType.SmallCaution);
  }

  private void OnXenoEmpowerAction(
    Entity<XenoEmpowerComponent> xeno,
    ref XenoEmpowerActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (!xeno.Comp.ActivatedOnce)
    {
      this._actions.SetUseDelay(new Entity<ActionComponent>?(args.Action.AsNullable()), new TimeSpan?(TimeSpan.Zero));
      if (!this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.Cost))
        return;
      xeno.Comp.ActivatedOnce = true;
      this._shield.ApplyShield((EntityUid) xeno, XenoShieldSystem.ShieldType.Ravager, (FixedPoint2) xeno.Comp.InitialShield);
      xeno.Comp.ShieldDecayAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.ShieldDecayTime);
      xeno.Comp.TimeoutAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.TimeoutDuration);
      xeno.Comp.FirstActivationAt = this._timing.CurTime;
      foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoEmpowerActionEvent>((EntityUid) xeno))
        this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), true);
      this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-empower-start-self"), this.Loc.GetString("rmc-xeno-empower-start-others", ("user", (object) xeno)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
    }
    else
      this.FullEmpower(xeno);
  }

  private void FullEmpower(Entity<XenoEmpowerComponent> xeno)
  {
    if (this._net.IsClient)
      return;
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoEmpowerActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), false);
    this.SpawnAttachedTo((string) xeno.Comp.EmpowerEffect, xeno.Owner.ToCoordinates(), rotation: new Angle());
    xeno.Comp.ActivatedOnce = false;
    this._mobs.Clear();
    this._lookup.GetEntitiesInRange<MobStateComponent>(xeno.Owner.ToCoordinates(), xeno.Comp.Range, this._mobs);
    int num = 0;
    foreach (Entity<MobStateComponent> mob in this._mobs)
    {
      if (this._examine.InRangeUnOccluded(xeno.Owner, (EntityUid) mob, xeno.Comp.Range) && this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) mob) && !this.HasComp<XenoNestedComponent>((EntityUid) mob))
      {
        ++num;
        TileRef? tileRef = this._turf.GetTileRef(mob.Owner.ToCoordinates());
        if (tileRef.HasValue)
        {
          TileRef valueOrDefault = tileRef.GetValueOrDefault();
          this.SpawnAtPosition((string) xeno.Comp.TargetEffect, this._turf.GetTileCenter(valueOrDefault));
        }
        if (num >= xeno.Comp.MaxTargets)
          break;
      }
    }
    if (num > 0)
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-ravager-empower"), (EntityUid) xeno, (EntityUid) xeno, PopupType.SmallCaution);
    else
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-ravager-empower-fizzle"), (EntityUid) xeno, (EntityUid) xeno, PopupType.SmallCaution);
    this._shield.ApplyShield((EntityUid) xeno, XenoShieldSystem.ShieldType.Ravager, (FixedPoint2) (num * xeno.Comp.ShieldPerTarget));
    xeno.Comp.ShieldDecayAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.ShieldDecayTime);
    if (num >= xeno.Comp.SuperThreshold)
    {
      this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.RoarEmote);
      this._aura.GiveAura((EntityUid) xeno, xeno.Comp.SuperEmpowerColor, new TimeSpan?(xeno.Comp.SuperEmpowerPartialDuration), 4f);
      XenoSuperEmpoweredComponent empoweredComponent = this.EnsureComp<XenoSuperEmpoweredComponent>((EntityUid) xeno);
      empoweredComponent.PartialExpireAt = this._timing.CurTime + xeno.Comp.SuperEmpowerPartialDuration;
      empoweredComponent.EmpoweredTargets = num;
      empoweredComponent.DamageIncreasePer = xeno.Comp.DamageIncreasePer;
      empoweredComponent.DamageTailIncreasePer = xeno.Comp.DamageTailIncreasePer;
      empoweredComponent.LeapDamage = xeno.Comp.LeapDamage;
    }
    else
      this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.TailEmote);
    this.Dirty<XenoEmpowerComponent>(xeno);
    xeno.Comp.TimeoutAt = new TimeSpan?();
    this.DoCooldown(xeno);
  }

  private void OnXenoSuperEmpoweredGetMeleeDamage(
    Entity<XenoSuperEmpoweredComponent> xeno,
    ref GetMeleeDamageEvent args)
  {
    args.Damage += xeno.Comp.DamageIncreasePer * (float) xeno.Comp.EmpoweredTargets;
  }

  private void OnXenoSuperEmpoweredGetTailDamage(
    Entity<XenoSuperEmpoweredComponent> xeno,
    ref RMCGetTailStabBonusDamageEvent args)
  {
    args.Damage += xeno.Comp.DamageTailIncreasePer * (float) xeno.Comp.EmpoweredTargets;
  }

  private void OnXenoSuperEmpoweredLeapHit(
    Entity<XenoSuperEmpoweredComponent> xeno,
    ref XenoLeapHitEvent args)
  {
    if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, args.Hit))
      return;
    this._rmcPulling.TryStopAllPullsFromAndOn(args.Hit);
    FixedPoint2? total = this._damagable.TryChangeDamage(new EntityUid?(args.Hit), xeno.Comp.LeapDamage, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(args.Hit, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(args.Hit);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
    }
    if (this._net.IsClient)
      return;
    this._stun.TryParalyze(args.Hit, xeno.Comp.StunDuration, true);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    this._sizeStun.KnockBack(args.Hit, new MapCoordinates?(mapCoordinates), xeno.Comp.FlingDistance, xeno.Comp.FlingDistance, 10f, true);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoEmpowerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoEmpowerComponent>();
    EntityUid uid1;
    XenoEmpowerComponent comp1_1;
    TimeSpan? nullable;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (comp1_1.ShieldDecayAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        nullable = comp1_1.ShieldDecayAt;
        if ((nullable.HasValue ? (timeSpan >= nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          this._shield.RemoveShield(uid1, XenoShieldSystem.ShieldType.Ravager);
          comp1_1.ShieldDecayAt = new TimeSpan?();
        }
      }
      if (comp1_1.TimeoutAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        nullable = comp1_1.TimeoutAt;
        if ((nullable.HasValue ? (timeSpan < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          this.FullEmpower((Entity<XenoEmpowerComponent>) (uid1, comp1_1));
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoSuperEmpoweredComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<XenoSuperEmpoweredComponent>();
    EntityUid uid2;
    XenoSuperEmpoweredComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      int num1 = comp1_2.ExpiresAt.HasValue ? 1 : 0;
      TimeSpan timeSpan = curTime;
      nullable = comp1_2.ExpiresAt;
      int num2 = nullable.HasValue ? (timeSpan >= nullable.GetValueOrDefault() ? 1 : 0) : 0;
      if ((num1 & num2) != 0)
      {
        this.RemCompDeferred<XenoSuperEmpoweredComponent>(uid2);
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-ravager-super-empower-fade"), uid2, uid2, PopupType.SmallCaution);
      }
      else if (!comp1_2.ExpiresAt.HasValue && !(curTime < comp1_2.PartialExpireAt))
      {
        this._aura.GiveAura(uid2, comp1_2.FadingEmpowerColor, new TimeSpan?(comp1_2.ExpireTime), 3f);
        comp1_2.ExpiresAt = new TimeSpan?(curTime + comp1_2.ExpireTime);
      }
    }
  }

  private void DoCooldown(Entity<XenoEmpowerComponent> xeno)
  {
    foreach (Entity<ActionComponent> entity1 in this._rmcActions.GetActionsWithEvent<XenoEmpowerActionEvent>((EntityUid) xeno))
    {
      Entity<ActionComponent> entity2 = entity1.AsNullable();
      this._actions.SetToggled(new Entity<ActionComponent>?(entity2), false);
      TimeSpan cooldown = xeno.Comp.CooldownDuration - (this._timing.CurTime - xeno.Comp.FirstActivationAt);
      this._actions.SetUseDelay(new Entity<ActionComponent>?(entity2), new TimeSpan?(cooldown));
      this._actions.SetCooldown(new Entity<ActionComponent>?(entity2), cooldown);
    }
  }
}
