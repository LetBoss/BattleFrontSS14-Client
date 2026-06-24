using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Light;

public sealed class AfterLightTargetOverlay : Overlay
{
	[Dependency]
	private IOverlayManager _overlay;

	public const int ContentZIndex = -6;

	public override OverlaySpace Space => (OverlaySpace)512;

	public AfterLightTargetOverlay()
	{
		IoCManager.InjectDependencies<AfterLightTargetOverlay>(this);
		((Overlay)this).ZIndex = -6;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		IClydeViewport viewport = args.Viewport;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		if (viewport.Eye != null)
		{
			BeforeLightTargetOverlay lightOverlay = _overlay.GetOverlay<BeforeLightTargetOverlay>();
			Box2Rotated bounds = args.WorldBounds;
			Vector2 vector = Vector2i.op_Implicit(((IRenderTarget)viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
			Vector2 vector2 = viewport.RenderScale / (Vector2.One / vector);
			Matrix3x2 localMatrix = ((IRenderTarget)viewport.LightRenderTarget).GetWorldToLocalMatrix(viewport.Eye, vector2);
			Vector2i val = ((IRenderTarget)lightOverlay.EnlargedLightTarget).Size - ((IRenderTarget)viewport.LightRenderTarget).Size;
			Vector2i halfDiff = val / 2;
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).RenderInRenderTarget((IRenderTarget)(object)viewport.LightRenderTarget, (Action)delegate
			{
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				UIBox2i val2 = default(UIBox2i);
				((UIBox2i)(ref val2))._002Ector(halfDiff.X, halfDiff.Y, ((IRenderTarget)viewport.LightRenderTarget).Size.X + halfDiff.X, ((IRenderTarget)viewport.LightRenderTarget).Size.Y + halfDiff.Y);
				((DrawingHandleBase)worldHandle).SetTransform(ref localMatrix);
				DrawingHandleWorld obj = worldHandle;
				Texture texture = lightOverlay.EnlargedLightTarget.Texture;
				ref Box2Rotated reference = ref bounds;
				UIBox2? val3 = UIBox2i.op_Implicit(val2);
				obj.DrawTextureRectRegion(texture, ref reference, (Color?)null, val3);
			}, (Color?)Color.Transparent);
		}
	}
}
