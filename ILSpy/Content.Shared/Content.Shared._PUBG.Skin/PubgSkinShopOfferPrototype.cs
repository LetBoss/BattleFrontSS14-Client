using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Skin;

[DataDefinition]
public sealed class PubgSkinShopOfferPrototype : ISerializationGenerated<PubgSkinShopOfferPrototype>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Currency = "premium";

	[DataField(null, false, 1, false, false, null)]
	public int Price;

	[DataField(null, false, 1, false, false, null)]
	public int? DurationDays;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgSkinShopOfferPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PubgSkinShopOfferPrototype>(this, ref target, hookCtx, false, context))
		{
			string CurrencyTemp = null;
			if (Currency == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Currency, ref CurrencyTemp, hookCtx, false, context))
			{
				CurrencyTemp = Currency;
			}
			target.Currency = CurrencyTemp;
			int PriceTemp = 0;
			if (!serialization.TryCustomCopy<int>(Price, ref PriceTemp, hookCtx, false, context))
			{
				PriceTemp = Price;
			}
			target.Price = PriceTemp;
			int? DurationDaysTemp = null;
			if (!serialization.TryCustomCopy<int?>(DurationDays, ref DurationDaysTemp, hookCtx, false, context))
			{
				DurationDaysTemp = DurationDays;
			}
			target.DurationDays = DurationDaysTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgSkinShopOfferPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSkinShopOfferPrototype cast = (PubgSkinShopOfferPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PubgSkinShopOfferPrototype Instantiate()
	{
		return new PubgSkinShopOfferPrototype();
	}
}
