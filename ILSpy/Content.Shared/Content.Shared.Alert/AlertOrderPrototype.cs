using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Alert;

[Prototype(null, 1)]
[DataDefinition]
public sealed class AlertOrderPrototype : IPrototype, IComparer<AlertPrototype>, ISerializationGenerated<AlertOrderPrototype>, ISerializationGenerated
{
	private readonly Dictionary<ProtoId<AlertPrototype>, int> _typeToIdx = new Dictionary<ProtoId<AlertPrototype>, int>();

	private readonly Dictionary<ProtoId<AlertCategoryPrototype>, int> _categoryToIdx = new Dictionary<ProtoId<AlertCategoryPrototype>, int>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	private (string type, string alert)[] Order
	{
		get
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			(string, string)[] res = new(string, string)[_typeToIdx.Count + _categoryToIdx.Count];
			int value;
			foreach (KeyValuePair<ProtoId<AlertPrototype>, int> item in _typeToIdx)
			{
				item.Deconstruct(out var key, out value);
				ProtoId<AlertPrototype> type = key;
				int id = value;
				res[id] = ("alertType", ((object)type/*cast due to constrained. prefix*/).ToString());
			}
			foreach (KeyValuePair<ProtoId<AlertCategoryPrototype>, int> item2 in _categoryToIdx)
			{
				item2.Deconstruct(out var key2, out value);
				ProtoId<AlertCategoryPrototype> category = key2;
				int id2 = value;
				res[id2] = ("category", ((object)category/*cast due to constrained. prefix*/).ToString());
			}
			return res;
		}
		set
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			for (int j = 0; j < value.Length; j++)
			{
				var (type, alert) = value[j];
				if (!(type == "alertType"))
				{
					if (!(type == "category"))
					{
						throw new ArgumentException();
					}
					_categoryToIdx[ProtoId<AlertCategoryPrototype>.op_Implicit(alert)] = i++;
				}
				else
				{
					_typeToIdx[ProtoId<AlertPrototype>.op_Implicit(alert)] = i++;
				}
			}
		}
	}

	private int GetOrderIndex(AlertPrototype alert)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_typeToIdx.TryGetValue(ProtoId<AlertPrototype>.op_Implicit(alert.ID), out var idx))
		{
			return idx;
		}
		if (alert.Category.HasValue && _categoryToIdx.TryGetValue(alert.Category.Value, out idx))
		{
			return idx;
		}
		return -1;
	}

	public int Compare(AlertPrototype? x, AlertPrototype? y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		int idx = GetOrderIndex(x);
		int idy = GetOrderIndex(y);
		if (idx == -1 && idy == -1)
		{
			return string.Compare(x.ID, y.ID, StringComparison.InvariantCulture);
		}
		if (idx == -1)
		{
			return 1;
		}
		if (idy == -1)
		{
			return -1;
		}
		int result = idx - idy;
		if (result == 0)
		{
			return string.Compare(x.ID, y.ID, StringComparison.InvariantCulture);
		}
		return result;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AlertOrderPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<AlertOrderPrototype>(this, ref target, hookCtx, false, context))
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
			(string, string)[] OrderTemp = null;
			if (Order == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<(string, string)[]>(Order, ref OrderTemp, hookCtx, true, context))
			{
				OrderTemp = serialization.CreateCopy<(string, string)[]>(Order, hookCtx, context, false);
			}
			target.Order = OrderTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AlertOrderPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlertOrderPrototype cast = (AlertOrderPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AlertOrderPrototype Instantiate()
	{
		return new AlertOrderPrototype();
	}
}
