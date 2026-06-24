using System.Numerics;
using Content.Client.Gameplay;
using Content.Client.Hands.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.RCD.Components;
using Content.Shared.RCD.Systems;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client.RCD;

public sealed class AlignRCDConstruction : PlacementMode
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IMapManager _mapManager;

	private readonly SharedMapSystem _mapSystem;

	private readonly HandsSystem _handsSystem;

	private readonly RCDSystem _rcdSystem;

	private readonly SharedTransformSystem _transformSystem;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IStateManager _stateManager;

	private const float SearchBoxSize = 2f;

	private const float PlaceColorBaseAlpha = 0.5f;

	private EntityCoordinates _unalignedMouseCoords;

	public AlignRCDConstruction(PlacementManager pMan)
		: base(pMan)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<AlignRCDConstruction>(this);
		_mapSystem = _entityManager.System<SharedMapSystem>();
		_handsSystem = _entityManager.System<HandsSystem>();
		_rcdSystem = _entityManager.System<RCDSystem>();
		_transformSystem = _entityManager.System<SharedTransformSystem>();
		Color validPlaceColor = ((PlacementMode)this).ValidPlaceColor;
		((PlacementMode)this).ValidPlaceColor = ((Color)(ref validPlaceColor)).WithAlpha(0.5f);
	}

	public override void AlignPlacementMode(ScreenCoordinates mouseScreen)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		_unalignedMouseCoords = ((PlacementMode)this).ScreenToCursorGrid(mouseScreen);
		((PlacementMode)this).MouseCoords = CoordinatesExtensions.AlignWithClosestGridTile(_unalignedMouseCoords, 2f, _entityManager, _mapManager);
		EntityUid? grid = _transformSystem.GetGrid(((PlacementMode)this).MouseCoords);
		MapGridComponent val = default(MapGridComponent);
		if (_entityManager.TryGetComponent<MapGridComponent>(grid, ref val))
		{
			((PlacementMode)this).CurrentTile = _mapSystem.GetTileRef(grid.Value, val, ((PlacementMode)this).MouseCoords);
			float num = (base.GridDistancing = (int)val.TileSize);
			TileRef currentTile;
			if (base.pManager.CurrentPermission.IsTile)
			{
				EntityUid entityId = ((PlacementMode)this).MouseCoords.EntityId;
				currentTile = ((PlacementMode)this).CurrentTile;
				float x = (float)((TileRef)(ref currentTile)).X + num / 2f;
				currentTile = ((PlacementMode)this).CurrentTile;
				((PlacementMode)this).MouseCoords = new EntityCoordinates(entityId, new Vector2(x, (float)((TileRef)(ref currentTile)).Y + num / 2f));
			}
			else
			{
				EntityUid entityId2 = ((PlacementMode)this).MouseCoords.EntityId;
				currentTile = ((PlacementMode)this).CurrentTile;
				float x2 = (float)((TileRef)(ref currentTile)).X + num / 2f + (float)base.pManager.PlacementOffset.X;
				currentTile = ((PlacementMode)this).CurrentTile;
				((PlacementMode)this).MouseCoords = new EntityCoordinates(entityId2, new Vector2(x2, (float)((TileRef)(ref currentTile)).Y + num / 2f + (float)base.pManager.PlacementOffset.Y));
			}
		}
	}

	public override bool IsValidPosition(EntityCoordinates position)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		TransformComponent val2 = default(TransformComponent);
		if (!_entityManager.TryGetComponent<TransformComponent>(val, ref val2))
		{
			return false;
		}
		Color invalidPlaceColor;
		if (!_transformSystem.InRange(val2.Coordinates, position, 1.5f))
		{
			invalidPlaceColor = ((PlacementMode)this).InvalidPlaceColor;
			((PlacementMode)this).InvalidPlaceColor = ((Color)(ref invalidPlaceColor)).WithAlpha((byte)0);
			return false;
		}
		invalidPlaceColor = ((PlacementMode)this).InvalidPlaceColor;
		((PlacementMode)this).InvalidPlaceColor = ((Color)(ref invalidPlaceColor)).WithAlpha(0.5f);
		if (!_handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(val.Value), out var item))
		{
			return false;
		}
		RCDComponent component = default(RCDComponent);
		if (!_entityManager.TryGetComponent<RCDComponent>(item, ref component))
		{
			return false;
		}
		EntityUid? grid = _transformSystem.GetGrid(position);
		MapGridComponent val3 = default(MapGridComponent);
		if (!_entityManager.TryGetComponent<MapGridComponent>(grid, ref val3))
		{
			return false;
		}
		TileRef tileRef = _mapSystem.GetTileRef(grid.Value, val3, position);
		Vector2i position2 = _mapSystem.TileIndicesFor(grid.Value, val3, position);
		if (!(_stateManager.CurrentState is GameplayStateBase gameplayStateBase))
		{
			return false;
		}
		EntityUid? clickedEntity = gameplayStateBase.GetClickedEntity(_transformSystem.ToMapCoordinates(_unalignedMouseCoords, true));
		if (!_rcdSystem.IsRCDOperationStillValid(item.Value, component, grid.Value, val3, tileRef, position2, clickedEntity, val.Value, popMsgs: false))
		{
			return false;
		}
		return true;
	}
}
