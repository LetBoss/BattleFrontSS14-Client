using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Laws;

[Serializable]
[Virtual]
[DataDefinition]
[NetSerializable]
public class SiliconLaw : IComparable<SiliconLaw>, IEquatable<SiliconLaw>, ISerializationGenerated<SiliconLaw>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LawString = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public FixedPoint2 Order;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? LawIdentifierOverride;

	public int CompareTo(SiliconLaw? other)
	{
		if (other == null)
		{
			return -1;
		}
		return Order.CompareTo(other.Order);
	}

	public bool Equals(SiliconLaw? other)
	{
		if (other == null)
		{
			return false;
		}
		if (LawString == other.LawString && Order == other.Order)
		{
			return LawIdentifierOverride == other.LawIdentifierOverride;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		return Equals(obj as SiliconLaw);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(LawString, Order, LawIdentifierOverride);
	}

	public SiliconLaw ShallowClone()
	{
		return new SiliconLaw
		{
			LawString = LawString,
			Order = Order,
			LawIdentifierOverride = LawIdentifierOverride
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SiliconLaw target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<SiliconLaw>(this, ref target, hookCtx, false, context))
		{
			string LawStringTemp = null;
			if (LawString == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LawString, ref LawStringTemp, hookCtx, false, context))
			{
				LawStringTemp = LawString;
			}
			target.LawString = LawStringTemp;
			FixedPoint2 OrderTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Order, ref OrderTemp, hookCtx, false, context))
			{
				OrderTemp = serialization.CreateCopy<FixedPoint2>(Order, hookCtx, context, false);
			}
			target.Order = OrderTemp;
			string LawIdentifierOverrideTemp = null;
			if (!serialization.TryCustomCopy<string>(LawIdentifierOverride, ref LawIdentifierOverrideTemp, hookCtx, false, context))
			{
				LawIdentifierOverrideTemp = LawIdentifierOverride;
			}
			target.LawIdentifierOverride = LawIdentifierOverrideTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SiliconLaw target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLaw cast = (SiliconLaw)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual SiliconLaw Instantiate()
	{
		return new SiliconLaw();
	}
}
