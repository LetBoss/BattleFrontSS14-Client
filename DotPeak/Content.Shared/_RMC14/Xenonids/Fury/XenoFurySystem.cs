// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fury.XenoFurySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Tantrum;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fury;

public sealed class XenoFurySystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private DamageableSystem _damageable;
  private readonly HashSet<Entity<XenoComponent>> _xenos = new HashSet<Entity<XenoComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoFuryComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoFuryComponent, MeleeHitEvent>(this.OnFuryHit));
  }

  private void OnFuryHit(Entity<XenoFuryComponent> xeno, ref MeleeHitEvent args)
  {
    if (!this._xeno.CanHeal((EntityUid) xeno))
      return;
    bool flag = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    int amount = this.HasComp<TantrumingComponent>((EntityUid) xeno) ? xeno.Comp.BoostedHeal : xeno.Comp.Heal;
    this._xenos.Clear();
    this._entityLookup.GetEntitiesInRange<XenoComponent>(xeno.Owner.ToCoordinates(), xeno.Comp.Range, this._xenos);
    foreach (Entity<XenoComponent> xeno1 in this._xenos)
    {
      if (this._xeno.CanHeal((EntityUid) xeno1) && !this._mob.IsDead((EntityUid) xeno1) && this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) xeno1.Owner))
      {
        DamageSpecifier damage = -this._rmcDamageable.DistributeTypesTotal((Entity<DamageableComponent>) xeno1.Owner, (FixedPoint2) amount);
        this._damageable.TryChangeDamage(new EntityUid?(xeno1.Owner), damage, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
        if (this._net.IsServer)
          this.SpawnAttachedTo((string) xeno.Comp.Effect, xeno1.Owner.ToCoordinates(), rotation: new Angle());
      }
    }
  }
}
