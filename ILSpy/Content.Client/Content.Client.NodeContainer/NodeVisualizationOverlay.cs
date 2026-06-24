using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Content.Client.Resources;
using Content.Shared.NodeContainer;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.NodeContainer;

public sealed class NodeVisualizationOverlay : Overlay
{
	private sealed class NodeDisplayComparer : IComparer<(NodeVis.GroupData, NodeVis.NodeDatum)>
	{
		public static readonly NodeDisplayComparer Instance = new NodeDisplayComparer();

		public int Compare((NodeVis.GroupData, NodeVis.NodeDatum) x, (NodeVis.GroupData, NodeVis.NodeDatum) y)
		{
			(NodeVis.GroupData, NodeVis.NodeDatum) tuple = x;
			NodeVis.GroupData item = tuple.Item1;
			NodeVis.NodeDatum item2 = tuple.Item2;
			(NodeVis.GroupData, NodeVis.NodeDatum) tuple2 = y;
			NodeVis.GroupData item3 = tuple2.Item1;
			NodeVis.NodeDatum item4 = tuple2.Item2;
			int num = item.NetId.CompareTo(item3.NetId);
			if (num != 0)
			{
				return num;
			}
			return item2.NetId.CompareTo(item4.NetId);
		}
	}

	private sealed class NodeRenderData
	{
		public NodeVis.GroupData GroupData;

		public NodeVis.NodeDatum NodeDatum;

		public Vector2 NodePos;

		public NodeRenderData(NodeVis.GroupData groupData, NodeVis.NodeDatum nodeDatum, Vector2 nodePos)
		{
			GroupData = groupData;
			NodeDatum = nodeDatum;
			NodePos = nodePos;
		}
	}

	private readonly NodeGroupSystem _system;

	private readonly EntityLookupSystem _lookup;

	private readonly IMapManager _mapManager;

	private readonly IInputManager _inputManager;

	private readonly IEntityManager _entityManager;

	private readonly SharedTransformSystem _transformSystem;

	private readonly SharedMapSystem _mapSystem;

	private readonly Dictionary<(int, int), NodeRenderData> _nodeIndex = new Dictionary<(int, int), NodeRenderData>();

	private readonly Dictionary<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>> _gridIndex = new Dictionary<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>>();

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	private readonly Font _font;

	private Vector2 _mouseWorldPos;

	private (int group, int node)? _hovered;

	private float _time;

	public override OverlaySpace Space => (OverlaySpace)6;

	public NodeVisualizationOverlay(NodeGroupSystem system, EntityLookupSystem lookup, IMapManager mapManager, IInputManager inputManager, IResourceCache cache, IEntityManager entityManager)
	{
		_system = system;
		_lookup = lookup;
		_mapManager = mapManager;
		_inputManager = inputManager;
		_entityManager = entityManager;
		_transformSystem = _entityManager.System<SharedTransformSystem>();
		_mapSystem = _entityManager.System<SharedMapSystem>();
		_font = cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((args.Space & 4) != 0)
		{
			DrawWorld(in args);
		}
		else if ((args.Space & 2) != 0)
		{
			DrawScreen(in args);
		}
	}

