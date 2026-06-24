using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Light;

public sealed class RoofOverlay : Overlay
{
	private const int LeakDepth = 2;

	private static readonly Vector2i[] LeakDirs = (Vector2i[])(object)new Vector2i[4]
	{
		new Vector2i(0, -1),
		new Vector2i(1, 0),
		new Vector2i(0, 1),
		new Vector2i(-1, 0)
	};

	private readonly IEntityManager _entManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IOverlayManager _overlay;

	private readonly EntityLookupSystem _lookup;

	private readonly SharedMapSystem _mapSystem;

	private readonly SharedRoofSystem _roof;

	private readonly SharedTransformSystem _xformSystem;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	private readonly Queue<(Vector2i Tile, int Depth)> _open = new Queue<(Vector2i, int)>();

	private readonly Dictionary<Vector2i, int> _leaks = new Dictionary<Vector2i, int>();

	private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();

	public const int ContentZIndex = -9;

	public override OverlaySpace Space => (OverlaySpace)512;

	public RoofOverlay(IEntityManager entManager)
	{
		_entManager = entManager;
		IoCManager.InjectDependencies<RoofOverlay>(this);
		_lookup = _entManager.System<EntityLookupSystem>();
		_mapSystem = _entManager.System<SharedMapSystem>();
		_roof = _entManager.System<SharedRoofSystem>();
		_xformSystem = _entManager.System<SharedTransformSystem>();
		((Overlay)this).ZIndex = -9;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye == null || !_entManager.HasComponent<MapLightComponent>(args.MapUid))
		{
			return;
		}
		IClydeViewport viewport = args.Viewport;
		IEye eye = args.Viewport.Eye;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		BeforeLightTargetOverlay overlay = _overlay.GetOverlay<BeforeLightTargetOverlay>();
		Box2Rotated bounds = overlay.EnlargedBounds;
		IRenderTexture target = overlay.EnlargedLightTarget;
		_grids.Clear();
		_mapManager.FindGridsIntersecting(args.MapId, bounds, ref _grids, true, true);
		Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
		Vector2 scale = viewport.RenderScale / (Vector2.One / vector);
		((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)target, (Action)delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			Matrix3x2 worldToLocalMatrix = ((IRenderTarget)target).GetWorldToLocalMatrix(eye, scale);
			ImplicitRoofComponent implicitRoofComponent = default(ImplicitRoofComponent);
			TileRef val2 = default(TileRef);
			for (int i = 0; i < _grids.Count; i++)
			{
				Entity<MapGridComponent> val = _grids[i];
				if (_entManager.TryGetComponent<ImplicitRoofComponent>(val.Owner, ref implicitRoofComponent))
				{
					Matrix3x2 matrix3x = Matrix3x2.Multiply(_xformSystem.GetWorldMatrix(val.Owner), worldToLocalMatrix);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					TilesEnumerator tilesEnumerator = _mapSystem.GetTilesEnumerator(val.Owner, Entity<MapGridComponent>.op_Implicit(val), bounds, true, (Predicate<TileRef>)null);
					Color color = implicitRoofComponent.Color;
					while (((TilesEnumerator)(ref tilesEnumerator)).MoveNext(ref val2))
					{
						Box2 localBounds = _lookup.GetLocalBounds(val2, val.Comp.TileSize);
						worldHandle.DrawRect(localBounds, color, true);
					}
					_grids.RemoveAt(i);
					i--;
				}
			}
		}, (Color?)null);
		((DrawingHandleBase)worldHandle).RenderInRenderTarget((IRenderTarget)(object)target, (Action)delegate
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			Matrix3x2 worldToLocalMatrix = ((IRenderTarget)target).GetWorldToLocalMatrix(eye, scale);
			RoofComponent item = default(RoofComponent);
			TileRef val = default(TileRef);
			foreach (Entity<MapGridComponent> grid in _grids)
			{
				if (_entManager.TryGetComponent<RoofComponent>(grid.Owner, ref item))
				{
					Matrix3x2 matrix3x = Matrix3x2.Multiply(_xformSystem.GetWorldMatrix(grid.Owner), worldToLocalMatrix);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					TilesEnumerator tilesEnumerator = _mapSystem.GetTilesEnumerator(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), bounds, true, (Predicate<TileRef>)null);
					(EntityUid, MapGridComponent, RoofComponent) tuple = (grid.Owner, grid.Comp, item);
					FillLeaks(Entity<MapGridComponent, RoofComponent>.op_Implicit(tuple), bounds);
					while (((TilesEnumerator)(ref tilesEnumerator)).MoveNext(ref val))
					{
						Color? val2 = _roof.GetColor(Entity<MapGridComponent, RoofComponent>.op_Implicit(tuple), val.GridIndices);
						if (val2.HasValue)
						{
							if (_leaks.TryGetValue(val.GridIndices, out var value))
							{
								Color value2 = val2.Value;
								val2 = ((Color)(ref value2)).WithAlpha(val2.Value.A * GetLeakAlpha(value));
							}
							Box2 localBounds = _lookup.GetLocalBounds(val, grid.Comp.TileSize);
							worldHandle.DrawRect(localBounds, val2.Value, true);
						}
					}
				}
			}
		}, (Color?)null);
		DrawingHandleWorld obj = worldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj).SetTransform(ref identity);
	}

	private void FillLeaks(Entity<MapGridComponent, RoofComponent> roof, Box2Rotated bounds)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		_open.Clear();
		_leaks.Clear();
		TilesEnumerator tilesEnumerator = _mapSystem.GetTilesEnumerator(roof.Owner, roof.Comp1, bounds, true, (Predicate<TileRef>)null);
		TileRef val = default(TileRef);
		while (((TilesEnumerator)(ref tilesEnumerator)).MoveNext(ref val))
		{
			Vector2i gridIndices = val.GridIndices;
			if (_roof.GetColor(roof, gridIndices).HasValue || BlocksLeak(roof.Owner, gridIndices))
			{
				continue;
			}
			Vector2i[] leakDirs = LeakDirs;
			foreach (Vector2i val2 in leakDirs)
			{
				Vector2i val3 = gridIndices + val2;
				if (!_leaks.ContainsKey(val3) && _roof.GetColor(roof, val3).HasValue && !BlocksLeak(roof.Owner, val3))
				{
					_open.Enqueue((val3, 1));
				}
			}
		}
		(Vector2i, int) result;
		while (_open.TryDequeue(out result))
		{
			if (_leaks.ContainsKey(result.Item1))
			{
				continue;
			}
			_leaks[result.Item1] = result.Item2;
			if (result.Item2 >= 2)
			{
				continue;
			}
			Vector2i[] leakDirs = LeakDirs;
			foreach (Vector2i val4 in leakDirs)
			{
				Vector2i val5 = result.Item1 + val4;
				if (!_leaks.ContainsKey(val5) && _roof.GetColor(roof, val5).HasValue && !BlocksLeak(roof.Owner, val5))
				{
					_open.Enqueue((val5, result.Item2 + 1));
				}
			}
		}
	}

	private bool BlocksLeak(EntityUid gridUid, Vector2i tile)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		_occluders.Clear();
		_lookup.GetLocalEntitiesIntersecting<OccluderComponent>(gridUid, tile, _occluders, -0.04f, (LookupFlags)110, (MapGridComponent)null);
		foreach (Entity<OccluderComponent> occluder in _occluders)
		{
			if (occluder.Comp.Enabled)
			{
				return true;
			}
		}
		return false;
	}

	private static float GetLeakAlpha(int depth)
	{
		return depth switch
		{
			1 => 0.15f, 
			2 => 0.4f, 
			_ => 0.55f, 
		};
	}
}
