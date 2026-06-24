// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Smackdown.XenoSmackdownSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Slow;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Smackdown;

public sealed class XenoSmackdownSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoSmackdownComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoSmackdownComponent, MeleeHitEvent>(this.OnSmackdownMelee));
  }

  private void OnSmackdownMelee(Entity<XenoSmackdownComponent> xeno, ref MeleeHitEvent args)
  {
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity) && (this.HasComp<RMCSlowdownComponent>(hitEntity) || this.HasComp<RMCSuperSlowdownComponent>(hitEntity) || this.HasComp<RMCRootedComponent>(hitEntity) || this.HasComp<StunnedComponent>(hitEntity) || this._standing.IsDown(hitEntity)))
      {
        FixedPoint2? total = this._damageable.TryChangeDamage(new EntityUid?(hitEntity), this._xeno.TryApplyXenoSlashDamageMultiplier(hitEntity, xeno.Comp.Damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
        FixedPoint2 zero = FixedPoint2.Zero;
        if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
        {
          Filter filter1 = Filter.Pvs(hitEntity, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
          SharedColorFlashEffectSystem colorFlash = this._colorFlash;
          Color red = Color.Red;
          List<EntityUid> entities = new List<EntityUid>();
          entities.Add(hitEntity);
          Filter filter2 = filter1;
          colorFlash.RaiseEffect(red, entities, filter2);
        }
        this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
        break;
      }
    }
  }
}
