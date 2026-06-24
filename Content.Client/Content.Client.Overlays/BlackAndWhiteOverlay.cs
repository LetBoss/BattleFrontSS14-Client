using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class BlackAndWhiteOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("GreyscaleFullscreen");

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly ShaderInstance _greyscaleShader;

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => true;

	public BlackAndWhiteOverlay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<BlackAndWhiteOverlay>(this);
		_greyscaleShader = _prototypeManager.Index<ShaderPrototype>(Shader).InstanceUnique();
		((Overlay)this).ZIndex = 10;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (base.ScreenTexture != null)
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			_greyscaleShader.SetParameter("SCREEN_TEXTURE", base.ScreenTexture);
			((DrawingHandleBase)worldHandle).UseShader(_greyscaleShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}
}
