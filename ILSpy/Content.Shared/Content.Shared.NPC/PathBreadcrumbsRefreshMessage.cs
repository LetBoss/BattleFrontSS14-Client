using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public sealed class PathBreadcrumbsRefreshMessage : EntityEventArgs
{
	public NetEntity GridUid;

	public Vector2i Origin;

	public List<PathfindingBreadcrumb> Data = new List<PathfindingBreadcrumb>();
}
