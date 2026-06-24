// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OnCollide.SharedOnCollideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor.ThermalCloak;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared._RMC14.Xenonids.Projectile.Spit;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.OnCollide;

public abstract class SharedOnCollideSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private ThermalCloakSystem _cloak;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private XenoSpitSystem _xenoSpit;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private StandingStateSystem _standing;
  private Robust.Shared.GameObjects.EntityQuery<CollideChainComponent> _collideChainQuery;
  private Robust.Shared.GameObjects.EntityQuery<DamageOnCollideComponent> _damageOnCollideQuery;
  private readonly List<Entity<DamageOnCollideComponent>> _damageOnCollide = new List<Entity<DamageOnCollideComponent>>();

  public override void Initialize()
  {
    this._collideChainQuery = this.GetEntityQuery<CollideChainComponent>();
    this._damageOnCollideQuery = this.GetEntityQuery<DamageOnCollideComponent>();
    this.SubscribeLocalEvent<DamageOnCollideComponent, StartCollideEvent>(new EntityEventRefHandler<DamageOnCollideComponent, StartCollideEvent>(this.OnStartCollide));
  }

  private void OnStartCollide(Entity<DamageOnCollideComponent> ent, ref StartCollideEvent args)
  {
    this.OnCollide(ent, args.OtherEntity);
  }

  private void OnCollide(Entity<DamageOnCollideComponent> ent, EntityUid other)
  {
    if (ent.Comp.Damaged.Contains(other) || !this._whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, other) || !ent.Comp.DamageDead && this._mobState.IsDead(other) || this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) other) || ent.Comp.Fire && this.HasComp<RMCImmuneToFireTileDamageComponent>(other))
      return;
    if (this.HasComp<UncloakOnHitComponent>(ent.Owner))
      this._cloak.TrySetInvisibility(other, false, true);
    ent.Comp.Damaged.Add(other);
    this.Dirty<DamageOnCollideComponent>(ent);
    bool flag = false;
    if (!ent.Comp.Chain.HasValue || this.AddToChain((Entity<CollideChainComponent>) ent.Comp.Chain.Value, other))
    {
      DamageSpecifier damageSpecifier = ent.Comp.Damage;
      if (ent.Comp.Acidic)
        damageSpecifier = this._xeno.TryApplyXenoAcidDamageMultiplier(other, damageSpecifier);
      this._damageable.TryChangeDamage(new EntityUid?(other), damageSpecifier, ent.Comp.IgnoreResistances);
      this.DoEmote(ent, other);
      flag = true;
    }
    else
    {
      DamageSpecifier damageSpecifier = ent.Comp.ChainDamage;
      if (ent.Comp.Acidic)
        damageSpecifier = this._xeno.TryApplyXenoAcidDamageMultiplier(other, damageSpecifier);
      this._damageable.TryChangeDamage(new EntityUid?(other), damageSpecifier, ent.Comp.IgnoreResistances);
    }
    this._xenoSpit.SetAcidCombo((Entity<UserAcidedComponent>) other, ent.Comp.AcidComboDuration, ent.Comp.AcidComboDamage, ent.Comp.AcidComboParalyze, ent.Comp.AcidComboResists);
    RMCSizes size;
    if (ent.Comp.Paralyze > TimeSpan.Zero && !this._standing.IsDown(other) && (!this._size.TryGetSize(other, out size) || size < RMCSizes.Big))
    {
      this._stun.TryParalyze(other, ent.Comp.Paralyze, true);
      if (!flag)
        this.DoEmote(ent, other);
    }
    DamageCollideEvent args = new DamageCollideEvent(other);
    this.RaiseLocalEvent<DamageCollideEvent>((EntityUid) ent, ref args);
  }

  protected virtual void DoEmote(Entity<DamageOnCollideComponent> ent, EntityUid other)
  {
  }

  private bool AddToChain(Entity<CollideChainComponent?> chain, EntityUid add)
  {
    if (!this._collideChainQuery.Resolve((EntityUid) chain, ref chain.Comp, false))
      return true;
    if (!chain.Comp.Hit.Add(add))
      return false;
    this.Dirty<CollideChainComponent>(chain);
    return true;
  }

  public Entity<CollideChainComponent> SpawnChain()
  {
    EntityUid uid = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
    CollideChainComponent collideChainComponent = this.EnsureComp<CollideChainComponent>(uid);
    return (Entity<CollideChainComponent>) (uid, collideChainComponent);
  }

  public void SetChain(Entity<DamageOnCollideComponent?> ent, EntityUid chain)
  {
    if (!this._damageOnCollideQuery.Resolve((EntityUid) ent, ref ent.Comp, false))
      return;
    ent.Comp.Chain = new EntityUid?(chain);
    this.Dirty<DamageOnCollideComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    this._damageOnCollide.Clear();
    try
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<DamageOnCollideComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DamageOnCollideComponent>();
      EntityUid uid;
      DamageOnCollideComponent comp1;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      {
        if (!comp1.InitDamaged)
        {
          comp1.InitDamaged = true;
          this._damageOnCollide.Add((Entity<DamageOnCollideComponent>) (uid, comp1));
        }
      }
      foreach (Entity<DamageOnCollideComponent> entity in this._damageOnCollide)
      {
        foreach (EntityUid other in this._physics.GetEntitiesIntersectingBody((EntityUid) entity, (int) entity.Comp.Collision))
          this.OnCollide(entity, other);
      }
    }
    finally
    {
      this._damageOnCollide.Clear();
    }
  }
}
