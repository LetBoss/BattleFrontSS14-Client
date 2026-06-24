using Robust.Shared.GameObjects;
using Robust.Shared.Physics;

namespace Robust.Shared.ComponentTrees;

public interface IComponentTreeEntry<TComp> where TComp : Component
{
	EntityUid? TreeUid { get; set; }

	DynamicTree<ComponentTreeEntry<TComp>>? Tree { get; set; }

	bool AddToTree { get; }

	bool TreeUpdateQueued { get; set; }
}
