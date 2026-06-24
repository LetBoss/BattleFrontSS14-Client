// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Punch.XenoPunchSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
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
namespace Content.Shared._RMC14.Xenonids.Punch;

public sealed class XenoPunchSystem : EntitySystem
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
  private RMCObstacleSlammingSystem _obstacleSlamming;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCSizeStunSystem _size;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoPunchComponent, XenoPunchActionEvent>(new EntityEventRefHandler<XenoPunchComponent, XenoPunchActionEvent>(this.OnXenoPunchAction));
  }

  private void OnXenoPunchAction(Entity<XenoPunchComponent> xeno, ref XenoPunchActionEvent args)
  {
    if (args.Handled)
      return;
    XenoPunchAttemptEvent args1 = new XenoPunchAttemptEvent();
    this.RaiseLocalEvent<XenoPunchAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    args.Handled = true;
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
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
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    this._rmcMelee.DoLunge((EntityUid) xeno, target);
    this._obstacleSlamming.MakeImmune(target);
    this._size.KnockBack(target, new MapCoordinates?(mapCoordinates), xeno.Comp.Range, xeno.Comp.Range, xeno.Comp.ThrowSpeed);
    if (!this.HasComp<XenoComponent>(target))
      this._slow.TrySlowdown(target, xeno.Comp.SlowDuration);
    if (!this._net.IsServer)
      return;
    this.SpawnAttachedTo((string) xeno.Comp.Effect, target.ToCoordinates(), rotation: new Angle());
  }
}
