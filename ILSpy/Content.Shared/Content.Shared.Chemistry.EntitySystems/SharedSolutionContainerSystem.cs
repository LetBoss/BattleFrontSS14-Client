using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Kitchen.Components;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Chemistry.EntitySystems;

public abstract class SharedSolutionContainerSystem : EntitySystem
{
	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	protected ChemicalReactionSystem ChemicalReactionSystem;

	[Dependency]
	protected ExamineSystemShared ExamineSystem;

	[Dependency]
	protected SharedAppearanceSystem AppearanceSystem;

	[Dependency]
	protected SharedHandsSystem Hands;

	[Dependency]
	protected SharedContainerSystem ContainerSystem;

	[Dependency]
	protected MetaDataSystem MetaDataSys;

	[Dependency]
	protected INetManager NetManager;

	[Dependency]
	private RMCReagentSystem _rmcReagents;

	public bool TryGetRefillableSolution(Entity<RefillableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RefillableSolutionComponent>(Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetDrainableSolution(Entity<DrainableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DrainableSolutionComponent>(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetExtractableSolution(Entity<ExtractableComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ExtractableComponent>(Entity<ExtractableComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.GrindableSolution, out soln, out solution);
	}

	public bool TryGetDumpableSolution(Entity<DumpableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DumpableSolutionComponent>(Entity<DumpableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetDrawableSolution(Entity<DrawableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DrawableSolutionComponent>(Entity<DrawableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetInjectableSolution(Entity<InjectableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InjectableSolutionComponent>(Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetFitsInDispenser(Entity<FitsInDispenserComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FitsInDispenserComponent>(Entity<FitsInDispenserComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public bool TryGetMixableSolution(Entity<MixableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MixableSolutionComponent>(Entity<MixableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(entity), ref entity.Comp1, false))
		{
			soln = null;
			solution = null;
			return false;
		}
		return TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp2)), entity.Comp1.Solution, out soln, out solution);
	}

	public void Refill(Entity<RefillableSolutionComponent?> entity, Entity<SolutionComponent> soln, Solution refill)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RefillableSolutionComponent>(Entity<RefillableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			AddSolution(soln, refill);
		}
	}

	public void Inject(Entity<InjectableSolutionComponent?> entity, Entity<SolutionComponent> soln, Solution inject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<InjectableSolutionComponent>(Entity<InjectableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			AddSolution(soln, inject);
		}
	}

	public Solution Drain(Entity<DrainableSolutionComponent?> entity, Entity<SolutionComponent> soln, FixedPoint2 quantity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DrainableSolutionComponent>(Entity<DrainableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return new Solution();
		}
		return SplitSolution(soln, quantity);
	}

	public Solution Draw(Entity<DrawableSolutionComponent?> entity, Entity<SolutionComponent> soln, FixedPoint2 quantity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DrawableSolutionComponent>(Entity<DrawableSolutionComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return new Solution();
		}
		return SplitSolution(soln, quantity);
	}

	public float PercentFull(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(uid), out Entity<SolutionComponent>? _, out Solution solution) || solution.MaxVolume.Equals(FixedPoint2.Zero))
		{
			return 0f;
		}
		return solution.FillFraction * 100f;
	}

	public static string ToPrettyString(Solution solution)
	{
		StringBuilder sb = new StringBuilder();
		if (solution.Name == null)
		{
			sb.Append("[");
		}
		else
		{
			StringBuilder stringBuilder = sb;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
			handler.AppendFormatted(solution.Name);
			handler.AppendLiteral(":[");
			stringBuilder.Append(ref handler);
		}
		bool first = true;
		foreach (var (id, quantity) in solution.Contents)
		{
			if (first)
			{
				first = false;
			}
			else
			{
				sb.Append(", ");
			}
			sb.AppendFormat("{0}: {1}u", id, quantity);
		}
		sb.Append(']');
		return sb.ToString();
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeRelays();
		((EntitySystem)this).SubscribeLocalEvent<SolutionComponent, ComponentInit>((EntityEventRefHandler<SolutionComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionComponent, ComponentStartup>((EntityEventRefHandler<SolutionComponent, ComponentStartup>)OnSolutionStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionComponent, ComponentShutdown>((EntityEventRefHandler<SolutionComponent, ComponentShutdown>)OnSolutionShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentInit>((EntityEventRefHandler<SolutionContainerManagerComponent, ComponentInit>)OnContainerManagerInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExaminableSolutionComponent, ExaminedEvent>((EntityEventRefHandler<ExaminableSolutionComponent, ExaminedEvent>)OnExamineSolution, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExaminableSolutionComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<ExaminableSolutionComponent, GetVerbsEvent<ExamineVerb>>)OnSolutionExaminableVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerManagerComponent, MapInitEvent>((EntityEventRefHandler<SolutionContainerManagerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		if (NetManager.IsServer)
		{
			((EntitySystem)this).SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentShutdown>((EntityEventRefHandler<SolutionContainerManagerComponent, ComponentShutdown>)OnContainerManagerShutdown, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeLocalEvent<ContainedSolutionComponent, ComponentShutdown>((EntityEventRefHandler<ContainedSolutionComponent, ComponentShutdown>)OnContainedSolutionShutdown, (Type[])null, (Type[])null);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ResolveSolution(Entity<SolutionContainerManagerComponent?> container, string? name, [NotNullWhen(true)] ref Entity<SolutionComponent>? entity, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveSolution(container, name, ref entity))
		{
			solution = null;
			return false;
		}
		solution = entity.Value.Comp.Solution;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ResolveSolution(Entity<SolutionContainerManagerComponent?> container, string? name, [NotNullWhen(true)] ref Entity<SolutionComponent>? entity)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent>? val = entity;
		if (val.HasValue)
		{
			return true;
		}
		return TryGetSolution(container, name, out entity);
	}

	public bool TryGetSolution(Entity<SolutionContainerManagerComponent?> container, string? name, [NotNullWhen(true)] out Entity<SolutionComponent>? entity, [NotNullWhen(true)] out Solution? solution, bool errorOnMissing = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetSolution(container, name, out entity, errorOnMissing))
		{
			solution = null;
			return false;
		}
		solution = entity.Value.Comp.Solution;
		return true;
	}

	public bool TryGetSolution(Entity<SolutionContainerManagerComponent?> container, string? name, [NotNullWhen(true)] out Entity<SolutionComponent>? entity, bool errorOnMissing = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		GetConnectedContainerEvent ev = default(GetConnectedContainerEvent);
		((EntitySystem)this).RaiseLocalEvent<GetConnectedContainerEvent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref ev, false);
		if (ev.ContainerEntity.HasValue)
		{
			container = Entity<SolutionContainerManagerComponent>.op_Implicit(ev.ContainerEntity.Value);
		}
		EntityUid uid;
		if (name == null)
		{
			uid = Entity<SolutionContainerManagerComponent>.op_Implicit(container);
			goto IL_0115;
		}
		BaseContainer solutionContainer = default(BaseContainer);
		if (ContainerSystem.TryGetContainer(Entity<SolutionContainerManagerComponent>.op_Implicit(container), "solution@" + name, ref solutionContainer, (ContainerManagerComponent)null))
		{
			ContainerSlot solutionSlot = (ContainerSlot)(object)((solutionContainer is ContainerSlot) ? solutionContainer : null);
			if (solutionSlot != null)
			{
				EntityUid? containedEntity = solutionSlot.ContainedEntity;
				if (containedEntity.HasValue)
				{
					EntityUid containedSolution = containedEntity.GetValueOrDefault();
					SolutionAccessAttemptEvent attemptEv = new SolutionAccessAttemptEvent(name);
					((EntitySystem)this).RaiseLocalEvent<SolutionAccessAttemptEvent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref attemptEv, false);
					if (attemptEv.Cancelled)
					{
						entity = null;
						return false;
					}
					uid = containedSolution;
					goto IL_0115;
				}
			}
		}
		entity = null;
		if (!errorOnMissing)
		{
			return false;
		}
		((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<SolutionContainerManagerComponent>.op_Implicit(container), (MetaDataComponent)null)} does not have a solution with ID: {name}");
		return false;
		IL_0115:
		SolutionComponent comp = default(SolutionComponent);
		if (!((EntitySystem)this).TryComp<SolutionComponent>(uid, ref comp))
		{
			entity = null;
			if (!errorOnMissing)
			{
				return false;
			}
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<SolutionContainerManagerComponent>.op_Implicit(container), (MetaDataComponent)null)} does not have a solution with ID: {name}");
			return false;
		}
		entity = Entity<SolutionComponent>.op_Implicit((uid, comp));
		return true;
	}

	public bool TryGetSolution(SolutionContainerManagerComponent container, string name, [NotNullWhen(true)] out Solution? solution, bool errorOnMissing = false)
	{
		solution = null;
		if (container.Solutions != null)
		{
			return container.Solutions.TryGetValue(name, out solution);
		}
		if (!errorOnMissing)
		{
			return false;
		}
		((EntitySystem)this).Log.Error($"{container} does not have a solution with ID: {name}");
		return false;
	}

	public IEnumerable<(string? Name, Entity<SolutionComponent> Solution)> EnumerateSolutions(Entity<SolutionContainerManagerComponent?> container, bool includeSelf = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SolutionComponent solutionComp = default(SolutionComponent);
		if (includeSelf && ((EntitySystem)this).TryComp<SolutionComponent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref solutionComp))
		{
			yield return (Name: null, Solution: Entity<SolutionComponent>.op_Implicit((container.Owner, solutionComp)));
		}
		if (!((EntitySystem)this).Resolve<SolutionContainerManagerComponent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref container.Comp, false))
		{
			yield break;
		}
		foreach (string name in container.Comp.Containers)
		{
			SolutionAccessAttemptEvent attemptEv = new SolutionAccessAttemptEvent(name);
			((EntitySystem)this).RaiseLocalEvent<SolutionAccessAttemptEvent>(Entity<SolutionContainerManagerComponent>.op_Implicit(container), ref attemptEv, false);
			if (attemptEv.Cancelled)
			{
				continue;
			}
			BaseContainer container2 = ContainerSystem.GetContainer(Entity<SolutionContainerManagerComponent>.op_Implicit(container), "solution@" + name, (ContainerManagerComponent)null);
			ContainerSlot slot = (ContainerSlot)(object)((container2 is ContainerSlot) ? container2 : null);
			if (slot != null)
			{
				EntityUid? containedEntity = slot.ContainedEntity;
				if (containedEntity.HasValue)
				{
					EntityUid solutionId = containedEntity.GetValueOrDefault();
					yield return (Name: name, Solution: Entity<SolutionComponent>.op_Implicit((solutionId, ((EntitySystem)this).Comp<SolutionComponent>(solutionId))));
				}
			}
		}
	}

	public IEnumerable<(string Name, Solution Solution)> EnumerateSolutions(SolutionContainerManagerComponent container)
	{
		Dictionary<string, Solution> solutions = container.Solutions;
		if (solutions == null || solutions.Count <= 0)
		{
			yield break;
		}
		foreach (var (name, solution2) in solutions)
		{
			yield return (Name: name, Solution: solution2);
		}
	}

	protected void UpdateAppearance(Entity<AppearanceComponent?> container, Entity<SolutionComponent, ContainedSolutionComponent> soln)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Entity<AppearanceComponent> val = container;
		EntityUid val2 = default(EntityUid);
		AppearanceComponent val3 = default(AppearanceComponent);
		val.Deconstruct(ref val2, ref val3);
		EntityUid uid = val2;
		AppearanceComponent appearanceComponent = val3;
		if (((EntitySystem)this).HasComp<SolutionContainerVisualsComponent>(uid) && ((EntitySystem)this).Resolve<AppearanceComponent>(uid, ref appearanceComponent, false))
		{
			Entity<SolutionComponent, ContainedSolutionComponent> val4 = soln;
			SolutionComponent solutionComponent = default(SolutionComponent);
			ContainedSolutionComponent containedSolutionComponent = default(ContainedSolutionComponent);
			val4.Deconstruct(ref val2, ref solutionComponent, ref containedSolutionComponent);
			SolutionComponent solutionComponent2 = solutionComponent;
			ContainedSolutionComponent relation = containedSolutionComponent;
			Solution solution = solutionComponent2.Solution;
			AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.FillFraction, (object)solution.FillFraction, appearanceComponent);
			AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.Color, (object)solution.GetColor(PrototypeManager), appearanceComponent);
			AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.SolutionName, (object)relation.ContainerName, appearanceComponent);
			ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
			if (primaryReagentId.HasValue)
			{
				ReagentId reagent = primaryReagentId.GetValueOrDefault();
				AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.BaseOverride, (object)reagent.ToString(), appearanceComponent);
			}
		}
	}

	public FixedPoint2 GetTotalPrototypeQuantity(EntityUid owner, string reagentId)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 reagentQuantity = FixedPoint2.New(0);
		SolutionContainerManagerComponent managerComponent = default(SolutionContainerManagerComponent);
		if (((EntitySystem)this).Exists(owner) && ((EntitySystem)this).TryComp<SolutionContainerManagerComponent>(owner, ref managerComponent))
		{
			foreach (var item in EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((owner, managerComponent))))
			{
				Solution solution = item.Solution.Comp.Solution;
				reagentQuantity += solution.GetTotalPrototypeQuantity(reagentId);
			}
		}
		return reagentQuantity;
	}

