using System;
using System.Numerics;
using Content.Shared._CIV14merka.Weapons;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._CIV14merka.Weapons;

public sealed class CivSuppressionOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> SuppressionShader = ProtoId<ShaderPrototype>.op_Implicit("CivSuppression");

	private static readonly ProtoId<ShaderPrototype> VignetteShader = ProtoId<ShaderPrototype>.op_Implicit("GradientCircleMask");

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly CivSuppressionSystem _system;

	private readonly ShaderInstance _suppressionShader;

	private readonly ShaderInstance _vignetteShader;

	private float _strength;

	private float _stress;

	private float _pulse;

	private CivSuppressionVisualProfile _profile;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public CivSuppressionOverlay(CivSuppressionSystem system)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<CivSuppressionOverlay>(this);
		_system = system;
		_suppressionShader = _prototypeManager.Index<ShaderPrototype>(SuppressionShader).InstanceUnique();
		_vignetteShader = _prototypeManager.Index<ShaderPrototype>(VignetteShader).InstanceUnique();
		((Overlay)this).ZIndex = 500;
	}

	protected override void DisposeBehavior()
	{
		((Overlay)this).DisposeBehavior();
		_suppressionShader.Dispose();
		_vignetteShader.Dispose();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		_strength = _system.CurrentIntensity;
		_stress = _system.Stress;
		_pulse = _system.Pulse;
		_profile = _system.VisualProfile;
		return _system.Active;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		Box2 worldAABB = args.WorldAABB;
		if (base.ScreenTexture != null)
		{
			_suppressionShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			_suppressionShader.SetParameter("strength", _strength);
			_suppressionShader.SetParameter("stress", _stress);
			_suppressionShader.SetParameter("pulse", _pulse);
			_suppressionShader.SetParameter("profile", (float)(int)_profile);
			((DrawingHandleBase)worldHandle).UseShader(_suppressionShader);
			worldHandle.DrawRect(worldAABB, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
		float num;
		Color val = default(Color);
		float num2;
		float num3;
		float num4;
		float num5;
		Vector3 vector;
		switch (_profile)
		{
		case CivSuppressionVisualProfile.Explosion:
			num = Math.Clamp(_pulse * 0.14f + _strength * 0.1f + _stress * 0.12f, 0f, 0.32f);
			((Color)(ref val))._002Ector((byte)118, (byte)120, (byte)124, (byte)(num * 255f));
			num2 = Math.Clamp(0.86f - _pulse * 0.1f - _strength * 0.06f, 0.58f, 0.92f);
			num3 = Math.Clamp(0.984f - _pulse * 0.02f - _stress * 0.018f, 0.9f, 1f);
			num4 = Math.Clamp(_pulse * 0.22f + _strength * 0.14f + _stress * 0.12f, 0f, 0.52f);
			num5 = Math.Clamp(_pulse * 0.03f + _strength * 0.02f, 0f, 0.08f);
			vector = new Vector3(0.02f, 0.021f, 0.023f);
			break;
		case CivSuppressionVisualProfile.Mortar:
			num = Math.Clamp(_stress * 0.24f + _strength * 0.14f + _pulse * 0.04f, 0f, 0.36f);
			((Color)(ref val))._002Ector((byte)112, (byte)106, (byte)98, (byte)(num * 255f));
			num2 = Math.Clamp(0.74f - _stress * 0.2f - _strength * 0.08f - _pulse * 0.02f, 0.42f, 0.82f);
			num3 = Math.Clamp(0.958f - _stress * 0.05f - _pulse * 0.016f, 0.82f, 0.99f);
			num4 = Math.Clamp(_stress * 0.74f + _strength * 0.14f + _pulse * 0.06f, 0f, 0.84f);
			num5 = Math.Clamp(_stress * 0.08f + _strength * 0.025f, 0f, 0.14f);
			vector = new Vector3(0.021f, 0.018f, 0.014f);
			break;
		default:
			num = Math.Clamp(_stress * 0.34f + _strength * 0.12f + _pulse * 0.03f, 0f, 0.5f);
			((Color)(ref val))._002Ector((byte)80, (byte)85, (byte)92, (byte)(num * 255f));
			num2 = Math.Clamp(0.76f - _stress * 0.28f - _strength * 0.18f - _pulse * 0.05f, 0.3f, 0.82f);
			num3 = Math.Clamp(0.972f - _stress * 0.06f - _strength * 0.04f, 0.8f, 1f);
			num4 = Math.Clamp(_stress * 0.9f + _strength * 0.22f + _pulse * 0.06f, 0f, 0.995f);
			num5 = Math.Clamp(_stress * 0.14f + _strength * 0.04f, 0f, 0.22f);
			vector = new Vector3(0.005f, 0.006f, 0.008f);
			break;
		}
		if (num > 0.001f)
		{
			worldHandle.DrawRect(worldAABB, val, true);
		}
		float num6 = MathF.Max(((UIBox2i)(ref args.ViewportBounds)).Width, ((UIBox2i)(ref args.ViewportBounds)).Height);
		float num7 = num2 * num6 * 0.5f;
		float num8 = num3 * num6 * 0.5f;
		_vignetteShader.SetParameter("color", vector);
		_vignetteShader.SetParameter("darknessAlphaOuter", num4);
		_vignetteShader.SetParameter("darknessAlphaInner", num5);
		_vignetteShader.SetParameter("innerCircleRadius", num7);
		_vignetteShader.SetParameter("innerCircleMaxRadius", num7);
		_vignetteShader.SetParameter("outerCircleRadius", num8);
		_vignetteShader.SetParameter("outerCircleMaxRadius", num8);
		((DrawingHandleBase)worldHandle).UseShader(_vignetteShader);
		worldHandle.DrawRect(worldAABB, Color.White, true);
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
