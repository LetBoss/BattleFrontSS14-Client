using System;
using System.Collections.Generic;

namespace Content.Shared.Roles;

public sealed class JobUIComparer : IComparer<JobPrototype>
{
	public static readonly JobUIComparer Instance = new JobUIComparer();

	public int Compare(JobPrototype? x, JobPrototype? y)
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
		int cmp = -x.RealDisplayWeight.CompareTo(y.RealDisplayWeight);
		if (cmp != 0)
		{
			return cmp;
		}
		return string.Compare(x.ID, y.ID, StringComparison.Ordinal);
	}
}