	public void UpdateChemicals(Entity<SolutionComponent> soln, bool needsReactionsProcessing = true, ReactionMixerComponent? mixerComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty<SolutionComponent>(soln, (MetaDataComponent)null);
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		EntityUid uid = val2;
		SolutionComponent comp = solutionComponent;
		Solution solution = comp.Solution;
		if (needsReactionsProcessing && solution.CanReact)
		{
			ChemicalReactionSystem.FullyReactSolution(soln, mixerComponent);
		}
		FixedPoint2 overflow = solution.Volume - solution.MaxVolume;
		if (overflow > FixedPoint2.Zero)
		{
			SolutionOverflowEvent overflowEv = new SolutionOverflowEvent(soln, overflow);
			((EntitySystem)this).RaiseLocalEvent<SolutionOverflowEvent>(uid, ref overflowEv, false);
		}
		UpdateAppearance(Entity<SolutionComponent, AppearanceComponent>.op_Implicit((ValueTuple<EntityUid, SolutionComponent, AppearanceComponent>)(uid, comp, null)));
		SolutionChangedEvent changedEv = new SolutionChangedEvent(soln);
		((EntitySystem)this).RaiseLocalEvent<SolutionChangedEvent>(uid, ref changedEv, false);
	}

	public void UpdateAppearance(Entity<SolutionComponent, AppearanceComponent?> soln)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent, AppearanceComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		AppearanceComponent val3 = default(AppearanceComponent);
		val.Deconstruct(ref val2, ref solutionComponent, ref val3);
		EntityUid uid = val2;
		SolutionComponent solutionComponent2 = solutionComponent;
		AppearanceComponent appearanceComponent = val3;
		Solution solution = solutionComponent2.Solution;
		if (((EntitySystem)this).Exists(uid) && ((EntitySystem)this).Resolve<AppearanceComponent>(uid, ref appearanceComponent, false))
		{
			AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.FillFraction, (object)solution.FillFraction, appearanceComponent);
			AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.Color, (object)solution.GetColor(PrototypeManager), appearanceComponent);
			ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
			if (primaryReagentId.HasValue)
			{
				ReagentId reagent = primaryReagentId.GetValueOrDefault();
				AppearanceSystem.SetData(uid, (Enum)SolutionContainerVisuals.BaseOverride, (object)reagent.ToString(), appearanceComponent);
			}
		}
	}

	public Solution SplitSolution(Entity<SolutionComponent> soln, FixedPoint2 quantity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution result = solutionComponent.Solution.SplitSolution(quantity);
		UpdateChemicals(soln);
		return result;
	}

	public Solution SplitStackSolution(Entity<SolutionComponent> soln, FixedPoint2 quantity, int stackCount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		Solution splitSol = solution.SplitSolution(quantity / stackCount);
		solution.SplitSolution(quantity - splitSol.Volume);
		UpdateChemicals(soln);
		return splitSol;
	}

	public Solution SplitSolutionWithout(Entity<SolutionComponent> soln, FixedPoint2 quantity, params string[] reagents)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution result = solutionComponent.Solution.SplitSolutionWithout(quantity, reagents);
		UpdateChemicals(soln);
		return result;
	}

	public void RemoveAllSolution(Entity<SolutionComponent> soln)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (!(solution.Volume == 0))
		{
			solution.RemoveAllSolution();
			UpdateChemicals(soln);
		}
	}

	public void SetCapacity(Entity<SolutionComponent> soln, FixedPoint2 capacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (!(solution.MaxVolume == capacity))
		{
			solution.MaxVolume = capacity;
			UpdateChemicals(soln);
		}
	}

	public bool TryAddReagent(Entity<SolutionComponent> soln, ReagentQuantity reagentQuantity, out FixedPoint2 acceptedQuantity, float? temperature = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		acceptedQuantity = ((solution.AvailableVolume > reagentQuantity.Quantity) ? reagentQuantity.Quantity : solution.AvailableVolume);
		if (acceptedQuantity <= 0)
		{
			return reagentQuantity.Quantity == 0;
		}
		if (!temperature.HasValue)
		{
			solution.AddReagent(reagentQuantity.Reagent, acceptedQuantity);
		}
		else
		{
			Content.Shared._RMC14.Chemistry.Reagent.Reagent proto = _rmcReagents.Index(ProtoId<ReagentPrototype>.op_Implicit(reagentQuantity.Reagent.Prototype));
			solution.AddReagent(proto, acceptedQuantity, temperature.Value, PrototypeManager);
		}
		UpdateChemicals(soln);
		return acceptedQuantity == reagentQuantity.Quantity;
	}

	public bool TryAddReagent(Entity<SolutionComponent> soln, string prototype, FixedPoint2 quantity, float? temperature = null, List<ReagentData>? data = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 acceptedQuantity;
		return TryAddReagent(soln, new ReagentQuantity(prototype, quantity, data), out acceptedQuantity, temperature);
	}

	public bool TryAddReagent(Entity<SolutionComponent> soln, string prototype, FixedPoint2 quantity, out FixedPoint2 acceptedQuantity, float? temperature = null, List<ReagentData>? data = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ReagentQuantity reagent = new ReagentQuantity(prototype, quantity, data);
		return TryAddReagent(soln, reagent, out acceptedQuantity, temperature);
	}

	public bool TryAddReagent(Entity<SolutionComponent> soln, ReagentId reagentId, FixedPoint2 quantity, out FixedPoint2 acceptedQuantity, float? temperature = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		ReagentQuantity quant = new ReagentQuantity(reagentId, quantity);
		return TryAddReagent(soln, quant, out acceptedQuantity, temperature);
	}

	public bool RemoveReagent(Entity<SolutionComponent> soln, ReagentQuantity reagentQuantity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		if (solutionComponent.Solution.RemoveReagent(reagentQuantity) <= FixedPoint2.Zero)
		{
			return false;
		}
		UpdateChemicals(soln);
		return true;
	}

	public bool RemoveReagent(Entity<SolutionComponent> soln, string prototype, FixedPoint2 quantity, List<ReagentData>? data = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return RemoveReagent(soln, new ReagentQuantity(prototype, quantity, data));
	}

	public bool RemoveReagent(Entity<SolutionComponent> soln, ReagentId reagentId, FixedPoint2 quantity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return RemoveReagent(soln, new ReagentQuantity(reagentId, quantity));
	}

	public bool TryTransferSolution(Entity<SolutionComponent> soln, Solution source, FixedPoint2 quantity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (quantity < 0)
		{
			throw new InvalidOperationException("Quantity must be positive");
		}
		quantity = FixedPoint2.Min(quantity, solution.AvailableVolume, source.Volume);
		if (quantity == 0)
		{
			return false;
		}
		solution.AddSolution(source.SplitSolution(quantity), PrototypeManager);
		UpdateChemicals(soln);
		return true;
	}

	public bool TryAddSolution(Entity<SolutionComponent> soln, Solution toAdd)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (toAdd.Volume == FixedPoint2.Zero)
		{
			return true;
		}
		if (toAdd.Volume > solution.AvailableVolume)
		{
			return false;
		}
		ForceAddSolution(soln, toAdd);
		return true;
	}

	public FixedPoint2 AddSolution(Entity<SolutionComponent> soln, Solution toAdd)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (toAdd.Volume == FixedPoint2.Zero)
		{
			return FixedPoint2.Zero;
		}
		FixedPoint2 quantity = FixedPoint2.Max(FixedPoint2.Zero, FixedPoint2.Min(toAdd.Volume, solution.AvailableVolume));
		if (quantity < toAdd.Volume)
		{
			TryTransferSolution(soln, toAdd, quantity);
		}
		else
		{
			ForceAddSolution(soln, toAdd);
		}
		return quantity;
	}

	public bool ForceAddSolution(Entity<SolutionComponent> soln, Solution toAdd)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (toAdd.Volume == FixedPoint2.Zero)
		{
			return false;
		}
		solution.AddSolution(toAdd, PrototypeManager);
		UpdateChemicals(soln);
		return true;
	}

	public bool TryMixAndOverflow(Entity<SolutionComponent> soln, Solution toAdd, FixedPoint2 overflowThreshold, [MaybeNullWhen(false)] out Solution overflowingSolution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (toAdd.Volume == 0 || overflowThreshold > solution.MaxVolume)
		{
			overflowingSolution = null;
			return false;
		}
		solution.AddSolution(toAdd, PrototypeManager);
		overflowingSolution = solution.SplitSolution(FixedPoint2.Max(FixedPoint2.Zero, solution.Volume - overflowThreshold));
		UpdateChemicals(soln);
		return true;
	}

	public Solution RemoveEachReagent(Entity<SolutionComponent> soln, FixedPoint2 quantity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (quantity <= 0)
		{
			return new Solution();
		}
		Solution removedSolution = new Solution();
		for (int i = solution.Contents.Count - 1; i >= 0; i--)
		{
			solution.Contents[i].Deconstruct(out var id, out var _);
			ReagentId reagent = id;
			FixedPoint2 removedQuantity = solution.RemoveReagent(reagent, quantity);
			removedSolution.AddReagent(reagent, removedQuantity);
		}
		UpdateChemicals(soln);
		return removedSolution;
	}

	public void SetTemperature(Entity<SolutionComponent> soln, float temperature)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (temperature != solution.Temperature)
		{
			solution.Temperature = temperature;
			UpdateChemicals(soln);
		}
	}

	public void SetThermalEnergy(Entity<SolutionComponent> soln, float thermalEnergy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		float heatCap = solution.GetHeatCapacity(PrototypeManager);
		solution.Temperature = ((heatCap == 0f) ? 0f : (thermalEnergy / heatCap));
		UpdateChemicals(soln);
	}

	public void AddThermalEnergy(Entity<SolutionComponent> soln, float thermalEnergy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		if (thermalEnergy != 0f)
		{
			float heatCap = solution.GetHeatCapacity(PrototypeManager);
			solution.Temperature += ((heatCap == 0f) ? 0f : (thermalEnergy / heatCap));
			UpdateChemicals(soln);
		}
	}

	private void OnComponentInit(Entity<SolutionComponent> entity, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.Solution.ValidateSolution();
	}

	private void OnSolutionStartup(Entity<SolutionComponent> entity, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateChemicals(entity);
	}

	private void OnSolutionShutdown(Entity<SolutionComponent> entity, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveAllSolution(entity);
	}

	private void OnContainerManagerInit(Entity<SolutionContainerManagerComponent> entity, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> containers = entity.Comp.Containers;
		if (containers == null || containers.Count <= 0)
		{
			return;
		}
		ContainerManagerComponent containerManager = ((EntitySystem)this).EnsureComp<ContainerManagerComponent>(Entity<SolutionContainerManagerComponent>.op_Implicit(entity));
		foreach (string name in containers)
		{
			ContainerSystem.EnsureContainer<ContainerSlot>(entity.Owner, "solution@" + name, containerManager);
		}
	}

	private void OnExamineSolution(Entity<ExaminableSolutionComponent> entity, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(entity.Owner), entity.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution) || !CanSeeHiddenSolution(entity, args.Examiner))
		{
			return;
		}
		ReagentId? primaryReagent = solution.GetPrimaryReagentId();
		if (string.IsNullOrEmpty(primaryReagent?.Prototype))
		{
			args.PushText(base.Loc.GetString("shared-solution-container-component-on-examine-empty-container"));
			return;
		}
		if (!_rmcReagents.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(primaryReagent.Value.Prototype), out Content.Shared._RMC14.Chemistry.Reagent.Reagent primary))
		{
			((EntitySystem)this).Log.Error($"{"Solution"} could not find the prototype associated with {primaryReagent}.");
			return;
		}
		Color val = solution.GetColor(PrototypeManager);
		string colorHex = ((Color)(ref val)).ToHexNoAlpha();
		string messageString = "shared-solution-container-component-on-examine-main-text";
		using (args.PushGroup("ExaminableSolutionComponent"))
		{
			args.PushMarkup(base.Loc.GetString(messageString, new(string, object)[3]
			{
				("color", colorHex),
				("wordedAmount", base.Loc.GetString((solution.Contents.Count == 1) ? "shared-solution-container-component-on-examine-worded-amount-one-reagent" : "shared-solution-container-component-on-examine-worded-amount-multiple-reagents")),
				("desc", primary.LocalizedPhysicalDescription)
			}));
			IOrderedEnumerable<KeyValuePair<ReagentPrototype, FixedPoint2>> orderedEnumerable = from pair in solution.GetReagentPrototypes(PrototypeManager)
				orderby pair.Value.Value descending, pair.Key.LocalizedName
				select pair;
			List<ReagentPrototype> recognized = new List<ReagentPrototype>();
			foreach (KeyValuePair<ReagentPrototype, FixedPoint2> item in orderedEnumerable)
			{
				ReagentPrototype proto = item.Key;
				if (proto.Recognizable)
				{
					recognized.Add(proto);
				}
			}
			if (recognized.Count == 0)
			{
				return;
			}
			StringBuilder msg = new StringBuilder();
			foreach (ReagentPrototype reagent in recognized)
			{
				string part;
				if (reagent == recognized[0])
				{
					part = "examinable-solution-recognized-first";
				}
				else if (reagent == recognized[recognized.Count - 1])
				{
					msg.Append(' ');
					part = "examinable-solution-recognized-last";
				}
				else
				{
					part = "examinable-solution-recognized-next";
				}
				ILocalizationManager loc = base.Loc;
				string text = part;
				val = reagent.SubstanceColor;
				msg.Append(loc.GetString(text, (ValueTuple<string, object>)("color", ((Color)(ref val)).ToHexNoAlpha()), (ValueTuple<string, object>)("chemical", reagent.LocalizedName)));
			}
			args.PushMarkup(base.Loc.GetString("examinable-solution-has-recognizable-chemicals", (ValueTuple<string, object>)("recognizedString", msg.ToString())));
		}
	}

	private void OnSolutionExaminableVerb(Entity<ExaminableSolutionComponent> entity, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		if (!args.CanInteract || !args.CanAccess)
		{
			return;
		}
		SolutionScanEvent scanEvent = new SolutionScanEvent();
		((EntitySystem)this).RaiseLocalEvent<SolutionScanEvent>(args.User, scanEvent, false);
		if (scanEvent.CanScan && TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(args.Target), entity.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solutionHolder) && CanSeeHiddenSolution(entity, args.User))
		{
			EntityUid target = args.Target;
			EntityUid user = args.User;
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate
				{
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					FormattedMessage solutionExamine = GetSolutionExamine(solutionHolder);
					ExamineSystem.SendExamineTooltip(user, target, solutionExamine, getVerbs: false, centerAtCursor: false);
				},
				Text = base.Loc.GetString("scannable-solution-verb-text"),
				Message = base.Loc.GetString("scannable-solution-verb-message"),
				Category = VerbCategory.Examine,
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/drink.svg.192dpi.png"))
			};
			args.Verbs.Add(verb);
		}
	}

	private FormattedMessage GetSolutionExamine(Solution solution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage msg = new FormattedMessage();
		if (solution.Volume == 0)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("scannable-solution-empty-container"));
			return msg;
		}
		msg.AddMarkupOrThrow(base.Loc.GetString("scannable-solution-main-text"));
		foreach (var (proto, quantity) in from pair in solution.GetReagentPrototypes(PrototypeManager)
			orderby pair.Value.Value descending, pair.Key.LocalizedName
			select pair)
		{
			msg.PushNewline();
			ILocalizationManager loc = base.Loc;
			(string, object)[] obj = new(string, object)[3]
			{
				("type", proto.LocalizedName),
				default((string, object)),
				default((string, object))
			};
			Color substanceColor = proto.SubstanceColor;
			obj[1] = ("color", ((Color)(ref substanceColor)).ToHexNoAlpha());
			obj[2] = ("amount", quantity);
			msg.AddMarkupOrThrow(loc.GetString("scannable-solution-chemical", obj));
		}
		msg.PushNewline();
		msg.AddMarkupOrThrow(base.Loc.GetString("scannable-solution-temperature", (ValueTuple<string, object>)("temperature", Math.Round(solution.Temperature))));
		return msg;
	}

	private bool CanSeeHiddenSolution(Entity<ExaminableSolutionComponent> entity, EntityUid examiner)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.HeldOnly)
		{
			return true;
		}
		string inHand;
		return Hands.IsHolding(Entity<HandsComponent>.op_Implicit(examiner), Entity<ExaminableSolutionComponent>.op_Implicit(entity), out inHand);
	}

	private void OnMapInit(Entity<SolutionContainerManagerComponent> entity, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		EnsureAllSolutions(entity);
	}

	private void OnContainerManagerShutdown(Entity<SolutionContainerManagerComponent> entity, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer solutionContainer = default(BaseContainer);
		foreach (string name in entity.Comp.Containers)
		{
			if (ContainerSystem.TryGetContainer(Entity<SolutionContainerManagerComponent>.op_Implicit(entity), "solution@" + name, ref solutionContainer, (ContainerManagerComponent)null))
			{
				ContainerSystem.ShutdownContainer(solutionContainer);
			}
		}
		entity.Comp.Containers.Clear();
	}

	private void OnContainedSolutionShutdown(Entity<ContainedSolutionComponent> entity, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		SolutionContainerManagerComponent container = default(SolutionContainerManagerComponent);
		if (((EntitySystem)this).TryComp<SolutionContainerManagerComponent>(entity.Comp.Container, ref container))
		{
			container.Containers.Remove(entity.Comp.ContainerName);
			((EntitySystem)this).Dirty(entity.Comp.Container, (IComponent)(object)container, (MetaDataComponent)null);
		}
		BaseContainer solutionContainer = default(BaseContainer);
		if (ContainerSystem.TryGetContainer(Entity<ContainedSolutionComponent>.op_Implicit(entity), "solution@" + entity.Comp.ContainerName, ref solutionContainer, (ContainerManagerComponent)null))
		{
			ContainerSystem.ShutdownContainer(solutionContainer);
		}
	}

	public bool EnsureSolution(Entity<MetaDataComponent?> entity, string name, [NotNullWhen(true)] out Solution? solution, FixedPoint2 maxVol = default(FixedPoint2))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		bool existed;
		return EnsureSolution(entity, name, maxVol, null, out existed, out solution);
	}

	public bool EnsureSolution(Entity<MetaDataComponent?> entity, string name, out bool existed, [NotNullWhen(true)] out Solution? solution, FixedPoint2 maxVol = default(FixedPoint2))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return EnsureSolution(entity, name, maxVol, null, out existed, out solution);
	}

	public bool EnsureSolution(Entity<MetaDataComponent?> entity, string name, FixedPoint2 maxVol, Solution? prototype, out bool existed, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		solution = null;
		existed = false;
		Entity<MetaDataComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		MetaDataComponent val3 = default(MetaDataComponent);
		val.Deconstruct(ref val2, ref val3);
		EntityUid uid = val2;
		MetaDataComponent meta = val3;
		if (!((EntitySystem)this).Resolve(uid, ref meta, true))
		{
			throw new InvalidOperationException("Attempted to ensure solution on invalid entity.");
		}
		SolutionContainerManagerComponent manager = ((EntitySystem)this).EnsureComp<SolutionContainerManagerComponent>(uid);
		if ((int)meta.EntityLifeStage >= 3)
		{
			EnsureSolutionEntity(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, manager)), name, out existed, out Entity<SolutionComponent>? solEnt, maxVol, prototype);
			solution = solEnt.Value.Comp.Solution;
			return true;
		}
		solution = EnsureSolutionPrototype(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, manager)), name, maxVol, prototype, out existed);
		return true;
	}

	public void EnsureAllSolutions(Entity<SolutionContainerManagerComponent> entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (NetManager.IsClient)
		{
			return;
		}
		Dictionary<string, Solution> prototypes = entity.Comp.Solutions;
		if (prototypes == null)
		{
			return;
		}
		foreach (var (name, prototype) in prototypes)
		{
			EnsureSolutionEntity(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp)), name, out bool _, out Entity<SolutionComponent>? _, prototype.MaxVolume, prototype);
		}
		entity.Comp.Solutions = null;
		((EntitySystem)this).Dirty<SolutionContainerManagerComponent>(entity, (MetaDataComponent)null);
	}

	public bool EnsureSolutionEntity(Entity<SolutionContainerManagerComponent?> entity, string name, [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEntity, FixedPoint2 maxVol = default(FixedPoint2))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		bool existed;
		return EnsureSolutionEntity(entity, name, out existed, out solutionEntity, maxVol);
	}

	public bool EnsureSolutionEntity(Entity<SolutionContainerManagerComponent?> entity, string name, out bool existed, [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEntity, FixedPoint2 maxVol = default(FixedPoint2), Solution? prototype = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		existed = true;
		solutionEntity = null;
		Entity<SolutionContainerManagerComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		SolutionContainerManagerComponent solutionContainerManagerComponent = default(SolutionContainerManagerComponent);
		val.Deconstruct(ref val2, ref solutionContainerManagerComponent);
		EntityUid uid = val2;
		SolutionContainerManagerComponent container = solutionContainerManagerComponent;
		ContainerSlot solutionSlot = ContainerSystem.EnsureContainer<ContainerSlot>(uid, "solution@" + name, ref existed, (ContainerManagerComponent)null);
		if (!((EntitySystem)this).Resolve<SolutionContainerManagerComponent>(uid, ref container, false))
		{
			existed = false;
			container = ((EntitySystem)this).AddComp<SolutionContainerManagerComponent>(uid);
			container.Containers.Add(name);
			if (NetManager.IsClient)
			{
				return false;
			}
		}
		else if (!existed)
		{
			container.Containers.Add(name);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)container, (MetaDataComponent)null);
		}
		bool needsInit = false;
		EntityUid? containedEntity = solutionSlot.ContainedEntity;
		EntityUid solutionId;
		SolutionComponent solutionComp;
		if (containedEntity.HasValue)
		{
			solutionId = containedEntity.GetValueOrDefault();
			solutionComp = ((EntitySystem)this).Comp<SolutionComponent>(solutionId);
			Solution solution = solutionComp.Solution;
			solution.MaxVolume = FixedPoint2.Max(solution.MaxVolume, maxVol);
			if (prototype != null && prototype.Volume.Value > 0)
			{
				solution.AddSolution(prototype, PrototypeManager);
			}
			((EntitySystem)this).Dirty(solutionId, (IComponent)(object)solutionComp, (MetaDataComponent)null);
		}
		else
		{
			if (NetManager.IsClient)
			{
				return false;
			}
			if (prototype == null)
			{
				prototype = new Solution
				{
					MaxVolume = maxVol
				};
			}
			prototype.Name = name;
			SolutionComponent solutionComponent = default(SolutionComponent);
			ContainedSolutionComponent containedSolutionComponent = default(ContainedSolutionComponent);
			SpawnSolutionUninitialized(solutionSlot, name, maxVol, prototype).Deconstruct(ref val2, ref solutionComponent, ref containedSolutionComponent);
			solutionId = val2;
			solutionComp = solutionComponent;
			existed = false;
			needsInit = true;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)container, (MetaDataComponent)null);
		}
		if (needsInit)
		{
			base.EntityManager.InitializeAndStartEntity(solutionId, (MapId?)((EntitySystem)this).Transform(solutionId).MapID);
		}
		solutionEntity = Entity<SolutionComponent>.op_Implicit((solutionId, solutionComp));
		return true;
	}

	private Solution EnsureSolutionPrototype(Entity<SolutionContainerManagerComponent?> entity, string name, FixedPoint2 maxVol, Solution? prototype, out bool existed)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		existed = true;
		Entity<SolutionContainerManagerComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		SolutionContainerManagerComponent solutionContainerManagerComponent = default(SolutionContainerManagerComponent);
		val.Deconstruct(ref val2, ref solutionContainerManagerComponent);
		EntityUid uid = val2;
		SolutionContainerManagerComponent container = solutionContainerManagerComponent;
		if (!((EntitySystem)this).Resolve<SolutionContainerManagerComponent>(uid, ref container, false))
		{
			container = ((EntitySystem)this).AddComp<SolutionContainerManagerComponent>(uid);
			existed = false;
		}
		if (container.Solutions == null)
		{
			container.Solutions = new Dictionary<string, Solution>(2);
		}
		if (!container.Solutions.TryGetValue(name, out Solution solution))
		{
			solution = prototype ?? new Solution
			{
				Name = name,
				MaxVolume = maxVol
			};
			container.Solutions.Add(name, solution);
			existed = false;
		}
		else
		{
			solution.MaxVolume = FixedPoint2.Max(solution.MaxVolume, maxVol);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)container, (MetaDataComponent)null);
		return solution;
	}

	private Entity<SolutionComponent, ContainedSolutionComponent> SpawnSolutionUninitialized(ContainerSlot container, string name, FixedPoint2 maxVol, Solution prototype)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(((BaseContainer)container).Owner, Vector2.Zero);
		EntityUid uid = base.EntityManager.CreateEntityUninitialized((string)null, coords, (ComponentRegistry)null, default(Angle));
		SolutionComponent solution = new SolutionComponent
		{
			Solution = prototype
		};
		((EntitySystem)this).AddComp<SolutionComponent>(uid, solution, false);
		ContainedSolutionComponent relation = new ContainedSolutionComponent
		{
			Container = ((BaseContainer)container).Owner,
			ContainerName = name
		};
		((EntitySystem)this).AddComp<ContainedSolutionComponent>(uid, relation, false);
		MetaDataSys.SetEntityName(uid, "solution - " + name, (MetaDataComponent)null, true);
		ContainerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), (BaseContainer)(object)container, (TransformComponent)null, true);
		return Entity<SolutionComponent, ContainedSolutionComponent>.op_Implicit((uid, solution, relation));
	}

	public void AdjustDissolvedReagent(Entity<SolutionComponent> dissolvedSolution, FixedPoint2 volume, ReagentId reagent, float concentrationChange)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (concentrationChange != 0f)
		{
			Solution dissolvedSol = dissolvedSolution.Comp.Solution;
			FixedPoint2 amtChange = GetReagentQuantityFromConcentration(dissolvedSolution, volume, MathF.Abs(concentrationChange));
			if (concentrationChange > 0f)
			{
				dissolvedSol.AddReagent(reagent, amtChange);
			}
			else
			{
				dissolvedSol.RemoveReagent(reagent, amtChange);
			}
			UpdateChemicals(dissolvedSolution);
		}
	}

	public FixedPoint2 GetReagentQuantityFromConcentration(Entity<SolutionComponent> dissolvedSolution, FixedPoint2 volume, float concentration)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Solution dissolvedSol = dissolvedSolution.Comp.Solution;
		if (volume == 0 || dissolvedSol.Volume == 0)
		{
			return 0;
		}
		return concentration * volume;
	}

	public float GetReagentConcentration(Entity<SolutionComponent> dissolvedSolution, FixedPoint2 volume, ReagentId dissolvedReagent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Solution dissolvedSol = dissolvedSolution.Comp.Solution;
		if (volume == 0 || dissolvedSol.Volume == 0 || !dissolvedSol.TryGetReagentQuantity(dissolvedReagent, out var dissolvedVol))
		{
			return 0f;
		}
		return (float)dissolvedVol / volume.Float();
	}

	public FixedPoint2 ClampReagentAmountByConcentration(Entity<SolutionComponent> dissolvedSolution, FixedPoint2 volume, ReagentId dissolvedReagent, FixedPoint2 dissolvedReagentAmount, float maxConcentration = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Solution dissolvedSol = dissolvedSolution.Comp.Solution;
		if (volume == 0 || dissolvedSol.Volume == 0 || !dissolvedSol.TryGetReagentQuantity(dissolvedReagent, out var dissolvedVol))
		{
			return 0;
		}
		volume *= maxConcentration;
		dissolvedVol += dissolvedReagentAmount;
		FixedPoint2 overflow = volume - dissolvedVol;
		if (overflow < 0)
		{
			dissolvedReagentAmount += overflow;
		}
		return dissolvedReagentAmount;
	}

	protected void InitializeRelays()
	{
		((EntitySystem)this).SubscribeLocalEvent<ContainedSolutionComponent, SolutionChangedEvent>((EntityEventRefHandler<ContainedSolutionComponent, SolutionChangedEvent>)OnSolutionChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainedSolutionComponent, SolutionOverflowEvent>((EntityEventRefHandler<ContainedSolutionComponent, SolutionOverflowEvent>)OnSolutionOverflow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainedSolutionComponent, ReactionAttemptEvent>((EntityEventRefHandler<ContainedSolutionComponent, ReactionAttemptEvent>)RelaySolutionRefEvent, (Type[])null, (Type[])null);
	}

	protected virtual void OnSolutionChanged(Entity<ContainedSolutionComponent> entity, ref SolutionChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		args.Solution.Deconstruct(ref val, ref solutionComponent);
		EntityUid solutionId = val;
		SolutionComponent solutionComp = solutionComponent;
		Solution solution = solutionComp.Solution;
		UpdateAppearance(Entity<AppearanceComponent>.op_Implicit(entity.Comp.Container), Entity<SolutionComponent, ContainedSolutionComponent>.op_Implicit((solutionId, solutionComp, entity.Comp)));
		SolutionContainerChangedEvent relayEvent = new SolutionContainerChangedEvent(solution, entity.Comp.ContainerName);
		((EntitySystem)this).RaiseLocalEvent<SolutionContainerChangedEvent>(entity.Comp.Container, ref relayEvent, false);
	}

	protected virtual void OnSolutionOverflow(Entity<ContainedSolutionComponent> entity, ref SolutionOverflowEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Solution solution = args.Solution.Comp.Solution;
		Solution overflow = solution.SplitSolution(args.Overflow);
		SolutionContainerOverflowEvent solutionContainerOverflowEvent = new SolutionContainerOverflowEvent(entity.Owner, solution, overflow);
		solutionContainerOverflowEvent.Handled = args.Handled;
		SolutionContainerOverflowEvent relayEv = solutionContainerOverflowEvent;
		((EntitySystem)this).RaiseLocalEvent<SolutionContainerOverflowEvent>(entity.Comp.Container, ref relayEv, false);
		args.Handled = relayEv.Handled;
	}

	private void RelaySolutionValEvent<TEvent>(EntityUid uid, ContainedSolutionComponent comp, TEvent @event)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		SolutionRelayEvent<TEvent> relayEvent = new SolutionRelayEvent<TEvent>(@event, uid, comp.ContainerName);
		((EntitySystem)this).RaiseLocalEvent<SolutionRelayEvent<TEvent>>(comp.Container, ref relayEvent, false);
	}

	private void RelaySolutionRefEvent<TEvent>(Entity<ContainedSolutionComponent> entity, ref TEvent @event)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		SolutionRelayEvent<TEvent> relayEvent = new SolutionRelayEvent<TEvent>(@event, entity.Owner, entity.Comp.ContainerName);
		((EntitySystem)this).RaiseLocalEvent<SolutionRelayEvent<TEvent>>(entity.Comp.Container, ref relayEvent, false);
		@event = relayEvent.Event;
	}

	private void RelaySolutionContainerEvent<TEvent>(EntityUid uid, SolutionContainerManagerComponent comp, TEvent @event)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		foreach (var item in EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, comp))))
		{
			string name = item.Name;
			Entity<SolutionComponent> soln = item.Solution;
			SolutionContainerRelayEvent<TEvent> relayEvent = new SolutionContainerRelayEvent<TEvent>(@event, soln, name);
			((EntitySystem)this).RaiseLocalEvent<SolutionContainerRelayEvent<TEvent>>(Entity<SolutionComponent>.op_Implicit(soln), ref relayEvent, false);
		}
	}

	private void RelaySolutionContainerEvent<TEvent>(Entity<SolutionContainerManagerComponent> entity, ref TEvent @event)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		foreach (var item in EnumerateSolutions(Entity<SolutionContainerManagerComponent>.op_Implicit((entity.Owner, entity.Comp))))
		{
			string name = item.Name;
			Entity<SolutionComponent> soln = item.Solution;
			SolutionContainerRelayEvent<TEvent> relayEvent = new SolutionContainerRelayEvent<TEvent>(@event, soln, name);
			((EntitySystem)this).RaiseLocalEvent<SolutionContainerRelayEvent<TEvent>>(Entity<SolutionComponent>.op_Implicit(soln), ref relayEvent, false);
			@event = relayEvent.Event;
		}
	}
}
