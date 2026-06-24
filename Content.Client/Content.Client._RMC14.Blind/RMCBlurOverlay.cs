using Content.Shared._RMC14.BlurredVision;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Blind;

public sealed class RMCBlurOverlay : Overlay
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEntityManager _entityManager;

	private readonly ShaderInstance _blurShader;

	private const float BlurAmount = 0.01f;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public RMCBlurOverlay(IEntityManager entManager)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<RMCBlurOverlay>(this);
		_blurShader = _prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCBlurryVisionX")).InstanceUnique();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		EyeComponent val = default(EyeComponent);
		if (!_entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager)_playerManager).LocalEntity, ref val))
		{
			return false;
		}
		if (!_entityManager.HasComponent<RMCBlindedComponent>(((ISharedPlayerManager)_playerManager).LocalEntity))
		{
			return false;
		}
		return (object)args.Viewport.Eye == val.Eye;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null && ((ISharedPlayerManager)_playerManager).LocalEntity.HasValue)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_blurShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			_blurShader.SetParameter("BLUR_AMOUNT", 0.01f);
			((DrawingHandleBase)worldHandle).UseShader(_blurShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
