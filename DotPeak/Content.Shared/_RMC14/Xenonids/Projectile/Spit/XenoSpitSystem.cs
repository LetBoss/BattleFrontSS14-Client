// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.XenoSpitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Ball;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Scattered;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Shield;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Slowing;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Standard;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit;

public sealed class XenoSpitSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoProjectileSystem _xenoProjectile;
  [Dependency]
  private XenoShieldSystem _xenoShield;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private CMArmorSystem _armor;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private XenoSystem _xeno;
  private static readonly ProtoId<ReagentPrototype> AcidRemovedBy = (ProtoId<ReagentPrototype>) "Water";
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;

  public override void Initialize()
  {
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this.SubscribeLocalEvent<XenoSpitComponent, XenoSpitActionEvent>(new EntityEventRefHandler<XenoSpitComponent, XenoSpitActionEvent>(this.OnXenoSpitAction));
    this.SubscribeLocalEvent<XenoSlowingSpitComponent, XenoSlowingSpitActionEvent>(new EntityEventRefHandler<XenoSlowingSpitComponent, XenoSlowingSpitActionEvent>(this.OnXenoSlowingSpitAction));
    this.SubscribeLocalEvent<XenoScatteredSpitComponent, XenoScatteredSpitActionEvent>(new EntityEventRefHandler<XenoScatteredSpitComponent, XenoScatteredSpitActionEvent>(this.OnXenoScatteredSpitAction));
    this.SubscribeLocalEvent<XenoChargeSpitComponent, XenoChargeSpitActionEvent>(new EntityEventRefHandler<XenoChargeSpitComponent, XenoChargeSpitActionEvent>(this.OnXenoChargeSpitAction));
    this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, ComponentStartup>(new EntityEventRefHandler<XenoActiveChargingSpitComponent, ComponentStartup>(this.OnActiveChargingSpitAdded));
    this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, ComponentRemove>(new EntityEventRefHandler<XenoActiveChargingSpitComponent, ComponentRemove>(this.OnActiveChargingSpitRemove));
    this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, CMGetArmorEvent>(new EntityEventRefHandler<XenoActiveChargingSpitComponent, CMGetArmorEvent>(this.OnActiveChargingSpitGetArmor));
    this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoActiveChargingSpitComponent, RefreshMovementSpeedModifiersEvent>(this.OnActiveChargingSpitRefreshSpeed));
    this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, XenoGetSpitProjectileEvent>(new EntityEventRefHandler<XenoActiveChargingSpitComponent, XenoGetSpitProjectileEvent>(this.OnActiveChargingSpitGetProjectile));
    this.SubscribeLocalEvent<XenoSlowingSpitProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<XenoSlowingSpitProjectileComponent, ProjectileHitEvent>(this.OnXenoSlowingSpitHit), after: new Type[1]
    {
      typeof (CMClusterGrenadeSystem)
    });
    this.SubscribeLocalEvent<XenoAcidBallComponent, XenoAcidBallActionEvent>(new EntityEventRefHandler<XenoAcidBallComponent, XenoAcidBallActionEvent>(this.OnXenoAcidBallAction));
    this.SubscribeLocalEvent<XenoAcidBallComponent, XenoAcidBallDoAfterEvent>(new EntityEventRefHandler<XenoAcidBallComponent, XenoAcidBallDoAfterEvent>(this.OnXenoAcidBallDoAfter));
    this.SubscribeLocalEvent<ApplyAcidStacksComponent, ProjectileHitEvent>(new EntityEventRefHandler<ApplyAcidStacksComponent, ProjectileHitEvent>(this.OnApplyAcidStacksProjectileHit), after: new Type[1]
    {
      typeof (CMClusterGrenadeSystem)
    });
    this.SubscribeLocalEvent<ApplyAcidStacksComponent, DamageCollideEvent>(new EntityEventRefHandler<ApplyAcidStacksComponent, DamageCollideEvent>(this.OnApplyAcidStacksDamageCollide));
    this.SubscribeLocalEvent<XenoProjectileShieldOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<XenoProjectileShieldOnHitComponent, ProjectileHitEvent>(this.OnShieldOnHit), after: new Type[1]
    {
      typeof (CMClusterGrenadeSystem)
    });
    this.SubscribeLocalEvent<XenoProjectileShieldOnHitComponent, CMClusterSpawnedEvent>(new EntityEventRefHandler<XenoProjectileShieldOnHitComponent, CMClusterSpawnedEvent>(this.OnShieldOnHitClusterSpawned));
    this.SubscribeLocalEvent<UserAcidedComponent, MapInitEvent>(new EntityEventRefHandler<UserAcidedComponent, MapInitEvent>(this.OnUserAcidedMapInit));
    this.SubscribeLocalEvent<UserAcidedComponent, ComponentRemove>(new EntityEventRefHandler<UserAcidedComponent, ComponentRemove>(this.OnUserAcidedRemove));
    this.SubscribeLocalEvent<UserAcidedComponent, ShowFireAlertEvent>(new EntityEventRefHandler<UserAcidedComponent, ShowFireAlertEvent>(this.OnUserAcidedShowFireAlert));
    this.SubscribeLocalEvent<UserAcidedComponent, VaporHitEvent>(new EntityEventRefHandler<UserAcidedComponent, VaporHitEvent>(this.OnUserAcidedVaporHit));
    this.SubscribeLocalEvent<UserAcidedComponent, MobStateChangedEvent>(new EntityEventRefHandler<UserAcidedComponent, MobStateChangedEvent>(this.OnUserAcidedMobStateChanged));
    this.SubscribeLocalEvent<InventoryComponent, HitBySlowingSpitEvent>(new EntityEventRefHandler<InventoryComponent, HitBySlowingSpitEvent>(this._inventory.RelayEvent<HitBySlowingSpitEvent>));
    this.SubscribeLocalEvent<DrainOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<DrainOnHitComponent, ProjectileHitEvent>(this.OnDrainOnHitProjectileHit), after: new Type[1]
    {
      typeof (CMClusterGrenadeSystem)
    });
  }

  private void OnActiveChargingSpitRemove(
    Entity<XenoActiveChargingSpitComponent> ent,
    ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ((EntityUid) ent, (CMArmorComponent) null));
  }

  private void OnActiveChargingSpitAdded(
    Entity<XenoActiveChargingSpitComponent> ent,
    ref ComponentStartup args)
  {
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ((EntityUid) ent, (CMArmorComponent) null));
  }

  private void OnActiveChargingSpitGetArmor(
    Entity<XenoActiveChargingSpitComponent> ent,
    ref CMGetArmorEvent args)
  {
    args.XenoArmor += ent.Comp.Armor;
  }

  private void OnActiveChargingSpitRefreshSpeed(
    Entity<XenoActiveChargingSpitComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    args.ModifySpeed(ent.Comp.Speed, ent.Comp.Speed);
  }

  private void OnActiveChargingSpitGetProjectile(
    Entity<XenoActiveChargingSpitComponent> ent,
    ref XenoGetSpitProjectileEvent args)
  {
    if (ent.Comp.FiredProjectile)
      return;
    args.Id = ent.Comp.Projectile;
  }

  private void OnXenoSpitAction(Entity<XenoSpitComponent> xeno, ref XenoSpitActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((WorldTargetActionEvent) args))
      return;
    XenoGetSpitProjectileEvent args1 = new XenoGetSpitProjectileEvent(xeno.Comp.ProjectileId);
    this.RaiseLocalEvent<XenoGetSpitProjectileEvent>((EntityUid) xeno, ref args1);
    XenoSpitActionEvent xenoSpitActionEvent = args;
    XenoProjectileSystem xenoProjectile = this._xenoProjectile;
    EntityUid xeno1 = (EntityUid) xeno;
    EntityCoordinates target1 = args.Target;
    FixedPoint2 plasmaCost = xeno.Comp.PlasmaCost;
    EntProtoId id = args1.Id;
    SoundSpecifier sound = xeno.Comp.Sound;
    Angle zero = Angle.Zero;
    double speed = (double) xeno.Comp.Speed;
    EntityUid? entity = args.Entity;
    float? stopAtDistance = new float?();
    EntityUid? target2 = entity;
    int? projectileHitLimit = new int?();
    int num = xenoProjectile.TryShoot(xeno1, target1, plasmaCost, id, sound, 1, zero, (float) speed, stopAtDistance, target2, projectileHitLimit: projectileHitLimit) ? 1 : 0;
    xenoSpitActionEvent.Handled = num != 0;
    XenoActiveChargingSpitComponent comp;
    if (!this.TryComp<XenoActiveChargingSpitComponent>((EntityUid) xeno, out comp))
      return;
    comp.FiredProjectile = true;
    this.Dirty((EntityUid) xeno, (IComponent) comp);
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-charge-spit-expire"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
  }

  private void OnXenoSlowingSpitAction(
    Entity<XenoSlowingSpitComponent> xeno,
    ref XenoSlowingSpitActionEvent args)
  {
    if (args.Handled)
      return;
    XenoSlowingSpitActionEvent slowingSpitActionEvent = args;
    XenoProjectileSystem xenoProjectile = this._xenoProjectile;
    EntityUid xeno1 = (EntityUid) xeno;
    EntityCoordinates target1 = args.Target;
    FixedPoint2 plasmaCost = xeno.Comp.PlasmaCost;
    EntProtoId projectileId = xeno.Comp.ProjectileId;
    SoundSpecifier sound = xeno.Comp.Sound;
    Angle zero = Angle.Zero;
    double speed = (double) xeno.Comp.Speed;
    EntityUid? entity = args.Entity;
    float? stopAtDistance = new float?();
    EntityUid? target2 = entity;
    int? projectileHitLimit = new int?();
    int num = xenoProjectile.TryShoot(xeno1, target1, plasmaCost, projectileId, sound, 1, zero, (float) speed, stopAtDistance, target2, projectileHitLimit: projectileHitLimit) ? 1 : 0;
    slowingSpitActionEvent.Handled = num != 0;
  }

  private void OnXenoScatteredSpitAction(
    Entity<XenoScatteredSpitComponent> xeno,
    ref XenoScatteredSpitActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((WorldTargetActionEvent) args))
      return;
    XenoScatteredSpitActionEvent scatteredSpitActionEvent = args;
    XenoProjectileSystem xenoProjectile = this._xenoProjectile;
    EntityUid xeno1 = (EntityUid) xeno;
    EntityCoordinates target1 = args.Target;
    FixedPoint2 plasmaCost = xeno.Comp.PlasmaCost;
    EntProtoId projectileId = xeno.Comp.ProjectileId;
    SoundSpecifier sound = xeno.Comp.Sound;
    int maxProjectiles = xeno.Comp.MaxProjectiles;
    Angle maxDeviation = xeno.Comp.MaxDeviation;
    double speed = (double) xeno.Comp.Speed;
    EntityUid? entity = args.Entity;
    float? stopAtDistance = new float?();
    EntityUid? target2 = entity;
    int? projectileHitLimit = new int?();
    int num = xenoProjectile.TryShoot(xeno1, target1, plasmaCost, projectileId, sound, maxProjectiles, maxDeviation, (float) speed, stopAtDistance, target2, projectileHitLimit: projectileHitLimit) ? 1 : 0;
    scatteredSpitActionEvent.Handled = num != 0;
  }

  private void OnXenoChargeSpitAction(
    Entity<XenoChargeSpitComponent> xeno,
    ref XenoChargeSpitActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((InstantActionEvent) args))
      return;
    args.Handled = true;
    XenoActiveChargingSpitComponent chargingSpitComponent = this.EnsureComp<XenoActiveChargingSpitComponent>((EntityUid) xeno);
    chargingSpitComponent.ExpiresAt = this._timing.CurTime + xeno.Comp.Duration;
    chargingSpitComponent.Armor = xeno.Comp.Armor;
    chargingSpitComponent.Speed = xeno.Comp.Speed;
    this.Dirty((EntityUid) xeno, (IComponent) chargingSpitComponent);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-charge-spit"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    if (!this._net.IsServer)
      return;
    this.SpawnAttachedTo((string) xeno.Comp.Effect, xeno.Owner.ToCoordinates(), rotation: new Angle());
  }

  private void OnXenoSlowingSpitHit(
    Entity<XenoSlowingSpitProjectileComponent> spit,
    ref ProjectileHitEvent args)
  {
    EntityUid target = args.Target;
    if (this._hive.FromSameHive((Entity<HiveMemberComponent>) spit.Owner, (Entity<HiveMemberComponent>) target) || this.HasComp<XenoComponent>(target))
      this.PredictedQueueDel(spit.Owner);
    else if (this.HasComp<SynthComponent>(target))
    {
      this._popup.PopupEntity(this.Loc.GetString("cm-xeno-paralyzing-slash-immune", ("target", (object) target)), target, target, PopupType.SmallCaution);
    }
    else
    {
      Filter filter1 = Filter.Pvs(target);
      XenoProjectileShotComponent comp;
      if (this.TryComp<XenoProjectileShotComponent>((EntityUid) spit, out comp))
      {
        ICommonSession shooter = comp.Shooter;
        if (shooter != null)
          filter1 = filter1.RemovePlayer(shooter);
      }
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
      if (this._net.IsClient)
        return;
      if (spit.Comp.Slow > TimeSpan.Zero)
      {
        if (spit.Comp.SuperSlow)
          this._slow.TrySuperSlowdown(target, spit.Comp.Slow);
        else
          this._slow.TrySlowdown(target, spit.Comp.Slow);
      }
      bool flag = false;
      if (spit.Comp.ArmorResistsKnockdown)
      {
        HitBySlowingSpitEvent args1 = new HitBySlowingSpitEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
        this.RaiseLocalEvent<HitBySlowingSpitEvent>(args.Target, ref args1);
        flag = args1.Cancelled;
      }
      if (flag)
        return;
      this._stun.TryParalyze(target, spit.Comp.Paralyze, true);
    }
  }

  private void OnXenoAcidBallAction(
    Entity<XenoAcidBallComponent> ent,
    ref XenoAcidBallActionEvent args)
  {
    if (args.Handled)
      return;
    XenoAcidBallDoAfterEvent @event = new XenoAcidBallDoAfterEvent(this.GetNetCoordinates(args.Target));
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) ent, ent.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      RootEntity = true
    });
  }

  private void OnXenoAcidBallDoAfter(
    Entity<XenoAcidBallComponent> ent,
    ref XenoAcidBallDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(coordinates);
    MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates((EntityUid) ent);
    if (mapCoordinates2.MapId != mapCoordinates1.MapId)
      return;
    Vector2 vector2 = mapCoordinates1.Position - mapCoordinates2.Position;
    float num = Math.Min(ent.Comp.MaxRange, vector2.Length());
    args.Handled = this._xenoProjectile.TryShoot((EntityUid) ent, coordinates, ent.Comp.PlasmaCost, ent.Comp.ProjectileId, (SoundSpecifier) null, 1, Angle.Zero, ent.Comp.Speed, new float?(num));
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoAcidBallActionEvent>((EntityUid) ent))
      this._actions.SetCooldown(new Entity<ActionComponent>?(entity.AsNullable()), ent.Comp.Cooldown);
    if (!args.Handled)
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-acid-ball-shoot-self"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
  }

  private void OnApplyAcidStacksProjectileHit(
    Entity<ApplyAcidStacksComponent> ent,
    ref ProjectileHitEvent args)
  {
    if (args.Handled)
      return;
    this.ApplyAcidStacks(args.Target, ent.Comp.Amount, ent.Comp.Max, ent.Comp.Damage, ent.Comp.Whitelist);
  }

  private void OnApplyAcidStacksDamageCollide(
    Entity<ApplyAcidStacksComponent> ent,
    ref DamageCollideEvent args)
  {
    this.ApplyAcidStacks(args.Target, ent.Comp.Amount, ent.Comp.Max, ent.Comp.Damage, ent.Comp.Whitelist);
  }

  private void OnShieldOnHit(
    Entity<XenoProjectileShieldOnHitComponent> ent,
    ref ProjectileHitEvent args)
  {
    ProjectileComponent component;
    if (!this._projectileQuery.TryComp((EntityUid) ent, out component))
      return;
    EntityUid? shooter = component.Shooter;
    if (!shooter.HasValue)
      return;
    EntityUid valueOrDefault = shooter.GetValueOrDefault();
    if (!valueOrDefault.Valid || !this._xeno.CanAbilityAttackTarget(valueOrDefault, args.Target))
      return;
    XenoShieldSystem xenoShield = this._xenoShield;
    EntityUid uid = valueOrDefault;
    int shield = (int) ent.Comp.Shield;
    FixedPoint2 amount = ent.Comp.Amount;
    double num = ent.Comp.Max.Double();
    TimeSpan? duration = new TimeSpan?();
    double maxShield = num;
    xenoShield.ApplyShield(uid, (XenoShieldSystem.ShieldType) shield, amount, duration, addShield: true, maxShield: maxShield);
  }

  private void OnShieldOnHitClusterSpawned(
    Entity<XenoProjectileShieldOnHitComponent> ent,
    ref CMClusterSpawnedEvent args)
  {
    EntityUid? shooter = (EntityUid?) this._projectileQuery.CompOrNull((EntityUid) ent)?.Shooter;
    foreach (EntityUid uid in args.Spawned)
    {
      this.EnsureComp<XenoProjectileShieldOnHitComponent>(uid);
      if (shooter.HasValue)
      {
        ProjectileComponent projectileComponent = this.EnsureComp<ProjectileComponent>(uid);
        projectileComponent.Shooter = shooter;
        this.Dirty(uid, (IComponent) projectileComponent);
      }
    }
  }

  private void OnUserAcidedMapInit(Entity<UserAcidedComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.ExpiresAt = this._timing.CurTime + ent.Comp.Duration;
    this.Dirty<UserAcidedComponent>(ent);
    this.UpdateAppearance(ent);
  }

  private void OnUserAcidedRemove(Entity<UserAcidedComponent> ent, ref ComponentRemove args)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) UserAcidedVisuals.Acided, (object) UserAcidedEffects.None);
  }

  private void OnUserAcidedShowFireAlert(
    Entity<UserAcidedComponent> ent,
    ref ShowFireAlertEvent args)
  {
    args.Show = true;
  }

  private void OnUserAcidedVaporHit(Entity<UserAcidedComponent> ent, ref VaporHitEvent args)
  {
    if (ent.Comp.AllowVaporHitAfter > this._timing.CurTime)
      return;
    Entity<SolutionContainerManagerComponent> solution = args.Solution;
    foreach ((string Name, Entity<SolutionComponent> Solution) enumerateSolution in this._solution.EnumerateSolutions((Entity<SolutionContainerManagerComponent>) ((EntityUid) solution, (SolutionContainerManagerComponent) solution)))
    {
      if (enumerateSolution.Solution.Comp.Solution.ContainsReagent((string) XenoSpitSystem.AcidRemovedBy, (List<ReagentData>) null))
      {
        if (--ent.Comp.ResistsNeeded <= 0)
        {
          this.RemCompDeferred<UserAcidedComponent>((EntityUid) ent);
          break;
        }
        ent.Comp.AllowVaporHitAfter = this._timing.CurTime + ent.Comp.ExtinguishGracePeriod;
        this.Dirty<UserAcidedComponent>(ent);
        break;
      }
    }
  }

  private void OnUserAcidedMobStateChanged(
    Entity<UserAcidedComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    this.RemCompDeferred<UserAcidedComponent>((EntityUid) ent);
  }

  public void SetAcidCombo(
    Entity<UserAcidedComponent?> acided,
    TimeSpan duration,
    DamageSpecifier? damage,
    TimeSpan paralyze,
    int resists)
  {
    if (!this.Resolve<UserAcidedComponent>((EntityUid) acided, ref acided.Comp, false) || acided.Comp.Combo)
      return;
    acided.Comp.Combo = true;
    if (damage != null)
      acided.Comp.Damage = damage;
    if (duration != new TimeSpan())
    {
      TimeSpan duration1 = acided.Comp.Duration;
      acided.Comp.Duration = duration;
      acided.Comp.ExpiresAt = acided.Comp.ExpiresAt - duration1 + duration;
    }
    if (paralyze != new TimeSpan())
    {
      this._stun.TryParalyze(acided.Owner, paralyze, true);
      acided.Comp.ResistsNeeded = resists;
    }
    this.Dirty<UserAcidedComponent>(acided);
    this.UpdateAppearance((Entity<UserAcidedComponent>) ((EntityUid) acided, acided.Comp));
  }

  private void UpdateAppearance(Entity<UserAcidedComponent> acided)
  {
    UserAcidedEffects userAcidedEffects = acided.Comp.Combo ? UserAcidedEffects.Enhanced : UserAcidedEffects.Normal;
    this._appearance.SetData((EntityUid) acided, (Enum) UserAcidedVisuals.Acided, (object) userAcidedEffects);
  }

  public void Resist(Entity<UserAcidedComponent?> player)
  {
    if (!this.Resolve<UserAcidedComponent>((EntityUid) player, ref player.Comp, false) || !this._actionBlocker.CanInteract((EntityUid) player, new EntityUid?()))
      return;
    this._stun.TryParalyze(player.Owner, player.Comp.ResistDuration, true);
    if (--player.Comp.ResistsNeeded <= 0)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-acid-resist"), (EntityUid) player, (EntityUid) player);
      this.RemCompDeferred<UserAcidedComponent>((EntityUid) player);
    }
    else
      this._popup.PopupEntity(this.Loc.GetString("rmc-acid-resist-partial"), (EntityUid) player, (EntityUid) player);
  }

  private void ApplyAcidStacks(
    EntityUid target,
    int amount,
    int max,
    DamageSpecifier? damage,
    EntityWhitelist? whitelist)
  {
    if (!this._entityWhitelist.IsWhitelistPassOrNull(whitelist, target) || this._mobState.IsDead(target))
      return;
    VictimXenoAcidStacksComponent acidStacksComponent = this.EnsureComp<VictimXenoAcidStacksComponent>(target);
    acidStacksComponent.Current = Math.Min(max, acidStacksComponent.Current + amount);
    acidStacksComponent.LastIncrement = this._timing.CurTime;
    this.Dirty(target, (IComponent) acidStacksComponent);
    if (acidStacksComponent.Current < max)
      return;
    if (damage != null)
    {
      this._damageable.TryChangeDamage(new EntityUid?(target), damage);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-praetorian-acid-spit-hit-self"), target, target, PopupType.SmallCaution);
    }
    this.RemCompDeferred<VictimXenoAcidStacksComponent>(target);
  }

  private void OnDrainOnHitProjectileHit(
    Entity<DrainOnHitComponent> spit,
    ref ProjectileHitEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid target = args.Target;
    Entity<SolutionComponent>? entity;
    Solution solution;
    if (this._hive.FromSameHive((Entity<HiveMemberComponent>) spit.Owner, (Entity<HiveMemberComponent>) target) || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) target, spit.Comp.TargetSolution, out entity, out solution) || solution == null || !entity.HasValue)
      return;
    foreach (ReagentPrototype key in solution.GetReagentPrototypes(this._prototypeManager).Keys)
    {
      if (key.Group == spit.Comp.DrainGroup)
        this._solution.RemoveReagent(entity.Value, key.ID, spit.Comp.DrainAmount);
    }
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoActiveChargingSpitComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoActiveChargingSpitComponent>();
    EntityUid uid1;
    XenoActiveChargingSpitComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(comp1_1.ExpiresAt > curTime))
      {
        this.RemCompDeferred<XenoActiveChargingSpitComponent>(uid1);
        if (!comp1_1.DidPopup)
        {
          comp1_1.DidPopup = true;
          this._popup.PopupClient(this.Loc.GetString("cm-xeno-charge-spit-expire"), uid1, new EntityUid?(uid1), PopupType.SmallCaution);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<UserAcidedComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<UserAcidedComponent>();
    EntityUid uid2;
    UserAcidedComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (curTime >= comp1_2.ExpiresAt)
        this.RemCompDeferred<UserAcidedComponent>(uid2);
      else if (!(curTime < comp1_2.NextDamageAt))
      {
        comp1_2.NextDamageAt = curTime + comp1_2.DamageEvery;
        DamageableSystem damageable = this._damageable;
        EntityUid? uid3 = new EntityUid?(uid2);
        DamageSpecifier damage = comp1_2.Damage;
        int armorPiercing1 = comp1_2.ArmorPiercing;
        EntityUid? origin = new EntityUid?();
        EntityUid? tool = new EntityUid?();
        int armorPiercing2 = armorPiercing1;
        damageable.TryChangeDamage(uid3, damage, origin: origin, tool: tool, armorPiercing: armorPiercing2);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<VictimXenoAcidStacksComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<VictimXenoAcidStacksComponent>();
    EntityUid uid4;
    VictimXenoAcidStacksComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid4, out comp1_3))
    {
      if (!(curTime < comp1_3.LastDecrement + comp1_3.DecrementEvery) && !(curTime < comp1_3.LastIncrement + comp1_3.IncrementFor))
      {
        --comp1_3.Current;
        comp1_3.LastDecrement = this._timing.CurTime;
        this.Dirty(uid4, (IComponent) comp1_3);
        if (comp1_3.Current <= 0)
          this.RemCompDeferred<VictimXenoAcidStacksComponent>(uid4);
      }
    }
  }
}
