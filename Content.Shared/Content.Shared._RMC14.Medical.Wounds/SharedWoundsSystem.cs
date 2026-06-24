using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.DoAfter;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Medical.Wounds;

public abstract class SharedWoundsSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private RMCDoAfterSystem _rmcDoAfter;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedStackSystem _stacks;

	[Dependency]
	private IGameTiming _timing;

	private float _bloodlossMultiplier = 1f;

	private float _bleedTimeMultiplier = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, CMBleedAttemptEvent>((EntityEventRefHandler<MobStateComponent, CMBleedAttemptEvent>)OnMobStateBleedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundableComponent, DamageChangedEvent>((EntityEventRefHandler<WoundableComponent, DamageChangedEvent>)OnWoundableDamaged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundableComponent, CMBleedEvent>((EntityEventRefHandler<WoundableComponent, CMBleedEvent>)OnWoundableBleed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundedComponent, RejuvenateEvent>((EntityEventRefHandler<WoundedComponent, RejuvenateEvent>)OnWoundedRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundedComponent, EntityUnpausedEvent>((EntityEventRefHandler<WoundedComponent, EntityUnpausedEvent>)OnWoundedUnpaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundTreaterComponent, UseInHandEvent>((EntityEventRefHandler<WoundTreaterComponent, UseInHandEvent>)OnWoundTreaterUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundTreaterComponent, AfterInteractEvent>((EntityEventRefHandler<WoundTreaterComponent, AfterInteractEvent>)OnWoundTreaterAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WoundTreaterComponent, TreatWoundDoAfterEvent>((EntityEventRefHandler<WoundTreaterComponent, TreatWoundDoAfterEvent>)OnWoundTreaterDoAfter, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.CMBloodlossMultiplier, (Action<float>)delegate(float v)
		{
			_bloodlossMultiplier = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.CMBleedTimeMultiplier, (Action<float>)delegate(float v)
		{
			_bleedTimeMultiplier = v;
		}, true);
	}

	private void OnMobStateBleedAttempt(Entity<MobStateComponent> ent, ref CMBleedAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CurrentState == MobState.Dead)
		{
			args.Cancelled = true;
		}
	}

	private void OnWoundableDamaged(Entity<WoundableComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && args.DamageDelta != null)
		{
			FixedPoint2? limit = (((EntitySystem)this).HasComp<WoundTreaterComponent>(args.Tool) ? new FixedPoint2?(FixedPoint2.New(0.5f)) : ((FixedPoint2?)null));
			TryHealWounds(Entity<DamageableComponent, WoundedComponent>.op_Implicit((ent.Owner, args.Damageable)), args.DamageDelta, limit);
			if (args.DamageIncreased)
			{
				TryAddWound(ent, ent.Comp.BruteWoundGroup, args.DamageDelta, WoundType.Brute);
				TryAddWound(ent, ent.Comp.BurnWoundGroup, args.DamageDelta, WoundType.Burn);
			}
		}
	}

	private void OnWoundableBleed(Entity<WoundableComponent> ent, ref CMBleedEvent args)
	{
		args.Handled = true;
	}

	private void OnWoundedRejuvenate(Entity<WoundedComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<WoundedComponent>(Entity<WoundedComponent>.op_Implicit(ent));
	}

	private void OnWoundedUnpaused(Entity<WoundedComponent> ent, ref EntityUnpausedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Span<Wound> span = CollectionsMarshal.AsSpan(ent.Comp.Wounds);
		for (int i = 0; i < span.Length; i++)
		{
			ref Wound wound = ref span[i];
			if (wound.StopBleedAt.HasValue)
			{
				wound.StopBleedAt += args.PausedTime;
			}
		}
		ent.Comp.UpdateAt += args.PausedTime;
		((EntitySystem)this).Dirty<WoundedComponent>(ent, (MetaDataComponent)null);
	}

	private void OnWoundTreaterUseInHand(Entity<WoundTreaterComponent> ent, ref UseInHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		StartTreatment(args.User, args.User, ent, out var handled);
		((HandledEntityEventArgs)args).Handled = handled;
	}

	private void OnWoundTreaterAfterInteract(Entity<WoundTreaterComponent> ent, ref AfterInteractEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && args.Target.HasValue)
		{
			StartTreatment(args.User, args.Target.Value, ent, out var handled);
			((HandledEntityEventArgs)args).Handled = handled;
		}
	}

	private void OnWoundTreaterDoAfter(Entity<WoundTreaterComponent> treater, ref TreatWoundDoAfterEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(target2, ref damageable))
		{
			return;
		}
		if (!CanTreatPopup(user, target2, treater, out WoundedComponent wounded, out FixedPoint2 damage, out bool handled))
		{
			((HandledEntityEventArgs)args).Handled = handled;
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (damage != FixedPoint2.Zero)
		{
			DamageSpecifier total = _rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit((target2, damageable)), treater.Comp.Group, damage);
			_damageable.TryChangeDamage(target2, total, ignoreResistances: true, interruptsDoAfters: true, damageable, user, args.Used);
		}
		bool anyWounds = false;
		Span<Wound> span = CollectionsMarshal.AsSpan(wounded.Wounds);
		for (int i = 0; i < span.Length; i++)
		{
			ref Wound wound = ref span[i];
			if (wound.Type == treater.Comp.Wound && !wound.Treated && (treater.Comp.Treats || !(FixedPoint2.Abs(wound.Healed) < wound.Damage / 2f)))
			{
				wound.Treated = true;
				anyWounds = true;
			}
		}
		if (anyWounds)
		{
			((EntitySystem)this).Dirty(target2, (IComponent)(object)wounded, (MetaDataComponent)null);
		}
		else if (damage == FixedPoint2.Zero)
		{
			if (user == target2)
			{
				LocId? noWoundsOnUserPopup = treater.Comp.NoWoundsOnUserPopup;
				if (noWoundsOnUserPopup.HasValue)
				{
					LocId popup = noWoundsOnUserPopup.GetValueOrDefault();
					_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(popup)), user, user);
				}
			}
			else
			{
				LocId? noWoundsOnUserPopup = treater.Comp.NoWoundsOnTargetPopup;
				if (noWoundsOnUserPopup.HasValue)
				{
					LocId popup2 = noWoundsOnUserPopup.GetValueOrDefault();
					_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(popup2)), user, user);
				}
			}
			return;
		}
		_audio.PlayPredicted(treater.Comp.TreatEndSound, user, (EntityUid?)user, (AudioParams?)null);
		WoundedComponent wounded2;
		FixedPoint2 damage2;
		bool handle;
		if (treater.Comp.Consumable)
		{
			StackComponent stack = default(StackComponent);
			if (((EntitySystem)this).TryComp<StackComponent>(Entity<WoundTreaterComponent>.op_Implicit(treater), ref stack))
			{
				_stacks.Use(Entity<WoundTreaterComponent>.op_Implicit(treater), 2, stack);
			}
			else if (_net.IsServer)
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<WoundTreaterComponent>.op_Implicit(treater));
			}
		}
		else if (CanTreatPopup(user, target2, treater, out wounded2, out damage2, out handle, doPopups: false))
		{
			args.Repeat = true;
		}
		LocId? userPopup = treater.Comp.UserPopup;
		LocId? targetPopup = treater.Comp.TargetPopup;
		LocId? othersPopup = treater.Comp.OthersPopup;
		if (!args.Repeat)
		{
			if (treater.Comp.UserFinishPopup.HasValue)
			{
				userPopup = treater.Comp.UserFinishPopup;
			}
			if (treater.Comp.TargetFinishPopup.HasValue)
			{
				targetPopup = treater.Comp.TargetFinishPopup;
			}
			if (treater.Comp.OthersFinishPopup.HasValue)
			{
				othersPopup = treater.Comp.OthersFinishPopup;
			}
		}
		if (userPopup.HasValue)
		{
			SharedPopupSystem popup3 = _popup;
			ILocalizationManager loc = base.Loc;
			LocId? noWoundsOnUserPopup = userPopup;
			popup3.PopupClient(loc.GetString(noWoundsOnUserPopup.HasValue ? LocId.op_Implicit(noWoundsOnUserPopup.GetValueOrDefault()) : null, (ValueTuple<string, object>)("target", target2)), target2, user);
		}
		if (user != target2 && targetPopup.HasValue)
		{
			SharedPopupSystem popup4 = _popup;
			ILocalizationManager loc2 = base.Loc;
			LocId? noWoundsOnUserPopup = targetPopup;
			popup4.PopupEntity(loc2.GetString(noWoundsOnUserPopup.HasValue ? LocId.op_Implicit(noWoundsOnUserPopup.GetValueOrDefault()) : null, (ValueTuple<string, object>)("user", user)), target2, target2, PopupType.Large);
		}
		if (user != target2 && othersPopup.HasValue)
		{
			Filter others = Filter.PvsExcept(target2, 2f, (IEntityManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => e == user || e == target2));
			SharedPopupSystem popup5 = _popup;
			ILocalizationManager loc3 = base.Loc;
			LocId? noWoundsOnUserPopup = othersPopup;
			popup5.PopupEntity(loc3.GetString(noWoundsOnUserPopup.HasValue ? LocId.op_Implicit(noWoundsOnUserPopup.GetValueOrDefault()) : null, (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("target", target2)), user, others, recordReplay: true);
		}
	}

	private bool CanTreatPopup(EntityUid user, EntityUid target, Entity<WoundTreaterComponent> treater, [NotNullWhen(true)] out WoundedComponent? wounded, out FixedPoint2 damage, out bool handle, bool doPopups = true)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		handle = false;
		wounded = null;
		damage = FixedPoint2.Zero;
		if (!((EntitySystem)this).HasComp<WoundableComponent>(target) && !((EntitySystem)this).TryComp<WoundedComponent>(target, ref wounded))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<WoundableUntreatableComponent>(target))
		{
			return false;
		}
		IdentityEntity targetName = Identity.Name(target, (IEntityManager)(object)base.EntityManager, user);
		bool hasSkills = _skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(user), treater.Comp.Skills);
		if (!treater.Comp.CanUseUnskilled && !hasSkills)
		{
			if (doPopups)
			{
				_popup.PopupClient(base.Loc.GetString("cm-wounds-failed-unskilled", (ValueTuple<string, object>)("treater", treater.Owner)), target, user, PopupType.SmallCaution);
			}
			return false;
		}
		if (!((EntitySystem)this).TryComp<WoundedComponent>(target, ref wounded) || wounded.Wounds.Count == 0)
		{
			if (user == target)
			{
				if (doPopups)
				{
					LocId? noneSelfPopup = treater.Comp.NoneSelfPopup;
					if (noneSelfPopup.HasValue)
					{
						LocId selfPopup = noneSelfPopup.GetValueOrDefault();
						_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(selfPopup)), target, user);
					}
				}
				return false;
			}
			if (doPopups)
			{
				LocId? noneSelfPopup = treater.Comp.NoneOtherPopup;
				if (noneSelfPopup.HasValue)
				{
					LocId otherPopup = noneSelfPopup.GetValueOrDefault();
					_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(otherPopup), (ValueTuple<string, object>)("target", target)), target, user);
				}
			}
			return false;
		}
		FixedPoint2 max = (hasSkills ? treater.Comp.Damage : treater.Comp.UnskilledDamage) ?? FixedPoint2.Zero;
		bool untreated = false;
		bool surgeryUntreated = false;
		bool otherUntreated = false;
		FixedPoint2 divisor = FixedPoint2.New(2);
		Span<Wound> span = CollectionsMarshal.AsSpan(wounded.Wounds);
		for (int i = 0; i < span.Length; i++)
		{
			ref Wound wound = ref span[i];
			if (wound.Type == WoundType.Surgery && treater.Comp.Wound != WoundType.Surgery && !wound.Treated)
			{
				surgeryUntreated = true;
			}
			if (wound.Type != treater.Comp.Wound && !wound.Treated)
			{
				otherUntreated = true;
			}
			if (wound.Type != treater.Comp.Wound || (treater.Comp.Treats && wound.Treated))
			{
				continue;
			}
			if (max == FixedPoint2.Zero)
			{
				if (!wound.Treated)
				{
					untreated = true;
				}
				continue;
			}
			FixedPoint2 limit = wound.Damage / divisor;
			if (FixedPoint2.Abs(wound.Healed) < limit)
			{
				damage += -FixedPoint2.Min(limit - wound.Healed, FixedPoint2.Abs(max - damage));
			}
			if (damage <= max)
			{
				break;
			}
			if (!wound.Treated)
			{
				untreated = true;
			}
		}
		if (untreated || damage != FixedPoint2.Zero)
		{
			StackComponent stack = default(StackComponent);
			if (treater.Comp.Consumable && ((EntitySystem)this).TryComp<StackComponent>(Entity<WoundTreaterComponent>.op_Implicit(treater), ref stack) && _stacks.GetCount(Entity<WoundTreaterComponent>.op_Implicit(treater), stack) < 2)
			{
				_popup.PopupClient(base.Loc.GetString("cm-wounds-failed-not-enough", (ValueTuple<string, object>)("treater", treater.Owner)), target, user, PopupType.SmallCaution);
				return false;
			}
			return true;
		}
		if (doPopups)
		{
			if (surgeryUntreated)
			{
				_popup.PopupClient(base.Loc.GetString("cm-wounds-open-cut", (ValueTuple<string, object>)("target", targetName), (ValueTuple<string, object>)("treater", treater.Owner)), target, user, PopupType.SmallCaution);
			}
			else if (otherUntreated)
			{
				_popup.PopupClient(base.Loc.GetString("cm-wounds-cannot-treat", (ValueTuple<string, object>)("treater", treater.Owner)), target, user, PopupType.SmallCaution);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("cm-wounds-already-treated", (ValueTuple<string, object>)("target", target)), target, user);
			}
		}
		wounded = null;
		return false;
	}

	private void StartTreatment(EntityUid user, EntityUid target, Entity<WoundTreaterComponent> treater, out bool handled)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		handled = false;
		if (!CanTreatPopup(user, target, treater, out WoundedComponent _, out FixedPoint2 damage, out handled))
		{
			return;
		}
		handled = true;
		TimeSpan delay = _skills.GetDelay(user, Entity<WoundTreaterComponent>.op_Implicit(treater));
		if (delay > TimeSpan.Zero)
		{
			_popup.PopupClient(base.Loc.GetString("cm-wounds-start-fumbling", (ValueTuple<string, object>)("name", treater.Owner)), target, user);
		}
		TimeSpan scaling = treater.Comp.ScalingDoAfter;
		scaling *= (double)_skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), treater.Comp.DoAfterSkill, treater.Comp.DoAfterSkillMultipliers);
		if (user == target)
		{
			scaling *= (double)treater.Comp.SelfTargetDoAfterMultiplier;
		}
		if (scaling > TimeSpan.Zero)
		{
			TimeSpan scaledDoAfter = scaling * Math.Abs(damage.Double());
			if (scaledDoAfter > TimeSpan.Zero)
			{
				delay += scaledDoAfter;
			}
		}
		if (user != target && treater.Comp.TargetStartPopup.HasValue)
		{
			SharedPopupSystem popup = _popup;
			ILocalizationManager loc = base.Loc;
			LocId? targetStartPopup = treater.Comp.TargetStartPopup;
			popup.PopupEntity(loc.GetString(targetStartPopup.HasValue ? LocId.op_Implicit(targetStartPopup.GetValueOrDefault()) : null, (ValueTuple<string, object>)("user", user)), target, target, PopupType.Medium);
		}
		TreatWoundDoAfterEvent ev = new TreatWoundDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<WoundTreaterComponent>.op_Implicit(treater), target, Entity<WoundTreaterComponent>.op_Implicit(treater))
		{
			BreakOnMove = true,
			BreakOnHandChange = true,
			NeedHand = true,
			CancelDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent,
			TargetEffect = EntProtoId.op_Implicit("RMCEffectHealBusy"),
			MovementThreshold = 0.5f
		};
		_rmcDoAfter.TryCancel(Entity<DoAfterComponent>.op_Implicit(user), treater.Comp.CurrentDoAfter);
		if (_doAfter.TryStartDoAfter(doAfter, out var doAfterId))
		{
			treater.Comp.CurrentDoAfter = doAfterId.Value.Index;
			((EntitySystem)this).DirtyField<WoundTreaterComponent>(Entity<WoundTreaterComponent>.op_Implicit(treater), treater.Comp, "CurrentDoAfter", (MetaDataComponent)null);
			_audio.PlayPredicted(treater.Comp.TreatBeginSound, user, (EntityUid?)user, (AudioParams?)null);
		}
	}

	private void TryAddWound(Entity<WoundableComponent> woundable, ProtoId<DamageGroupPrototype> groupId, DamageSpecifier damage, WoundType type)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		DamageGroupPrototype group = default(DamageGroupPrototype);
		if (_prototypes.TryIndex<DamageGroupPrototype>(groupId, ref group) && damage.TryGetDamageInGroup(group, out var total) && !(total <= FixedPoint2.Zero))
		{
			AddWound(Entity<WoundableComponent>.op_Implicit((Entity<WoundableComponent>.op_Implicit(woundable), Entity<WoundableComponent>.op_Implicit(woundable))), total, type);
		}
	}

	public void TryHealWounds(Entity<DamageableComponent, WoundedComponent?> wounded, DamageSpecifier damage, FixedPoint2? limit = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Entity<DamageableComponent, WoundedComponent> val = wounded;
		EntityUid val2 = default(EntityUid);
		DamageableComponent damageableComponent = default(DamageableComponent);
		WoundedComponent woundedComponent = default(WoundedComponent);
		val.Deconstruct(ref val2, ref damageableComponent, ref woundedComponent);
		EntityUid uid = val2;
		DamageableComponent damageable = damageableComponent;
		WoundedComponent comp = woundedComponent;
		if (((EntitySystem)this).Resolve<WoundedComponent>(Entity<DamageableComponent, WoundedComponent>.op_Implicit(wounded), ref comp, false) && comp.Wounds.Count != 0)
		{
			HealOrRemove(Entity<DamageableComponent, WoundedComponent>.op_Implicit((uid, damageable, comp)), comp.BruteWoundGroup, WoundType.Brute, damage, limit);
			HealOrRemove(Entity<DamageableComponent, WoundedComponent>.op_Implicit((uid, damageable, comp)), comp.BurnWoundGroup, WoundType.Burn, damage, limit);
		}
	}

	private void HealOrRemove(Entity<DamageableComponent, WoundedComponent> wounded, ProtoId<DamageGroupPrototype> group, WoundType type, DamageSpecifier damage, FixedPoint2? limit = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		DamageGroupPrototype bruteGroup = default(DamageGroupPrototype);
		FixedPoint2 bruteDelta;
		if (wounded.Comp1.DamagePerGroup.GetValueOrDefault(ProtoId<DamageGroupPrototype>.op_Implicit(group)) <= FixedPoint2.Zero)
		{
			RemoveWounds(Entity<WoundedComponent>.op_Implicit((Entity<DamageableComponent, WoundedComponent>.op_Implicit(wounded), Entity<DamageableComponent, WoundedComponent>.op_Implicit(wounded))), type);
		}
		else if (_prototypes.TryIndex<DamageGroupPrototype>(group, ref bruteGroup) && damage.TryGetDamageInGroup(bruteGroup, out bruteDelta))
		{
			TryHealWounds(Entity<WoundedComponent>.op_Implicit((Entity<DamageableComponent, WoundedComponent>.op_Implicit(wounded), Entity<DamageableComponent, WoundedComponent>.op_Implicit(wounded))), type, bruteDelta, limit);
		}
	}

	public void TryHealWounds(Entity<WoundedComponent?> wounded, WoundType type, FixedPoint2 amount, FixedPoint2? limit = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (amount >= FixedPoint2.Zero || !((EntitySystem)this).Resolve<WoundedComponent>(Entity<WoundedComponent>.op_Implicit(wounded), ref wounded.Comp, false) || wounded.Comp.Wounds.Count == 0)
		{
			return;
		}
		FixedPoint2 valueOrDefault = limit.GetValueOrDefault();
		if (!limit.HasValue)
		{
			valueOrDefault = FixedPoint2.New(1f);
			limit = valueOrDefault;
		}
		Span<Wound> wounds = CollectionsMarshal.AsSpan(wounded.Comp.Wounds);
		for (int i = 0; i < wounds.Length; i++)
		{
			ref Wound wound = ref wounds[i];
			if (wound.Type != type)
			{
				continue;
			}
			FixedPoint2 healing = -FixedPoint2.Max(-(wound.Damage * limit.Value - wound.Healed), amount);
			if (!(healing == FixedPoint2.Zero))
			{
				wound.Healed += healing;
				amount += healing;
				if (amount == FixedPoint2.Zero)
				{
					break;
				}
			}
		}
	}

	public void AddWound(Entity<WoundableComponent?> woundable, FixedPoint2 total, WoundType type, TimeSpan? fixedDuration = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WoundableComponent>(Entity<WoundableComponent>.op_Implicit(woundable), ref woundable.Comp, false))
		{
			return;
		}
		float bloodloss = 0f;
		if (type != WoundType.Burn && total >= woundable.Comp.BleedMinDamage)
		{
			bloodloss = total.Float() * woundable.Comp.BloodLossMultiplier;
		}
		bloodloss *= _bloodlossMultiplier;
		TimeSpan time = _timing.CurTime;
		TimeSpan duration = fixedDuration ?? (total.Float() * woundable.Comp.DurationMultiplier * _bleedTimeMultiplier);
		WoundedComponent wounded = default(WoundedComponent);
		if (((EntitySystem)this).EnsureComp<WoundedComponent>(Entity<WoundableComponent>.op_Implicit(woundable), ref wounded))
		{
			Span<Wound> wounds = CollectionsMarshal.AsSpan(wounded.Wounds);
			for (int i = wounds.Length - 1; i >= 0; i--)
			{
				ref Wound wound = ref wounds[i];
				if (wound.Type == type)
				{
					TimeSpan? stopBleedAt = wound.StopBleedAt;
					if (stopBleedAt.HasValue)
					{
						TimeSpan stopBleedAt2 = stopBleedAt.GetValueOrDefault();
						if (!(time >= stopBleedAt2) && wound.Bloodloss > 0f)
						{
							wound.Bloodloss += bloodloss / 1.5f;
							wound.StopBleedAt = stopBleedAt2 + duration / 1.5;
							bloodloss = 0f;
							duration = TimeSpan.Zero;
							break;
						}
					}
				}
			}
		}
		wounded.BruteWoundGroup = woundable.Comp.BruteWoundGroup;
		wounded.BurnWoundGroup = woundable.Comp.BurnWoundGroup;
		TimeSpan? newDuration = ((duration == TimeSpan.MaxValue) ? ((TimeSpan?)null) : new TimeSpan?(time + duration));
		wounded.Wounds.Add(new Wound(total, FixedPoint2.Zero, bloodloss, newDuration, type, Treated: false));
		((EntitySystem)this).Dirty(Entity<WoundableComponent>.op_Implicit(woundable), (IComponent)(object)wounded, (MetaDataComponent)null);
	}

	public void RemoveWounds(Entity<WoundedComponent?> wounded, WoundType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WoundedComponent>(Entity<WoundedComponent>.op_Implicit(wounded), ref wounded.Comp, false))
		{
			return;
		}
		List<Wound> wounds = wounded.Comp.Wounds;
		for (int i = wounds.Count - 1; i >= 0; i--)
		{
			if (wounds[i].Type == type)
			{
				Extensions.RemoveSwap<Wound>((IList<Wound>)wounds, i);
			}
		}
	}

	public bool HasUntreated(Entity<WoundedComponent?> wounded, ProtoId<DamageGroupPrototype> group)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WoundedComponent>(Entity<WoundedComponent>.op_Implicit(wounded), ref wounded.Comp, false) || wounded.Comp.Wounds.Count == 0)
		{
			return false;
		}
		WoundType type;
		if (group == wounded.Comp.BruteWoundGroup)
		{
			type = WoundType.Brute;
		}
		else
		{
			if (!(group == wounded.Comp.BurnWoundGroup))
			{
				return false;
			}
			type = WoundType.Burn;
		}
		Span<Wound> span = CollectionsMarshal.AsSpan(wounded.Comp.Wounds);
		for (int i = 0; i < span.Length; i++)
		{
			ref Wound wound = ref span[i];
			if (wound.Type == type && !wound.Treated)
			{
				return true;
			}
		}
		return false;
	}
}
