using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public abstract class ContainerAttemptEventBase : CancellableEntityEventArgs
{
	public readonly BaseContainer Container;

	public readonly EntityUid EntityUid;

	public ContainerAttemptEventBase(BaseContainer container, EntityUid entityUid)
	{
		Container = container;
		EntityUid = entityUid;
	}
}
