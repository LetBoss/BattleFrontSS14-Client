using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._PUBG;

public sealed class RedZoneOverlay : Overlay
{
	public bool BombActive;

	public Vector2 BombCenter;

	public float BombRadius;

	public override OverlaySpace Space => (OverlaySpace)4;

	public RedZoneOverlay()
	{
		IoCManager.InjectDependencies<RedZoneOverlay>(this);
		((Overlay)this).ZIndex = -5;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (BombActive && !(BombRadius <= 0f))
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			Vector2 bombCenter = BombCenter;
			float bombRadius = BombRadius;
			Color red = Color.Red;
			DrawFilledCircle(worldHandle, bombCenter, bombRadius, ((Color)(ref red)).WithAlpha(0.6f));
			DrawCircleOutline(worldHandle, BombCenter, BombRadius, Color.OrangeRed, 0.2f);
		}
	}

	private void DrawFilledCircle(DrawingHandleWorld handle, Vector2 center, float radius, Color color)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Vector2[] array = new Vector2[3];
		for (int i = 0; i < 32; i++)
		{
			float x = (float)i / 32f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 32f * MathF.PI * 2f;
			Vector2 vector = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
			Vector2 vector2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
			array[0] = center;
			array[1] = vector;
			array[2] = vector2;
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)array, color);
		}
	}

	private void DrawCircleOutline(DrawingHandleWorld handle, Vector2 center, float radius, Color color, float thickness)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 64; i++)
		{
			float x = (float)i / 64f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 64f * MathF.PI * 2f;
			Vector2 vector = new Vector2(center.X + MathF.Cos(x) * radius, center.Y + MathF.Sin(x) * radius);
			Vector2 vector2 = new Vector2(center.X + MathF.Cos(x2) * radius, center.Y + MathF.Sin(x2) * radius);
			((DrawingHandleBase)handle).DrawLine(vector, vector2, color);
		}
	}
}
