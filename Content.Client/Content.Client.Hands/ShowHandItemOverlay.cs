using System;
using System.Numerics;
using Content.Client.Hands.Systems;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Hands;

public sealed class ShowHandItemOverlay : Overlay
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IEntityManager _entMan;

	private HandsSystem? _hands;

	private readonly IRenderTexture _renderBackbuffer;

	public Texture? IconOverride;

	public EntityUid? EntityOverride;

	public override OverlaySpace Space => (OverlaySpace)2;

	public ShowHandItemOverlay()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<ShowHandItemOverlay>(this);
		IClyde clyde = _clyde;
		Vector2i val = Vector2i.op_Implicit((64, 64));
		RenderTargetFormatParameters val2 = new RenderTargetFormatParameters((RenderTargetColorFormat)1, true);
		TextureSampleParameters value = default(TextureSampleParameters);
		((TextureSampleParameters)(ref value)).Filter = true;
		_renderBackbuffer = clyde.CreateRenderTarget(val, val2, (TextureSampleParameters?)value, "ShowHandItemOverlay");
	}

	protected override void DisposeBehavior()
	{
		((Overlay)this).DisposeBehavior();
		((IDisposable)_renderBackbuffer).Dispose();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		if (!_cfg.GetCVar<bool>(CCVars.HudHeldItemShow))
		{
			return false;
		}
		return ((Overlay)this).BeforeDraw(ref args);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (mouseScreenPosition.Window == WindowId.Invalid)
		{
			return;
		}
		DrawingHandleScreen screen = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		float cVar = _cfg.GetCVar<float>(CCVars.HudHeldItemOffset);
		Vector2 vector = new Vector2(cVar, cVar);
		Color white;
		if (IconOverride != null)
		{
			DrawingHandleScreen obj = screen;
			Texture? iconOverride = IconOverride;
			Vector2 vector2 = mouseScreenPosition.Position - Vector2i.op_Implicit(IconOverride.Size / 2) + vector;
			white = Color.White;
			((DrawingHandleBase)obj).DrawTexture(iconOverride, vector2, (Color?)((Color)(ref white)).WithAlpha(0.75f));
			return;
		}
		if (_hands == null)
		{
			_hands = _entMan.System<HandsSystem>();
		}
		EntityUid? handEntity = _hands.GetActiveHandEntity();
		SpriteComponent sprite = default(SpriteComponent);
		if (handEntity.HasValue && _entMan.TryGetComponent<SpriteComponent>(handEntity, ref sprite))
		{
			Vector2i halfSize = ((IRenderTarget)_renderBackbuffer).Size / 2;
			IViewportControl viewportControl = args.ViewportControl;
			IViewportControl obj2 = ((viewportControl is Control) ? viewportControl : null);
			float uiScale = ((obj2 != null) ? ((Control)obj2).UIScale : 1f);
			((DrawingHandleBase)screen).RenderInRenderTarget((IRenderTarget)(object)_renderBackbuffer, (Action)delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				screen.DrawEntity(handEntity.Value, Vector2i.op_Implicit(halfSize), new Vector2(1f, 1f) * uiScale, (Angle?)Angle.Zero, Angle.Zero, (Direction?)(Direction)0, sprite, (TransformComponent)null, (SharedTransformSystem)null);
			}, (Color?)Color.Transparent);
			DrawingHandleScreen obj3 = screen;
			Texture texture = _renderBackbuffer.Texture;
			Vector2 vector3 = mouseScreenPosition.Position - Vector2i.op_Implicit(halfSize) + vector;
			white = Color.White;
			((DrawingHandleBase)obj3).DrawTexture(texture, vector3, (Color?)((Color)(ref white)).WithAlpha(0.75f));
		}
	}
}
