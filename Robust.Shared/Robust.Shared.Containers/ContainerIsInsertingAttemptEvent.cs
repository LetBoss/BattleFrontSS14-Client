using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class ContainerIsInsertingAttemptEvent : ContainerAttemptEventBase
{
	public bool AssumeEmpty { get; set; }

	public ContainerIsInsertingAttemptEvent(BaseContainer container, EntityUid entityUid, bool assumeEmpty)
		: base(container, entityUid)
	{
		AssumeEmpty = assumeEmpty;
	}
}
