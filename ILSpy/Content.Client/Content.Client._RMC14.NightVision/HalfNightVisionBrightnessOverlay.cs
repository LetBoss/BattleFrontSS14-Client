using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.NightVision;

public sealed class HalfNightVisionBrightnessOverlay : Overlay
{
	public override OverlaySpace Space => (OverlaySpace)512;

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleBase drawingHandle = args.DrawingHandle;
		DrawingHandleWorld val = (DrawingHandleWorld)(object)((drawingHandle is DrawingHandleWorld) ? drawingHandle : null);
		if (val != null)
		{
			Box2 worldAABB = args.WorldAABB;
			Color val2 = default(Color);
			((Color)(ref val2))._002Ector(0.45f, 0.45f, 0.45f, 1f);
			val.DrawRect(worldAABB, val2, true);
		}
	}
}
