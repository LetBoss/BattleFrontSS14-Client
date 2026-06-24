using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store;

[Prototype(null, 1)]
[DataDefinition]
public sealed class CurrencyPrototype : IPrototype, ISerializationGenerated<CurrencyPrototype>, ISerializationGenerated
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("displayName", false, 1, false, false, null)]
	public string DisplayName { get; private set; } = string.Empty;

	[DataField("cash", false, 1, false, false, typeof(PrototypeIdValueDictionarySerializer<FixedPoint2, EntityPrototype>))]
	public Dictionary<FixedPoint2, string>? Cash { get; private set; }

	[DataField("canWithdraw", false, 1, false, false, null)]
	public bool CanWithdraw { get; private set; } = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CurrencyPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CurrencyPrototype>(this, ref target, hookCtx, false, context))
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
			string DisplayNameTemp = null;
			if (DisplayName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DisplayName, ref DisplayNameTemp, hookCtx, false, context))
			{
				DisplayNameTemp = DisplayName;
			}
			target.DisplayName = DisplayNameTemp;
			Dictionary<FixedPoint2, string> CashTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<FixedPoint2, string>>(Cash, ref CashTemp, hookCtx, true, context))
			{
				CashTemp = serialization.CreateCopy<Dictionary<FixedPoint2, string>>(Cash, hookCtx, context, false);
			}
			target.Cash = CashTemp;
			bool CanWithdrawTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanWithdraw, ref CanWithdrawTemp, hookCtx, false, context))
			{
				CanWithdrawTemp = CanWithdraw;
			}
			target.CanWithdraw = CanWithdrawTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CurrencyPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CurrencyPrototype cast = (CurrencyPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CurrencyPrototype Instantiate()
	{
		return new CurrencyPrototype();
	}
}
