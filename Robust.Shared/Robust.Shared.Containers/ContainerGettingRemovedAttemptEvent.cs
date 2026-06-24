using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class ContainerGettingRemovedAttemptEvent : ContainerAttemptEventBase
{
	public ContainerGettingRemovedAttemptEvent(BaseContainer container, EntityUid entityUid)
		: base(container, entityUid)
	{
	}
}
