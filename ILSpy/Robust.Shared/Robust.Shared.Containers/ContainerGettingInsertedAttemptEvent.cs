using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class ContainerGettingInsertedAttemptEvent : ContainerAttemptEventBase
{
	public bool AssumeEmpty { get; set; }

	public ContainerGettingInsertedAttemptEvent(BaseContainer container, EntityUid entityUid, bool assumeEmpty)
		: base(container, entityUid)
	{
		AssumeEmpty = assumeEmpty;
	}
}
