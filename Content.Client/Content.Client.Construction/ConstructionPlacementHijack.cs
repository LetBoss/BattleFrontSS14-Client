using System.Linq;
using Content.Shared.Construction.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Placement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Construction;

public sealed class ConstructionPlacementHijack : PlacementHijack
{
	private readonly ConstructionSystem _constructionSystem;

	private readonly ConstructionPrototype? _prototype;

	public ConstructionSystem? CurrentConstructionSystem => _constructionSystem;

	public ConstructionPrototype? CurrentPrototype => _prototype;

	public override bool CanRotate { get; }

	public ConstructionPlacementHijack(ConstructionSystem constructionSystem, ConstructionPrototype? prototype)
	{
		_constructionSystem = constructionSystem;
		_prototype = prototype;
		CanRotate = prototype?.CanRotate ?? true;
	}

	public override bool HijackPlacementRequest(EntityCoordinates coordinates)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (_prototype != null)
		{
			Direction direction = ((PlacementHijack)this).Manager.Direction;
			_constructionSystem.SpawnGhost(_prototype, coordinates, direction);
		}
		return true;
	}

	public unsafe override bool HijackDeletion(EntityUid entity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (IoCManager.Resolve<IEntityManager>().HasComponent<ConstructionGhostComponent>(entity))
		{
			_constructionSystem.ClearGhost(((object)(*(EntityUid*)(&entity))/*cast due to constrained. prefix*/).GetHashCode());
		}
		return true;
	}

	public override void StartHijack(PlacementManager manager)
	{
		((PlacementHijack)this).StartHijack(manager);
		EntityPrototype val = default(EntityPrototype);
		if (_prototype != null && _constructionSystem.TryGetRecipePrototype(_prototype.ID, out string targetProtoId) && IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(targetProtoId, ref val))
		{
			manager.CurrentTextures = IoCManager.Resolve<IEntityManager>().System<SpriteSystem>().GetPrototypeTextures(val)
				.ToList();
		}
	}
}
