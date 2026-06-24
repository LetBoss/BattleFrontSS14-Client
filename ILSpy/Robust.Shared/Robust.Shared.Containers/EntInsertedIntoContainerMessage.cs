using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class EntInsertedIntoContainerMessage : ContainerModifiedMessage
{
	public readonly EntityUid OldParent;

	public EntInsertedIntoContainerMessage(EntityUid entity, EntityUid oldParent, BaseContainer container)
		: base(entity, container)
	{
		OldParent = oldParent;
	}
}
