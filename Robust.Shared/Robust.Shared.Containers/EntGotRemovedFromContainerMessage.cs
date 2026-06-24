using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class EntGotRemovedFromContainerMessage : ContainerModifiedMessage
{
	public EntGotRemovedFromContainerMessage(EntityUid entity, BaseContainer container)
		: base(entity, container)
	{
	}
}
