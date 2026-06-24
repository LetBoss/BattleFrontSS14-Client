using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Tools;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Repairable;

public sealed class RMCRepairableSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedStackSystem _stack;

	private const string SOLUTION_WELDER = "Welder";

	private const string REAGENT_WELDER = "WeldingFuel";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCRepairableComponent, InteractUsingEvent>((EntityEventRefHandler<RMCRepairableComponent, InteractUsingEvent>)OnRepairableInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRepairableComponent, RMCRepairableDoAfterEvent>((EntityEventRefHandler<RMCRepairableComponent, RMCRepairableDoAfterEvent>)OnRepairableDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NailgunRepairableComponent, InteractUsingEvent>((EntityEventRefHandler<NailgunRepairableComponent, InteractUsingEvent>)OnNailgunRepairableInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NailgunRepairableComponent, RMCNailgunRepairableDoAfterEvent>((EntityEventRefHandler<NailgunRepairableComponent, RMCNailgunRepairableDoAfterEvent>)OnNailgunRepairableDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReagentTankComponent, InteractUsingEvent>((EntityEventRefHandler<ReagentTankComponent, InteractUsingEvent>)OnWelderInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnRepairableInteractUsing(Entity<RMCRepairableComponent> repairable, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid used = args.Used;
		if (!_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(repairable.Comp.Quality)))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		if (!((EntitySystem)this).HasComp<BlowtorchComponent>(used))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-repairable-need-blowtorch"), user, user, PopupType.SmallCaution);
		}
		else
		{
			DamageableComponent damageable = default(DamageableComponent);
			if (!((EntitySystem)this).TryComp<DamageableComponent>(Entity<RMCRepairableComponent>.op_Implicit(repairable), ref damageable))
			{
				return;
			}
			if (damageable.TotalDamage <= FixedPoint2.Zero)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-repairable-not-damaged", (ValueTuple<string, object>)("target", repairable)), user, user, PopupType.SmallCaution);
			}
			else if (repairable.Comp.RepairableDamageLimit > 0f && damageable.TotalDamage > repairable.Comp.RepairableDamageLimit)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-repairable-too-damaged", (ValueTuple<string, object>)("target", repairable)), user, user, PopupType.SmallCaution);
			}
			else if (repairable.Comp.SkillRequired > 0 && !_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), repairable.Comp.Skill, repairable.Comp.SkillRequired))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-repairable-not-trained", (ValueTuple<string, object>)("target", repairable)), user, user, PopupType.SmallCaution);
			}
			else if (CanRepairPopup(user, Entity<RMCRepairableComponent>.op_Implicit(repairable)) && UseFuel(args.Used, args.User, repairable.Comp.FuelUsed, attempt: true))
			{
				RMCRepairableDoAfterEvent ev = new RMCRepairableDoAfterEvent();
				EntityManager entityManager = base.EntityManager;
				TimeSpan delay = repairable.Comp.Delay;
				EntityUid? eventTarget = Entity<RMCRepairableComponent>.op_Implicit(repairable);
				EntityUid? used2 = args.Used;
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, delay, ev, eventTarget, null, used2)
				{
					NeedHand = true,
					BreakOnMove = true,
					BlockDuplicate = true,
					DuplicateCondition = DuplicateConditions.SameEvent
				};
				RMCToolUseEvent toolEvent = new RMCToolUseEvent(user, doAfter.Delay);
				((EntitySystem)this).RaiseLocalEvent<RMCToolUseEvent>(args.Used, ref toolEvent, false);
				if (toolEvent.Handled)
				{
					doAfter.Delay = toolEvent.Delay;
				}
				if (_doAfter.TryStartDoAfter(doAfter))
				{
					string selfMsg = base.Loc.GetString("rmc-repairable-start-self", (ValueTuple<string, object>)("target", repairable));
					string othersMsg = base.Loc.GetString("rmc-repairable-start-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("target", repairable));
					_popup.PopupPredicted(selfMsg, othersMsg, user, user);
				}
			}
		}
	}

	private void OnRepairableDoAfter(Entity<RMCRepairableComponent> repairable, ref RMCRepairableDoAfterEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (CanRepairPopup(args.User, Entity<RMCRepairableComponent>.op_Implicit(repairable)) && args.Used.HasValue && UseFuel(args.Used.Value, args.User, repairable.Comp.FuelUsed))
		{
			DamageSpecifier heal = -_rmcDamageable.DistributeTypesTotal(Entity<DamageableComponent>.op_Implicit(repairable.Owner), repairable.Comp.Heal);
			_damageable.TryChangeDamage(Entity<RMCRepairableComponent>.op_Implicit(repairable), heal, ignoreResistances: true);
			EntityUid user = args.User;
			string selfMsg = base.Loc.GetString("rmc-repairable-finish-self", (ValueTuple<string, object>)("target", repairable));
			string othersMsg = base.Loc.GetString("rmc-repairable-finish-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("target", repairable));
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
			_audio.PlayPredicted(repairable.Comp.Sound, Entity<RMCRepairableComponent>.op_Implicit(repairable), (EntityUid?)user, (AudioParams?)null);
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(Entity<RMCRepairableComponent>.op_Implicit(repairable), ref damageable) && damageable.TotalDamage > FixedPoint2.Zero && heal.GetTotal() != FixedPoint2.Zero)
			{
				args.Repeat = true;
			}
		}
	}

	public bool UseFuel(EntityUid tool, EntityUid user, FixedPoint2 fuelUsed, bool attempt = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		SolutionContainerManagerComponent welderCon = default(SolutionContainerManagerComponent);
		if (!((EntitySystem)this).TryComp<SolutionContainerManagerComponent>(tool, ref welderCon))
		{
			return false;
		}
		ItemToggleComponent toggle = default(ItemToggleComponent);
		if (!((EntitySystem)this).TryComp<ItemToggleComponent>(tool, ref toggle) || !toggle.Activated)
		{
			_popup.PopupClient(base.Loc.GetString("welder-component-welder-not-lit-message"), user, PopupType.SmallCaution);
			return false;
		}
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((tool, welderCon)), "Welder", out Entity<SolutionComponent>? solutionComp, out Solution solution))
		{
			return false;
		}
		if (solution.GetTotalPrototypeQuantity("WeldingFuel") == 0 || solution.GetTotalPrototypeQuantity("WeldingFuel") < fuelUsed)
		{
			_popup.PopupClient(base.Loc.GetString("welder-component-no-fuel-message"), user, PopupType.SmallCaution);
			return false;
		}
		if (!attempt && _net.IsServer)
		{
			_solution.RemoveReagent(solutionComp.Value, "WeldingFuel", fuelUsed);
			((EntitySystem)this).Dirty<SolutionComponent>(solutionComp.Value, (MetaDataComponent)null);
		}
		return true;
	}

	private void OnNailgunRepairableInteractUsing(Entity<NailgunRepairableComponent> repairable, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid used = args.Used;
		EntityUid user = args.User;
		NailgunComponent nailgunComp = default(NailgunComponent);
		HandsComponent handsComp = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<NailgunComponent>(used, ref nailgunComp) || !((EntitySystem)this).TryComp<HandsComponent>(user, ref handsComp))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(Entity<NailgunRepairableComponent>.op_Implicit(repairable), ref damageable) || damageable.TotalDamage <= FixedPoint2.Zero)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-repairable-not-damaged", (ValueTuple<string, object>)("target", repairable)), user, user, PopupType.SmallCaution);
			return;
		}
		GetAmmoCountEvent getAmmoCountEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(used, ref getAmmoCountEv, false);
		if (getAmmoCountEv.Count < 4)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-nailgun-no-nails-message"), user, PopupType.SmallCaution);
			return;
		}
		EntityUid? held;
		float repairValue = GetRepairValue(repairable, Entity<HandsComponent>.op_Implicit((user, handsComp)), nailgunComp, out held);
		if (!held.HasValue || repairValue <= FixedPoint2.Zero)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-nailgun-no-material-message", (ValueTuple<string, object>)("target", repairable)), user, PopupType.SmallCaution);
			return;
		}
		float delay = nailgunComp.NailingSpeed;
		RMCNailgunRepairableDoAfterEvent ev = new RMCNailgunRepairableDoAfterEvent();
		EntityManager entityManager = base.EntityManager;
		EntityUid? eventTarget = Entity<NailgunRepairableComponent>.op_Implicit(repairable);
		EntityUid? used2 = args.Used;
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, delay, ev, eventTarget, null, used2)
		{
			NeedHand = true,
			BreakOnMove = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string selfMsg = base.Loc.GetString("rmc-repairable-start-self", (ValueTuple<string, object>)("target", repairable));
			string othersMsg = base.Loc.GetString("rmc-repairable-start-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("target", repairable));
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
	}

	private void OnNailgunRepairableDoAfter(Entity<NailgunRepairableComponent> repairable, ref RMCNailgunRepairableDoAfterEvent args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? used = args.Used;
		if (!used.HasValue)
		{
			return;
		}
		EntityUid used2 = used.GetValueOrDefault();
		EntityUid user = args.User;
		NailgunComponent nailgunComponent = default(NailgunComponent);
		if (!((EntitySystem)this).TryComp<NailgunComponent>(used2, ref nailgunComponent))
		{
			return;
		}
		GetAmmoCountEvent getAmmoCountEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(used2, ref getAmmoCountEv, false);
		if (getAmmoCountEv.Count < 4)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-nailgun-no-nails-message"), user, PopupType.SmallCaution);
		}
		else
		{
			HandsComponent handsComp = default(HandsComponent);
			if (!((EntitySystem)this).TryComp<HandsComponent>(user, ref handsComp))
			{
				return;
			}
			EntityUid? held;
			float repairValue = GetRepairValue(repairable, Entity<HandsComponent>.op_Implicit((user, handsComp)), nailgunComponent, out held);
			if (!held.HasValue || repairValue <= FixedPoint2.Zero)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-nailgun-lost-stack"), user, PopupType.SmallCaution);
				return;
			}
			DamageSpecifier heal = -_rmcDamageable.DistributeTypesTotal(Entity<DamageableComponent>.op_Implicit(repairable.Owner), repairValue);
			_damageable.TryChangeDamage(Entity<NailgunRepairableComponent>.op_Implicit(repairable), heal, ignoreResistances: true);
			StackComponent stack = default(StackComponent);
			if (((EntitySystem)this).TryComp<StackComponent>(held, ref stack))
			{
				_stack.SetCount(held.Value, stack.Count - nailgunComponent.MaterialPerRepair);
			}
			List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
			TakeAmmoEvent ev = new TakeAmmoEvent(4, ammo, ((EntitySystem)this).Transform(user).Coordinates, user);
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(used2, ev, false);
			foreach (var item in ev.Ammo)
			{
				EntityUid? bullet = item.Entity;
				((EntitySystem)this).QueueDel(bullet);
			}
			string selfMsg = base.Loc.GetString("rmc-nailgun-finish-self", (ValueTuple<string, object>)("material", held), (ValueTuple<string, object>)("target", repairable));
			string othersMsg = base.Loc.GetString("rmc-repairable-finish-others", new(string, object)[3]
			{
				("user", user),
				("material", held),
				("target", repairable)
			});
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
			_audio.PlayPredicted(nailgunComponent.RepairSound, Entity<NailgunRepairableComponent>.op_Implicit(repairable), (EntityUid?)user, (AudioParams?)null);
		}
	}

	private float GetRepairValue(Entity<NailgunRepairableComponent> repairable, Entity<HandsComponent?> hands, NailgunComponent nailgunComponent, out EntityUid? heldStack)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		float repairValue = 0f;
		heldStack = null;
		StackComponent stackComponent = default(StackComponent);
		foreach (EntityUid held in _hands.EnumerateHeld(hands))
		{
			if (!((EntitySystem)this).TryComp<StackComponent>(held, ref stackComponent))
			{
				continue;
			}
			string stackType = stackComponent.StackTypeId;
			heldStack = held;
			if (stackComponent.Count >= nailgunComponent.MaterialPerRepair)
			{
				switch (stackType)
				{
				case "CMSteel":
					repairValue = repairable.Comp.RepairMetal;
					goto end_IL_00ae;
				case "CMPlasteel":
					repairValue = repairable.Comp.RepairPlasteel;
					goto end_IL_00ae;
				case "RMCPlankWood":
					repairValue = repairable.Comp.RepairWood;
					goto end_IL_00ae;
				}
			}
			continue;
			end_IL_00ae:
			break;
		}
		return repairValue;
	}

	private void OnWelderInteractUsing(Entity<ReagentTankComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid used = args.Used;
		EntityUid target = args.Target;
		WelderComponent welder = default(WelderComponent);
		if (((EntitySystem)this).TryComp<WelderComponent>(used, ref welder) && ent.Comp.TankType == ReagentTankType.Fuel && _solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target), out Entity<SolutionComponent>? targetSoln, out Solution targetSolution) && _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(used), welder.FuelSolutionName, out Entity<SolutionComponent>? solutionComp, out Solution welderSolution))
		{
			FixedPoint2 trans = FixedPoint2.Min(welderSolution.AvailableVolume, targetSolution.Volume);
			if (welder.Enabled)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-welder-component-danger"), used, args.User, PopupType.MediumCaution);
			}
			else if (trans > 0)
			{
				Solution drained = _solution.Drain(Entity<DrainableSolutionComponent>.op_Implicit(target), targetSoln.Value, trans);
				_solution.TryAddSolution(solutionComp.Value, drained);
				_audio.PlayPredicted(welder.WelderRefill, used, (EntityUid?)args.User, (AudioParams?)null);
				_popup.PopupClient(base.Loc.GetString("welder-component-after-interact-refueled-message"), used, args.User);
			}
			else if (welderSolution.AvailableVolume <= 0)
			{
				_popup.PopupClient(base.Loc.GetString("welder-component-already-full"), used, args.User);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("welder-component-no-fuel-in-tank", (ValueTuple<string, object>)("owner", args.Target)), used, args.User);
			}
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool CanRepairPopup(EntityUid user, EntityUid target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		RMCRepairableTargetAttemptEvent ev = new RMCRepairableTargetAttemptEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<RMCRepairableTargetAttemptEvent>(target, ref ev, false);
		if (!ev.Cancelled)
		{
			return true;
		}
		_popup.PopupClient(ev.Popup, user, user, PopupType.MediumCaution);
		return false;
	}
}
