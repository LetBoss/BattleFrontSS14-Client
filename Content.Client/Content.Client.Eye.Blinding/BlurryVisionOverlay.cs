using System;
using Content.Shared.CCVar;
using Content.Shared.Eye.Blinding.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Eye.Blinding;

public sealed class BlurryVisionOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> CataractsShader = ProtoId<ShaderPrototype>.op_Implicit("Cataracts");

	private static readonly ProtoId<ShaderPrototype> CircleShader = ProtoId<ShaderPrototype>.op_Implicit("CircleMask");

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _configManager;

	private readonly ShaderInstance _cataractsShader;

	private readonly ShaderInstance _circleMaskShader;

	private float _magnitude;

	private float _correctionPower = 2f;

	private const float Distortion_Pow = 2f;

	private const float Cloudiness_Pow = 1f;

	private const float NoMotion_Radius = 30f;

	private const float NoMotion_Pow = 0.2f;

	private const float NoMotion_Max = 8f;

	private const float NoMotion_Mult = 0.75f;

	public override bool RequestScreenTexture => true;

	public override OverlaySpace Space => (OverlaySpace)4;

	public BlurryVisionOverlay()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<BlurryVisionOverlay>(this);
		_cataractsShader = _prototypeManager.Index<ShaderPrototype>(CataractsShader).InstanceUnique();
		_circleMaskShader = _prototypeManager.Index<ShaderPrototype>(CircleShader).InstanceUnique();
		_circleMaskShader.SetParameter("CircleMinDist", 0f);
		_circleMaskShader.SetParameter("CirclePow", 0.2f);
		_circleMaskShader.SetParameter("CircleMax", 8f);
		_circleMaskShader.SetParameter("CircleMult", 0.75f);
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
		BlurryVisionComponent blurryVisionComponent = default(BlurryVisionComponent);
		if (!_entityManager.TryGetComponent<BlurryVisionComponent>(val2, ref blurryVisionComponent))
		{
			return false;
		}
		if (blurryVisionComponent.Magnitude <= 0f)
		{
			return false;
		}
		BlindableComponent blindableComponent = default(BlindableComponent);
		if (_entityManager.TryGetComponent<BlindableComponent>(val2, ref blindableComponent) && blindableComponent.IsBlind)
		{
			return false;
		}
		_magnitude = blurryVisionComponent.Magnitude;
		_correctionPower = blurryVisionComponent.CorrectionPower;
		return true;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null)
		{
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			Box2Rotated worldBounds = args.WorldBounds;
			float num = (float)Math.Pow(Math.Min(_magnitude / 6f, 1f), _correctionPower);
			float num2 = 1f;
			EyeComponent val2 = default(EyeComponent);
			if (_entityManager.TryGetComponent<EyeComponent>(val, ref val2))
			{
				num2 = val2.Zoom.X;
			}
			if (_configManager.GetCVar<bool>(CCVars.ReducedMotion))
			{
				_circleMaskShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
				_circleMaskShader.SetParameter("Zoom", num2);
				_circleMaskShader.SetParameter("CircleRadius", 30f / num);
				((DrawingHandleBase)worldHandle).UseShader(_circleMaskShader);
				worldHandle.DrawRect(ref worldBounds, Color.White, true);
				((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
			}
			else
			{
				_cataractsShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
				_cataractsShader.SetParameter("LIGHT_TEXTURE", args.Viewport.LightRenderTarget.Texture);
				_cataractsShader.SetParameter("Zoom", num2);
				_cataractsShader.SetParameter("DistortionScalar", (float)Math.Pow(num, 2.0));
				_cataractsShader.SetParameter("CloudinessScalar", (float)Math.Pow(num, 1.0));
				((DrawingHandleBase)worldHandle).UseShader(_cataractsShader);
				worldHandle.DrawRect(ref worldBounds, Color.White, true);
				((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
			}
		}
	}
}
