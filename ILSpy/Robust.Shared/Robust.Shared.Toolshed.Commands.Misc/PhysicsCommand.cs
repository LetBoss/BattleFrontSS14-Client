using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class PhysicsCommand : ToolshedCommand
{
	private SharedTransformSystem? _xform;

	[CommandImplementation("velocity")]
	public IEnumerable<float> Velocity([PipedArgument] IEnumerable<EntityUid> input)
	{
		EntityQuery<PhysicsComponent> physQuery = GetEntityQuery<PhysicsComponent>();
		foreach (EntityUid item in input)
		{
			if (physQuery.TryGetComponent(item, out PhysicsComponent component))
			{
				yield return component.LinearVelocity.Length();
			}
		}
	}

	[CommandImplementation("parent")]
	public IEnumerable<EntityUid> Parent([PipedArgument] IEnumerable<EntityUid> input)
	{
		if (_xform == null)
		{
			_xform = GetSys<SharedTransformSystem>();
		}
		return input.Select((EntityUid x) => Comp<TransformComponent>(x).ParentUid);
	}

	[CommandImplementation("angular_velocity")]
	public IEnumerable<float> AngularVelocity([PipedArgument] IEnumerable<EntityUid> input)
	{
		EntityQuery<PhysicsComponent> physQuery = GetEntityQuery<PhysicsComponent>();
		foreach (EntityUid item in input)
		{
			if (physQuery.TryGetComponent(item, out PhysicsComponent component))
			{
				yield return component.AngularVelocity;
			}
		}
	}
}
