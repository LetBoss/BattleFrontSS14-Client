using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Jittering;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Heal;

public abstract class SharedXenoHealSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedRMCFlammableSystem _flammable;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private SharedInteractionSystem _interact;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MobThresholdSystem _mobThreshold;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private QueenEyeSystem _queenEye;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private XenoEnergySystem _xenoEnergy;

	[Dependency]
	private SharedXenoAnnounceSystem _xenoAnnounce;

	[Dependency]
	private XenoStrainSystem _xenoStrain;

	[Dependency]
	private StatusEffectsSystem _status;

	private static readonly ProtoId<DamageGroupPrototype> BruteGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	private static readonly ProtoId<DamageGroupPrototype> BurnGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Burn");

	private static readonly ProtoId<DamageTypePrototype> BluntGroup = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private readonly HashSet<Entity<XenoComponent>> _xenos = new HashSet<Entity<XenoComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoHealComponent, XenoHealActionEvent>((EntityEventRefHandler<XenoHealComponent, XenoHealActionEvent>)OnXenoHealAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoApplySalveActionEvent>((EntityEventRefHandler<XenoComponent, XenoApplySalveActionEvent>)OnXenoApplySalveAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoSacrificeHealActionEvent>((EntityEventRefHandler<XenoComponent, XenoSacrificeHealActionEvent>)OnXenoSacrificeHealAction, (Type[])null, (Type[])null);
	}

	private void OnXenoHealAction(Entity<XenoHealComponent> ent, ref XenoHealActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(ent.Owner)) && !_queenEye.CanSeeTarget(Entity<QueenEyeActionComponent>.op_Implicit(ent.Owner), args.Target)) || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_xenos.Clear();
		_entityLookup.GetEntitiesInRange<XenoComponent>(args.Target, (float)ent.Comp.Radius, _xenos, (LookupFlags)110);
		if (_xenos.Count == 0)
		{
			return;
		}
		string msg = "We channel our plasma to heal our sisters' wounds around this area.";
		_popup.PopupClient(msg, args.Target, Entity<XenoHealComponent>.op_Implicit(ent), PopupType.Large);
		foreach (Entity<XenoComponent> xeno in _xenos)
		{
			if (!_mobState.IsDead(Entity<XenoComponent>.op_Implicit(xeno)) && !_flammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(xeno.Owner)) && _mobThreshold.TryGetIncapThreshold(Entity<XenoComponent>.op_Implicit(xeno), out var threshold) && !(threshold <= FixedPoint2.Zero) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(xeno.Owner)))
			{
				XenoBeingHealedComponent xenoBeingHealedComponent = ((EntitySystem)this).EnsureComp<XenoBeingHealedComponent>(Entity<XenoComponent>.op_Implicit(xeno));
				XenoHealStack healStack = new XenoHealStack
				{
					HealAmount = threshold.Value * ent.Comp.Percentage / (ent.Comp.Duration.TotalSeconds * 10.0) * (ent.Comp.TimeBetweenHeals.TotalSeconds * 10.0),
					Charges = (int)(ent.Comp.Duration.TotalSeconds / ent.Comp.TimeBetweenHeals.TotalSeconds),
					TimeBetweenHeals = ent.Comp.TimeBetweenHeals
				};
				xenoBeingHealedComponent.HealStacks.Add(healStack);
				if (_net.IsServer)
				{
					EntProtoId? healEffect = ent.Comp.HealEffect;
					((EntitySystem)this).SpawnAttachedTo(healEffect.HasValue ? EntProtoId.op_Implicit(healEffect.GetValueOrDefault()) : null, xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
				}
			}
		}
	}

	private void OnXenoApplySalveAction(Entity<XenoComponent> ent, ref XenoApplySalveActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		LocId? failureMessageId = null;
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-not-xeno-failure");
		}
		if (ent.Owner == target)
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-self-failure");
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-hostile-failure");
		}
		if (!_interact.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(ent.Owner), Entity<TransformComponent>.op_Implicit(target), args.Range))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-too-far-away-failure");
		}
		if (_mobState.IsDead(target))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-dead-failure");
		}
		if (_flammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(target)))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-on-fire-failure");
		}
		DamageableComponent damageComp = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(target, ref damageComp) && damageComp.TotalDamage == 0)
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-apply-salve-target-full-health-failure");
		}
		if (failureMessageId.HasValue)
		{
			SharedPopupSystem popup = _popup;
			ILocalizationManager loc = base.Loc;
			LocId? val = failureMessageId;
			popup.PopupClient(loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null, (ValueTuple<string, object>)("target_xeno", target)), Entity<XenoComponent>.op_Implicit(ent));
			return;
		}
		FixedPoint2 totalHealAmount = args.StandardHealAmount;
		FixedPoint2 damageTakenModifier = args.DamageTakenModifier;
		bool healedHealerOrSmallXeno = false;
		RMCSizeComponent sizeComp = default(RMCSizeComponent);
		if (((EntitySystem)this).TryComp<RMCSizeComponent>(target, ref sizeComp) && (sizeComp.Size == RMCSizes.Small || sizeComp.Size == RMCSizes.VerySmallXeno))
		{
			totalHealAmount = args.SmallHealAmount;
			damageTakenModifier = 1;
			healedHealerOrSmallXeno = true;
		}
		if (_xenoStrain.AreSameStrain(Entity<XenoStrainComponent>.op_Implicit(ent.Owner), Entity<XenoStrainComponent>.op_Implicit(target)))
		{
			damageTakenModifier = 1;
			healedHealerOrSmallXeno = true;
		}
		if (!_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(ent.Owner), totalHealAmount * args.PlasmaCostModifier))
		{
			return;
		}
		FixedPoint2 damageTaken = totalHealAmount * damageTakenModifier;
		DamageSpecifier damageTakenSpecifier = new DamageSpecifier
		{
			DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(BluntGroup)] = damageTaken }
		};
		DamageableComponent damage = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<XenoComponent>.op_Implicit(ent), ref damage))
		{
			_damageable.AddDamage(ent.Owner, damage, damageTakenSpecifier);
		}
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-apply-salve-self", (ValueTuple<string, object>)("target_xeno", target)), Entity<XenoComponent>.op_Implicit(ent), PopupType.Medium);
		((HandledEntityEventArgs)args).Handled = true;
		XenoBeingHealedComponent heal = ((EntitySystem)this).EnsureComp<XenoBeingHealedComponent>(target);
		XenoHealStack healStack = new XenoHealStack
		{
			Charges = (int)(args.TotalHealDuration.TotalSeconds / args.TimeBetweenHeals.TotalSeconds),
			TimeBetweenHeals = args.TimeBetweenHeals
		};
		healStack.HealAmount = totalHealAmount / healStack.Charges;
		healStack.NextHealAt = _timing.CurTime + healStack.TimeBetweenHeals;
		heal.HealStacks.Add(healStack);
		heal.ParallizeHealing = true;
		((EntitySystem)this).EnsureComp<RecentlySalvedComponent>(Entity<XenoComponent>.op_Implicit(ent)).ExpiresAt = _timing.CurTime + args.TotalHealDuration;
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(args.HealEffect), target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		_jitter.DoJitter(target, TimeSpan.FromSeconds(1L), refresh: true, 80f, 8f, forceValueChange: true);
		_audio.PlayPredicted(args.HealSound, target.ToCoordinates(), (EntityUid?)Entity<XenoComponent>.op_Implicit(ent), (AudioParams?)null);
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-apply-salve-target", (ValueTuple<string, object>)("healer_xeno", ent)), target, PopupType.SmallCaution);
		XenoEnergyComponent xenoEnergyComp = default(XenoEnergyComponent);
		if (!healedHealerOrSmallXeno && ((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoComponent>.op_Implicit(ent), ref xenoEnergyComp) && !_xenoEnergy.HasEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoComponent>.op_Implicit(ent), xenoEnergyComp)), xenoEnergyComp.Max))
		{
			_xenoEnergy.AddEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoComponent>.op_Implicit(ent), xenoEnergyComp)), (int)damageTaken, popup: false);
			if (_xenoEnergy.HasEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoComponent>.op_Implicit(ent), xenoEnergyComp)), xenoEnergyComp.Max))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-sacrifice-heal-will-respawn"), Entity<XenoComponent>.op_Implicit(ent), PopupType.Large);
			}
		}
		((EntitySystem)this).Dirty(target, (IComponent)(object)heal, (MetaDataComponent)null);
	}

	private void OnXenoSacrificeHealAction(Entity<XenoComponent> ent, ref XenoSacrificeHealActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		LocId? failureMessageId = null;
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-not-xeno-failure");
		}
		if (ent.Owner == target)
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-self-failure");
		}
		RMCSizeComponent rmcSizeComp = default(RMCSizeComponent);
		if (((EntitySystem)this).HasComp<XenoParasiteComponent>(target) || (((EntitySystem)this).TryComp<RMCSizeComponent>(target, ref rmcSizeComp) && rmcSizeComp.Size == RMCSizes.VerySmallXeno))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-low-level-failure");
		}
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-hostile-failure");
		}
		if (!_interact.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(ent.Owner), Entity<TransformComponent>.op_Implicit(target), args.Range))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-too-far-away-failure");
		}
		if (_mobState.IsDead(target))
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-dead-failure");
		}
		DamageableComponent targetDamageComp = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(target, ref targetDamageComp) && targetDamageComp.TotalDamage == 0)
		{
			failureMessageId = LocId.op_Implicit("rmc-xeno-sacrifice-heal-target-full-health-failure");
		}
		if (failureMessageId.HasValue)
		{
			SharedPopupSystem popup = _popup;
			ILocalizationManager loc = base.Loc;
			LocId? val = failureMessageId;
			popup.PopupClient(loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null, (ValueTuple<string, object>)("target_xeno", target)), Entity<XenoComponent>.op_Implicit(ent));
		}
		else
		{
			DamageableComponent userDamageComp = default(DamageableComponent);
			MobThresholdsComponent targetThresholdsComp = default(MobThresholdsComponent);
			MobThresholdsComponent userThresholdsComp = default(MobThresholdsComponent);
			if (!((EntitySystem)this).TryComp<DamageableComponent>(target, ref targetDamageComp) || !((EntitySystem)this).TryComp<DamageableComponent>(Entity<XenoComponent>.op_Implicit(ent), ref userDamageComp) || !((EntitySystem)this).TryComp<MobThresholdsComponent>(target, ref targetThresholdsComp) || !((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<XenoComponent>.op_Implicit(ent), ref userThresholdsComp))
			{
				return;
			}
			_flammable.Extinguish(Entity<FlammableComponent>.op_Implicit(target));
			FixedPoint2? targetCriticalThreshold = null;
			foreach (KeyValuePair<FixedPoint2, MobState> threshold in targetThresholdsComp.Thresholds)
			{
				if (threshold.Value == MobState.Critical)
				{
					targetCriticalThreshold = threshold.Key;
				}
			}
			FixedPoint2? userDeathThreshold = null;
			foreach (KeyValuePair<FixedPoint2, MobState> threshold2 in userThresholdsComp.Thresholds)
			{
				if (threshold2.Value == MobState.Dead)
				{
					userDeathThreshold = threshold2.Key;
				}
			}
			if (!userDeathThreshold.HasValue || !targetCriticalThreshold.HasValue)
			{
				return;
			}
			SacrificialHealShout(Entity<XenoComponent>.op_Implicit(ent));
			_xenoAnnounce.AnnounceSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), base.Loc.GetString("rmc-xeno-sacrifice-heal-target-announcement", (ValueTuple<string, object>)("healer_xeno", ent), (ValueTuple<string, object>)("target_xeno", target)), null, PopupType.Large);
			_popup.PopupPredicted(base.Loc.GetString("rmc-xeno-sacrifice-heal-target-enviorment", (ValueTuple<string, object>)("healer_xeno", ent), (ValueTuple<string, object>)("target_xeno", target)), target, Entity<XenoComponent>.op_Implicit(ent), PopupType.Medium);
			FixedPoint2 diffToThreshold = targetDamageComp.TotalDamage - targetCriticalThreshold.Value;
			if (diffToThreshold > 0)
			{
				Heal(target, diffToThreshold);
			}
			FixedPoint2 userTotalDamage = userDamageComp.TotalDamage;
			FixedPoint2 healAmount = (userDeathThreshold.Value - userTotalDamage) * args.TransferProportion;
			Heal(target, healAmount);
			ProtoId<StatusEffectPrototype>[] ailmentsRemove = args.AilmentsRemove;
			foreach (ProtoId<StatusEffectPrototype> status in ailmentsRemove)
			{
				_status.TryRemoveStatusEffect(target, ProtoId<StatusEffectPrototype>.op_Implicit(status));
			}
			base.EntityManager.RemoveComponents(target, args.ComponentsRemove);
			_jitter.DoJitter(target, TimeSpan.FromSeconds(1L), refresh: true, 80f, 8f, forceValueChange: true);
			XenoEnergyComponent xenoEnergyComp = default(XenoEnergyComponent);
			if (((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoComponent>.op_Implicit(ent), ref xenoEnergyComp) && _xenoEnergy.HasEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoComponent>.op_Implicit(ent), xenoEnergyComp)), xenoEnergyComp.Max))
			{
				EntityCoordinates corpsePosition = _transform.GetMoverCoordinates(Entity<XenoComponent>.op_Implicit(ent));
				if (GetHiveCore(Entity<XenoComponent>.op_Implicit(ent)))
				{
					SacrificialHealRespawn(Entity<XenoComponent>.op_Implicit(ent), args.RespawnDelay);
				}
				else
				{
					SacrificialHealRespawn(Entity<XenoComponent>.op_Implicit(ent), args.RespawnDelay, atCorpse: true, corpsePosition);
				}
			}
			else
			{
				SacrificeNoRespawn(Entity<XenoComponent>.op_Implicit(ent));
			}
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(args.HealEffect), target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
				_body.GibBody(Entity<XenoComponent>.op_Implicit(ent));
			}
		}
	}

	public void Heal(EntityUid target, FixedPoint2 amount)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = _rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit(target), BruteGroup, amount);
		FixedPoint2 totalHeal = damage.GetTotal();
		FixedPoint2 leftover = amount - totalHeal;
		if (leftover > FixedPoint2.Zero)
		{
			damage = _rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit(target), BurnGroup, leftover, damage);
		}
		_damageable.TryChangeDamage(target, -damage, ignoreResistances: true);
	}

	public void CreateHealStacks(EntityUid target, FixedPoint2 healAmount, TimeSpan timeBetweenHeals, int charges, TimeSpan nextHealAt, bool ignoreFire = false)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (ignoreFire || !_flammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(target)))
		{
			XenoBeingHealedComponent xenoBeingHealedComponent = ((EntitySystem)this).EnsureComp<XenoBeingHealedComponent>(target);
			XenoHealStack healStack = new XenoHealStack
			{
				Charges = charges,
				TimeBetweenHeals = timeBetweenHeals
			};
			healStack.HealAmount = healAmount;
			healStack.NextHealAt = _timing.CurTime + nextHealAt;
			xenoBeingHealedComponent.HealStacks.Add(healStack);
			xenoBeingHealedComponent.ParallizeHealing = true;
		}
	}

	private bool GetHiveCore(EntityUid xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent> cores = ((EntitySystem)this).EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent>();
		EntityUid uid = default(EntityUid);
		HiveCoreComponent hiveCoreComponent = default(HiveCoreComponent);
		HiveMemberComponent hiveMemberComponent = default(HiveMemberComponent);
		while (cores.MoveNext(ref uid, ref hiveCoreComponent, ref hiveMemberComponent))
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveMemberComponent>.op_Implicit(uid)) && !_mobState.IsDead(uid))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void SacrificialHealShout(EntityUid xeno)
	{
	}

	protected virtual void SacrificialHealRespawn(EntityUid xeno, TimeSpan time, bool atCorpse = false, EntityCoordinates? corpse = null)
	{
	}

	protected virtual void SacrificeNoRespawn(EntityUid xeno)
	{
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoBeingHealedComponent> healQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoBeingHealedComponent>();
		EntityUid uid = default(EntityUid);
		XenoBeingHealedComponent heal = default(XenoBeingHealedComponent);
		while (healQuery.MoveNext(ref uid, ref heal))
		{
			if (heal.HealStacks.Count == 0 || _mobState.IsDead(uid))
			{
				((EntitySystem)this).RemCompDeferred<XenoBeingHealedComponent>(uid);
				continue;
			}
			List<XenoHealStack> finishedStacks = new List<XenoHealStack>();
			foreach (XenoHealStack healStack in heal.HealStacks)
			{
				if (healStack.Charges <= 0)
				{
					finishedStacks.Add(healStack);
				}
				else if (!(healStack.NextHealAt > time))
				{
					((EntitySystem)this).Dirty(uid, (IComponent)(object)heal, (MetaDataComponent)null);
					Heal(uid, healStack.HealAmount);
					healStack.NextHealAt = time + healStack.TimeBetweenHeals;
					healStack.Charges--;
					if (!heal.ParallizeHealing)
					{
						break;
					}
				}
			}
			foreach (XenoHealStack stack in finishedStacks)
			{
				heal.HealStacks.Remove(stack);
			}
		}
	}
}
