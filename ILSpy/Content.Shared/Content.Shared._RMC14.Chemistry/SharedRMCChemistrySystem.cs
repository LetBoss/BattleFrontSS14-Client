using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Chemistry;

public abstract class SharedRMCChemistrySystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private RMCReagentSystem _reagents;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private IGameTiming _timing;

	private readonly List<Entity<RMCChemicalDispenserComponent>> _dispensers = new List<Entity<RMCChemicalDispenserComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SolutionComponent, ComponentGetState>((EntityEventRefHandler<SolutionComponent, ComponentGetState>)OnSolutionGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionComponent, ComponentHandleState>((EntityEventRefHandler<SolutionComponent, ComponentHandleState>)OnSolutionHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DetailedExaminableSolutionComponent, ExaminedEvent>((EntityEventRefHandler<DetailedExaminableSolutionComponent, ExaminedEvent>)OnDetailedSolutionExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCChemicalDispenserComponent, MapInitEvent>((EntityEventRefHandler<RMCChemicalDispenserComponent, MapInitEvent>)OnDispenserMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCToggleableSolutionTransferComponent, MapInitEvent>((EntityEventRefHandler<RMCToggleableSolutionTransferComponent, MapInitEvent>)OnToggleableSolutionTransferMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCToggleableSolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCToggleableSolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>)OnToggleableSolutionTransferVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSolutionTransferWhitelistComponent, SolutionTransferAttemptEvent>((EntityEventRefHandler<RMCSolutionTransferWhitelistComponent, SolutionTransferAttemptEvent>)OnTransferWhitelistAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoMixingReagentsComponent, ExaminedEvent>((EntityEventRefHandler<NoMixingReagentsComponent, ExaminedEvent>)OnNoMixingReagentsExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoMixingReagentsComponent, SolutionTransferAttemptEvent>((EntityEventRefHandler<NoMixingReagentsComponent, SolutionTransferAttemptEvent>)OnNoMixingReagentsTransferAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCEmptySolutionComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCEmptySolutionComponent, GetVerbsEvent<AlternativeVerb>>)OnEmptySolutionGetVerbs, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCChemicalDispenserComponent>(((EntitySystem)this).Subs, (object)RMCChemicalDispenserUi.Key, (BuiEventSubscriber<RMCChemicalDispenserComponent>)delegate(Subscriber<RMCChemicalDispenserComponent> subs)
		{
			subs.Event<RMCChemicalDispenserDispenseSettingBuiMsg>((EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserDispenseSettingBuiMsg>)OnChemicalDispenserSettingMsg);
			subs.Event<RMCChemicalDispenserBeakerBuiMsg>((EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserBeakerBuiMsg>)OnChemicalDispenserBeakerSettingMsg);
			subs.Event<RMCChemicalDispenserEjectBeakerBuiMsg>((EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserEjectBeakerBuiMsg>)OnChemicalDispenserEjectBeakerMsg);
			subs.Event<RMCChemicalDispenserDispenseBuiMsg>((EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserDispenseBuiMsg>)OnChemicalDispenserDispenseMsg);
		});
	}

	private void OnSolutionGetState(Entity<SolutionComponent> ent, ref ComponentGetState args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Solution s = new Solution(ent.Comp.Solution, _prototypes);
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new SolutionComponentState(s);
	}

	private void OnSolutionHandleState(Entity<SolutionComponent> ent, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is SolutionComponentState s)
		{
			ent.Comp.Solution = new Solution(s.Solution, _prototypes);
		}
	}

	private void OnDetailedSolutionExamined(Entity<DetailedExaminableSolutionComponent> ent, ref ExaminedEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("DetailedExaminableSolutionComponent"))
		{
			args.PushText("It contains:");
			if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution) || solution.Volume <= FixedPoint2.Zero)
			{
				args.PushText("Nothing.");
			}
			else
			{
				foreach (ReagentQuantity reagent in solution.Contents)
				{
					string name = reagent.Reagent.Prototype;
					if (_reagents.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(reagent.Reagent.Prototype), out Content.Shared._RMC14.Chemistry.Reagent.Reagent reagentProto))
					{
						name = reagentProto.LocalizedName;
					}
					args.PushText($"{reagent.Quantity.Float():F2} units of {name}");
				}
				args.PushText($"Total volume: {solution.Volume} / {solution.MaxVolume}.");
			}
			RMCToggleableSolutionTransferComponent transferComp = default(RMCToggleableSolutionTransferComponent);
			if (((EntitySystem)this).TryComp<RMCToggleableSolutionTransferComponent>(ent.Owner, ref transferComp))
			{
				string directionText = transferComp.Direction switch
				{
					SolutionTransferDirection.Input => "Transfer mode: Drawing", 
					SolutionTransferDirection.Output => "Transfer mode: Dispensing", 
					_ => string.Empty, 
				};
				if (!string.IsNullOrEmpty(directionText))
				{
					args.PushText(directionText);
				}
			}
		}
	}

	private void OnDispenserMapInit(Entity<RMCChemicalDispenserComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<ContainerSlot>(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), ent.Comp.ContainerSlotId, (ContainerManagerComponent)null);
	}

	private void OnToggleableSolutionTransferMapInit(Entity<RMCToggleableSolutionTransferComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Direction = SolutionTransferDirection.Input;
		((EntitySystem)this).RemCompDeferred<DrainableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
		RefillableSolutionComponent refillable = ((EntitySystem)this).EnsureComp<RefillableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
		refillable.Solution = ent.Comp.Solution;
		((EntitySystem)this).Dirty(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent), (IComponent)(object)refillable, (MetaDataComponent)null);
	}

	private void OnToggleableSolutionTransferVerbs(Entity<RMCToggleableSolutionTransferComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		bool dispensing = ((EntitySystem)this).HasComp<DrainableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
		args.Verbs.Add(new AlternativeVerb
		{
			Text = (dispensing ? "Enable drawing" : "Enable dispensing"),
			Act = delegate
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				dispensing = ((EntitySystem)this).HasComp<DrainableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
				if (dispensing)
				{
					((EntitySystem)this).RemCompDeferred<DrainableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
					RefillableSolutionComponent refillableSolutionComponent = ((EntitySystem)this).EnsureComp<RefillableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
					refillableSolutionComponent.Solution = ent.Comp.Solution;
					ent.Comp.Direction = SolutionTransferDirection.Input;
					((EntitySystem)this).Dirty(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent), (IComponent)(object)refillableSolutionComponent, (MetaDataComponent)null);
					_popup.PopupClient("Now drawing", Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent), user, PopupType.Medium);
				}
				else
				{
					((EntitySystem)this).RemCompDeferred<RefillableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
					DrainableSolutionComponent drainableSolutionComponent = ((EntitySystem)this).EnsureComp<DrainableSolutionComponent>(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent));
					drainableSolutionComponent.Solution = ent.Comp.Solution;
					ent.Comp.Direction = SolutionTransferDirection.Output;
					((EntitySystem)this).Dirty(Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent), (IComponent)(object)drainableSolutionComponent, (MetaDataComponent)null);
					_popup.PopupClient("Now dispensing", Entity<RMCToggleableSolutionTransferComponent>.op_Implicit(ent), user, PopupType.Medium);
				}
			}
		});
	}

	private void OnTransferWhitelistAttempt(Entity<RMCSolutionTransferWhitelistComponent> ent, ref SolutionTransferAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.From)
		{
			if (_entityWhitelist.IsWhitelistFail(ent.Comp.SourceWhitelist, args.To))
			{
				args.Cancel(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)));
			}
		}
		else if (_entityWhitelist.IsWhitelistFail(ent.Comp.TargetWhitelist, args.From))
		{
			args.Cancel(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Popup)));
		}
	}

	private void OnNoMixingReagentsExamined(Entity<NoMixingReagentsComponent> ent, ref ExaminedEvent args)
	{
		using (args.PushGroup("NoMixingReagentsComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-fuel-examine-cant-mix"));
		}
	}

	private void OnNoMixingReagentsTransferAttempt(Entity<NoMixingReagentsComponent> ent, ref SolutionTransferAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Solution tankSolution = args.FromSolution.Comp.Solution;
		Solution targetSolution = args.ToSolution.Comp.Solution;
		if (targetSolution.Contents.Count > 1)
		{
			args.Cancel(base.Loc.GetString("rmc-fuel-cant-mix"));
			return;
		}
		foreach (ReagentQuantity content in targetSolution.Contents)
		{
			if (tankSolution.Volume > FixedPoint2.Zero && !tankSolution.ContainsReagent(content.Reagent))
			{
				args.Cancel(base.Loc.GetString("rmc-fuel-cant-mix"));
				break;
			}
		}
	}

	private void OnEmptySolutionGetVerbs(Entity<RMCEmptySolutionComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanComplexInteract || !_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out Entity<SolutionComponent>? solutionEnt, out Solution _) || solutionEnt.Value.Comp.Solution.Volume <= FixedPoint2.Zero)
		{
			return;
		}
		args.Verbs.Add(new AlternativeVerb
		{
			Text = base.Loc.GetString("rmc-empty-solution-verb"),
			Act = delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				if (_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out solutionEnt, out Solution _))
				{
					_solution.RemoveAllSolution(solutionEnt.Value);
				}
			},
			Priority = 1
		});
	}

	private void OnChemicalDispenserSettingMsg(Entity<RMCChemicalDispenserComponent> ent, ref RMCChemicalDispenserDispenseSettingBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (Enumerable.Contains(ent.Comp.Settings, args.Amount))
		{
			ent.Comp.DispenseSetting = args.Amount;
			((EntitySystem)this).Dirty<RMCChemicalDispenserComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnChemicalDispenserBeakerSettingMsg(Entity<RMCChemicalDispenserComponent> ent, ref RMCChemicalDispenserBeakerBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (!_itemSlots.TryGetSlot(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), ent.Comp.ContainerSlotId, out ItemSlot slot))
		{
			return;
		}
		ContainerSlot? containerSlot = slot.ContainerSlot;
		EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid contained = val.GetValueOrDefault();
			if (_solution.TryGetMixableSolution(Entity<MixableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained), out Entity<SolutionComponent>? solutionEnt, out Solution _) && Enumerable.Contains(ent.Comp.Settings, args.Amount))
			{
				_solution.SplitSolution(solutionEnt.Value, args.Amount);
				DispenserUpdated(ent);
			}
		}
	}

	private void OnChemicalDispenserEjectBeakerMsg(Entity<RMCChemicalDispenserComponent> ent, ref RMCChemicalDispenserEjectBeakerBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (_itemSlots.TryGetSlot(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), ent.Comp.ContainerSlotId, out ItemSlot slot))
		{
			_itemSlots.TryEjectToHands(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), slot, ((BaseBoundUserInterfaceEvent)args).Actor, excludeUserAudio: true);
			((EntitySystem)this).Dirty<RMCChemicalDispenserComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnChemicalDispenserDispenseMsg(Entity<RMCChemicalDispenserComponent> ent, ref RMCChemicalDispenserDispenseBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (!_itemSlots.TryGetSlot(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), ent.Comp.ContainerSlotId, out ItemSlot slot))
		{
			return;
		}
		ContainerSlot? containerSlot = slot.ContainerSlot;
		EntityUid? val = ((containerSlot != null) ? containerSlot.ContainedEntity : ((EntityUid?)null));
		if (!val.HasValue)
		{
			return;
		}
		EntityUid contained = val.GetValueOrDefault();
		if (!_solution.TryGetMixableSolution(Entity<MixableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained), out Entity<SolutionComponent>? solutionEnt, out Solution _) || !Enumerable.Contains(ent.Comp.Reagents, args.Reagent) || !TryGetStorage(ent.Comp.Network, out Entity<RMCChemicalStorageComponent> storage))
		{
			return;
		}
		FixedPoint2 dispense = ent.Comp.DispenseSetting;
		FixedPoint2 available = solutionEnt.Value.Comp.Solution.AvailableVolume;
		if (dispense > available)
		{
			dispense = available;
		}
		if (!(dispense == FixedPoint2.Zero))
		{
			FixedPoint2 cost = (ent.Comp.FreeReagents.Contains(args.Reagent) ? FixedPoint2.Zero : (ent.Comp.CostPerUnit * dispense));
			if (!(cost > storage.Comp.Energy))
			{
				ChangeStorageEnergy(storage, storage.Comp.Energy - cost);
				_solution.TryAddReagent(solutionEnt.Value, ProtoId<ReagentPrototype>.op_Implicit(args.Reagent), ent.Comp.DispenseSetting);
			}
		}
	}

	public bool TryGetStorage(EntProtoId network, out Entity<RMCChemicalStorageComponent> storage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RMCChemicalStorageComponent> storages = ((EntitySystem)this).EntityQueryEnumerator<RMCChemicalStorageComponent>();
		EntityUid storageId = default(EntityUid);
		RMCChemicalStorageComponent storageComp = default(RMCChemicalStorageComponent);
		while (storages.MoveNext(ref storageId, ref storageComp))
		{
			if (!((EntitySystem)this).IsClientSide(storageId, (MetaDataComponent)null) && storageComp.Network == network)
			{
				storage = Entity<RMCChemicalStorageComponent>.op_Implicit((storageId, storageComp));
				return true;
			}
		}
		storage = default(Entity<RMCChemicalStorageComponent>);
		return false;
	}

	public void ChangeStorageEnergy(Entity<RMCChemicalStorageComponent> storage, FixedPoint2 energy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		storage.Comp.Energy = FixedPoint2.Clamp(energy, FixedPoint2.Zero, storage.Comp.MaxEnergy);
		((EntitySystem)this).Dirty<RMCChemicalStorageComponent>(storage, (MetaDataComponent)null);
		EntityQueryEnumerator<RMCChemicalDispenserComponent> dispensers = ((EntitySystem)this).EntityQueryEnumerator<RMCChemicalDispenserComponent>();
		EntityUid dispenserId = default(EntityUid);
		RMCChemicalDispenserComponent dispenserComp = default(RMCChemicalDispenserComponent);
		while (dispensers.MoveNext(ref dispenserId, ref dispenserComp))
		{
			if (!(dispenserComp.Network != storage.Comp.Network))
			{
				dispenserComp.Energy = energy;
				((EntitySystem)this).Dirty(dispenserId, (IComponent)(object)dispenserComp, (MetaDataComponent)null);
			}
		}
	}

	protected virtual void DispenserUpdated(Entity<RMCChemicalDispenserComponent> ent)
	{
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCChemicalStorageComponent> storages = ((EntitySystem)this).EntityQueryEnumerator<RMCChemicalStorageComponent>();
		EntityUid storageId = default(EntityUid);
		RMCChemicalStorageComponent storage = default(RMCChemicalStorageComponent);
		EntityUid dispenserId = default(EntityUid);
		RMCChemicalDispenserComponent dispenser = default(RMCChemicalDispenserComponent);
		while (storages.MoveNext(ref storageId, ref storage))
		{
			if (time < storage.RechargeAt)
			{
				continue;
			}
			storage.RechargeAt = time + storage.RechargeEvery;
			((EntitySystem)this).Dirty(storageId, (IComponent)(object)storage, (MetaDataComponent)null);
			TransformComponent storageTransform = ((EntitySystem)this).Transform(storageId);
			_dispensers.Clear();
			EntityQueryEnumerator<RMCChemicalDispenserComponent> dispensers = ((EntitySystem)this).EntityQueryEnumerator<RMCChemicalDispenserComponent>();
			while (dispensers.MoveNext(ref dispenserId, ref dispenser))
			{
				TransformComponent dispenserTransform = ((EntitySystem)this).Transform(dispenserId);
				if (dispenser.Network == storage.Network)
				{
					EntityUid? gridUid = storageTransform.GridUid;
					EntityUid? gridUid2 = dispenserTransform.GridUid;
					if (gridUid.HasValue == gridUid2.HasValue && (!gridUid.HasValue || gridUid.GetValueOrDefault() == gridUid2.GetValueOrDefault()))
					{
						_dispensers.Add(Entity<RMCChemicalDispenserComponent>.op_Implicit((dispenserId, dispenser)));
					}
				}
			}
			storage.MaxEnergy = storage.BaseMax + storage.MaxPer * _dispensers.Count;
			storage.Recharge = storage.BaseRecharge + storage.RechargePer * _dispensers.Count;
			if (!storage.Updated)
			{
				storage.Updated = true;
				storage.Energy = storage.MaxEnergy;
			}
			else
			{
				storage.Energy = FixedPoint2.Min(storage.MaxEnergy, storage.Energy + storage.Recharge);
			}
			foreach (Entity<RMCChemicalDispenserComponent> dispenser2 in _dispensers)
			{
				dispenser2.Comp.Energy = storage.Energy;
				dispenser2.Comp.MaxEnergy = storage.MaxEnergy;
				((EntitySystem)this).Dirty<RMCChemicalDispenserComponent>(dispenser2, (MetaDataComponent)null);
			}
		}
	}
}
