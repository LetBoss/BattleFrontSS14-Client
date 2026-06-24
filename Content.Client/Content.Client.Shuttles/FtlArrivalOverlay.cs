using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Shuttles.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Shuttles;

public sealed class FtlArrivalOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	private EntityLookupSystem _lookups;

	private SharedMapSystem _maps;

	private SharedTransformSystem _transforms;

	private SpriteSystem _sprites;

	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _protos;

	private readonly HashSet<Entity<FtlVisualizerComponent>> _visualizers = new HashSet<Entity<FtlVisualizerComponent>>();

	private ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)8;

	public FtlArrivalOverlay()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<FtlArrivalOverlay>(this);
		_lookups = _entManager.System<EntityLookupSystem>();
		_transforms = _entManager.System<SharedTransformSystem>();
		_maps = _entManager.System<SharedMapSystem>();
		_sprites = _entManager.System<SpriteSystem>();
		_shader = _protos.Index<ShaderPrototype>(UnshadedShader).Instance();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_visualizers.Clear();
		_lookups.GetEntitiesOnMap<FtlVisualizerComponent>(args.MapId, _visualizers);
		return _visualizers.Count > 0;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).UseShader(_shader);
		EntityUid val = default(EntityUid);
		FtlVisualizerComponent ftlVisualizerComponent = default(FtlVisualizerComponent);
		MapGridComponent val3 = default(MapGridComponent);
		TileRef val5 = default(TileRef);
		foreach (Entity<FtlVisualizerComponent> visualizer in _visualizers)
		{
			visualizer.Deconstruct(ref val, ref ftlVisualizerComponent);
			EntityUid val2 = val;
			FtlVisualizerComponent ftlVisualizerComponent2 = ftlVisualizerComponent;
			EntityUid grid = ftlVisualizerComponent2.Grid;
			if (_entManager.TryGetComponent<MapGridComponent>(grid, ref val3))
			{
				Texture frame = _sprites.GetFrame((SpriteSpecifier)(object)ftlVisualizerComponent2.Sprite, TimeSpan.FromSeconds(ftlVisualizerComponent2.Elapsed), false);
				ftlVisualizerComponent2.Elapsed += (float)_timing.FrameTime.TotalSeconds;
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transforms.GetWorldPositionRotationMatrixWithInv(val2);
				Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
				Matrix3x2 item2 = worldPositionRotationMatrixWithInv.Item4;
				((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).SetTransform(ref item);
				Box2 val4 = Matrix3Helpers.TransformBox(item2, ref args.WorldBounds);
				TilesEnumerator localTilesEnumerator = _maps.GetLocalTilesEnumerator(grid, val3, val4, true, (Predicate<TileRef>)null);
				while (((TilesEnumerator)(ref localTilesEnumerator)).MoveNext(ref val5))
				{
					Box2 localBounds = _lookups.GetLocalBounds(val5, val3.TileSize);
					((OverlayDrawArgs)(ref args)).WorldHandle.DrawTextureRect(frame, localBounds, (Color?)null);
				}
			}
		}
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).UseShader((ShaderInstance)null);
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}
}
