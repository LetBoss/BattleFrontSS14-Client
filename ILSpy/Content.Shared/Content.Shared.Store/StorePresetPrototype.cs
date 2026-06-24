using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store;

[Prototype(null, 1)]
[DataDefinition]
public sealed class StorePresetPrototype : IPrototype, ISerializationGenerated<StorePresetPrototype>, ISerializationGenerated
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("storeName", false, 1, true, false, null)]
	public string StoreName { get; private set; } = string.Empty;

	[DataField("categories", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<StoreCategoryPrototype>))]
	public HashSet<string> Categories { get; private set; } = new HashSet<string>();

	[DataField("initialBalance", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, CurrencyPrototype>))]
	public Dictionary<string, FixedPoint2>? InitialBalance { get; private set; }

	[DataField("currencyWhitelist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<CurrencyPrototype>))]
	public HashSet<string> CurrencyWhitelist { get; private set; } = new HashSet<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StorePresetPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<StorePresetPrototype>(this, ref target, hookCtx, false, context))
		{
			string IDTemp = null;
			if (ID == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ID, ref IDTemp, hookCtx, false, context))
			{
				IDTemp = ID;
			}
			target.ID = IDTemp;
			string StoreNameTemp = null;
			if (StoreName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StoreName, ref StoreNameTemp, hookCtx, false, context))
			{
				StoreNameTemp = StoreName;
			}
			target.StoreName = StoreNameTemp;
			HashSet<string> CategoriesTemp = null;
			if (Categories == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(Categories, ref CategoriesTemp, hookCtx, true, context))
			{
				CategoriesTemp = serialization.CreateCopy<HashSet<string>>(Categories, hookCtx, context, false);
			}
			target.Categories = CategoriesTemp;
			Dictionary<string, FixedPoint2> InitialBalanceTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, FixedPoint2>>(InitialBalance, ref InitialBalanceTemp, hookCtx, true, context))
			{
				InitialBalanceTemp = serialization.CreateCopy<Dictionary<string, FixedPoint2>>(InitialBalance, hookCtx, context, false);
			}
			target.InitialBalance = InitialBalanceTemp;
			HashSet<string> CurrencyWhitelistTemp = null;
			if (CurrencyWhitelist == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(CurrencyWhitelist, ref CurrencyWhitelistTemp, hookCtx, true, context))
			{
				CurrencyWhitelistTemp = serialization.CreateCopy<HashSet<string>>(CurrencyWhitelist, hookCtx, context, false);
			}
			target.CurrencyWhitelist = CurrencyWhitelistTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StorePresetPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StorePresetPrototype cast = (StorePresetPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public StorePresetPrototype Instantiate()
	{
		return new StorePresetPrototype();
	}
}
