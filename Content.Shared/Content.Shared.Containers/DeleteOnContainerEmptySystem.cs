using System;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Containers;

public sealed class DeleteOnContainerEmptySystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DeleteOnContainerEmptyComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<DeleteOnContainerEmptyComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
	}

	private void OnEntRemoved(Entity<DeleteOnContainerEmptyComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId) && _container.TryGetContainer(ent.Owner, ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count == 0)
		{
			((EntitySystem)this).PredictedQueueDel(ent.Owner);
		}
	}
}
