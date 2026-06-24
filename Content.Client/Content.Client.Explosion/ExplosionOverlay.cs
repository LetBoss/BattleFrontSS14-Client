using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Explosion.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client.Explosion;

public sealed class ExplosionOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	[Dependency]
	private IRobustRandom _robustRandom;

	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private IPrototypeManager _proto;

	private readonly SharedTransformSystem _transformSystem;

	private SharedAppearanceSystem _appearance;

	private ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)8;

	public ExplosionOverlay(SharedAppearanceSystem appearanceSystem)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<ExplosionOverlay>(this);
		_shader = _proto.Index<ShaderPrototype>(UnshadedShader).Instance();
		_transformSystem = _entMan.System<SharedTransformSystem>();
		_appearance = appearanceSystem;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		EntityQuery<TransformComponent> entityQuery = _entMan.GetEntityQuery<TransformComponent>();
		EntityQueryEnumerator<ExplosionVisualsComponent, ExplosionVisualsTexturesComponent> val = _entMan.EntityQueryEnumerator<ExplosionVisualsComponent, ExplosionVisualsTexturesComponent>();
		EntityUid val2 = default(EntityUid);
		ExplosionVisualsComponent explosionVisualsComponent = default(ExplosionVisualsComponent);
		ExplosionVisualsTexturesComponent textures = default(ExplosionVisualsTexturesComponent);
		int num = default(int);
		while (val.MoveNext(ref val2, ref explosionVisualsComponent, ref textures))
		{
			if (!(explosionVisualsComponent.Epicenter.MapId != args.MapId) && _appearance.TryGetData<int>(val2, (Enum)ExplosionAppearanceData.Progress, ref num, (AppearanceComponent)null))
			{
				num = Math.Min(num, explosionVisualsComponent.Intensity.Count - 1);
				DrawExplosion(worldHandle, args.WorldBounds, explosionVisualsComponent, num, entityQuery, textures);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}

	private void DrawExplosion(DrawingHandleWorld drawHandle, Box2Rotated worldBounds, ExplosionVisualsComponent visuals, int index, EntityQuery<TransformComponent> xforms, ExplosionVisualsTexturesComponent textures)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent val3 = default(MapGridComponent);
		Box2 val4;
		foreach (var (val2, tileSets) in visuals.Tiles)
		{
			if (_entMan.TryGetComponent<MapGridComponent>(val2, ref val3))
			{
				TransformComponent component = xforms.GetComponent(val2);
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transformSystem.GetWorldPositionRotationMatrixWithInv(component, xforms);
				Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
				val4 = Matrix3Helpers.TransformBox(worldPositionRotationMatrixWithInv.Item4, ref worldBounds);
				Box2 gridBounds = ((Box2)(ref val4)).Enlarged((float)(val3.TileSize * 2));
				((DrawingHandleBase)drawHandle).SetTransform(ref item);
				DrawTiles(drawHandle, gridBounds, index, tileSets, visuals, val3.TileSize, textures);
			}
		}
		if (visuals.SpaceTiles != null)
		{
			Matrix3x2.Invert(visuals.SpaceMatrix, out var result);
			val4 = Matrix3Helpers.TransformBox(result, ref worldBounds);
			Box2 gridBounds = ((Box2)(ref val4)).Enlarged(2f);
			((DrawingHandleBase)drawHandle).SetTransform(ref visuals.SpaceMatrix);
			DrawTiles(drawHandle, gridBounds, index, visuals.SpaceTiles, visuals, visuals.SpaceTileSize, textures);
		}
	}

	private void DrawTiles(DrawingHandleWorld drawHandle, Box2 gridBounds, int index, Dictionary<int, List<Vector2i>> tileSets, ExplosionVisualsComponent visuals, ushort tileSize, ExplosionVisualsTexturesComponent textures)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i <= index; i++)
		{
			if (!tileSets.TryGetValue(i, out List<Vector2i> value))
			{
				continue;
			}
			int index2 = (int)Math.Min(visuals.Intensity[i] / textures.IntensityPerState, textures.FireFrames.Count - 1);
			Texture[] array = textures.FireFrames[index2];
			foreach (Vector2i item in value)
			{
				Vector2 vector = (Vector2i.op_Implicit(item) + Vector2Helpers.Half) * (int)tileSize;
				if (((Box2)(ref gridBounds)).Contains(vector, true))
				{
					Texture val = RandomExtensions.Pick<Texture>(_robustRandom, (IReadOnlyList<Texture>)array);
					drawHandle.DrawTextureRect(val, Box2.CenteredAround(vector, new Vector2((int)tileSize, (int)tileSize)), textures.FireColor);
				}
			}
		}
	}
}
