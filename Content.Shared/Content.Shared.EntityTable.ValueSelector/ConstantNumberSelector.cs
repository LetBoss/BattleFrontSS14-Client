using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable.ValueSelector;

public sealed class ConstantNumberSelector : NumberSelector, ISerializationGenerated<ConstantNumberSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Value = 1;

	public ConstantNumberSelector(int value)
	{
		Value = value;
	}

	public override int Get(System.Random rand)
	{
		return Value;
	}

	public ConstantNumberSelector()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ConstantNumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NumberSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ConstantNumberSelector)definitionCast;
		if (!serialization.TryCustomCopy<ConstantNumberSelector>(this, ref target, hookCtx, false, context))
		{
			int ValueTemp = 0;
			if (!serialization.TryCustomCopy<int>(Value, ref ValueTemp, hookCtx, false, context))
			{
				ValueTemp = Value;
			}
			target.Value = ValueTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ConstantNumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref NumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstantNumberSelector cast = (ConstantNumberSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstantNumberSelector cast = (ConstantNumberSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ConstantNumberSelector Instantiate()
	{
		return new ConstantNumberSelector();
	}
}
