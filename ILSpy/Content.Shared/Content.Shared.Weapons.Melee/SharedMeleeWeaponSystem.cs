using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
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
using Content.Shared.Physics;
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

	private const int AttackMask = 31;

	private static readonly EntProtoId DisarmEffect = EntProtoId.op_Implicit("RMCWeaponArcDisarm");

	public int MaxTargets = 5;

	public const float GracePeriod = 0.05f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_configuration.OnValueChanged<int>(RMCCVars.CMMaxHeavyAttackTargets, (Action<int>)delegate(int v)
		{
			MaxTargets = v;
		}, true);
		((EntitySystem)this).SubscribeLocalEvent<MeleeWeaponComponent, HandSelectedEvent>((ComponentEventHandler<MeleeWeaponComponent, HandSelectedEvent>)OnMeleeSelected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeWeaponComponent, ShotAttemptedEvent>((ComponentEventRefHandler<MeleeWeaponComponent, ShotAttemptedEvent>)OnMeleeShotAttempted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MeleeWeaponComponent, GunShotEvent>((ComponentEventRefHandler<MeleeWeaponComponent, GunShotEvent>)OnMeleeShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BonusMeleeDamageComponent, GetMeleeDamageEvent>((ComponentEventRefHandler<BonusMeleeDamageComponent, GetMeleeDamageEvent>)OnGetBonusMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BonusMeleeDamageComponent, GetHeavyDamageModifierEvent>((ComponentEventRefHandler<BonusMeleeDamageComponent, GetHeavyDamageModifierEvent>)OnGetBonusHeavyDamageModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BonusMeleeAttackRateComponent, GetMeleeAttackRateEvent>((ComponentEventRefHandler<BonusMeleeAttackRateComponent, GetMeleeAttackRateEvent>)OnGetBonusMeleeAttackRate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleMeleeWeaponComponent, ItemToggledEvent>((ComponentEventHandler<ItemToggleMeleeWeaponComponent, ItemToggledEvent>)OnItemToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<HeavyAttackEvent>((EntitySessionEventHandler<HeavyAttackEvent>)OnHeavyAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<LightAttackEvent>((EntitySessionEventHandler<LightAttackEvent>)OnLightAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<DisarmAttackEvent>((EntitySessionEventHandler<DisarmAttackEvent>)OnDisarmAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<StopAttackEvent>((EntitySessionEventHandler<StopAttackEvent>)OnStopAttack, (Type[])null, (Type[])null);
	}

	private void OnMeleeShotAttempted(EntityUid uid, MeleeWeaponComponent comp, ref ShotAttemptedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(uid, ref gun) && gun.MeleeCooldownOnShoot && comp.NextAttack > Timing.CurTime)
		{
			args.Cancel();
		}
	}

	private void OnMeleeShot(EntityUid uid, MeleeWeaponComponent component, ref GunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(uid, ref gun) && gun.MeleeCooldownOnShoot && gun.NextFire > component.NextAttack)
		{
			component.NextAttack = gun.NextFire;
			((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, component, "NextAttack", (MetaDataComponent)null);
		}
	}

	private void OnMeleeSelected(EntityUid uid, MeleeWeaponComponent component, HandSelectedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float attackRate = GetAttackRate(uid, args.User, component);
		if (!attackRate.Equals(0f) && component.ResetOnHandSelected && !((EntitySystem)this).Paused(uid, (MetaDataComponent)null))
		{
			TimeSpan minimum = Timing.CurTime + TimeSpan.FromSeconds(1f / attackRate);
			if (!(minimum < component.NextAttack))
			{
				component.NextAttack = minimum;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, component, "NextAttack", (MetaDataComponent)null);
			}
		}
	}

	private void OnGetBonusMeleeDamage(EntityUid uid, BonusMeleeDamageComponent component, ref GetMeleeDamageEvent args)
	{
		if (component.BonusDamage != null)
		{
			args.Damage += component.BonusDamage;
		}
		if (component.DamageModifierSet != null)
		{
			args.Modifiers.Add(component.DamageModifierSet);
		}
	}

	private void OnGetBonusHeavyDamageModifier(EntityUid uid, BonusMeleeDamageComponent component, ref GetHeavyDamageModifierEvent args)
	{
		args.DamageModifier += component.HeavyDamageFlatModifier;
		args.Multipliers *= component.HeavyDamageMultiplier;
	}

	private void OnGetBonusMeleeAttackRate(EntityUid uid, BonusMeleeAttackRateComponent component, ref GetMeleeAttackRateEvent args)
	{
		args.Rate += component.FlatModifier;
		args.Multipliers *= component.Multiplier;
	}

	private void OnStopAttack(StopAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (user.HasValue && TryGetWeapon(user.Value, out EntityUid weaponUid, out MeleeWeaponComponent weapon) && !(weaponUid != ((EntitySystem)this).GetEntity(msg.Weapon)) && weapon.Attacking)
		{
			weapon.Attacking = false;
			((EntitySystem)this).DirtyField<MeleeWeaponComponent>(weaponUid, weapon, "Attacking", (MetaDataComponent)null);
		}
	}

	private void OnLightAttack(LightAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (TryGetWeapon(user, out EntityUid weaponUid, out MeleeWeaponComponent weapon) && !(weaponUid != ((EntitySystem)this).GetEntity(msg.Weapon)))
			{
				AttemptAttack(user, weaponUid, weapon, msg, ((EntitySessionEventArgs)(ref args)).SenderSession);
			}
		}
	}

	private void OnHeavyAttack(HeavyAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (TryGetWeapon(user, out EntityUid weaponUid, out MeleeWeaponComponent weapon) && !(weaponUid != ((EntitySystem)this).GetEntity(msg.Weapon)))
			{
				AttemptAttack(user, weaponUid, weapon, msg, ((EntitySessionEventArgs)(ref args)).SenderSession);
			}
		}
	}

	private void OnDisarmAttack(DisarmAttackEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (TryGetWeapon(user, out EntityUid weaponUid, out MeleeWeaponComponent weapon))
			{
				AttemptAttack(user, weaponUid, weapon, msg, ((EntitySessionEventArgs)(ref args)).SenderSession);
			}
		}
	}

	public DamageSpecifier GetDamage(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MeleeWeaponComponent>(uid, ref component, false))
		{
			return new DamageSpecifier();
		}
		GetMeleeDamageEvent ev = new GetMeleeDamageEvent(uid, new DamageSpecifier(component.Damage * Damageable.UniversalMeleeDamageModifier), new List<DamageModifierSet>(), user, component.ResistanceBypass);
		((EntitySystem)this).RaiseLocalEvent<GetMeleeDamageEvent>(uid, ref ev, true);
		return DamageSpecifier.ApplyModifierSets(ev.Damage, ev.Modifiers);
	}

	public float GetAttackRate(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MeleeWeaponComponent>(uid, ref component, true))
		{
			return 0f;
		}
		GetMeleeAttackRateEvent ev = new GetMeleeAttackRateEvent(uid, component.AttackRate, 1f, user);
		((EntitySystem)this).RaiseLocalEvent<GetMeleeAttackRateEvent>(uid, ref ev, false);
		return ev.Rate * ev.Multipliers;
	}

	public FixedPoint2 GetHeavyDamageModifier(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MeleeWeaponComponent>(uid, ref component, true))
		{
			return FixedPoint2.Zero;
		}
		GetHeavyDamageModifierEvent ev = new GetHeavyDamageModifierEvent(uid, component.ClickDamageModifier, 1f, user);
		((EntitySystem)this).RaiseLocalEvent<GetHeavyDamageModifierEvent>(uid, ref ev, false);
		return ev.DamageModifier * ev.Multipliers;
	}

	public bool GetResistanceBypass(EntityUid uid, EntityUid user, MeleeWeaponComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MeleeWeaponComponent>(uid, ref component, true))
		{
			return false;
		}
		GetMeleeDamageEvent ev = new GetMeleeDamageEvent(uid, new DamageSpecifier(component.Damage * Damageable.UniversalMeleeDamageModifier), new List<DamageModifierSet>(), user, component.ResistanceBypass);
		((EntitySystem)this).RaiseLocalEvent<GetMeleeDamageEvent>(uid, ref ev, false);
		return ev.ResistanceBypass;
	}

	public bool TryGetWeapon(EntityUid entity, out EntityUid weaponUid, [NotNullWhen(true)] out MeleeWeaponComponent? melee)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		weaponUid = default(EntityUid);
		melee = null;
		GetMeleeWeaponEvent ev = new GetMeleeWeaponEvent();
		((EntitySystem)this).RaiseLocalEvent<GetMeleeWeaponEvent>(entity, ev, false);
		if (((HandledEntityEventArgs)ev).Handled)
		{
			if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(ev.Weapon, ref melee))
			{
				weaponUid = ev.Weapon.Value;
				return true;
			}
			return false;
		}
		if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(entity), out var held))
		{
			if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(held, ref melee) && !melee.MustBeEquippedToUse)
			{
				weaponUid = held.Value;
				return true;
			}
			if (!((EntitySystem)this).HasComp<VirtualItemComponent>(held))
			{
				return false;
			}
		}
		MeleeWeaponComponent glovesMelee = default(MeleeWeaponComponent);
		if (_inventory.TryGetSlotEntity(entity, "gloves", out var gloves) && ((EntitySystem)this).TryComp<MeleeWeaponComponent>(gloves, ref glovesMelee))
		{
			weaponUid = gloves.Value;
			melee = glovesMelee;
			return true;
		}
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(entity, ref melee))
		{
			weaponUid = entity;
			return true;
		}
		return false;
	}

	public void AttemptLightAttackMiss(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		AttemptAttack(user, weaponUid, weapon, new LightAttackEvent(null, ((EntitySystem)this).GetNetEntity(weaponUid, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null)), null);
	}

	public bool AttemptLightAttack(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent targetXform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(target, ref targetXform))
		{
			return false;
		}
		return AttemptAttack(user, weaponUid, weapon, new LightAttackEvent(((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(weaponUid, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(targetXform.Coordinates, (MetaDataComponent)null)), null);
	}

	public bool AttemptDisarmAttack(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent targetXform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(target, ref targetXform))
		{
			return false;
		}
		return AttemptAttack(user, weaponUid, weapon, new DisarmAttackEvent(((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(targetXform.Coordinates, (MetaDataComponent)null)), null);
	}

	private bool AttemptAttack(EntityUid user, EntityUid weaponUid, MeleeWeaponComponent weapon, AttackEvent attack, ICommonSession? session)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = Timing.CurTime;
		if (weapon.NextAttack > curTime)
		{
			return false;
		}
		if (!CombatMode.IsInCombatMode(user))
		{
			return false;
		}
		EntityUid? target = null;
		if (!(attack is LightAttackEvent light))
		{
			if (attack is DisarmAttackEvent disarm)
			{
				if (disarm.Target.HasValue && !((EntitySystem)this).TryGetEntity(disarm.Target, ref target))
				{
					return false;
				}
				if (!Blocker.CanAttack(user, target, Entity<MeleeWeaponComponent>.op_Implicit((weaponUid, weapon)), disarm: true))
				{
					return false;
				}
			}
			else
			{
				ActionBlockerSystem blocker = Blocker;
				Entity<MeleeWeaponComponent>? weapon2 = Entity<MeleeWeaponComponent>.op_Implicit((weaponUid, weapon));
				if (!blocker.CanAttack(user, null, weapon2))
				{
					return false;
				}
			}
		}
		else
		{
			if (light.Target.HasValue && !((EntitySystem)this).TryGetEntity(light.Target, ref target))
			{
				return false;
			}
			if (!Blocker.CanAttack(user, target, Entity<MeleeWeaponComponent>.op_Implicit((weaponUid, weapon))))
			{
				return false;
			}
			EntityUid? val = target;
			if (val.HasValue && weaponUid == val.GetValueOrDefault())
			{
				return false;
			}
		}
		if (target.HasValue)
		{
			if (!_rmcMelee.AttemptOverrideAttack(target.Value, Entity<MeleeWeaponComponent>.op_Implicit((weaponUid, weapon)), user, attack, out AttackEvent newAttack))
			{
				return false;
			}
			attack = newAttack;
		}
		TimeSpan fireRate = TimeSpan.FromSeconds(1f / GetAttackRate(weaponUid, user, weapon));
		int swings = 0;
		if (weapon.NextAttack < curTime)
		{
			weapon.NextAttack = curTime;
		}
		while (weapon.NextAttack <= curTime)
		{
			weapon.NextAttack += fireRate;
			swings++;
		}
		((EntitySystem)this).DirtyField<MeleeWeaponComponent>(weaponUid, weapon, "NextAttack", (MetaDataComponent)null);
		AttemptMeleeEvent ev = new AttemptMeleeEvent(user);
		((EntitySystem)this).RaiseLocalEvent<AttemptMeleeEvent>(weaponUid, ref ev, false);
		if (ev.Cancelled)
		{
			if (ev.Message != null)
			{
				PopupSystem.PopupClient(ev.Message, weaponUid, user);
			}
			return false;
		}
		for (int i = 0; i < swings; i++)
		{
			float range = weapon.Range;
			string animation;
			if (!(attack is LightAttackEvent light2))
			{
				if (!(attack is DisarmAttackEvent disarm2))
				{
					if (!(attack is HeavyAttackEvent heavy))
					{
						throw new NotImplementedException();
					}
					if (!DoHeavyAttack(user, heavy, weaponUid, weapon, session))
					{
						return false;
					}
					animation = EntProtoId.op_Implicit(weapon.WideAnimation);
				}
				else
				{
					if (!DoDisarm(user, disarm2, weaponUid, weapon, session))
					{
						weapon.NextAttack = curTime + TimeSpan.FromSeconds(0.6);
					}
					animation = EntProtoId.op_Implicit(DisarmEffect);
				}
			}
			else
			{
				DoLightAttack(user, light2, weaponUid, weapon, session);
				animation = EntProtoId.op_Implicit(weapon.Animation);
				range = _rmcMelee.GetUserLightAttackRange(user, target, weapon);
			}
			DoLungeAnimation(user, weaponUid, weapon.Angle, TransformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(attack.Coordinates), true), range, animation);
		}
		MeleeAttackEvent attackEv = new MeleeAttackEvent(weaponUid);
		((EntitySystem)this).RaiseLocalEvent<MeleeAttackEvent>(user, ref attackEv, false);
		weapon.Attacking = true;
		((EntitySystem)this).DirtyField<MeleeWeaponComponent>(weaponUid, weapon, "Attacking", (MetaDataComponent)null);
		return true;
	}

	protected abstract bool InRange(EntityUid user, EntityUid target, float range, ICommonSession? session);

	protected virtual void DoLightAttack(EntityUid user, LightAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, ICommonSession? session)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = GetDamage(meleeUid, user, component) * GetHeavyDamageModifier(meleeUid, user, component);
		EntityUid? target = ((EntitySystem)this).GetEntity(ev.Target);
		bool resistanceBypass = GetResistanceBypass(meleeUid, user, component);
		TransformComponent targetXform = default(TransformComponent);
		if (((EntitySystem)this).Deleted(target) || !((EntitySystem)this).HasComp<DamageableComponent>(target) || !((EntitySystem)this).TryComp(target, ref targetXform) || !InRange(user, target.Value, _rmcMelee.GetUserLightAttackRange(user, target, component), session))
		{
			if (meleeUid == user)
			{
				ISharedAdminLogManager adminLogger = AdminLogger;
				LogStringHandler handler = new LogStringHandler(52, 1);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
				handler.AppendLiteral(" melee attacked (light) using their hands and missed");
				adminLogger.Add(LogType.MeleeHit, LogImpact.Low, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = AdminLogger;
				LogStringHandler handler2 = new LogStringHandler(41, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
				handler2.AppendLiteral(" melee attacked (light) using ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(meleeUid)), "tool", "ToPrettyString(meleeUid)");
				handler2.AppendLiteral(" and missed");
				adminLogger2.Add(LogType.MeleeHit, LogImpact.Low, ref handler2);
			}
			MeleeHitEvent missEvent = new MeleeHitEvent(new List<EntityUid>(), user, meleeUid, damage, null);
			((EntitySystem)this).RaiseLocalEvent<MeleeHitEvent>(meleeUid, missEvent, false);
			_meleeSound.PlaySwingSound(user, meleeUid, component);
			return;
		}
		MeleeHitEvent hitEvent = new MeleeHitEvent(new List<EntityUid> { target.Value }, user, meleeUid, damage, null);
		((EntitySystem)this).RaiseLocalEvent<MeleeHitEvent>(meleeUid, hitEvent, false);
		if (((HandledEntityEventArgs)hitEvent).Handled)
		{
			return;
		}
		List<EntityUid> targets = new List<EntityUid>(1) { target.Value };
		EntityUid weapon = ((EntitySystem)this).GetEntity(ev.Weapon);
		Interaction.DoContactInteraction(user, weapon);
		Interaction.DoContactInteraction(user, target);
		AttackedEvent attackedEvent = new AttackedEvent(meleeUid, user, targetXform.Coordinates);
		((EntitySystem)this).RaiseLocalEvent<AttackedEvent>(target.Value, attackedEvent, false);
		DamageSpecifier modifiedDamage = DamageSpecifier.ApplyModifierSets(damage + hitEvent.BonusDamage + attackedEvent.BonusDamage, hitEvent.ModifiersList);
		DamageableSystem damageable = Damageable;
		EntityUid? uid = target;
		EntityUid? origin = user;
		DamageSpecifier damageResult = damageable.TryChangeDamage(uid, modifiedDamage, resistanceBypass, interruptsDoAfters: true, null, origin, meleeUid);
		if (damageResult != null && !damageResult.Empty)
		{
			if (damageResult.DamageDict.TryGetValue("Blunt", out var bluntDamage))
			{
				_stamina.TakeStaminaDamage(target.Value, (bluntDamage * component.BluntStaminaDamageFactor).Float(), null, user, (meleeUid == user) ? ((EntityUid?)null) : new EntityUid?(meleeUid), visual: false);
			}
			if (meleeUid == user)
			{
				ISharedAdminLogManager adminLogger3 = AdminLogger;
				LogStringHandler handler3 = new LogStringHandler(60, 3);
				handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
				handler3.AppendLiteral(" melee attacked (light) ");
				handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target.Value)), "subject", "ToPrettyString(target.Value)");
				handler3.AppendLiteral(" using their hands and dealt ");
				handler3.AppendFormatted(damageResult.GetTotal(), "damage", "damageResult.GetTotal()");
				handler3.AppendLiteral(" damage");
				adminLogger3.Add(LogType.MeleeHit, LogImpact.Medium, ref handler3);
			}
			else
			{
				ISharedAdminLogManager adminLogger4 = AdminLogger;
				LogStringHandler handler4 = new LogStringHandler(49, 4);
				handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
				handler4.AppendLiteral(" melee attacked (light) ");
				handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target.Value)), "subject", "ToPrettyString(target.Value)");
				handler4.AppendLiteral(" using ");
				handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(meleeUid)), "tool", "ToPrettyString(meleeUid)");
				handler4.AppendLiteral(" and dealt ");
				handler4.AppendFormatted(damageResult.GetTotal(), "damage", "damageResult.GetTotal()");
				handler4.AppendLiteral(" damage");
				adminLogger4.Add(LogType.MeleeHit, LogImpact.Medium, ref handler4);
			}
		}
		if (damageResult != null)
		{
			_meleeSound.PlayHitSound(target.Value, user, GetHighestDamageSound(damageResult, _protoManager), hitEvent.HitSoundOverride, component);
		}
		if (damageResult?.GetTotal() > FixedPoint2.Zero)
		{
			DoDamageEffect(targets, user, targetXform);
		}
	}

	protected abstract void DoDamageEffect(List<EntityUid> targets, EntityUid? user, TransformComponent targetXform);

	private bool DoHeavyAttack(EntityUid user, HeavyAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, ICommonSession? session)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent userXform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(user, ref userXform))
		{
			return false;
		}
		MapCoordinates targetMap = TransformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(ev.Coordinates), true);
		if (targetMap.MapId != userXform.MapID)
		{
			return false;
		}
		Vector2 userPos = TransformSystem.GetWorldPosition(userXform);
		Vector2 direction = targetMap.Position - userPos;
		float distance = Math.Min(component.Range, direction.Length());
		DamageSpecifier damage = GetDamage(meleeUid, user, component);
		List<EntityUid> entities = ((EntitySystem)this).GetEntityList(ev.Entities);
		if (entities.Count == 0)
		{
			if (meleeUid == user)
			{
				ISharedAdminLogManager adminLogger = AdminLogger;
				LogStringHandler handler = new LogStringHandler(52, 1);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
				handler.AppendLiteral(" melee attacked (heavy) using their hands and missed");
				adminLogger.Add(LogType.MeleeHit, LogImpact.Low, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = AdminLogger;
				LogStringHandler handler2 = new LogStringHandler(41, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
				handler2.AppendLiteral(" melee attacked (heavy) using ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(meleeUid)), "tool", "ToPrettyString(meleeUid)");
				handler2.AppendLiteral(" and missed");
				adminLogger2.Add(LogType.MeleeHit, LogImpact.Low, ref handler2);
			}
			MeleeHitEvent missEvent = new MeleeHitEvent(new List<EntityUid>(), user, meleeUid, damage, direction);
			((EntitySystem)this).RaiseLocalEvent<MeleeHitEvent>(meleeUid, missEvent, false);
			_meleeSound.PlaySwingSound(user, meleeUid, component);
			return true;
		}
		if (entities.Count > MaxTargets)
		{
			entities.RemoveRange(MaxTargets, entities.Count - MaxTargets);
		}
		for (int i = entities.Count - 1; i >= 0; i--)
		{
			if (!ArcRaySuccessful(entities[i], userPos, DirectionExtensions.ToWorldAngle(direction), component.Angle, distance, userXform.MapID, user, session))
			{
				entities.RemoveAt(i);
			}
		}
		List<EntityUid> targets = new List<EntityUid>();
		EntityQuery<DamageableComponent> damageQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		foreach (EntityUid entity in entities)
		{
			if (!(entity == user) && damageQuery.HasComponent(entity))
			{
				targets.Add(entity);
			}
		}
		MeleeHitEvent hitEvent = new MeleeHitEvent(targets, user, meleeUid, damage, direction);
		((EntitySystem)this).RaiseLocalEvent<MeleeHitEvent>(meleeUid, hitEvent, false);
		if (((HandledEntityEventArgs)hitEvent).Handled)
		{
			return true;
		}
		EntityUid weapon = ((EntitySystem)this).GetEntity(ev.Weapon);
		Interaction.DoContactInteraction(user, weapon);
		foreach (EntityUid target in targets)
		{
			Interaction.DoContactInteraction(user, target);
		}
		DamageSpecifier appliedDamage = new DamageSpecifier();
		for (int i2 = targets.Count - 1; i2 >= 0; i2--)
		{
			EntityUid entity2 = targets[i2];
			if (!Blocker.CanAttack(user, entity2, Entity<MeleeWeaponComponent>.op_Implicit((weapon, component))))
			{
				targets.RemoveAt(i2);
			}
			else
			{
				AttackedEvent attackedEvent = new AttackedEvent(meleeUid, user, ((EntitySystem)this).GetCoordinates(ev.Coordinates));
				((EntitySystem)this).RaiseLocalEvent<AttackedEvent>(entity2, attackedEvent, false);
				DamageSpecifier modifiedDamage = DamageSpecifier.ApplyModifierSets(damage + hitEvent.BonusDamage + attackedEvent.BonusDamage, hitEvent.ModifiersList);
				DamageSpecifier damageResult = Damageable.TryChangeDamage(entity2, modifiedDamage, ignoreResistances: false, interruptsDoAfters: true, null, user, meleeUid);
				if (damageResult != null && damageResult.GetTotal() > FixedPoint2.Zero)
				{
					if (damageResult.DamageDict.TryGetValue("Blunt", out var bluntDamage))
					{
						_stamina.TakeStaminaDamage(entity2, (bluntDamage * component.BluntStaminaDamageFactor).Float(), null, user, (meleeUid == user) ? ((EntityUid?)null) : new EntityUid?(meleeUid), visual: false);
					}
					appliedDamage += damageResult;
					if (meleeUid == user)
					{
						ISharedAdminLogManager adminLogger3 = AdminLogger;
						LogStringHandler handler3 = new LogStringHandler(60, 3);
						handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
						handler3.AppendLiteral(" melee attacked (heavy) ");
						handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity2)), "subject", "ToPrettyString(entity)");
						handler3.AppendLiteral(" using their hands and dealt ");
						handler3.AppendFormatted(damageResult.GetTotal(), "damage", "damageResult.GetTotal()");
						handler3.AppendLiteral(" damage");
						adminLogger3.Add(LogType.MeleeHit, LogImpact.Medium, ref handler3);
					}
					else
					{
						ISharedAdminLogManager adminLogger4 = AdminLogger;
						LogStringHandler handler4 = new LogStringHandler(49, 4);
						handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
						handler4.AppendLiteral(" melee attacked (heavy) ");
						handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity2)), "subject", "ToPrettyString(entity)");
						handler4.AppendLiteral(" using ");
						handler4.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(meleeUid)), "tool", "ToPrettyString(meleeUid)");
						handler4.AppendLiteral(" and dealt ");
						handler4.AppendFormatted(damageResult.GetTotal(), "damage", "damageResult.GetTotal()");
						handler4.AppendLiteral(" damage");
						adminLogger4.Add(LogType.MeleeHit, LogImpact.Medium, ref handler4);
					}
				}
			}
		}
		if (entities.Count != 0)
		{
			EntityUid target2 = entities.First();
			_meleeSound.PlayHitSound(target2, user, GetHighestDamageSound(appliedDamage, _protoManager), hitEvent.HitSoundOverride, component);
		}
		if (appliedDamage.GetTotal() > FixedPoint2.Zero)
		{
			DoDamageEffect(targets, user, ((EntitySystem)this).Transform(targets[0]));
		}
		return true;
	}

	public HashSet<EntityUid> ArcRayCast(Vector2 position, Angle angle, Angle arcWidth, float range, MapId mapId, EntityUid ignore)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		int increments = 1 + 35 * (int)Math.Ceiling(Angle.op_Implicit(arcWidth) / (Math.PI * 2.0));
		double increment = Angle.op_Implicit(arcWidth) / (double)increments;
		Angle baseAngle = angle - Angle.op_Implicit(Angle.op_Implicit(arcWidth) / 2.0);
		HashSet<EntityUid> resSet = new HashSet<EntityUid>();
		Angle castAngle = default(Angle);
		for (int i = 0; i < increments; i++)
		{
			((Angle)(ref castAngle))._002Ector(Angle.op_Implicit(baseAngle + Angle.op_Implicit(increment * (double)i)));
			List<RayCastResults> res = _physics.IntersectRay(mapId, new CollisionRay(position, ((Angle)(ref castAngle)).ToWorldVec(), 31), range, (EntityUid?)ignore, false).ToList();
			if (res.Count == 0)
			{
				continue;
			}
			foreach (RayCastResults item in res.Where(delegate(RayCastResults x)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				float distance = ((RayCastResults)(ref x)).Distance;
				RayCastResults val = res[0];
				return distance.Equals(((RayCastResults)(ref val)).Distance);
			}))
			{
				RayCastResults r = item;
				if (Interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(ignore), Entity<TransformComponent>.op_Implicit(((RayCastResults)(ref r)).HitEntity), range + 0.1f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: false, overlapCheck: false))
				{
					resSet.Add(((RayCastResults)(ref r)).HitEntity);
				}
			}
		}
		return resSet;
	}

	protected virtual bool ArcRaySuccessful(EntityUid targetUid, Vector2 position, Angle angle, Angle arcWidth, float range, MapId mapId, EntityUid ignore, ICommonSession? session)
	{
		return true;
	}

	public static string? GetHighestDamageSound(DamageSpecifier modifiedDamage, IPrototypeManager protoManager)
	{
		if (modifiedDamage.GetTotal() <= FixedPoint2.Zero)
		{
			return null;
		}
		Dictionary<string, FixedPoint2> groups = modifiedDamage.GetDamagePerGroup(protoManager);
		if (groups.Count == 1)
		{
			return groups.Keys.First();
		}
		FixedPoint2 highestDamage = FixedPoint2.Zero;
		string highestDamageType = null;
		foreach (var (type, fixedPoint2) in modifiedDamage.DamageDict)
		{
			if (!(fixedPoint2 <= highestDamage))
			{
				highestDamageType = type;
			}
		}
		return highestDamageType;
	}

	private float CalculateDisarmChance(EntityUid disarmer, EntityUid disarmed, EntityUid? inTargetHand, CombatModeComponent disarmerComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DisarmProneComponent>(disarmer))
		{
			return 1f;
		}
		if (((EntitySystem)this).HasComp<DisarmProneComponent>(disarmed))
		{
			return 0f;
		}
		float chance = disarmerComp.BaseDisarmFailChance;
		DisarmMalusComponent malus = default(DisarmMalusComponent);
		if (inTargetHand.HasValue && ((EntitySystem)this).TryComp<DisarmMalusComponent>(inTargetHand, ref malus))
		{
			chance += malus.Malus;
		}
		return Math.Clamp(chance, 0f, 1f);
	}

	private bool DoDisarm(EntityUid user, DisarmAttackEvent ev, EntityUid meleeUid, MeleeWeaponComponent component, ICommonSession? session)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		_meleeSound.PlaySwingSound(user, meleeUid, component);
		EntityUid? target = ((EntitySystem)this).GetEntity(ev.Target);
		if (!((EntitySystem)this).Deleted(target))
		{
			EntityUid? val = target;
			if (!val.HasValue || !(user == val.GetValueOrDefault()))
			{
				if (MobState.IsIncapacitated(target.Value))
				{
					return false;
				}
				CombatModeComponent combatMode = default(CombatModeComponent);
				if (!((EntitySystem)this).TryComp<CombatModeComponent>(user, ref combatMode) || combatMode.CanDisarm != true)
				{
					return false;
				}
				HandsComponent targetHandsComponent = default(HandsComponent);
				StatusEffectsComponent status = default(StatusEffectsComponent);
				if (!((EntitySystem)this).TryComp<HandsComponent>(target, ref targetHandsComponent) && (!((EntitySystem)this).TryComp<StatusEffectsComponent>(target, ref status) || !status.AllowedEffects.Contains("KnockedDown")))
				{
					if (((EntitySystem)this).HasComp<MobStateComponent>(target.Value))
					{
						PopupSystem.PopupClient(base.Loc.GetString("disarm-action-disarmable", (ValueTuple<string, object>)("targetName", target.Value)), target.Value);
					}
					return false;
				}
				if (!InRange(user, target.Value, component.Range, session))
				{
					return false;
				}
				CMDisarmEvent cmDisarmEvent = new CMDisarmEvent(user);
				((EntitySystem)this).RaiseLocalEvent<CMDisarmEvent>(target.Value, ref cmDisarmEvent, false);
				if (cmDisarmEvent.Handled)
				{
					return true;
				}
				EntityUid? inTargetHand = null;
				if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(target.Value), out var activeHeldEntity))
				{
					inTargetHand = activeHeldEntity.Value;
				}
				DisarmAttemptEvent attemptEvent = new DisarmAttemptEvent(target.Value, user, inTargetHand);
				if (inTargetHand.HasValue)
				{
					((EntitySystem)this).RaiseLocalEvent<DisarmAttemptEvent>(inTargetHand.Value, ref attemptEvent, false);
				}
				((EntitySystem)this).RaiseLocalEvent<DisarmAttemptEvent>(target.Value, ref attemptEvent, false);
				if (attemptEvent.Cancelled)
				{
					return false;
				}
				float chance = CalculateDisarmChance(user, target.Value, inTargetHand, combatMode);
				if (_netMan.IsClient)
				{
					_meleeSound.PlaySwingSound(user, meleeUid, component);
					return true;
				}
				if (RandomExtensions.Prob(_random, chance))
				{
					return false;
				}
				DisarmedEvent eventArgs = new DisarmedEvent(target.Value, user, 1f - chance);
				((EntitySystem)this).RaiseLocalEvent<DisarmedEvent>(target.Value, ref eventArgs, false);
				if (!eventArgs.Handled)
				{
					return false;
				}
				Interaction.DoContactInteraction(user, target);
				ISharedAdminLogManager adminLogger = AdminLogger;
				LogStringHandler handler = new LogStringHandler(16, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
				handler.AppendLiteral(" used disarm on ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(target, (MetaDataComponent)null), "target", "ToPrettyString(target)");
				adminLogger.Add(LogType.DisarmedAction, ref handler);
				ISharedAdminLogManager adminLogger2 = AdminLogger;
				LogStringHandler handler2 = new LogStringHandler(16, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
				handler2.AppendLiteral(" used disarm on ");
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(target, (MetaDataComponent)null), "target", "ToPrettyString(target)");
				adminLogger2.Add(LogType.DisarmedAction, ref handler2);
				SharedAudioSystem audio = _audio;
				SoundSpecifier disarmSuccessSound = combatMode.DisarmSuccessSound;
				EntityUid value = target.Value;
				AudioParams val2 = ((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.025f);
				audio.PlayPvs(disarmSuccessSound, value, (AudioParams?)((AudioParams)(ref val2)).WithVolume(5f));
				EntityUid targetEnt = Identity.Entity(target.Value, (IEntityManager)(object)base.EntityManager);
				EntityUid userEnt = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
				string msgOther = base.Loc.GetString(eventArgs.PopupPrefix + "popup-message-other-clients", (ValueTuple<string, object>)("performerName", userEnt), (ValueTuple<string, object>)("targetName", targetEnt));
				string msgUser = base.Loc.GetString(eventArgs.PopupPrefix + "popup-message-cursor", (ValueTuple<string, object>)("targetName", targetEnt));
				Filter filterOther = Filter.PvsExcept(user, 2f, (IEntityManager)(object)base.EntityManager);
				PopupSystem.PopupEntity(msgOther, user, filterOther, recordReplay: true);
				PopupSystem.PopupEntity(msgUser, target.Value, user);
				if (eventArgs.IsStunned)
				{
					PopupSystem.PopupEntity(base.Loc.GetString("stunned-component-disarm-success-others", (ValueTuple<string, object>)("source", userEnt), (ValueTuple<string, object>)("target", targetEnt)), targetEnt, Filter.PvsExcept(user, 2f, (IEntityManager)null), recordReplay: true, PopupType.LargeCaution);
					PopupSystem.PopupCursor(base.Loc.GetString("stunned-component-disarm-success", (ValueTuple<string, object>)("target", targetEnt)), user, PopupType.Large);
					ISharedAdminLogManager adminLogger3 = AdminLogger;
					LogStringHandler handler3 = new LogStringHandler(14, 2);
					handler3.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
					handler3.AppendLiteral(" knocked down ");
					handler3.AppendFormatted(((EntitySystem)this).ToPrettyString(target, (MetaDataComponent)null), "target", "ToPrettyString(target)");
					adminLogger3.Add(LogType.DisarmedKnockdown, LogImpact.Medium, ref handler3);
				}
				return true;
			}
		}
		return false;
	}

	private void DoLungeAnimation(EntityUid user, EntityUid weapon, Angle angle, MapCoordinates coordinates, float length, string? animation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent userXform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(user, ref userXform))
		{
			return;
		}
		Matrix3x2 invMatrix = TransformSystem.GetInvWorldMatrix(userXform);
		Vector2 localPos = Vector2.Transform(coordinates.Position, invMatrix);
		if (!(localPos.LengthSquared() <= 0f))
		{
			Angle localRotation = userXform.LocalRotation;
			localPos = ((Angle)(ref localRotation)).RotateVec(ref localPos);
			float visualLength = length - 0.2f;
			if (localPos.Length() > visualLength)
			{
				localPos = Vector2Helpers.Normalized(localPos) * visualLength;
			}
			DoLunge(user, weapon, angle, localPos, animation);
		}
	}

	public abstract void DoLunge(EntityUid user, EntityUid weapon, Angle angle, Vector2 localPos, string? animation, bool predicted = true);

	private void OnItemToggle(EntityUid uid, ItemToggleMeleeWeaponComponent itemToggleMelee, ItemToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent meleeWeapon = default(MeleeWeaponComponent);
		if (!((EntitySystem)this).TryComp<MeleeWeaponComponent>(uid, ref meleeWeapon))
		{
			return;
		}
		if (args.Activated)
		{
			if (itemToggleMelee.ActivatedDamage != null)
			{
				ItemToggleMeleeWeaponComponent itemToggleMeleeWeaponComponent = itemToggleMelee;
				if (itemToggleMeleeWeaponComponent.DeactivatedDamage == null)
				{
					itemToggleMeleeWeaponComponent.DeactivatedDamage = meleeWeapon.Damage;
				}
				meleeWeapon.Damage = itemToggleMelee.ActivatedDamage;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "Damage", (MetaDataComponent)null);
			}
			SoundSpecifier? hitSound = meleeWeapon.HitSound;
			if (hitSound == null || !((object)hitSound).Equals((object?)itemToggleMelee.ActivatedSoundOnHit))
			{
				meleeWeapon.HitSound = itemToggleMelee.ActivatedSoundOnHit;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "HitSound", (MetaDataComponent)null);
			}
			if (itemToggleMelee.ActivatedSoundOnHitNoDamage != null)
			{
				ItemToggleMeleeWeaponComponent itemToggleMeleeWeaponComponent = itemToggleMelee;
				if (itemToggleMeleeWeaponComponent.DeactivatedSoundOnHitNoDamage == null)
				{
					itemToggleMeleeWeaponComponent.DeactivatedSoundOnHitNoDamage = meleeWeapon.NoDamageSound;
				}
				meleeWeapon.NoDamageSound = itemToggleMelee.ActivatedSoundOnHitNoDamage;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "NoDamageSound", (MetaDataComponent)null);
			}
			if (itemToggleMelee.ActivatedSoundOnSwing != null)
			{
				ItemToggleMeleeWeaponComponent itemToggleMeleeWeaponComponent = itemToggleMelee;
				if (itemToggleMeleeWeaponComponent.DeactivatedSoundOnSwing == null)
				{
					itemToggleMeleeWeaponComponent.DeactivatedSoundOnSwing = meleeWeapon.SwingSound;
				}
				meleeWeapon.SwingSound = itemToggleMelee.ActivatedSoundOnSwing;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "SwingSound", (MetaDataComponent)null);
			}
			if (itemToggleMelee.DeactivatedSecret)
			{
				meleeWeapon.Hidden = false;
			}
		}
		else
		{
			if (itemToggleMelee.DeactivatedDamage != null)
			{
				meleeWeapon.Damage = itemToggleMelee.DeactivatedDamage;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "Damage", (MetaDataComponent)null);
			}
			meleeWeapon.HitSound = itemToggleMelee.DeactivatedSoundOnHit;
			((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "HitSound", (MetaDataComponent)null);
			if (itemToggleMelee.DeactivatedSoundOnHitNoDamage != null)
			{
				meleeWeapon.NoDamageSound = itemToggleMelee.DeactivatedSoundOnHitNoDamage;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "NoDamageSound", (MetaDataComponent)null);
			}
			if (itemToggleMelee.DeactivatedSoundOnSwing != null)
			{
				meleeWeapon.SwingSound = itemToggleMelee.DeactivatedSoundOnSwing;
				((EntitySystem)this).DirtyField<MeleeWeaponComponent>(uid, meleeWeapon, "SwingSound", (MetaDataComponent)null);
			}
			if (itemToggleMelee.DeactivatedSecret)
			{
				meleeWeapon.Hidden = true;
			}
		}
	}
}
