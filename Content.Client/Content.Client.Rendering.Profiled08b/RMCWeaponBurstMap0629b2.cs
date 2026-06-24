using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCWeaponBurstMap0629b2 : Overlay
{
	private readonly RMCProfileCacheNodea4fdbc _f25087a37569a;

	public override OverlaySpace Space => (OverlaySpace)2;

	public RMCWeaponBurstMap0629b2(RMCProfileCacheNodea4fdbc weapon)
	{
		_f25087a37569a = weapon;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		RMCDrawSkewSlice91dac4 challenge;
		return _f25087a37569a._m49a1ad791f32(out challenge);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		if (_f25087a37569a._m49a1ad791f32(out var challenge))
		{
			int num = Math.Max(1, args.ViewportBounds.Right - args.ViewportBounds.Left);
			int num2 = Math.Max(1, args.ViewportBounds.Bottom - args.ViewportBounds.Top);
			int num3 = Math.Max(1, (int)challenge.Grid);
			float num4 = MathF.Max(10f, MathF.Min(num, num2) * (float)(int)challenge.SizePercent / 100f);
			float num5 = (float)num / (float)num3;
			float num6 = (float)num2 / (float)num3;
			float num7 = (float)args.ViewportBounds.Left + num5 * ((float)(int)challenge.CellX + 0.5f);
			float num8 = (float)args.ViewportBounds.Top + num6 * ((float)(int)challenge.CellY + 0.5f);
			Vector2 vector = new Vector2(num7 - num4 * 0.55f, num8 - num4 * 0.55f);
			Vector2 vector2 = new Vector2(num7 - num4 * 0.4f, num8 - num4 * 0.4f);
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawRect(UIBox2.FromDimensions(vector, new Vector2(num4 * 1.1f, num4 * 1.1f)), Color.Black, true);
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawRect(UIBox2.FromDimensions(vector2, new Vector2(num4 * 0.8f, num4 * 0.8f)), challenge.Color, true);
		}
	}
}
