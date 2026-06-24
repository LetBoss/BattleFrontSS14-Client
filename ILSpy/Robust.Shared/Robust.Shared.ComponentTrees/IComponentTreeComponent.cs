using Robust.Shared.GameObjects;
using Robust.Shared.Physics;

namespace Robust.Shared.ComponentTrees;

public interface IComponentTreeComponent<TComp> where TComp : Component, IComponentTreeEntry<TComp>
{
	DynamicTree<ComponentTreeEntry<TComp>> Tree { get; set; }
}
