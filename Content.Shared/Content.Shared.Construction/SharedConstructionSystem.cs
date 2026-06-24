using System.Collections.Generic;
using System.Linq;
using Content.Shared.Construction.Components;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction;

public abstract class SharedConstructionSystem : EntitySystem
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	public SharedInteractionSystem.Ignored? GetPredicate(bool canBuildInImpassable, MapCoordinates coords)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!canBuildInImpassable)
		{
			return null;
		}
		EntityUid gridUid = default(EntityUid);
		MapGridComponent grid = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(coords, ref gridUid, ref grid))
		{
			return null;
		}
		HashSet<EntityUid> ignored = _map.GetAnchoredEntities(Entity<MapGridComponent>.op_Implicit((gridUid, grid)), coords).ToHashSet();
		return (EntityUid e) => ignored.Contains(e);
	}

	public string GetExamineName(GenericPartInfo info)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		LocId? examineName = info.ExamineName;
		if (examineName.HasValue)
		{
			return base.Loc.GetString(LocId.op_Implicit(info.ExamineName.Value));
		}
		return PrototypeManager.Index(info.DefaultPrototype).Name;
	}
}
