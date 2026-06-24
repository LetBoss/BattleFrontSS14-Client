using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Access.Components;

[ByRefEvent]
public record struct GetAccessTagsEvent(HashSet<ProtoId<AccessLevelPrototype>> Tags, IPrototypeManager PrototypeManager)
{
	public void AddGroup(ProtoId<AccessGroupPrototype> group)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AccessGroupPrototype groupPrototype = default(AccessGroupPrototype);
		if (PrototypeManager.TryIndex<AccessGroupPrototype>(group, ref groupPrototype))
		{
			Tags.UnionWith(groupPrototype.Tags);
		}
	}
}
