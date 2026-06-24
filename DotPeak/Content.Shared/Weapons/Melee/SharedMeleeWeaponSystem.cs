// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.SharedMeleeWeaponSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Tackle;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Melee;

public abstract class SharedMeleeWeaponSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  protected IMapManager MapManager;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  protected ISharedAdminLogManager AdminLogger;
  [Dependency]
  protected ActionBlockerSystem Blocker;
  [Dependency]
  protected DamageableSystem Damageable;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private MeleeSoundSystem _meleeSound;
  [Dependency]
  protected MobStateSystem MobState;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  protected SharedCombatModeSystem CombatMode;
  [Dependency]
  protected SharedInteractionSystem Interaction;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected SharedPopupSystem PopupSystem;
  [Dependency]
  protected SharedTransformSystem TransformSystem;
  [Dependency]
  private SharedStaminaSystem _stamina;
  [Dependency]
  private IConfigurationManager _configuration;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  private const int AttackMask = 31 /*0x1F*/;
  private static readonly EntProtoId DisarmEffect = (EntProtoId) "RMCWeaponArcDisarm";
  public int MaxTargets = 5;
  public const float GracePeriod = 0.05f;

  public override void Initialize()
  {
    base.Initialize();
    this._configuration.OnValueChanged<int>(RMCCVars.CMMaxHeavyAttackTargets, (Action<int>) (v => this.MaxTargets = v), true);
    this.SubscribeLocalEvent<MeleeWeaponComponent, HandSelectedEvent>(new ComponentEventHandler<MeleeWeaponComponent, HandSelectedEvent>(this.OnMeleeSelected));
    this.SubscribeLocalEvent<MeleeWeaponComponent, ShotAttemptedEvent>(new ComponentEventRefHandler<MeleeWeaponComponent, ShotAttemptedEvent>(this.OnMeleeShotAttempted));
    this.SubscribeLocalEvent<MeleeWeaponComponent, GunShotEvent>(new ComponentEventRefHandler<MeleeWeaponComponent, GunShotEvent>(this.OnMeleeShot));
    this.SubscribeLocalEvent<BonusMeleeDamageComponent, GetMeleeDamageEvent>(new ComponentEventRefHandler<BonusMeleeDamageComponent, GetMeleeDamageEvent>(this.OnGetBonusMeleeDamage));
    this.SubscribeLocalEvent<BonusMeleeDamageComponent, GetHeavyDamageModifierEvent>(new ComponentEventRefHandler<BonusMeleeDamageComponent, GetHeavyDamageModifierEvent>(this.OnGetBonusHeavyDamageModifier));
    this.SubscribeLocalEvent<BonusMeleeAttackRateComponent, GetMeleeAttackRateEvent>(new ComponentEventRefHandler<BonusMeleeAttackRateComponent, GetMeleeAttackRateEvent>(this.OnGetBonusMeleeAttackRate));
    this.SubscribeLocalEvent<ItemToggleMeleeWeaponComponent, ItemToggledEvent>(new ComponentEventHandler<ItemToggleMeleeWeaponComponent, ItemToggledEvent>(this.OnItemToggle));
    this.SubscribeAllEvent<HeavyAttackEvent>(new EntitySessionEventHandler<HeavyAttackEvent>(this.OnHeavyAttack));
    this.SubscribeAllEvent<LightAttackEvent>(new EntitySessionEventHandler<LightAttackEvent>(this.OnLightAttack));
    this.SubscribeAllEvent<DisarmAttackEvent>(new EntitySessionEventHandler<DisarmAttackEvent>(this.OnDisarmAttack));
    this.SubscribeAllEvent<StopAttackEvent>(new EntitySessionEventHandler<StopAttackEvent>(this.OnStopAttack));
  }

  private void OnMeleeShotAttempted(
    EntityUid uid,
    MeleeWeaponComponent comp,
    ref ShotAttemptedEvent args)
  {
    GunComponent comp1;
    if (!this.TryComp<GunComponent>(uid, out comp1) || !comp1.MeleeCooldownOnShoot || !(comp.NextAttack > this.Timing.CurTime))
      return;
    args.Cancel();
  }

  private void OnMeleeShot(EntityUid uid, MeleeWeaponComponent component, ref GunShotEvent args)
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>(uid, out comp) || !comp.MeleeCooldownOnShoot || !(comp.NextFire > component.NextAttack))
      return;
    component.NextAttack = comp.NextFire;
    this.DirtyField<MeleeWeaponComponent>(uid, component, "NextAttack");
  }

  private void OnMeleeSelected(
    EntityUid uid,
    MeleeWeaponComponent component,
    HandSelectedEvent args)
  {
    float attackRate = this.GetAttackRate(uid, args.User, component);
    if (attackRate.Equals(0.0f) || !component.ResetOnHandSelected || this.Paused(uid))
      return;
    TimeSpan timeSpan = this.Timing.CurTime + TimeSpan.FromSeconds(1.0 / (double) attackRate);
    if (timeSpan < component.NextAttack)
      return;
    component.NextAttack = timeSpan;
    this.DirtyField<MeleeWeaponComponent>(uid, component, "NextAttack");
  }

  private void OnGetBonusMeleeDamage(
    EntityUid uid,
    BonusMeleeDamageComponent component,
    ref GetMeleeDamageEvent args)
  {
    if (component.BonusDamage != null)
      args.Damage += component.BonusDamage;
    if (component.DamageModifierSet == null)
      return;
    args.Modifiers.Add(component.DamageModifierSet);
  }

  private void OnGetBonusHeavyDamageModifier(
    EntityUid uid,
    BonusMeleeDamageComponent component,
    ref GetHeavyDamageModifierEvent args)
  {
    args.DamageModifier += component.HeavyDamageFlatModifier;
    args.Multipliers *= component.HeavyDamageMultiplier;
  }

  private void OnGetBonusMeleeAttackRate(
    EntityUid uid,
    BonusMeleeAttackRateComponent component,
    ref GetMeleeAttackRateEvent args)
  {
    args.Rate += component.FlatModifier;
    args.Multipliers *= component.Multiplier;
  }

  private void OnStopAttack(StopAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!attachedEntity.HasValue || !this.TryGetWeapon(attachedEntity.Value, out weaponUid, out melee) || weaponUid != this.GetEntity(msg.Weapon) || !melee.Attacking)
      return;
    melee.Attacking = false;
    this.DirtyField<MeleeWeaponComponent>(weaponUid, melee, "Attacking");
  }

  private void OnLightAttack(LightAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!this.TryGetWeapon(valueOrDefault, out weaponUid, out melee) || weaponUid != this.GetEntity(msg.Weapon))
      return;
    this.AttemptAttack(valueOrDefault, weaponUid, melee, (AttackEvent) msg, args.SenderSession);
  }

  private void OnHeavyAttack(HeavyAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!this.TryGetWeapon(valueOrDefault, out weaponUid, out melee) || weaponUid != this.GetEntity(msg.Weapon))
      return;
    this.AttemptAttack(valueOrDefault, weaponUid, melee, (AttackEvent) msg, args.SenderSession);
  }

  private void OnDisarmAttack(DisarmAttackEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!this.TryGetWeapon(valueOrDefault, out weaponUid, out melee))
      return;
    this.AttemptAttack(valueOrDefault, weaponUid, melee, (AttackEvent) msg, args.SenderSession);
  }

  public DamageSpecifier GetDamage(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
  {
    if (!this.Resolve<MeleeWeaponComponent>(uid, ref component, false))
      return new DamageSpecifier();
    GetMeleeDamageEvent args = new GetMeleeDamageEvent(uid, new DamageSpecifier(component.Damage * this.Damageable.UniversalMeleeDamageModifier), new List<DamageModifierSet>(), user, component.ResistanceBypass);
    this.RaiseLocalEvent<GetMeleeDamageEvent>(uid, ref args, true);
    return DamageSpecifier.ApplyModifierSets(args.Damage, (IEnumerable<DamageModifierSet>) args.Modifiers);
  }

  public float GetAttackRate(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
  {
    if (!this.Resolve<MeleeWeaponComponent>(uid, ref component))
      return 0.0f;
    GetMeleeAttackRateEvent args = new GetMeleeAttackRateEvent(uid, component.AttackRate, 1f, user);
    this.RaiseLocalEvent<GetMeleeAttackRateEvent>(uid, ref args);
    return args.Rate * args.Multipliers;
  }

  public FixedPoint2 GetHeavyDamageModifier(
    EntityUid uid,
    EntityUid user,
    MeleeWeaponComponent? component = null)
  {
    if (!this.Resolve<MeleeWeaponComponent>(uid, ref component))
      return FixedPoint2.Zero;
    GetHeavyDamageModifierEvent args = new GetHeavyDamageModifierEvent(uid, component.ClickDamageModifier, 1f, user);
    this.RaiseLocalEvent<GetHeavyDamageModifierEvent>(uid, ref args);
    return args.DamageModifier * args.Multipliers;
  }

  public bool GetResistanceBypass(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
  {
    if (!this.Resolve<MeleeWeaponComponent>(uid, ref component))
      return false;
    GetMeleeDamageEvent args = new GetMeleeDamageEvent(uid, new DamageSpecifier(component.Damage * this.Damageable.UniversalMeleeDamageModifier), new List<DamageModifierSet>(), user, component.ResistanceBypass);
    this.RaiseLocalEvent<GetMeleeDamageEvent>(uid, ref args);
    return args.ResistanceBypass;
  }

  public bool TryGetWeapon(
    EntityUid entity,
    out EntityUid weaponUid,
    [NotNullWhen(true)] out MeleeWeaponComponent? melee)
  {
    weaponUid = new EntityUid();
    melee = (MeleeWeaponComponent) null;
    GetMeleeWeaponEvent args = new GetMeleeWeaponEvent();
    this.RaiseLocalEvent<GetMeleeWeaponEvent>(entity, args);
    if (args.Handled)
    {
      if (!this.TryComp<MeleeWeaponComponent>(args.Weapon, out melee))
        return false;
      weaponUid = args.Weapon.Value;
      return true;
    }
    EntityUid? uid;
    if (this._hands.TryGetActiveItem((Entity<HandsComponent>) entity, out uid))
    {
      if (this.TryComp<MeleeWeaponComponent>(uid, out melee) && !melee.MustBeEquippedToUse)
      {
        weaponUid = uid.Value;
        return true;
      }
      if (!this.HasComp<VirtualItemComponent>(uid))
        return false;
    }
    EntityUid? entityUid;
    MeleeWeaponComponent comp;
    if (this._inventory.TryGetSlotEntity(entity, "gloves", out entityUid) && this.TryComp<MeleeWeaponComponent>(entityUid, out comp))
    {
      weaponUid = entityUid.Value;
      melee = comp;
      return true;
    }
    if (!this.TryComp<MeleeWeaponComponent>(entity, out melee))
      return false;
    weaponUid = entity;
    return true;
  }

  public void AttemptLightAttackMiss(
    EntityUid user,
    EntityUid weaponUid,
    MeleeWeaponComponent weapon,
    EntityCoordinates coordinates)
  {
    this.AttemptAttack(user, weaponUid, weapon, (AttackEvent) new LightAttackEvent(new NetEntity?(), this.GetNetEntity(weaponUid), this.GetNetCoordinates(coordinates)), (ICommonSession) null);
  }

  public bool AttemptLightAttack(
    EntityUid user,
    EntityUid weaponUid,
    MeleeWeaponComponent weapon,
    EntityUid target)
  {
    TransformComponent comp;
    return this.TryComp(target, out comp) && this.AttemptAttack(user, weaponUid, weapon, (AttackEvent) new LightAttackEvent(new NetEntity?(this.GetNetEntity(target)), this.GetNetEntity(weaponUid), this.GetNetCoordinates(comp.Coordinates)), (ICommonSession) null);
  }

  public bool AttemptDisarmAttack(
    EntityUid user,
    EntityUid weaponUid,
    MeleeWeaponComponent weapon,
    EntityUid target)
  {
    TransformComponent comp;
    return this.TryComp(target, out comp) && this.AttemptAttack(user, weaponUid, weapon, (AttackEvent) new DisarmAttackEvent(new NetEntity?(this.GetNetEntity(target)), this.GetNetCoordinates(comp.Coordinates)), (ICommonSession) null);
  }

  private bool AttemptAttack(
    EntityUid user,
    EntityUid weaponUid,
    MeleeWeaponComponent weapon,
    AttackEvent attack,
    ICommonSession? session)
  {
    TimeSpan curTime = this.Timing.CurTime;
    if (weapon.NextAttack > curTime || !this.CombatMode.IsInCombatMode(new EntityUid?(user)))
      return false;
    EntityUid? entity = new EntityUid?();
    switch (attack)
    {
      case LightAttackEvent lightAttackEvent:
        if (lightAttackEvent.Target.HasValue && !this.TryGetEntity(lightAttackEvent.Target, out entity) || !this.Blocker.CanAttack(user, entity, new Entity<MeleeWeaponComponent>?((Entity<MeleeWeaponComponent>) (weaponUid, weapon))))
          return false;
        EntityUid entityUid = weaponUid;
        EntityUid? nullable1 = entity;
        if ((nullable1.HasValue ? (entityUid == nullable1.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          return false;
        break;
      case DisarmAttackEvent disarmAttackEvent:
        if (disarmAttackEvent.Target.HasValue && !this.TryGetEntity(disarmAttackEvent.Target, out entity) || !this.Blocker.CanAttack(user, entity, new Entity<MeleeWeaponComponent>?((Entity<MeleeWeaponComponent>) (weaponUid, weapon)), true))
          return false;
        break;
      default:
        ActionBlockerSystem blocker = this.Blocker;
        EntityUid uid = user;
        Entity<MeleeWeaponComponent>? nullable2 = new Entity<MeleeWeaponComponent>?((Entity<MeleeWeaponComponent>) (weaponUid, weapon));
        EntityUid? target = new EntityUid?();
        Entity<MeleeWeaponComponent>? weapon1 = nullable2;
        if (!blocker.CanAttack(uid, target, weapon1))
          return false;
        break;
    }
    if (entity.HasValue)
    {
      AttackEvent newAttack;
      if (!this._rmcMelee.AttemptOverrideAttack(entity.Value, (Entity<MeleeWeaponComponent>) (weaponUid, weapon), user, attack, out newAttack))
        return false;
      attack = newAttack;
    }
    TimeSpan timeSpan = TimeSpan.FromSeconds(1.0 / (double) this.GetAttackRate(weaponUid, user, weapon));
    int num = 0;
    if (weapon.NextAttack < curTime)
      weapon.NextAttack = curTime;
    while (weapon.NextAttack <= curTime)
    {
      weapon.NextAttack += timeSpan;
      ++num;
    }
    this.DirtyField<MeleeWeaponComponent>(weaponUid, weapon, "NextAttack");
    AttemptMeleeEvent args1 = new AttemptMeleeEvent(user);
    this.RaiseLocalEvent<AttemptMeleeEvent>(weaponUid, ref args1);
    if (args1.Cancelled)
    {
      if (args1.Message != null)
        this.PopupSystem.PopupClient(args1.Message, weaponUid, new EntityUid?(user));
      return false;
    }
    for (int index = 0; index < num; ++index)
    {
      float length = weapon.Range;
      string animation;
      switch (attack)
      {
        case LightAttackEvent ev1:
          this.DoLightAttack(user, ev1, weaponUid, weapon, session);
          animation = (string) weapon.Animation;
          length = this._rmcMelee.GetUserLightAttackRange(user, entity, weapon);
          break;
        case DisarmAttackEvent ev2:
          if (!this.DoDisarm(user, ev2, weaponUid, weapon, session))
            weapon.NextAttack = curTime + TimeSpan.FromSeconds(0.6);
          animation = (string) SharedMeleeWeaponSystem.DisarmEffect;
          break;
        case HeavyAttackEvent ev3:
          if (!this.DoHeavyAttack(user, ev3, weaponUid, weapon, session))
            return false;
          animation = (string) weapon.WideAnimation;
          break;
        default:
          throw new NotImplementedException();
      }
      this.DoLungeAnimation(user, weaponUid, weapon.Angle, this.TransformSystem.ToMapCoordinates(this.GetCoordinates(attack.Coordinates)), length, animation);
    }
    MeleeAttackEvent args2 = new MeleeAttackEvent(weaponUid);
    this.RaiseLocalEvent<MeleeAttackEvent>(user, ref args2);
    weapon.Attacking = true;
    this.DirtyField<MeleeWeaponComponent>(weaponUid, weapon, "Attacking");
    return true;
  }

  protected abstract bool InRange(
    EntityUid user,
    EntityUid target,
    float range,
    ICommonSession? session);

  protected virtual void DoLightAttack(
    EntityUid user,
    LightAttackEvent ev,
    EntityUid meleeUid,
    MeleeWeaponComponent component,
    ICommonSession? session)
  {
    DamageSpecifier baseDamage = this.GetDamage(meleeUid, user, component) * this.GetHeavyDamageModifier(meleeUid, user, component);
    EntityUid? entity1 = this.GetEntity(ev.Target);
    bool resistanceBypass = this.GetResistanceBypass(meleeUid, user, component);
    TransformComponent comp;
    if (this.Deleted(entity1) || !this.HasComp<DamageableComponent>(entity1) || !this.TryComp(entity1, out comp) || !this.InRange(user, entity1.Value, this._rmcMelee.GetUserLightAttackRange(user, entity1, component), session))
    {
      if (meleeUid == user)
      {
        ISharedAdminLogManager adminLogger = this.AdminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(52, 1);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" melee attacked (light) using their hands and missed");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.MeleeHit, LogImpact.Low, ref local);
      }
      else
      {
        ISharedAdminLogManager adminLogger = this.AdminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(41, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" melee attacked (light) using ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) meleeUid), "tool", "ToPrettyString(meleeUid)");
        logStringHandler.AppendLiteral(" and missed");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.MeleeHit, LogImpact.Low, ref local);
      }
      MeleeHitEvent args = new MeleeHitEvent(new List<EntityUid>(), user, meleeUid, baseDamage, new Vector2?());
      this.RaiseLocalEvent<MeleeHitEvent>(meleeUid, args);
      this._meleeSound.PlaySwingSound(user, meleeUid, component);
    }
    else
    {
      MeleeHitEvent args1 = new MeleeHitEvent(new List<EntityUid>()
      {
        entity1.Value
      }, user, meleeUid, baseDamage, new Vector2?());
      this.RaiseLocalEvent<MeleeHitEvent>(meleeUid, args1);
      if (args1.Handled)
        return;
      List<EntityUid> targets = new List<EntityUid>(1)
      {
        entity1.Value
      };
      EntityUid entity2 = this.GetEntity(ev.Weapon);
      this.Interaction.DoContactInteraction(user, new EntityUid?(entity2));
      this.Interaction.DoContactInteraction(user, entity1);
      AttackedEvent args2 = new AttackedEvent(meleeUid, user, comp.Coordinates);
      this.RaiseLocalEvent<AttackedEvent>(entity1.Value, args2);
      DamageSpecifier damageSpecifier = DamageSpecifier.ApplyModifierSets(baseDamage + args1.BonusDamage + args2.BonusDamage, (IEnumerable<DamageModifierSet>) args1.ModifiersList);
      DamageableSystem damageable = this.Damageable;
      EntityUid? uid1 = entity1;
      DamageSpecifier damage = damageSpecifier;
      EntityUid? nullable = new EntityUid?(user);
      int num1 = resistanceBypass ? 1 : 0;
      EntityUid? origin = nullable;
      EntityUid? tool = new EntityUid?(meleeUid);
      DamageSpecifier modifiedDamage = damageable.TryChangeDamage(uid1, damage, num1 != 0, origin: origin, tool: tool);
      if (modifiedDamage != null && !modifiedDamage.Empty)
      {
        FixedPoint2 fixedPoint2;
        if (modifiedDamage.DamageDict.TryGetValue("Blunt", out fixedPoint2))
        {
          SharedStaminaSystem stamina = this._stamina;
          EntityUid uid2 = entity1.Value;
          double num2 = (double) (fixedPoint2 * component.BluntStaminaDamageFactor).Float();
          EntityUid? source = new EntityUid?(user);
          EntityUid? with;
          if (!(meleeUid == user))
          {
            with = new EntityUid?(meleeUid);
          }
          else
          {
            nullable = new EntityUid?();
            with = nullable;
          }
          stamina.TakeStaminaDamage(uid2, (float) num2, source: source, with: with, visual: false);
        }
        if (meleeUid == user)
        {
          ISharedAdminLogManager adminLogger = this.AdminLogger;
          LogStringHandler logStringHandler = new LogStringHandler(60, 3);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" melee attacked (light) ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity1.Value), "subject", "ToPrettyString(target.Value)");
          logStringHandler.AppendLiteral(" using their hands and dealt ");
          logStringHandler.AppendFormatted<FixedPoint2>(modifiedDamage.GetTotal(), "damage", "damageResult.GetTotal()");
          logStringHandler.AppendLiteral(" damage");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.MeleeHit, LogImpact.Medium, ref local);
        }
        else
        {
          ISharedAdminLogManager adminLogger = this.AdminLogger;
          LogStringHandler logStringHandler = new LogStringHandler(49, 4);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" melee attacked (light) ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity1.Value), "subject", "ToPrettyString(target.Value)");
          logStringHandler.AppendLiteral(" using ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) meleeUid), "tool", "ToPrettyString(meleeUid)");
          logStringHandler.AppendLiteral(" and dealt ");
          logStringHandler.AppendFormatted<FixedPoint2>(modifiedDamage.GetTotal(), "damage", "damageResult.GetTotal()");
          logStringHandler.AppendLiteral(" damage");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.MeleeHit, LogImpact.Medium, ref local);
        }
      }
      if (modifiedDamage != null)
        this._meleeSound.PlayHitSound(entity1.Value, new EntityUid?(user), SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, this._protoManager), args1.HitSoundOverride, component);
      FixedPoint2? total = modifiedDamage?.GetTotal();
      FixedPoint2 zero = FixedPoint2.Zero;
      if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) == 0)
        return;
      this.DoDamageEffect(targets, new EntityUid?(user), comp);
    }
  }

  protected abstract void DoDamageEffect(
    List<EntityUid> targets,
    EntityUid? user,
    TransformComponent targetXform);

  private bool DoHeavyAttack(
    EntityUid user,
    HeavyAttackEvent ev,
    EntityUid meleeUid,
    MeleeWeaponComponent component,
    ICommonSession? session)
  {
    TransformComponent comp;
    if (!this.TryComp(user, out comp))
      return false;
    MapCoordinates mapCoordinates = this.TransformSystem.ToMapCoordinates(this.GetCoordinates(ev.Coordinates));
    if (mapCoordinates.MapId != comp.MapID)
      return false;
    Vector2 worldPosition = this.TransformSystem.GetWorldPosition(comp);
    Vector2 vector2 = mapCoordinates.Position - worldPosition;
    float range = Math.Min(component.Range, vector2.Length());
    DamageSpecifier damage1 = this.GetDamage(meleeUid, user, component);
    List<EntityUid> entityList = this.GetEntityList(ev.Entities);
    if (entityList.Count == 0)
    {
      if (meleeUid == user)
      {
        ISharedAdminLogManager adminLogger = this.AdminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(52, 1);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" melee attacked (heavy) using their hands and missed");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.MeleeHit, LogImpact.Low, ref local);
      }
      else
      {
        ISharedAdminLogManager adminLogger = this.AdminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(41, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" melee attacked (heavy) using ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) meleeUid), "tool", "ToPrettyString(meleeUid)");
        logStringHandler.AppendLiteral(" and missed");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.MeleeHit, LogImpact.Low, ref local);
      }
      MeleeHitEvent args = new MeleeHitEvent(new List<EntityUid>(), user, meleeUid, damage1, new Vector2?(vector2));
      this.RaiseLocalEvent<MeleeHitEvent>(meleeUid, args);
      this._meleeSound.PlaySwingSound(user, meleeUid, component);
      return true;
    }
    if (entityList.Count > this.MaxTargets)
      entityList.RemoveRange(this.MaxTargets, entityList.Count - this.MaxTargets);
    for (int index = entityList.Count - 1; index >= 0; --index)
    {
      if (!this.ArcRaySuccessful(entityList[index], worldPosition, DirectionExtensions.ToWorldAngle(vector2), component.Angle, range, comp.MapID, user, session))
        entityList.RemoveAt(index);
    }
    List<EntityUid> entityUidList = new List<EntityUid>();
    Robust.Shared.GameObjects.EntityQuery<DamageableComponent> entityQuery = this.GetEntityQuery<DamageableComponent>();
    foreach (EntityUid uid in entityList)
    {
      if (!(uid == user) && entityQuery.HasComponent(uid))
        entityUidList.Add(uid);
    }
    MeleeHitEvent args1 = new MeleeHitEvent(entityUidList, user, meleeUid, damage1, new Vector2?(vector2));
    this.RaiseLocalEvent<MeleeHitEvent>(meleeUid, args1);
    if (args1.Handled)
      return true;
    EntityUid entity = this.GetEntity(ev.Weapon);
    this.Interaction.DoContactInteraction(user, new EntityUid?(entity));
    foreach (EntityUid entityUid in entityUidList)
      this.Interaction.DoContactInteraction(user, new EntityUid?(entityUid));
    DamageSpecifier modifiedDamage = new DamageSpecifier();
    for (int index = entityUidList.Count - 1; index >= 0; --index)
    {
      EntityUid uid = entityUidList[index];
      if (!this.Blocker.CanAttack(user, new EntityUid?(uid), new Entity<MeleeWeaponComponent>?((Entity<MeleeWeaponComponent>) (entity, component))))
      {
        entityUidList.RemoveAt(index);
      }
      else
      {
        AttackedEvent args2 = new AttackedEvent(meleeUid, user, this.GetCoordinates(ev.Coordinates));
        this.RaiseLocalEvent<AttackedEvent>(uid, args2);
        DamageSpecifier damage2 = DamageSpecifier.ApplyModifierSets(damage1 + args1.BonusDamage + args2.BonusDamage, (IEnumerable<DamageModifierSet>) args1.ModifiersList);
        DamageSpecifier damageSpecifier = this.Damageable.TryChangeDamage(new EntityUid?(uid), damage2, origin: new EntityUid?(user), tool: new EntityUid?(meleeUid));
        if (damageSpecifier != null && damageSpecifier.GetTotal() > FixedPoint2.Zero)
        {
          FixedPoint2 fixedPoint2;
          if (damageSpecifier.DamageDict.TryGetValue("Blunt", out fixedPoint2))
            this._stamina.TakeStaminaDamage(uid, (fixedPoint2 * component.BluntStaminaDamageFactor).Float(), source: new EntityUid?(user), with: meleeUid == user ? new EntityUid?() : new EntityUid?(meleeUid), visual: false);
          modifiedDamage += damageSpecifier;
          if (meleeUid == user)
          {
            ISharedAdminLogManager adminLogger = this.AdminLogger;
            LogStringHandler logStringHandler = new LogStringHandler(60, 3);
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
            logStringHandler.AppendLiteral(" melee attacked (heavy) ");
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "subject", "ToPrettyString(entity)");
            logStringHandler.AppendLiteral(" using their hands and dealt ");
            logStringHandler.AppendFormatted<FixedPoint2>(damageSpecifier.GetTotal(), "damage", "damageResult.GetTotal()");
            logStringHandler.AppendLiteral(" damage");
            ref LogStringHandler local = ref logStringHandler;
            adminLogger.Add(LogType.MeleeHit, LogImpact.Medium, ref local);
          }
          else
          {
            ISharedAdminLogManager adminLogger = this.AdminLogger;
            LogStringHandler logStringHandler = new LogStringHandler(49, 4);
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
            logStringHandler.AppendLiteral(" melee attacked (heavy) ");
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "subject", "ToPrettyString(entity)");
            logStringHandler.AppendLiteral(" using ");
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) meleeUid), "tool", "ToPrettyString(meleeUid)");
            logStringHandler.AppendLiteral(" and dealt ");
            logStringHandler.AppendFormatted<FixedPoint2>(damageSpecifier.GetTotal(), "damage", "damageResult.GetTotal()");
            logStringHandler.AppendLiteral(" damage");
            ref LogStringHandler local = ref logStringHandler;
            adminLogger.Add(LogType.MeleeHit, LogImpact.Medium, ref local);
          }
        }
      }
    }
    if (entityList.Count != 0)
      this._meleeSound.PlayHitSound(entityList.First<EntityUid>(), new EntityUid?(user), SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, this._protoManager), args1.HitSoundOverride, component);
    if (modifiedDamage.GetTotal() > FixedPoint2.Zero)
      this.DoDamageEffect(entityUidList, new EntityUid?(user), this.Transform(entityUidList[0]));
    return true;
  }

  public HashSet<EntityUid> ArcRayCast(
    Vector2 position,
    Angle angle,
    Angle arcWidth,
    float range,
    MapId mapId,
    EntityUid ignore)
  {
    Angle angle1 = arcWidth;
    int num1 = 1 + 35 * (int) Math.Ceiling(Angle.op_Implicit(angle1) / (2.0 * Math.PI));
    double num2 = Angle.op_Implicit(angle1) / (double) num1;
    Angle angle2 = Angle.op_Subtraction(angle, Angle.op_Implicit(Angle.op_Implicit(angle1) / 2.0));
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
    for (int index = 0; index < num1; ++index)
    {
      Angle angle3;
      // ISSUE: explicit constructor call
      ((Angle) ref angle3).\u002Ector(Angle.op_Implicit(Angle.op_Addition(angle2, Angle.op_Implicit(num2 * (double) index))));
      List<RayCastResults> res = this._physics.IntersectRay(mapId, new CollisionRay(position, ((Angle) ref angle3).ToWorldVec(), 31 /*0x1F*/), range, new EntityUid?(ignore), false).ToList<RayCastResults>();
      if (res.Count != 0)
      {
        foreach (RayCastResults rayCastResults in res.Where<RayCastResults>((Func<RayCastResults, bool>) (x => x.Distance.Equals(res[0].Distance))))
        {
          if (this.Interaction.InRangeUnobstructed((Entity<TransformComponent>) ignore, (Entity<TransformComponent>) rayCastResults.HitEntity, range + 0.1f, overlapCheck: false))
            entityUidSet.Add(rayCastResults.HitEntity);
        }
      }
    }
    return entityUidSet;
  }

  protected virtual bool ArcRaySuccessful(
    EntityUid targetUid,
    Vector2 position,
    Angle angle,
    Angle arcWidth,
    float range,
    MapId mapId,
    EntityUid ignore,
    ICommonSession? session)
  {
    return true;
  }

  public static string? GetHighestDamageSound(
    DamageSpecifier modifiedDamage,
    IPrototypeManager protoManager)
  {
    if (modifiedDamage.GetTotal() <= FixedPoint2.Zero)
      return (string) null;
    Dictionary<string, FixedPoint2> damagePerGroup = modifiedDamage.GetDamagePerGroup(protoManager);
    if (damagePerGroup.Count == 1)
      return damagePerGroup.Keys.First<string>();
    FixedPoint2 zero = FixedPoint2.Zero;
    string highestDamageSound = (string) null;
    foreach ((string key, FixedPoint2 fixedPoint2) in modifiedDamage.DamageDict)
    {
      if (!(fixedPoint2 <= zero))
        highestDamageSound = key;
    }
    return highestDamageSound;
  }

  private float CalculateDisarmChance(
    EntityUid disarmer,
    EntityUid disarmed,
    EntityUid? inTargetHand,
    CombatModeComponent disarmerComp)
  {
    if (this.HasComp<DisarmProneComponent>(disarmer))
      return 1f;
    if (this.HasComp<DisarmProneComponent>(disarmed))
      return 0.0f;
    float disarmFailChance = disarmerComp.BaseDisarmFailChance;
    DisarmMalusComponent comp;
    if (inTargetHand.HasValue && this.TryComp<DisarmMalusComponent>(inTargetHand, out comp))
      disarmFailChance += comp.Malus;
    return Math.Clamp(disarmFailChance, 0.0f, 1f);
  }

  private bool DoDisarm(
    EntityUid user,
    DisarmAttackEvent ev,
    EntityUid meleeUid,
    MeleeWeaponComponent component,
    ICommonSession? session)
  {
    this._meleeSound.PlaySwingSound(user, meleeUid, component);
    EntityUid? entity = this.GetEntity(ev.Target);
    if (!this.Deleted(entity))
    {
      EntityUid entityUid1 = user;
      EntityUid? nullable1 = entity;
      CombatModeComponent comp1;
      if ((nullable1.HasValue ? (entityUid1 == nullable1.GetValueOrDefault() ? 1 : 0) : 0) == 0 && !this.MobState.IsIncapacitated(entity.Value) && this.TryComp<CombatModeComponent>(user, out comp1) && comp1.CanDisarm.GetValueOrDefault())
      {
        StatusEffectsComponent comp2;
        if (!this.TryComp<HandsComponent>(entity, out HandsComponent _) && (!this.TryComp<StatusEffectsComponent>(entity, out comp2) || !comp2.AllowedEffects.Contains("KnockedDown")))
        {
          if (this.HasComp<MobStateComponent>(entity.Value))
            this.PopupSystem.PopupClient(this.Loc.GetString("disarm-action-disarmable", ("targetName", (object) entity.Value)), new EntityUid?(entity.Value));
          return false;
        }
        if (!this.InRange(user, entity.Value, component.Range, session))
          return false;
        CMDisarmEvent args1 = new CMDisarmEvent(user);
        this.RaiseLocalEvent<CMDisarmEvent>(entity.Value, ref args1);
        if (args1.Handled)
          return true;
        EntityUid? nullable2 = new EntityUid?();
        EntityUid? nullable3;
        if (this._hands.TryGetActiveItem((Entity<HandsComponent>) entity.Value, out nullable3))
          nullable2 = new EntityUid?(nullable3.Value);
        DisarmAttemptEvent args2 = new DisarmAttemptEvent(entity.Value, user, nullable2);
        if (nullable2.HasValue)
          this.RaiseLocalEvent<DisarmAttemptEvent>(nullable2.Value, ref args2);
        this.RaiseLocalEvent<DisarmAttemptEvent>(entity.Value, ref args2);
        if (args2.Cancelled)
          return false;
        float disarmChance = this.CalculateDisarmChance(user, entity.Value, nullable2, comp1);
        if (this._netMan.IsClient)
        {
          this._meleeSound.PlaySwingSound(user, meleeUid, component);
          return true;
        }
        if (this._random.Prob(disarmChance))
          return false;
        DisarmedEvent args3 = new DisarmedEvent(entity.Value, user, 1f - disarmChance);
        this.RaiseLocalEvent<DisarmedEvent>(entity.Value, ref args3);
        if (!args3.Handled)
          return false;
        this.Interaction.DoContactInteraction(user, entity);
        ISharedAdminLogManager adminLogger1 = this.AdminLogger;
        LogStringHandler logStringHandler1 = new LogStringHandler(16 /*0x10*/, 2);
        logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler1.AppendLiteral(" used disarm on ");
        logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "target", "ToPrettyString(target)");
        ref LogStringHandler local1 = ref logStringHandler1;
        adminLogger1.Add(LogType.DisarmedAction, ref local1);
        ISharedAdminLogManager adminLogger2 = this.AdminLogger;
        LogStringHandler logStringHandler2 = new LogStringHandler(16 /*0x10*/, 2);
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler2.AppendLiteral(" used disarm on ");
        logStringHandler2.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "target", "ToPrettyString(target)");
        ref LogStringHandler local2 = ref logStringHandler2;
        adminLogger2.Add(LogType.DisarmedAction, ref local2);
        this._audio.PlayPvs(comp1.DisarmSuccessSound, entity.Value, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.025f)).WithVolume(5f)));
        EntityUid uid1 = entity.Value;
        EntityManager entityManager1 = this.EntityManager;
        nullable1 = new EntityUid?();
        EntityUid? viewer1 = nullable1;
        EntityUid uid2 = Identity.Entity(uid1, (IEntityManager) entityManager1, viewer1);
        EntityUid uid3 = user;
        EntityManager entityManager2 = this.EntityManager;
        nullable1 = new EntityUid?();
        EntityUid? viewer2 = nullable1;
        EntityUid entityUid2 = Identity.Entity(uid3, (IEntityManager) entityManager2, viewer2);
        string message1 = this.Loc.GetString(args3.PopupPrefix + "popup-message-other-clients", ("performerName", (object) entityUid2), ("targetName", (object) uid2));
        string message2 = this.Loc.GetString(args3.PopupPrefix + "popup-message-cursor", ("targetName", (object) uid2));
        Filter filter = Filter.PvsExcept(user, entityManager: (IEntityManager) this.EntityManager);
        this.PopupSystem.PopupEntity(message1, user, filter, true);
        this.PopupSystem.PopupEntity(message2, entity.Value, user);
        if (args3.IsStunned)
        {
          this.PopupSystem.PopupEntity(this.Loc.GetString("stunned-component-disarm-success-others", ("source", (object) entityUid2), ("target", (object) uid2)), uid2, Filter.PvsExcept(user), true, PopupType.LargeCaution);
          this.PopupSystem.PopupCursor(this.Loc.GetString("stunned-component-disarm-success", ("target", (object) uid2)), user, PopupType.Large);
          ISharedAdminLogManager adminLogger3 = this.AdminLogger;
          LogStringHandler logStringHandler3 = new LogStringHandler(14, 2);
          logStringHandler3.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
          logStringHandler3.AppendLiteral(" knocked down ");
          logStringHandler3.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "target", "ToPrettyString(target)");
          ref LogStringHandler local3 = ref logStringHandler3;
          adminLogger3.Add(LogType.DisarmedKnockdown, LogImpact.Medium, ref local3);
        }
        return true;
      }
    }
    return false;
  }

  private void DoLungeAnimation(
    EntityUid user,
    EntityUid weapon,
    Angle angle,
    MapCoordinates coordinates,
    float length,
    string? animation)
  {
    TransformComponent comp;
    if (!this.TryComp(user, out comp))
      return;
    Matrix3x2 invWorldMatrix = this.TransformSystem.GetInvWorldMatrix(comp);
    Vector2 localPos = Vector2.Transform(coordinates.Position, invWorldMatrix);
    if ((double) localPos.LengthSquared() <= 0.0)
      return;
    Angle localRotation = comp.LocalRotation;
    localPos = ((Angle) ref localRotation).RotateVec(ref localPos);
    float num = length - 0.2f;
    if ((double) localPos.Length() > (double) num)
      localPos = Vector2Helpers.Normalized(localPos) * num;
    this.DoLunge(user, weapon, angle, localPos, animation);
  }

  public abstract void DoLunge(
    EntityUid user,
    EntityUid weapon,
    Angle angle,
    Vector2 localPos,
    string? animation,
    bool predicted = true);

  private void OnItemToggle(
    EntityUid uid,
    ItemToggleMeleeWeaponComponent itemToggleMelee,
    ItemToggledEvent args)
  {
    MeleeWeaponComponent comp;
    if (!this.TryComp<MeleeWeaponComponent>(uid, out comp))
      return;
    if (args.Activated)
    {
      if (itemToggleMelee.ActivatedDamage != null)
      {
        ItemToggleMeleeWeaponComponent meleeWeaponComponent = itemToggleMelee;
        if (meleeWeaponComponent.DeactivatedDamage == null)
          meleeWeaponComponent.DeactivatedDamage = comp.Damage;
        comp.Damage = itemToggleMelee.ActivatedDamage;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "Damage");
      }
      SoundSpecifier hitSound = comp.HitSound;
      if ((hitSound != null ? (!hitSound.Equals((object) itemToggleMelee.ActivatedSoundOnHit) ? 1 : 0) : 1) != 0)
      {
        comp.HitSound = itemToggleMelee.ActivatedSoundOnHit;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "HitSound");
      }
      if (itemToggleMelee.ActivatedSoundOnHitNoDamage != null)
      {
        ItemToggleMeleeWeaponComponent meleeWeaponComponent = itemToggleMelee;
        if (meleeWeaponComponent.DeactivatedSoundOnHitNoDamage == null)
          meleeWeaponComponent.DeactivatedSoundOnHitNoDamage = comp.NoDamageSound;
        comp.NoDamageSound = itemToggleMelee.ActivatedSoundOnHitNoDamage;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "NoDamageSound");
      }
      if (itemToggleMelee.ActivatedSoundOnSwing != null)
      {
        ItemToggleMeleeWeaponComponent meleeWeaponComponent = itemToggleMelee;
        if (meleeWeaponComponent.DeactivatedSoundOnSwing == null)
          meleeWeaponComponent.DeactivatedSoundOnSwing = comp.SwingSound;
        comp.SwingSound = itemToggleMelee.ActivatedSoundOnSwing;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "SwingSound");
      }
      if (!itemToggleMelee.DeactivatedSecret)
        return;
      comp.Hidden = false;
    }
    else
    {
      if (itemToggleMelee.DeactivatedDamage != null)
      {
        comp.Damage = itemToggleMelee.DeactivatedDamage;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "Damage");
      }
      comp.HitSound = itemToggleMelee.DeactivatedSoundOnHit;
      this.DirtyField<MeleeWeaponComponent>(uid, comp, "HitSound");
      if (itemToggleMelee.DeactivatedSoundOnHitNoDamage != null)
      {
        comp.NoDamageSound = itemToggleMelee.DeactivatedSoundOnHitNoDamage;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "NoDamageSound");
      }
      if (itemToggleMelee.DeactivatedSoundOnSwing != null)
      {
        comp.SwingSound = itemToggleMelee.DeactivatedSoundOnSwing;
        this.DirtyField<MeleeWeaponComponent>(uid, comp, "SwingSound");
      }
      if (!itemToggleMelee.DeactivatedSecret)
        return;
      comp.Hidden = true;
    }
  }
}
