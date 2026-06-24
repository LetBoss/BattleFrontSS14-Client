using System.Collections.Generic;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Chemistry;

public sealed class ReactiveSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private RMCReagentSystem _reagents;

	[Dependency]
	private IRobustRandom _robustRandom;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	public void DoEntityReaction(EntityUid uid, Solution solution, ReactionMethod method)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ReagentQuantity[] array = solution.Contents.ToArray();
		foreach (ReagentQuantity reagent in array)
		{
			ReactionEntity(uid, method, reagent, solution);
		}
	}

	public void ReactionEntity(EntityUid uid, ReactionMethod method, ReagentQuantity reagentQuantity, Solution? source)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Content.Shared._RMC14.Chemistry.Reagent.Reagent proto = _reagents.Index(ProtoId<ReagentPrototype>.op_Implicit(reagentQuantity.Reagent.Prototype));
		ReactionEntity(uid, method, proto, reagentQuantity, source);
	}

	public void ReactionEntity(EntityUid uid, ReactionMethod method, ReagentPrototype proto, ReagentQuantity reagentQuantity, Solution? source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		ReactiveComponent reactive = default(ReactiveComponent);
		if (!((EntitySystem)this).TryComp<ReactiveComponent>(uid, ref reactive))
		{
			return;
		}
		ReactionEntityEvent ev = new ReactionEntityEvent(method, proto, reagentQuantity, source);
		((EntitySystem)this).RaiseLocalEvent<ReactionEntityEvent>(uid, ref ev, false);
		EntityEffectReagentArgs args = new EntityEffectReagentArgs(uid, (IEntityManager)(object)base.EntityManager, null, source, source?.GetReagentQuantity(reagentQuantity.Reagent) ?? reagentQuantity.Quantity, proto, method, 1f);
		if (proto.ReactiveEffects != null && reactive.ReactiveGroups != null)
		{
			foreach (var (key, val2) in proto.ReactiveEffects)
			{
				if (!val2.Methods.Contains(method) || !reactive.ReactiveGroups.ContainsKey(ProtoId<ReactiveGroupPrototype>.op_Implicit(key)) || !reactive.ReactiveGroups[ProtoId<ReactiveGroupPrototype>.op_Implicit(key)].Contains(method))
				{
					continue;
				}
				EntityEffect[] effects = val2.Effects;
				foreach (EntityEffect effect in effects)
				{
					if (effect.ShouldApply(args, _robustRandom))
					{
						if (effect.ShouldLog)
						{
							EntityUid entity = args.TargetEntity;
							ISharedAdminLogManager adminLogger = _adminLogger;
							LogImpact logImpact = effect.LogImpact;
							LogStringHandler handler = new LogStringHandler(64, 5);
							handler.AppendLiteral("Reactive effect ");
							handler.AppendFormatted(effect.GetType().Name, 0, "effect");
							handler.AppendLiteral(" of reagent ");
							handler.AppendFormatted(proto.ID, 0, "reagent");
							handler.AppendLiteral(" with method ");
							handler.AppendFormatted(method, "method");
							handler.AppendLiteral(" applied on entity ");
							handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity)), "entity", "ToPrettyString(entity)");
							handler.AppendLiteral(" at ");
							handler.AppendFormatted<EntityCoordinates>(((EntitySystem)this).Transform(entity).Coordinates, "coordinates", "Transform(entity).Coordinates");
							adminLogger.Add(LogType.ReagentEffect, logImpact, ref handler);
						}
						effect.Effect(args);
					}
				}
			}
		}
		if (reactive.Reactions == null)
		{
			return;
		}
		foreach (Content.Shared.Chemistry.Reaction.ReactiveReagentEffectEntry entry in reactive.Reactions)
		{
			if (!entry.Methods.Contains(method) || (entry.Reagents != null && !entry.Reagents.Contains(proto.ID)))
			{
				continue;
			}
			foreach (EntityEffect effect2 in entry.Effects)
			{
				if (effect2.ShouldApply(args, _robustRandom))
				{
					if (effect2.ShouldLog)
					{
						EntityUid entity2 = args.TargetEntity;
						ISharedAdminLogManager adminLogger2 = _adminLogger;
						LogImpact logImpact2 = effect2.LogImpact;
						LogStringHandler handler2 = new LogStringHandler(52, 5);
						handler2.AppendLiteral("Reactive effect ");
						handler2.AppendFormatted(effect2.GetType().Name, 0, "effect");
						handler2.AppendLiteral(" of ");
						handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity2)), "entity", "ToPrettyString(entity)");
						handler2.AppendLiteral(" using reagent ");
						handler2.AppendFormatted(proto.ID, 0, "reagent");
						handler2.AppendLiteral(" with method ");
						handler2.AppendFormatted(method, "method");
						handler2.AppendLiteral(" at ");
						handler2.AppendFormatted<EntityCoordinates>(((EntitySystem)this).Transform(entity2).Coordinates, "coordinates", "Transform(entity).Coordinates");
						adminLogger2.Add(LogType.ReagentEffect, logImpact2, ref handler2);
					}
					effect2.Effect(args);
				}
			}
		}
	}
}