	private void DrawScreen(in OverlayDrawArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = _inputManager.MouseScreenPosition.Position;
		_mouseWorldPos = args.ViewportControl.PixelToMap(position).Position;
		if (_hovered.HasValue)
		{
			(int group, int node) value = _hovered.Value;
			int item = value.group;
			int item2 = value.node;
			NodeVis.GroupData groupData = _system.Groups[item];
			NodeVis.NodeDatum nodeDatum = _system.NodeLookup[(item, item2)];
			TransformComponent component = _entityManager.GetComponent<TransformComponent>(_entityManager.GetEntity(nodeDatum.Entity));
			MapGridComponent item3 = default(MapGridComponent);
			if (_entityManager.TryGetComponent<MapGridComponent>(component.GridUid, ref item3))
			{
				Vector2i value2 = _mapSystem.TileIndicesFor(Entity<MapGridComponent>.op_Implicit((component.GridUid.Value, item3)), component.Coordinates);
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
				handler.AppendLiteral("entity: ");
				handler.AppendFormatted<NetEntity>(nodeDatum.Entity);
				handler.AppendLiteral("\n");
				stringBuilder3.Append(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder2);
				handler.AppendLiteral("group id: ");
				handler.AppendFormatted(groupData.GroupId);
				handler.AppendLiteral("\n");
				stringBuilder4.Append(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
				handler.AppendLiteral("node: ");
				handler.AppendFormatted(nodeDatum.Name);
				handler.AppendLiteral("\n");
				stringBuilder5.Append(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
				handler.AppendLiteral("type: ");
				handler.AppendFormatted(nodeDatum.Type);
				handler.AppendLiteral("\n");
				stringBuilder6.Append(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder7 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder2);
				handler.AppendLiteral("grid pos: ");
				handler.AppendFormatted<Vector2i>(value2);
				handler.AppendLiteral("\n");
				stringBuilder7.Append(ref handler);
				stringBuilder.Append(groupData.DebugData);
				((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, position + new Vector2(20f, -20f), stringBuilder.ToString());
			}
		}
	}

	private void DrawWorld(in OverlayDrawArgs overlayDrawArgs)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref overlayDrawArgs)).WorldHandle;
		IEye eye = overlayDrawArgs.Viewport.Eye;
		MapId val = (MapId)((eye != null) ? eye.Position.MapId : default(MapId));
		if (val == MapId.Nullspace)
		{
			return;
		}
		_hovered = null;
		Box2 val2 = Box2.CenteredAround(_mouseWorldPos, new Vector2(0.25f, 0.25f));
		Box2 worldAABB = overlayDrawArgs.WorldAABB;
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		_grids.Clear();
		_mapManager.FindGridsIntersecting(val, worldAABB, ref _grids, false, true);
		foreach (Entity<MapGridComponent> grid in _grids)
		{
			foreach (EntityUid item6 in _lookup.GetEntitiesIntersecting(Entity<MapGridComponent>.op_Implicit(grid), worldAABB, (LookupFlags)110))
			{
				if (!_system.Entities.TryGetValue(item6, out (NodeVis.GroupData, NodeVis.NodeDatum)[] value))
				{
					continue;
				}
				Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>> orNew = Extensions.GetOrNew<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>>(_gridIndex, Entity<MapGridComponent>.op_Implicit(grid));
				EntityCoordinates coordinates = entityQuery.GetComponent(item6).Coordinates;
				if (float.IsNaN(coordinates.Position.X) || float.IsNaN(coordinates.Position.Y))
				{
					continue;
				}
				List<(NodeVis.GroupData, NodeVis.NodeDatum)> orNew2 = Extensions.GetOrNew<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>(orNew, _mapSystem.TileIndicesFor(grid, coordinates));
				(NodeVis.GroupData, NodeVis.NodeDatum)[] array = value;
				for (int i = 0; i < array.Length; i++)
				{
					var (groupData, item) = array[i];
					if (!_system.Filtered.Contains(groupData.GroupId))
					{
						orNew2.Add((groupData, item));
					}
				}
			}
		}
		foreach (KeyValuePair<EntityUid, Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>>> item7 in _gridIndex)
		{
			item7.Deconstruct(out var key, out var value2);
			EntityUid val3 = key;
			Dictionary<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>> dictionary = value2;
			MapGridComponent component = _entityManager.GetComponent<MapGridComponent>(val3);
			ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transformSystem.GetWorldPositionRotationMatrixWithInv(val3);
			Matrix3x2 item2 = worldPositionRotationMatrixWithInv.Item3;
			Box2 val4 = Matrix3Helpers.TransformBox(worldPositionRotationMatrixWithInv.Item4, ref val2);
			foreach (KeyValuePair<Vector2i, List<(NodeVis.GroupData, NodeVis.NodeDatum)>> item8 in dictionary)
			{
				item8.Deconstruct(out var key2, out var value3);
				Vector2i val5 = key2;
				List<(NodeVis.GroupData, NodeVis.NodeDatum)> list = value3;
				Vector2 vector = Vector2i.op_Implicit(val5) + component.TileSizeHalfVector;
				list.Sort(NodeDisplayComparer.Instance);
				float num = (float)(-(list.Count - 1)) * 0.1875f / 2f;
				foreach (var item9 in list)
				{
					NodeVis.GroupData item3 = item9.Item1;
					NodeVis.NodeDatum item4 = item9.Item2;
					Vector2 vector2 = vector + new Vector2(num, num);
					if (((Box2)(ref val4)).Contains(vector2, true))
					{
						_hovered = (item3.NetId, item4.NetId);
					}
					_nodeIndex[(item3.NetId, item4.NetId)] = new NodeRenderData(item3, item4, vector2);
					num += 0.1875f;
				}
			}
			((DrawingHandleBase)worldHandle).SetTransform(ref item2);
			foreach (NodeRenderData value5 in _nodeIndex.Values)
			{
				Vector2 nodePos = value5.NodePos;
				Box2 val6 = Box2.CenteredAround(nodePos, new Vector2(0.25f, 0.25f));
				NodeVis.GroupData groupData2 = value5.GroupData;
				Color color = groupData2.Color;
				if (!_hovered.HasValue)
				{
					color.A = 0.5f;
				}
				else if (_hovered.Value.group != groupData2.NetId)
				{
					color.A = 0.2f;
				}
				else
				{
					color.A = 0.75f + MathF.Sin(_time * 4f) * 0.25f;
				}
				worldHandle.DrawRect(val6, color, true);
				int[] reachable = value5.NodeDatum.Reachable;
				foreach (int item5 in reachable)
				{
					if (_nodeIndex.TryGetValue((groupData2.NetId, item5), out NodeRenderData value4))
					{
						((DrawingHandleBase)worldHandle).DrawLine(nodePos, value4.NodePos, color);
					}
				}
			}
			_nodeIndex.Clear();
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		_gridIndex.Clear();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Overlay)this).FrameUpdate(args);
		_time += ((FrameEventArgs)(ref args)).DeltaSeconds;
	}
}
