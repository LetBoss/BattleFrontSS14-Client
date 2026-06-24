using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Light;

public sealed class LightBlurOverlay : Overlay
{
	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IOverlayManager _overlay;

	public const int ContentZIndex = -7;

	private IRenderTarget? _blurTarget;

	public override OverlaySpace Space => (OverlaySpace)512;

	public LightBlurOverlay()
	{
		IoCManager.InjectDependencies<LightBlurOverlay>(this);
		((Overlay)this).ZIndex = -7;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (args.Viewport.Eye != null)
		{
			BeforeLightTargetOverlay overlay = _overlay.GetOverlay<BeforeLightTargetOverlay>();
			Vector2i size = ((IRenderTarget)overlay.EnlargedLightTarget).Size;
			IRenderTarget? blurTarget = _blurTarget;
			if (blurTarget == null || blurTarget.Size != size)
			{
				_blurTarget = (IRenderTarget?)(object)_clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "enlarged-light-blur");
			}
			IRenderTexture enlargedLightTarget = overlay.EnlargedLightTarget;
			_clyde.BlurRenderTarget(args.Viewport, (IRenderTarget)(object)enlargedLightTarget, _blurTarget, args.Viewport.Eye, 70f);
		}
	}
}
