using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reagent;

[Serializable]
[NetSerializable]
[DataDefinition]
public struct ReagentQuantity : IEquatable<ReagentQuantity>, ISerializationGenerated<ReagentQuantity>, ISerializationGenerated
{
	[DataField("Quantity", false, 1, true, false, null)]
	public FixedPoint2 Quantity { get; private set; }

	[IncludeDataField(false, 1, false, null)]
	[ViewVariables]
	public ReagentId Reagent { get; private set; }

	public ReagentQuantity(string reagentId, FixedPoint2 quantity, List<ReagentData>? data = null)
		: this(new ReagentId(reagentId, data), quantity)
	{
	}

	public ReagentQuantity(ReagentId reagent, FixedPoint2 quantity)
	{
		Reagent = reagent;
		Quantity = quantity;
	}

	public ReagentQuantity()
		: this(default(ReagentId), default(FixedPoint2))
	{
	}

	public override string ToString()
	{
		return Reagent.ToString(Quantity);
	}

	public void Deconstruct(out string prototype, out FixedPoint2 quantity, out List<ReagentData>? data)
	{
		prototype = Reagent.Prototype;
		quantity = Quantity;
		data = Reagent.Data;
	}

	public void Deconstruct(out ReagentId id, out FixedPoint2 quantity)
	{
		id = Reagent;
		quantity = Quantity;
	}

	public bool Equals(ReagentQuantity other)
	{
		if (Quantity != other.Quantity)
		{
			return Reagent.Equals(other.Reagent);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ReagentQuantity other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Reagent.GetHashCode(), Quantity);
	}

	public static bool operator ==(ReagentQuantity left, ReagentQuantity right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ReagentQuantity left, ReagentQuantity right)
	{
		return !(left == right);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReagentQuantity target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<ReagentQuantity>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 QuantityTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Quantity, ref QuantityTemp, hookCtx, false, context))
			{
				QuantityTemp = serialization.CreateCopy<FixedPoint2>(Quantity, hookCtx, context, false);
			}
			ReagentId ReagentTemp = default(ReagentId);
			if (!serialization.TryCustomCopy<ReagentId>(Reagent, ref ReagentTemp, hookCtx, false, context))
			{
				serialization.CopyTo<ReagentId>(Reagent, ref ReagentTemp, hookCtx, context, false);
			}
			ReagentQuantity reagentQuantity = target;
			reagentQuantity.Quantity = QuantityTemp;
			reagentQuantity.Reagent = ReagentTemp;
			target = reagentQuantity;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReagentQuantity target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentQuantity cast = (ReagentQuantity)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ReagentQuantity Instantiate()
	{
		return new ReagentQuantity();
	}
}
