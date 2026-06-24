using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Chemistry.Reagent;

[Serializable]
[NetSerializable]
[DataDefinition]
public struct ReagentId : IEquatable<ReagentId>, ISerializationGenerated<ReagentId>, ISerializationGenerated
{
	[DataField("ReagentId", false, 1, true, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string Prototype { get; private set; }

	[DataField("data", false, 1, false, false, null)]
	public List<ReagentData>? Data { get; private set; }

	public ReagentId(string prototype, List<ReagentData>? data)
	{
		Data = new List<ReagentData>();
		Prototype = prototype;
		Data = data ?? new List<ReagentData>();
	}

	public ReagentId()
	{
		Data = new List<ReagentData>();
		Prototype = null;
		Data = new List<ReagentData>();
	}

	public List<ReagentData> EnsureReagentData()
	{
		if (Data == null)
		{
			return new List<ReagentData>();
		}
		return Data;
	}

	public bool Equals(ReagentId other)
	{
		if (Prototype != other.Prototype)
		{
			return false;
		}
		if (Data == null)
		{
			return other.Data == null;
		}
		if (other.Data == null)
		{
			return false;
		}
		if (Data.Except(other.Data).Any() || other.Data.Except(Data).Any() || Data.Count != other.Data.Count)
		{
			return false;
		}
		return true;
	}

	public bool Equals(string prototype, List<ReagentData>? otherData = null)
	{
		if (Prototype != prototype)
		{
			return false;
		}
		if (Data == null)
		{
			return otherData == null;
		}
		return Data.Equals(otherData);
	}

	public override bool Equals(object? obj)
	{
		if (obj is ReagentId other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Prototype, Data);
	}

	public string ToString(FixedPoint2 quantity)
	{
		return $"{Prototype}:{quantity}";
	}

	public override string ToString()
	{
		return Prototype ?? "";
	}

	public static bool operator ==(ReagentId left, ReagentId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ReagentId left, ReagentId right)
	{
		return !(left == right);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReagentId target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ReagentId>(this, ref target, hookCtx, false, context))
		{
			string PrototypeTemp = null;
			if (Prototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = Prototype;
			}
			List<ReagentData> DataTemp = null;
			if (!serialization.TryCustomCopy<List<ReagentData>>(Data, ref DataTemp, hookCtx, true, context))
			{
				DataTemp = serialization.CreateCopy<List<ReagentData>>(Data, hookCtx, context, false);
			}
			ReagentId reagentId = target;
			reagentId.Prototype = PrototypeTemp;
			reagentId.Data = DataTemp;
			target = reagentId;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReagentId target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentId cast = (ReagentId)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ReagentId Instantiate()
	{
		return new ReagentId();
	}
}
