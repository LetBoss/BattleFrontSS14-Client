using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public sealed class EntRemovedFromContainerMessage : ContainerModifiedMessage
{
	public EntRemovedFromContainerMessage(EntityUid entity, BaseContainer container)
		: base(entity, container)
	{
	}
}
