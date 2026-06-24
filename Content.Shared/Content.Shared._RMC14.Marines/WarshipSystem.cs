using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared._RMC14.Marines;

public sealed class WarshipSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	public bool TryGetWarshipMap(EntityUid reference, out MapId mapId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? referenceMap = _transform.GetMap(Entity<TransformComponent>.op_Implicit(reference));
		if (((EntitySystem)this).HasComp<AlmayerComponent>(referenceMap))
		{
			mapId = _transform.GetMapId(Entity<TransformComponent>.op_Implicit(reference));
			return true;
		}
		AlmayerComponent almayerComponent = default(AlmayerComponent);
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<AlmayerComponent, TransformComponent>().MoveNext(ref almayerComponent, ref xform))
		{
			mapId = xform.MapID;
			return true;
		}
		mapId = default(MapId);
		return false;
	}
}
