using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Projectiles.Aimed;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared._RMC14.Targeting;
using Content.Shared._RMC14.Weapons.Ranged.Homing;
using Content.Shared._RMC14.Weapons.Ranged.Laser;
using Content.Shared._RMC14.Weapons.Ranged.Whitelist;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.CombatMode;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot;

public abstract class SharedRMCAimedShotSystem : EntitySystem
{
	[Dependency]
	private SharedRMCTargetingSystem _targeting;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedGunSystem _gunSystem;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private SharedCombatModeSystem _combatMode;

	[Dependency]
	protected IGameTiming Timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AimedShotComponent, GetItemActionsEvent>((EntityEventRefHandler<AimedShotComponent, GetItemActionsEvent>)OnAimedShotGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AimedShotComponent, AimedShotActionEvent>((EntityEventRefHandler<AimedShotComponent, AimedShotActionEvent>)OnAimedShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AimedShotComponent, TargetingFinishedEvent>((EntityEventRefHandler<AimedShotComponent, TargetingFinishedEvent>)OnTargetingFinished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AimedShotComponent, TargetingCancelledEvent>((EntityEventRefHandler<AimedShotComponent, TargetingCancelledEvent>)OnTargetingCancelled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AimedShotComponent, AmmoShotEvent>((EntityEventRefHandler<AimedShotComponent, AmmoShotEvent>)OnAmmoShot, (Type[])null, (Type[])null);
	}

	private void OnAimedShotGetItemActions(Entity<AimedShotComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		GunUserWhitelistComponent whitelist = default(GunUserWhitelistComponent);
		if (((EntitySystem)this).TryComp<GunUserWhitelistComponent>(args.Provider, ref whitelist))
		{
			ent.Comp.Whitelist = whitelist.Whitelist;
		}
		else
		{
			ent.Comp.Whitelist = new EntityWhitelist();
		}
		((EntitySystem)this).Dirty<AimedShotComponent>(ent, (MetaDataComponent)null);
	}

	protected void AimedShotRequested(NetEntity netGun, NetEntity netUser, NetEntity netTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gun = ((EntitySystem)this).GetEntity(netGun);
		EntityUid user = ((EntitySystem)this).GetEntity(netUser);
		EntityUid target = ((EntitySystem)this).GetEntity(netTarget);
		AimedShotComponent aimedShot = default(AimedShotComponent);
		if (!((EntitySystem)this).TryComp<AimedShotComponent>(gun, ref aimedShot) || !aimedShot.Activated || !CanAimShot(Entity<AimedShotComponent>.op_Implicit((gun, aimedShot)), target, user))
		{
			return;
		}
		float laserDuration = (float)((double)aimedShot.AimDuration + (double)(_transform.GetMoverCoordinates(target).Position - _transform.GetMoverCoordinates(user).Position).Length() * aimedShot.AimDistanceDifficulty);
		bool appliedSpotterBuff = false;
		float aimMultiplier = 1f;
		TargetedEffects targetEffect = aimedShot.TargetEffect;
		DirectionTargetedEffects directionEffect = aimedShot.DirectionTargetEffect;
		SpottedComponent spotted = default(SpottedComponent);
		if (((EntitySystem)this).TryComp<SpottedComponent>(target, ref spotted))
		{
			aimMultiplier = spotted.AimDurationMultiplier;
			appliedSpotterBuff = true;
		}
		GunToggleableLaserComponent toggleLaser = default(GunToggleableLaserComponent);
		if (((EntitySystem)this).TryComp<GunToggleableLaserComponent>(gun, ref toggleLaser))
		{
			if (toggleLaser.Active)
			{
				aimMultiplier = ((!appliedSpotterBuff) ? toggleLaser.AimDurationMultiplier : (aimMultiplier - toggleLaser.SpottedAimDurationMultiplierSubtraction));
				directionEffect = DirectionTargetedEffects.None;
			}
			TargetingLaserComponent targetingLaser = default(TargetingLaserComponent);
			if (((EntitySystem)this).TryComp<TargetingLaserComponent>(gun, ref targetingLaser))
			{
				targetingLaser.ShowLaser = toggleLaser.Active;
				((EntitySystem)this).Dirty(gun, (IComponent)(object)targetingLaser, (MetaDataComponent)null);
			}
		}
		laserDuration *= aimMultiplier;
		aimedShot.Targets.Add(target);
		aimedShot.NextAimedShot = Timing.CurTime + aimedShot.AimedShotCooldown;
		((EntitySystem)this).Dirty(gun, (IComponent)(object)aimedShot, (MetaDataComponent)null);
		_audio.PlayPredicted(aimedShot.AimingSound, gun, (EntityUid?)user, (AudioParams?)null);
		_targeting.Target(gun, user, target, laserDuration, targetEffect, directionEffect);
	}

	private void OnAimedShot(Entity<AimedShotComponent> ent, ref AimedShotActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Activated = !ent.Comp.Activated;
		((EntitySystem)this).Dirty<AimedShotComponent>(ent, (MetaDataComponent)null);
		SharedActionsSystem actions = _actions;
		EntityUid? action = ent.Comp.Action;
		actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), ent.Comp.Activated);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnAmmoShot(Entity<AimedShotComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CurrentTarget.HasValue || ((EntitySystem)this).TerminatingOrDeleted(ent.Comp.CurrentTarget, (MetaDataComponent)null))
		{
			return;
		}
		EntityUid target = ent.Comp.CurrentTarget.Value;
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			AimedShotEvent ev = new AimedShotEvent(target);
			((EntitySystem)this).RaiseLocalEvent<AimedShotEvent>(Entity<AimedShotComponent>.op_Implicit(ent), ref ev, false);
			AimedProjectileComponent aimedProjectile = ((EntitySystem)this).EnsureComp<AimedProjectileComponent>(projectile);
			aimedProjectile.Target = target;
			aimedProjectile.Source = Entity<AimedShotComponent>.op_Implicit(ent);
			((EntitySystem)this).Dirty(projectile, (IComponent)(object)aimedProjectile, (MetaDataComponent)null);
			HomingProjectileComponent homingProjectile = ((EntitySystem)this).EnsureComp<HomingProjectileComponent>(projectile);
			homingProjectile.Target = target;
			homingProjectile.ProjectileSpeed = ent.Comp.ProjectileSpeed;
			((EntitySystem)this).Dirty(projectile, (IComponent)(object)homingProjectile, (MetaDataComponent)null);
			TargetedProjectileComponent targeted = ((EntitySystem)this).EnsureComp<TargetedProjectileComponent>(projectile);
			targeted.Target = target;
			((EntitySystem)this).Dirty(projectile, (IComponent)(object)targeted, (MetaDataComponent)null);
			ShotByAimedShotEvent ev2 = new ShotByAimedShotEvent(Entity<AimedShotComponent>.op_Implicit(ent), target);
			((EntitySystem)this).RaiseLocalEvent<ShotByAimedShotEvent>(projectile, ref ev2, false);
		}
		RemoveTarget(ent, target);
	}

