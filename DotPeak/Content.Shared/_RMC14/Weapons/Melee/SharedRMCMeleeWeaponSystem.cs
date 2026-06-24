// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Melee.SharedRMCMeleeWeaponSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Melee;

public abstract class SharedRMCMeleeWeaponSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private SharedMeleeWeaponSystem _melee;
  [Dependency]
  private INetConfigurationManager _netConfig;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private Robust.Shared.GameObjects.EntityQuery<MeleeWeaponComponent> _meleeWeaponQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoComponent> _xenoQuery;

  public override void Initialize()
  {
    this._meleeWeaponQuery = this.GetEntityQuery<MeleeWeaponComponent>();
    this._xenoQuery = this.GetEntityQuery<XenoComponent>();
    this.SubscribeLocalEvent<ActorComponent, AttackAttemptEvent>(new EntityEventRefHandler<ActorComponent, AttackAttemptEvent>(this.OnActorAttackAttempt));
    this.SubscribeLocalEvent<ImmuneToUnarmedComponent, GettingAttackedAttemptEvent>(new EntityEventRefHandler<ImmuneToUnarmedComponent, GettingAttackedAttemptEvent>(this.OnImmuneToUnarmedGettingAttacked));
    this.SubscribeLocalEvent<ImmuneToMeleeComponent, GettingAttackedAttemptEvent>(new EntityEventRefHandler<ImmuneToMeleeComponent, GettingAttackedAttemptEvent>(this.OnImmuneToMeleeGettingAttacked));
    this.SubscribeLocalEvent<MeleeReceivedMultiplierComponent, DamageModifyEvent>(new EntityEventRefHandler<MeleeReceivedMultiplierComponent, DamageModifyEvent>(this.OnMeleeReceivedMultiplierDamageModify));
    this.SubscribeLocalEvent<StunOnHitComponent, MeleeHitEvent>(new EntityEventRefHandler<StunOnHitComponent, MeleeHitEvent>(this.OnStunOnHitMeleeHit));
    this.SubscribeLocalEvent<MeleeDamageMultiplierComponent, MeleeHitEvent>(new EntityEventRefHandler<MeleeDamageMultiplierComponent, MeleeHitEvent>(this.OnMultiplierOnHitMeleeHit));
    this.SubscribeLocalEvent<RMCMeleeDamageSkillComponent, MeleeHitEvent>(new EntityEventRefHandler<RMCMeleeDamageSkillComponent, MeleeHitEvent>(this.OnSkilledOnHitMeleeHit));
    this.SubscribeAllEvent<LightAttackEvent>(new EntitySessionEventHandler<LightAttackEvent>(this.OnLightAttack), new Type[1]
    {
      typeof (SharedMeleeWeaponSystem)
    });
    this.SubscribeAllEvent<HeavyAttackEvent>(new EntitySessionEventHandler<HeavyAttackEvent>(this.OnHeavyAttack), new Type[1]
    {
      typeof (SharedMeleeWeaponSystem)
    });
    this.SubscribeAllEvent<DisarmAttackEvent>(new EntitySessionEventHandler<DisarmAttackEvent>(this.OnDisarmAttack), new Type[1]
    {
      typeof (SharedMeleeWeaponSystem)
    });
  }

  public void MeleeResetInit(Entity<MeleeResetComponent> ent)
  {
    MeleeWeaponComponent comp;
    if (!this.TryComp<MeleeWeaponComponent>((EntityUid) ent, out comp))
    {
      this.RemComp<MeleeResetComponent>((EntityUid) ent);
    }
    else
    {
      ent.Comp.OriginalTime = comp.NextAttack;
      comp.NextAttack = this._timing.CurTime;
      this.Dirty((EntityUid) ent, (IComponent) comp);
      this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    }
  }

  private void OnStunOnHitMeleeHit(Entity<StunOnHitComponent> ent, ref MeleeHitEvent args)
  {
    if (!args.IsHit)
      return;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._whitelist.IsValid(ent.Comp.Whitelist, hitEntity))
        this._stun.TryParalyze(hitEntity, ent.Comp.Duration, true);
    }
  }

  private void OnMultiplierOnHitMeleeHit(
    Entity<MeleeDamageMultiplierComponent> ent,
    ref MeleeHitEvent args)
  {
    if (!args.IsHit)
      return;
    MeleeDamageMultiplierComponent comp = ent.Comp;
    args.BonusDamage = this._skills.ApplyMeleeSkillModifier(args.User, args.BonusDamage);
    DamageSpecifier damageSpecifier1 = args.BaseDamage + args.BonusDamage;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._whitelist.IsValid(comp.Whitelist, hitEntity))
      {
        DamageSpecifier damageSpecifier2 = damageSpecifier1 * comp.Multiplier;
        args.BonusDamage += damageSpecifier2;
        break;
      }
    }
  }

  private void OnSkilledOnHitMeleeHit(
    Entity<RMCMeleeDamageSkillComponent> ent,
    ref MeleeHitEvent args)
  {
    DamageGroupPrototype prototype;
    if (!args.IsHit || !this._prototypeManager.TryIndex<DamageGroupPrototype>((string) ent.Comp.BonusDamageType, out prototype))
      return;
    DamageSpecifier damageSpecifier = new DamageSpecifier(prototype, (FixedPoint2) this._skills.GetSkill((Entity<SkillsComponent>) ent.Owner, ent.Comp.Skill));
    args.BonusDamage += damageSpecifier;
  }

  private void OnActorAttackAttempt(Entity<ActorComponent> ent, ref AttackAttemptEvent args)
  {
    EntityUid uid = args.Uid;
    EntityUid? target = args.Target;
    if ((target.HasValue ? (uid != target.GetValueOrDefault() ? 1 : 0) : 1) != 0 || this._netConfig.GetClientCVar<bool>(ent.Comp.PlayerSession.Channel, RMCCVars.RMCDamageYourself))
      return;
    args.Cancel();
  }

  private void OnImmuneToUnarmedGettingAttacked(
    Entity<ImmuneToUnarmedComponent> ent,
    ref GettingAttackedAttemptEvent args)
  {
    if (!ent.Comp.ApplyToXenos && this._xenoQuery.HasComp(args.Attacker))
      return;
    EntityUid attacker = args.Attacker;
    EntityUid? weapon = args.Weapon;
    if ((weapon.HasValue ? (attacker == weapon.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      return;
    args.Cancelled = true;
  }

  private void OnImmuneToMeleeGettingAttacked(
    Entity<ImmuneToMeleeComponent> ent,
    ref GettingAttackedAttemptEvent args)
  {
    if (!this._whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Attacker))
      return;
    args.Cancelled = true;
  }

  private void OnMeleeReceivedMultiplierDamageModify(
    Entity<MeleeReceivedMultiplierComponent> ent,
    ref DamageModifyEvent args)
  {
    if (!this._meleeWeaponQuery.HasComp(args.Tool))
      return;
    if (this._xenoQuery.HasComp(args.Origin))
      args.Damage = new DamageSpecifier(ent.Comp.XenoDamage);
    else
      args.Damage *= ent.Comp.OtherMultiplier;
  }

  private void OnLightAttack(LightAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!attachedEntity.HasValue || !this._melee.TryGetWeapon(attachedEntity.GetValueOrDefault(), out weaponUid, out melee) || weaponUid != this.GetEntity(msg.Weapon))
      return;
    this.TryMeleeReset(weaponUid, melee, false);
  }

  private void OnHeavyAttack(HeavyAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!attachedEntity.HasValue || !this._melee.TryGetWeapon(attachedEntity.GetValueOrDefault(), out weaponUid, out melee) || weaponUid != this.GetEntity(msg.Weapon))
      return;
    this.TryMeleeReset(weaponUid, melee, false);
  }

  private void OnDisarmAttack(DisarmAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!attachedEntity.HasValue || !this._melee.TryGetWeapon(attachedEntity.GetValueOrDefault(), out weaponUid, out melee))
      return;
    this.TryMeleeReset(weaponUid, melee, true);
  }

  private void TryMeleeReset(EntityUid weaponUid, MeleeWeaponComponent weapon, bool disarm)
  {
    MeleeResetComponent comp;
    if (!this.TryComp<MeleeResetComponent>(weaponUid, out comp))
      return;
    if (disarm)
      weapon.NextAttack = comp.OriginalTime;
    this.RemComp<MeleeResetComponent>(weaponUid);
    this.Dirty(weaponUid, (IComponent) weapon);
  }

  public void DoLunge(EntityUid user, EntityUid target)
  {
    TransformComponent component = this.Transform(user);
    Vector2 vector2 = Vector2.Transform(this._transform.GetWorldPosition(target), this._transform.GetInvWorldMatrix(component));
    Angle localRotation = component.LocalRotation;
    Vector2 localPos = ((Angle) ref localRotation).RotateVec(ref vector2);
    this._melee.DoLunge(user, target, Angle.Zero, localPos, (string) null);
  }

  public bool AttemptOverrideAttack(
    EntityUid target,
    Entity<MeleeWeaponComponent> weapon,
    EntityUid user,
    AttackEvent attack,
    out AttackEvent newAttack,
    float range = 1.5f)
  {
    Vector2 position1 = this._transform.GetMoverCoordinates(target).Position;
    Vector2 position2 = this._transform.GetMoverCoordinates(user).Position;
    List<NetEntity> netEntityList = this.GetNetEntityList(this._melee.ArcRayCast(position2, DirectionExtensions.ToWorldAngle(position1 - position2), Angle.op_Implicit(0.0f), range, this._transform.GetMapId((Entity<TransformComponent>) user), user).ToList<EntityUid>());
    MeleeAttackAttemptEvent args = new MeleeAttackAttemptEvent(this.GetNetEntity(target), attack, attack.Coordinates, netEntityList, this.GetNetEntity((EntityUid) weapon));
    this.RaiseLocalEvent<MeleeAttackAttemptEvent>(user, ref args);
    newAttack = args.Attack;
    if (attack == newAttack)
      return true;
    if (args.Weapon == args.Target)
      return false;
    bool disarm = newAttack is DisarmAttackEvent;
    return this._blocker.CanAttack(user, new EntityUid?(this.GetEntity(args.Target)), new Entity<MeleeWeaponComponent>?(weapon), disarm);
  }

  public float GetUserLightAttackRange(
    EntityUid user,
    EntityUid? target,
    MeleeWeaponComponent melee)
  {
    RMCMeleeUserGetRangeEvent args = new RMCMeleeUserGetRangeEvent(target, melee.Range);
    this.RaiseLocalEvent<RMCMeleeUserGetRangeEvent>(user, ref args);
    return args.Range;
  }
}
