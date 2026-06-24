using System;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable.ValueSelector;

public sealed class BinomialNumberSelector : NumberSelector, ISerializationGenerated<BinomialNumberSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Trials = 1;

	[DataField(null, false, 1, false, false, null)]
	public float Chance = 0.5f;

	public override int Get(System.Random rand)
	{
		IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
		int count = 0;
		for (int i = 0; i < Trials; i++)
		{
			if (RandomExtensions.Prob(random, Chance))
			{
				count++;
			}
		}
		return count;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BinomialNumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NumberSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BinomialNumberSelector)definitionCast;
		if (!serialization.TryCustomCopy<BinomialNumberSelector>(this, ref target, hookCtx, false, context))
		{
			int TrialsTemp = 0;
			if (!serialization.TryCustomCopy<int>(Trials, ref TrialsTemp, hookCtx, false, context))
			{
				TrialsTemp = Trials;
			}
			target.Trials = TrialsTemp;
			float ChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Chance, ref ChanceTemp, hookCtx, false, context))
			{
				ChanceTemp = Chance;
			}
			target.Chance = ChanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BinomialNumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref NumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BinomialNumberSelector cast = (BinomialNumberSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BinomialNumberSelector cast = (BinomialNumberSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BinomialNumberSelector Instantiate()
	{
		return new BinomialNumberSelector();
	}
}
