using Content.Shared.Eye.Blinding.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Eye.Blinding;

public sealed class BlindOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> GreyscaleShader = ProtoId<ShaderPrototype>.op_Implicit("GreyscaleFullscreen");

	private static readonly ProtoId<ShaderPrototype> CircleShader = ProtoId<ShaderPrototype>.op_Implicit("CircleMask");

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private ILightManager _lightManager;

	private readonly ShaderInstance _greyscaleShader;

	private readonly ShaderInstance _circleMaskShader;

	private BlindableComponent _blindableComponent;

	public override bool RequestScreenTexture => true;

	public override OverlaySpace Space => (OverlaySpace)4;

	public BlindOverlay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<BlindOverlay>(this);
		_greyscaleShader = _prototypeManager.Index<ShaderPrototype>(GreyscaleShader).InstanceUnique();
		_circleMaskShader = _prototypeManager.Index<ShaderPrototype>(CircleShader).InstanceUnique();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		IEntityManager entityManager = _entityManager;
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EyeComponent val = default(EyeComponent);
		if (!entityManager.TryGetComponent<EyeComponent>((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null), ref val))
		{
			return false;
		}
		if ((object)args.Viewport.Eye != val.Eye)
		{
			return false;
		}
		ICommonSession localSession2 = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val2 = ((localSession2 != null) ? localSession2.AttachedEntity : ((EntityUid?)null));
		if (!val2.HasValue)
		{
			return false;
		}
		BlindableComponent blindableComponent = default(BlindableComponent);
		if (!_entityManager.TryGetComponent<BlindableComponent>(val2, ref blindableComponent))
		{
			return false;
		}
		_blindableComponent = blindableComponent;
		bool isBlind = _blindableComponent.IsBlind;
		if (!isBlind && _blindableComponent.LightSetup)
		{
			_lightManager.Enabled = true;
			_blindableComponent.LightSetup = false;
			_blindableComponent.GraceFrame = true;
			return true;
		}
		return isBlind;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture == null)
		{
			return;
		}
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		if (!val.HasValue)
		{
			return;
		}
		if (!_blindableComponent.GraceFrame)
		{
			_blindableComponent.LightSetup = true;
			_lightManager.Enabled = false;
		}
		else
		{
			_blindableComponent.GraceFrame = false;
		}
		EyeComponent val2 = default(EyeComponent);
		if (_entityManager.TryGetComponent<EyeComponent>(val, ref val2))
		{
			ShaderInstance circleMaskShader = _circleMaskShader;
			if (circleMaskShader != null)
			{
				circleMaskShader.SetParameter("Zoom", val2.Zoom.X);
			}
		}
		ShaderInstance greyscaleShader = _greyscaleShader;
		if (greyscaleShader != null)
		{
			greyscaleShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Box2Rotated worldBounds = args.WorldBounds;
		((DrawingHandleBase)worldHandle).UseShader(_greyscaleShader);
		worldHandle.DrawRect(ref worldBounds, Color.White, true);
		((DrawingHandleBase)worldHandle).UseShader(_circleMaskShader);
		worldHandle.DrawRect(ref worldBounds, Color.White, true);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
