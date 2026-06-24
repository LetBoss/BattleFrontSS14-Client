// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.RMCAreaDamageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Projectiles;

public sealed class RMCAreaDamageSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCAreaDamageComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCAreaDamageComponent, ProjectileHitEvent>(this.OnAreaDamageProjectileHit));
  }

  private void OnAreaDamageProjectileHit(
    Entity<RMCAreaDamageComponent> ent,
    ref ProjectileHitEvent args)
  {
    BeforeAreaDamageEvent args1 = new BeforeAreaDamageEvent(args.Target, args.Damage);
    this.RaiseLocalEvent<BeforeAreaDamageEvent>((EntityUid) ent, ref args1);
    if (args1.Cancelled)
      return;
    this.ApplyAreaDamage((EntityUid) ent, args.Target, args.Damage, args.Shooter);
  }

  private void ApplyAreaDamage(
    EntityUid uid,
    EntityUid target,
    DamageSpecifier damage,
    EntityUid? shooter = null,
    RMCAreaDamageComponent? areaDamage = null)
  {
    if (!this.Resolve<RMCAreaDamageComponent>(uid, ref areaDamage) || (double) areaDamage.DamageArea == 0.0 || !this.TryComp<MobStateComponent>(target, out MobStateComponent _))
      return;
    foreach (Entity<MobStateComponent> entity1 in this._entityLookup.GetEntitiesInRange<MobStateComponent>(this.Transform(target).Coordinates, areaDamage.DamageArea))
    {
      Entity<MobStateComponent> entity = entity1;
      if (!(entity.Owner == target))
      {
        EntityUid entityUid = (EntityUid) entity;
        EntityUid? nullable = shooter;
        if ((nullable.HasValue ? (entityUid == nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(target);
          Vector2 vector2 = this._transform.GetMapCoordinates((EntityUid) entity).Position - mapCoordinates.Position;
          DamageSpecifier damageSpecifier = damage;
          int num1 = 0;
          if ((double) areaDamage.FalloffDistance / (double) vector2.Length() < 1.0)
            damageSpecifier *= areaDamage.FalloffDistance / vector2.Length();
          RMCSizes size;
          this._sizeStun.TryGetSize((EntityUid) entity, out size);
          CMArmorPiercingComponent comp;
          if (this.TryComp<CMArmorPiercingComponent>(uid, out comp))
            num1 = comp.Amount;
          if (size >= RMCSizes.SmallXeno)
            damageSpecifier *= 2f;
          DamageableSystem damage1 = this._damage;
          EntityUid? uid1 = new EntityUid?((EntityUid) entity);
          DamageSpecifier damage2 = damageSpecifier;
          int num2 = num1;
          nullable = new EntityUid?();
          EntityUid? origin = nullable;
          nullable = new EntityUid?();
          EntityUid? tool = nullable;
          int armorPiercing = num2;
          FixedPoint2? total = damage1.TryChangeDamage(uid1, damage2, origin: origin, tool: tool, armorPiercing: armorPiercing)?.GetTotal();
          FixedPoint2 zero = FixedPoint2.Zero;
          if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0 && this._net.IsClient)
          {
            Filter filter1 = Filter.Pvs((EntityUid) entity, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (hit => hit == entity.Owner));
            SharedColorFlashEffectSystem colorFlash = this._colorFlash;
            Color red = Color.Red;
            List<EntityUid> entities = new List<EntityUid>();
            entities.Add((EntityUid) entity);
            Filter filter2 = filter1;
            colorFlash.RaiseEffect(red, entities, filter2);
          }
        }
      }
    }
  }
}
