using System;
using System.Collections.Generic;
using Content.Shared.EntityTable.Conditions;
using Content.Shared.EntityTable.ValueSelector;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityTable.EntitySelectors;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityTableSelector : ISerializationGenerated<EntityTableSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public NumberSelector Rolls = new ConstantNumberSelector(1);

	[DataField(null, false, 1, false, false, null)]
	public float Weight = 1f;

	[DataField(null, false, 1, false, false, null)]
	public double Prob = 1.0;

	[DataField(null, false, 1, false, false, null)]
	public List<EntityTableCondition> Conditions = new List<EntityTableCondition>();

	[DataField(null, false, 1, false, false, null)]
	public bool RequireAll = true;

	public IEnumerable<EntProtoId> GetSpawns(System.Random rand, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		if (!CheckConditions(entMan, proto, ctx))
		{
			yield break;
		}
		int rolls = Rolls.Get(rand);
		for (int i = 0; i < rolls; i++)
		{
			if (!(rand.NextDouble() < Prob))
			{
				continue;
			}
			foreach (EntProtoId item in GetSpawnsImplementation(rand, entMan, proto, ctx))
			{
				yield return item;
			}
		}
	}

	public bool CheckConditions(IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		if (Conditions.Count == 0)
		{
			return true;
		}
		bool success = false;
		foreach (EntityTableCondition condition in Conditions)
		{
			bool res = condition.Evaluate(this, entMan, proto, ctx);
			if (RequireAll && !res)
			{
				return false;
			}
			success = success || res;
		}
		if (RequireAll)
		{
			return true;
		}
		return success;
	}

	protected abstract IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx);

	public EntityTableSelector()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EntityTableSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<EntityTableSelector>(this, ref target, hookCtx, false, context))
		{
			NumberSelector RollsTemp = null;
			if (Rolls == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<NumberSelector>(Rolls, ref RollsTemp, hookCtx, true, context))
			{
				RollsTemp = serialization.CreateCopy<NumberSelector>(Rolls, hookCtx, context, false);
			}
			target.Rolls = RollsTemp;
			float WeightTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Weight, ref WeightTemp, hookCtx, false, context))
			{
				WeightTemp = Weight;
			}
			target.Weight = WeightTemp;
			double ProbTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(Prob, ref ProbTemp, hookCtx, false, context))
			{
				ProbTemp = Prob;
			}
			target.Prob = ProbTemp;
			List<EntityTableCondition> ConditionsTemp = null;
			if (Conditions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntityTableCondition>>(Conditions, ref ConditionsTemp, hookCtx, true, context))
			{
				ConditionsTemp = serialization.CreateCopy<List<EntityTableCondition>>(Conditions, hookCtx, context, false);
			}
			target.Conditions = ConditionsTemp;
			bool RequireAllTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireAll, ref RequireAllTemp, hookCtx, false, context))
			{
				RequireAllTemp = RequireAll;
			}
			target.RequireAll = RequireAllTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EntityTableSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTableSelector cast = (EntityTableSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual EntityTableSelector Instantiate()
	{
		throw new NotImplementedException();
	}
}
