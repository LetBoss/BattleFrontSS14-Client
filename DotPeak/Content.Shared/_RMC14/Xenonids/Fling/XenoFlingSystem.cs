// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fling.XenoFlingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Rage;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fling;

public sealed class XenoFlingSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private RMCSlowSystem _rmcSlow;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedXenoHealSystem _xenoHeal;
  [Dependency]
  private XenoRageSystem _rage;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCDazedSystem _daze;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoFlingComponent, XenoFlingActionEvent>(new EntityEventRefHandler<XenoFlingComponent, XenoFlingActionEvent>(this.OnXenoFlingAction));
  }

  private void OnXenoFlingAction(Entity<XenoFlingComponent> xeno, ref XenoFlingActionEvent args)
  {
    if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, args.Target) || args.Handled)
      return;
    XenoFlingAttemptEvent args1 = new XenoFlingAttemptEvent();
    this.RaiseLocalEvent<XenoFlingAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    RMCSizes size;
    if (this._size.TryGetSize(args.Target, out size) && size >= RMCSizes.Big)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fling-too-big", ("target", (object) args.Target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
    }
    else
    {
      if (this._net.IsServer)
      {
        args.Handled = true;
        this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
      }
      int rage = this._rage.GetRage(xeno.Owner);
      EntityUid target = args.Target;
      this._rmcPulling.TryStopAllPullsFromAndOn(target);
      FixedPoint2? total = this._damageable.TryChangeDamage(new EntityUid?(target), this._xeno.TryApplyXenoSlashDamageMultiplier(target, xeno.Comp.Damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
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
      int healAmount = xeno.Comp.HealAmount;
      float range = xeno.Comp.Range;
      bool flag = false;
      if (rage >= 2)
      {
        range += xeno.Comp.EnragedRange;
        healAmount += xeno.Comp.EnragedHealAmount;
        flag = true;
      }
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
      this._rmcMelee.DoLunge((EntityUid) xeno, target);
      this._xenoHeal.CreateHealStacks((EntityUid) xeno, (FixedPoint2) healAmount, xeno.Comp.HealDelay, 1, xeno.Comp.HealDelay);
      if (!this._net.IsServer)
        return;
      this._rmcSlow.TrySlowdown(target, xeno.Comp.SlowTime);
      this._stun.TryParalyze(target, this._xeno.TryApplyXenoDebuffMultiplier(target, xeno.Comp.ParalyzeTime), true);
      if (flag)
        this._daze.TryDaze(target, xeno.Comp.DazeTime);
      this._daze.TryDaze(target, xeno.Comp.DazeTime, true);
      this._size.KnockBack(target, new MapCoordinates?(mapCoordinates), range, range, xeno.Comp.ThrowSpeed);
      this.SpawnAttachedTo((string) xeno.Comp.Effect, target.ToCoordinates(), rotation: new Angle());
    }
  }
}
