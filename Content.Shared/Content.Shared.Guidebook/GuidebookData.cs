using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Guidebook;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class GuidebookData : ISerializationGenerated<GuidebookData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, Dictionary<string, Dictionary<string, object?>>> Data = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

	public FrozenDictionary<string, FrozenDictionary<string, FrozenDictionary<string, object?>>> FrozenData;

	public bool IsFrozen;

	[DataField(null, false, 1, false, false, null)]
	public int Count { get; private set; }

	public void AddData(string prototype, string component, string field, object? value)
	{
		if (IsFrozen)
		{
			throw new InvalidOperationException("Attempted to add data to GuidebookData while it is frozen!");
		}
		Extensions.GetOrNew<string, Dictionary<string, object>>(Extensions.GetOrNew<string, Dictionary<string, Dictionary<string, object>>>(Data, prototype), component).Add(field, value);
		Count++;
	}

	public bool TryGetValue(string prototype, string component, string field, out object? value)
	{
		if (!IsFrozen)
		{
			throw new InvalidOperationException("Freeze the GuidebookData before calling TryGetValue!");
		}
		if (FrozenData.TryGetValue(prototype, out FrozenDictionary<string, FrozenDictionary<string, object>> p) && p.TryGetValue(component, out var c) && c.TryGetValue(field, out value))
		{
			return true;
		}
		value = null;
		return false;
	}

	public void Clear()
	{
		Data.Clear();
		Count = 0;
		IsFrozen = false;
	}

	public void Freeze()
	{
		Dictionary<string, FrozenDictionary<string, FrozenDictionary<string, object>>> protos = new Dictionary<string, FrozenDictionary<string, FrozenDictionary<string, object>>>();
		foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, object>>> datum in Data)
		{
			datum.Deconstruct(out var key, out var value);
			string protoId = key;
			Dictionary<string, Dictionary<string, object>> dictionary = value;
			Dictionary<string, FrozenDictionary<string, object>> comps = new Dictionary<string, FrozenDictionary<string, object>>();
			foreach (KeyValuePair<string, Dictionary<string, object>> item in dictionary)
			{
				item.Deconstruct(out key, out var value2);
				string compId = key;
				Dictionary<string, object> compData = value2;
				comps.Add(compId, compData.ToFrozenDictionary());
			}
			protos.Add(protoId, comps.ToFrozenDictionary());
		}
		FrozenData = protos.ToFrozenDictionary();
		Data.Clear();
		IsFrozen = true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GuidebookData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<GuidebookData>(this, ref target, hookCtx, false, context))
		{
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			Dictionary<string, Dictionary<string, Dictionary<string, object>>> DataTemp = null;
			if (Data == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(Data, ref DataTemp, hookCtx, true, context))
			{
				DataTemp = serialization.CreateCopy<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(Data, hookCtx, context, false);
			}
			target.Data = DataTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GuidebookData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GuidebookData cast = (GuidebookData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public GuidebookData Instantiate()
	{
		return new GuidebookData();
	}
}
