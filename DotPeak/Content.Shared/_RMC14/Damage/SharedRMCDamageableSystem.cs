// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.SharedRMCDamageableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Armor;
using Content.Shared.Blocking;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Damage;

public abstract class SharedRMCDamageableSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MobThresholdSystem _mobThresholds;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  private static readonly ProtoId<DamageGroupPrototype> BruteGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  private static readonly ProtoId<DamageGroupPrototype> BurnGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  private static readonly ProtoId<DamageTypePrototype> LethalDamageType = (ProtoId<DamageTypePrototype>) "Asphyxiation";
  private readonly HashSet<ProtoId<DamageTypePrototype>> _bruteTypes = new HashSet<ProtoId<DamageTypePrototype>>();
  private readonly HashSet<ProtoId<DamageTypePrototype>> _burnTypes = new HashSet<ProtoId<DamageTypePrototype>>();
  private readonly List<string> _types = new List<string>();
  private Robust.Shared.GameObjects.EntityQuery<BarricadeComponent> _barricadeQuery;
  private Robust.Shared.GameObjects.EntityQuery<DamageableComponent> _damageableQuery;
  private Robust.Shared.GameObjects.EntityQuery<DamageOverTimeComponent> _damageOverTimeQuery;
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;
  private Robust.Shared.GameObjects.EntityQuery<VictimInfectedComponent> _victimInfectedQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoNestedComponent> _xenoNestedQuery;

  public override void Initialize()
  {
    this._barricadeQuery = this.GetEntityQuery<BarricadeComponent>();
    this._damageableQuery = this.GetEntityQuery<DamageableComponent>();
    this._damageOverTimeQuery = this.GetEntityQuery<DamageOverTimeComponent>();
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    this._victimInfectedQuery = this.GetEntityQuery<VictimInfectedComponent>();
    this._xenoNestedQuery = this.GetEntityQuery<XenoNestedComponent>();
    this.SubscribeLocalEvent<DamageMobStateComponent, MapInitEvent>(new EntityEventRefHandler<DamageMobStateComponent, MapInitEvent>(this.OnDamageMobStateMapInit));
    this.SubscribeLocalEvent<DamageOverTimeComponent, StartCollideEvent>(new EntityEventRefHandler<DamageOverTimeComponent, StartCollideEvent>(this.OnDamageOverTimeStartCollide));
    this.SubscribeLocalEvent<UserDamageOverTimeComponent, EndCollideEvent>(new EntityEventRefHandler<UserDamageOverTimeComponent, EndCollideEvent>(this.OnDamageOverTimeEndCollide));
    this.SubscribeLocalEvent<DamageMultiplierFlagsComponent, DamageModifyEvent>(new EntityEventRefHandler<DamageMultiplierFlagsComponent, DamageModifyEvent>(this.OnMultiplierFlagsDamageModify), after: new Type[7]
    {
      typeof (SharedArmorSystem),
      typeof (BlockingSystem),
      typeof (InventorySystem),
      typeof (SharedBorgSystem),
      typeof (SharedMarineOrdersSystem),
      typeof (CMArmorSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<GunDamageMultipliersComponent, AmmoShotEvent>(new EntityEventRefHandler<GunDamageMultipliersComponent, AmmoShotEvent>(this.OnGunDamageMultipliersAmmoShot));
    this.SubscribeLocalEvent<MaxDamageComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<MaxDamageComponent, BeforeDamageChangedEvent>(this.OnMaxBeforeDamageChanged));
    this.SubscribeLocalEvent<MaxDamageComponent, DamageModifyEvent>(new EntityEventRefHandler<MaxDamageComponent, DamageModifyEvent>(this.OnMaxDamageModify), after: new Type[7]
    {
      typeof (SharedArmorSystem),
      typeof (BlockingSystem),
      typeof (InventorySystem),
      typeof (SharedBorgSystem),
      typeof (SharedMarineOrdersSystem),
      typeof (CMArmorSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<DamageDealtModifierComponent, GetMeleeDamageEvent>(new EntityEventRefHandler<DamageDealtModifierComponent, GetMeleeDamageEvent>(this.OnDamageModifierGetMeleeDamage));
    this.SubscribeLocalEvent<DamageReceivedModifierComponent, DamageModifyEvent>(new EntityEventRefHandler<DamageReceivedModifierComponent, DamageModifyEvent>(this.OnDamageReceivedDamageModify), after: new Type[7]
    {
      typeof (SharedArmorSystem),
      typeof (BlockingSystem),
      typeof (InventorySystem),
      typeof (SharedBorgSystem),
      typeof (SharedMarineOrdersSystem),
      typeof (CMArmorSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<ProjectileDamageReceivedComponent, DamageModifyEvent>(new EntityEventRefHandler<ProjectileDamageReceivedComponent, DamageModifyEvent>(this.OnProjectileDamageReceivedModify), after: new Type[7]
    {
      typeof (SharedArmorSystem),
      typeof (BlockingSystem),
      typeof (InventorySystem),
      typeof (SharedBorgSystem),
      typeof (SharedMarineOrdersSystem),
      typeof (CMArmorSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<DamageOnPulledWhileCritComponent, MobStateChangedEvent>(new EntityEventRefHandler<DamageOnPulledWhileCritComponent, MobStateChangedEvent>(this.OnDamageOnPulledMobState));
    this.SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, MoveEvent>(new EntityEventRefHandler<ActiveDamageOnPulledWhileCritComponent, MoveEvent>(this.OnActiveDamageOnPulledMove));
    this.SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, MobStateChangedEvent>(new EntityEventRefHandler<ActiveDamageOnPulledWhileCritComponent, MobStateChangedEvent>(this.OnActiveDamageOnPulledMobState));
    this.SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, XenoTargetDevouredAttemptEvent>(new EntityEventRefHandler<ActiveDamageOnPulledWhileCritComponent, XenoTargetDevouredAttemptEvent>(this.OnActiveDamageOnPulledDevoured));
    this.SubscribeLocalEvent<DamageReceivedModifierWhenUnanchoredComponent, DamageModifyEvent>(new EntityEventRefHandler<DamageReceivedModifierWhenUnanchoredComponent, DamageModifyEvent>(this.OnDamageReceivedModifierWhenUnanchoredModify), after: new Type[7]
    {
      typeof (SharedArmorSystem),
      typeof (BlockingSystem),
      typeof (InventorySystem),
      typeof (SharedBorgSystem),
      typeof (SharedMarineOrdersSystem),
      typeof (CMArmorSystem),
      typeof (SharedXenoPheromonesSystem)
    });
    this._bruteTypes.Clear();
    this._burnTypes.Clear();
    DamageGroupPrototype prototype1;
    if (this._prototypes.TryIndex<DamageGroupPrototype>(SharedRMCDamageableSystem.BruteGroup, out prototype1))
    {
      foreach (string damageType in prototype1.DamageTypes)
        this._bruteTypes.Add((ProtoId<DamageTypePrototype>) damageType);
    }
    DamageGroupPrototype prototype2;
    if (!this._prototypes.TryIndex<DamageGroupPrototype>(SharedRMCDamageableSystem.BurnGroup, out prototype2))
      return;
    foreach (string damageType in prototype2.DamageTypes)
      this._burnTypes.Add((ProtoId<DamageTypePrototype>) damageType);
  }

  private void OnProjectileDamageReceivedModify(
    Entity<ProjectileDamageReceivedComponent> ent,
    ref DamageModifyEvent args)
  {
    if (!this.HasComp<ProjectileComponent>(args.Tool))
      return;
    args.Damage *= ent.Comp.Multiplier;
  }

  private void OnDamageMobStateMapInit(Entity<DamageMobStateComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.DamageAt = this._timing.CurTime + ent.Comp.Cooldown;
    this.DirtyField<DamageMobStateComponent>((EntityUid) ent, ent.Comp, "DamageAt");
  }

  private void OnDamageOverTimeStartCollide(
    Entity<DamageOverTimeComponent> ent,
    ref StartCollideEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid otherEntity = args.OtherEntity;
    if (!this.CanDamage(ent, (Entity<MobStateComponent>) otherEntity))
      return;
    this.EnsureComp<UserDamageOverTimeComponent>(otherEntity);
  }

  private void OnDamageOverTimeEndCollide(
    Entity<UserDamageOverTimeComponent> ent,
    ref EndCollideEvent args)
  {
    if (this._net.IsClient)
      return;
    foreach (EntityUid contactingEntity in this._physics.GetContactingEntities((EntityUid) ent, approximate: true))
    {
      if (this._damageOverTimeQuery.HasComp(contactingEntity))
        return;
    }
    this.RemCompDeferred<UserDamageOverTimeComponent>((EntityUid) ent);
  }

  private void OnMultiplierFlagsDamageModify(
    Entity<DamageMultiplierFlagsComponent> ent,
    ref DamageModifyEvent args)
  {
    DamageMultipliersComponent comp;
    if (!this._damageableQuery.HasComp((EntityUid) ent) || !this.TryComp<DamageMultipliersComponent>(args.Tool, out comp))
      return;
    foreach (DamageMultiplierFlag key in comp.Multipliers.Keys)
    {
      if ((ent.Comp.Flags & key) != DamageMultiplierFlag.None)
        args.Damage *= comp.Multipliers[key];
    }
  }

  private void OnGunDamageMultipliersAmmoShot(
    Entity<GunDamageMultipliersComponent> ent,
    ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
      this.EnsureComp<DamageMultipliersComponent>(firedProjectile).Multipliers = ent.Comp.Multipliers;
  }

  private void OnMaxBeforeDamageChanged(
    Entity<MaxDamageComponent> ent,
    ref BeforeDamageChangedEvent args)
  {
    DamageableComponent component;
    if (args.Cancelled || !this._damageableQuery.TryComp((EntityUid) ent, out component) || !(component.TotalDamage >= ent.Comp.Max) || !(args.Damage.GetTotal() > FixedPoint2.Zero))
      return;
    args.Cancelled = true;
  }

  private void OnMaxDamageModify(Entity<MaxDamageComponent> ent, ref DamageModifyEvent args)
  {
    DamageableComponent component;
    if (!this._damageableQuery.TryComp((EntityUid) ent, out component))
      return;
    FixedPoint2 total = args.Damage.GetTotal();
    if (total <= FixedPoint2.Zero || component.TotalDamage + total <= ent.Comp.Max)
      return;
    FixedPoint2 fixedPoint2 = ent.Comp.Max - component.TotalDamage;
    if (ent.Comp.Max <= FixedPoint2.Zero)
    {
      args.Damage *= 0.0f;
    }
    else
    {
      if (total <= fixedPoint2)
        return;
      args.Damage *= fixedPoint2.Float() / total.Float();
    }
  }

  private void OnDamageModifierGetMeleeDamage(
    Entity<DamageDealtModifierComponent> ent,
    ref GetMeleeDamageEvent args)
  {
    args.Damage *= ent.Comp.MeleeMultiplier;
  }

  private void OnDamageReceivedDamageModify(
    Entity<DamageReceivedModifierComponent> ent,
    ref DamageModifyEvent args)
  {
    foreach ((string key, FixedPoint2 fixedPoint2) in args.Damage.DamageDict)
    {
      string str = key;
      if (!(fixedPoint2 <= FixedPoint2.Zero))
      {
        if (ent.Comp.Multiplier != 1)
        {
          Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
          key = str;
          damageDict[key] *= ent.Comp.Multiplier;
        }
        if (ent.Comp.BruteMultiplier != 1 && this._bruteTypes.Contains((ProtoId<DamageTypePrototype>) str))
        {
          Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
          key = str;
          damageDict[key] *= ent.Comp.BruteMultiplier;
        }
        else if (ent.Comp.BurnMultiplier != 1 && this._burnTypes.Contains((ProtoId<DamageTypePrototype>) str))
        {
          Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
          key = str;
          damageDict[key] *= ent.Comp.BurnMultiplier;
        }
      }
    }
  }

  private void OnDamageOnPulledMobState(
    Entity<DamageOnPulledWhileCritComponent> ent,
    ref MobStateChangedEvent args)
  {
    ActiveDamageOnPulledWhileCritComponent comp;
    if (args.NewMobState != MobState.Critical || this.EnsureComp<ActiveDamageOnPulledWhileCritComponent>((EntityUid) ent, out comp))
      return;
    comp.Damage = ent.Comp.Damage;
    comp.PullerWhitelist = ent.Comp.PullerWhitelist;
    comp.Every = ent.Comp.Every;
    this.Dirty((EntityUid) ent, (IComponent) comp);
  }

  private void OnActiveDamageOnPulledMove(
    Entity<ActiveDamageOnPulledWhileCritComponent> ent,
    ref MoveEvent args)
  {
    if (ent.Comp.Damage == null)
      return;
    EntityUid user;
    if (!this._rmcPulling.IsBeingPulled((Entity<PullableComponent>) ent.Owner, out user) || !this._entityWhitelist.IsWhitelistPassOrNull(ent.Comp.PullerWhitelist, user))
    {
      if (!ent.Comp.Pulled)
        return;
      ent.Comp.Pulled = false;
      this.Dirty<ActiveDamageOnPulledWhileCritComponent>(ent);
    }
    else
    {
      float distance;
      if (!args.NewPosition.TryDistance((IEntityManager) this.EntityManager, this._transform, args.OldPosition, out distance))
        return;
      if (!ent.Comp.Pulled)
        ent.Comp.Pulled = true;
      ent.Comp.Accumulator += (double) distance;
      this.Dirty<ActiveDamageOnPulledWhileCritComponent>(ent);
    }
  }

  private void OnActiveDamageOnPulledMobState(
    Entity<ActiveDamageOnPulledWhileCritComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Critical)
      return;
    this.RemCompDeferred<ActiveDamageOnPulledWhileCritComponent>((EntityUid) ent);
  }

  private void OnActiveDamageOnPulledDevoured(
    Entity<ActiveDamageOnPulledWhileCritComponent> ent,
    ref XenoTargetDevouredAttemptEvent args)
  {
    FixedPoint2? threshold;
    DamageableComponent comp;
    if (this._mobThresholds.TryGetDeadThreshold(ent.Owner, out threshold) && this.TryComp<DamageableComponent>((EntityUid) ent, out comp))
    {
      FixedPoint2 fixedPoint2 = threshold.Value - comp.TotalDamage;
      DamageSpecifier damage = new DamageSpecifier(this._prototypes.Index<DamageTypePrototype>(SharedRMCDamageableSystem.LethalDamageType), fixedPoint2);
      this._damageable.TryChangeDamage(new EntityUid?(ent.Owner), damage, true);
    }
    args.Cancelled = true;
  }

  private void OnDamageReceivedModifierWhenUnanchoredModify(
    Entity<DamageReceivedModifierWhenUnanchoredComponent> ent,
    ref DamageModifyEvent args)
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp) || comp.Anchored || args.Damage.GetTotal() <= FixedPoint2.Zero)
      return;
    args.Damage *= ent.Comp.Multiplier;
  }

  public DamageSpecifier DistributeDamageCached(
    Entity<DamageableComponent?> damageable,
    ProtoId<DamageGroupPrototype> groupId,
    FixedPoint2 amount,
    DamageSpecifier? equal = null)
  {
    if (equal == null)
      equal = new DamageSpecifier();
    DamageGroupPrototype prototype;
    if (!this._damageableQuery.Resolve((EntityUid) damageable, ref damageable.Comp, false) || !this._prototypes.TryIndex<DamageGroupPrototype>(groupId, out prototype))
      return equal;
    this._types.Clear();
    foreach (string damageType in prototype.DamageTypes)
    {
      FixedPoint2 fixedPoint2;
      if (damageable.Comp.Damage.DamageDict.TryGetValue(damageType, out fixedPoint2) && fixedPoint2 > FixedPoint2.Zero)
        this._types.Add(damageType);
    }
    Dictionary<string, FixedPoint2> damageDict = equal.DamageDict;
    bool flag = amount > FixedPoint2.Zero;
    FixedPoint2 fixedPoint2_1 = amount;
    while ((flag ? (fixedPoint2_1 > 0 ? 1 : 0) : (fixedPoint2_1 < 0 ? 1 : 0)) != 0)
    {
      FixedPoint2 fixedPoint2_2 = fixedPoint2_1;
      for (int index = this._types.Count - 1; index >= 0; --index)
      {
        string type = this._types[index];
        FixedPoint2 b = damageable.Comp.Damage.DamageDict[type];
        FixedPoint2 fixedPoint2_3 = flag ? -damageDict.GetValueOrDefault<string, FixedPoint2>(type) : damageDict.GetValueOrDefault<string, FixedPoint2>(type);
        FixedPoint2 fixedPoint2_4 = fixedPoint2_1 + fixedPoint2_3;
        FixedPoint2 a = flag ? FixedPoint2.Min(fixedPoint2_3 + fixedPoint2_4 / (float) (index + 1), b) : -FixedPoint2.Min(-(fixedPoint2_3 + fixedPoint2_4 / (float) (index + 1)), b);
        if (b <= FixedPoint2.Abs(a))
          this._types.RemoveAt(index);
        damageDict[type] = a;
        fixedPoint2_1 = fixedPoint2_4 - a;
      }
      if (fixedPoint2_2 == fixedPoint2_1)
        break;
    }
    return equal;
  }

  public DamageSpecifier DistributeHealing(
    Entity<DamageableComponent?> damageable,
    ProtoId<DamageGroupPrototype> groupId,
    FixedPoint2 amount)
  {
    if (amount > FixedPoint2.Zero)
      amount = -amount;
    return this.DistributeDamageCached(damageable, groupId, amount);
  }

  public DamageSpecifier DistributeHealingCached(
    Entity<DamageableComponent?> damageable,
    ProtoId<DamageGroupPrototype> groupId,
    FixedPoint2 amount,
    DamageSpecifier? equal = null)
  {
    if (amount > FixedPoint2.Zero)
      amount = -amount;
    return this.DistributeDamageCached(damageable, groupId, amount, equal);
  }

  public DamageSpecifier DistributeTypes(Entity<DamageableComponent?> damageable, FixedPoint2 amount)
  {
    DamageSpecifier equal = new DamageSpecifier();
    foreach (DamageGroupPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<DamageGroupPrototype>())
      equal = this.DistributeDamageCached(damageable, (ProtoId<DamageGroupPrototype>) enumeratePrototype.ID, amount, equal);
    return equal;
  }

  public DamageSpecifier DistributeTypesTotal(
    Entity<DamageableComponent?> damageable,
    FixedPoint2 amount)
  {
    DamageSpecifier equal = new DamageSpecifier();
    foreach (DamageGroupPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<DamageGroupPrototype>())
    {
      FixedPoint2 fixedPoint2 = equal != null ? equal.GetTotal() : FixedPoint2.Zero;
      FixedPoint2 amount1 = amount - fixedPoint2;
      if (!(amount1 <= FixedPoint2.Zero))
        equal = this.DistributeDamageCached(damageable, (ProtoId<DamageGroupPrototype>) enumeratePrototype.ID, amount1, equal);
      else
        break;
    }
    return equal ?? new DamageSpecifier();
  }

  protected virtual void DoEmote(EntityUid ent, ProtoId<EmotePrototype> emote)
  {
  }

  private bool CanDamage(Entity<DamageOverTimeComponent> damage, Entity<MobStateComponent?> target)
  {
    return damage.Comp.BarricadeDamage != null && this._barricadeQuery.HasComp((EntityUid) target) || this.Resolve<MobStateComponent>((EntityUid) target, ref target.Comp, false) && this._entityWhitelist.IsWhitelistPassOrNull(damage.Comp.Whitelist, (EntityUid) target) && (damage.Comp.AffectsDead || !this._mobState.IsDead((EntityUid) target)) && (damage.Comp.AffectsCrit || !this._mobState.IsCritical((EntityUid) target)) && (damage.Comp.AffectsInfectedNested || !this._xenoNestedQuery.HasComp((EntityUid) target) || !this._victimInfectedQuery.HasComp((EntityUid) target));
  }

  private void DoDamage(
    Entity<DamageOverTimeComponent> damageEnt,
    EntityUid target,
    DamageSpecifier damage,
    bool ignoreResistances = false,
    bool acidic = false)
  {
    DamageSpecifier damageSpecifier = damage;
    if (acidic)
      damageSpecifier = this._xeno.TryApplyXenoAcidDamageMultiplier(target, damageSpecifier);
    List<DamageOverTimeComponent.DamageMultiplier> multipliers = damageEnt.Comp.Multipliers;
    if (multipliers != null)
    {
      foreach (DamageOverTimeComponent.DamageMultiplier damageMultiplier in multipliers)
      {
        if (this._entityWhitelist.IsWhitelistPass(damageMultiplier.Whitelist, target))
        {
          this._damageable.TryChangeDamage(new EntityUid?(target), damageSpecifier * damageMultiplier.Multiplier, ignoreResistances);
          return;
        }
      }
    }
    this._damageable.TryChangeDamage(new EntityUid?(target), damageSpecifier, ignoreResistances);
  }

  public virtual bool TryGetDestroyedAt(EntityUid destructible, [NotNullWhen(true)] out FixedPoint2? destroyed)
  {
    destroyed = new FixedPoint2?();
    return false;
  }

  public bool HasAnyDamage(Entity<DamageableComponent?> damageable, DamageSpecifier damage)
  {
    if (!this._damageableQuery.Resolve((EntityUid) damageable, ref damageable.Comp, false))
      return false;
    foreach ((string key, FixedPoint2 _) in damage.DamageDict)
    {
      FixedPoint2 fixedPoint2;
      if (damageable.Comp.Damage.DamageDict.TryGetValue(key, out fixedPoint2) && fixedPoint2 > FixedPoint2.Zero)
        return true;
    }
    return false;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DamageMobStateComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<DamageMobStateComponent>();
    EntityUid uid1;
    DamageMobStateComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(curTime < comp1_1.DamageAt))
      {
        comp1_1.DamageAt = curTime + comp1_1.Cooldown;
        this.DirtyField<DamageMobStateComponent>(uid1, comp1_1, "DamageAt");
        MobStateComponent component1;
        DamageableComponent component2;
        if (this._mobStateQuery.TryComp(uid1, out component1) && this._damageableQuery.TryComp(uid1, out component2))
        {
          switch (component1.CurrentState)
          {
            case MobState.Alive:
              if (this.HasAnyDamage((Entity<DamageableComponent>) (uid1, component2), comp1_1.NonDeadDamage))
              {
                this._damageable.TryChangeDamage(new EntityUid?(uid1), comp1_1.NonDeadDamage, true, damageable: component2);
                continue;
              }
              continue;
            case MobState.Critical:
              this._damageable.TryChangeDamage(new EntityUid?(uid1), comp1_1.NonDeadDamage, true, damageable: component2);
              DamageStateCritBeforeDamageEvent args = new DamageStateCritBeforeDamageEvent(comp1_1.CritDamage);
              this.RaiseLocalEvent<DamageStateCritBeforeDamageEvent>(uid1, ref args);
              this._damageable.TryChangeDamage(new EntityUid?(uid1), args.Damage, true, damageable: component2);
              continue;
            default:
              continue;
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveDamageOnPulledWhileCritComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<ActiveDamageOnPulledWhileCritComponent>();
    EntityUid uid2;
    ActiveDamageOnPulledWhileCritComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (comp1_2.Accumulator >= comp1_2.Every)
      {
        double num1 = comp1_2.Accumulator / comp1_2.Every;
        double num2 = comp1_2.Accumulator % comp1_2.Every;
        double num3 = num1;
        if (comp1_2.Damage != null)
          this._damageable.TryChangeDamage(new EntityUid?(uid2), comp1_2.Damage * (FixedPoint2) num3);
        comp1_2.Accumulator = num2;
      }
    }
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DamageOverTimeComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<DamageOverTimeComponent>();
    EntityUid uid3;
    DamageOverTimeComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      if (curTime >= comp1_3.NextDamageAt)
      {
        comp1_3.NextDamageAt = curTime + comp1_3.DamageEvery;
        RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(uid3, facing: (DirectionFlag) 0);
        EntityUid uid4;
        while (entitiesEnumerator.MoveNext(out uid4))
        {
          if (this._barricadeQuery.HasComp(uid4) && comp1_3.BarricadeDamage != null)
          {
            UserDamageOverTimeComponent overTimeComponent = this.EnsureComp<UserDamageOverTimeComponent>(uid4);
            if (!(curTime < overTimeComponent.NextDamageAt))
            {
              overTimeComponent.NextDamageAt = curTime;
              this.DoDamage((Entity<DamageOverTimeComponent>) (uid3, comp1_3), uid4, comp1_3.BarricadeDamage);
              if (this._random.Prob(0.75f))
                this._audio.PlayPvs(comp1_3.BarricadeSound, uid4);
            }
          }
        }
      }
      if (!comp1_3.InitDamaged)
      {
        comp1_3.InitDamaged = true;
        foreach (EntityUid entityUid in this._physics.GetEntitiesIntersectingBody(uid3, (int) comp1_3.Collision))
        {
          if (this.CanDamage((Entity<DamageOverTimeComponent>) (uid3, comp1_3), (Entity<MobStateComponent>) entityUid))
            this.EnsureComp<UserDamageOverTimeComponent>(entityUid);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<UserDamageOverTimeComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<UserDamageOverTimeComponent>();
    EntityUid uid5;
    UserDamageOverTimeComponent comp1_4;
    while (entityQueryEnumerator4.MoveNext(out uid5, out comp1_4))
    {
      if (!(curTime < comp1_4.NextDamageAt))
      {
        HashSet<EntityUid> intersectingBody = this._physics.GetEntitiesIntersectingBody(uid5, (int) comp1_4.Collision);
        if (intersectingBody.Count == 0)
        {
          this.RemCompDeferred<UserDamageOverTimeComponent>(uid5);
        }
        else
        {
          foreach (EntityUid entityUid in intersectingBody)
          {
            DamageOverTimeComponent component;
            if (this._damageOverTimeQuery.TryComp(entityUid, out component) && (component.AffectsDead || !this._mobState.IsDead(uid5)) && (component.AffectsCrit || !this._mobState.IsCritical(uid5)) && (component.AffectsInfectedNested || !this._xenoNestedQuery.HasComp(uid5) || !this._victimInfectedQuery.HasComp(uid5)) && this._entityWhitelist.IsWhitelistPassOrNull(component.Whitelist, uid5) && !this._hive.FromSameHive((Entity<HiveMemberComponent>) entityUid, (Entity<HiveMemberComponent>) uid5))
            {
              comp1_4.NextDamageAt = curTime + comp1_4.DamageEvery;
              if (component.Damage != null)
                this.DoDamage((Entity<DamageOverTimeComponent>) (entityUid, component), uid5, component.Damage);
              if (component.ArmorPiercingDamage != null)
                this.DoDamage((Entity<DamageOverTimeComponent>) (entityUid, component), uid5, component.ArmorPiercingDamage, true, component.Acidic);
              List<ProtoId<EmotePrototype>> emotes = component.Emotes;
              if (emotes != null && emotes.Count > 0)
              {
                ProtoId<EmotePrototype> emote = RandomExtensions.Pick<ProtoId<EmotePrototype>>(this._random, (IReadOnlyList<ProtoId<EmotePrototype>>) emotes);
                this.DoEmote(uid5, emote);
              }
              string popup = component.Popup;
              if (popup != null && this._random.Prob(0.5f))
                this._popup.PopupEntity(popup, uid5, uid5, PopupType.SmallCaution);
              this._audio.PlayPvs(component.Sound, uid5);
              break;
            }
          }
        }
      }
    }
  }
}
