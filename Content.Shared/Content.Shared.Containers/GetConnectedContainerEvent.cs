using Robust.Shared.GameObjects;

namespace Content.Shared.Containers;

[ByRefEvent]
public struct GetConnectedContainerEvent
{
	public EntityUid? ContainerEntity;
}
