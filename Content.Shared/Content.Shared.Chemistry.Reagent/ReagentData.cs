using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reagent;

[Serializable]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
public abstract class ReagentData : IEquatable<ReagentData>, ISerializationGenerated<ReagentData>, ISerializationGenerated
{
	public virtual string ToString(string prototype, FixedPoint2 quantity)
	{
		return $"{prototype}:{GetType().Name}:{quantity}";
	}

	public virtual string ToString(string prototype)
	{
		return prototype + ":" + GetType().Name;
	}

	public abstract bool Equals(ReagentData? other);

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((ReagentData)obj);
	}

	public abstract override int GetHashCode();

	public abstract ReagentData Clone();

	public ReagentData()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref ReagentData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<ReagentData>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref ReagentData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentData cast = (ReagentData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual ReagentData Instantiate()
	{
		throw new NotImplementedException();
	}
}
