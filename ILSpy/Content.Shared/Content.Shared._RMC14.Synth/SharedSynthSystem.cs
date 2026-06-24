using System;
using System.Collections.Generic;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.StatusEffect;
using Content.Shared.Bed.Sleep;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Synth;

public abstract class SharedSynthSystem : EntitySystem
{
	[Dependency]
	private RMCRepairableSystem _repairable;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private RMCStatusEffectSystem _rmcStatusEffects;

	[Dependency]
	private MobThresholdSystem _mobThreshold;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, MapInitEvent>((EntityEventRefHandler<SynthComponent, MapInitEvent>)OnMapInit, (Type[])null, new Type[1] { typeof(SharedBloodstreamSystem) });
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, AttackAttemptEvent>((EntityEventRefHandler<SynthComponent, AttackAttemptEvent>)OnMeleeAttempted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, ShotAttemptedEvent>((EntityEventRefHandler<SynthComponent, ShotAttemptedEvent>)OnShotAttempted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, TryingToSleepEvent>((EntityEventRefHandler<SynthComponent, TryingToSleepEvent>)OnSleepAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, InteractUsingEvent>((EntityEventRefHandler<SynthComponent, InteractUsingEvent>)OnSynthInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SynthComponent, RMCSynthRepairEvent>((EntityEventRefHandler<SynthComponent, RMCSynthRepairEvent>)OnSynthRepairDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UseOnSynthBlockedComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<UseOnSynthBlockedComponent, BeforeRangedInteractEvent>)OnSynthBlockedBeforeRangedInteract, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<SynthComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MakeSynth(ent);
	}

	protected virtual void MakeSynth(Entity<SynthComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype addComponents = default(EntityPrototype);
		if (_prototypes.TryIndex(ent.Comp.AddComponents, ref addComponents))
		{
			base.EntityManager.AddComponents(ent.Owner, addComponents.Components, true);
		}
		EntityPrototype removeComponents = default(EntityPrototype);
		if (_prototypes.TryIndex(ent.Comp.RemoveComponents, ref removeComponents))
		{
			base.EntityManager.RemoveComponents(ent.Owner, removeComponents.Components);
		}
		if (ent.Comp.StunResistance.HasValue)
		{
			_rmcStatusEffects.GiveStunResistance(ent.Owner, ent.Comp.StunResistance.Value);
		}
		FixedIdentityComponent fixedIdentity = default(FixedIdentityComponent);
		if (((EntitySystem)this).TryComp<FixedIdentityComponent>(ent.Owner, ref fixedIdentity))
		{
			fixedIdentity.Name = ent.Comp.FixedIdentityReplacement;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)fixedIdentity, (MetaDataComponent)null);
		}
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		if (((EntitySystem)this).TryComp<MobThresholdsComponent>(ent.Owner, ref thresholds))
		{
			_mobThreshold.SetMobStateThreshold(ent.Owner, ent.Comp.CritThreshold, MobState.Critical, thresholds);
		}
		RMCHealthIconsComponent healthIcons = default(RMCHealthIconsComponent);
		if (((EntitySystem)this).TryComp<RMCHealthIconsComponent>(ent.Owner, ref healthIcons))
		{
			healthIcons.Icons = ent.Comp.HealthIconOverrides;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)healthIcons, (MetaDataComponent)null);
		}
		((EntitySystem)this).RemCompDeferred<RMCRevivableComponent>(ent.Owner);
		((EntitySystem)this).RemCompDeferred<SlowOnDamageComponent>(ent.Owner);
	}

	private void OnMeleeAttempted(Entity<SynthComponent> ent, ref AttackAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!(ent.Owner != args.Uid) && !ent.Comp.CanUseMeleeWeapons && args.Weapon.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
			DoSynthUnableToUsePopup(Entity<SynthComponent>.op_Implicit(ent), args.Weapon.Value.Owner);
		}
	}

	private void OnShotAttempted(Entity<SynthComponent> ent, ref ShotAttemptedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CanUseGuns)
		{
			args.Cancel();
			DoSynthUnableToUsePopup(Entity<SynthComponent>.op_Implicit(ent), Entity<GunComponent>.op_Implicit(args.Used));
		}
	}

	private void OnSleepAttempt(Entity<SynthComponent> ent, ref TryingToSleepEvent args)
	{
		args.Cancelled = true;
	}

	private void OnSynthInteractUsing(Entity<SynthComponent> synth, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid used = args.Used;
		EntityUid user = args.User;
		bool selfRepair = args.User == synth.Owner;
		RMCSynthRepairEvent ev = new RMCSynthRepairEvent();
		TimeSpan repairTime = (selfRepair ? synth.Comp.SelfRepairTime : synth.Comp.RepairTime);
		EntityManager entityManager = base.EntityManager;
		EntityUid? eventTarget = Entity<SynthComponent>.op_Implicit(synth);
		EntityUid? used2 = args.Used;
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, repairTime, ev, eventTarget, null, used2)
		{
			BreakOnMove = true,
			BreakOnDropItem = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (((EntitySystem)this).HasComp<BlowtorchComponent>(used) && _tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(synth.Comp.RepairQuality)))
		{
			if (HasDamage(Entity<SynthComponent>.op_Implicit(synth), synth.Comp.WelderDamageGroup) && _repairable.UseFuel(args.Used, args.User, 5, attempt: true))
			{
				((HandledEntityEventArgs)args).Handled = true;
				if (_doAfter.TryStartDoAfter(doAfter))
				{
					string selfMsg = base.Loc.GetString("rmc-synth-repair-brute-start-self", new(string, object)[4]
					{
						("user", user),
						("target", synth),
						("tool", used),
						("limb", "chest")
					});
					string othersMsg = base.Loc.GetString("rmc-synth-repair-brute-start-others", new(string, object)[4]
					{
						("user", user),
						("target", synth),
						("tool", used),
						("limb", "chest")
					});
					if (selfRepair)
					{
						_popup.PopupPredicted(selfMsg, othersMsg, user, user);
					}
				}
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-repairable-not-damaged", (ValueTuple<string, object>)("target", synth)), user, user, PopupType.SmallCaution);
			}
		}
		else
		{
			if (!((EntitySystem)this).HasComp<RMCCableCoilComponent>(used))
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			if (HasDamage(Entity<SynthComponent>.op_Implicit(synth), synth.Comp.CableCoilDamageGroup))
			{
				if (_doAfter.TryStartDoAfter(doAfter))
				{
					string selfMsg2 = base.Loc.GetString("rmc-synth-repair-burn-start-self", new(string, object)[4]
					{
						("user", user),
						("target", synth),
						("tool", used),
						("limb", "chest")
					});
					string othersMsg2 = base.Loc.GetString("rmc-synth-repair-burn-start-others", new(string, object)[4]
					{
						("user", user),
						("target", synth),
						("tool", used),
						("limb", "chest")
					});
					if (selfRepair)
					{
						_popup.PopupPredicted(selfMsg2, othersMsg2, user, user);
					}
				}
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("rmc-repairable-not-damaged", (ValueTuple<string, object>)("target", synth)), user, user, PopupType.SmallCaution);
			}
		}
	}

	private void OnSynthRepairDoAfter(Entity<SynthComponent> synth, ref RMCSynthRepairEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? used = args.Used;
		EntityUid user = args.User;
		if (!used.HasValue)
		{
			return;
		}
		if (((EntitySystem)this).HasComp<BlowtorchComponent>(used) && _repairable.UseFuel(used.Value, user, 5))
		{
			if (synth.Comp.WelderDamageToRepair != null)
			{
				_damageable.TryChangeDamage(Entity<SynthComponent>.op_Implicit(synth), synth.Comp.WelderDamageToRepair, ignoreResistances: true, interruptsDoAfters: false, null, user);
			}
			string selfMsg = base.Loc.GetString("rmc-synth-repair-brute-finish-self", new(string, object)[4]
			{
				("user", user),
				("target", synth),
				("tool", used),
				("limb", "chest")
			});
			string othersMsg = base.Loc.GetString("rmc-synth-repair-brute-finish", new(string, object)[4]
			{
				("user", user),
				("target", synth),
				("tool", used),
				("limb", "chest")
			});
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
		else if (((EntitySystem)this).HasComp<RMCCableCoilComponent>(args.Used) && _stack.Use(args.Used.Value, 1))
		{
			if (synth.Comp.CableCoilDamageToRepair != null)
			{
				_damageable.TryChangeDamage(Entity<SynthComponent>.op_Implicit(synth), synth.Comp.CableCoilDamageToRepair, ignoreResistances: true, interruptsDoAfters: false, null, args.User);
			}
			string selfMsg2 = base.Loc.GetString("rmc-synth-repair-burn-finish-self", new(string, object)[4]
			{
				("user", user),
				("target", synth),
				("tool", used),
				("limb", "chest")
			});
			string othersMsg2 = base.Loc.GetString("rmc-synth-repair-burn-finish", new(string, object)[4]
			{
				("user", user),
				("target", synth),
				("tool", used),
				("limb", "chest")
			});
			_popup.PopupPredicted(selfMsg2, othersMsg2, user, user);
		}
	}

	private void OnSynthBlockedBeforeRangedInteract(Entity<UseOnSynthBlockedComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && args.Target.HasValue && _whitelist.CheckBoth(args.Target, ent.Comp.Blacklist, ent.Comp.Whitelist))
		{
			if (((EntitySystem)this).HasComp<SynthComponent>(args.Target) && !ent.Comp.Reversed)
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (!((EntitySystem)this).HasComp<SynthComponent>(args.Target) && ent.Comp.Reversed)
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
			if (((HandledEntityEventArgs)args).Handled)
			{
				string msg = base.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup), new(string, object)[3]
				{
					("user", args.User),
					("used", args.Used),
					("target", args.Target)
				});
				_popup.PopupClient(msg, args.User, args.User, PopupType.SmallCaution);
			}
		}
	}

	public bool HasDamage(EntityUid synth, ProtoId<DamageGroupPrototype> group)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(synth, ref damageable))
		{
			return false;
		}
		if (damageable.Damage.Empty)
		{
			return false;
		}
		if (damageable.Damage.GetDamagePerGroup(_prototypes).GetValueOrDefault(ProtoId<DamageGroupPrototype>.op_Implicit(group)) <= FixedPoint2.Zero)
		{
			return false;
		}
		return true;
	}

	public void DoSynthUnableToUsePopup(EntityUid synth, EntityUid tool)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		string msg = base.Loc.GetString("rmc-species-synth-programming-prevents-use", (ValueTuple<string, object>)("user", synth), (ValueTuple<string, object>)("tool", tool));
		_popup.PopupClient(msg, synth, synth, PopupType.SmallCaution);
	}
}
