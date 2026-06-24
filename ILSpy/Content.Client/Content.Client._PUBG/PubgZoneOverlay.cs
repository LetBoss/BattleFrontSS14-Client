using System;
using System.Numerics;
using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._PUBG;

public sealed class PubgZoneOverlay : Overlay
{
	private readonly IResourceCache _resourceCache;

	public Vector2? CurrentCenter;

	public float CurrentRadius;

	public Vector2? NextCenter;

	public float NextRadius;

	public ZoneState State;

	public bool Active;

	public bool Visible;

	public MapId ZoneMapId;

	public override OverlaySpace Space => (OverlaySpace)4;

	public PubgZoneOverlay(IResourceCache resourceCache)
	{
		IoCManager.InjectDependencies<PubgZoneOverlay>(this);
		_resourceCache = resourceCache;
		((Overlay)this).ZIndex = -10;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if (Active && Visible && CurrentCenter.HasValue && !(args.MapId != ZoneMapId))
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			Box2 viewport = ((Box2Rotated)(ref args.WorldBounds)).CalcBoundingBox();
			DrawFogOutsideZone(worldHandle, viewport, CurrentCenter.Value, CurrentRadius);
			if (NextCenter.HasValue)
			{
				DrawZoneCircle(worldHandle, NextCenter.Value, NextRadius, Color.White, 0.5f);
			}
			Color color = ((State == ZoneState.Waiting) ? Color.CornflowerBlue : Color.OrangeRed);
			float alpha = ((State == ZoneState.Waiting) ? 0.7f : 1f);
			float thickness = ((State == ZoneState.Waiting) ? 0.3f : 0.5f);
			DrawZoneCircle(worldHandle, CurrentCenter.Value, CurrentRadius, color, alpha, thickness);
		}
	}

	private void DrawFogOutsideZone(DrawingHandleWorld handle, Box2 viewport, Vector2 center, float radius)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		((Color)(ref val))._002Ector((byte)80, (byte)60, (byte)60, (byte)180);
		((Box2)(ref viewport)).Enlarged(radius * 2f);
		Vector2[] array = new Vector2[6];
		for (int i = 0; i < 64; i++)
		{
			float x = (float)i / 64f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 64f * MathF.PI * 2f;
			Vector2 vector = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
			Vector2 vector2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
			float num = radius + Math.Max(((Box2)(ref viewport)).Width, ((Box2)(ref viewport)).Height);
			Vector2 vector3 = new Vector2(center.X + MathF.Cos(x) * num, center.Y + MathF.Sin(x) * num);
			Vector2 vector4 = new Vector2(center.X + MathF.Cos(x2) * num, center.Y + MathF.Sin(x2) * num);
			array[0] = vector;
			array[1] = vector2;
			array[2] = vector3;
			array[3] = vector2;
			array[4] = vector4;
			array[5] = vector3;
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)array, val);
		}
	}

	private void DrawZoneCircle(DrawingHandleWorld handle, Vector2 center, float radius, Color color, float alpha, float thickness = 0.3f)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 128; i++)
		{
			float x = (float)i / 128f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 128f * MathF.PI * 2f;
			Vector2 vector = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
			Vector2 vector2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
			((DrawingHandleBase)handle).DrawLine(vector, vector2, ((Color)(ref color)).WithAlpha(alpha));
		}
	}
}
