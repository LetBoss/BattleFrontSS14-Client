using System;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Physics.Systems;

public record struct QueryFilter()
{
	public long LayerBits = 0L;

	public long MaskBits = 0L;

	public Func<EntityUid, bool>? IsIgnored = null;

	public QueryFlags Flags = QueryFlags.Dynamic | QueryFlags.Static;
}