	private void OnTargetingFinished(Entity<AimedShotComponent> ent, ref TargetingFinishedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		GunComponent gun = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(Entity<AimedShotComponent>.op_Implicit(ent), ref gun))
		{
			return;
		}
		if (!_examine.InRangeUnOccluded(args.User, args.Target, ent.Comp.Range))
		{
			RemoveTarget(ent, args.Target);
			string message = base.Loc.GetString("rmc-action-popup-aiming-target-blocked", (ValueTuple<string, object>)("gun", ent));
			_popup.PopupClient(message, args.User, args.User);
			return;
		}
		ent.Comp.CurrentTarget = args.Target;
		((EntitySystem)this).Dirty<AimedShotComponent>(ent, (MetaDataComponent)null);
		if (_net.IsServer)
		{
			List<EntityUid>? list = _gunSystem.AttemptShoot(Entity<GunComponent>.op_Implicit((Entity<AimedShotComponent>.op_Implicit(ent), gun)), args.User, args.Coordinates);
			GetAmmoCountEvent ammoCount = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(Entity<AimedShotComponent>.op_Implicit(ent), ref ammoCount, false);
			if (list != null)
			{
				_audio.PlayEntity(gun.SoundGunshotModified, args.User, Entity<AimedShotComponent>.op_Implicit(ent), (AudioParams?)null);
				if (ammoCount.Count == 0)
				{
					_audio.PlayPvs((SoundSpecifier)new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg", (AudioParams?)null), Entity<AimedShotComponent>.op_Implicit(ent), (AudioParams?)null);
				}
			}
			else
			{
				RemoveTarget(ent, args.Target);
				if (ammoCount.Count < 0)
				{
					_audio.PlayEntity(gun.SoundEmpty, args.User, Entity<AimedShotComponent>.op_Implicit(ent), (AudioParams?)null);
				}
			}
		}
		UpdateClientAmmoEvent ev = default(UpdateClientAmmoEvent);
		((EntitySystem)this).RaiseLocalEvent<UpdateClientAmmoEvent>(Entity<AimedShotComponent>.op_Implicit(ent), ref ev, false);
	}

