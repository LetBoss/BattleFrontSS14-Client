using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Rules;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Medical.Refill;

public sealed class CMRefillableSolutionSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private SolutionTransferSystem _solutionTransfer;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedDoAfterSystem _doafter;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CMRefillableSolutionComponent, ExaminedEvent>((EntityEventRefHandler<CMRefillableSolutionComponent, ExaminedEvent>)OnRefillableSolutionExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSolutionRefillerComponent, MapInitEvent>((EntityEventRefHandler<CMSolutionRefillerComponent, MapInitEvent>)OnRefillerMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMSolutionRefillerComponent, InteractUsingEvent>((EntityEventRefHandler<CMSolutionRefillerComponent, InteractUsingEvent>)OnRefillerInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRefillSolutionOnStoreComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCRefillSolutionOnStoreComponent, EntInsertedIntoContainerMessage>)OnRefillSolutionOnStoreInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, EntInsertedIntoContainerMessage>)OnRefillSolutionFromContainerOnStoreInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, GetVerbsEvent<AlternativeVerb>>)OnRefillSolutionFromContainerOnStoreGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRefillSolutionFromContainerOnStoreComponent, ContainerFlushDoAfterEvent>((EntityEventRefHandler<RMCRefillSolutionFromContainerOnStoreComponent, ContainerFlushDoAfterEvent>)OnRefillSolutionFromContainerOnStoreFlush, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlushableSolutionComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCFlushableSolutionComponent, GetVerbsEvent<AlternativeVerb>>)OnFlushableSolutionGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFlushableSolutionComponent, ContainerFlushDoAfterEvent>((EntityEventRefHandler<RMCFlushableSolutionComponent, ContainerFlushDoAfterEvent>)OnFlushableSolutionFlush, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPressurizedSolutionComponent, AfterInteractEvent>((EntityEventRefHandler<RMCPressurizedSolutionComponent, AfterInteractEvent>)OnPressurizedRefillAttempt, (Type[])null, (Type[])null);
	}

	private void OnRefillableSolutionExamined(Entity<CMRefillableSolutionComponent> ent, ref ExaminedEvent args)
	{
		using (args.PushGroup("CMRefillableSolutionComponent"))
		{
			args.PushMarkup("[color=cyan]This can be refilled by clicking on a medical vendor with it![/color]");
		}
	}

	private void OnRefillerMapInit(Entity<CMSolutionRefillerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = ((EntitySystem)this).Transform(ent.Owner);
		if (ent.Comp.RandomizeReagentsPlanetside && _rmcPlanet.IsOnPlanet(transform))
		{
			double amount = _random.NextDouble(0.0, ent.Comp.Max.Double() * 0.04) * 25.0;
			ent.Comp.Current = FixedPoint2.New(amount);
			((EntitySystem)this).Dirty<CMSolutionRefillerComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnRefillerInteractUsing(Entity<CMSolutionRefillerComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		EntityUid fillable = args.Used;
		RMCHyposprayComponent hypo = default(RMCHyposprayComponent);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryComp<RMCHyposprayComponent>(args.Used, ref hypo) && _container.TryGetContainer(args.Used, hypo.SlotId, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count != 0)
		{
			fillable = container.ContainedEntities[0];
		}
		CMRefillableSolutionComponent refillable = default(CMRefillableSolutionComponent);
		if (!((EntitySystem)this).TryComp<CMRefillableSolutionComponent>(fillable, ref refillable))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!_whitelist.IsValid(ent.Comp.Whitelist, fillable))
		{
			_popup.PopupClient(base.Loc.GetString("cm-refillable-solution-cannot-refill", (ValueTuple<string, object>)("user", ent.Owner), (ValueTuple<string, object>)("target", fillable)), args.User, args.User, PopupType.SmallCaution);
		}
		else
		{
			if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(fillable), refillable.Solution, out Entity<SolutionComponent>? solution))
			{
				return;
			}
			Solution solutionComp = solution.Value.Comp.Solution;
			if (solutionComp.AvailableVolume == FixedPoint2.Zero)
			{
				_popup.PopupClient(base.Loc.GetString("cm-refillable-solution-full", (ValueTuple<string, object>)("target", fillable)), args.User, args.User);
				return;
			}
			bool anyRefilled = false;
			foreach (var (reagent, amount) in refillable.Reagents)
			{
				if (ent.Comp.Reagents.Contains(reagent))
				{
					FixedPoint2 refill = FixedPoint2.Min(ent.Comp.Current, amount);
					refill = FixedPoint2.Min(refill, solutionComp.AvailableVolume);
					if (refill == FixedPoint2.Zero)
					{
						break;
					}
					ent.Comp.Current -= refill;
					_solution.TryAddReagent(solution.Value, ProtoId<ReagentPrototype>.op_Implicit(reagent), refill);
					anyRefilled = true;
				}
			}
			if (anyRefilled)
			{
				((EntitySystem)this).Dirty<CMSolutionRefillerComponent>(ent, (MetaDataComponent)null);
				RefilledSolutionEvent ev = new RefilledSolutionEvent();
				((EntitySystem)this).RaiseLocalEvent<RefilledSolutionEvent>(args.Used, ref ev, false);
				_popup.PopupClient(base.Loc.GetString("cm-refillable-solution-whirring-noise", (ValueTuple<string, object>)("user", ent.Owner), (ValueTuple<string, object>)("target", fillable)), args.User, args.User);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("cm-refillable-solution-cannot-refill", (ValueTuple<string, object>)("user", ent.Owner), (ValueTuple<string, object>)("target", fillable)), args.User, args.User, PopupType.SmallCaution);
			}
		}
	}

	private void OnRefillSolutionOnStoreInserted(Entity<RMCRefillSolutionOnStoreComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.SolutionId, out Entity<SolutionComponent>? solutionEnt) && TryGetStorageFillableSolution(Entity<SolutionStorageFillableComponent, SolutionContainerManagerComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity), out Entity<SolutionComponent>? refillable, out Solution _))
		{
			FixedPoint2 volume = refillable.Value.Comp.Solution.AvailableVolume;
			_solutionTransfer.Transfer(null, Entity<RMCRefillSolutionOnStoreComponent>.op_Implicit(ent), solutionEnt.Value, ((ContainerModifiedMessage)args).Entity, refillable.Value, volume);
		}
	}

	private void OnRefillSolutionFromContainerOnStoreInserted(Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		EntityUid? contained = default(EntityUid?);
		if (_container.TryGetContainer(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref contained) && (_solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained.Value), out Entity<SolutionComponent>? drainable, out Solution sol) || TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained.Value), out drainable, out sol)))
		{
			if (sol != null)
			{
				_appearance.SetData(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), (Enum)SolutionContainerStoreVisuals.Color, (object)sol.GetColor(_proto), (AppearanceComponent)null);
			}
			if (TryGetStorageFillableSolution(Entity<SolutionStorageFillableComponent, SolutionContainerManagerComponent>.op_Implicit(((ContainerModifiedMessage)args).Entity), out Entity<SolutionComponent>? refillable, out Solution _))
			{
				FixedPoint2 volume = refillable.Value.Comp.Solution.AvailableVolume;
				Solution drained = _solution.SplitSolution(drainable.Value, volume);
				_solution.AddSolution(refillable.Value, drained);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<CMSolutionRefillerComponent, TransformComponent> refillers = ((EntitySystem)this).EntityQueryEnumerator<CMSolutionRefillerComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		CMSolutionRefillerComponent comp = default(CMSolutionRefillerComponent);
		TransformComponent xform = default(TransformComponent);
		while (refillers.MoveNext(ref uid, ref comp, ref xform))
		{
			if (time < comp.RechargeAt)
			{
				continue;
			}
			comp.RechargeAt = time + comp.RechargeCooldown;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			if (!xform.Anchored)
			{
				continue;
			}
			RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(uid, null, (DirectionFlag)0);
			bool any = false;
			EntityUid anchoredId;
			while (anchored.MoveNext(out anchoredId))
			{
				if (((EntitySystem)this).HasComp<CMMedicalSupplyLinkComponent>(anchoredId))
				{
					any = true;
					break;
				}
			}
			if (any)
			{
				comp.Current = FixedPoint2.Min(comp.Max, comp.Current + comp.Recharge);
			}
		}
	}

	public bool TryGetStorageFillableSolution(Entity<SolutionStorageFillableComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SolutionStorageFillableComponent>(Entity<SolutionStorageFillableComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RMCPressurizedSolutionComponent>(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	private void OnPressurizedRefillAttempt(Entity<RMCPressurizedSolutionComponent> beaker, ref AfterInteractEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		Entity<RMCPressurizedSolutionComponent> val = beaker;
		EntityUid val2 = default(EntityUid);
		RMCPressurizedSolutionComponent rMCPressurizedSolutionComponent = default(RMCPressurizedSolutionComponent);
		val.Deconstruct(ref val2, ref rMCPressurizedSolutionComponent);
		EntityUid uid = val2;
		SolutionTransferComponent transfer = default(SolutionTransferComponent);
		if (!((EntitySystem)this).HasComp<ReagentTankComponent>(target2) || !((EntitySystem)this).TryComp<SolutionTransferComponent>(Entity<RMCPressurizedSolutionComponent>.op_Implicit(beaker), ref transfer))
		{
			return;
		}
		if (!((EntitySystem)this).HasComp<RefillableSolutionComponent>(target2) && _solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(target2), out Entity<SolutionComponent>? targetSoln, out Solution solution) && TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((ValueTuple<EntityUid, RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>)(uid, null, null)), out Entity<SolutionComponent>? ownerSoln, out Solution ownerRefill))
		{
			FixedPoint2 transferAmount = transfer.TransferAmount;
			FixedPoint2 transferred = _solutionTransfer.Transfer(args.User, target2, targetSoln.Value, uid, ownerSoln.Value, transferAmount);
			((HandledEntityEventArgs)args).Handled = true;
			if (transferred > 0)
			{
				string msg = ((ownerRefill.AvailableVolume == 0) ? "comp-solution-transfer-fill-fully" : "comp-solution-transfer-fill-normal");
				_popup.PopupPredicted(base.Loc.GetString(msg, new(string, object)[3]
				{
					("owner", args.Target),
					("amount", transferred),
					("target", uid)
				}), uid, args.User);
				return;
			}
		}
		RefillableSolutionComponent targetRefill = default(RefillableSolutionComponent);
		if (((EntitySystem)this).TryComp<RefillableSolutionComponent>(target2, ref targetRefill) && _solution.TryGetRefillableSolution(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit((ValueTuple<EntityUid, RefillableSolutionComponent, SolutionContainerManagerComponent>)(target2, targetRefill, null)), out targetSoln, out solution) && TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(uid), out ownerSoln, out solution))
		{
			FixedPoint2 transferAmount2 = transfer.TransferAmount;
			FixedPoint2? fixedPoint = targetRefill?.MaxRefill;
			if (fixedPoint.HasValue)
			{
				FixedPoint2 maxRefill = fixedPoint.GetValueOrDefault();
				transferAmount2 = FixedPoint2.Min(transferAmount2, maxRefill);
			}
			FixedPoint2 transferred2 = _solutionTransfer.Transfer(args.User, uid, ownerSoln.Value, target2, targetSoln.Value, transferAmount2);
			((HandledEntityEventArgs)args).Handled = true;
			if (transferred2 > 0)
			{
				string message = base.Loc.GetString("comp-solution-transfer-transfer-solution", (ValueTuple<string, object>)("amount", transferred2), (ValueTuple<string, object>)("target", target2));
				_popup.PopupEntity(message, uid, args.User);
			}
		}
	}

	private void OnRefillSolutionFromContainerOnStoreGetVerbs(Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		EntityUid? contained = default(EntityUid?);
		if (args.CanAccess && args.CanInteract && ent.Comp.CanFlush && _container.TryGetContainer(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref contained))
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("rmc-refillsolution-flush"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					TryFlushContainer(ent, user);
				}
			});
		}
	}

	private void TryFlushContainer(Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		EntityUid? contained = default(EntityUid?);
		if (ent.Comp.CanFlush && _container.TryGetContainer(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref contained) && (_solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained.Value), out Entity<SolutionComponent>? drainable, out Solution sol) || TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained.Value), out drainable, out sol)))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-refillsolution-flush-start", (ValueTuple<string, object>)("time", ent.Comp.FlushTime.TotalSeconds)), user, user, PopupType.SmallCaution);
			_doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.FlushTime, new ContainerFlushDoAfterEvent(), Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent))
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameTarget
			});
		}
	}

	private void OnRefillSolutionFromContainerOnStoreFlush(Entity<RMCRefillSolutionFromContainerOnStoreComponent> ent, ref ContainerFlushDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CanFlush || args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		BaseContainer container = default(BaseContainer);
		EntityUid? contained = default(EntityUid?);
		if (_container.TryGetContainer(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) && Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref contained) && (_solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained.Value), out Entity<SolutionComponent>? drainable, out Solution sol) || TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(contained.Value), out drainable, out sol)))
		{
			_solution.RemoveAllSolution(drainable.Value);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), ref appearance))
			{
				_appearance.QueueUpdate(Entity<RMCRefillSolutionFromContainerOnStoreComponent>.op_Implicit(ent), appearance);
			}
		}
	}

	private void OnFlushableSolutionGetVerbs(Entity<RMCFlushableSolutionComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("rmc-refillsolution-flush"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					TryFlushSolution(ent, user);
				}
			});
		}
	}

	private void TryFlushSolution(Entity<RMCFlushableSolutionComponent> ent, EntityUid user)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-refillsolution-flush-start", (ValueTuple<string, object>)("time", ent.Comp.FlushTime.TotalSeconds)), user, user, PopupType.SmallCaution);
		_doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.FlushTime, new ContainerFlushDoAfterEvent(), Entity<RMCFlushableSolutionComponent>.op_Implicit(ent), Entity<RMCFlushableSolutionComponent>.op_Implicit(ent))
		{
			BreakOnMove = true,
			DuplicateCondition = DuplicateConditions.SameTarget
		});
	}

	private void OnFlushableSolutionFlush(Entity<RMCFlushableSolutionComponent> ent, ref ContainerFlushDoAfterEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out Entity<SolutionComponent>? solution, out Solution _))
		{
			_solution.RemoveAllSolution(solution.Value);
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<RMCFlushableSolutionComponent>.op_Implicit(ent), ref appearance))
			{
				_appearance.QueueUpdate(Entity<RMCFlushableSolutionComponent>.op_Implicit(ent), appearance);
			}
		}
	}
}
