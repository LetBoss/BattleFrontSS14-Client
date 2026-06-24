using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
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

	private EntityQuery<MeleeWeaponComponent> _meleeWeaponQuery;

	private EntityQuery<XenoComponent> _xenoQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_meleeWeaponQuery = ((EntitySystem)this).GetEntityQuery<MeleeWeaponComponent>();
		_xenoQuery = ((EntitySystem)this).GetEntityQuery<XenoComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ActorComponent, AttackAttemptEvent>((EntityEventRefHandler<ActorComponent, AttackAttemptEvent>)OnActorAttackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImmuneToUnarmedComponent, GettingAttackedAttemptEvent>((EntityEventRefHandler<ImmuneToUnarmedComponent, GettingAttackedAttemptEvent>)OnImmuneToUnarmedGettingAttacked, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImmuneToMeleeComponent, GettingAttackedAttemptEvent>((EntityEventRefHandler<ImmuneToMeleeComponent, GettingAttackedAttemptEvent>)OnImmuneToMeleeGettingAttacked, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeReceivedMultiplierComponent, DamageModifyEvent>((EntityEventRefHandler<MeleeReceivedMultiplierComponent, DamageModifyEvent>)OnMeleeReceivedMultiplierDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StunOnHitComponent, MeleeHitEvent>((EntityEventRefHandler<StunOnHitComponent, MeleeHitEvent>)OnStunOnHitMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeDamageMultiplierComponent, MeleeHitEvent>((EntityEventRefHandler<MeleeDamageMultiplierComponent, MeleeHitEvent>)OnMultiplierOnHitMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMeleeDamageSkillComponent, MeleeHitEvent>((EntityEventRefHandler<RMCMeleeDamageSkillComponent, MeleeHitEvent>)OnSkilledOnHitMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<LightAttackEvent>((EntitySessionEventHandler<LightAttackEvent>)OnLightAttack, new Type[1] { typeof(SharedMeleeWeaponSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<HeavyAttackEvent>((EntitySessionEventHandler<HeavyAttackEvent>)OnHeavyAttack, new Type[1] { typeof(SharedMeleeWeaponSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<DisarmAttackEvent>((EntitySessionEventHandler<DisarmAttackEvent>)OnDisarmAttack, new Type[1] { typeof(SharedMeleeWeaponSystem) }, (Type[])null);
	}

	public void MeleeResetInit(Entity<MeleeResetComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent weapon = default(MeleeWeaponComponent);
		if (!((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<MeleeResetComponent>.op_Implicit(ent), ref weapon))
		{
			((EntitySystem)this).RemComp<MeleeResetComponent>(Entity<MeleeResetComponent>.op_Implicit(ent));
			return;
		}
		ent.Comp.OriginalTime = weapon.NextAttack;
		weapon.NextAttack = _timing.CurTime;
		((EntitySystem)this).Dirty(Entity<MeleeResetComponent>.op_Implicit(ent), (IComponent)(object)weapon, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(Entity<MeleeResetComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	private void OnStunOnHitMeleeHit(Entity<StunOnHitComponent> ent, ref MeleeHitEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit)
		{
			return;
		}
		foreach (EntityUid hit in args.HitEntities)
		{
			if (_whitelist.IsValid(ent.Comp.Whitelist, hit))
			{
				_stun.TryParalyze(hit, ent.Comp.Duration, refresh: true);
			}
		}
	}

	private void OnMultiplierOnHitMeleeHit(Entity<MeleeDamageMultiplierComponent> ent, ref MeleeHitEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit)
		{
			return;
		}
		MeleeDamageMultiplierComponent comp = ent.Comp;
		args.BonusDamage = _skills.ApplyMeleeSkillModifier(args.User, args.BonusDamage);
		DamageSpecifier totalDamage = args.BaseDamage + args.BonusDamage;
		foreach (EntityUid hit in args.HitEntities)
		{
			if (_whitelist.IsValid(comp.Whitelist, hit))
			{
				DamageSpecifier damage = totalDamage * comp.Multiplier;
				args.BonusDamage += damage;
				break;
			}
		}
	}

	private void OnSkilledOnHitMeleeHit(Entity<RMCMeleeDamageSkillComponent> ent, ref MeleeHitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		DamageGroupPrototype bonusType = default(DamageGroupPrototype);
		if (args.IsHit && _prototypeManager.TryIndex<DamageGroupPrototype>(ProtoId<DamageTypePrototype>.op_Implicit(ent.Comp.BonusDamageType), ref bonusType))
		{
			DamageSpecifier totalBonusDamage = new DamageSpecifier(bonusType, _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(ent.Owner), ent.Comp.Skill));
			args.BonusDamage += totalBonusDamage;
		}
	}

	private void OnActorAttackAttempt(Entity<ActorComponent> ent, ref AttackAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = args.Uid;
		EntityUid? target = args.Target;
		if (target.HasValue && !(uid != target.GetValueOrDefault()) && !_netConfig.GetClientCVar<bool>(ent.Comp.PlayerSession.Channel, RMCCVars.RMCDamageYourself))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnImmuneToUnarmedGettingAttacked(Entity<ImmuneToUnarmedComponent> ent, ref GettingAttackedAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ApplyToXenos || !_xenoQuery.HasComp(args.Attacker))
		{
			EntityUid attacker = args.Attacker;
			EntityUid? weapon = args.Weapon;
			if (weapon.HasValue && attacker == weapon.GetValueOrDefault())
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnImmuneToMeleeGettingAttacked(Entity<ImmuneToMeleeComponent> ent, ref GettingAttackedAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Attacker))
		{
			args.Cancelled = true;
		}
	}

	private void OnMeleeReceivedMultiplierDamageModify(Entity<MeleeReceivedMultiplierComponent> ent, ref DamageModifyEvent args)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_meleeWeaponQuery.HasComp(args.Tool))
		{
			if (_xenoQuery.HasComp(args.Origin))
			{
				args.Damage = new DamageSpecifier(ent.Comp.XenoDamage);
			}
			else
			{
				args.Damage *= ent.Comp.OtherMultiplier;
			}
		}
	}

	private void OnLightAttack(LightAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (_melee.TryGetWeapon(user, out EntityUid weaponUid, out MeleeWeaponComponent weapon) && !(weaponUid != ((EntitySystem)this).GetEntity(msg.Weapon)))
			{
				TryMeleeReset(weaponUid, weapon, disarm: false);
			}
		}
	}

	private void OnHeavyAttack(HeavyAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (_melee.TryGetWeapon(user, out EntityUid weaponUid, out MeleeWeaponComponent weapon) && !(weaponUid != ((EntitySystem)this).GetEntity(msg.Weapon)))
			{
				TryMeleeReset(weaponUid, weapon, disarm: false);
			}
		}
	}

	private void OnDisarmAttack(DisarmAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (_melee.TryGetWeapon(user, out EntityUid weaponUid, out MeleeWeaponComponent weapon))
			{
				TryMeleeReset(weaponUid, weapon, disarm: true);
			}
		}
	}

	private void TryMeleeReset(EntityUid weaponUid, MeleeWeaponComponent weapon, bool disarm)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		MeleeResetComponent reset = default(MeleeResetComponent);
		if (((EntitySystem)this).TryComp<MeleeResetComponent>(weaponUid, ref reset))
		{
			if (disarm)
			{
				weapon.NextAttack = reset.OriginalTime;
			}
			((EntitySystem)this).RemComp<MeleeResetComponent>(weaponUid);
			((EntitySystem)this).Dirty(weaponUid, (IComponent)(object)weapon, (MetaDataComponent)null);
		}
	}

	public void DoLunge(EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent userXform = ((EntitySystem)this).Transform(user);
		Vector2 localPos = Vector2.Transform(_transform.GetWorldPosition(target), _transform.GetInvWorldMatrix(userXform));
		Angle localRotation = userXform.LocalRotation;
		localPos = ((Angle)(ref localRotation)).RotateVec(ref localPos);
		_melee.DoLunge(user, target, Angle.Zero, localPos, null);
	}

	public bool AttemptOverrideAttack(EntityUid target, Entity<MeleeWeaponComponent> weapon, EntityUid user, AttackEvent attack, out AttackEvent newAttack, float range = 1.5f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Vector2 targetPosition = _transform.GetMoverCoordinates(target).Position;
		Vector2 userPosition = _transform.GetMoverCoordinates(user).Position;
		List<NetEntity> entities = ((EntitySystem)this).GetNetEntityList(_melee.ArcRayCast(userPosition, DirectionExtensions.ToWorldAngle(targetPosition - userPosition), Angle.op_Implicit(0f), range, _transform.GetMapId(Entity<TransformComponent>.op_Implicit(user)), user).ToList());
		MeleeAttackAttemptEvent meleeEv = new MeleeAttackAttemptEvent(((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null), attack, attack.Coordinates, entities, ((EntitySystem)this).GetNetEntity(Entity<MeleeWeaponComponent>.op_Implicit(weapon), (MetaDataComponent)null));
		((EntitySystem)this).RaiseLocalEvent<MeleeAttackAttemptEvent>(user, ref meleeEv, false);
		newAttack = meleeEv.Attack;
		if (attack == newAttack)
		{
			return true;
		}
		if (meleeEv.Weapon == meleeEv.Target)
		{
			return false;
		}
		bool flag = newAttack is DisarmAttackEvent;
		bool disarm = flag;
		if (!_blocker.CanAttack(user, ((EntitySystem)this).GetEntity(meleeEv.Target), weapon, disarm))
		{
			return false;
		}
		return true;
	}

	public float GetUserLightAttackRange(EntityUid user, EntityUid? target, MeleeWeaponComponent melee)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		RMCMeleeUserGetRangeEvent ev = new RMCMeleeUserGetRangeEvent(target, melee.Range);
		((EntitySystem)this).RaiseLocalEvent<RMCMeleeUserGetRangeEvent>(user, ref ev, false);
		return ev.Range;
	}
}
