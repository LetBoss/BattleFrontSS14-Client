using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Chemistry.Reaction;

public sealed class ChemicalReactionSystem : EntitySystem
{
	public static readonly ProtoId<ReactionPrototype> FoamReaction = ProtoId<ReactionPrototype>.op_Implicit("Foam");

	private const int MaxReactionIterations = 20;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	private FrozenDictionary<string, List<ReactionPrototype>> _reactionsSingle;

	private FrozenDictionary<string, List<ReactionPrototype>> _reactions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeReactionCache();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
	}

	private void InitializeReactionCache()
	{
		Dictionary<string, List<ReactionPrototype>> dict = new Dictionary<string, List<ReactionPrototype>>();
		foreach (ReactionPrototype reaction in _prototypeManager.EnumeratePrototypes<ReactionPrototype>())
		{
			string reagent = reaction.Reactants.Keys.First();
			Extensions.GetOrNew<string, List<ReactionPrototype>>(dict, reagent).Add(reaction);
		}
		_reactionsSingle = dict.ToFrozenDictionary();
		dict.Clear();
		foreach (ReactionPrototype reaction2 in _prototypeManager.EnumeratePrototypes<ReactionPrototype>())
		{
			foreach (string reagent2 in reaction2.Reactants.Keys)
			{
				Extensions.GetOrNew<string, List<ReactionPrototype>>(dict, reagent2).Add(reaction2);
			}
		}
		_reactions = dict.ToFrozenDictionary();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs eventArgs)
	{
		if (eventArgs.WasModified<ReactionPrototype>())
		{
			InitializeReactionCache();
		}
	}

	private bool CanReact(Entity<SolutionComponent> soln, ReactionPrototype reaction, ReactionMixerComponent? mixerComponent, out FixedPoint2 lowestUnitReactions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Solution solution = soln.Comp.Solution;
		lowestUnitReactions = FixedPoint2.MaxValue;
		if (solution.Temperature < reaction.MinimumTemperature)
		{
			lowestUnitReactions = FixedPoint2.Zero;
			return false;
		}
		if (solution.Temperature > reaction.MaximumTemperature)
		{
			lowestUnitReactions = FixedPoint2.Zero;
			return false;
		}
		if ((mixerComponent == null && reaction.MixingCategories != null) || (mixerComponent != null && reaction.MixingCategories != null && reaction.MixingCategories.Except(mixerComponent.ReactionTypes).Any()))
		{
			lowestUnitReactions = FixedPoint2.Zero;
			return false;
		}
		ReactionAttemptEvent attempt = new ReactionAttemptEvent(reaction, soln);
		((EntitySystem)this).RaiseLocalEvent<ReactionAttemptEvent>(Entity<SolutionComponent>.op_Implicit(soln), ref attempt, false);
		if (attempt.Cancelled)
		{
			lowestUnitReactions = FixedPoint2.Zero;
			return false;
		}
		foreach (KeyValuePair<string, ReactantPrototype> reactantData in reaction.Reactants)
		{
			string reactantName = reactantData.Key;
			FixedPoint2 reactantCoefficient = reactantData.Value.Amount;
			FixedPoint2 reactantQuantity = solution.GetTotalPrototypeQuantity(reactantName);
			if (reactantQuantity <= FixedPoint2.Zero)
			{
				return false;
			}
			if (reactantData.Value.Catalyst)
			{
				if (reactantQuantity == FixedPoint2.Zero || (reaction.Quantized && reactantQuantity < reactantCoefficient))
				{
					return false;
				}
				continue;
			}
			FixedPoint2 unitReactions = reactantQuantity / reactantCoefficient;
			if (unitReactions < lowestUnitReactions)
			{
				lowestUnitReactions = unitReactions;
			}
		}
		if (reaction.Quantized)
		{
			lowestUnitReactions = (int)lowestUnitReactions;
		}
		return lowestUnitReactions > 0;
	}

	private List<string> PerformReaction(Entity<SolutionComponent> soln, ReactionPrototype reaction, FixedPoint2 unitReactions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent> val = soln;
		EntityUid val2 = default(EntityUid);
		SolutionComponent solutionComponent = default(SolutionComponent);
		val.Deconstruct(ref val2, ref solutionComponent);
		Solution solution = solutionComponent.Solution;
		float energy = (reaction.ConserveEnergy ? solution.GetThermalEnergy(_prototypeManager) : 0f);
		foreach (KeyValuePair<string, ReactantPrototype> reactant in reaction.Reactants)
		{
			if (!reactant.Value.Catalyst)
			{
				FixedPoint2 amountToRemove = unitReactions * reactant.Value.Amount;
				solution.RemoveReagent(reactant.Key, amountToRemove, null, ignoreReagentData: true);
			}
		}
		List<string> products = new List<string>();
		foreach (KeyValuePair<string, FixedPoint2> product in reaction.Products)
		{
			products.Add(product.Key);
			solution.AddReagent(product.Key, product.Value * unitReactions);
		}
		if (reaction.ConserveEnergy)
		{
			float newCap = solution.GetHeatCapacity(_prototypeManager);
			if (newCap > 0f)
			{
				solution.Temperature = energy / newCap;
			}
		}
		OnReaction(soln, reaction, null, unitReactions);
		return products;
	}

	private void OnReaction(Entity<SolutionComponent> soln, ReactionPrototype reaction, ReagentPrototype? reagent, FixedPoint2 unitReactions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		EntityEffectReagentArgs args = new EntityEffectReagentArgs(Entity<SolutionComponent>.op_Implicit(soln), (IEntityManager)(object)base.EntityManager, null, soln.Comp.Solution, unitReactions, reagent, null, 1f);
		EntityCoordinates? gridPos = default(EntityCoordinates?);
		bool posFound = _transformSystem.TryGetMapOrGridCoordinates(Entity<SolutionComponent>.op_Implicit(soln), ref gridPos, (TransformComponent)null);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogImpact impact = reaction.Impact;
		LogStringHandler handler = new LogStringHandler(61, 4);
		handler.AppendLiteral("Chemical reaction ");
		handler.AppendFormatted(reaction.ID, 0, "reaction");
		handler.AppendLiteral(" occurred with strength ");
		handler.AppendFormatted(unitReactions, "strength", "unitReactions");
		handler.AppendLiteral(" on entity ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<SolutionComponent>.op_Implicit(soln), (MetaDataComponent)null), "metabolizer", "ToPrettyString(soln)");
		handler.AppendLiteral(" at Pos:");
		handler.AppendFormatted(posFound ? $"{gridPos:coordinates}" : "[Grid or Map not Found]");
		adminLogger.Add(LogType.ChemicalReaction, impact, ref handler);
		foreach (EntityEffect effect in reaction.Effects)
		{
			if (effect.ShouldApply(args))
			{
				if (effect.ShouldLog)
				{
					EntityUid entity = args.TargetEntity;
					ISharedAdminLogManager adminLogger2 = _adminLogger;
					LogImpact logImpact = effect.LogImpact;
					LogStringHandler handler2 = new LogStringHandler(56, 4);
					handler2.AppendLiteral("Reaction effect ");
					handler2.AppendFormatted(effect.GetType().Name, 0, "effect");
					handler2.AppendLiteral(" of reaction ");
					handler2.AppendFormatted(reaction.ID, 0, "reaction");
					handler2.AppendLiteral(" applied on entity ");
					handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity)), "entity", "ToPrettyString(entity)");
					handler2.AppendLiteral(" at Pos:");
					handler2.AppendFormatted(posFound ? $"{gridPos:coordinates}" : "[Grid or Map not Found");
					adminLogger2.Add(LogType.ReagentEffect, logImpact, ref handler2);
				}
				effect.Effect(args);
			}
		}
		_audio.PlayPvs(reaction.Sound, Entity<SolutionComponent>.op_Implicit(soln), (AudioParams?)null);
	}

	private bool ProcessReactions(Entity<SolutionComponent> soln, SortedSet<ReactionPrototype> reactions, ReactionMixerComponent? mixerComponent)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ReactionPrototype> toRemove = new HashSet<ReactionPrototype>();
		List<string> products = null;
		foreach (ReactionPrototype reaction in reactions)
		{
			if (!CanReact(soln, reaction, mixerComponent, out var unitReactions))
			{
				toRemove.Add(reaction);
				continue;
			}
			products = PerformReaction(soln, reaction, unitReactions);
			break;
		}
		if (products == null)
		{
			return false;
		}
		if (products.Count == 0)
		{
			return true;
		}
		foreach (string product in products)
		{
			if (_reactions.TryGetValue(product, out List<ReactionPrototype> reactantReactions))
			{
				reactions.UnionWith(reactantReactions);
			}
		}
		return true;
	}

	public void FullyReactSolution(Entity<SolutionComponent> soln, ReactionMixerComponent? mixerComponent = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		SortedSet<ReactionPrototype> reactions = new SortedSet<ReactionPrototype>();
		foreach (ReagentQuantity reactant in soln.Comp.Solution.Contents)
		{
			if (_reactionsSingle.TryGetValue(reactant.Reagent.Prototype, out List<ReactionPrototype> reactantReactions))
			{
				reactions.UnionWith(reactantReactions);
			}
		}
		for (int i = 0; i < 20; i++)
		{
			if (!ProcessReactions(soln, reactions, mixerComponent))
			{
				return;
			}
		}
		((EntitySystem)this).Log.Error($"{"Solution"} {soln.Owner} could not finish reacting in under {20} loops.");
	}
}
