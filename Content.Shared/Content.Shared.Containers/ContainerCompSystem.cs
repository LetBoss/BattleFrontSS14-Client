using System;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Containers;

public sealed class ContainerCompSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _proto;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ContainerCompComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<ContainerCompComponent, EntInsertedIntoContainerMessage>)OnConInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainerCompComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<ContainerCompComponent, EntRemovedFromContainerMessage>)OnConRemove, (Type[])null, (Type[])null);
	}

	private void OnConRemove(Entity<ContainerCompComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype entProto = default(EntityPrototype);
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.Container) && !_timing.ApplyingState && _proto.TryIndex(ent.Comp.Proto, ref entProto))
		{
			base.EntityManager.RemoveComponents(((ContainerModifiedMessage)args).Entity, entProto.Components);
		}
	}

	private void OnConInsert(Entity<ContainerCompComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype entProto = default(EntityPrototype);
		if (!(((ContainerModifiedMessage)args).Container.ID != ent.Comp.Container) && !_timing.ApplyingState && _proto.TryIndex(ent.Comp.Proto, ref entProto))
		{
			base.EntityManager.AddComponents(((ContainerModifiedMessage)args).Entity, entProto.Components, true);
		}
	}
}
