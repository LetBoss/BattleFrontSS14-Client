using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Light.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Light;

public sealed class TileEmissionOverlay : Overlay
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IOverlayManager _overlay;

	private SharedMapSystem _mapSystem;

	private SharedTransformSystem _xformSystem;

	private readonly EntityLookupSystem _lookup;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly HashSet<Entity<TileEmissionComponent>> _entities = new HashSet<Entity<TileEmissionComponent>>();

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public const int ContentZIndex = -8;

	public override OverlaySpace Space => (OverlaySpace)512;

	public TileEmissionOverlay(IEntityManager entManager)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<TileEmissionOverlay>(this);
		_lookup = entManager.System<EntityLookupSystem>();
		_mapSystem = entManager.System<SharedMapSystem>();
		_xformSystem = entManager.System<SharedTransformSystem>();
		_xformQuery = entManager.GetEntityQuery<TransformComponent>();
		((Overlay)this).ZIndex = -8;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye == null)
		{
			return;
		}
		MapId mapId = args.MapId;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		BeforeLightTargetOverlay overlay = _overlay.GetOverlay<BeforeLightTargetOverlay>();
		Box2Rotated bounds = overlay.EnlargedBounds;
		IRenderTexture target = overlay.EnlargedLightTarget;
		IClydeViewport viewport = args.Viewport;
		_grids.Clear();
		_mapManager.FindGridsIntersecting(mapId, bounds, ref _grids, true, true);
		if (_grids.Count == 0)
		{
			return;
		}
		Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
		Vector2 scale = viewport.RenderScale / (Vector2.One / vector);
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)target, (Action)delegate
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			Matrix3x2 worldToLocalMatrix = ((IRenderTarget)target).GetWorldToLocalMatrix(viewport.Eye, scale);
			foreach (Entity<MapGridComponent> grid in _grids)
			{
				Box2 val = Matrix3Helpers.TransformBox(_xformSystem.GetInvWorldMatrix(Entity<MapGridComponent>.op_Implicit(grid)), ref bounds);
				_entities.Clear();
				_lookup.GetLocalEntitiesIntersecting<TileEmissionComponent>(grid.Owner, val, _entities, (LookupFlags)110);
				if (_entities.Count != 0)
				{
					Matrix3x2 worldMatrix = _xformSystem.GetWorldMatrix(grid.Owner);
					foreach (Entity<TileEmissionComponent> entity in _entities)
					{
						TransformComponent val2 = _xformQuery.Comp(Entity<TileEmissionComponent>.op_Implicit(entity));
						Vector2i val3 = _mapSystem.LocalToTile(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), val2.Coordinates);
						Matrix3x2 matrix3x = Matrix3x2.Multiply(worldMatrix, worldToLocalMatrix);
						((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
						Box2 localBounds = _lookup.GetLocalBounds(val3, grid.Comp.TileSize);
						Box2 val4 = ((Box2)(ref localBounds)).Enlarged(entity.Comp.Range);
						worldHandle.DrawRect(val4, entity.Comp.Color, true);
					}
				}
			}
		}, (Color?)null);
	}
}
