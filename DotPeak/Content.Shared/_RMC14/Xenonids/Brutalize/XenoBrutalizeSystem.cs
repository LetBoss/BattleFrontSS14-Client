// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Brutalize.XenoBrutalizeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Charge;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Brutalize;

public sealed class XenoBrutalizeSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoBrutalizeComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoBrutalizeComponent, MeleeHitEvent>(this.OnBrutalMeleeHit));
  }

  private void OnBrutalMeleeHit(Entity<XenoBrutalizeComponent> xeno, ref MeleeHitEvent args)
  {
    EntityUid? nullable = new EntityUid?();
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity))
      {
        nullable = new EntityUid?(hitEntity);
        break;
      }
    }
    if (!nullable.HasValue)
      return;
    int hits = 0;
    DamageSpecifier damage = xeno.Comp.Damage;
    foreach (Entity<MobStateComponent> entity in this._entityLookup.GetEntitiesInRange<MobStateComponent>(this._transform.GetMapCoordinates(nullable.Value), xeno.Comp.Range))
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) entity) && !this._mobState.IsDead((EntityUid) entity) && !args.HitEntities.Contains<EntityUid>((EntityUid) entity))
      {
        ++hits;
        FixedPoint2? total = this._damageable.TryChangeDamage(new EntityUid?((EntityUid) entity), this._xeno.TryApplyXenoSlashDamageMultiplier((EntityUid) entity, damage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
        FixedPoint2 zero = FixedPoint2.Zero;
        if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
        {
          Filter filter1 = Filter.Pvs((EntityUid) entity, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
          SharedColorFlashEffectSystem colorFlash = this._colorFlash;
          Color red = Color.Red;
          List<EntityUid> entities = new List<EntityUid>();
          entities.Add((EntityUid) entity);
          Filter filter2 = filter1;
          colorFlash.RaiseEffect(red, entities, filter2);
        }
        if (xeno.Comp.MaxTargets.HasValue)
        {
          int num = hits;
          int? maxTargets = xeno.Comp.MaxTargets;
          int valueOrDefault = maxTargets.GetValueOrDefault();
          if (num >= valueOrDefault & maxTargets.HasValue)
            break;
        }
        if (this._net.IsServer)
          this.SpawnAttachedTo((string) xeno.Comp.Effect, entity.Owner.ToCoordinates(), rotation: new Angle());
      }
    }
    this.RefreshCooldowns(xeno, hits);
  }

  private void RefreshCooldowns(Entity<XenoBrutalizeComponent> xeno, int hits)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) xeno))
    {
      BaseActionEvent baseActionEvent = this._actions.GetEvent((EntityUid) action);
      if ((baseActionEvent is XenoChargeActionEvent || baseActionEvent is XenoDefensiveShieldActionEvent) && action.Comp.Cooldown.HasValue)
      {
        TimeSpan end = action.Comp.Cooldown.Value.End - (xeno.Comp.BaseCooldownReduction + (baseActionEvent is XenoChargeActionEvent ? (double) hits * xeno.Comp.AddtionalCooldownReductions : TimeSpan.Zero));
        if (end < action.Comp.Cooldown.Value.Start)
          this._actions.ClearCooldown(new Entity<ActionComponent>?(action.AsNullable()));
        else
          this._actions.SetCooldown(new Entity<ActionComponent>?(action.AsNullable()), action.Comp.Cooldown.Value.Start, end);
      }
    }
  }
}
