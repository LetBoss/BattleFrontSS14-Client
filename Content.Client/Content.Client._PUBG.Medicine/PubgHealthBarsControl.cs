using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Medicine;

public sealed class PubgHealthBarsControl : Control
{
	[Dependency]
	private readonly IOverlayManager _overlays;

	private const float BarHeight = 30f;

	private const float ResourceBarHeight = 18f;

	private const float Padding = 10f;

	public PubgHealthBarsControl()
	{
		IoCManager.InjectDependencies<PubgHealthBarsControl>(this);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Draw(handle);
		PubgHealthOverlay pubgHealthOverlay = default(PubgHealthOverlay);
		if (_overlays.TryGetOverlay<PubgHealthOverlay>(ref pubgHealthOverlay) && pubgHealthOverlay != null)
		{
			float barWidth = MathF.Max(120f, (float)((Control)this).PixelSize.X - 20f);
			float barY = (float)((Control)this).PixelSize.Y - 30f - 10f;
			pubgHealthOverlay.DrawBars(handle, 10f, barY, barWidth, 30f, 18f, stackTempBar: true);
		}
	}
}
