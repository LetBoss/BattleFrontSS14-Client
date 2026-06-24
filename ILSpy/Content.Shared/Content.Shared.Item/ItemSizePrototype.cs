using System;
using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Item;

[Prototype(null, 1)]
public sealed class ItemSizePrototype : IPrototype, IComparable<ItemSizePrototype>
{
	[DataField(null, false, 1, false, false, null)]
	public int Weight = 1;

	[DataField(null, false, 1, false, false, null)]
	public LocId Name;

	[DataField(null, false, 1, true, false, null)]
	public IReadOnlyList<Box2i> DefaultShape = new List<Box2i>();

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LatheTime = TimeSpan.FromSeconds(2L);

	[IdDataField(1, null)]
	public string ID { get; private set; }

	public int CompareTo(ItemSizePrototype? other)
	{
		if (other == null)
		{
			return 0;
		}
		return Weight.CompareTo(other.Weight);
	}

	public static bool operator <(ItemSizePrototype a, ItemSizePrototype b)
	{
		return a.Weight < b.Weight;
	}

	public static bool operator >(ItemSizePrototype a, ItemSizePrototype b)
	{
		return a.Weight > b.Weight;
	}

	public static bool operator <=(ItemSizePrototype a, ItemSizePrototype b)
	{
		return a.Weight <= b.Weight;
	}

	public static bool operator >=(ItemSizePrototype a, ItemSizePrototype b)
	{
		return a.Weight >= b.Weight;
	}
}
