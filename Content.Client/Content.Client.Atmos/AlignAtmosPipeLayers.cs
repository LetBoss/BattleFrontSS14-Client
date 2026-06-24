using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Construction;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Construction.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Client.Placement.Modes;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Atmos;

public sealed class AlignAtmosPipeLayers : SnapgridCenter
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IEyeManager _eyeManager;

	private readonly SharedMapSystem _mapSystem;

	private readonly SharedTransformSystem _transformSystem;

	private readonly SharedAtmosPipeLayersSystem _pipeLayersSystem;

	private readonly SpriteSystem _spriteSystem;

	private const float SearchBoxSize = 2f;

	private EntityCoordinates _unalignedMouseCoords;

	private const float MouseDeadzoneRadius = 0.25f;

	private Color _guideColor = new Color(0f, 0f, 0.5785f, 1f);

	private const float GuideRadius = 0.1f;

	private const float GuideOffset = 7f / 32f;

	public AlignAtmosPipeLayers(PlacementManager pMan)
		: base(pMan)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<AlignAtmosPipeLayers>(this);
		_mapSystem = _entityManager.System<SharedMapSystem>();
		_transformSystem = _entityManager.System<SharedTransformSystem>();
		_pipeLayersSystem = _entityManager.System<SharedAtmosPipeLayersSystem>();
		_spriteSystem = _entityManager.System<SpriteSystem>();
	}

	public override void Render(in OverlayDrawArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _entityManager.System<SharedTransformSystem>().GetGrid(((PlacementMode)this).MouseCoords);
		if (grid.HasValue && base.Grid != null)
		{
			if ((int)((PlacementMode)this).pManager.PlacementType == 0)
			{
				Angle worldRotation = _transformSystem.GetWorldRotation(grid.Value);
				Vector2 vector = _mapSystem.LocalToWorld(grid.Value, base.Grid, ((PlacementMode)this).MouseCoords.Position);
				Angle val = _eyeManager.CurrentEye.Rotation + worldRotation + Angle.op_Implicit(Math.PI / 2.0);
				Direction cardinalDir = ((Angle)(ref val)).GetCardinalDir();
				float num = (((int)cardinalDir == 4 || (int)cardinalDir == 0) ? (-1f) : 1f);
				((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawCircle(vector, 0.1f, _guideColor, true);
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Vector2 vector2 = new Vector2(num * (7f / 32f), 7f / 32f);
				((DrawingHandleBase)worldHandle).DrawCircle(vector + ((Angle)(ref worldRotation)).RotateVec(ref vector2), 0.1f, _guideColor, true);
				DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs)(ref args)).WorldHandle;
				vector2 = new Vector2(num * (7f / 32f), 7f / 32f);
				((DrawingHandleBase)worldHandle2).DrawCircle(vector - ((Angle)(ref worldRotation)).RotateVec(ref vector2), 0.1f, _guideColor, true);
			}
			((SnapgridCenter)this).Render(ref args);
		}
	}

	public override void AlignPlacementMode(ScreenCoordinates mouseScreen)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Invalid comparison between Unknown and I4
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Invalid comparison between Unknown and I4
		_unalignedMouseCoords = ((PlacementMode)this).ScreenToCursorGrid(mouseScreen);
		((SnapgridCenter)this).AlignPlacementMode(mouseScreen);
		if ((int)((PlacementMode)this).pManager.PlacementType != 0)
		{
			return;
		}
		((PlacementMode)this).MouseCoords = CoordinatesExtensions.AlignWithClosestGridTile(_unalignedMouseCoords, 2f, _entityManager, _mapManager);
		EntityUid? grid = _transformSystem.GetGrid(((PlacementMode)this).MouseCoords);
		MapGridComponent val = default(MapGridComponent);
		if (_entityManager.TryGetComponent<MapGridComponent>(grid, ref val))
		{
			Angle worldRotation = _transformSystem.GetWorldRotation(grid.Value);
			((PlacementMode)this).CurrentTile = _mapSystem.GetTileRef(grid.Value, val, ((PlacementMode)this).MouseCoords);
			float num = (((PlacementMode)this).GridDistancing = (int)val.TileSize);
			EntityUid entityId = ((PlacementMode)this).MouseCoords.EntityId;
			TileRef currentTile = ((PlacementMode)this).CurrentTile;
			float x = (float)((TileRef)(ref currentTile)).X + num / 2f + (float)((PlacementMode)this).pManager.PlacementOffset.X;
			currentTile = ((PlacementMode)this).CurrentTile;
			((PlacementMode)this).MouseCoords = new EntityCoordinates(entityId, new Vector2(x, (float)((TileRef)(ref currentTile)).Y + num / 2f + (float)((PlacementMode)this).pManager.PlacementOffset.Y));
			Vector2 vector = _unalignedMouseCoords.Position - ((PlacementMode)this).MouseCoords.Position;
			AtmosPipeLayer layer = AtmosPipeLayer.Primary;
			if (vector.Length() > 0.25f)
			{
				Angle val2 = new Angle(vector) + _eyeManager.CurrentEye.Rotation + worldRotation + Angle.op_Implicit(Math.PI / 2.0);
				Direction cardinalDir = ((Angle)(ref val2)).GetCardinalDir();
				layer = (((int)cardinalDir == 4 || (int)cardinalDir == 2) ? AtmosPipeLayer.Secondary : AtmosPipeLayer.Tertiary);
			}
			if (((PlacementMode)this).pManager.Hijack != null)
			{
				UpdateHijackedPlacer(layer, mouseScreen);
			}
			else
			{
				UpdatePlacer(layer);
			}
		}
	}

	private void UpdateHijackedPlacer(AtmosPipeLayer layer, ScreenCoordinates mouseScreen)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		ConstructionSystem constructionSystem = (((PlacementMode)this).pManager.Hijack as ConstructionPlacementHijack)?.CurrentConstructionSystem;
		ProtoId<ConstructionPrototype>[] array = (((PlacementMode)this).pManager.Hijack as ConstructionPlacementHijack)?.CurrentPrototype?.AlternativePrototypes;
		if (constructionSystem == null || array == null || (int)layer >= array.Length)
		{
			return;
		}
		ProtoId<ConstructionPrototype> val = array[(int)layer];
		ConstructionPrototype constructionPrototype = default(ConstructionPrototype);
		if (!_protoManager.TryIndex<ConstructionPrototype>(val, ref constructionPrototype))
		{
			return;
		}
		if (constructionPrototype.Type != ConstructionType.Structure)
		{
			((PlacementMode)this).pManager.Clear();
		}
		else if (!(constructionPrototype.ID == (((PlacementMode)this).pManager.Hijack as ConstructionPlacementHijack)?.CurrentPrototype?.ID))
		{
			((PlacementMode)this).pManager.BeginPlacing(new PlacementInformation
			{
				IsTile = false,
				PlacementOption = constructionPrototype.PlacementMode
			}, (PlacementHijack)(object)new ConstructionPlacementHijack(constructionSystem, constructionPrototype));
			if (((PlacementMode)this).pManager.CurrentMode is AlignAtmosPipeLayers alignAtmosPipeLayers)
			{
				alignAtmosPipeLayers.RefreshGrid(mouseScreen);
			}
			constructionSystem.GetGuide(constructionPrototype);
		}
	}

	private void UpdatePlacer(AtmosPipeLayer layer)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		PlacementInformation currentPermission = ((PlacementMode)this).pManager.CurrentPermission;
		EntityPrototype val = default(EntityPrototype);
		AtmosPipeLayersComponent component = default(AtmosPipeLayersComponent);
		EntityPrototype val2 = default(EntityPrototype);
		if (((currentPermission != null) ? currentPermission.EntityType : null) == null || !_protoManager.TryIndex<EntityPrototype>(((PlacementMode)this).pManager.CurrentPermission.EntityType, ref val) || !val.TryGetComponent<AtmosPipeLayersComponent>(ref component, _entityManager.ComponentFactory) || !_pipeLayersSystem.TryGetAlternativePrototype(component, layer, out var proto) || !_protoManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(proto), ref val2))
		{
			return;
		}
		((PlacementMode)this).pManager.CurrentPermission.EntityType = EntProtoId.op_Implicit(proto);
		SpriteComponent val3 = default(SpriteComponent);
		if (!val2.TryGetComponent<SpriteComponent>(ref val3, _entityManager.ComponentFactory))
		{
			return;
		}
		List<IDirectionalTextureProvider> list = new List<IDirectionalTextureProvider>();
		foreach (ISpriteLayer allLayer in val3.AllLayers)
		{
			RSI actualRsi = allLayer.ActualRsi;
			if (actualRsi != null)
			{
				_ = actualRsi.Path;
				if (true && allLayer.RsiState.Name != null)
				{
					list.Add((IDirectionalTextureProvider)(object)_spriteSystem.RsiStateLike((SpriteSpecifier)new Rsi(allLayer.ActualRsi.Path, allLayer.RsiState.Name)));
				}
			}
		}
		((PlacementMode)this).pManager.CurrentTextures = list;
	}

	private void RefreshGrid(ScreenCoordinates mouseScreen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((SnapgridCenter)this).AlignPlacementMode(mouseScreen);
	}
}
