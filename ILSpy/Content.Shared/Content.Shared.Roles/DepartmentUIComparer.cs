using System;
using System.Collections.Generic;

namespace Content.Shared.Roles;

public sealed class DepartmentUIComparer : IComparer<DepartmentPrototype>
{
	public static readonly DepartmentUIComparer Instance = new DepartmentUIComparer();

	public int Compare(DepartmentPrototype? x, DepartmentPrototype? y)
	{
		if (x == y)
		{
			return 0;
		}
		if (y == null)
		{
			return 1;
		}
		if (x == null)
		{
			return -1;
		}
		int cmp = -x.Weight.CompareTo(y.Weight);
		if (cmp == 0)
		{
			return string.Compare(x.ID, y.ID, StringComparison.Ordinal);
		}
		return cmp;
	}
}
