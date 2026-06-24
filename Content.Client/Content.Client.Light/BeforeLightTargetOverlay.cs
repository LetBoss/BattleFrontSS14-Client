using System;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Light;

public sealed class BeforeLightTargetOverlay : Overlay
{
	[Dependency]
	private IClyde _clyde;

	public IRenderTexture EnlargedLightTarget;

	public Box2Rotated EnlargedBounds;

	private float _skirting = 2f;

	public const int ContentZIndex = -10;

	public override OverlaySpace Space => (OverlaySpace)512;

	public BeforeLightTargetOverlay()
	{
		IoCManager.InjectDependencies<BeforeLightTargetOverlay>(this);
		((Overlay)this).ZIndex = -10;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = ((IRenderTarget)args.Viewport.LightRenderTarget).Size + (int)(_skirting * 32f);
		EnlargedBounds = ((Box2Rotated)(ref args.WorldBounds)).Enlarged(_skirting / 2f);
		IRenderTexture enlargedLightTarget = EnlargedLightTarget;
		if (enlargedLightTarget == null || ((IRenderTarget)enlargedLightTarget).Size != val)
		{
			EnlargedLightTarget = _clyde.CreateRenderTarget(val, new RenderTargetFormatParameters((RenderTargetColorFormat)1, false), (TextureSampleParameters?)null, "enlarged-light-copy");
		}
		((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)EnlargedLightTarget, (Action)delegate
		{
		}, (Color?)_clyde.GetClearColor(args.MapUid));
	}
}
