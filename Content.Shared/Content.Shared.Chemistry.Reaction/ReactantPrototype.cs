using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reaction;

[DataDefinition]
public sealed class ReactantPrototype : ISerializationGenerated<ReactantPrototype>, ISerializationGenerated
{
	[DataField("amount", false, 1, false, false, null)]
	private FixedPoint2 _amount = FixedPoint2.New(1);

	[DataField("catalyst", false, 1, false, false, null)]
	private bool _catalyst;

	public FixedPoint2 Amount => _amount;

	public bool Catalyst => _catalyst;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReactantPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<ReactantPrototype>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 _amountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(_amount, ref _amountTemp, hookCtx, false, context))
			{
				_amountTemp = serialization.CreateCopy<FixedPoint2>(_amount, hookCtx, context, false);
			}
			target._amount = _amountTemp;
			bool _catalystTemp = false;
			if (!serialization.TryCustomCopy<bool>(_catalyst, ref _catalystTemp, hookCtx, false, context))
			{
				_catalystTemp = _catalyst;
			}
			target._catalyst = _catalystTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReactantPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactantPrototype cast = (ReactantPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ReactantPrototype Instantiate()
	{
		return new ReactantPrototype();
	}
}