	private void OnTargetingCancelled(Entity<AimedShotComponent> ent, ref TargetingCancelledEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			ent.Comp.Targets.Clear();
			((EntitySystem)this).Dirty<AimedShotComponent>(ent, (MetaDataComponent)null);
			args.Handled = true;
		}
	}

	private bool CanAimShot(Entity<AimedShotComponent> ent, EntityUid target, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<SpottableComponent>(target))
		{
			return false;
		}
		if (!_examine.InRangeUnOccluded(user, target, ent.Comp.Range))
		{
			return false;
		}
		if (ent.Comp.NextAimedShot > Timing.CurTime)
		{
			return false;
		}
		if (!_combatMode.IsInCombatMode(user))
		{
			return false;
		}
		if (!_whitelist.IsValid(ent.Comp.Whitelist, user) && ent.Comp.Whitelist.Components != null)
		{
			string message = base.Loc.GetString("cm-gun-unskilled", (ValueTuple<string, object>)("gun", ent));
			_popup.PopupClient(message, user, user, PopupType.SmallCaution);
			return false;
		}
		WieldableComponent wieldable = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(Entity<AimedShotComponent>.op_Implicit(ent), ref wieldable) && !wieldable.Wielded)
		{
			string message2 = base.Loc.GetString("rmc-action-popup-aiming-user-must-wield", (ValueTuple<string, object>)("gun", ent));
			_popup.PopupClient(message2, user, user);
			return false;
		}
		GetAmmoCountEvent ammoCount = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(Entity<AimedShotComponent>.op_Implicit(ent), ref ammoCount, false);
		if (ammoCount.Count <= 0)
		{
			string message3 = base.Loc.GetString("rmc-action-popup-aiming-gun-no-ammo", (ValueTuple<string, object>)("gun", ent));
			_popup.PopupClient(message3, user, user);
			return false;
		}
		if (_transform.InRange(((EntitySystem)this).Transform(target).Coordinates, ((EntitySystem)this).Transform(user).Coordinates, (float)ent.Comp.MinRange))
		{
			string message4 = base.Loc.GetString("rmc-action-popup-aiming-target-too-close", (ValueTuple<string, object>)("target", target));
			_popup.PopupClient(message4, user, user);
			return false;
		}
		return true;
	}

	private void RemoveTarget(Entity<AimedShotComponent> ent, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		_targeting.StopTargeting(Entity<TargetingComponent>.op_Implicit(ent.Owner), target);
		ent.Comp.Targets.Remove(target);
		ent.Comp.CurrentTarget = null;
		((EntitySystem)this).Dirty<AimedShotComponent>(ent, (MetaDataComponent)null);
	}
}
