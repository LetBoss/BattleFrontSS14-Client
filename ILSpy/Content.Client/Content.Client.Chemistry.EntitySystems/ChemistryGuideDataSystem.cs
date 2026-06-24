using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Body.Part;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Kitchen.Components;
using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class ChemistryGuideDataSystem : SharedChemistryGuideDataSystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	private static readonly ProtoId<MixingCategoryPrototype> DefaultMixingCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyMix");

	private static readonly ProtoId<MixingCategoryPrototype> DefaultGrindCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyGrind");

	private static readonly ProtoId<MixingCategoryPrototype> DefaultJuiceCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyJuice");

	private static readonly ProtoId<MixingCategoryPrototype> DefaultCondenseCategory = ProtoId<MixingCategoryPrototype>.op_Implicit("DummyCondense");

	private readonly Dictionary<string, List<ReagentSourceData>> _reagentSources = new Dictionary<string, List<ReagentSourceData>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<ReagentGuideRegistryChangedEvent>((EntityEventHandler<ReagentGuideRegistryChangedEvent>)OnReceiveRegistryUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		OnPrototypesReloaded(null);
	}

	private void OnReceiveRegistryUpdate(ReagentGuideRegistryChangedEvent message)
	{
		ReagentGuideChangeset changeset = message.Changeset;
		foreach (string item in changeset.Removed)
		{
			Registry.Remove(item);
		}
		foreach (var (key, value) in changeset.GuideEntries)
		{
			Registry[key] = value;
		}
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs? ev)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		_reagentSources.Clear();
		foreach (ReagentPrototype item5 in PrototypeManager.EnumeratePrototypes<ReagentPrototype>())
		{
			_reagentSources.Add(item5.ID, new List<ReagentSourceData>());
		}
		foreach (ReactionPrototype item6 in PrototypeManager.EnumeratePrototypes<ReactionPrototype>())
		{
			if (!item6.Source)
			{
				continue;
			}
			ReagentReactionSourceData item = new ReagentReactionSourceData(item6.MixingCategories ?? new List<ProtoId<MixingCategoryPrototype>> { DefaultMixingCategory }, item6);
			foreach (string key in item6.Products.Keys)
			{
				_reagentSources[key].Add(item);
			}
		}
		foreach (GasPrototype item7 in PrototypeManager.EnumeratePrototypes<GasPrototype>())
		{
			if (item7.Reagent != null)
			{
				ReagentGasSourceData item2 = new ReagentGasSourceData(new List<ProtoId<MixingCategoryPrototype>> { DefaultCondenseCategory }, item7);
				_reagentSources[item7.Reagent].Add(item2);
			}
		}
		List<string> list = new List<string>();
		ExtractableComponent extractableComponent = default(ExtractableComponent);
		SolutionContainerManagerComponent container = default(SolutionContainerManagerComponent);
		foreach (EntityPrototype item8 in PrototypeManager.EnumeratePrototypes<EntityPrototype>())
		{
			if (item8.Abstract || list.Contains(item8.Name) || !item8.TryGetComponent<ExtractableComponent>(ref extractableComponent, ((EntitySystem)this).EntityManager.ComponentFactory) || item8.HasComponent<BodyPartComponent>((IComponentFactory?)null) || item8.HasComponent<PillComponent>((IComponentFactory?)null))
			{
				continue;
			}
			Solution juiceSolution = extractableComponent.JuiceSolution;
			ReagentId id;
			FixedPoint2 quantity;
			if (juiceSolution != null)
			{
				ReagentEntitySourceData item3 = new ReagentEntitySourceData(new List<ProtoId<MixingCategoryPrototype>> { DefaultJuiceCategory }, item8, juiceSolution);
				foreach (ReagentQuantity content in juiceSolution.Contents)
				{
					content.Deconstruct(out id, out quantity);
					ReagentId reagentId = id;
					_reagentSources[reagentId.Prototype].Add(item3);
				}
				list.Add(item8.Name);
			}
			string grindableSolution = extractableComponent.GrindableSolution;
			if (grindableSolution == null || !item8.TryGetComponent<SolutionContainerManagerComponent>(ref container, ((EntitySystem)this).EntityManager.ComponentFactory) || !_solutionContainer.TryGetSolution(container, grindableSolution, out Solution solution))
			{
				continue;
			}
			ReagentEntitySourceData item4 = new ReagentEntitySourceData(new List<ProtoId<MixingCategoryPrototype>> { DefaultGrindCategory }, item8, solution);
			foreach (ReagentQuantity content2 in solution.Contents)
			{
				content2.Deconstruct(out id, out quantity);
				ReagentId reagentId2 = id;
				_reagentSources[reagentId2.Prototype].Add(item4);
			}
			list.Add(item8.Name);
		}
	}

	public List<ReagentSourceData> GetReagentSources(string id)
	{
		return _reagentSources.GetValueOrDefault(id) ?? new List<ReagentSourceData>();
	}

	public override void ReloadAllReagentPrototypes()
	{
	}
}
