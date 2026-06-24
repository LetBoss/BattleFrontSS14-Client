using Robust.Shared.GameObjects;

namespace Robust.Shared.Containers;

public abstract class ContainerModifiedMessage : EntityEventArgs
{
	public BaseContainer Container { get; }

	public EntityUid Entity { get; }

	protected ContainerModifiedMessage(EntityUid entity, BaseContainer container)
	{
		Entity = entity;
		Container = container;
	}
}
