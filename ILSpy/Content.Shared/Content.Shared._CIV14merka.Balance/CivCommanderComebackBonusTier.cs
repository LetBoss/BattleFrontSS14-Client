using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Balance;

[DataDefinition]
public sealed class CivCommanderComebackBonusTier : ISerializationGenerated<CivCommanderComebackBonusTier>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int LosingPercent;

	[DataField(null, false, 1, true, false, null)]
	public int Currency;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivCommanderComebackBonusTier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<CivCommanderComebackBonusTier>(this, ref target, hookCtx, false, context))
		{
			int LosingPercentTemp = 0;
			if (!serialization.TryCustomCopy<int>(LosingPercent, ref LosingPercentTemp, hookCtx, false, context))
			{
				LosingPercentTemp = LosingPercent;
			}
			target.LosingPercent = LosingPercentTemp;
			int CurrencyTemp = 0;
			if (!serialization.TryCustomCopy<int>(Currency, ref CurrencyTemp, hookCtx, false, context))
			{
				CurrencyTemp = Currency;
			}
			target.Currency = CurrencyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivCommanderComebackBonusTier target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivCommanderComebackBonusTier cast = (CivCommanderComebackBonusTier)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CivCommanderComebackBonusTier Instantiate()
	{
		return new CivCommanderComebackBonusTier();
	}
}
