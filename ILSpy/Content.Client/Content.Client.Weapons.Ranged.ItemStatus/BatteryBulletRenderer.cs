using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Maths;

namespace Content.Client.Weapons.Ranged.ItemStatus;

public sealed class BatteryBulletRenderer : BaseBulletRenderer
{
	private static readonly Color ItemColor = Color.FromHex((ReadOnlySpan<char>)"#E00000", (Color?)null);

	private static readonly Color ItemColorGone = Color.Black;

	private const int SizeH = 10;

	private const int SizeV = 10;

	private const int Separation = 4;

	public BatteryBulletRenderer()
	{
		base.Parameters = new LayoutParameters
		{
			ItemWidth = 10,
			ItemHeight = 10,
			ItemSeparation = 14,
			MinCountPerRow = 3,
			VerticalSeparation = 4
		};
	}

	protected override void DrawItem(DrawingHandleScreen handle, Vector2 renderPos, bool spent, bool altColor)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Color val = (spent ? ItemColorGone : ItemColor);
		handle.DrawRect(UIBox2.FromDimensions(renderPos, new Vector2(10f, 10f)), val, true);
	}
}
