// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.XenoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Medical.Scanner;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Tackle;
using Content.Shared._RMC14.Vendors;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared._RMC14.Xenonids.ScissorCut;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Actions;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.DragDrop;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Lathe;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Radio;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Stunnable;
using Content.Shared.Tools.Systems;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Melee.Events;
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

#nullable enable
namespace Content.Shared._RMC14.Xenonids;

public sealed class XenoSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private HiveLeaderSystem _hiveLeader;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MobThresholdSystem _mobThresholds;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedNightVisionSystem _nightVision;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private SharedRMCFlammableSystem _rmcFlammable;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private WeldableSystem _weldable;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private SharedXenoWeedsSystem _weeds;
  private static readonly ProtoId<DamageTypePrototype> HeatDamage = (ProtoId<DamageTypePrototype>) "Heat";
  private Robust.Shared.GameObjects.EntityQuery<AffectableByWeedsComponent> _affectableQuery;
  private Robust.Shared.GameObjects.EntityQuery<DamageableComponent> _damageableQuery;
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;
  private Robust.Shared.GameObjects.EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoFriendlyComponent> _xenoFriendlyQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoNestedComponent> _xenoNestedQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoPlasmaComponent> _xenoPlasmaQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoRecoveryPheromonesComponent> _xenoRecoveryQuery;
  private Robust.Shared.GameObjects.EntityQuery<VictimInfectedComponent> _victimInfectedQuery;
  private float _xenoDamageDealtMultiplier;
  private float _xenoDamageReceivedMultiplier;
  private float _xenoSpeedMultiplier;
  private TimeSpan _xenoSpawnMuteDuration;
  [Dependency]
  private RMCSizeStunSystem _size;
  public const float XENO_SLASH_DAMAGE_MULT = 1.5f;
  public const float XENO_DEBUFF_MULT = 1.25f;
  public const float XENO_ACID_DAMAGE_MULT = 2.625f;
  public const float XENO_PROJECTILE_DAMAGE_MULT = 2.625f;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this._affectableQuery = this.GetEntityQuery<AffectableByWeedsComponent>();
    this._damageableQuery = this.GetEntityQuery<DamageableComponent>();
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    this._mobThresholdsQuery = this.GetEntityQuery<MobThresholdsComponent>();
    this._xenoFriendlyQuery = this.GetEntityQuery<XenoFriendlyComponent>();
    this._xenoNestedQuery = this.GetEntityQuery<XenoNestedComponent>();
    this._xenoPlasmaQuery = this.GetEntityQuery<XenoPlasmaComponent>();
    this._xenoRecoveryQuery = this.GetEntityQuery<XenoRecoveryPheromonesComponent>();
    this._victimInfectedQuery = this.GetEntityQuery<VictimInfectedComponent>();
    this.SubscribeLocalEvent<XenoComponent, MapInitEvent>(new EntityEventRefHandler<XenoComponent, MapInitEvent>(this.OnXenoMapInit), new Type[1]
    {
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<XenoComponent, GetAccessTagsEvent>(new EntityEventRefHandler<XenoComponent, GetAccessTagsEvent>(this.OnXenoGetAdditionalAccess));
    this.SubscribeLocalEvent<XenoComponent, NewXenoEvolvedEvent>(new EntityEventRefHandler<XenoComponent, NewXenoEvolvedEvent>(this.OnNewXenoEvolved));
    this.SubscribeLocalEvent<XenoComponent, XenoDevolvedEvent>(new EntityEventRefHandler<XenoComponent, XenoDevolvedEvent>(this.OnXenoDevolved));
    this.SubscribeLocalEvent<XenoComponent, HealthScannerAttemptTargetEvent>(new EntityEventRefHandler<XenoComponent, HealthScannerAttemptTargetEvent>(this.OnXenoHealthScannerAttemptTarget));
    this.SubscribeLocalEvent<XenoComponent, GetDefaultRadioChannelEvent>(new EntityEventRefHandler<XenoComponent, GetDefaultRadioChannelEvent>(this.OnXenoGetDefaultRadioChannel));
    this.SubscribeLocalEvent<XenoComponent, AttackAttemptEvent>(new EntityEventRefHandler<XenoComponent, AttackAttemptEvent>(this.OnXenoAttackAttempt));
    this.SubscribeLocalEvent<XenoComponent, MeleeAttackAttemptEvent>(new EntityEventRefHandler<XenoComponent, MeleeAttackAttemptEvent>(this.OnXenoMeleeAttackAttempt));
    this.SubscribeLocalEvent<XenoComponent, XenoHealAttemptEvent>(new EntityEventRefHandler<XenoComponent, XenoHealAttemptEvent>(this.OnHealAttempt));
    this.SubscribeLocalEvent<XenoComponent, UserOpenActivatableUIAttemptEvent>(new EntityEventRefHandler<XenoComponent, UserOpenActivatableUIAttemptEvent>(this.OnXenoOpenActivatableUIAttempt));
    this.SubscribeLocalEvent<XenoComponent, GetMeleeDamageEvent>(new EntityEventRefHandler<XenoComponent, GetMeleeDamageEvent>(this.OnXenoGetMeleeDamage));
    this.SubscribeLocalEvent<XenoComponent, DamageModifyEvent>(new EntityEventRefHandler<XenoComponent, DamageModifyEvent>(this.OnXenoDamageModify));
    this.SubscribeLocalEvent<XenoComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoRefreshSpeed));
    this.SubscribeLocalEvent<XenoComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoComponent, MeleeHitEvent>(this.OnXenoMeleeHit));
    this.SubscribeLocalEvent<XenoComponent, HiveChangedEvent>(new EntityEventRefHandler<XenoComponent, HiveChangedEvent>(this.OnHiveChanged));
    this.SubscribeLocalEvent<XenoComponent, IgnitedEvent>(new EntityEventRefHandler<XenoComponent, IgnitedEvent>(this.OnXenoIgnite));
    this.SubscribeLocalEvent<XenoComponent, CanDragEvent>(new EntityEventRefHandler<XenoComponent, CanDragEvent>(this.OnXenoCanDrag));
    this.SubscribeLocalEvent<XenoComponent, BuckleAttemptEvent>(new EntityEventRefHandler<XenoComponent, BuckleAttemptEvent>(this.OnXenoBuckleAttempt));
    this.SubscribeLocalEvent<XenoComponent, GetVisMaskEvent>(new EntityEventRefHandler<XenoComponent, GetVisMaskEvent>(this.OnXenoGetVisMask));
    this.SubscribeLocalEvent<XenoComponent, CMDisarmEvent>(new EntityEventRefHandler<XenoComponent, CMDisarmEvent>(this.OnLeaderDisarmed), new Type[2]
    {
      typeof (SharedHandsSystem),
      typeof (SharedStaminaSystem)
    }, new Type[1]{ typeof (TackleSystem) });
    this.SubscribeLocalEvent<XenoComponent, DisarmedEvent>(new EntityEventRefHandler<XenoComponent, DisarmedEvent>(this.OnDisarmed), new Type[1]
    {
      typeof (SharedHandsSystem)
    });
    this.SubscribeLocalEvent<XenoRegenComponent, MapInitEvent>(new EntityEventRefHandler<XenoRegenComponent, MapInitEvent>(this.OnXenoRegenMapInit), new Type[1]
    {
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<XenoRegenComponent, DamageStateCritBeforeDamageEvent>(new EntityEventRefHandler<XenoRegenComponent, DamageStateCritBeforeDamageEvent>(this.OnXenoRegenBeforeCritDamage), new Type[1]
    {
      typeof (SharedXenoPheromonesSystem)
    });
    this.SubscribeLocalEvent<XenoStateVisualsComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoStateVisualsComponent, MobStateChangedEvent>(this.OnVisualsMobStateChanged));
    this.SubscribeLocalEvent<XenoStateVisualsComponent, XenoFortifiedEvent>(new EntityEventRefHandler<XenoStateVisualsComponent, XenoFortifiedEvent>(this.OnVisualsFortified));
    this.SubscribeLocalEvent<XenoStateVisualsComponent, XenoRestEvent>(new EntityEventRefHandler<XenoStateVisualsComponent, XenoRestEvent>(this.OnVisualsRest));
    this.SubscribeLocalEvent<XenoStateVisualsComponent, DownedEvent>(new EntityEventRefHandler<XenoStateVisualsComponent, DownedEvent>(this.OnVisualsProne));
    this.SubscribeLocalEvent<XenoStateVisualsComponent, StoodEvent>(new EntityEventRefHandler<XenoStateVisualsComponent, StoodEvent>(this.OnVisualsStand));
    this.SubscribeLocalEvent<XenoStateVisualsComponent, XenoOvipositorChangedEvent>(new EntityEventRefHandler<XenoStateVisualsComponent, XenoOvipositorChangedEvent>(this.OnVisualsOvipositor));
    this.Subs.CVar<float>(this._config, RMCCVars.CMXenoDamageDealtMultiplier, (Action<float>) (v => this._xenoDamageDealtMultiplier = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.CMXenoDamageReceivedMultiplier, (Action<float>) (v => this._xenoDamageReceivedMultiplier = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.CMXenoSpeedMultiplier, new Action<float>(this.UpdateXenoSpeedMultiplier), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCXenoSpawnInitialMuteDurationSeconds, (Action<float>) (v => this._xenoSpawnMuteDuration = TimeSpan.FromSeconds((double) v)), true);
    this.UpdatesAfter.Add(typeof (SharedXenoPheromonesSystem));
  }

  private void OnXenoMapInit(Entity<XenoComponent> xeno, ref MapInitEvent args)
  {
    foreach (EntProtoId actionId in xeno.Comp.ActionIds)
    {
      if (!xeno.Comp.Actions.ContainsKey(actionId))
      {
        EntityUid? nullable = this._action.AddAction((EntityUid) xeno, (string) actionId);
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          xeno.Comp.Actions[actionId] = valueOrDefault;
        }
      }
    }
    if (!MathHelper.CloseTo(this._xenoSpeedMultiplier, 1f, 1E-07f))
      this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    if (xeno.Comp.MuteOnSpawn)
      this._status.TryAddStatusEffect((EntityUid) xeno, "Muted", this._xenoSpawnMuteDuration, true, "Muted");
    this._eye.RefreshVisibilityMask((Entity<EyeComponent>) xeno.Owner);
    this.Dirty<XenoComponent>(xeno);
  }

  private void OnXenoGetAdditionalAccess(Entity<XenoComponent> xeno, ref GetAccessTagsEvent args)
  {
    args.Tags.UnionWith((IEnumerable<ProtoId<AccessLevelPrototype>>) xeno.Comp.AccessLevels);
  }

  private void OnNewXenoEvolved(Entity<XenoComponent> newXeno, ref NewXenoEvolvedEvent args)
  {
    Angle worldRotation = this._transform.GetWorldRotation((EntityUid) args.OldXeno);
    this._transform.SetWorldRotation((EntityUid) newXeno, worldRotation);
  }

  private void OnXenoDevolved(Entity<XenoComponent> newXeno, ref XenoDevolvedEvent args)
  {
    Angle worldRotation = this._transform.GetWorldRotation(args.OldXeno);
    this._transform.SetWorldRotation((EntityUid) newXeno, worldRotation);
  }

  private void OnXenoHealthScannerAttemptTarget(
    Entity<XenoComponent> ent,
    ref HealthScannerAttemptTargetEvent args)
  {
    args.Popup = "The scanner can't make sense of this creature.";
    args.Cancelled = true;
  }

  private void OnXenoGetDefaultRadioChannel(
    Entity<XenoComponent> ent,
    ref GetDefaultRadioChannelEvent args)
  {
    args.Channel = (string) SharedChatSystem.HivemindChannel;
  }

  private void OnXenoAttackAttempt(Entity<XenoComponent> xeno, ref AttackAttemptEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (this._xenoFriendlyQuery.HasComp(valueOrDefault) && this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) valueOrDefault) || this._mobState.IsDead(valueOrDefault))
    {
      if (args.Disarm)
        return;
      args.Cancel();
    }
    else
    {
      if (!this._xenoNestedQuery.HasComp(valueOrDefault) || !this._victimInfectedQuery.HasComp(valueOrDefault) || args.Disarm)
        return;
      args.Cancel();
    }
  }

  private void OnXenoMeleeAttackAttempt(
    Entity<XenoComponent> xeno,
    ref MeleeAttackAttemptEvent args)
  {
    XenoNestComponent comp;
    if (!this.TryComp<XenoNestComponent>(this.GetEntity(args.Target), out comp) || !comp.Nested.HasValue || !this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) this.GetEntity(args.Target)))
      return;
    NetEntity netEntity = this.GetNetEntity((EntityUid) xeno);
    args.Target = this.GetNetEntity(comp.Nested.Value);
    switch (args.Attack)
    {
      case LightAttackEvent lightAttackEvent:
        args.Attack = (AttackEvent) new LightAttackEvent(new NetEntity?(args.Target), netEntity, lightAttackEvent.Coordinates);
        break;
      case DisarmAttackEvent disarmAttackEvent:
        args.Attack = (AttackEvent) new DisarmAttackEvent(new NetEntity?(args.Target), disarmAttackEvent.Coordinates);
        break;
    }
  }

  private void OnHealAttempt(Entity<XenoComponent> ent, ref XenoHealAttemptEvent args)
  {
    if (!this._rmcFlammable.IsOnFire((Entity<FlammableComponent>) ent.Owner))
      return;
    args.Cancelled = true;
  }

  private void OnXenoOpenActivatableUIAttempt(
    Entity<XenoComponent> ent,
    ref UserOpenActivatableUIAttemptEvent args)
  {
    if (args.Cancelled || !this.HasComp<LatheComponent>(args.Target) && !this.HasComp<CMAutomatedVendorComponent>(args.Target))
      return;
    args.Cancel();
  }

  private void OnXenoGetMeleeDamage(Entity<XenoComponent> ent, ref GetMeleeDamageEvent args)
  {
    if (MathHelper.CloseTo(this._xenoDamageDealtMultiplier, 1f, 1E-07f))
      return;
    args.Damage *= this._xenoDamageDealtMultiplier;
  }

  private void OnXenoDamageModify(Entity<XenoComponent> ent, ref DamageModifyEvent args)
  {
    if (MathHelper.CloseTo(this._xenoDamageReceivedMultiplier, 1f, 1E-07f))
      return;
    args.Damage *= this._xenoDamageReceivedMultiplier;
  }

  private void OnXenoRefreshSpeed(
    Entity<XenoComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (MathHelper.CloseTo(this._xenoSpeedMultiplier, 1f, 1E-07f))
      return;
    args.ModifySpeed(this._xenoSpeedMultiplier, this._xenoSpeedMultiplier);
  }

  private void OnXenoMeleeHit(Entity<XenoComponent> xeno, ref MeleeHitEvent args)
  {
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      SharedEntityStorageComponent component = (SharedEntityStorageComponent) null;
      if (this._entityStorage.ResolveStorage(hitEntity, ref component))
      {
        if (this._weldable.IsWelded(hitEntity))
          this._weldable.SetWeldedState(hitEntity, false);
        this._entityStorage.TryOpenStorage((EntityUid) xeno, hitEntity);
      }
    }
  }

  private void OnHiveChanged(Entity<XenoComponent> ent, ref HiveChangedEvent args)
  {
    SharedNightVisionSystem nightVision = this._nightVision;
    Entity<NightVisionComponent> owner = (Entity<NightVisionComponent>) ent.Owner;
    Entity<HiveComponent>? hive = args.Hive;
    ref Entity<HiveComponent>? local = ref hive;
    int num = local.HasValue ? (local.GetValueOrDefault().Comp.SeeThroughContainers ? 1 : 0) : 0;
    nightVision.SetSeeThroughContainers(owner, num != 0);
  }

  private void OnXenoIgnite(Entity<XenoComponent> ent, ref IgnitedEvent args)
  {
    foreach (EntityUid entityUid in this._hands.EnumerateHeld((Entity<HandsComponent>) ent.Owner).ToArray<EntityUid>())
    {
      if (this.HasComp<XenoParasiteComponent>(entityUid))
      {
        DamageSpecifier damage = new DamageSpecifier()
        {
          DamageDict = {
            [(string) XenoSystem.HeatDamage] = (FixedPoint2) 100
          }
        };
        this._damageable.TryChangeDamage(new EntityUid?(entityUid), damage, true);
        this._hands.TryDrop((Entity<HandsComponent>) ent.Owner, entityUid);
      }
    }
  }

  private void OnXenoCanDrag(Entity<XenoComponent> ent, ref CanDragEvent args)
  {
    if (!this._mobState.IsDead((EntityUid) ent))
      return;
    args.Handled = true;
  }

  private void OnXenoBuckleAttempt(Entity<XenoComponent> ent, ref BuckleAttemptEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.User) && this._mobState.IsDead((EntityUid) ent))
      return;
    args.Cancelled = true;
  }

  private void OnXenoGetVisMask(Entity<XenoComponent> ent, ref GetVisMaskEvent args)
  {
    args.VisibilityMask |= (int) ent.Comp.Visibility;
  }

  private void OnLeaderDisarmed(Entity<XenoComponent> ent, ref CMDisarmEvent args)
  {
    TimeSpan time;
    if (args.Handled || !this.CanTackleOtherXeno(args.User, (EntityUid) ent, out time))
      return;
    this._stun.TryParalyze((EntityUid) ent, time, true);
  }

  private void OnDisarmed(Entity<XenoComponent> ent, ref DisarmedEvent args)
  {
    args.PopupPrefix = "disarm-action-shove-";
    args.Handled = true;
  }

  private void OnXenoRegenMapInit(Entity<XenoRegenComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextRegenTime = this._timing.CurTime + ent.Comp.RegenCooldown;
    this.Dirty<XenoRegenComponent>(ent);
  }

  private void OnXenoRegenBeforeCritDamage(
    Entity<XenoRegenComponent> ent,
    ref DamageStateCritBeforeDamageEvent args)
  {
    if (!this._rmcFlammable.IsOnFire((Entity<FlammableComponent>) ent.Owner) && !ent.Comp.HealOffWeeds && !this._weeds.IsOnFriendlyWeeds((Entity<TransformComponent>) ent.Owner))
      return;
    args.Damage.ClampMax((FixedPoint2) 0);
  }

  private void UpdateXenoSpeedMultiplier(float speed)
  {
    this._xenoSpeedMultiplier = speed;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoComponent, MovementSpeedModifierComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoComponent, MovementSpeedModifierComponent>();
    EntityUid uid;
    MovementSpeedModifierComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out XenoComponent _, out comp2))
      this._movementSpeed.RefreshMovementSpeedModifiers(uid, comp2);
  }

  public void MakeXeno(Entity<XenoComponent?> xeno)
  {
    this.EnsureComp<XenoComponent>((EntityUid) xeno);
  }

  private FixedPoint2 GetWeedsHealAmount(Entity<XenoRegenComponent> xeno)
  {
    MobThresholdsComponent component;
    FixedPoint2? threshold;
    if (!this._mobThresholdsQuery.TryComp((EntityUid) xeno, out component) || !this._mobThresholds.TryGetIncapThreshold((EntityUid) xeno, out threshold, component))
      return FixedPoint2.Zero;
    FixedPoint2 fixedPoint2_1 = !this._mobState.IsCritical((EntityUid) xeno) ? (this._standing.IsDown((EntityUid) xeno) || this.HasComp<XenoRestingComponent>((EntityUid) xeno) ? xeno.Comp.RestHealMultiplier : xeno.Comp.StandHealingMultiplier) : xeno.Comp.CritHealMultiplier;
    FixedPoint2 fixedPoint2_2 = threshold.Value / 65f + xeno.Comp.FlatHealing;
    XenoRecoveryPheromonesComponent pheromonesComponent = this.CompOrNull<XenoRecoveryPheromonesComponent>((EntityUid) xeno);
    FixedPoint2 fixedPoint2_3 = pheromonesComponent != null ? pheromonesComponent.Multiplier : (FixedPoint2) 0;
    if (!this.CanHeal((EntityUid) xeno))
      fixedPoint2_3 = FixedPoint2.Zero;
    FixedPoint2 fixedPoint2_4 = threshold.Value / 65f * (fixedPoint2_3 / 2f);
    return (fixedPoint2_2 + fixedPoint2_4) * fixedPoint2_1 / 2f;
  }

  public void HealDamage(Entity<DamageableComponent?> xeno, FixedPoint2 amount)
  {
    MobStateComponent component;
    if (this._rmcFlammable.IsOnFire((Entity<FlammableComponent>) xeno.Owner) || !this._damageableQuery.Resolve((EntityUid) xeno, ref xeno.Comp, false) || xeno.Comp.Damage.GetTotal() <= FixedPoint2.Zero || this._mobStateQuery.TryGetComponent((EntityUid) xeno, out component) && this._mobState.IsDead((EntityUid) xeno, component))
      return;
    DamageSpecifier damage = this._rmcDamageable.DistributeTypes((Entity<DamageableComponent>) ((EntityUid) xeno, xeno.Comp), -amount);
    if (damage.GetTotal() > FixedPoint2.Zero)
      this.Log.Error($"Tried to deal damage while healing xeno {this.ToPrettyString(new EntityUid?((EntityUid) xeno))}");
    else
      this._damageable.TryChangeDamage(new EntityUid?((EntityUid) xeno), damage, true, origin: new EntityUid?((EntityUid) xeno));
  }

  public bool CanAbilityAttackTarget(
    EntityUid xeno,
    EntityUid target,
    bool canAttackBarricades = false,
    bool canAttackWindows = false)
  {
    if (xeno == target || this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno, (Entity<HiveMemberComponent>) target) || this._mobState.IsDead(target) || this.HasComp<DevouredComponent>(target) || this._xenoNestedQuery.HasComp(target))
      return false;
    return canAttackBarricades && this.HasComp<BarricadeComponent>(target) || canAttackWindows && this.HasComp<DestroyOnXenoPierceScissorComponent>(target) || this.HasComp<MarineComponent>(target) || this.HasComp<XenoComponent>(target);
  }

  public bool CanHeal(EntityUid xeno)
  {
    XenoHealAttemptEvent args = new XenoHealAttemptEvent();
    this.RaiseLocalEvent<XenoHealAttemptEvent>(xeno, ref args);
    return !args.Cancelled;
  }

  public int GetGroundXenosAlive()
  {
    int groundXenosAlive = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActorComponent, XenoComponent, MobStateComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActorComponent, XenoComponent, MobStateComponent, TransformComponent>();
    MobStateComponent comp3;
    TransformComponent comp4;
    while (entityQueryEnumerator.MoveNext(out ActorComponent _, out XenoComponent _, out comp3, out comp4))
    {
      if (comp3.CurrentState != MobState.Dead && this._rmcPlanet.IsOnPlanet(comp4))
        ++groundXenosAlive;
    }
    return groundXenosAlive;
  }

  public bool CanTackleOtherXeno(EntityUid sourceXeno, EntityUid targetXeno, out TimeSpan time)
  {
    time = TimeSpan.Zero;
    HiveLeaderComponent leaderComp;
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) targetXeno, (Entity<HiveMemberComponent>) sourceXeno) || !this._hiveLeader.IsLeader(sourceXeno, out leaderComp) || this._hiveLeader.IsLeader(targetXeno, out HiveLeaderComponent _) || this.HasComp<XenoEvolutionGranterComponent>(targetXeno))
      return false;
    time = leaderComp.FriendlyStunTime;
    return true;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoRegenComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoRegenComponent>();
    EntityUid uid;
    XenoRegenComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.NextRegenTime))
      {
        comp1.NextRegenTime = curTime + comp1.RegenCooldown;
        this.DirtyField<XenoRegenComponent>(uid, comp1, "NextRegenTime");
        if (!comp1.HealOffWeeds)
        {
          if (this.Transform(uid).Anchored)
            this._weeds.UpdateQueued(uid);
          AffectableByWeedsComponent byWeedsComponent = this._affectableQuery.CompOrNull(uid);
          bool flag = byWeedsComponent != null && byWeedsComponent.OnXenoWeeds && byWeedsComponent.OnFriendlyWeeds;
          if (byWeedsComponent == null || !flag)
          {
            XenoPlasmaComponent component;
            if (this._xenoPlasmaQuery.TryComp(uid, out component))
            {
              FixedPoint2 amount = FixedPoint2.Max(component.PlasmaRegenOffWeeds * component.MaxPlasma / 100f / 2f, (FixedPoint2) 0.01);
              this._xenoPlasma.RegenPlasma((Entity<XenoPlasmaComponent>) (uid, component), amount);
              continue;
            }
            continue;
          }
        }
        FixedPoint2 weedsHealAmount = this.GetWeedsHealAmount((Entity<XenoRegenComponent>) (uid, comp1));
        if (weedsHealAmount > FixedPoint2.Zero)
        {
          this.HealDamage((Entity<DamageableComponent>) uid, weedsHealAmount);
          XenoPlasmaComponent component1;
          if (this._xenoPlasmaQuery.TryComp(uid, out component1))
          {
            FixedPoint2 amount1 = component1.PlasmaRegenOnWeeds * component1.MaxPlasma / 100f / 2f;
            this._xenoPlasma.RegenPlasma((Entity<XenoPlasmaComponent>) (uid, component1), amount1);
            XenoRecoveryPheromonesComponent component2;
            if (this._xenoRecoveryQuery.TryComp(uid, out component2))
            {
              FixedPoint2 amount2 = amount1 * component2.Multiplier / 4f;
              this._xenoPlasma.RegenPlasma((Entity<XenoPlasmaComponent>) (uid, component1), amount2);
            }
          }
        }
      }
    }
  }

  public DamageSpecifier TryApplyXenoSlashDamageMultiplier(
    EntityUid target,
    DamageSpecifier baseDamage)
  {
    RMCSizes size;
    return !this._size.TryGetSize(target, out size) || !this._size.IsXenoSized(size) ? baseDamage : baseDamage * 1.5f;
  }

  public TimeSpan TryApplyXenoDebuffMultiplier(EntityUid target, TimeSpan baseDuration)
  {
    RMCSizes size;
    return !this._size.TryGetSize(target, out size) || !this._size.IsXenoSized(size) ? baseDuration : baseDuration * 1.25;
  }

  public DamageSpecifier TryApplyXenoAcidDamageMultiplier(
    EntityUid target,
    DamageSpecifier baseDamage)
  {
    return !this.HasComp<XenoComponent>(target) ? baseDamage : baseDamage * 2.625f;
  }

  public DamageSpecifier TryApplyXenoProjectileDamageMultiplier(
    EntityUid target,
    DamageSpecifier baseDamage)
  {
    return !this.HasComp<XenoComponent>(target) ? baseDamage : baseDamage * 2.625f;
  }

  private void OnVisualsMobStateChanged(
    Entity<XenoStateVisualsComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) RMCXenoStateVisuals.Downed, (object) (args.NewMobState != MobState.Alive));
    this._appearance.SetData((EntityUid) ent, (Enum) RMCXenoStateVisuals.Dead, (object) (args.NewMobState == MobState.Dead));
  }

  private void OnVisualsFortified(
    Entity<XenoStateVisualsComponent> ent,
    ref XenoFortifiedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) RMCXenoStateVisuals.Fortified, (object) args.Fortified);
  }

  private void OnVisualsRest(Entity<XenoStateVisualsComponent> ent, ref XenoRestEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) RMCXenoStateVisuals.Resting, (object) args.Resting);
  }

  private void OnVisualsProne(Entity<XenoStateVisualsComponent> xeno, ref DownedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) xeno, (Enum) RMCXenoStateVisuals.Downed, (object) true);
  }

  private void OnVisualsStand(Entity<XenoStateVisualsComponent> xeno, ref StoodEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) xeno, (Enum) RMCXenoStateVisuals.Downed, (object) false);
  }

  private void OnVisualsOvipositor(
    Entity<XenoStateVisualsComponent> xeno,
    ref XenoOvipositorChangedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) xeno, (Enum) RMCXenoStateVisuals.Ovipositor, (object) args.Attached);
  }
}
