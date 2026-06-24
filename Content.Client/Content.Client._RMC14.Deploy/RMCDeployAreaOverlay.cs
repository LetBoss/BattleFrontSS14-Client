using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Deploy;

public sealed class RMCDeployAreaOverlay : Overlay
{
	public override OverlaySpace Space => (OverlaySpace)4;

	public Box2 Box { get; set; }

	public Color Color { get; set; }

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Color color = Color;
		Color val = ((Color)(ref color)).WithAlpha(0.5f);
		((OverlayDrawArgs)(ref args)).WorldHandle.DrawRect(Box, val, true);
	}
}
