using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.HeliSupply;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliRouteOverlay : Overlay
{
	private readonly CivHeliSupplySystem _system;

	private const float LineThickness = 0.3f;

	private const float PointRadius = 0.25f;

	private static readonly Color RouteColor = Color.FromHex((ReadOnlySpan<char>)"#FFD23F", (Color?)null);

	private static readonly Color DropColor = Color.FromHex((ReadOnlySpan<char>)"#E63946", (Color?)null);

	public override OverlaySpace Space => (OverlaySpace)8;

	public CivHeliRouteOverlay(CivHeliSupplySystem system)
	{
		_system = system;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		return _system.IsRouteMode;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<Vector2> routePoints = _system.RoutePoints;
		if (routePoints.Count > 0 && _system.RouteMapId != args.MapId)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		CivHeliStateMessage lastState = _system.LastState;
		if (lastState != null && lastState.HasOrigin && lastState.OriginMapId == args.MapId)
		{
			Vector2 origin = lastState.Origin;
			((DrawingHandleBase)worldHandle).DrawCircle(origin, 0.325f, ((Color)(ref RouteColor)).WithAlpha(0.7f), true);
			if (routePoints.Count > 0)
			{
				DrawThickLine(worldHandle, origin, routePoints[0], 0.24000001f, ((Color)(ref RouteColor)).WithAlpha(0.6f));
			}
			else if (_system.GetCursorMapId() == args.MapId)
			{
				DrawThickLine(worldHandle, origin, _system.GetCursorWorldPosition(), 0.18f, ((Color)(ref RouteColor)).WithAlpha(0.35f));
			}
		}
		for (int i = 0; i < routePoints.Count - 1; i++)
		{
			DrawThickLine(worldHandle, routePoints[i], routePoints[i + 1], 0.3f, ((Color)(ref RouteColor)).WithAlpha(0.8f));
		}
		for (int j = 0; j < routePoints.Count; j++)
		{
			bool flag = j == routePoints.Count - 1;
			((DrawingHandleBase)worldHandle).DrawCircle(routePoints[j], flag ? 0.4f : 0.25f, flag ? DropColor : RouteColor, true);
		}
		if (routePoints.Count > 0 && _system.GetCursorMapId() == args.MapId)
		{
			Vector2 cursorWorldPosition = _system.GetCursorWorldPosition();
			DrawThickLine(worldHandle, routePoints[routePoints.Count - 1], cursorWorldPosition, 0.21000001f, ((Color)(ref RouteColor)).WithAlpha(0.45f));
			((DrawingHandleBase)worldHandle).DrawCircle(cursorWorldPosition, 0.2f, ((Color)(ref RouteColor)).WithAlpha(0.6f), true);
		}
	}

	private static void DrawThickLine(DrawingHandleWorld handle, Vector2 a, Vector2 b, float thickness, Color color)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = b - a;
		float num = vector.Length();
		if (!(num < 0.001f))
		{
			Vector2 vector2 = vector / num;
			Vector2 vector3 = new Vector2(0f - vector2.Y, vector2.X);
			float num2 = thickness * 0.5f;
			((DrawingHandleBase)handle).DrawLine(a, b, color);
			int num3 = (int)MathF.Ceiling(num2 / 0.04f);
			for (int i = 1; i <= num3; i++)
			{
				float num4 = MathF.Min(num2, (float)i * 0.04f);
				Vector2 vector4 = vector3 * num4;
				((DrawingHandleBase)handle).DrawLine(a + vector4, b + vector4, color);
				((DrawingHandleBase)handle).DrawLine(a - vector4, b - vector4, color);
			}
		}
	}
}
