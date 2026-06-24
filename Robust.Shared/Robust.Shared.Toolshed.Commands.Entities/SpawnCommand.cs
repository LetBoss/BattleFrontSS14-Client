using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class SpawnCommand : ToolshedCommand
{
	private SharedContainerSystem? sharedContainerSystem;

	[CommandImplementation("at")]
	public EntityUid SpawnAt([PipedArgument] EntityCoordinates target, EntProtoId proto)
	{
		return Spawn(proto, target);
	}

	[CommandImplementation("at")]
	public IEnumerable<EntityUid> SpawnAt([PipedArgument] IEnumerable<EntityCoordinates> target, EntProtoId proto)
	{
		return target.Select((EntityCoordinates x) => SpawnAt(x, proto));
	}

	[CommandImplementation("on")]
	public EntityUid SpawnOn([PipedArgument] EntityUid target, EntProtoId proto)
	{
		return Spawn(proto, Transform(target).Coordinates);
	}

	[CommandImplementation("on")]
	public IEnumerable<EntityUid> SpawnOn([PipedArgument] IEnumerable<EntityUid> target, EntProtoId proto)
	{
		return target.Select((EntityUid x) => SpawnOn(x, proto));
	}

	[CommandImplementation("in")]
	public EntityUid SpawnIn([PipedArgument] EntityUid target, string containerId, EntProtoId proto)
	{
		EntityUid entityUid = SpawnOn(target, proto);
		if (!TryComp<TransformComponent>(entityUid, out TransformComponent component) || !TryComp<MetaDataComponent>(entityUid, out MetaDataComponent component2))
		{
			return entityUid;
		}
		TryComp<PhysicsComponent>(entityUid, out PhysicsComponent component3);
		if (sharedContainerSystem == null)
		{
			sharedContainerSystem = EntityManager.System<SharedContainerSystem>();
		}
		BaseContainer container = sharedContainerSystem.GetContainer(target, containerId);
		sharedContainerSystem.InsertOrDrop((Owner: entityUid, Comp1: component, Comp2: component2, Comp3: component3), container);
		return entityUid;
	}

	[CommandImplementation("in")]
	public IEnumerable<EntityUid> SpawnIn([PipedArgument] IEnumerable<EntityUid> target, string containerId, EntProtoId proto)
	{
		return target.Select((EntityUid x) => SpawnIn(x, containerId, proto));
	}

	[CommandImplementation("attached")]
	public EntityUid SpawnIn([PipedArgument] EntityUid target, EntProtoId proto)
	{
		return Spawn(proto, new EntityCoordinates(target, Vector2.Zero));
	}

	[CommandImplementation("attached")]
	public IEnumerable<EntityUid> SpawnIn([PipedArgument] IEnumerable<EntityUid> target, EntProtoId proto)
	{
		return target.Select((EntityUid x) => SpawnIn(x, proto));
	}
}
