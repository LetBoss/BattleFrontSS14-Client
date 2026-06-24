using System;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Destructible.Thresholds;

[Serializable]
[DataDefinition]
public struct MinMax : ISerializationGenerated<MinMax>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Min;

	[DataField(null, false, 1, false, false, null)]
	public int Max;

	public MinMax(int min, int max)
	{
		Min = min;
		Max = max;
	}

	public readonly int Next(IRobustRandom random)
	{
		return random.Next(Min, Max + 1);
	}

	public readonly int Next(System.Random random)
	{
		return random.Next(Min, Max + 1);
	}

	public MinMax()
	{
		Min = 0;
		Max = 0;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MinMax target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<MinMax>(this, ref target, hookCtx, false, context))
		{
			int MinTemp = 0;
			if (!serialization.TryCustomCopy<int>(Min, ref MinTemp, hookCtx, false, context))
			{
				MinTemp = Min;
			}
			int MaxTemp = 0;
			if (!serialization.TryCustomCopy<int>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = Max;
			}
			MinMax minMax = target;
			minMax.Min = MinTemp;
			minMax.Max = MaxTemp;
			target = minMax;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MinMax target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MinMax cast = (MinMax)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MinMax Instantiate()
	{
		return new MinMax();
	}
}
