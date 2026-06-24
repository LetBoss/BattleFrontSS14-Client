using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Refill;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Forensics;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Chemistry;

public abstract class RMCSharedHypospraySystem : EntitySystem
{
	[Dependency]
	protected ISharedAdminLogManager _adminLog;

	[Dependency]
	protected SharedAppearanceSystem _appearance;

	[Dependency]
	protected SharedAudioSystem _audio;

	[Dependency]
	protected SharedContainerSystem _container;

	[Dependency]
	protected SharedDoAfterSystem _doAfter;

	[Dependency]
	protected SharedInteractionSystem _interaction;

	[Dependency]
	protected HypospraySystem _hypospray;

	[Dependency]
	protected IPrototypeManager _prototype;

	[Dependency]
	protected ReactiveSystem _reactive;

	[Dependency]
	protected SharedSolutionContainerSystem _solution;

	[Dependency]
	protected INetManager _net;

	[Dependency]
	protected SharedPopupSystem _popup;

	[Dependency]
	protected SkillsSystem _skills;

	[Dependency]
	protected ItemSlotsSystem _slots;

	[Dependency]
	protected EntityWhitelistSystem _whitelist;

	[Dependency]
	protected UseDelaySystem _useDelay;

	[Dependency]
	protected SolutionTransferSystem _transfer;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCHyposprayComponent, GetVerbsEvent<AlternativeVerb>>)AddSetTransferVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, ComponentStartup>((EntityEventRefHandler<RMCHyposprayComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCHyposprayComponent, EntInsertedIntoContainerMessage>)OnInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<RMCHyposprayComponent, EntRemovedFromContainerMessage>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, AfterInteractEvent>((EntityEventRefHandler<RMCHyposprayComponent, AfterInteractEvent>)OnInteractAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, UseInHandEvent>((EntityEventRefHandler<RMCHyposprayComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, ExaminedEvent>((EntityEventRefHandler<RMCHyposprayComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, InteractUsingEvent>((EntityEventRefHandler<RMCHyposprayComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, TacticalReloadHyposprayDoAfterEvent>((EntityEventRefHandler<RMCHyposprayComponent, TacticalReloadHyposprayDoAfterEvent>)OnTacticalReload, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, HyposprayDoAfterEvent>((EntityEventRefHandler<RMCHyposprayComponent, HyposprayDoAfterEvent>)OnHypoInject, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHyposprayComponent, RefilledSolutionEvent>((EntityEventRefHandler<RMCHyposprayComponent, RefilledSolutionEvent>)OnRefilled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HyposprayComponent, HyposprayDoAfterEvent>((EntityEventRefHandler<HyposprayComponent, HyposprayDoAfterEvent>)OnHyposprayDoAfter, (Type[])null, (Type[])null);
	}

	private void OnExamine(Entity<RMCHyposprayComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(ent), ent.Comp.SlotId, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count != 0)
		{
			EntityUid vial = container.ContainedEntities[0];
			args.PushText(base.Loc.GetString("rmc-hypospray-loaded", (ValueTuple<string, object>)("vial", vial)));
		}
	}

	private void AddSetTransferVerbs(Entity<RMCHyposprayComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null)
		{
			return;
		}
		EntityUid user = args.User;
		Entity<RMCHyposprayComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		RMCHyposprayComponent rMCHyposprayComponent = default(RMCHyposprayComponent);
		val.Deconstruct(ref val2, ref rMCHyposprayComponent);
		RMCHyposprayComponent component = rMCHyposprayComponent;
		int priority = 0;
		FixedPoint2[] transferAmounts = entity.Comp.TransferAmounts;
		foreach (FixedPoint2 amount in transferAmounts)
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Text = base.Loc.GetString("comp-solution-transfer-verb-amount", (ValueTuple<string, object>)("amount", amount)),
				Category = VerbCategory.SetTransferAmount,
				Act = delegate
				{
					//IL_005b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					//IL_0087: Unknown result type (might be due to invalid IL or missing references)
					component.TransferAmount = amount;
					_popup.PopupClient(base.Loc.GetString("comp-solution-transfer-set-amount", (ValueTuple<string, object>)("amount", amount)), user, user);
					((EntitySystem)this).Dirty<RMCHyposprayComponent>(entity, (MetaDataComponent)null);
				},
				Priority = priority
			};
			priority--;
			args.Verbs.Add(verb);
		}
	}

	private void OnStartup(Entity<RMCHyposprayComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnInsert(Entity<RMCHyposprayComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)ent.Comp).Initialized && !(((ContainerModifiedMessage)args).Container.ID != ent.Comp.SlotId))
		{
			UpdateAppearance(ent);
		}
	}

	private void OnRemove(Entity<RMCHyposprayComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.SlotId))
		{
			UpdateAppearance(ent);
		}
	}

	private void OnUseInHand(Entity<RMCHyposprayComponent> ent, ref UseInHandEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			int index = Array.IndexOf(ent.Comp.TransferAmounts, ent.Comp.TransferAmount) + 1;
			if (index >= ent.Comp.TransferAmounts.Length)
			{
				index = 0;
			}
			ent.Comp.TransferAmount = ent.Comp.TransferAmounts[index];
			_popup.PopupClient(base.Loc.GetString("rmc-hypospray-amount-change", (ValueTuple<string, object>)("amount", ent.Comp.TransferAmount)), args.User, args.User);
			((EntitySystem)this).Dirty<RMCHyposprayComponent>(ent, (MetaDataComponent)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnInteractAfter(Entity<RMCHyposprayComponent> ent, ref AfterInteractEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		ItemSlotsComponent slots = default(ItemSlotsComponent);
		if (((HandledEntityEventArgs)args).Handled || !args.Target.HasValue || !args.CanReach || !_container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(ent), ent.Comp.SlotId, ref container, (ContainerManagerComponent)null) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref slots))
		{
			return;
		}
		if (_slots.CanInsert(Entity<RMCHyposprayComponent>.op_Implicit(ent), args.Target.Value, args.User, slots.Slots[ent.Comp.SlotId], swap: true))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (!_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.TacticalSkills))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-hypospray-fail-tacreload"), args.Used, args.User);
				return;
			}
			if (container.ContainedEntities.Count == 0)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-hypospray-load-tacreload", (ValueTuple<string, object>)("hypo", ent)), args.Used, args.User);
			}
			else
			{
				if (!_slots.TryEjectToHands(Entity<RMCHyposprayComponent>.op_Implicit(ent), slots.Slots[ent.Comp.SlotId], args.User, excludeUserAudio: true))
				{
					return;
				}
				_popup.PopupClient(base.Loc.GetString("rmc-hypospray-swap-tacreload"), args.Used, args.User);
			}
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.TacticalReloadTime, new TacticalReloadHyposprayDoAfterEvent(), Entity<RMCHyposprayComponent>.op_Implicit(ent), args.Target, Entity<RMCHyposprayComponent>.op_Implicit(ent))
			{
				BreakOnMove = true,
				BreakOnWeightlessMove = false,
				BreakOnDamage = true,
				NeedHand = ent.Comp.NeedHand,
				BreakOnHandChange = ent.Comp.BreakOnHandChange,
				MovementThreshold = ent.Comp.MovementThreshold
			});
		}
		else if (container.ContainedEntities.Count == 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-hypospray-no-vial"), Entity<RMCHyposprayComponent>.op_Implicit(ent), args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
		else if (((EntitySystem)this).HasComp<InjectableSolutionComponent>(args.Target.Value) && (!ent.Comp.OnlyAffectsMobs || ((EntitySystem)this).HasComp<MobStateComponent>(args.Target.Value)))
		{
			((HandledEntityEventArgs)args).Handled = true;
			UseDelayComponent delayComp = default(UseDelayComponent);
			if (!((EntitySystem)this).TryComp<UseDelayComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref delayComp) || !_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<RMCHyposprayComponent>.op_Implicit(ent), delayComp))))
			{
				AttemptHyposprayUseEvent attemptEv = new AttemptHyposprayUseEvent(args.User, args.Target.Value, TimeSpan.Zero);
				((EntitySystem)this).RaiseLocalEvent<AttemptHyposprayUseEvent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref attemptEv, false);
				HyposprayDoAfterEvent doAfter = new HyposprayDoAfterEvent();
				DoAfterArgs argsu = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, attemptEv.DoAfter, doAfter, Entity<RMCHyposprayComponent>.op_Implicit(ent), args.Target, Entity<RMCHyposprayComponent>.op_Implicit(ent))
				{
					BreakOnMove = true,
					BreakOnHandChange = true,
					NeedHand = true,
					LagCompensated = true
				};
				_doAfter.TryStartDoAfter(argsu);
			}
		}
	}

	protected virtual void OnInteractUsing(Entity<RMCHyposprayComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		ItemSlotsComponent slots = default(ItemSlotsComponent);
		if (((HandledEntityEventArgs)args).Handled || !_container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(ent), ent.Comp.SlotId, ref container, (ContainerManagerComponent)null) || !((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref slots) || _slots.CanInsert(Entity<RMCHyposprayComponent>.op_Implicit(ent), args.Used, args.User, slots.Slots[ent.Comp.SlotId], swap: true))
		{
			return;
		}
		if (container.ContainedEntities.Count == 0)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-hypospray-no-vial"), Entity<RMCHyposprayComponent>.op_Implicit(ent), args.User);
			return;
		}
		EntityUid vial = container.ContainedEntities[0];
		SolutionTransferComponent solt = default(SolutionTransferComponent);
		if (_solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(vial), out Entity<SolutionComponent>? solm, out Solution _) && _solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(args.Used), out Entity<SolutionComponent>? soln, out Solution _) && ((EntitySystem)this).TryComp<SolutionTransferComponent>(args.Used, ref solt))
		{
			((HandledEntityEventArgs)args).Handled = true;
			FixedPoint2 transferr = _transfer.Transfer(args.User, args.Used, soln.Value, vial, solm.Value, solt.TransferAmount);
			if (transferr > 0)
			{
				string message = base.Loc.GetString("comp-solution-transfer-transfer-solution", (ValueTuple<string, object>)("amount", transferr), (ValueTuple<string, object>)("target", vial));
				_popup.PopupClient(message, Entity<RMCHyposprayComponent>.op_Implicit(ent), args.User);
			}
			((EntitySystem)this).Dirty<SolutionComponent>(soln.Value, (MetaDataComponent)null);
			((EntitySystem)this).Dirty<SolutionComponent>(solm.Value, (MetaDataComponent)null);
			UpdateAppearance(ent);
		}
	}

	private void OnHyposprayDoAfter(Entity<HyposprayComponent> ent, ref HyposprayDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				((HandledEntityEventArgs)args).Handled = true;
				_hypospray.TryDoInject(ent, target2, args.User, doAfter: false);
			}
		}
	}

	private void OnRefilled(Entity<RMCHyposprayComponent> ent, ref RefilledSolutionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnHypoInject(Entity<RMCHyposprayComponent> ent, ref HyposprayDoAfterEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		((HandledEntityEventArgs)args).Handled = true;
		string msgFormat = null;
		if (target2 == args.User)
		{
			msgFormat = "hypospray-component-inject-self-message";
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(ent), ent.Comp.SlotId, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-hypospray-no-vial"), Entity<RMCHyposprayComponent>.op_Implicit(ent), args.User);
			return;
		}
		EntityUid vial = container.ContainedEntities[0];
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(vial), ent.Comp.VialName, out Entity<SolutionComponent>? soln, out Solution solu) || solu.Volume == 0)
		{
			_popup.PopupClient(base.Loc.GetString("hypospray-component-empty-message"), target2, args.User);
			return;
		}
		if (!_solution.TryGetInjectableSolution(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target2), out Entity<SolutionComponent>? targetSoln, out Solution targetSolution))
		{
			_popup.PopupClient(base.Loc.GetString("hypospray-cant-inject", (ValueTuple<string, object>)("target", Identity.Entity(target2, (IEntityManager)(object)base.EntityManager))), target2, args.User);
			return;
		}
		_popup.PopupClient(base.Loc.GetString(msgFormat ?? "hypospray-component-inject-other-message", (ValueTuple<string, object>)("other", target2)), target2, args.User);
		if (target2 != args.User)
		{
			_popup.PopupEntity(base.Loc.GetString("hypospray-component-feel-prick-message"), target2, target2);
		}
		_audio.PlayPredicted(ent.Comp.InjectSound, Entity<RMCHyposprayComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		UseDelayComponent delayComp = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref delayComp))
		{
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<RMCHyposprayComponent>.op_Implicit(ent), delayComp)));
		}
		FixedPoint2 transferAmount = FixedPoint2.Min(ent.Comp.TransferAmount, targetSolution.AvailableVolume);
		if (transferAmount <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("hypospray-component-transfer-already-full-message", (ValueTuple<string, object>)("owner", target2)), target2, args.User);
			return;
		}
		Solution removedSolution = _solution.SplitSolution(soln.Value, transferAmount);
		if (targetSolution.CanAddSolution(removedSolution))
		{
			_reactive.DoEntityReaction(target2, removedSolution, ReactionMethod.Injection);
			_solution.TryAddSolution(targetSoln.Value, removedSolution);
			TransferDnaEvent transferDnaEvent = new TransferDnaEvent();
			transferDnaEvent.Donor = target2;
			transferDnaEvent.Recipient = Entity<RMCHyposprayComponent>.op_Implicit(ent);
			TransferDnaEvent ev = transferDnaEvent;
			((EntitySystem)this).RaiseLocalEvent<TransferDnaEvent>(target2, ref ev, false);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(36, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" injected ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target2)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(" with a solution ");
			handler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(removedSolution), 0, "removedSolution");
			handler.AppendLiteral(" using a ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<RMCHyposprayComponent>.op_Implicit(ent), (MetaDataComponent)null), "using", "ToPrettyString(ent)");
			adminLog.Add(LogType.ForceFeed, ref handler);
			UpdateAppearance(ent);
		}
	}

	private void OnTacticalReload(Entity<RMCHyposprayComponent> ent, ref TacticalReloadHyposprayDoAfterEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		ItemSlotsComponent slots = default(ItemSlotsComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && _container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(ent), ent.Comp.SlotId, ref container, (ContainerManagerComponent)null) && ((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref slots) && args.Target.HasValue)
		{
			_slots.TryInsertEmpty(Entity<ItemSlotsComponent>.op_Implicit((Entity<RMCHyposprayComponent>.op_Implicit(ent), slots)), args.Target.Value, null);
		}
	}

	protected void UpdateAppearance(Entity<RMCHyposprayComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		BaseContainer container = default(BaseContainer);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent), ref appearance) || !_container.TryGetContainer(Entity<RMCHyposprayComponent>.op_Implicit(ent), ent.Comp.SlotId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		int containerEnts = container.ContainedEntities.Count;
		_appearance.SetData(Entity<RMCHyposprayComponent>.op_Implicit(ent), (Enum)VialVisuals.Occupied, (object)(containerEnts != 0), appearance);
		if (!((EntitySystem)this).HasComp<SolutionContainerVisualsComponent>(Entity<RMCHyposprayComponent>.op_Implicit(ent)))
		{
			return;
		}
		Solution solution;
		if (containerEnts == 0)
		{
			solution = new Solution();
		}
		else
		{
			EntityUid vial = container.ContainedEntities[0];
			if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(vial), ent.Comp.VialName, out Entity<SolutionComponent>? soln))
			{
				return;
			}
			solution = soln.Value.Comp.Solution;
		}
		_appearance.SetData(Entity<RMCHyposprayComponent>.op_Implicit(ent), (Enum)SolutionContainerVisuals.FillFraction, (object)solution.FillFraction, appearance);
		_appearance.SetData(Entity<RMCHyposprayComponent>.op_Implicit(ent), (Enum)SolutionContainerVisuals.Color, (object)solution.GetColor(_prototype), appearance);
		_appearance.SetData(Entity<RMCHyposprayComponent>.op_Implicit(ent), (Enum)SolutionContainerVisuals.SolutionName, (object)ent.Comp.SolutionName, appearance);
		ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
		if (primaryReagentId.HasValue)
		{
			ReagentId reagent = primaryReagentId.GetValueOrDefault();
			_appearance.SetData(Entity<RMCHyposprayComponent>.op_Implicit(ent), (Enum)SolutionContainerVisuals.BaseOverride, (object)reagent.ToString(), appearance);
		}
		((EntitySystem)this).Dirty(Entity<RMCHyposprayComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	public bool DoAfter(Entity<HyposprayComponent> entity, EntityUid target, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!_hypospray.EligibleEntity(target, Entity<HyposprayComponent>.op_Implicit(entity)))
		{
			return false;
		}
		UseDelayComponent delayComp = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<HyposprayComponent>.op_Implicit(entity), ref delayComp) && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<HyposprayComponent>.op_Implicit(entity), delayComp))))
		{
			return false;
		}
		AttemptHyposprayUseEvent attemptEv = new AttemptHyposprayUseEvent(user, target, TimeSpan.Zero);
		((EntitySystem)this).RaiseLocalEvent<AttemptHyposprayUseEvent>(Entity<HyposprayComponent>.op_Implicit(entity), ref attemptEv, false);
		HyposprayDoAfterEvent doAfter = new HyposprayDoAfterEvent();
		DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, attemptEv.DoAfter, doAfter, Entity<HyposprayComponent>.op_Implicit(entity), target, Entity<HyposprayComponent>.op_Implicit(entity))
		{
			BreakOnMove = true,
			BreakOnHandChange = true,
			NeedHand = true
		};
		_doAfter.TryStartDoAfter(args);
		return true;
	}
}
