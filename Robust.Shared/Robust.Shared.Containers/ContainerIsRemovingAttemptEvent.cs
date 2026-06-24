using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class ContainerIsRemovingAttemptEvent : ContainerAttemptEventBase
{
	public ContainerIsRemovingAttemptEvent(BaseContainer container, EntityUid entityUid)
		: base(container, entityUid)
	{
	}
}
