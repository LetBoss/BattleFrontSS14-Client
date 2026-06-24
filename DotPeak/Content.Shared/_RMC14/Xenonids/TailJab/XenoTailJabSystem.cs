// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailJab.XenoTailJabSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Rotate;
using Content.Shared._RMC14.Xenonids.ScissorCut;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Xenonids.TailJab;

public sealed class XenoTailJabSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private SharedColorFlashEffectSystem _flash;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private RMCObstacleSlammingSystem _rmcObstacleSlamming;
  [Dependency]
  private RMCSlowSystem _rmcSlow;
  [Dependency]
  private XenoRotateSystem _rotate;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  private const string WindowBonusDamageType = "Structural";
  private const int WindowDamageBonus = 100;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoTailJabComponent, XenoTailJabActionEvent>(new EntityEventRefHandler<XenoTailJabComponent, XenoTailJabActionEvent>(this.OnXenoImpaleAction));
  }

  private void OnXenoImpaleAction(
    Entity<XenoTailJabComponent> xeno,
    ref XenoTailJabActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((EntityTargetActionEvent) args))
      return;
    args.Handled = true;
    EntityUid target = args.Target;
    DamageSpecifier damageSpecifier = new DamageSpecifier(xeno.Comp.Damage);
    RMCGetTailStabBonusDamageEvent args1 = new RMCGetTailStabBonusDamageEvent(new DamageSpecifier());
    this.RaiseLocalEvent<RMCGetTailStabBonusDamageEvent>((EntityUid) xeno, ref args1);
    DamageSpecifier baseDamage = damageSpecifier + args1.Damage;
    if (this.HasComp<DestroyOnXenoPierceScissorComponent>(target))
      baseDamage.DamageDict.TryAdd("Structural", (FixedPoint2) 100);
    FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?(target), this._xeno.TryApplyXenoSlashDamageMultiplier(target, baseDamage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem flash = this._flash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      flash.RaiseEffect(red, entities, filter2);
    }
    this._rmcMelee.DoLunge((EntityUid) xeno, target);
    this._rmcSlow.TrySlowdown(target, xeno.Comp.SlowdownTime);
    this._rmcObstacleSlamming.ApplyBonuses(target, xeno.Comp.WallSlamStunTime, xeno.Comp.WallSlamSlowdownTime);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    this._size.KnockBack(target, new MapCoordinates?(mapCoordinates), xeno.Comp.ThrowRange, xeno.Comp.ThrowRange);
    Angle worldRotation = this._transform.GetWorldRotation((EntityUid) xeno);
    Angle angle = Angle.op_Subtraction(DirectionExtensions.ToAngle(((Angle) ref worldRotation).GetDir()), Angle.FromDegrees(180.0));
    this._rotate.RotateXeno((EntityUid) xeno, ((Angle) ref angle).GetDir());
    if (this._net.IsClient)
      return;
    this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote, cooldown: xeno.Comp.EmoteCooldown);
    this.SpawnAttachedTo((string) xeno.Comp.AttackEffect, target.ToCoordinates(), rotation: new Angle());
  }
}
