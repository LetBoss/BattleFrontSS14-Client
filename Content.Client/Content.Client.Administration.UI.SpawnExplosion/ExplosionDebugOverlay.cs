using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.SpawnExplosion;

public sealed class ExplosionDebugOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IEyeManager _eyeManager;

	public Dictionary<int, List<Vector2i>>? SpaceTiles;

	public Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> Tiles = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();

	public List<float> Intensity = new List<float>();

	public float TotalIntensity;

	public float Slope;

	public ushort SpaceTileSize;

	public Matrix3x2 SpaceMatrix;

	public MapId Map;

	private readonly Font _font;

	public override OverlaySpace Space => (OverlaySpace)6;

	public ExplosionDebugOverlay()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		IoCManager.InjectDependencies<ExplosionDebugOverlay>(this);
		IResourceCache val = IoCManager.Resolve<IResourceCache>();
		_font = (Font)new VectorFont(val.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Invalid comparison between Unknown and I4
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		MapId map = Map;
		IEye eye = args.Viewport.Eye;
		MapId? val = ((eye != null) ? new MapId?(eye.Position.MapId) : ((MapId?)null));
		if (!val.HasValue || map != val.GetValueOrDefault() || (Tiles.Count == 0 && SpaceTiles == null))
		{
			return;
		}
		OverlaySpace space = args.Space;
		if ((int)space != 2)
		{
			if ((int)space == 4)
			{
				DrawWorld(in args);
			}
		}
		else
		{
			DrawScreen(args);
		}
	}

	private void DrawScreen(OverlayDrawArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		TransformSystem val = _entityManager.System<TransformSystem>();
		MapGridComponent val4 = default(MapGridComponent);
		foreach (var (val3, tileSets) in Tiles)
		{
			if (_entityManager.TryGetComponent<MapGridComponent>(val3, ref val4))
			{
				TransformComponent component = entityQuery.GetComponent(val3);
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = ((SharedTransformSystem)val).GetWorldPositionRotationMatrixWithInv(component, entityQuery);
				Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
				Box2 val5 = Matrix3Helpers.TransformBox(worldPositionRotationMatrixWithInv.Item4, ref args.WorldBounds);
				Box2 gridBounds = ((Box2)(ref val5)).Enlarged((float)(val4.TileSize * 2));
				DrawText(screenHandle, gridBounds, item, tileSets, val4.TileSize);
			}
		}
		if (SpaceTiles != null)
		{
			Matrix3x2.Invert(SpaceMatrix, out var result);
			Box2 gridBounds = Matrix3Helpers.TransformBox(result, ref args.WorldBounds);
			DrawText(screenHandle, gridBounds, SpaceMatrix, SpaceTiles, SpaceTileSize);
		}
	}

	private void DrawText(DrawingHandleScreen handle, Box2 gridBounds, Matrix3x2 transform, Dictionary<int, List<Vector2i>> tileSets, ushort tileSize)
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 1; i < Intensity.Count; i++)
		{
			if (!tileSets.TryGetValue(i, out List<Vector2i> value))
			{
				continue;
			}
			foreach (Vector2i item in value)
			{
				Vector2 vector = (Vector2i.op_Implicit(item) + Vector2Helpers.Half) * (int)tileSize;
				if (((Box2)(ref gridBounds)).Contains(vector, true))
				{
					Vector2 vector2 = Vector2.Transform(vector, transform);
					Vector2 vector3 = _eyeManager.WorldToScreen(vector2);
					if (Intensity[i] > 9f)
					{
						vector3 += new Vector2(-12f, -8f);
					}
					else
					{
						vector3 += new Vector2(-8f, -8f);
					}
					handle.DrawString(_font, vector3, Intensity[i].ToString("F2"));
				}
			}
		}
		if (tileSets.TryGetValue(0, out List<Vector2i> value2))
		{
			Vector2 vector4 = Vector2.Transform((Vector2i.op_Implicit(value2.First()) + Vector2Helpers.Half) * (int)tileSize, transform);
			Vector2 vector5 = _eyeManager.WorldToScreen(vector4) + new Vector2(-24f, -24f);
			string text = $"{Intensity[0]:F2}\nΣ={TotalIntensity:F1}\nΔ={Slope:F1}";
			handle.DrawString(_font, vector5, text);
		}
	}

	private void DrawWorld(in OverlayDrawArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		TransformSystem val = _entityManager.System<TransformSystem>();
		MapGridComponent val4 = default(MapGridComponent);
		Box2 val5;
		foreach (var (val3, tileSets) in Tiles)
		{
			if (_entityManager.TryGetComponent<MapGridComponent>(val3, ref val4))
			{
				TransformComponent component = entityQuery.GetComponent(val3);
				ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = ((SharedTransformSystem)val).GetWorldPositionRotationMatrixWithInv(component, entityQuery);
				Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
				val5 = Matrix3Helpers.TransformBox(worldPositionRotationMatrixWithInv.Item4, ref args.WorldBounds);
				Box2 gridBounds = ((Box2)(ref val5)).Enlarged((float)(val4.TileSize * 2));
				((DrawingHandleBase)worldHandle).SetTransform(ref item);
				DrawTiles(worldHandle, gridBounds, tileSets, SpaceTileSize);
			}
		}
		if (SpaceTiles != null)
		{
			Matrix3x2.Invert(SpaceMatrix, out var result);
			val5 = Matrix3Helpers.TransformBox(result, ref args.WorldBounds);
			Box2 gridBounds = ((Box2)(ref val5)).Enlarged(2f);
			((DrawingHandleBase)worldHandle).SetTransform(ref SpaceMatrix);
			DrawTiles(worldHandle, gridBounds, SpaceTiles, SpaceTileSize);
			Matrix3x2 identity = Matrix3x2.Identity;
			((DrawingHandleBase)worldHandle).SetTransform(ref identity);
		}
	}

	private void DrawTiles(DrawingHandleWorld handle, Box2 gridBounds, Dictionary<int, List<Vector2i>> tileSets, ushort tileSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Intensity.Count; i++)
		{
			Color val = ColorMap(Intensity[i]);
			Color val2 = val;
			val2.A = 0.2f;
			if (!tileSets.TryGetValue(i, out List<Vector2i> value))
			{
				continue;
			}
			foreach (Vector2i item in value)
			{
				Vector2 vector = (Vector2i.op_Implicit(item) + Vector2Helpers.Half) * (int)tileSize;
				if (((Box2)(ref gridBounds)).Contains(vector, true))
				{
					Box2 val3 = ((Box2)(ref Box2.UnitCentered)).Translated(vector);
					handle.DrawRect(val3, val, false);
					handle.DrawRect(val3, val2, true);
				}
			}
		}
	}

	private Color ColorMap(float intensity)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - intensity / Intensity[0];
		if (num < 0.5f)
		{
			return Color.InterpolateBetween(Color.Red, Color.Orange, num * 2f);
		}
		return Color.InterpolateBetween(Color.Orange, Color.Yellow, (num - 0.5f) * 2f);
	}
}
