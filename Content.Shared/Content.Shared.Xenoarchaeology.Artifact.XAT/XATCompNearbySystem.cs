using System.Collections.Generic;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATCompNearbySystem : BaseQueryUpdateXATSystem<XATCompNearbyComponent>
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly HashSet<Entity<IComponent>> _entities = new HashSet<Entity<IComponent>>();

	protected override void UpdateXAT(Entity<XenoArtifactComponent> artifact, Entity<XATCompNearbyComponent, XenoArtifactNodeComponent> node, float frameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		XATCompNearbyComponent compNearbyComponent = node.Comp1;
		MapCoordinates pos = _transform.GetMapCoordinates(Entity<XenoArtifactComponent>.op_Implicit(artifact), (TransformComponent)null);
		ComponentRegistration comp = ((EntitySystem)this).EntityManager.ComponentFactory.GetRegistration(compNearbyComponent.RequireComponentWithName, false);
		_entities.Clear();
		_entityLookup.GetEntitiesInRange(comp.Type, pos, compNearbyComponent.Radius, _entities, (LookupFlags)110);
		if (_entities.Count >= compNearbyComponent.Count)
		{
			Trigger(artifact, node);
		}
	}
}
