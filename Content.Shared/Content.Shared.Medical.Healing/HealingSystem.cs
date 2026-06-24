using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Medical.Healing;

public sealed class HealingSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedBloodstreamSystem _bloodstreamSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedStackSystem _stacks;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private MobThresholdSystem _mobThresholdSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HealingComponent, UseInHandEvent>((EntityEventRefHandler<HealingComponent, UseInHandEvent>)OnHealingUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealingComponent, AfterInteractEvent>((EntityEventRefHandler<HealingComponent, AfterInteractEvent>)OnHealingAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, HealingDoAfterEvent>((EntityEventRefHandler<DamageableComponent, HealingDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnDoAfter(Entity<DamageableComponent> target, ref HealingDoAfterEvent args)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		HealingComponent healing = default(HealingComponent);
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || !((EntitySystem)this).TryComp<HealingComponent>(args.Used, ref healing))
		{
			return;
		}
		if (healing.DamageContainers != null)
		{
			ProtoId<DamageContainerPrototype>? damageContainerID = target.Comp.DamageContainerID;
			if (damageContainerID.HasValue && !healing.DamageContainers.Contains(target.Comp.DamageContainerID.Value))
			{
				return;
			}
		}
		BloodstreamComponent bloodstream = default(BloodstreamComponent);
		((EntitySystem)this).TryComp<BloodstreamComponent>(Entity<DamageableComponent>.op_Implicit(target), ref bloodstream);
		if (healing.BloodlossModifier != 0f && bloodstream != null)
		{
			bool num = bloodstream.BleedAmount > 0f;
			_bloodstreamSystem.TryModifyBleedAmount(Entity<BloodstreamComponent>.op_Implicit((target.Owner, bloodstream)), healing.BloodlossModifier);
			if (num != bloodstream.BleedAmount > 0f)
			{
				string popup = ((args.User == target.Owner) ? base.Loc.GetString("medical-item-stop-bleeding-self") : base.Loc.GetString("medical-item-stop-bleeding", (ValueTuple<string, object>)("target", Identity.Entity(target.Owner, (IEntityManager)(object)base.EntityManager))));
				_popupSystem.PopupClient(popup, Entity<DamageableComponent>.op_Implicit(target), args.User);
			}
		}
		if (healing.ModifyBloodLevel != 0f && bloodstream != null)
		{
			_bloodstreamSystem.TryModifyBloodLevel(Entity<BloodstreamComponent>.op_Implicit((target.Owner, bloodstream)), healing.ModifyBloodLevel);
		}
		DamageSpecifier healed = _damageable.TryChangeDamage(target.Owner, healing.Damage * _damageable.UniversalTopicalsHealModifier, ignoreResistances: true, interruptsDoAfters: true, null, args.Args.User);
		if (healed == null && healing.BloodlossModifier != 0f)
		{
			return;
		}
		FixedPoint2 total = healed?.GetTotal() ?? FixedPoint2.Zero;
		bool dontRepeat = false;
		StackComponent stackComp = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(args.Used.Value, ref stackComp))
		{
			_stacks.Use(args.Used.Value, 1, stackComp);
			if (_stacks.GetCount(args.Used.Value, stackComp) <= 0)
			{
				dontRepeat = true;
			}
		}
		else
		{
			((EntitySystem)this).PredictedQueueDel(args.Used.Value);
		}
		if (target.Owner != args.User)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(20, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" healed ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target.Owner)), "target", "ToPrettyString(target.Owner)");
			handler.AppendLiteral(" for ");
			handler.AppendFormatted(total, "damage", "total");
			handler.AppendLiteral(" damage");
			adminLogger.Add(LogType.Healed, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(30, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler2.AppendLiteral(" healed themselves for ");
			handler2.AppendFormatted(total, "damage", "total");
			handler2.AppendLiteral(" damage");
			adminLogger2.Add(LogType.Healed, ref handler2);
		}
		_audio.PlayPredicted(healing.HealingEndSound, target.Owner, (EntityUid?)args.User, (AudioParams?)null);
		args.Repeat = HasDamage(Entity<HealingComponent>.op_Implicit((args.Used.Value, healing)), target) && !dontRepeat;
		if (!args.Repeat && !dontRepeat)
		{
			_popupSystem.PopupClient(base.Loc.GetString("medical-item-finished-using", (ValueTuple<string, object>)("item", args.Used)), target.Owner, args.User);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private bool HasDamage(Entity<HealingComponent> healing, Entity<DamageableComponent> target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, FixedPoint2> damageableDict = target.Comp.Damage.DamageDict;
		foreach (KeyValuePair<string, FixedPoint2> item in healing.Comp.Damage.DamageDict)
		{
			if (damageableDict[item.Key].Value > 0)
			{
				return true;
			}
		}
		BloodstreamComponent bloodstream = default(BloodstreamComponent);
		if (((EntitySystem)this).TryComp<BloodstreamComponent>(Entity<DamageableComponent>.op_Implicit(target), ref bloodstream))
		{
			if (healing.Comp.ModifyBloodLevel > 0f && _solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(target.Owner), bloodstream.BloodSolutionName, ref bloodstream.BloodSolution, out Solution bloodSolution) && bloodSolution.Volume < bloodSolution.MaxVolume)
			{
				return true;
			}
			if (healing.Comp.BloodlossModifier < 0f && bloodstream.BleedAmount > 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void OnHealingUse(Entity<HealingComponent> healing, ref UseInHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && TryHeal(healing, Entity<DamageableComponent>.op_Implicit(args.User), args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnHealingAfterInteract(Entity<HealingComponent> healing, ref AfterInteractEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && args.Target.HasValue && TryHeal(healing, Entity<DamageableComponent>.op_Implicit(args.Target.Value), args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool TryHeal(Entity<HealingComponent> healing, Entity<DamageableComponent?> target, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DamageableComponent>(Entity<DamageableComponent>.op_Implicit(target), ref target.Comp, false))
		{
			return false;
		}
		if (healing.Comp.DamageContainers != null)
		{
			ProtoId<DamageContainerPrototype>? damageContainerID = target.Comp.DamageContainerID;
			if (damageContainerID.HasValue && !healing.Comp.DamageContainers.Contains(target.Comp.DamageContainerID.Value))
			{
				return false;
			}
		}
		if (user != target.Owner && !_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target.Owner), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return false;
		}
		StackComponent stack = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(Entity<HealingComponent>.op_Implicit(healing), ref stack) && stack.Count < 1)
		{
			return false;
		}
		if (!HasDamage(healing, target))
		{
			_popupSystem.PopupClient(base.Loc.GetString("medical-item-cant-use", (ValueTuple<string, object>)("item", healing.Owner)), Entity<HealingComponent>.op_Implicit(healing), user);
			return false;
		}
		_audio.PlayPredicted(healing.Comp.HealingBeginSound, Entity<HealingComponent>.op_Implicit(healing), (EntityUid?)user, (AudioParams?)null);
		bool num = user != target.Owner;
		if (num)
		{
			string msg = base.Loc.GetString("medical-item-popup-target", (ValueTuple<string, object>)("user", Identity.Entity(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", healing.Owner));
			_popupSystem.PopupEntity(msg, Entity<DamageableComponent>.op_Implicit(target), Entity<DamageableComponent>.op_Implicit(target), PopupType.Medium);
		}
		float delay = (num ? healing.Comp.Delay : (healing.Comp.Delay * GetScaledHealingPenalty(healing)));
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, new HealingDoAfterEvent(), Entity<DamageableComponent>.op_Implicit(target), Entity<DamageableComponent>.op_Implicit(target), Entity<HealingComponent>.op_Implicit(healing))
		{
			NeedHand = true,
			BreakOnMove = true,
			BreakOnWeightlessMove = false
		};
		_doAfter.TryStartDoAfter(doAfterEventArgs);
		return true;
	}

	public float GetScaledHealingPenalty(Entity<HealingComponent> healing)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float output = healing.Comp.Delay;
		MobThresholdsComponent mobThreshold = default(MobThresholdsComponent);
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<HealingComponent>.op_Implicit(healing), ref mobThreshold) || !((EntitySystem)this).TryComp<DamageableComponent>(Entity<HealingComponent>.op_Implicit(healing), ref damageable))
		{
			return output;
		}
		if (!_mobThresholdSystem.TryGetThresholdForState(Entity<HealingComponent>.op_Implicit(healing), MobState.Critical, out var amount, mobThreshold))
		{
			return 1f;
		}
		FixedPoint2 totalDamage = damageable.TotalDamage;
		FixedPoint2? fixedPoint = amount;
		return Math.Max((float)(totalDamage / fixedPoint).Value * (healing.Comp.SelfHealPenaltyMultiplier - 1f) + 1f, 1f);
	}
}
