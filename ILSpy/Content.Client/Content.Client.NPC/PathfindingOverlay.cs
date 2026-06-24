using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Content.Shared.NPC;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.NPC;

public sealed class PathfindingOverlay : Overlay
{
	private readonly IEntityManager _entManager;

	private readonly IEyeManager _eyeManager;

	private readonly IInputManager _inputManager;

	private readonly IMapManager _mapManager;

	private readonly PathfindingSystem _system;

	private readonly MapSystem _mapSystem;

	private readonly SharedTransformSystem _transformSystem;

	private readonly Font _font;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public override OverlaySpace Space => (OverlaySpace)6;

	public PathfindingOverlay(IEntityManager entManager, IEyeManager eyeManager, IInputManager inputManager, IMapManager mapManager, IResourceCache cache, PathfindingSystem system, MapSystem mapSystem, SharedTransformSystem transformSystem)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		_entManager = entManager;
		_eyeManager = eyeManager;
		_inputManager = inputManager;
		_mapManager = mapManager;
		_system = system;
		_mapSystem = mapSystem;
		_transformSystem = transformSystem;
		_font = (Font)new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleBase drawingHandle = args.DrawingHandle;
		DrawingHandleScreen val = (DrawingHandleScreen)(object)((drawingHandle is DrawingHandleScreen) ? drawingHandle : null);
		if (val == null)
		{
			DrawingHandleWorld val2 = (DrawingHandleWorld)(object)((drawingHandle is DrawingHandleWorld) ? drawingHandle : null);
			if (val2 != null)
			{
				DrawWorld(args, val2);
			}
		}
		else
		{
			DrawScreen(args, val);
		}
	}

	private unsafe void DrawScreen(OverlayDrawArgs args, DrawingHandleScreen screenHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
		Box2 val2 = default(Box2);
		((Box2)(ref val2))._002Ector(val.Position - SharedPathfindingSystem.ChunkSizeVec, val.Position + SharedPathfindingSystem.ChunkSizeVec);
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		if ((_system.Modes & PathfindingDebugMode.Crumb) != PathfindingDebugMode.None && val.MapId == args.MapId)
		{
			bool flag = false;
			_grids.Clear();
			_mapManager.FindGridsIntersecting(val.MapId, val2, ref _grids, false, true);
			TransformComponent val3 = default(TransformComponent);
			Box2 val7 = default(Box2);
			foreach (Entity<MapGridComponent> grid in _grids)
			{
				NetEntity netEntity = _entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent)null);
				if (flag || !_system.Breadcrumbs.TryGetValue(netEntity, out Dictionary<Vector2i, List<PathfindingBreadcrumb>> value) || !entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref val3))
				{
					continue;
				}
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transformSystem.GetWorldPositionRotationMatrixWithInv(val3);
				Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
				Matrix3x2 item2 = worldPositionRotationMatrixWithInv.Item4;
				Box2 val4 = ((Box2)(ref val2)).Enlarged(-8f);
				Box2 val5 = Matrix3Helpers.TransformBox(item2, ref val4);
				foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> item3 in value)
				{
					if (flag)
					{
						continue;
					}
					Vector2i val6 = item3.Key * 8;
					((Box2)(ref val7))._002Ector(Vector2i.op_Implicit(val6), Vector2i.op_Implicit(val6 + 8));
					if (!((Box2)(ref val7)).Intersects(ref val5))
					{
						continue;
					}
					PathfindingBreadcrumb? pathfindingBreadcrumb = null;
					float num = float.MaxValue;
					foreach (PathfindingBreadcrumb item4 in item3.Value)
					{
						float num2 = (Vector2.Transform(_system.GetCoordinate(item3.Key, item4.Coordinates), item) - val.Position).Length();
						if (num2 < num)
						{
							num = num2;
							pathfindingBreadcrumb = item4;
						}
					}
					if (!pathfindingBreadcrumb.HasValue)
					{
						continue;
					}
					StringBuilder stringBuilder = new StringBuilder();
					PathfindingBreadcrumb value2 = pathfindingBreadcrumb.Value;
					string value3 = "Point coordinates: " + ((object)(*(Vector2i*)(&value2.Coordinates))/*cast due to constrained. prefix*/).ToString();
					string value4 = "Grid coordinates: " + _system.GetCoordinate(item3.Key, pathfindingBreadcrumb.Value.Coordinates);
					string value5 = "Layer: " + pathfindingBreadcrumb.Value.Data.CollisionLayer;
					string value6 = "Mask: " + pathfindingBreadcrumb.Value.Data.CollisionMask;
					stringBuilder.AppendLine(value3);
					stringBuilder.AppendLine(value4);
					stringBuilder.AppendLine(value5);
					stringBuilder.AppendLine(value6);
					stringBuilder.AppendLine("Flags:");
					PathfindingBreadcrumbFlag[] values = Enum.GetValues<PathfindingBreadcrumbFlag>();
					for (int i = 0; i < values.Length; i++)
					{
						PathfindingBreadcrumbFlag pathfindingBreadcrumbFlag = values[i];
						if ((pathfindingBreadcrumbFlag & pathfindingBreadcrumb.Value.Data.Flags) != PathfindingBreadcrumbFlag.None)
						{
							string value7 = "- " + pathfindingBreadcrumbFlag;
							stringBuilder.AppendLine(value7);
						}
					}
					screenHandle.DrawString(_font, mouseScreenPosition.Position, stringBuilder.ToString());
					flag = true;
					break;
				}
			}
		}
		EntityUid val8 = default(EntityUid);
		MapGridComponent val9 = default(MapGridComponent);
		TransformComponent val10 = default(TransformComponent);
		if ((_system.Modes & PathfindingDebugMode.Poly) == 0 || !(val.MapId == args.MapId) || !_mapManager.TryFindGridAt(val, ref val8, ref val9) || !entityQuery.TryGetComponent(val8, ref val10) || !_system.Polys.TryGetValue(_entManager.GetNetEntity(val8, (MetaDataComponent)null), out Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> value8))
		{
			return;
		}
		Vector2i gridIndices = ((SharedMapSystem)_mapSystem).GetTileRef(val8, val9, val).GridIndices;
		Vector2i key = gridIndices / 8;
		if (!value8.TryGetValue(key, out var value9) || !value9.TryGetValue(new Vector2i(gridIndices.X % 8, gridIndices.Y % 8), out var value10))
		{
			return;
		}
		Matrix3x2 invWorldMatrix = _transformSystem.GetInvWorldMatrix(val10);
		DebugPathPoly debugPathPoly = null;
		foreach (DebugPathPoly item5 in value10)
		{
			if (((Box2)(ref item5.Box)).Contains(Vector2.Transform(val.Position, invWorldMatrix), true))
			{
				debugPathPoly = item5;
				break;
			}
		}
		if (debugPathPoly != null)
		{
			new StringBuilder();
		}
	}

	private void DrawWorld(OverlayDrawArgs args, DrawingHandleWorld worldHandle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0910: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
		Box2 val2 = default(Box2);
		((Box2)(ref val2))._002Ector(val.Position - Vector2.One / 4f, val.Position + Vector2.One / 4f);
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		if ((_system.Modes & PathfindingDebugMode.Breadcrumbs) != PathfindingDebugMode.None && val.MapId == args.MapId)
		{
			_grids.Clear();
			_mapManager.FindGridsIntersecting(val.MapId, val2, ref _grids, false, true);
			TransformComponent val3 = default(TransformComponent);
			Box2 val6 = default(Box2);
			foreach (Entity<MapGridComponent> grid in _grids)
			{
				NetEntity netEntity = _entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid), (MetaDataComponent)null);
				if (!_system.Breadcrumbs.TryGetValue(netEntity, out Dictionary<Vector2i, List<PathfindingBreadcrumb>> value) || !entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid), ref val3))
				{
					continue;
				}
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transformSystem.GetWorldPositionRotationMatrixWithInv(val3);
				Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
				Matrix3x2 item2 = worldPositionRotationMatrixWithInv.Item4;
				((DrawingHandleBase)worldHandle).SetTransform(ref item);
				Box2 val4 = Matrix3Helpers.TransformBox(item2, ref val2);
				foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> item9 in value)
				{
					Vector2i val5 = item9.Key * 8;
					((Box2)(ref val6))._002Ector(Vector2i.op_Implicit(val5), Vector2i.op_Implicit(val5 + 8));
					if (!((Box2)(ref val6)).Intersects(ref val4))
					{
						continue;
					}
					foreach (PathfindingBreadcrumb item10 in item9.Value)
					{
						if (!item10.Equals(PathfindingBreadcrumb.Invalid))
						{
							Vector2 vector = new Vector2(0.0625f, 0.0625f);
							bool flag = item10.Data.CollisionMask != 0 || item10.Data.CollisionLayer != 0;
							Color val7 = (((item10.Data.Flags & PathfindingBreadcrumbFlag.Space) != PathfindingBreadcrumbFlag.None) ? Color.Green : ((!flag) ? Color.Orange : Color.Blue));
							Vector2 coordinate = _system.GetCoordinate(item9.Key, item10.Coordinates);
							worldHandle.DrawRect(new Box2(coordinate - vector, coordinate + vector), ((Color)(ref val7)).WithAlpha(0.25f), true);
						}
					}
				}
			}
		}
		Color val12;
		if ((_system.Modes & PathfindingDebugMode.Polys) != PathfindingDebugMode.None && val.MapId == args.MapId)
		{
			_grids.Clear();
			_mapManager.FindGridsIntersecting(args.MapId, val2, ref _grids, false, true);
			TransformComponent val8 = default(TransformComponent);
			Box2 val11 = default(Box2);
			foreach (Entity<MapGridComponent> grid2 in _grids)
			{
				NetEntity netEntity2 = _entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid2), (MetaDataComponent)null);
				if (!_system.Polys.TryGetValue(netEntity2, out Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> value2) || !entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid2), ref val8))
				{
					continue;
				}
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv2 = _transformSystem.GetWorldPositionRotationMatrixWithInv(val8);
				Matrix3x2 item3 = worldPositionRotationMatrixWithInv2.Item3;
				Matrix3x2 item4 = worldPositionRotationMatrixWithInv2.Item4;
				((DrawingHandleBase)worldHandle).SetTransform(ref item3);
				Box2 val9 = Matrix3Helpers.TransformBox(item4, ref val2);
				foreach (KeyValuePair<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> item11 in value2)
				{
					Vector2i val10 = item11.Key * 8;
					((Box2)(ref val11))._002Ector(Vector2i.op_Implicit(val10), Vector2i.op_Implicit(val10 + 8));
					if (!((Box2)(ref val11)).Intersects(ref val9))
					{
						continue;
					}
					foreach (KeyValuePair<Vector2i, List<DebugPathPoly>> item12 in item11.Value)
					{
						foreach (DebugPathPoly item13 in item12.Value)
						{
							Box2 box = item13.Box;
							val12 = Color.Green;
							worldHandle.DrawRect(box, ((Color)(ref val12)).WithAlpha(0.25f), true);
							worldHandle.DrawRect(item13.Box, Color.Red, false);
						}
					}
				}
			}
		}
		if ((_system.Modes & PathfindingDebugMode.PolyNeighbors) != PathfindingDebugMode.None && val.MapId == args.MapId)
		{
			_grids.Clear();
			_mapManager.FindGridsIntersecting(args.MapId, val2, ref _grids, false, true);
			TransformComponent val13 = default(TransformComponent);
			Box2 val16 = default(Box2);
			foreach (Entity<MapGridComponent> grid3 in _grids)
			{
				NetEntity netEntity3 = _entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid3), (MetaDataComponent)null);
				if (!_system.Polys.TryGetValue(netEntity3, out Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> value3) || !entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid3), ref val13))
				{
					continue;
				}
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv3 = _transformSystem.GetWorldPositionRotationMatrixWithInv(val13);
				Matrix3x2 item5 = worldPositionRotationMatrixWithInv3.Item3;
				Matrix3x2 item6 = worldPositionRotationMatrixWithInv3.Item4;
				((DrawingHandleBase)worldHandle).SetTransform(ref item5);
				Box2 val14 = Matrix3Helpers.TransformBox(item6, ref val2);
				foreach (KeyValuePair<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> item14 in value3)
				{
					Vector2i val15 = item14.Key * 8;
					((Box2)(ref val16))._002Ector(Vector2i.op_Implicit(val15), Vector2i.op_Implicit(val15 + 8));
					if (!((Box2)(ref val16)).Intersects(ref val14))
					{
						continue;
					}
					foreach (KeyValuePair<Vector2i, List<DebugPathPoly>> item15 in item14.Value)
					{
						foreach (DebugPathPoly item16 in item15.Value)
						{
							foreach (NetCoordinates neighbor in item16.Neighbors)
							{
								Color val17;
								Vector2 vector2;
								if (neighbor.NetEntity != item16.GraphUid)
								{
									val17 = Color.Green;
									MapCoordinates val18 = _transformSystem.ToMapCoordinates(_entManager.GetCoordinates(neighbor), true);
									if (val18.MapId != args.MapId)
									{
										continue;
									}
									vector2 = Vector2.Transform(val18.Position, item6);
								}
								else
								{
									val17 = Color.Blue;
									vector2 = neighbor.Position;
								}
								((DrawingHandleBase)worldHandle).DrawLine(((Box2)(ref item16.Box)).Center, vector2, val17);
							}
						}
					}
				}
			}
		}
		if ((_system.Modes & PathfindingDebugMode.Chunks) != PathfindingDebugMode.None)
		{
			_grids.Clear();
			_mapManager.FindGridsIntersecting(args.MapId, args.WorldBounds, ref _grids, false, true);
			TransformComponent val19 = default(TransformComponent);
			Box2 val22 = default(Box2);
			foreach (Entity<MapGridComponent> grid4 in _grids)
			{
				NetEntity netEntity4 = _entManager.GetNetEntity(Entity<MapGridComponent>.op_Implicit(grid4), (MetaDataComponent)null);
				if (!_system.Breadcrumbs.TryGetValue(netEntity4, out Dictionary<Vector2i, List<PathfindingBreadcrumb>> value4) || !entityQuery.TryGetComponent(Entity<MapGridComponent>.op_Implicit(grid4), ref val19))
				{
					continue;
				}
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv4 = _transformSystem.GetWorldPositionRotationMatrixWithInv(val19);
				Matrix3x2 item7 = worldPositionRotationMatrixWithInv4.Item3;
				Matrix3x2 item8 = worldPositionRotationMatrixWithInv4.Item4;
				((DrawingHandleBase)worldHandle).SetTransform(ref item7);
				Box2 val20 = Matrix3Helpers.TransformBox(item8, ref args.WorldBounds);
				foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> item17 in value4)
				{
					Vector2i val21 = item17.Key * 8;
					((Box2)(ref val22))._002Ector(Vector2i.op_Implicit(val21), Vector2i.op_Implicit(val21 + 8));
					if (((Box2)(ref val22)).Intersects(ref val20))
					{
						worldHandle.DrawRect(val22, Color.Red, false);
					}
				}
			}
		}
		Matrix3x2 worldMatrix;
		if ((_system.Modes & PathfindingDebugMode.Routes) != PathfindingDebugMode.None)
		{
			TransformComponent val23 = default(TransformComponent);
			foreach (var route in _system.Routes)
			{
				foreach (DebugPathPoly item18 in route.Message.Path)
				{
					if (_entManager.TryGetComponent<TransformComponent>(_entManager.GetEntity(item18.GraphUid), ref val23))
					{
						worldMatrix = _transformSystem.GetWorldMatrix(val23);
						((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
						Box2 box2 = item18.Box;
						val12 = Color.Orange;
						worldHandle.DrawRect(box2, ((Color)(ref val12)).WithAlpha(0.1f), true);
					}
				}
			}
		}
		if ((_system.Modes & PathfindingDebugMode.RouteCosts) != PathfindingDebugMode.None)
		{
			EntityUid val24 = EntityUid.Invalid;
			TransformComponent val25 = default(TransformComponent);
			foreach (var route2 in _system.Routes)
			{
				float num = route2.Message.Costs.Values.Max();
				foreach (KeyValuePair<DebugPathPoly, float> cost in route2.Message.Costs)
				{
					cost.Deconstruct(out var key, out var value5);
					DebugPathPoly debugPathPoly = key;
					float num2 = value5;
					EntityUid entity = _entManager.GetEntity(debugPathPoly.GraphUid);
					if (val24 != entity)
					{
						if (!_entManager.TryGetComponent<TransformComponent>(entity, ref val25))
						{
							continue;
						}
						val24 = entity;
						worldMatrix = _transformSystem.GetWorldMatrix(val25);
						((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
					}
					worldHandle.DrawRect(debugPathPoly.Box, new Color(0f, num2 / num, 1f - num2 / num, 0.1f), true);
				}
			}
		}
		worldMatrix = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
	}
}
