using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store;

[Prototype(null, 1)]
[DataDefinition]
public sealed class DiscountCategoryPrototype : IPrototype, ISerializationGenerated<DiscountCategoryPrototype>, ISerializationGenerated
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int Weight { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int? MaxItems { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DiscountCategoryPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DiscountCategoryPrototype>(this, ref target, hookCtx, false, context))
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
			int WeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(Weight, ref WeightTemp, hookCtx, false, context))
			{
				WeightTemp = Weight;
			}
			target.Weight = WeightTemp;
			int? MaxItemsTemp = null;
			if (!serialization.TryCustomCopy<int?>(MaxItems, ref MaxItemsTemp, hookCtx, false, context))
			{
				MaxItemsTemp = MaxItems;
			}
			target.MaxItems = MaxItemsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DiscountCategoryPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DiscountCategoryPrototype cast = (DiscountCategoryPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DiscountCategoryPrototype Instantiate()
	{
		return new DiscountCategoryPrototype();
	}
}
