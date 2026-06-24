using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class EntGotInsertedIntoContainerMessage : ContainerModifiedMessage
{
	public EntGotInsertedIntoContainerMessage(EntityUid entity, BaseContainer container)
		: base(entity, container)
	{
	}
}
