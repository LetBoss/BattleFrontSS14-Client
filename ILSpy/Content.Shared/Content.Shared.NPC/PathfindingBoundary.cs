using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public struct PathfindingBoundary(bool closed, List<PathfindingBreadcrumb> crumbs)
{
	public List<PathfindingBreadcrumb> Breadcrumbs = crumbs;

	public bool Closed = closed;
}
