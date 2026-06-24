using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Roof;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Roof;

public sealed class CivRoofCrackOverlay : Overlay
{
	private static readonly Color LeakColor = Color.FromHex((ReadOnlySpan<char>)"#FFF4C8", (Color?)null);

	private static readonly Color CoreColor = Color.White;

	private readonly IEntityManager _ent;

	[Dependency]
	private IMapManager _maps;

	private readonly EntityLookupSystem _lookup;

	private readonly SharedMapSystem _map;

	private readonly SharedRoofSystem _roof;

	private readonly SharedTransformSystem _xform;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public override OverlaySpace Space => (OverlaySpace)512;

	public CivRoofCrackOverlay(IEntityManager ent)
	{
		_ent = ent;
		IoCManager.InjectDependencies<CivRoofCrackOverlay>(this);
		_lookup = ent.System<EntityLookupSystem>();
		_map = ent.System<SharedMapSystem>();
		_roof = ent.System<SharedRoofSystem>();
		_xform = ent.System<SharedTransformSystem>();
		((Overlay)this).ZIndex = -5;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		MapLightComponent light = default(MapLightComponent);
		if (args.Viewport.Eye == null || !_ent.TryGetComponent<MapLightComponent>(args.MapUid, ref light))
		{
			return;
		}
		float sun = GetSun(light);
		if (sun <= 0.02f)
		{
			return;
		}
		IClydeViewport viewport = args.Viewport;
		Box2Rotated bounds = args.WorldBounds;
		IRenderTexture target = viewport.LightRenderTarget;
		DrawingHandleWorld handle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		_grids.Clear();
		_maps.FindGridsIntersecting(args.MapId, bounds, ref _grids, true, true);
		if (_grids.Count == 0)
		{
			return;
		}
		Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
		Vector2 scale = viewport.RenderScale / (Vector2.One / vector);
		((DrawingHandleBase)handle).RenderInRenderTarget((IRenderTarget)(object)target, (Action)delegate
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			Matrix3x2 worldToLocalMatrix = ((IRenderTarget)target).GetWorldToLocalMatrix(viewport.Eye, scale);
			RoofComponent item = default(RoofComponent);
			CivRoofGridComponent civRoofGridComponent = default(CivRoofGridComponent);
			TileRef val = default(TileRef);
			foreach (Entity<MapGridComponent> grid in _grids)
			{
				if (_ent.TryGetComponent<RoofComponent>(grid.Owner, ref item) && _ent.TryGetComponent<CivRoofGridComponent>(grid.Owner, ref civRoofGridComponent) && civRoofGridComponent.Tiles.Count != 0)
				{
					Matrix3x2 matrix3x = Matrix3x2.Multiply(_xform.GetWorldMatrix(grid.Owner), worldToLocalMatrix);
					((DrawingHandleBase)handle).SetTransform(ref matrix3x);
					TilesEnumerator tilesEnumerator = _map.GetTilesEnumerator(grid.Owner, grid.Comp, bounds, true, (Predicate<TileRef>)null);
					while (((TilesEnumerator)(ref tilesEnumerator)).MoveNext(ref val))
					{
						if (civRoofGridComponent.Tiles.TryGetValue(val.GridIndices, out var value) && _roof.GetColor(Entity<MapGridComponent, RoofComponent>.op_Implicit((grid.Owner, grid.Comp, item)), val.GridIndices).HasValue)
						{
							Box2 localBounds = _lookup.GetLocalBounds(val, grid.Comp.TileSize);
							DrawLeaks(handle, localBounds, sun, value, Seed(grid.Owner, val.GridIndices));
						}
					}
				}
			}
		}, (Color?)null);
		DrawingHandleWorld obj = handle;
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)obj).SetTransform(ref identity);
	}

	private static void DrawLeaks(DrawingHandleWorld handle, Box2 box, float sun, CivRoofStage stage, uint seed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		Vector2 bottomLeft = box.BottomLeft;
		Vector2 size = ((Box2)(ref box)).Size;
		float num = bottomLeft.Y + size.Y;
		float num2 = 0.08f + sun * 0.18f;
		if (stage == CivRoofStage.Broken)
		{
			DrawRect(handle, box, box, LeakColor, num2 * 0.46f);
			float num3 = bottomLeft.X + size.X * (0.28f + Next(ref seed) * 0.44f);
			float width = size.X * (0.24f + Next(ref seed) * 0.2f);
			float num4 = size.Y * (0.12f + Next(ref seed) * 0.06f);
			DrawSource(handle, box, num3, num, width, num4, num2 * 2.05f);
			int num5 = 3 + (int)(Next(ref seed) * 2f);
			for (int i = 0; i < num5; i++)
			{
				float x = num3 + size.X * ((Next(ref seed) - 0.5f) * 0.22f);
				float len = size.Y * (0.5f + Next(ref seed) * 0.35f);
				float drift = size.X * ((Next(ref seed) - 0.5f) * 0.16f);
				float num6 = size.X * (0.05f + Next(ref seed) * 0.03f);
				float endW = num6 * (1.9f + Next(ref seed) * 1.2f);
				float alpha = num2 * (1.2f + Next(ref seed) * 0.6f);
				DrawBeam(handle, box, x, num - num4 * 0.35f, len, num6, endW, drift, alpha);
			}
			float x2 = num3 + size.X * ((Next(ref seed) - 0.5f) * 0.08f);
			DrawPool(handle, box, x2, bottomLeft.Y + size.Y * 0.22f, size.X * 0.64f, size.Y * 0.24f, num2 * 0.62f);
			return;
		}
		DrawRect(handle, box, box, LeakColor, num2 * 0.2f);
		int num7 = ((!(Next(ref seed) > 0.35f)) ? 1 : 2);
		for (int j = 0; j < num7; j++)
		{
			float num8 = bottomLeft.X + size.X * (0.22f + Next(ref seed) * 0.56f);
			float width2 = size.X * (0.15f + Next(ref seed) * 0.12f);
			float num9 = size.Y * (0.08f + Next(ref seed) * 0.05f);
			float alpha2 = num2 * (1.02f + Next(ref seed) * 0.55f);
			DrawSource(handle, box, num8, num, width2, num9, alpha2);
			int num10 = 2 + (int)(Next(ref seed) * 2f);
			for (int k = 0; k < num10; k++)
			{
				float x3 = num8 + size.X * ((Next(ref seed) - 0.5f) * 0.16f);
				float len2 = size.Y * (0.42f + Next(ref seed) * 0.3f);
				float drift2 = size.X * ((Next(ref seed) - 0.5f) * 0.12f);
				float num11 = size.X * (0.04f + Next(ref seed) * 0.03f);
				float endW2 = num11 * (2f + Next(ref seed) * 1f);
				float alpha3 = num2 * (1.08f + Next(ref seed) * 0.55f);
				DrawBeam(handle, box, x3, num - num9 * 0.3f, len2, num11, endW2, drift2, alpha3);
			}
		}
		float x4 = bottomLeft.X + size.X * (0.3f + Next(ref seed) * 0.4f);
		DrawPool(handle, box, x4, bottomLeft.Y + size.Y * 0.2f, size.X * 0.46f, size.Y * 0.18f, num2 * 0.34f);
	}

	private static void DrawSource(DrawingHandleWorld handle, Box2 clip, float x, float top, float width, float height, float alpha)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Box2 box = default(Box2);
		((Box2)(ref box))._002Ector(new Vector2(x - width * 1.15f, top - height * 1.8f), new Vector2(x + width * 1.15f, top + height * 0.08f));
		Box2 box2 = default(Box2);
		((Box2)(ref box2))._002Ector(new Vector2(x - width * 0.72f, top - height * 1.15f), new Vector2(x + width * 0.72f, top));
		Box2 box3 = default(Box2);
		((Box2)(ref box3))._002Ector(new Vector2(x - width * 0.36f, top - height * 0.72f), new Vector2(x + width * 0.36f, top));
		DrawRect(handle, box, clip, LeakColor, alpha * 0.24f);
		DrawRect(handle, box2, clip, LeakColor, alpha * 0.46f);
		DrawRect(handle, box3, clip, CoreColor, alpha * 0.7f);
	}

	private static void DrawBeam(DrawingHandleWorld handle, Box2 clip, float x, float top, float len, float startW, float endW, float drift, float alpha)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		Box2 box = default(Box2);
		Box2 box2 = default(Box2);
		for (int i = 0; i < 5; i++)
		{
			float num = (float)i / 5f;
			float num2 = ((float)i + 1f) / 5f;
			float y = top - len * num;
			float y2 = top - len * num2;
			float num3 = x + drift * num;
			float num4 = x + drift * num2;
			float num5 = MathHelper.Lerp(startW, endW, num);
			float num6 = MathHelper.Lerp(startW, endW, num2);
			((Box2)(ref box))._002Ector(new Vector2(MathF.Min(num3 - num5, num4 - num6), y2), new Vector2(MathF.Max(num3 + num5, num4 + num6), y));
			((Box2)(ref box2))._002Ector(new Vector2(MathF.Min(num3 - num5 * 0.34f, num4 - num6 * 0.34f), y2), new Vector2(MathF.Max(num3 + num5 * 0.34f, num4 + num6 * 0.34f), y));
			float num7 = 1f - num * 0.58f;
			DrawRect(handle, box, clip, LeakColor, alpha * 0.26f * num7);
			DrawRect(handle, box2, clip, CoreColor, alpha * 0.18f * num7);
		}
		float x2 = x + drift;
		float num8 = top - len;
		DrawPool(handle, clip, x2, num8 + len * 0.08f, endW * 2.5f, len * 0.2f, alpha * 0.28f);
	}

	private static void DrawPool(DrawingHandleWorld handle, Box2 clip, float x, float y, float width, float height, float alpha)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Box2 box = default(Box2);
		((Box2)(ref box))._002Ector(new Vector2(x - width, y - height), new Vector2(x + width, y + height));
		Box2 box2 = default(Box2);
		((Box2)(ref box2))._002Ector(new Vector2(x - width * 0.55f, y - height * 0.55f), new Vector2(x + width * 0.55f, y + height * 0.55f));
		DrawRect(handle, box, clip, LeakColor, alpha * 0.18f);
		DrawRect(handle, box2, clip, CoreColor, alpha * 0.16f);
	}

	private static void DrawRect(DrawingHandleWorld handle, Box2 box, Box2 clip, Color color, float alpha)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		float num = Math.Clamp(box.BottomLeft.X, clip.BottomLeft.X, clip.TopRight.X);
		float num2 = Math.Clamp(box.BottomLeft.Y, clip.BottomLeft.Y, clip.TopRight.Y);
		float num3 = Math.Clamp(box.TopRight.X, clip.BottomLeft.X, clip.TopRight.X);
		float num4 = Math.Clamp(box.TopRight.Y, clip.BottomLeft.Y, clip.TopRight.Y);
		if (!(num >= num3) && !(num2 >= num4) && !(alpha <= 0f))
		{
			handle.DrawRect(new Box2(new Vector2(num, num2), new Vector2(num3, num4)), ((Color)(ref color)).WithAlpha(Math.Clamp(alpha, 0f, 1f)), true);
		}
	}

	private static float GetSun(MapLightComponent light)
	{
		return Math.Clamp((light.AmbientLightColor.R + light.AmbientLightColor.G + light.AmbientLightColor.B) / 3f, 0f, 1f);
	}

	private unsafe static uint Seed(EntityUid grid, Vector2i tile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		int num = ((object)(*(EntityUid*)(&grid))/*cast due to constrained. prefix*/).GetHashCode() ^ (tile.X * 73856093) ^ (tile.Y * 19349663);
		int num2 = (num ^ (num >>> 16)) * 2146121005;
		int num3 = (num2 ^ (num2 >>> 15)) * -2073254261;
		return (uint)(num3 ^ (num3 >>> 16));
	}

	private static float Next(ref uint seed)
	{
		seed = seed * 1664525 + 1013904223;
		return (float)(seed & 0xFFFFFF) / 16777215f;
	}
}
