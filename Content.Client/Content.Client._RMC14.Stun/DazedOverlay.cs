using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Stun;

public sealed class DazedOverlay : Overlay
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly ShaderInstance _vignetteShader;

	private float _outerFadeStart;

	private float _outerFadeEnd = 0.8f;

	private float _alpha = 1f;

	public override OverlaySpace Space => (OverlaySpace)4;

	public bool IsEnabled { get; set; }

	public DazedOverlay()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DazedOverlay>(this);
		_vignetteShader = _prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("GradientCircleMask")).InstanceUnique();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnabled)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			Box2 worldAABB = args.WorldAABB;
			int width = ((UIBox2i)(ref args.ViewportBounds)).Width;
			_vignetteShader.SetParameter("color", new Vector3(0f, 0f, 0f));
			_vignetteShader.SetParameter("darknessAlphaOuter", _alpha);
			_vignetteShader.SetParameter("darknessAlphaInner", 0f);
			_vignetteShader.SetParameter("innerCircleRadius", _outerFadeStart * (float)width * 0.5f);
			_vignetteShader.SetParameter("innerCircleMaxRadius", _outerFadeStart * (float)width * 0.5f);
			_vignetteShader.SetParameter("outerCircleRadius", _outerFadeEnd * (float)width * 0.5f);
			_vignetteShader.SetParameter("outerCircleMaxRadius", _outerFadeEnd * (float)width * 0.5f);
			((DrawingHandleBase)worldHandle).UseShader(_vignetteShader);
			worldHandle.DrawRect(worldAABB, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
