using System;
using Content.Shared.Climbing.Systems;
using Content.Shared.Movement.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Containers;

public sealed class ExitContainerOnMoveSystem : EntitySystem
{
	[Dependency]
	private ClimbSystem _climb;

	[Dependency]
	private SharedContainerSystem _container;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExitContainerOnMoveComponent, ContainerRelayMovementEntityEvent>((EntityEventRefHandler<ExitContainerOnMoveComponent, ContainerRelayMovementEntityEvent>)OnContainerRelay, (Type[])null, (Type[])null);
	}

	private void OnContainerRelay(Entity<ExitContainerOnMoveComponent> ent, ref ContainerRelayMovementEntityEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Entity<ExitContainerOnMoveComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ExitContainerOnMoveComponent exitContainerOnMoveComponent = default(ExitContainerOnMoveComponent);
		val.Deconstruct(ref val2, ref exitContainerOnMoveComponent);
		ExitContainerOnMoveComponent comp = exitContainerOnMoveComponent;
		ContainerManagerComponent containerManager = default(ContainerManagerComponent);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryComp<ContainerManagerComponent>(Entity<ExitContainerOnMoveComponent>.op_Implicit(ent), ref containerManager) && _container.TryGetContainer(Entity<ExitContainerOnMoveComponent>.op_Implicit(ent), comp.ContainerId, ref container, containerManager) && container.Contains(args.Entity))
		{
			_climb.ForciblySetClimbing(args.Entity, Entity<ExitContainerOnMoveComponent>.op_Implicit(ent));
			_container.RemoveEntity(Entity<ExitContainerOnMoveComponent>.op_Implicit(ent), args.Entity, containerManager, (TransformComponent)null, (MetaDataComponent)null, true, false, (EntityCoordinates?)null, (Angle?)null);
		}
	}
}
