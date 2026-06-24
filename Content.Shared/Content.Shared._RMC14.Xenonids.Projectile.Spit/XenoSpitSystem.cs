using System;
using System.Collections.Generic;
using System.Numerics;
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
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

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

	private static readonly ProtoId<ReagentPrototype> AcidRemovedBy = ProtoId<ReagentPrototype>.op_Implicit("Water");

	private EntityQuery<ProjectileComponent> _projectileQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoSpitComponent, XenoSpitActionEvent>((EntityEventRefHandler<XenoSpitComponent, XenoSpitActionEvent>)OnXenoSpitAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSlowingSpitComponent, XenoSlowingSpitActionEvent>((EntityEventRefHandler<XenoSlowingSpitComponent, XenoSlowingSpitActionEvent>)OnXenoSlowingSpitAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoScatteredSpitComponent, XenoScatteredSpitActionEvent>((EntityEventRefHandler<XenoScatteredSpitComponent, XenoScatteredSpitActionEvent>)OnXenoScatteredSpitAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoChargeSpitComponent, XenoChargeSpitActionEvent>((EntityEventRefHandler<XenoChargeSpitComponent, XenoChargeSpitActionEvent>)OnXenoChargeSpitAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveChargingSpitComponent, ComponentStartup>((EntityEventRefHandler<XenoActiveChargingSpitComponent, ComponentStartup>)OnActiveChargingSpitAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveChargingSpitComponent, ComponentRemove>((EntityEventRefHandler<XenoActiveChargingSpitComponent, ComponentRemove>)OnActiveChargingSpitRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveChargingSpitComponent, CMGetArmorEvent>((EntityEventRefHandler<XenoActiveChargingSpitComponent, CMGetArmorEvent>)OnActiveChargingSpitGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveChargingSpitComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoActiveChargingSpitComponent, RefreshMovementSpeedModifiersEvent>)OnActiveChargingSpitRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveChargingSpitComponent, XenoGetSpitProjectileEvent>((EntityEventRefHandler<XenoActiveChargingSpitComponent, XenoGetSpitProjectileEvent>)OnActiveChargingSpitGetProjectile, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSlowingSpitProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<XenoSlowingSpitProjectileComponent, ProjectileHitEvent>)OnXenoSlowingSpitHit, (Type[])null, new Type[1] { typeof(CMClusterGrenadeSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidBallComponent, XenoAcidBallActionEvent>((EntityEventRefHandler<XenoAcidBallComponent, XenoAcidBallActionEvent>)OnXenoAcidBallAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidBallComponent, XenoAcidBallDoAfterEvent>((EntityEventRefHandler<XenoAcidBallComponent, XenoAcidBallDoAfterEvent>)OnXenoAcidBallDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ApplyAcidStacksComponent, ProjectileHitEvent>((EntityEventRefHandler<ApplyAcidStacksComponent, ProjectileHitEvent>)OnApplyAcidStacksProjectileHit, (Type[])null, new Type[1] { typeof(CMClusterGrenadeSystem) });
		((EntitySystem)this).SubscribeLocalEvent<ApplyAcidStacksComponent, DamageCollideEvent>((EntityEventRefHandler<ApplyAcidStacksComponent, DamageCollideEvent>)OnApplyAcidStacksDamageCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShieldOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<XenoProjectileShieldOnHitComponent, ProjectileHitEvent>)OnShieldOnHit, (Type[])null, new Type[1] { typeof(CMClusterGrenadeSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoProjectileShieldOnHitComponent, CMClusterSpawnedEvent>((EntityEventRefHandler<XenoProjectileShieldOnHitComponent, CMClusterSpawnedEvent>)OnShieldOnHitClusterSpawned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserAcidedComponent, MapInitEvent>((EntityEventRefHandler<UserAcidedComponent, MapInitEvent>)OnUserAcidedMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserAcidedComponent, ComponentRemove>((EntityEventRefHandler<UserAcidedComponent, ComponentRemove>)OnUserAcidedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserAcidedComponent, ShowFireAlertEvent>((EntityEventRefHandler<UserAcidedComponent, ShowFireAlertEvent>)OnUserAcidedShowFireAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserAcidedComponent, VaporHitEvent>((EntityEventRefHandler<UserAcidedComponent, VaporHitEvent>)OnUserAcidedVaporHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserAcidedComponent, MobStateChangedEvent>((EntityEventRefHandler<UserAcidedComponent, MobStateChangedEvent>)OnUserAcidedMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, HitBySlowingSpitEvent>((EntityEventRefHandler<InventoryComponent, HitBySlowingSpitEvent>)_inventory.RelayEvent<HitBySlowingSpitEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrainOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<DrainOnHitComponent, ProjectileHitEvent>)OnDrainOnHitProjectileHit, (Type[])null, new Type[1] { typeof(CMClusterGrenadeSystem) });
	}

	private void OnActiveChargingSpitRemove(Entity<XenoActiveChargingSpitComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoActiveChargingSpitComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoActiveChargingSpitComponent>.op_Implicit(ent));
			_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((ValueTuple<EntityUid, CMArmorComponent>)(Entity<XenoActiveChargingSpitComponent>.op_Implicit(ent), null)));
		}
	}

	private void OnActiveChargingSpitAdded(Entity<XenoActiveChargingSpitComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((ValueTuple<EntityUid, CMArmorComponent>)(Entity<XenoActiveChargingSpitComponent>.op_Implicit(ent), null)));
	}

	private void OnActiveChargingSpitGetArmor(Entity<XenoActiveChargingSpitComponent> ent, ref CMGetArmorEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.XenoArmor += ent.Comp.Armor;
	}

	private void OnActiveChargingSpitRefreshSpeed(Entity<XenoActiveChargingSpitComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.ModifySpeed(ent.Comp.Speed, ent.Comp.Speed);
	}

	private void OnActiveChargingSpitGetProjectile(Entity<XenoActiveChargingSpitComponent> ent, ref XenoGetSpitProjectileEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.FiredProjectile)
		{
			args.Id = ent.Comp.Projectile;
		}
	}

	private void OnXenoSpitAction(Entity<XenoSpitComponent> xeno, ref XenoSpitActionEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _rmcActions.TryUseAction(args))
		{
			XenoGetSpitProjectileEvent ev = new XenoGetSpitProjectileEvent(xeno.Comp.ProjectileId);
			((EntitySystem)this).RaiseLocalEvent<XenoGetSpitProjectileEvent>(Entity<XenoSpitComponent>.op_Implicit(xeno), ref ev, false);
			XenoSpitActionEvent obj = args;
			XenoProjectileSystem xenoProjectile = _xenoProjectile;
			EntityUid xeno2 = Entity<XenoSpitComponent>.op_Implicit(xeno);
			EntityCoordinates target = args.Target;
			FixedPoint2 plasmaCost = xeno.Comp.PlasmaCost;
			EntProtoId id = ev.Id;
			SoundSpecifier sound = xeno.Comp.Sound;
			Angle zero = Angle.Zero;
			float speed = xeno.Comp.Speed;
			EntityUid? entity = args.Entity;
			((HandledEntityEventArgs)obj).Handled = xenoProjectile.TryShoot(xeno2, target, plasmaCost, id, sound, 1, zero, speed, null, entity);
			XenoActiveChargingSpitComponent active = default(XenoActiveChargingSpitComponent);
			if (((EntitySystem)this).TryComp<XenoActiveChargingSpitComponent>(Entity<XenoSpitComponent>.op_Implicit(xeno), ref active))
			{
				active.FiredProjectile = true;
				((EntitySystem)this).Dirty(Entity<XenoSpitComponent>.op_Implicit(xeno), (IComponent)(object)active, (MetaDataComponent)null);
				_popup.PopupClient(base.Loc.GetString("cm-xeno-charge-spit-expire"), Entity<XenoSpitComponent>.op_Implicit(xeno), Entity<XenoSpitComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			}
		}
	}

	private void OnXenoSlowingSpitAction(Entity<XenoSlowingSpitComponent> xeno, ref XenoSlowingSpitActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			XenoSlowingSpitActionEvent obj = args;
			XenoProjectileSystem xenoProjectile = _xenoProjectile;
			EntityUid xeno2 = Entity<XenoSlowingSpitComponent>.op_Implicit(xeno);
			EntityCoordinates target = args.Target;
			FixedPoint2 plasmaCost = xeno.Comp.PlasmaCost;
			EntProtoId projectileId = xeno.Comp.ProjectileId;
			SoundSpecifier sound = xeno.Comp.Sound;
			Angle zero = Angle.Zero;
			float speed = xeno.Comp.Speed;
			EntityUid? entity = args.Entity;
			((HandledEntityEventArgs)obj).Handled = xenoProjectile.TryShoot(xeno2, target, plasmaCost, projectileId, sound, 1, zero, speed, null, entity);
		}
	}

	private void OnXenoScatteredSpitAction(Entity<XenoScatteredSpitComponent> xeno, ref XenoScatteredSpitActionEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _rmcActions.TryUseAction(args))
		{
			XenoScatteredSpitActionEvent obj = args;
			XenoProjectileSystem xenoProjectile = _xenoProjectile;
			EntityUid xeno2 = Entity<XenoScatteredSpitComponent>.op_Implicit(xeno);
			EntityCoordinates target = args.Target;
			FixedPoint2 plasmaCost = xeno.Comp.PlasmaCost;
			EntProtoId projectileId = xeno.Comp.ProjectileId;
			SoundSpecifier sound = xeno.Comp.Sound;
			int maxProjectiles = xeno.Comp.MaxProjectiles;
			Angle maxDeviation = xeno.Comp.MaxDeviation;
			float speed = xeno.Comp.Speed;
			EntityUid? entity = args.Entity;
			((HandledEntityEventArgs)obj).Handled = xenoProjectile.TryShoot(xeno2, target, plasmaCost, projectileId, sound, maxProjectiles, maxDeviation, speed, null, entity);
		}
	}

	private void OnXenoChargeSpitAction(Entity<XenoChargeSpitComponent> xeno, ref XenoChargeSpitActionEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _rmcActions.TryUseAction(args))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoActiveChargingSpitComponent charging = ((EntitySystem)this).EnsureComp<XenoActiveChargingSpitComponent>(Entity<XenoChargeSpitComponent>.op_Implicit(xeno));
			charging.ExpiresAt = _timing.CurTime + xeno.Comp.Duration;
			charging.Armor = xeno.Comp.Armor;
			charging.Speed = xeno.Comp.Speed;
			((EntitySystem)this).Dirty(Entity<XenoChargeSpitComponent>.op_Implicit(xeno), (IComponent)(object)charging, (MetaDataComponent)null);
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoChargeSpitComponent>.op_Implicit(xeno));
			_popup.PopupClient(base.Loc.GetString("cm-xeno-charge-spit"), Entity<XenoChargeSpitComponent>.op_Implicit(xeno), Entity<XenoChargeSpitComponent>.op_Implicit(xeno));
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
	}

	private void OnXenoSlowingSpitHit(Entity<XenoSlowingSpitProjectileComponent> spit, ref ProjectileHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(spit.Owner), Entity<HiveMemberComponent>.op_Implicit(target)) || ((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			((EntitySystem)this).PredictedQueueDel(spit.Owner);
			return;
		}
		if (((EntitySystem)this).HasComp<SynthComponent>(target))
		{
			string immuneMsg = base.Loc.GetString("cm-xeno-paralyzing-slash-immune", (ValueTuple<string, object>)("target", target));
			_popup.PopupEntity(immuneMsg, target, target, PopupType.SmallCaution);
			return;
		}
		Filter filter = Filter.Pvs(target, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
		XenoProjectileShotComponent shot = default(XenoProjectileShotComponent);
		if (((EntitySystem)this).TryComp<XenoProjectileShotComponent>(Entity<XenoSlowingSpitProjectileComponent>.op_Implicit(spit), ref shot))
		{
			ICommonSession shooter = shot.Shooter;
			if (shooter != null)
			{
				filter = filter.RemovePlayer(shooter);
			}
		}
		_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { target }, filter);
		if (_net.IsClient)
		{
			return;
		}
		if (spit.Comp.Slow > TimeSpan.Zero)
		{
			if (spit.Comp.SuperSlow)
			{
				_slow.TrySuperSlowdown(target, spit.Comp.Slow);
			}
			else
			{
				_slow.TrySlowdown(target, spit.Comp.Slow);
			}
		}
		bool resisted = false;
		if (spit.Comp.ArmorResistsKnockdown)
		{
			HitBySlowingSpitEvent ev = new HitBySlowingSpitEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
			((EntitySystem)this).RaiseLocalEvent<HitBySlowingSpitEvent>(args.Target, ref ev, false);
			resisted = ev.Cancelled;
		}
		if (!resisted)
		{
			_stun.TryParalyze(target, spit.Comp.Paralyze, refresh: true);
		}
	}

	private void OnXenoAcidBallAction(Entity<XenoAcidBallComponent> ent, ref XenoAcidBallActionEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			XenoAcidBallDoAfterEvent ev = new XenoAcidBallDoAfterEvent(((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null));
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoAcidBallComponent>.op_Implicit(ent), ent.Comp.Delay, ev, Entity<XenoAcidBallComponent>.op_Implicit(ent))
			{
				BreakOnMove = true,
				RootEntity = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnXenoAcidBallDoAfter(Entity<XenoAcidBallComponent> ent, ref XenoAcidBallDoAfterEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityCoordinates target = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		MapCoordinates targetMap = _transform.ToMapCoordinates(target, true);
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoAcidBallComponent>.op_Implicit(ent), (TransformComponent)null);
		if (origin.MapId != targetMap.MapId)
		{
			return;
		}
		Vector2 direction = targetMap.Position - origin.Position;
		float distance = Math.Min(ent.Comp.MaxRange, direction.Length());
		((HandledEntityEventArgs)args).Handled = _xenoProjectile.TryShoot(Entity<XenoAcidBallComponent>.op_Implicit(ent), target, ent.Comp.PlasmaCost, ent.Comp.ProjectileId, null, 1, Angle.Zero, ent.Comp.Speed, distance);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoAcidBallActionEvent>(Entity<XenoAcidBallComponent>.op_Implicit(ent)))
		{
			_actions.SetCooldown(action.AsNullable(), ent.Comp.Cooldown);
		}
		if (((HandledEntityEventArgs)args).Handled)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-acid-ball-shoot-self"), Entity<XenoAcidBallComponent>.op_Implicit(ent), Entity<XenoAcidBallComponent>.op_Implicit(ent));
		}
	}

	private void OnApplyAcidStacksProjectileHit(Entity<ApplyAcidStacksComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			ApplyAcidStacks(args.Target, ent.Comp.Amount, ent.Comp.Max, ent.Comp.Damage, ent.Comp.Whitelist);
		}
	}

	private void OnApplyAcidStacksDamageCollide(Entity<ApplyAcidStacksComponent> ent, ref DamageCollideEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ApplyAcidStacks(args.Target, ent.Comp.Amount, ent.Comp.Max, ent.Comp.Damage, ent.Comp.Whitelist);
	}

	private void OnShieldOnHit(Entity<XenoProjectileShieldOnHitComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectile = default(ProjectileComponent);
		if (!_projectileQuery.TryComp(Entity<XenoProjectileShieldOnHitComponent>.op_Implicit(ent), ref projectile))
		{
			return;
		}
		EntityUid? shooter = projectile.Shooter;
		if (shooter.HasValue)
		{
			EntityUid shooter2 = shooter.GetValueOrDefault();
			if (((EntityUid)(ref shooter2)).Valid && _xeno.CanAbilityAttackTarget(shooter2, args.Target))
			{
				XenoShieldSystem xenoShield = _xenoShield;
				EntityUid uid = shooter2;
				XenoShieldSystem.ShieldType shield = ent.Comp.Shield;
				FixedPoint2 amount = ent.Comp.Amount;
				double maxShield = ent.Comp.Max.Double();
				xenoShield.ApplyShield(uid, shield, amount, null, 0.0, addShield: true, maxShield);
			}
		}
	}

	private void OnShieldOnHitClusterSpawned(Entity<XenoProjectileShieldOnHitComponent> ent, ref CMClusterSpawnedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? shooter = _projectileQuery.CompOrNull(Entity<XenoProjectileShieldOnHitComponent>.op_Implicit(ent))?.Shooter;
		foreach (EntityUid spawned in args.Spawned)
		{
			((EntitySystem)this).EnsureComp<XenoProjectileShieldOnHitComponent>(spawned);
			if (shooter.HasValue)
			{
				ProjectileComponent projectile = ((EntitySystem)this).EnsureComp<ProjectileComponent>(spawned);
				projectile.Shooter = shooter;
				((EntitySystem)this).Dirty(spawned, (IComponent)(object)projectile, (MetaDataComponent)null);
			}
		}
	}

	private void OnUserAcidedMapInit(Entity<UserAcidedComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ExpiresAt = _timing.CurTime + ent.Comp.Duration;
		((EntitySystem)this).Dirty<UserAcidedComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
	}

	private void OnUserAcidedRemove(Entity<UserAcidedComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<UserAcidedComponent>.op_Implicit(ent), (Enum)UserAcidedVisuals.Acided, (object)UserAcidedEffects.None, (AppearanceComponent)null);
	}

	private void OnUserAcidedShowFireAlert(Entity<UserAcidedComponent> ent, ref ShowFireAlertEvent args)
	{
		args.Show = true;
	}

	private void OnUserAcidedVaporHit(Entity<UserAcidedComponent> ent, ref VaporHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.AllowVaporHitAfter > _timing.CurTime)
		{
			return;
		}
		Entity<SolutionContainerManagerComponent> solEnt = args.Solution;
		foreach (var item in _solution.EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((Entity<SolutionContainerManagerComponent>.op_Implicit(solEnt), Entity<SolutionContainerManagerComponent>.op_Implicit(solEnt)))))
		{
			if (item.Solution.Comp.Solution.ContainsReagent(ProtoId<ReagentPrototype>.op_Implicit(AcidRemovedBy), null))
			{
				if (--ent.Comp.ResistsNeeded <= 0)
				{
					((EntitySystem)this).RemCompDeferred<UserAcidedComponent>(Entity<UserAcidedComponent>.op_Implicit(ent));
					break;
				}
				ent.Comp.AllowVaporHitAfter = _timing.CurTime + ent.Comp.ExtinguishGracePeriod;
				((EntitySystem)this).Dirty<UserAcidedComponent>(ent, (MetaDataComponent)null);
				break;
			}
		}
	}

	private void OnUserAcidedMobStateChanged(Entity<UserAcidedComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			((EntitySystem)this).RemCompDeferred<UserAcidedComponent>(Entity<UserAcidedComponent>.op_Implicit(ent));
		}
	}

	public void SetAcidCombo(Entity<UserAcidedComponent?> acided, TimeSpan duration, DamageSpecifier? damage, TimeSpan paralyze, int resists)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<UserAcidedComponent>(Entity<UserAcidedComponent>.op_Implicit(acided), ref acided.Comp, false) && !acided.Comp.Combo)
		{
			acided.Comp.Combo = true;
			if (damage != null)
			{
				acided.Comp.Damage = damage;
			}
			if (duration != default(TimeSpan))
			{
				TimeSpan oldDuration = acided.Comp.Duration;
				acided.Comp.Duration = duration;
				acided.Comp.ExpiresAt = acided.Comp.ExpiresAt - oldDuration + duration;
			}
			if (paralyze != default(TimeSpan))
			{
				_stun.TryParalyze(acided.Owner, paralyze, refresh: true);
				acided.Comp.ResistsNeeded = resists;
			}
			((EntitySystem)this).Dirty<UserAcidedComponent>(acided, (MetaDataComponent)null);
			UpdateAppearance(Entity<UserAcidedComponent>.op_Implicit((Entity<UserAcidedComponent>.op_Implicit(acided), acided.Comp)));
		}
	}

	private void UpdateAppearance(Entity<UserAcidedComponent> acided)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		UserAcidedEffects effect = ((!acided.Comp.Combo) ? UserAcidedEffects.Normal : UserAcidedEffects.Enhanced);
		_appearance.SetData(Entity<UserAcidedComponent>.op_Implicit(acided), (Enum)UserAcidedVisuals.Acided, (object)effect, (AppearanceComponent)null);
	}

	public void Resist(Entity<UserAcidedComponent?> player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<UserAcidedComponent>(Entity<UserAcidedComponent>.op_Implicit(player), ref player.Comp, false) && _actionBlocker.CanInteract(Entity<UserAcidedComponent>.op_Implicit(player), null))
		{
			_stun.TryParalyze(player.Owner, player.Comp.ResistDuration, refresh: true);
			if (--player.Comp.ResistsNeeded <= 0)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-acid-resist"), Entity<UserAcidedComponent>.op_Implicit(player), Entity<UserAcidedComponent>.op_Implicit(player));
				((EntitySystem)this).RemCompDeferred<UserAcidedComponent>(Entity<UserAcidedComponent>.op_Implicit(player));
			}
			else
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-acid-resist-partial"), Entity<UserAcidedComponent>.op_Implicit(player), Entity<UserAcidedComponent>.op_Implicit(player));
			}
		}
	}

	private void ApplyAcidStacks(EntityUid target, int amount, int max, DamageSpecifier? damage, EntityWhitelist? whitelist)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!_entityWhitelist.IsWhitelistPassOrNull(whitelist, target) || _mobState.IsDead(target))
		{
			return;
		}
		VictimXenoAcidStacksComponent victim = ((EntitySystem)this).EnsureComp<VictimXenoAcidStacksComponent>(target);
		victim.Current = Math.Min(max, victim.Current + amount);
		victim.LastIncrement = _timing.CurTime;
		((EntitySystem)this).Dirty(target, (IComponent)(object)victim, (MetaDataComponent)null);
		if (victim.Current >= max)
		{
			if (damage != null)
			{
				_damageable.TryChangeDamage(target, damage);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-praetorian-acid-spit-hit-self"), target, target, PopupType.SmallCaution);
			}
			((EntitySystem)this).RemCompDeferred<VictimXenoAcidStacksComponent>(target);
		}
	}

	private void OnDrainOnHitProjectileHit(Entity<DrainOnHitComponent> spit, ref ProjectileHitEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid target = args.Target;
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(spit.Owner), Entity<HiveMemberComponent>.op_Implicit(target)) || !_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(target), spit.Comp.TargetSolution, out Entity<SolutionComponent>? solEnt, out Solution solu) || solu == null || !solEnt.HasValue)
		{
			return;
		}
		foreach (ReagentPrototype chemical in solu.GetReagentPrototypes(_prototypeManager).Keys)
		{
			if (chemical.Group == spit.Comp.DrainGroup)
			{
				_solution.RemoveReagent(solEnt.Value, chemical.ID, spit.Comp.DrainAmount);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoActiveChargingSpitComponent> chargingQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoActiveChargingSpitComponent>();
		EntityUid uid = default(EntityUid);
		XenoActiveChargingSpitComponent charging = default(XenoActiveChargingSpitComponent);
		while (chargingQuery.MoveNext(ref uid, ref charging))
		{
			if (!(charging.ExpiresAt > time))
			{
				((EntitySystem)this).RemCompDeferred<XenoActiveChargingSpitComponent>(uid);
				if (!charging.DidPopup)
				{
					charging.DidPopup = true;
					_popup.PopupClient(base.Loc.GetString("cm-xeno-charge-spit-expire"), uid, uid, PopupType.SmallCaution);
				}
			}
		}
		EntityQueryEnumerator<UserAcidedComponent> acidedQuery = ((EntitySystem)this).EntityQueryEnumerator<UserAcidedComponent>();
		EntityUid uid2 = default(EntityUid);
		UserAcidedComponent acided = default(UserAcidedComponent);
		while (acidedQuery.MoveNext(ref uid2, ref acided))
		{
			if (time >= acided.ExpiresAt)
			{
				((EntitySystem)this).RemCompDeferred<UserAcidedComponent>(uid2);
			}
			else if (!(time < acided.NextDamageAt))
			{
				acided.NextDamageAt = time + acided.DamageEvery;
				DamageableSystem damageable = _damageable;
				EntityUid? uid3 = uid2;
				DamageSpecifier damage = acided.Damage;
				int armorPiercing = acided.ArmorPiercing;
				damageable.TryChangeDamage(uid3, damage, ignoreResistances: false, interruptsDoAfters: true, null, null, null, armorPiercing);
			}
		}
		EntityQueryEnumerator<VictimXenoAcidStacksComponent> stacks = ((EntitySystem)this).EntityQueryEnumerator<VictimXenoAcidStacksComponent>();
		EntityUid uid4 = default(EntityUid);
		VictimXenoAcidStacksComponent victim = default(VictimXenoAcidStacksComponent);
		while (stacks.MoveNext(ref uid4, ref victim))
		{
			if (!(time < victim.LastDecrement + victim.DecrementEvery) && !(time < victim.LastIncrement + victim.IncrementFor))
			{
				victim.Current--;
				victim.LastDecrement = _timing.CurTime;
				((EntitySystem)this).Dirty(uid4, (IComponent)(object)victim, (MetaDataComponent)null);
				if (victim.Current <= 0)
				{
					((EntitySystem)this).RemCompDeferred<VictimXenoAcidStacksComponent>(uid4);
				}
			}
		}
	}
}
